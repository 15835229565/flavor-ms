using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    abstract class CheckableProtocol<T>: Protocol<T> {
        protected CheckableProtocol(IByteDispatcher byteDispatcher)
            : base(byteDispatcher) { }
        #region IProtocol Members
        public override void Send(IEnumerable<byte> message) {
            base.Send(buildPackBody(message, ComputeCS(message)));
        }
        #endregion
        protected abstract byte ComputeCS(IEnumerable<byte> data);
        protected bool CheckCS(byte[] data) {
            return true ^ Convert.ToBoolean(ComputeCS(data));
        }
        protected abstract ICollection<byte> buildPackBody(IEnumerable<byte> data, byte checksum);
    }
}
