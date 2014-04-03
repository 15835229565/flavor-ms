using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flavor.Common.Messaging.Almazov {
    internal class AlexProtocol: IProtocol<AlexProtocol.CommandCode> {
        internal enum CommandCode { }
        private readonly IByteDispatcher byteDispatcher;
        public AlexProtocol(PortLevel port) {
            byteDispatcher = new ByteDispatcher(port);
            byteDispatcher.PackageReceived += Parse;
            byteDispatcher.Log += OnLog;
        }
        private void Parse(object sender, ByteArrayEventArgs e) {
            throw new NotImplementedException();
        }
        private class ByteDispatcher: IByteDispatcher {
            public ByteDispatcher(PortLevel port) {
                throw new NotImplementedException();
            }

            #region IByteDispatcher Members

            public event EventHandler<ByteArrayEventArgs> PackageReceived;

            public void Transmit(byte[] message, byte checksum) {
                throw new NotImplementedException();
            }

            #endregion

            #region IDisposable Members

            public void Dispose() {
                throw new NotImplementedException();
            }

            #endregion

            #region ILog Members

            public event MessageHandler Log;

            #endregion
        }
        #region IProtocol Members
        public event EventHandler<CommandReceivedEventArgs<CommandCode>> CommandReceived;
        protected void OnCommandReceived(ServicePacket<CommandCode> command) {
            if (CommandReceived != null)
                CommandReceived(this, new CommandReceivedEventArgs<CommandCode>(command));
        }
        public event EventHandler<ErrorCommandEventArgs> ErrorCommand;
        protected void OnErrorCommand(byte[] data, string message) {
            if (ErrorCommand != null)
                ErrorCommand(this, new ErrorCommandEventArgs(data, message));
        }
        public void Send(byte[] message) {
            throw new NotImplementedException();
        }

        #endregion
        #region IDisposable Members
        public void Dispose() {
            throw new NotImplementedException();
        }
        #endregion
        #region ILog Members
        public event MessageHandler Log;
        protected virtual void OnLog(string message) {
            if (Log != null)
                Log(message);
        }
        #endregion
    }
}
