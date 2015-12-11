using System;
using System.Collections.Generic;
using System.Linq;

namespace Flavor.Common.Messaging {
    abstract class ByteDispatcher: IByteDispatcher {
        readonly PortLevel port;
        readonly bool singleByteDispatching;
        protected ByteDispatcher(PortLevel port, bool singleByteDispatching) {
            this.port = port;
            this.singleByteDispatching = singleByteDispatching;
            if (singleByteDispatching)
                port.ByteReceived += PortByteReceived;
            else
                port.BytesReceived += PortBytesReceived;
        }
        void PortBytesReceived(object sender, PortLevel.BytesReceivedEventArgs e) {
            DispatchBytes(e.Bytes, e.Count);
        }
        void DispatchBytes(byte[] bytes, int count) {
            for (int i = 0; i < count; ++i) {
                DispatchByte(bytes[i]);
            }
        }
        void PortByteReceived(object sender, PortLevel.ByteReceivedEventArgs e) {
            DispatchByte(e.Byte);
        }
        protected abstract void DispatchByte(byte data);
        #region IByteDispatcher Members
        public event EventHandler<ByteArrayEventArgs> PackageReceived;
        protected virtual void OnPackageReceived(IList<byte> data) {
            PackageReceived.Raise(this, new ByteArrayEventArgs(data));
        }
        public virtual void Transmit(ICollection<byte> pack) {
            port.Send(pack.ToArray());
        }
        #endregion
        #region IDisposable Members
        public virtual void Dispose() {
            if (singleByteDispatching)
                port.ByteReceived -= PortByteReceived;
            else
                port.BytesReceived -= PortBytesReceived;
        }
        #endregion
        #region ILog Members
        public event MessageHandler Log;
        protected virtual void OnLog(string message) {
            if (Log != null)
                Log(message);
        }
        #endregion
    }
}
