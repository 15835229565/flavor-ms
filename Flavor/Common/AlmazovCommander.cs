using System;
using Flavor.Common.Messaging;
using Flavor.Common.Messaging.Almazov;
using Flavor.Common.Settings;
using Flavor.Common.Data.Measure;

namespace Flavor.Common {
    class AlmazovCommander: Commander {
        AlmazovRealizer realizer;
        //readonly EventHandler<EventArgs<Action>> onTheFlyAction;
        readonly EventHandler<EventArgs<uint[]>> deviceCountsUpdated;
        public AlmazovCommander()
            : base(new PortLevel(), new AlmazovDevice()) {
            deviceCountsUpdated = (s, e) => {
                CurrentMeasureMode.UpdateGraph();
                if (!CurrentMeasureMode.onUpdateCounts(device.Detectors)) {
                    OnErrorOccured("Измеряемая точка вышла за пределы допустимого диапазона.\nРежим измерения прекращен.");
                }
            };
            device.DeviceStateChanged += (s, e) => {
                // TODO: change temporary solution
                var state = (AlmazovDevice.DeviceStates)e.Value;
                if ((state & AlmazovDevice.DeviceStates.Alert) != 0) {
                    setProgramStateWithoutUndo(ProgramStates.Shutdown);
                    return;
                }
                if ((state & AlmazovDevice.DeviceStates.PRGE) != 0) {
                    hBlock = false;
                    setProgramStateWithoutUndo(ProgramStates.Ready);
                    return;
                }
                if ((state & AlmazovDevice.DeviceStates.HVE) != 0) {
                    hBlock = true;
                    setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);
                    return;
                }
                setProgramStateWithoutUndo(ProgramStates.Init);
            };
            device.VacuumStateChanged += (s, e) => {
                var ee = e as EventArgs<bool>;
                // workaround for detecting Relay1 change
                if (ee != null)
                    realizer.SetOperationToggle(ee.Value);
            };
                /*realizer.SystemDown += (s, e) => {
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
                        //Device.Init();
                    }
                };*/
            
            /*onTheFlyAction = (s, e) => {
                (s as IRealizer).FirstStatus -= onTheFlyAction;
                OnLog(pState.ToString());
            };
            realizer.FirstStatus += onTheFlyAction;*/
        }
        protected override IRealizer GetRealizer(PortLevel port, Generator<bool> notRare) {
            return realizer = new AlmazovRealizer(port, Config.Try, () => notRare() ? 500 : 10000);
        }

        public override void Bind(IMSControl view) {
            base.Bind(view);
            view.Unblock += Unblock;
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
            // TODO: move partially up
            if (pState == ProgramStates.Ready) {
                Graph.Instance.Reset();
                CurrentMeasureMode = new MeasureMode.Scan();
                CurrentMeasureMode.SuccessfulExit += (s, e) => Config.autoSaveSpectrumFile();
                CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => Graph.Instance.updateGraphDuringScanMeasure(p, device.Detectors);
                initMeasure(ProgramStates.Measure);
            }
        }
        public override bool Sense() {
            // TODO: move partially up
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
                    CurrentMeasureMode.GraphUpdateDelegate = (p, peak) => Graph.Instance.updateGraphDuringPreciseMeasure(p, peak, device.Detectors);
                    initMeasure(ProgramStates.Measure);
                    return true;
                } else {
                    OnLog("No points for precise mode measure.");
                    return false;
                }
            }
            return false;
        }
        public override bool? Monitor() {
            throw new NotImplementedException();
        }
        void initMeasure(ProgramStates state) {
            // TODO: move partially up
            OnLog(pState.ToString());
            if (CurrentMeasureMode != null && CurrentMeasureMode.isOperating) {
                //error. something in operation
                throw new Exception("Measure mode already in operation.");
            }
            CurrentMeasureMode.VoltageStepChangeRequested += measureMode_VoltageStepChangeRequested;
            CurrentMeasureMode.Disable += CurrentMeasureMode_Disable;
            // TODO: move inside MeasureMode
            device.CountsUpdated += deviceCountsUpdated;

            setProgramState(state);

            MeasureCancelRequested = false;
            SendSettings();
            // temporarily mandatory start
            //CurrentMeasureMode.Start();
        }
        void CurrentMeasureMode_Disable(object sender, EventArgs e) {
            /*if (CurrentMeasureMode is MeasureMode.Precise.Monitor) {
                if (pState == ProgramStates.Measure) {
                    Graph.Instance.NewGraphData -= NewMonitorMeasureReady;
                } else if (pState == ProgramStates.WaitBackgroundMeasure || pState == ProgramStates.BackgroundMeasureReady) {
                    Graph.Instance.NewGraphData -= NewBackgroundMeasureReady;
                }
                matrix = null;
            }*/
            // TODO: move inside MeasureMode
            device.CountsUpdated -= deviceCountsUpdated;
            CurrentMeasureMode.VoltageStepChangeRequested -= measureMode_VoltageStepChangeRequested;
            CurrentMeasureMode.Disable -= CurrentMeasureMode_Disable;

            setProgramStateWithoutUndo(ProgramStates.Ready);//really without undo?
            Disable();
        }
        void measureMode_VoltageStepChangeRequested(object sender, MeasureMode.VoltageStepEventArgs e) {
            realizer.SetMeasureStep(e.Step);
            // TODO: move to realizer ctor as extra action on measure step
            if (notRareModeRequested) {
                realizer.CheckStepVoltages();
            }
        }
    }
}
