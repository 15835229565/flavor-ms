using System;

namespace Flavor.Common.Messaging {
    class PackageRecord<T>
        where T: struct, IConvertible, IComparable {
        //public Generator<UserRequest<T>> Send { get; private set; }
        public Action<ServicePacket<T>> Act { get; private set; }
        public PackageRecord(Action<ServicePacket<T>> act/*, Generator<UserRequest<T>> send*/) {
            Act = act;
            //Send = send;
        }
    }
}