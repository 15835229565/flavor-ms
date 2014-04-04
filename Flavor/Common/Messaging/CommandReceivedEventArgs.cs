using System;

namespace Flavor.Common.Messaging {
    public class CommandReceivedEventArgs<T>: EventArgs {
        public T Code { get; private set; }
        public ServicePacket<T> Command { get; private set; }
        public CommandReceivedEventArgs(T code, ServicePacket<T> command) {
            Command = command;
            Code = code;
        }
    }
}
