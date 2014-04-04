using System;
using System.Collections.Generic;
using System.Linq;

namespace Flavor.Common.Messaging {
    abstract class Protocol<T>: IProtocol<T> {
        private readonly IByteDispatcher byteDispatcher;
        protected Protocol(IByteDispatcher byteDispatcher) {
            this.byteDispatcher = byteDispatcher;
            byteDispatcher.PackageReceived += Parse;
            byteDispatcher.Log += OnLog;
        }
        protected abstract void Parse(object sender, ByteArrayEventArgs e);
        #region IProtocol<T> Members
        public event EventHandler<CommandReceivedEventArgs<T>> CommandReceived;
        protected virtual void OnCommandReceived(T code, ServicePacket<T> command) {
            if (CommandReceived != null)
                CommandReceived(this, new CommandReceivedEventArgs<T>(code, command));
        }
        public event EventHandler<ErrorCommandEventArgs> ErrorCommand;
        protected virtual void OnErrorCommand(byte[] data, string message) {
            if (ErrorCommand != null)
                ErrorCommand(this, new ErrorCommandEventArgs(data, message));
        }
        public virtual void Send(IEnumerable<byte> message) {
            byteDispatcher.Transmit(message.ToList());
        }
        #endregion
        #region IDisposable Members
        public virtual void Dispose() {
            byteDispatcher.Dispose();
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
