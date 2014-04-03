using System;

namespace Flavor.Common.Messaging {
    class ErrorCommandEventArgs: ByteArrayEventArgs {
        public string Message { get; private set; }
        public ErrorCommandEventArgs(byte[] data, string message)
            : base(data) {
            Message = message;
        }
    }
}
