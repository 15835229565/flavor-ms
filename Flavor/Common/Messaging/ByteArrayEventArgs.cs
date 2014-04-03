using System;

namespace Flavor.Common.Messaging {
    class ByteArrayEventArgs: EventArgs {
        public byte[] Data { get; private set; }
        public ByteArrayEventArgs(byte[] data) {
            Data = data;
        }
    }
}
