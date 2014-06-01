using System;
using Flavor.Common.Messaging.Almazov.Commands;
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Common.Messaging.Almazov {
    class AlmazovRealizer: RealizerWithAutomatedStatusChecks<CommandCode> {
        public AlmazovRealizer(PortLevel port, byte attempts, Generator<double> interval)
            : this(new AlexProtocol(port), attempts, interval) { }
        AlmazovRealizer(ISyncAsyncProtocol<CommandCode> protocol, byte attempts, Generator<double> interval)
            : this(protocol, new MessageQueueWithAutomatedStatusChecks<CommandCode>(protocol,
                attempts,
                new StatusRequestGenerator(new TICStatusRequest(), new CPUStatusRequest(), new HighVoltagePermittedStatusRequest(), new OperationBlockRequest(null)),
                interval)) { }
        AlmazovRealizer(IAsyncProtocol<CommandCode> protocol, MessageQueueWithAutomatedStatusChecks<CommandCode> queue)
            : base(protocol, queue) { }
        class StatusRequestGenerator: IStatusRequestGenerator<CommandCode> {
            int i = 0;
            readonly int f;
            readonly UserRequest<CommandCode>[] checkCommands;
            public UserRequest<CommandCode> Next {
                get {
                    UserRequest<CommandCode> res;
                    res = checkCommands[i];
                    ++i;
                    i %= f;
                    return res;
                }
            }
            public void Reset() { }
            public StatusRequestGenerator(params UserRequest<CommandCode>[] checkCommands) {
                this.checkCommands = checkCommands;
                this.f = checkCommands.Length;
                Reset();
            }
        }

        protected override UserRequest<CommandCode> Block(bool block) {
            // TODO: stop status requests for several seconds or until SPI_conf_done is received
            //toSend.Stop();
            return new OperationBlockRequest(block);
        }

        [Obsolete]
        protected override UserRequest<CommandCode> OperationOnOff(bool on) {
            // workaround for detecting Relay1 change
            return new Valve1Request(on);
        }

        public new void SetSettings() {
            // TODO: proper data from config
            var co = Config.CommonOptions;
            toSend.Enqueue(new SetEmissionCurrentRequest(co.eCurrent));
            toSend.Enqueue(new SetIonizationVoltageRequest(co.iVoltage));
            toSend.Enqueue(new SetF1VoltageRequest(co.fV1));
            toSend.Enqueue(new SetF2VoltageRequest(co.fV2));
            toSend.Enqueue(new SetD1VoltageRequest(0));
            toSend.Enqueue(new SetD2VoltageRequest(0));
            toSend.Enqueue(new SetD3VoltageRequest(0));
            //toSend.Enqueue(new SetInletVoltageRequest(0));
            //toSend.Enqueue(new SetHeaterVoltageRequest(0));
            // and check
            toSend.Enqueue(new GetEmissionCurrentRequest());
        }
        [Obsolete]
        protected override UserRequest<CommandCode> Settings() {
            //Cannot autosend sequence
            throw new NotImplementedException();
        }

        protected override UserRequest<CommandCode> MeasureStep(ushort step) {
            throw new NotImplementedException();
        }

        bool onTheFly = true;
        protected override void PopulateDictionary(PackageDictionary<CommandCode> d) {
            //async error
            Add<LAMCriticalError>();
            Add<LAMInternalError>();
            //async
            Add<RTCMeasureEndLAM>(AutoSend<CountsRequest>);
            Add<SPIConfDoneLAM>(updateDevice, p => {
                OnOperationBlock(false);
                //toSend.Enqueue(new sendSVoltage(0));//Set ScanVoltage to low limit
                SetSettings();
            });
            Add<HVEnabledLAM>(updateDevice, p => OnSystemReady());
            Add<HVDisabledLAM>(updateDevice, p => OnSystemDown(true));
            //sync error
            Add<SyncErrorReply>();

            //add(CommandCode.Service_Message, p => {
            //    if (p is LAMEvent && (p as LAMEvent).number == 21) {
            //        OnSystemReady();
            //    }
            //});
            //sync
            Add<CPUStatusReply>(/*updateDevice*/);
            Add<HighVoltagePermittedStatusReply>(updateDevice/*, p => OnSystemReady()*/);
            Add<OperationBlockReply>();
            Add<OperationBlockOnReply>(updateDevice, p => OnOperationBlock(true));
            Add<OperationBlockOffReply>(updateDevice);
            // TODO: proper command detection!
            Add<TICStatusReply>(updateDevice, p => {
                if (onTheFly) {
                    OnFirstStatus(() => { });
                    onTheFly = false;
                }
            });
            Add<Valve1Reply>(updateDevice);

            Add<IonSourceSetReply>();
            Add<DetectorSetReply>();
            Add<InletSetReply>();

            Add<GetEmissionCurrentReply>(AutoSend<GetIonizationVoltageRequest>);
            Add<GetIonizationVoltageReply>(AutoSend<GetF1VoltageRequest>);
            Add<GetF2VoltageReply>(AutoSend<GetF2VoltageRequest>);
            Add<GetF2VoltageReply>(AutoSend<GetD1VoltageRequest>);
            Add<GetD1VoltageReply>(AutoSend<GetD2VoltageRequest>);
            Add<GetD2VoltageReply>(AutoSend<GetD3VoltageRequest>);
            Add<GetD3VoltageReply>(AutoSend<GetInletVoltageRequest>);
            Add<GetInletVoltageReply>(AutoSend<GetHeaterVoltageRequest>);
            Add<GetHeaterVoltageReply>();

            Add<CountsReply>(p => OnMeasureDone());
        }
    }
}
