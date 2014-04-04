using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    interface IByteDispatcher: IDisposable, ILog {
        event EventHandler<ByteArrayEventArgs> PackageReceived;
        void Transmit(ICollection<byte> pack);
    }
}
