using System;
using Flavor.Common.Messaging.Commands;

namespace Flavor.Common.Messaging {
    class CommandReceivedEventArgs: EventArgs {
        public ServicePacket Command { get; private set; }
        public CommandReceivedEventArgs(ServicePacket command) {
            Command = command;
        }
    }
}
