using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    interface IProtocol<T>: IDisposable, ILog
        where T: struct, IConvertible, IComparable {
        [Obsolete]
        event EventHandler<CommandReceivedEventArgs<T, ServicePacket<T>>> CommandReceived;
        event EventHandler<ErrorCommandEventArgs> ErrorCommand;
    }
    interface ISyncProtocol<T>: IProtocol<T>, IDisposable, ILog
        where T: struct, IConvertible, IComparable {
        event EventHandler<CommandReceivedEventArgs<T, SyncReply<T>>> SyncCommandReceived;
        event EventHandler<CommandReceivedEventArgs<T, SyncError<T>>> SyncErrorReceived;
        void Send(IList<byte> message);
    }
    interface IAsyncProtocol<T>: IProtocol<T>, IDisposable, ILog
        where T: struct, IConvertible, IComparable {
        event EventHandler<CommandReceivedEventArgs<T, Async<T>>> AsyncCommandReceived;
        event EventHandler<CommandReceivedEventArgs<T, AsyncError<T>>> AsyncErrorReceived;
    }
    interface ISyncAsyncProtocol<T>: IProtocol<T>, ISyncProtocol<T>, IAsyncProtocol<T>, IDisposable, ILog
        where T: struct, IConvertible, IComparable {
    }
}
