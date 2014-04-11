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
        class StatusRequestGenerator: IStatusRequestGenerator<CommandCode> {
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
        protected override UserRequest<CommandCode> Block(bool block) {
            return new enableHighVoltage(block);
        }
        protected override UserRequest<CommandCode> OperationOnOff(bool on) {
            if (on)
                return new sendInit();
            else
                return new sendShutdown();
        }
        protected override UserRequest<CommandCode> Settings() {
            return new sendIVoltage();
        }
        protected override UserRequest<CommandCode> MeasureStep(ushort step) {
            return new sendSVoltage(step);
        }
        public override void Connect() {
            base.Connect();
            toSend.Start();
        }
        public override void Disconnect() {
            toSend.Stop();
            base.Disconnect();
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
            //async error
            add(CommandCode.InternalError, null);
            add(CommandCode.InvalidSystemState, null);
            add(CommandCode.VacuumCrash, null);
            add(CommandCode.TurboPumpFailure, updateDevice);
            add(CommandCode.PowerFail, null);
            add(CommandCode.InvalidVacuumState, null);
            add(CommandCode.AdcPlaceIonSrc, null);
            add(CommandCode.AdcPlaceScanv, null);
            add(CommandCode.AdcPlaceControlm, null);
            //async
            add(CommandCode.Measured, autoSend);
            add(CommandCode.VacuumReady, updateDevice + (p => OnSystemReady()));
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
            //sync error
            add(CommandCode.InvalidCommand, null);
            add(CommandCode.InvalidChecksum, null);
            add(CommandCode.InvalidPacket, null);
            add(CommandCode.InvalidLength, null);
            add(CommandCode.InvalidData, null);
            add(CommandCode.InvalidState, null);
            //sync
            add(CommandCode.GetState, updateDevice/* + sendAction*/);
            add(CommandCode.GetStatus, updateDevice + (p => {
                if (onTheFly) {
                    // waiting for fake counts reply
                    OnFirstStatus(() => toSend.Enqueue(new getCounts()));
                    onTheFly = false;
                }
            }));
            add(CommandCode.Shutdown, p => OnOperationToggle(false));
            add(CommandCode.Init, p => OnOperationToggle(true)/* + sendAction*/);

            // settings sequence
            add(CommandCode.SetIonizationVoltage, autoSend);
            add(CommandCode.SetCapacitorVoltage, autoSend);
            add(CommandCode.heatCurrentEnable, autoSend);
            add(CommandCode.SetEmissionCurrent, autoSend);
            add(CommandCode.SetHeatCurrent, autoSend);
            add(CommandCode.SetFocusVoltage1, autoSend);
            add(CommandCode.SetFocusVoltage2, p => OnMeasurePreconfigured());

            add(CommandCode.SetScanVoltage, p => OnMeasureSend((t1, t2) => toSend.Enqueue(new sendMeasure(t1, t2))));

            add(CommandCode.Measure, null);
            add(CommandCode.GetCounts, updateDevice + (p => OnMeasureDone()));
            add(CommandCode.EnableHighVoltage, null);
            add(CommandCode.GetTurboPumpStatus, updateDevice);
            add(CommandCode.SetForvacuumLevel, null);
            return d;
        }
    }
}
