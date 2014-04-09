using System;

namespace Flavor.Common.Messaging.SevMorGeo {
    class SevMorGeoRealizer: Realizer<CommandCode> {
        readonly MessageQueueWithAutomatedStatusChecks<CommandCode> toSend;
        public SevMorGeoRealizer(PortLevel port, Generator<int> factor, Generator<double> interval)
            : this(new ModBus(port), factor, interval) { }
        SevMorGeoRealizer(ISyncAsyncProtocol<CommandCode> protocol, Generator<int> factor, Generator<double> interval)
            : this(protocol, new MessageQueueWithAutomatedStatusChecks<CommandCode>(protocol,
                new StatusRequestGenerator(new requestStatus(), new getTurboPumpStatus(), factor),
                interval)) { }
        SevMorGeoRealizer(IAsyncProtocol<CommandCode> protocol, MessageQueueWithAutomatedStatusChecks<CommandCode> queue)
            : base(protocol, queue) {
            toSend = queue;
        }
        class StatusRequestGenerator : IStatusRequestGenerator<CommandCode> {
            int i = 0;
            int f;
            readonly UserRequest<CommandCode> statusCheck, vacuumCheck;
            readonly Generator<int> factor;
            public UserRequest<CommandCode> Next {
                get {
                    UserRequest<CommandCode> res;
                    if (i == 0)
                        res = vacuumCheck;
                    else
                        res = statusCheck;
                    ++i;
                    i %= f;
                    return res;
                }
            }
            public void Reset() {
                f = factor();
            }
            public StatusRequestGenerator(UserRequest<CommandCode> statusCheck, UserRequest<CommandCode> vacuumCheck, Generator<int> factor) {
                this.factor = factor;
                this.statusCheck = statusCheck;
                this.vacuumCheck = vacuumCheck;
                Reset();
            }
        }
        public override void SetOperationBlock(bool block) {
            toSend.Enqueue(new enableHighVoltage(block));
        }
        public override void SetOperationToggle(bool on) {
            if (on)
                toSend.Enqueue(new sendInit());
            else
                toSend.Enqueue(new sendShutdown());
        }
        public override void SetSettings() {
            toSend.Enqueue(new sendIVoltage());
            //And all sequence (see in dictionary)
        }
        public override void SetMeasureStep(ushort step) {
            toSend.Enqueue(new sendSVoltage(step));
        }
        public override void Connect(Action undo) {
            base.Connect(undo);
            toSend.Start();
        }
        public override void Disconnect(Action undo) {
            toSend.Stop();
            base.Disconnect(undo);
            onTheFly = true;
        }
        // TODO: move to abstract ancestor
        public void Reset() {
            toSend.Stop();
            toSend.Start();
        }
        bool onTheFly = true;
        protected override PackageDictionary<CommandCode> GetDictionary() {
            var d = new PackageDictionary<CommandCode>();
            Action<CommandCode, Action<ServicePacket<CommandCode>>> add = (code, action) => d[(byte)code] = new PackageRecord<CommandCode>(action);
            Action<ServicePacket<CommandCode>> sendAction = p => toSend.Enqueue(((IAutomatedReply)p).AutomatedReply() as UserRequest<CommandCode>);
            Action<ServicePacket<CommandCode>> updateDeviceAction = p => ((IUpdateDevice)p).UpdateDevice();
            
            add(CommandCode.InternalError, null);
            add(CommandCode.InvalidSystemState, null);
            add(CommandCode.VacuumCrash, null);
            add(CommandCode.TurboPumpFailure, updateDeviceAction);
            add(CommandCode.PowerFail, null);
            add(CommandCode.InvalidVacuumState, null);
            add(CommandCode.AdcPlaceIonSrc, null);
            add(CommandCode.AdcPlaceScanv, null);
            add(CommandCode.AdcPlaceControlm, null);

            add(CommandCode.Measured, sendAction);
            add(CommandCode.VacuumReady, updateDeviceAction + (p => OnSystemReady()));
            add(CommandCode.SystemShutdowned, p => OnSystemDown(false));
            add(CommandCode.SystemReseted, p => {
                OnAsyncReplyReceived("Система переинициализировалась");
                OnSystemDown(true);
            });
            add(CommandCode.HighVoltageOff, p => OnOperationBlock(true));
            add(CommandCode.HighVoltageOn, p => {
                OnOperationBlock(false);
                toSend.Enqueue(new sendSVoltage(0));//Set ScanVoltage to low limit
                toSend.Enqueue(new sendIVoltage());// и остальные напряжения затем
            });
            
            add(CommandCode.InvalidCommand, null);
            add(CommandCode.InvalidChecksum, null);
            add(CommandCode.InvalidPacket, null);
            add(CommandCode.InvalidLength, null);
            add(CommandCode.InvalidData, null);
            add(CommandCode.InvalidState, null);

            add(CommandCode.GetState, updateDeviceAction/* + sendAction*/);
            add(CommandCode.GetStatus, updateDeviceAction + (p => {
                if (onTheFly) {
                    // waiting for fake counts reply
                    OnFirstStatus(() => toSend.Enqueue(new getCounts()));
                    onTheFly = false;
                }
            }));
            add(CommandCode.Shutdown, p => OnOperationToggle(false));
            add(CommandCode.Init, p => OnOperationToggle(true)/* + sendAction*/);

            // settings sequence
            add(CommandCode.SetIonizationVoltage, sendAction);
            add(CommandCode.SetCapacitorVoltage, sendAction);
            add(CommandCode.heatCurrentEnable, sendAction);
            add(CommandCode.SetEmissionCurrent, sendAction);
            add(CommandCode.SetHeatCurrent, sendAction);
            add(CommandCode.SetFocusVoltage1, sendAction);
            add(CommandCode.SetFocusVoltage2, p => OnMeasurePreconfigured());

            add(CommandCode.SetScanVoltage, p => OnMeasureSend((t1, t2) => toSend.Enqueue(new sendMeasure(t1, t2))));

            add(CommandCode.Measure, null);
            add(CommandCode.GetCounts, updateDeviceAction + (p => OnMeasureDone()));
            add(CommandCode.EnableHighVoltage, null);
            add(CommandCode.GetTurboPumpStatus, updateDeviceAction);
            add(CommandCode.SetForvacuumLevel, null);
            return d;
        }
    }
}
