using System;
using System.Collections.Generic;
using System.Linq;

namespace Flavor.Common.Messaging {
    abstract class SyncCheckableProtocol<T>: CheckableProtocol<T>, ISyncProtocol<T>
        where T: struct, IConvertible, IComparable {
        readonly IByteDispatcher byteDispatcher;
        protected SyncCheckableProtocol(IByteDispatcher byteDispatcher)
            : base() {
            this.byteDispatcher = byteDispatcher;
            byteDispatcher.PackageReceived += Parse;
            byteDispatcher.Log += OnLog;
        }
        public event EventHandler<CommandReceivedEventArgs<T, SyncReply<T>>> SyncCommandReceived;
        protected virtual void OnSyncCommandReceived(byte code, SyncReply<T> command) {
            OnCommandReceived(code, command);
            SyncCommandReceived.Raise(this, new CommandReceivedEventArgs<T, SyncReply<T>>(code, command));
        }
        public event EventHandler<CommandReceivedEventArgs<T, SyncError<T>>> SyncErrorReceived;
        protected virtual void OnSyncErrorReceived(byte code, SyncError<T> command) {
            OnCommandReceived(code, command);
            SyncErrorReceived.Raise(this, new CommandReceivedEventArgs<T, SyncError<T>>(code, command));
        }
        public virtual void Send(IList<byte> message) {
            byteDispatcher.Transmit(buildPackBody(message, ComputeCS(message)).ToList());
        }
        protected abstract IList<byte> buildPackBody(IList<byte> data, byte checksum);
        #region IDisposable Members
        public override void Dispose() {
            byteDispatcher.Dispose();
        }
        #endregion
    }
}
