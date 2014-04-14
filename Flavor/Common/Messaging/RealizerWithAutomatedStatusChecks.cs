using System;

namespace Flavor.Common.Messaging {
    abstract class RealizerWithAutomatedStatusChecks<T>: Realizer<T>
        where T: struct, IConvertible, IComparable {
        protected readonly MessageQueueWithAutomatedStatusChecks<T> toSend;
        protected RealizerWithAutomatedStatusChecks(IAsyncProtocol<T> protocol, MessageQueueWithAutomatedStatusChecks<T> queue)
            : base(protocol, queue) {
            toSend = queue;
        }
        public override void Connect() {
            base.Connect();
            toSend.Start();
        }
        public override void Disconnect() {
            toSend.Stop();
            base.Disconnect();
        }
        public override void Reset() {
            toSend.Stop();
            toSend.Start();
        }
    }
}
