using System;

namespace Flavor.Common.Messaging {
    class CommandRecord<T> {
        public Predicate<int> CheckLength { get; private set; }
        public delegate ServicePacket<T> Parser(byte[] rawData);
        public Parser Parse { get; private set; }
        public CommandRecord(Predicate<int> checkLength, Parser parse) {
            CheckLength = checkLength;
            Parse = parse;
        }
    }
}
