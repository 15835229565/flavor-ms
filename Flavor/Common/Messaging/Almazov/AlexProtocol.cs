using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flavor.Common.Messaging.Almazov {
    internal class AlexProtocol: IProtocol<AlexProtocol.CommandCode> {
        internal enum CommandCode { }
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
