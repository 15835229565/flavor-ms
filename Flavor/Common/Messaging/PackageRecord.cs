using System;

namespace Flavor.Common.Messaging {
    class PackageRecord<T>
        where T: struct, IConvertible, IComparable {
        public delegate UserRequest<T> Sender();
        public Sender Send { get; private set; }
        public PackageRecord(Sender send) {
            Send = send;
        }
    }
}