using System;

namespace Flavor.Common.Messaging.SevMorGeo {
    class SevMorGeoRealizer: RealizerWithAutomatedStatusChecks<CommandCode> {
        public SevMorGeoRealizer(PortLevel port, byte attempts, Generator<int> factor, Generator<double> interval)
            : this(new ModBus(port), attempts, factor, interval) { }
        SevMorGeoRealizer(ISyncAsyncProtocol<CommandCode> protocol, byte attempts, Generator<int> factor, Generator<double> interval)
            : this(protocol, new MessageQueueWithAutomatedStatusChecks<CommandCode>(protocol,
                attempts,
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
        protected override void PopulateDictionary(PackageDictionary<CommandCode> d) {
            Action<ServicePacket<CommandCode>> updateDevice = p => ((IUpdateDevice)p).UpdateDevice();
                
            //async error
            Add<logInternalError>();
            Add<logInvalidSystemState>();
            Add<logVacuumCrash>();
            Add<logTurboPumpFailure>(updateDevice);
            Add<logPowerFail>();
            Add<logInvalidVacuumState>();
            Add<logAdcPlaceIonSrc>();
            Add<logAdcPlaceScanv>();
            Add<logAdcPlaceControlm>();
            //async
            Add<requestCounts>(AutoSend<getCounts>);
            Add<confirmVacuumReady>(updateDevice, p => OnSystemReady());
            Add<confirmShutdowned>(p => OnSystemDown(false));
            Add<SystemReseted>(p => {
                OnAsyncReplyReceived("Система переинициализировалась");
                OnSystemDown(true);
            });
            Add<confirmHighVoltageOff>(p => OnOperationBlock(true));
            Add<confirmHighVoltageOn>(p => {
                OnOperationBlock(false);
                toSend.Enqueue(new sendSVoltage(0));//Set ScanVoltage to low limit
                SetSettings();
                //toSend.Enqueue(new sendIVoltage());
            });
            //sync error
            Add<logInvalidCommand>();
            Add<logInvalidChecksum>();
            Add<logInvalidPacket>();
            Add<logInvalidLength>();
            Add<logInvalidData>();
            Add<logInvalidState>();
            //sync
            Add<updateState>(updateDevice);
            Add<updateStatus>(updateDevice, p => {
                if (onTheFly) {
                    // waiting for fake counts reply
                    OnFirstStatus(() => toSend.Enqueue(new getCounts()));
                    onTheFly = false;
                }
            });
            Add<confirmShutdown>(p => OnOperationToggle(false));
            Add<confirmInit>(p => OnOperationToggle(true));

            // settings sequence
            Add<confirmIVoltage>(AutoSend<sendCapacitorVoltage>);
            Add<confirmCP>(AutoSend<enableHCurrent>);
            Add<confirmHECurrent>(AutoSend<sendECurrent>);
            Add<confirmECurrent>(AutoSend<sendHCurrent>);
            Add<confirmHCurrent>(AutoSend<sendF1Voltage>);
            Add<confirmF1Voltage>(AutoSend<sendF2Voltage>);
            Add<confirmF2Voltage>(p => OnMeasurePreconfigured());

            Add<confirmSVoltage>(p => OnMeasureSend((t1, t2) => toSend.Enqueue(new sendMeasure(t1, t2))));

            Add<confirmMeasure>();
            Add<updateCounts>(updateDevice, p => OnMeasureDone());
            Add<confirmHighVoltage>();
            Add<updateTurboPumpStatus>(updateDevice);
            Add<confirmForvacuumLevel>();
        }
    }
}
