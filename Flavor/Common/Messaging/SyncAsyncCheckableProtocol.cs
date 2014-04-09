﻿using System;

namespace Flavor.Common.Messaging {
    abstract class SyncAsyncCheckableProtocol<T>: SyncCheckableProtocol<T>, ISyncAsyncProtocol<T>
        where T: struct, IConvertible, IComparable {
        protected SyncAsyncCheckableProtocol(IByteDispatcher byteDispatcher)
            : base(byteDispatcher) { }
        public event EventHandler<CommandReceivedEventArgs<T, Async<T>>> AsyncCommandReceived;
        protected virtual void OnAsyncCommandReceived(byte code, Async<T> command) {
            OnCommandReceived(code, command);
            AsyncCommandReceived.Raise(this, new CommandReceivedEventArgs<T, Async<T>>(code, command));
        }
        public event EventHandler<CommandReceivedEventArgs<T, AsyncError<T>>> AsyncErrorReceived;
        protected virtual void OnAsyncErrorReceived(byte code, AsyncError<T> command) {
            OnCommandReceived(code, command);
            AsyncErrorReceived.Raise(this, new CommandReceivedEventArgs<T, AsyncError<T>>(code, command));
        }
    }
}
