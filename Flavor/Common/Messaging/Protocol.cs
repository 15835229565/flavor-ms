using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    abstract class Protocol<T>: IProtocol<T>
        where T: struct, IConvertible, IComparable {
        protected Protocol() {
            this.dictionary = GetDictionary();
        }
        readonly CommandDictionary<T> dictionary;
        protected delegate Predicate<int> PredicateGenerator(int value);
        protected delegate T1 PackageGenerator<T1>(IList<byte> rawCommand)
            where T1: ServicePacket<T>;
        protected delegate Action<IList<byte>> CodeAdder(byte code);
        protected CodeAdder Generic<T1>(PackageGenerator<T1> gen, Action<byte, T1> action)
            where T1: ServicePacket<T> {
            return code => (list => action(code, gen(list)));
        }
        protected delegate CodeAdder ActionGenerator<T1>(PackageGenerator<T1> gen)
            where T1: ServicePacket<T>;
        protected readonly PredicateGenerator eq = value => (l => l == value);
        protected readonly PredicateGenerator moreeq = value => (l => l >= value);
        //protected delegate Action<T, Predicate<int>, CodeAdder> AddGenerator(CommandDictionary<T> dictionary);
        //protected readonly AddGenerator addGenerator = d => ((code, predicate, action) =>
        //        d[(byte)code] = new CommandRecord<T>(predicate, action((byte)code)));
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
        protected readonly Processor<IList<byte>> trim = l => {
            // remove command byte at first position
            l.RemoveAt(0);
            return l;
        };
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
