using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    class PackageDictionary<T>: Dictionary<ServicePacket<T>, PackageRecord<T>>
        where T: struct, IConvertible, IComparable {
        public PackageDictionary()
            : base(new SymmetricEqualityComparer<ServicePacket<T>>()) { }
        class SymmetricEqualityComparer<T1>: IEqualityComparer<T1> {
            #region IEqualityComparer<T1> Members
            public bool Equals(T1 x, T1 y) {
                if (x == null || y == null)
                    return false;
                return x.Equals(y) && y.Equals(x);
            }
            public int GetHashCode(T1 obj) {
                return obj.GetHashCode();
            }
            #endregion
        }
    }
}
