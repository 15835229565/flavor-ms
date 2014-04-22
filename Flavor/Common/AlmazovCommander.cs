using System;
using Flavor.Common.Messaging;
using Flavor.Common.Messaging.Almazov;

namespace Flavor.Common {
    class AlmazovCommander: Commander {
        AlmazovRealizer realizer;
        //readonly EventHandler<EventArgs<Action>> onTheFlyAction;
        readonly EventHandler<EventArgs<int[]>> deviceCountsUpdated;
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
                    setProgramStateWithoutUndo(ProgramStates.Ready);
                    return;
                }
                if ((state & AlmazovDevice.DeviceStates.HVE) != 0) {
                    setProgramStateWithoutUndo(ProgramStates.WaitHighVoltage);
                    return;
                }
                setProgramStateWithoutUndo(ProgramStates.Init);
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
            return realizer = new AlmazovRealizer(port, () => notRare() ? 500 : 10000);
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
            throw new NotImplementedException();
        }

        public override bool Sense() {
            throw new NotImplementedException();
        }

        public override bool? Monitor() {
            throw new NotImplementedException();
        }
    }
}
