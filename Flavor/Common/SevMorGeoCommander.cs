using System;
using System.Collections.Generic;
using Flavor.Common.Messaging;
using Flavor.Common.Messaging.SevMorGeo;
using Flavor.Common.Library;
using Flavor.Common.Data.Measure;
using Flavor.Common.Settings;

namespace Flavor.Common {
    class SevMorGeoCommader: Commander {
        readonly EventHandler<EventArgs<uint[]>> deviceCountsUpdated;
        SevMorGeoRealizer realizer;
        readonly EventHandler<EventArgs<Action>> onTheFlyAction;
        public SevMorGeoCommader()
            // TODO: proper device
            : base(new PortLevel(), null) {
            deviceCountsUpdated = (s, e) => {
                CurrentMeasureMode.UpdateGraph();
                if (!CurrentMeasureMode.onUpdateCounts(Device.Detectors)) {
                    OnErrorOccured("Измеряемая точка вышла за пределы допустимого диапазона.\nРежим измерения прекращен.");
                }
            };
            realizer.SystemDown += (s, e) => {
                if (e.Value) {
                    if (pState != ProgramStates.Start) {
                        setProgramStateWithoutUndo(ProgramStates.Start);
                        MeasureCancelRequested = false;
                    }
                } else {
                    OnLog("System is shutdowned");
                    setProgramStateWithoutUndo(ProgramStates.Start);
                    hBlock = true;
                    OnLog(pState.ToString());
                    Device.Init();
                }
            };
            realizer.OperationToggle += (s, e) => {
                if (e.Value) {
                    OnLog("Init request confirmed");
                    setProgramStateWithoutUndo(ProgramStates.Init);
                } else {
                    OnLog("Shutdown request confirmed");
                    setProgramStateWithoutUndo(ProgramStates.Shutdown);
                }
                OnLog(pState.ToString());
            };
            onTheFlyAction = (s, e) => {
                (s as IRealizer).FirstStatus -= onTheFlyAction;
                switch (Device.sysState) {
                    case Device.DeviceStates.Init:
                    case Device.DeviceStates.VacuumInit:
                        hBlock = true;
                        setProgramStateWithoutUndo(ProgramStates.Init);
                        break;

                    case Device.DeviceStates.ShutdownInit:
                    case Device.DeviceStates.Shutdowning:
                        hBlock = true;
                        setProgramStateWithoutUndo(ProgramStates.Shutdown);
                        break;

                    case Device.DeviceStates.Measured:
                        e.Value();
                        // waiting for fake counts reply
                        break;
                    case Device.DeviceStates.Measuring:
                        // async message here with auto send-back
                        // and waiting for fake counts reply
                        break;

                    case Device.DeviceStates.Ready:
                        hBlock = false;
                        setProgramStateWithoutUndo(ProgramStates.Ready);
                        break;
                    case Device.DeviceStates.WaitHighVoltage:
                        hBlock = true;
                        setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);
                        break;
                }
                OnLog(pState.ToString());
            };
            realizer.FirstStatus += onTheFlyAction;
        }
        protected override IRealizer GetRealizer(PortLevel port, Generator<bool> notRare) {
            return realizer = new SevMorGeoRealizer(port, Config.Try, () => notRare() ? 5 : 3, () => notRare() ? 500 : 10000);
        }
        
        public override void Bind(IMSControl view) {
            base.Bind(view);
            view.Init += Init;
            view.Shutdown += Shutdown;
            view.Unblock += Unblock;
        }

        void Init(object sender, CallBackEventArgs<bool> e) {
            OnLog(pState.ToString());

            setProgramState(ProgramStates.WaitInit);
            e.Value = true;
            SubscribeToUndo(e.Handler);
            realizer.SetOperationToggle(true);

            OnLog(pState.ToString());
        }
        void Shutdown(object sender, CallBackEventArgs<bool> e) {
            Disable();
            setProgramState(ProgramStates.WaitShutdown);
            e.Value = true;
            SubscribeToUndo(e.Handler);
            realizer.SetOperationToggle(false);
            // TODO: добавить контрольное время ожидания выключения
        }
        // TODO: common
        void Unblock(object sender, CallBackEventArgs<bool> e) {
            if (pState == ProgramStates.Measure ||
                pState == ProgramStates.WaitBackgroundMeasure ||
                pState == ProgramStates.BackgroundMeasureReady)//strange..
                MeasureCancelRequested = true;
            // TODO: check!
            e.Value = hBlock;
            SubscribeToUndo(e.Handler);
            realizer.SetOperationBlock(hBlock);
        }

        public override void Scan() {
            if (pState == ProgramStates.Ready) {
                Graph.Instance.Reset();
                CurrentMeasureMode = new MeasureMode.Scan();
                CurrentMeasureMode.SuccessfulExit += (s, e) => Config.autoSaveSpectrumFile();
                CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => Graph.Instance.updateGraphDuringScanMeasure(p, Device.Detectors);
                initMeasure(ProgramStates.Measure);
            }
        }
        public override bool Sense() {
            if (pState == ProgramStates.Ready) {
                if (SomePointsUsed) {
                    Graph.Instance.Reset();
                    {
                        var temp = new MeasureMode.Precise();
                        temp.SaveResults += (s, e) => Config.autoSavePreciseSpectrumFile(e.Shift);
                        CurrentMeasureMode = temp;
                    }
                    CurrentMeasureMode.SuccessfulExit += (s, e) => {
                        var ee = e as MeasureMode.Precise.SuccessfulExitEventArgs;
                        Graph.Instance.updateGraphAfterPreciseMeasure(ee.Counts, ee.Points, ee.Shift);
                    };
                    CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => Graph.Instance.updateGraphDuringPreciseMeasure(p, peak, Device.Detectors);
                    initMeasure(ProgramStates.Measure);
                    return true;
                } else {
                    OnLog("No points for precise mode measure.");
                    return false;
                }
            }
            return false;
        }

        // TODO: use simple arrays
        FixedSizeQueue<List<long>> background;
        Matrix matrix;
        List<long> backgroundResult;
        bool doBackgroundPremeasure;
        public override bool? Monitor() {
            byte backgroundCycles = Config.BackgroundCycles;
            doBackgroundPremeasure = Config.BackgroundCycles != 0;
            if (pState == ProgramStates.Ready) {
                if (SomePointsUsed) {
                    //Order is important here!!!! Underlying data update before both matrix formation and measure mode init.
                    Graph.Instance.ResetForMonitor();

                    #warning matrix is formed too early
                    // TODO: move matrix formation to manual operator actions
                    // TODO: parallelize matrix formation, flag on completion
                    // TODO: duplicates
                    var peaksForMatrix = Graph.Instance.PreciseData.getUsed().getWithId();
                    if (peaksForMatrix.Count > 0) {
                        // To comply with other processing order (and saved information)
                        peaksForMatrix.Sort(PreciseEditorData.ComparePreciseEditorDataByPeakValue);
                        matrix = new Matrix(Config.LoadLibrary(peaksForMatrix));
                        // What do with empty matrix?
                        if (matrix != null)
                            matrix.Init();
                        else {
                            OnLog("Error in peak data format or duplicate substance.");
                            return null;
                        }
                    } else
                        matrix = null;

                    // TODO: feed measure mode with start shift value (really?)
                    short? startShiftValue = 0;
                    var temp = new MeasureMode.Precise.Monitor(Config.CheckerPeak == null ? null : startShiftValue, Config.AllowedShift, Config.TimeLimit);
                    temp.SaveResults += (s, e) => Config.autoSaveMonitorSpectrumFile(e.Shift);
                    CurrentMeasureMode = temp;
                    CurrentMeasureMode.Finalize += (s, e) => Config.finalizeMonitorFile();
                    CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => Graph.Instance.updateGraphDuringPreciseMeasure(p, peak, Device.Detectors);
                    
                    if (doBackgroundPremeasure) {
                        initMeasure(ProgramStates.WaitBackgroundMeasure);
                        background = new FixedSizeQueue<List<long>>(backgroundCycles);
                        // or maybe Enumerator realization: one item, always recounting (accumulate values)..
                        Graph.Instance.NewGraphData += NewBackgroundMeasureReady;
                    } else {
                        initMeasure(ProgramStates.Measure);
                        Graph.Instance.NewGraphData += NewMonitorMeasureReady;
                    }
                    return true;
                } else {
                    OnLog("No points for monitor mode measure.");
                    return null;
                }
            } else if (pState == ProgramStates.BackgroundMeasureReady) {
                Graph.Instance.NewGraphData -= NewBackgroundMeasureReady;

                backgroundResult = background.Aggregate(Summarize);
                for (int i = 0; i < backgroundResult.Count; ++i) {
                    // TODO: check integral operation behaviour here
                    backgroundResult[i] /= backgroundCycles;
                }

                setProgramStateWithoutUndo(ProgramStates.Measure);
                Graph.Instance.NewGraphData += NewMonitorMeasureReady;
                return false;
            } else {
                // wrong state, strange!
                return null;
            }
        }
        void NewBackgroundMeasureReady(uint[] counts, params int[] recreate) {
            // TODO: more accurately
            if (recreate.Length == Graph.Instance.Collectors.Count) {
                List<long> currentMeasure = new List<long>();
                // ! temporary solution
                var peaksForMatrix = Graph.Instance.PreciseData.getUsed().getWithId();
                if (peaksForMatrix.Count > 0) {
                    // To comply with other processing order (and saved information)
                    peaksForMatrix.Sort(PreciseEditorData.ComparePreciseEditorDataByPeakValue);
                    foreach (PreciseEditorData ped in peaksForMatrix) {
                        //!!!!! null PLSreference! race condition?
                        currentMeasure.Add(ped.AssociatedPoints.PLSreference.PeakSum);
                    }
                }
                //maybe null if background premeasure is false!
                background.Enqueue(currentMeasure);
                if (pState == ProgramStates.WaitBackgroundMeasure && background.IsFull) {
                    setProgramStateWithoutUndo(ProgramStates.BackgroundMeasureReady);
                }
            }
        }
        void NewMonitorMeasureReady(uint[] counts, params int[] recreate) {
            if (recreate.Length == 0)
                return;
            List<long> currentMeasure = new List<long>();
            // ! temporary solution
            var peaksForMatrix = Graph.Instance.PreciseData.getUsed().getWithId();
            if (peaksForMatrix.Count > 0) {
                // To comply with other processing order (and saved information)
                peaksForMatrix.Sort(PreciseEditorData.ComparePreciseEditorDataByPeakValue);
                foreach (PreciseEditorData ped in peaksForMatrix) {
                    currentMeasure.Add(ped.AssociatedPoints.PLSreference.PeakSum);
                }
            }
            if (doBackgroundPremeasure) {
                if (currentMeasure.Count != backgroundResult.Count) {
                    // length mismatch
                    // TODO: throw smth
                }
                // distract background
                for (int i = 0; i < backgroundResult.Count; ++i) {
                    currentMeasure[i] -= backgroundResult[i];
                }
            }
            if (matrix != null) {
                // solve matrix equation
                double[] result = matrix.Solve(currentMeasure.ConvertAll<double>(x => (double)x));
                // TODO: now it is normalized to 999 on maximum of peak spectrum component
                // but we want actual value
                // weight of mass measured also can differ from 999
                Config.AutoSaveSolvedSpectra(result);
                // TODO: put here all automatic logic from measure modes
            }
        }
        static List<long> Summarize(List<long> workingValue, List<long> nextElem) {
            // TODO: move from Commander to Utility
            if (workingValue.Count != nextElem.Count)
                // data length mismatch
                return null;
            for (int i = 0; i < workingValue.Count; ++i) {
                workingValue[i] += nextElem[i];
            }
            return workingValue;
        }

        void initMeasure(ProgramStates state) {
            OnLog(pState.ToString());
            if (CurrentMeasureMode != null && CurrentMeasureMode.isOperating) {
                //error. something in operation
                throw new Exception("Measure mode already in operation.");
            }
            CurrentMeasureMode.VoltageStepChangeRequested += measureMode_VoltageStepChangeRequested;
            CurrentMeasureMode.Disable += CurrentMeasureMode_Disable;
            // TODO: move inside MeasureMode
            Device.CountsUpdated += deviceCountsUpdated;

            setProgramState(state);

            MeasureCancelRequested = false;
            SendSettings();
        }
        void CurrentMeasureMode_Disable(object sender, EventArgs e) {
            if (CurrentMeasureMode is MeasureMode.Precise.Monitor) {
                if (pState == ProgramStates.Measure) {
                    Graph.Instance.NewGraphData -= NewMonitorMeasureReady;
                } else if (pState == ProgramStates.WaitBackgroundMeasure || pState == ProgramStates.BackgroundMeasureReady) {
                    Graph.Instance.NewGraphData -= NewBackgroundMeasureReady;
                }
                matrix = null;
            }
            // TODO: move inside MeasureMode
            Device.CountsUpdated -= deviceCountsUpdated;
            CurrentMeasureMode.VoltageStepChangeRequested -= measureMode_VoltageStepChangeRequested;
            CurrentMeasureMode.Disable -= CurrentMeasureMode_Disable;

            setProgramStateWithoutUndo(ProgramStates.Ready);//really without undo?
            Disable();
        }
        void measureMode_VoltageStepChangeRequested(object sender, MeasureMode.VoltageStepEventArgs e) {
            realizer.SetMeasureStep(e.Step);
        }
    }
}
