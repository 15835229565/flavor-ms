using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flavor.Common.Messaging.Almazov {
    class AlmazovRealizer: Realizer<CommandCode> {
        readonly MessageQueueWithAutomatedStatusChecks<CommandCode> toSend;
        public AlmazovRealizer(PortLevel port, Generator<int> factor, Generator<double> interval)
            : this(new AlexProtocol(port), factor, interval) { }
        AlmazovRealizer(ISyncAsyncProtocol<CommandCode> protocol, Generator<int> factor, Generator<double> interval)
            : this(protocol, new MessageQueueWithAutomatedStatusChecks<CommandCode>(protocol,
                new StatusRequestGenerator(null, null, factor),
                interval)) { }
        AlmazovRealizer(IAsyncProtocol<CommandCode> protocol, MessageQueueWithAutomatedStatusChecks<CommandCode> queue)
            : base(protocol, queue) {
            toSend = queue;
        }
        class StatusRequestGenerator: IStatusRequestGenerator<CommandCode> {
            int f;
            readonly UserRequest<CommandCode> statusCheck, vacuumCheck;
            readonly Generator<int> factor;
            public UserRequest<CommandCode> Next {
                get {
                    UserRequest<CommandCode> res;
                    throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        protected override UserRequest<CommandCode> OperationOnOff(bool on) {
            throw new NotImplementedException();
        }

        protected override UserRequest<CommandCode> Settings() {
            throw new NotImplementedException();
        }

        protected override UserRequest<CommandCode> MeasureStep(ushort step) {
            throw new NotImplementedException();
        }

        bool onTheFly = true;
        protected override PackageDictionary<CommandCode> GetDictionary() {
            var d = new PackageDictionary<CommandCode>();
            Action<CommandCode, Action<ServicePacket<CommandCode>>> add = (code, action) => d[(byte)code] = new PackageRecord<CommandCode>(action);
            //async error
            add(CommandCode.LAM_CriticalError, null);
            //async
            add(CommandCode.LAM_Event, null);
            //sync error
            add(CommandCode.Sync_Error, null);
            //sync
            add(CommandCode.CPU_Status, null/*updateDeviceAction*/);
            throw new NotImplementedException();
            return d;
        }
    }
}
