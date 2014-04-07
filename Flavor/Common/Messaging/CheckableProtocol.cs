using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    abstract class CheckableProtocol<T>: Protocol<T>
        where T: struct, IConvertible, IComparable {
        protected CheckableProtocol(IByteDispatcher byteDispatcher)
            : base(byteDispatcher) { }
        #region IProtocol Members
        public override void Send(IList<byte> message) {
            base.Send(buildPackBody(message, ComputeCS(message)));
        }
        #endregion
        protected override bool CheckPassed(IList<byte> rawCommand) {
            if (!CheckCS(rawCommand)) {
                OnErrorCommand(rawCommand, "Неверная контрольная сумма");
                return false;
            }
            return true;
        }
        protected abstract byte ComputeCS(IList<byte> data);
        protected bool CheckCS(IList<byte> data) {
            return true ^ Convert.ToBoolean(ComputeCS(data));
        }
        protected abstract IList<byte> buildPackBody(IList<byte> data, byte checksum);
    }
}
