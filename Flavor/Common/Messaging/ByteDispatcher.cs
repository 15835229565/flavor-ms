using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flavor.Common.Messaging {
    abstract class ByteDispatcher: IByteDispatcher {
        private readonly PortLevel port;
        private readonly bool singleByteDispatching;
        protected ByteDispatcher(PortLevel port, bool singleByteDispatching) {
            this.port = port;
            this.singleByteDispatching = singleByteDispatching;
            if (singleByteDispatching)
                port.ByteReceived += PortByteReceived;
            else
                port.BytesReceived += PortBytesReceived;
        }
        private void PortBytesReceived(object sender, PortLevel.BytesReceivedEventArgs e) {
            DispatchBytes(e.Bytes, e.Count);
        }
        private void DispatchBytes(byte[] bytes, int count) {
            for (int i = 0; i < count; ++i) {
                DispatchByte(bytes[i]);
            }
        }
        private void PortByteReceived(object sender, PortLevel.ByteReceivedEventArgs e) {
            DispatchByte(e.Byte);
        }
        protected abstract void DispatchByte(byte data);
        #region IByteDispatcher Members
        public event EventHandler<ByteArrayEventArgs> PackageReceived;
        protected virtual void OnPackageReceived(byte[] data) {
            if (PackageReceived != null)
                PackageReceived(this, new ByteArrayEventArgs(data));
        }
        public virtual void Transmit(ICollection<byte> pack) {
            port.Send(pack.ToArray());
            var sb = new StringBuilder("[out]");
            foreach (byte b in pack) {
                sb.Append((char)b);
            }
            OnLog(sb.ToString());
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
