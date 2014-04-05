using System;

namespace Flavor.Common.Messaging {
    public class CommandReceivedEventArgs<T>: EventArgs
        where T: struct, IConvertible, IComparable {
        public byte Code { get; private set; }
        public ServicePacket<T> Command { get; private set; }
        public CommandReceivedEventArgs(byte code, ServicePacket<T> command) {
            Command = command;
            Code = code;
        }
    }
}
