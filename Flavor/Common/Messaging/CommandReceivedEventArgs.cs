using System;

namespace Flavor.Common.Messaging {
    public class CommandReceivedEventArgs<T, T1>: EventArgs
        where T: struct, IConvertible, IComparable
        where T1: ServicePacket<T> {
        public byte Code { get; private set; }
        public T1 Command { get; private set; }
        public CommandReceivedEventArgs(byte code, T1 command) {
            Command = command;
            Code = code;
        }
    }
}
