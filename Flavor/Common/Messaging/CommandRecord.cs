using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    class CommandRecord<T>
        where T: struct, IConvertible, IComparable {
        public Predicate<int> CheckLength { get; private set; }
        public delegate ServicePacket<T> Parser(IList<byte> rawData);
        public Parser Parse { get; private set; }
        public CommandRecord(Predicate<int> checkLength, Parser parse) {
            CheckLength = checkLength;
            Parse = parse;
        }
    }
}
