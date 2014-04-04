using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    interface IProtocol<T>: IDisposable, ILog {
        event EventHandler<CommandReceivedEventArgs<T>> CommandReceived;
        event EventHandler<ErrorCommandEventArgs> ErrorCommand;
        void Send(IEnumerable<byte> message);
    }
}
