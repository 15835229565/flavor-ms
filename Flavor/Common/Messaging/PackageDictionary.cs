using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    class PackageDictionary<T>: Dictionary<byte, PackageRecord<T>>
        where T: struct, IConvertible, IComparable { }
}
