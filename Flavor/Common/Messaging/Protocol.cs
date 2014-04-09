using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    abstract class Protocol<T>: IProtocol<T>
        where T: struct, IConvertible, IComparable {
        protected Protocol() {
            this.dictionary = GetDictionary();
        }
        readonly CommandDictionary<T> dictionary;
        protected abstract CommandDictionary<T> GetDictionary();
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
            if (!dictionary.ContainsKey(code)) {
                OnErrorCommand(rawCommand, "Неверная команда");
                return;
            }
            var record = dictionary[code];
            if (!record.CheckLength(length)) {
                OnErrorCommand(rawCommand, "Неверная длина");
                return;
            }
            record.Act(rawCommand);
        }
        protected abstract bool CheckPassed(IList<byte> rawCommand);
        #region IProtocol<T> Members
        public event EventHandler<CommandReceivedEventArgs<T, ServicePacket<T>>> CommandReceived;
        protected virtual void OnCommandReceived(byte code, ServicePacket<T> command) {
            CommandReceived.Raise(this, new CommandReceivedEventArgs<T, ServicePacket<T>>(code, command));
        }
        public event EventHandler<ErrorCommandEventArgs> ErrorCommand;
        protected virtual void OnErrorCommand(IList<byte> data, string message) {
            ErrorCommand.Raise(this, new ErrorCommandEventArgs(data, message));
        }
        #endregion
        #region IDisposable Members
        public abstract void Dispose();
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
