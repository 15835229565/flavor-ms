using System;
using Flavor.Common.Messaging.Almazov.Commands;
using Flavor.Common.Settings;
using System.Timers;

namespace Flavor.Common.Messaging.Almazov {
    class AlmazovRealizer: RealizerWithAutomatedStatusChecks<CommandCode> {
        public AlmazovRealizer(PortLevel port, byte attempts, Generator<double> interval)
            : this(new AlexProtocol(port), attempts, interval) { }
        AlmazovRealizer(ISyncAsyncProtocol<CommandCode> protocol, byte attempts, Generator<double> interval)
            : this(protocol, new MessageQueueWithAutomatedStatusChecks<CommandCode>(protocol,
                attempts,
                new StatusRequestGenerator(//new TICStatusRequest(),
                    new VacuumStatusRequest(),
                    new AllVoltagesRequest(),
                    //new CPUStatusRequest(),
                    new HighVoltagePermittedStatusRequest(),
                    new OperationBlockRequest(null)
                    ),
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
            //return new Valve1Request(on);
            throw new NotImplementedException();
        }

        public override void SetSettings() {
            // TODO: proper data from config
            var co = Config.CommonOptions;
            toSend.Enqueue(new SetIonizationVoltageRequest(co.iVoltage));
            toSend.Enqueue(new SetEmissionCurrentRequest(co.eCurrent));
            toSend.Enqueue(new SetF1VoltageRequest(co.fV1));
            toSend.Enqueue(new SetF2VoltageRequest(co.fV2));
            toSend.Enqueue(new SetD1VoltageRequest(co.d1V));
            // now is turned off
            //toSend.Enqueue(new SetD2VoltageRequest(co.d2V));
            toSend.Enqueue(new SetD3VoltageRequest(co.d3V));
            // and check
            toSend.Enqueue(new GetD1VoltageRequest());
        }
        [Obsolete]
        protected override UserRequest<CommandCode> Settings() {
            //Cannot autosend sequence
            throw new NotImplementedException();
        }

        public override void SetMeasureStep(ushort step) {
            // TODO: proper data from config
            var co = Config.CommonOptions;
            toSend.Enqueue(new ParentScanVoltageSetRequest(co.parentScanVoltage(step)));
            toSend.Enqueue(new MainScanVoltageSetRequest(co.scanVoltageNew(step)));
            toSend.Enqueue(new CapacitorVoltageSetRequest(co.capVoltage(step)));
        }
        [Obsolete]
        protected override UserRequest<CommandCode> MeasureStep(ushort step) {
            //Cannot autosend sequence
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
                //SetSettings();
            }, AutoSend<GetHeaterVoltageRequest>/*fake request to workaround SPI misfunction (last in queue)*/);
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
            Add<VacuumStatusReply>(updateDevice, p => {
                if (onTheFly) {
                    OnFirstStatus(() => { });
                    onTheFly = false;
                }
            });

            Add<Valve1Reply>(updateDevice);
            
            Add<Valve2Reply>(updateDevice);
            Add<Valve3Reply>(updateDevice);
            Add<MicroPumpReply>(updateDevice);

            Add<AllVoltagesReply>(updateDevice);
            
            Add<IonSourceSetReply>();
            Add<DetectorSetReply>();
            Add<InletSetReply>();

            Add<GetD1VoltageReply>(AutoSend<GetD2VoltageRequest>);
            Add<GetD2VoltageReply>(AutoSend<GetD3VoltageRequest>);
            Add<GetD3VoltageReply>(AutoSend<GetIonizationVoltageRequest>);
            Add<GetIonizationVoltageReply>(AutoSend<GetEmissionCurrentRequest>);
            Add<GetEmissionCurrentReply>(AutoSend<GetF1VoltageRequest>);
            Add<GetF1VoltageReply>(AutoSend<GetF2VoltageRequest>);
            Add<GetF2VoltageReply>(AutoSend<GetInletVoltageRequest>);
            Add<GetInletVoltageReply>(AutoSend<GetHeaterVoltageRequest>);
            Add<GetHeaterVoltageReply>(p => OnMeasurePreconfigured());

            Add<ScanVoltageSetReply>();
            Add<CapacitorVoltageSetReply>(p => OnMeasureSend((t1, t2) => {
                // temporary solution for delayed measure request;
                if (t1 > 0) {
                    Timer delayed = new Timer(t1);
                    delayed.AutoReset = false;
                    var request = SendMeasureRequest.Form(t2);
                    delayed.Elapsed += new ElapsedEventHandler((o, args) => toSend.Enqueue(request));
                    delayed.Start();
                } else {
                    toSend.Enqueue(SendMeasureRequest.Form(t2));
                }
            }));

            Add<SendMeasureReply>();
            Add<DelayedMeasureReply>();
            Add<CountsReply>(updateDevice, p => OnMeasureDone());
        }
        // TODO: move to realizer ctor as extra action on measure step
        public void CheckStepVoltages() {
            //toSend.Enqueue(new ParentScanVoltageGetRequest());
        }
        public void SendInletSettings(bool? useCapillary, params ushort[] ps) {
            if (useCapillary.HasValue) {
                if (useCapillary.Value) {
                    toSend.Enqueue(new MicroPumpRequest(true));
                    toSend.Enqueue(new SetInletVoltageRequest(0));
                    toSend.Enqueue(new SetHeaterVoltageRequest(0));
                    toSend.Enqueue(new Valve3Request(true));
                    toSend.Enqueue(new Valve2Request(true));
                } else {
                    toSend.Enqueue(new MicroPumpRequest(true));
                    toSend.Enqueue(new Valve2Request(false));
                    toSend.Enqueue(new Valve3Request(false));
                    toSend.Enqueue(new SetInletVoltageRequest(ps[0]));
                    toSend.Enqueue(new SetHeaterVoltageRequest(ps[1]));
                    toSend.Enqueue(new GetInletVoltageRequest());
                }
            } else {
                toSend.Enqueue(new Valve2Request(false));
                toSend.Enqueue(new Valve3Request(false));
                toSend.Enqueue(new SetInletVoltageRequest(0));
                toSend.Enqueue(new SetHeaterVoltageRequest(0));
                toSend.Enqueue(new MicroPumpRequest(false));
            }
        }
    }
}
