using System;

namespace Flavor.Common.Messaging {
    public abstract class ServicePacket<T>
        where T: struct, IConvertible, IComparable { }
    public abstract class Sync<T>: ServicePacket<T>, IId<T>
        where T: struct, IConvertible, IComparable {
        public abstract T Id { get; }
    }
    public abstract class SyncReply<T>: Sync<T>
        where T: struct, IConvertible, IComparable { }
    public abstract class UserRequest<T>: Sync<T>, ISend
        where T: struct, IConvertible, IComparable {
        public abstract byte[] Data { get; }
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