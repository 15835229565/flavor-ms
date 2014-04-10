using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    public abstract class ServicePacket<T>
        where T: struct, IConvertible, IComparable { }
    public abstract class Sync<T>: ServicePacket<T>, IId<T>
        where T: struct, IConvertible, IComparable {
        public abstract T Id { get; }
        public override bool Equals(object other) {
            if (other is Sync<T>)
                return Id.Equals((other as Sync<T>).Id);
            return false;
        }
        public override int GetHashCode() {
            return base.GetHashCode() + 13 * Id.GetHashCode();
        }
    }
    public abstract class SyncReply<T>: Sync<T>
        where T: struct, IConvertible, IComparable { }
    public abstract class UserRequest<T>: Sync<T>, ISend
        where T: struct, IConvertible, IComparable {
        public abstract IList<byte> Data { get; }
    }
    public abstract class SyncError<T>: Sync<T>
        where T: struct, IConvertible, IComparable { }
    public abstract class Async<T>: ServicePacket<T>
        where T: struct, IConvertible, IComparable { }
    public abstract class AsyncError<T>: Async<T>
        where T: struct, IConvertible, IComparable {
        public abstract string Message { get; }
    }
}