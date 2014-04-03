using System;

namespace Flavor.Common.Messaging {
    interface IByteDispatcher: IDisposable, ILog {
        event EventHandler<ByteArrayEventArgs> PackageReceived;
        void Transmit(byte[] message, byte checksum);
    }
}
