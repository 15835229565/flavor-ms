using System;
using System.Collections.Generic;
using System.Linq;

namespace Flavor.Common.Messaging {
    abstract class Protocol<T>: IProtocol<T>
        where T: struct, IConvertible, IComparable {
        readonly IByteDispatcher byteDispatcher;
        protected Protocol(IByteDispatcher byteDispatcher, CommandDictionary<T> dictionary) {
            this.byteDispatcher = byteDispatcher;
            byteDispatcher.PackageReceived += Parse;
            byteDispatcher.Log += OnLog;
            this.dictionary = dictionary;
        }
        readonly CommandDictionary<T> dictionary;
        protected void Parse(object sender, ByteArrayEventArgs e) {
            var rawCommand = e.Data;
            int length = rawCommand.Count;
            if (length < dictionary.MinLength) {
                OnErrorCommand(rawCommand, "Короткий пакет");
                return;
            }
            if (!CheckPassed(rawCommand))
                return;
            byte code = rawCommand[0];
            //T code = (T)rawCommand[0];
            if (!dictionary.ContainsKey(code)) {
                OnErrorCommand(rawCommand, "Неверная команда");
                return;
            }
            var record = dictionary[code];
            if (!record.CheckLength(length)) {
                OnErrorCommand(rawCommand, "Неверная длина");
                return;
            }
            OnCommandReceived(code, record.Parse(rawCommand));
        }
        protected abstract bool CheckPassed(IList<byte> rawCommand);
        #region IProtocol<T> Members
        public event EventHandler<CommandReceivedEventArgs<T>> CommandReceived;
        protected virtual void OnCommandReceived(byte code, ServicePacket<T> command) {
            if (CommandReceived != null)
                CommandReceived(this, new CommandReceivedEventArgs<T>(code, command));
        }
        public event EventHandler<ErrorCommandEventArgs> ErrorCommand;
        protected virtual void OnErrorCommand(IList<byte> data, string message) {
            if (ErrorCommand != null)
                ErrorCommand(this, new ErrorCommandEventArgs(data, message));
        }
        public virtual void Send(IList<byte> message) {
            byteDispatcher.Transmit(message.ToList());
        }
        #endregion
        #region IDisposable Members
        public virtual void Dispose() {
            byteDispatcher.Dispose();
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
