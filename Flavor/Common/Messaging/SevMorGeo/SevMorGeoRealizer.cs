using System;

namespace Flavor.Common.Messaging.SevMorGeo {
    class SevMorGeoRealizer: RealizerWithAutomatedStatusChecks<CommandCode> {
        public SevMorGeoRealizer(PortLevel port, Generator<int> factor, Generator<double> interval)
            : this(new ModBus(port), factor, interval) { }
        SevMorGeoRealizer(ISyncAsyncProtocol<CommandCode> protocol, Generator<int> factor, Generator<double> interval)
            : this(protocol, new MessageQueueWithAutomatedStatusChecks<CommandCode>(protocol,
                new StatusRequestGenerator(new requestStatus(), new getTurboPumpStatus(), factor),
                interval)) { }
        SevMorGeoRealizer(IAsyncProtocol<CommandCode> protocol, MessageQueueWithAutomatedStatusChecks<CommandCode> queue)
            : base(protocol, queue) { }
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
        public override void Disconnect() {
            base.Disconnect();
            onTheFly = true;
        }
        bool onTheFly = true;
        void Add<T1>(PackageDictionary<CommandCode> d, Action<ServicePacket<CommandCode>> action)
            where T1: ServicePacket<CommandCode>, new() {
            d[new T1()] = new PackageRecord<CommandCode>(action);
        }
        Action<ServicePacket<CommandCode>> AutoSend<T1>()
            where T1: UserRequest<CommandCode>, new() {
            return p => toSend.Enqueue(new T1());
        }
        protected override PackageDictionary<CommandCode> GetDictionary() {
            var d = new PackageDictionary<CommandCode>();
            //Action<CommandCode, Action<ServicePacket<CommandCode>>> add = (code, action) => d[(byte)code] = new PackageRecord<CommandCode>(action);
            Action<ServicePacket<CommandCode>> updateDevice = p => ((IUpdateDevice)p).UpdateDevice();
                
            //async error
            Add<logInternalError>(d, null);
            Add<logInvalidSystemState>(d, null);
            Add<logVacuumCrash>(d, null);
            Add<logTurboPumpFailure>(d, updateDevice);
            Add<logPowerFail>(d, null);
            Add<logInvalidVacuumState>(d, null);
            Add<logAdcPlaceIonSrc>(d, null);
            Add<logAdcPlaceScanv>(d, null);
            Add<logAdcPlaceControlm>(d, null);
            //async
            Add<requestCounts>(d, AutoSend<getCounts>());
            Add<confirmVacuumReady>(d, updateDevice + (p => OnSystemReady()));
            Add<confirmShutdowned>(d, p => OnSystemDown(false));
            Add<SystemReseted>(d, p => {
                OnAsyncReplyReceived("Система переинициализировалась");
                OnSystemDown(true);
            });
            Add<confirmHighVoltageOff>(d, p => OnOperationBlock(true));
            Add<confirmHighVoltageOn>(d, p => {
                OnOperationBlock(false);
                toSend.Enqueue(new sendSVoltage(0));//Set ScanVoltage to low limit
                toSend.Enqueue(new sendIVoltage());// и остальные напряжения затем
            });
            //sync error
            Add<logInvalidCommand>(d, null);
            Add<logInvalidChecksum>(d, null);
            Add<logInvalidPacket>(d, null);
            Add<logInvalidLength>(d, null);
            Add<logInvalidData>(d, null);
            Add<logInvalidState>(d, null);
            //sync
            Add<updateState>(d, updateDevice/* + autoSend*/);
            Add<updateStatus>(d, updateDevice + (p => {
                if (onTheFly) {
                    // waiting for fake counts reply
                    OnFirstStatus(() => toSend.Enqueue(new getCounts()));
                    onTheFly = false;
                }
            }));
            Add<confirmShutdown>(d, p => OnOperationToggle(false));
            Add<confirmInit>(d, p => OnOperationToggle(true)/* + autoSend*/);

            // settings sequence
            Add<confirmIVoltage>(d, AutoSend<sendCapacitorVoltage>());
            Add<confirmCP>(d, AutoSend<enableHCurrent>());
            Add<confirmHECurrent>(d, AutoSend<sendECurrent>());
            Add<confirmECurrent>(d, AutoSend<sendHCurrent>());
            Add<confirmHCurrent>(d, AutoSend<sendF1Voltage>());
            Add<confirmF1Voltage>(d, AutoSend<sendF2Voltage>());
            Add<confirmF2Voltage>(d, p => OnMeasurePreconfigured());

            Add<confirmSVoltage>(d, p => OnMeasureSend((t1, t2) => toSend.Enqueue(new sendMeasure(t1, t2))));

            Add<confirmMeasure>(d, null);
            Add<updateCounts>(d, updateDevice + (p => OnMeasureDone()));
            Add<confirmHighVoltage>(d, null);
            Add<updateTurboPumpStatus>(d, updateDevice);
            Add<confirmForvacuumLevel>(d, null);
            return d;
        }
    }
}
