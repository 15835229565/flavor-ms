using System;
using Flavor.Common.Messaging.Almazov.Commands;

namespace Flavor.Common.Messaging.Almazov {
    class AlmazovRealizer: RealizerWithAutomatedStatusChecks<CommandCode> {
        public AlmazovRealizer(PortLevel port, Generator<double> interval)
            : this(new AlexProtocol(port), interval) { }
        AlmazovRealizer(ISyncAsyncProtocol<CommandCode> protocol, Generator<double> interval)
            : this(protocol, new MessageQueueWithAutomatedStatusChecks<CommandCode>(protocol,
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

        protected override UserRequest<CommandCode> Settings() {
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
            Add<LAMEvent>(/*updateDevice, */p => OnSystemReady());
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
            Add<OperationBlockReply>(updateDevice/*, p => OnOperationBlock(true)*/);
            // TODO: proper command detection!
            Add<TICStatusReply>(updateDevice, p => {
                if (onTheFly) {
                    OnFirstStatus(() => { });
                    onTheFly = false;
                }
            });
            Add<Valve1Reply>(updateDevice);
        }
    }
}
