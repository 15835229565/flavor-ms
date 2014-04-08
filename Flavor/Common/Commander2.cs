using System;
using System.Collections.Generic;
using System.Timers;
using Flavor.Common.Messaging;
using Flavor.Common.Messaging.SevMorGeo;
using Flavor.Common.Library;

namespace Flavor.Common {
    class Commander2: ICommander {
        ProgramStates programState = ProgramStates.Start;
        public override ProgramStates pState {
            get {
                return programState;
            }
            protected set {
                if (programState != value) {
                    programState = value;
                    if (value == ProgramStates.Start)
                        Disable();
                    OnProgramStateChanged();
                };
            }
        }

        void setProgramStateWithoutUndo(ProgramStates state) {
            pState = state;
            pStatePrev = pState;
        }
        void setProgramState(ProgramStates state) {
            pStatePrev = pState;
            pState = state;
        }

        bool handleBlock = true;
        public override bool hBlock {
            get {
                return handleBlock;
            }
            set {
                if (handleBlock != value) {
                    handleBlock = value;
                    OnProgramStateChanged();
                };
            }
        }

        bool isConnected = false;
        public override bool DeviceIsConnected {
            get {
                return isConnected;
            }
            protected set {
                if (isConnected != value) {
                    isConnected = value;
                }
            }
        }

        bool measureCancelRequested = false;
        public override bool MeasureCancelRequested {
            protected get { return measureCancelRequested; }
            set {
                measureCancelRequested = value;
                if (value && CurrentMeasureMode != null)
                    CurrentMeasureMode.CancelRequested = value;
            }
        }

        public override void Init(object sender, EventArgs<bool> e) {
            OnLog(pState.ToString());

            setProgramState(ProgramStates.WaitInit);
            e.Value = true;
            r.SetOperationToggle(true);

            OnLog(pState.ToString());
        }
        public override void Shutdown(object sender, EventArgs<bool> e) {
            Disable();
            r.SetOperationToggle(false);
            setProgramState(ProgramStates.WaitShutdown);
            e.Value = true;
            // TODO: добавить контрольное время ожидания выключения
        }

        public override void Scan() {
            if (pState == ProgramStates.Ready) {
                Graph.Instance.Reset();
                CurrentMeasureMode = new MeasureMode.Scan();
                CurrentMeasureMode.SuccessfulExit += (s, e) => Config.autoSaveSpectrumFile();
                CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => Graph.Instance.updateGraphDuringScanMeasure(p, Device.Detector1, Device.Detector2);
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
                    CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => Graph.Instance.updateGraphDuringPreciseMeasure(p, peak);
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
                        peaksForMatrix.Sort(Utility.PreciseEditorData.ComparePreciseEditorDataByPeakValue);
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
                    CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => Graph.Instance.updateGraphDuringPreciseMeasure(p, peak);
                    
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
        void NewBackgroundMeasureReady(int[] recreate) {
            // TODO: more accurately
            if (recreate.Length == Graph.Instance.Collectors.Count) {
                List<long> currentMeasure = new List<long>();
                // ! temporary solution
                var peaksForMatrix = Graph.Instance.PreciseData.getUsed().getWithId();
                if (peaksForMatrix.Count > 0) {
                    // To comply with other processing order (and saved information)
                    peaksForMatrix.Sort(Utility.PreciseEditorData.ComparePreciseEditorDataByPeakValue);
                    foreach (Utility.PreciseEditorData ped in peaksForMatrix) {
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
        void NewMonitorMeasureReady(int[] recreate) {
            if (recreate.Length == 0)
                return;
            List<long> currentMeasure = new List<long>();
            // ! temporary solution
            var peaksForMatrix = Graph.Instance.PreciseData.getUsed().getWithId();
            if (peaksForMatrix.Count > 0) {
                // To comply with other processing order (and saved information)
                peaksForMatrix.Sort(Utility.PreciseEditorData.ComparePreciseEditorDataByPeakValue);
                foreach (Utility.PreciseEditorData ped in peaksForMatrix) {
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

        void Disable() {
            MeasureCancelRequested = false;
            // TODO: lock here (request from ui may cause synchro errors)
            // or use async action paradigm
            OnMeasureCancelled();
            CurrentMeasureMode = null;//?
        }
        public override void SendSettings() {
            r.SetSettings();
        }
        void initMeasure(ProgramStates state) {
            OnLog(pState.ToString());
            if (CurrentMeasureMode != null && CurrentMeasureMode.isOperating) {
                //error. something in operation
                throw new Exception("Measure mode already in operation.");
            }
            CurrentMeasureMode.VoltageStepChangeRequested += measureMode_VoltageStepChangeRequested;
            CurrentMeasureMode.Disable += CurrentMeasureMode_Disable;
            
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
            CurrentMeasureMode.VoltageStepChangeRequested -= measureMode_VoltageStepChangeRequested;
            CurrentMeasureMode.Disable -= CurrentMeasureMode_Disable;

            setProgramStateWithoutUndo(ProgramStates.Ready);//really without undo?
            Disable();
        }
        void measureMode_VoltageStepChangeRequested(object sender, MeasureMode.VoltageStepEventArgs e) {
            r.SetMeasureStep(e.Step);
        }
        public override bool SomePointsUsed {
            get {
                if (Config.PreciseData.Count > 0)
                    foreach (Utility.PreciseEditorData ped in Config.PreciseData)
                        if (ped.Use) return true;
                return false;
            }
        }

        public override void Unblock(object sender, EventArgs<bool> e) {
            if (pState == ProgramStates.Measure ||
                pState == ProgramStates.WaitBackgroundMeasure ||
                pState == ProgramStates.BackgroundMeasureReady)//strange..
                MeasureCancelRequested = true;
            r.SetOperationBlock(hBlock);
            // TODO: check!
            e.Value = hBlock;
        }

        readonly PortLevel port = new PortLevel();
        readonly Realizer<CommandCode> r;
        public Commander2() {
            port.ErrorPort += (s, e) => {
                // TODO: more accurate
                OnErrorOccured(e.Message);
            };
            notRareModeRequested = true;
            var r = new SevMorGeoRealizer(port, () => notRare() ? 5 : 3, () => notRare() ? 500 : 10000);
            
            // eliminate local events!
            ProgramStateChanged += s => {
                if (s == ProgramStates.Measure || s == ProgramStates.BackgroundMeasureReady || s == ProgramStates.WaitBackgroundMeasure) {
                    r.Reset();
                }
            };
            MeasureCancelled += s => r.Reset();

            ConsoleWriter.Subscribe(r);
            r.SystemDown += (s, e) => {
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
            r.SystemReady += (s, e) => {
                if (hBlock) {
                    setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);
                } else {
                    setProgramStateWithoutUndo(ProgramStates.Ready);
                }
            };
            r.OperationBlock += (s, e) => {
                hBlock = e.Value;
                if (hBlock) {
                    setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);//???
                }
                else {
                    if (pState == ProgramStates.WaitHighVoltage) {
                        setProgramStateWithoutUndo(ProgramStates.Ready);
                    }
                }
            };
            r.OperationToggle += (s, e) => {
                if (e.Value) {
                    OnLog("Init request confirmed");
                    setProgramStateWithoutUndo(ProgramStates.Init);
                }
                else {
                    OnLog("Shutdown request confirmed");
                    setProgramStateWithoutUndo(ProgramStates.Shutdown);
                }
                OnLog(pState.ToString());
            };
            r.MeasurePreconfigured += (s, e) => {
                if (pState == ProgramStates.Measure ||
                    pState == ProgramStates.WaitBackgroundMeasure) {
                    if (!CurrentMeasureMode.Start()) {
                        OnErrorOccured("Нет точек для измерения.");
                    }
                }
            };
            r.MeasureSend += (s, e) => {
                if (CurrentMeasureMode != null && CurrentMeasureMode.isOperating) {
                    CurrentMeasureMode.NextMeasure(e.Value);
                }
            };
            r.MeasureDone += (s, e) => {
                if (CurrentMeasureMode == null) {
                    // fake reply caught here (in order to put device into proper state)
                    hBlock = false;
                    setProgramStateWithoutUndo(ProgramStates.Ready);
                    return;
                }
                // TODO: graph update event (move to Device-Graph bound, otherwise order is important)
                CurrentMeasureMode.UpdateGraph();
                if (!CurrentMeasureMode.onUpdateCounts()) {
                    OnErrorOccured("Измеряемая точка вышла за пределы допустимого диапазона.\nРежим измерения прекращен.");
                }
            };
            r.FirstStatus += r_FirstStatus;
            this.r = r;
        }
        void r_FirstStatus(object sender, EventArgs<Action> e) {
            (sender as Realizer<CommandCode>).FirstStatus -= r_FirstStatus;
            switch (Device.sysState)
            {
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
        }
        bool notRare() {
            if (pState == ProgramStates.Measure || pState == ProgramStates.BackgroundMeasureReady || pState == ProgramStates.WaitBackgroundMeasure)
                return notRareModeRequested;
            return true;
        }
        public override void Connect(object sender, CallBackEventArgs<bool, string> e) {
            if (DeviceIsConnected) {
                Disconnect();
            } else {
                Connect();
            }
            e.Value = DeviceIsConnected;
            //TODO:!
            //e.Handler = AsyncReplyReceived;
        }
        void Connect() {
            PortLevel.PortStates res = port.Open();
            switch (res) {
                case PortLevel.PortStates.Opening:
                    // TODO: to Realizer
                    r.Connect(() => setProgramStateWithoutUndo(pStatePrev));

                    DeviceIsConnected = true;
                    break;
                case PortLevel.PortStates.Opened:
                    DeviceIsConnected = true;
                    break;
                case PortLevel.PortStates.ErrorOpening:
                    break;
                default:
                    // фигня
                    break;
            }
        }
        public override void Disconnect() {
            r.Disconnect(() => setProgramStateWithoutUndo(pStatePrev));

            PortLevel.PortStates res = port.Close();
            switch (res) {
                case PortLevel.PortStates.Closing:
                    DeviceIsConnected = false;
                    //onTheFly = true;// надо ли здесь???
                    break;
                case PortLevel.PortStates.Closed:
                    DeviceIsConnected = false;
                    break;
                case PortLevel.PortStates.ErrorClosing:
                    break;
                default:
                    // фигня
                    break;
            }
        }

        public override void Reconnect() {
            if (DeviceIsConnected) {
                Disconnect();
                if (!DeviceIsConnected)
                    port.Open();
            }
        }
        public override string[] AvailablePorts {
            get { return PortLevel.AvailablePorts; }
        }
    }
}
