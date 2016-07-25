using System;

namespace Flavor.Common {
    using Data.Controlled;
    using Messaging;
    using Messaging.SevMorGeo;
    using Settings;
    class SevMorGeoCommader : Commander {
        SevMorGeoRealizer realizer;
        readonly EventHandler<EventArgs<Action>> onTheFlyAction;
        public SevMorGeoCommader()
            // TODO: proper device
            : base(new PortLevel(), null) {
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
        protected override IRealizer GetRealizer(PortLevel port, Func<bool> notRare) {
            return realizer = new SevMorGeoRealizer(port, Config.Try, () => notRare() ? 5 : 3, () => notRare() ? 500 : 10000);
        }
        
        public override void Bind(IMSControl view) {
            base.Bind(view);
            view.Init += Init;
            view.Shutdown += Shutdown;
            view.Unblock += Unblock;
        }
        protected override uint[] Counts { get { return Device.Detectors; } }
        protected override void SubscribeToCountsUpdated(EventHandler<EventArgs<uint[]>> handler) {
            Device.CountsUpdated += handler;
        }
        protected override void UnsubscribeToCountsUpdated(EventHandler<EventArgs<uint[]>> handler) {
            Device.CountsUpdated -= handler;
        }
    }
}
