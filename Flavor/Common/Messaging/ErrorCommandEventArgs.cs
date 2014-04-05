using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    public class ErrorCommandEventArgs: ByteArrayEventArgs {
        public string Message { get; private set; }
        public ErrorCommandEventArgs(IList<byte> data, string message)
            : base(data) {
            Message = message;
        }
    }
}
