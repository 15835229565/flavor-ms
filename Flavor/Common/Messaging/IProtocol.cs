using System;

namespace Flavor.Common.Messaging {
    interface IProtocol<T>: IDisposable, ILog {
        event EventHandler<CommandReceivedEventArgs<T>> CommandReceived;
        event EventHandler<ErrorCommandEventArgs> ErrorCommand;
        void Send(byte[] message);
    }
}
