using System;
using System.Timers;
using Flavor.Common.Settings;

namespace Flavor.Common.Messaging.Almazov {
    using Commands;
    class AlmazovRealizer: RealizerWithAutomatedStatusChecks<CommandCode> {
        public AlmazovRealizer(PortLevel port, byte attempts, Func<double> interval)
            : this(new AlexProtocol(port), attempts, interval) { }
        AlmazovRealizer(ISyncAsyncProtocol<CommandCode> protocol, byte attempts, Func<double> interval)
            : this(protocol, new MessageQueueWithAutomatedStatusChecks<CommandCode>(protocol,
                attempts,
                new StatusRequestGenerator(//new TICStatusRequest(),
                    new VacuumStatusRequest(),
                    new AllVoltagesRequest()//,
                    //new CPUStatusRequest(),
                    //new HighVoltagePermittedStatusRequest()//,
                    //new OperationBlockRequest(null)
                    ),
                interval)) { }
        AlmazovRealizer(IAsyncProtocol<CommandCode> protocol, MessageQueueWithAutomatedStatusChecks<CommandCode> queue)
            : base(protocol, queue) { }
        class StatusRequestGenerator: IStatusRequestGenerator<CommandCode> {
            int i = 0;
            readonly int _f;
            readonly UserRequest<CommandCode>[] _checkCommands;
            public UserRequest<CommandCode> Next {
                get {
                    var res = _checkCommands[i];
                    ++i;
                    i %= _f;
                    return res;
                }
            }
            public void Reset() { }
            public StatusRequestGenerator(params UserRequest<CommandCode>[] checkCommands) {
                _checkCommands = checkCommands;
                _f = checkCommands.Length;
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
            //toSend.Enqueue(new GetD1VoltageRequest());
            toSend.Enqueue(new GetHeaterVoltageRequest());//fake request to workaround SPI misfunction or start measure (last in queue)
        }
        [Obsolete]
        protected override UserRequest<CommandCode> Settings() {
            //Cannot autosend sequence
            throw new NotImplementedException();
        }
        ushort _prevStep = ushort.MaxValue;
        public override void SetMeasureStep(ushort step) {
            // TODO: proper data from config
            var co = Config.CommonOptions;
            // TODO: move logic to measure mode
            if (step != _prevStep) {
                toSend.Enqueue(new ParentScanVoltageSetRequest(co.parentScanVoltage(step)));
                toSend.Enqueue(new MainScanVoltageSetRequest(co.scanVoltageNew(step)));
                toSend.Enqueue(new CapacitorVoltageSetRequest(co.capVoltage(step)));
                _prevStep = step;
            } else {
                // measure request in case of the same step is sent instead of step packages
                SendMeasure();
            }
        }
        [Obsolete]
        protected override UserRequest<CommandCode> MeasureStep(ushort step) {
            //Cannot autosend sequence
            throw new NotImplementedException();
        }
        void SendMeasure() {
            OnMeasureSend((t1, t2) => {
                // temporary solution for delayed measure request;
                var request = SendMeasureRequest.Form(t2);
                if (t1 > 0) {
                    var delayed = new Timer(t1) { AutoReset = false };
                    delayed.Elapsed += (o, args) => toSend.Enqueue(request);
                    delayed.Start();
                } else {
                    toSend.Enqueue(request);
                }
            });
        }
        bool onTheFly = true;
        protected override void PopulateDictionary(PackageDictionary<CommandCode> d) {
            //async error
            Add<LAMCriticalError>();
            Add<LAMInternalError>();
            //async
            Add<RTCMeasureEndLAM>(AutoSend<CountsRequest>);
            Add<SPIConfDoneLAM>(updateDevice, p => {
                OnOperationBlock(false);//maybe duplicates event after update device
                SetSettings();
                //AutoSend<GetHeaterVoltageRequest>(p);//fake request to workaround SPI misfunction (last in queue)
                //set ScanVoltage to minimal limit
                //toSend.Enqueue(new ParentScanVoltageSetRequest(0));
                //toSend.Enqueue(new MainScanVoltageSetRequest(0));
                //toSend.Enqueue(new CapacitorVoltageSetRequest(0));
                SetMeasureStep(0);//set ScanVoltage to low limit
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
            Add<CapacitorVoltageSetReply>(p => SendMeasure());

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
                toSend.Enqueue(new MicroPumpRequest(true));
                if (useCapillary.Value) {
                    toSend.Enqueue(new SetInletVoltageRequest(0));
                    toSend.Enqueue(new SetHeaterVoltageRequest(ps.Length == 0 ? (ushort)0 : ps[0]));
                    toSend.Enqueue(new Valve3Request(true));
                    toSend.Enqueue(new Valve2Request(true));
                } else {
                    toSend.Enqueue(new Valve2Request(false));
                    toSend.Enqueue(new Valve3Request(false));
                    toSend.Enqueue(new SetInletVoltageRequest(ps[0]));
                    toSend.Enqueue(new SetHeaterVoltageRequest(ps.Length == 1 ? (ushort)0 : ps[1]));
                    toSend.Enqueue(new GetInletVoltageRequest());
                }
            } else {
                toSend.Enqueue(new Valve2Request(false));
                toSend.Enqueue(new Valve3Request(false));
                toSend.Enqueue(new SetInletVoltageRequest(0));
                toSend.Enqueue(new SetHeaterVoltageRequest(ps.Length == 0 ? (ushort)0 : ps[0]));
                toSend.Enqueue(new MicroPumpRequest(false));
            }
        }
    }
}
