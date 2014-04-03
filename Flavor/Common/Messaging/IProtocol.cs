using System;

namespace Flavor.Common.Messaging {
    interface IProtocol: IDisposable, ILog {
        event EventHandler<CommandReceivedEventArgs> CommandReceived;
        event EventHandler<ErrorCommandEventArgs> ErrorCommand;
        void Send(byte[] message);
    }
}
