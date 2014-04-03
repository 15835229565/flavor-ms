namespace Flavor.Common.Messaging {
    public abstract class ServicePacket<T> {
        //nesting is BAD!
        public abstract class Sync: ServicePacket<T>, IId<T> {
            public abstract T Id { get; }
        }
        public abstract class SyncError: Sync { }
        public abstract class Async: ServicePacket<T> { }
        public abstract class AsyncError: Async {
            public abstract string Message { get; }
        }
        public abstract class UserRequest: ServicePacket<T>, IId<T>, ISend {
            public abstract T Id { get; }
            public abstract byte[] Data { get; }
        }
    }
}