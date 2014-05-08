using System;

namespace Flavor.Common.Messaging {
    class PackageRecord<T>
        where T: struct, IConvertible, IComparable {
        public Action<ServicePacket<T>> Act { get; private set; }
        public PackageRecord(params Action<ServicePacket<T>>[] actions) {
            // TODO: check for null action list
            foreach (var action in actions) {
                Act += action;
            }
        }
    }
}