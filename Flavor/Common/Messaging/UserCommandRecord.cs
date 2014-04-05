using System;

namespace Flavor.Common.Messaging {
    class UserCommandRecord<T>: CommandRecord<T>
        where T: struct, IConvertible, IComparable {
        public delegate ServicePacket<T>.UserRequest Sender();
        public Sender Send { get; private set; }
        public UserCommandRecord(Predicate<int> checkLength, Parser parse, Sender send)
            : base(checkLength, parse) {
            Send = send;
        }
    }
}
