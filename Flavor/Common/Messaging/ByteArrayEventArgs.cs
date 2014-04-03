using System;

namespace Flavor.Common.Messaging {
    public class ByteArrayEventArgs: EventArgs {
        public byte[] Data { get; private set; }
        public ByteArrayEventArgs(byte[] data) {
            Data = data;
        }
    }
}
