using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    public class ByteArrayEventArgs: EventArgs {
        public IList<byte> Data { get; private set; }
        public ByteArrayEventArgs(IList<byte> data) {
            Data = data;
        }
    }
}
