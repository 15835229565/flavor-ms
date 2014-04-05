using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    interface IProtocol<T>: IDisposable, ILog
        where T: struct, IConvertible, IComparable {
        event EventHandler<CommandReceivedEventArgs<T>> CommandReceived;
        event EventHandler<ErrorCommandEventArgs> ErrorCommand;
        void Send(IList<byte> message);
    }
}
