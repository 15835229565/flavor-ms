using System;

namespace Flavor.Common.Messaging {
    class StatusUserCommandRecord<T>: UserCommandRecord<T>
        where T: struct, IConvertible, IComparable {
        public int Priority { get; private set; }
        public StatusUserCommandRecord(Predicate<int> checkLength, Parser parse, Sender send, int priority)
            : base(checkLength, parse, send) {
            Priority = priority;
        }
    }
}
