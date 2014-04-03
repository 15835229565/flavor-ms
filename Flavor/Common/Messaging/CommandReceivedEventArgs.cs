using System;

namespace Flavor.Common.Messaging {
    public class CommandReceivedEventArgs<T>: EventArgs {
        public ServicePacket<T> Command { get; private set; }
        public CommandReceivedEventArgs(ServicePacket<T> command) {
            Command = command;
        }
    }
}
