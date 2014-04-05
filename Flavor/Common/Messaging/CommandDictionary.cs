using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    class CommandDictionary<T>: Dictionary<byte, CommandRecord<T>>
        where T: struct, IConvertible, IComparable {
        public int MinLength { get; set; }
    }
}
