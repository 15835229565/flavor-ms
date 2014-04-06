using System;

namespace Flavor.Common.Messaging {
    interface IStatusRequestGenerator<T>
        where T: struct, IConvertible, IComparable {
        UserRequest<T> Next { get; }
        void Reset();
    }
}