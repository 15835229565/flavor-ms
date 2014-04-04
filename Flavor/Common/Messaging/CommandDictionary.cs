using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    class CommandDictionary<T>: Dictionary<T, CommandRecord<T>> {
        public int MinLength { get; set; }
    }
}
