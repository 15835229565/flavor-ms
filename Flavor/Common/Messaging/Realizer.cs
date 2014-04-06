using System;

namespace Flavor.Common.Messaging {
    abstract class Realizer<T>: ILog, IAsyncReplyReceived, IConnectionActions
        where T: struct, IConvertible, IComparable {
        readonly PackageDictionary<T> dictionary;
        IProtocol<T> protocol;
        protected Realizer(IProtocol<T> protocol, PackageDictionary<T> dictionary, MessageQueue<T> queue) {
            this.dictionary = dictionary;
            this.protocol = protocol;
            // here?
            ConsoleWriter.Subscribe(this);
            ConsoleWriter.Subscribe(protocol);
            protocol.ErrorCommand += (s, e) => {
                // TODO: more accurate
                OnLog(e.Message);
            };
            toSend = queue;
        }
        readonly MessageQueue<T> toSend;
        protected void Enqueue(UserRequest<T> command) {
            toSend.Enqueue(command);
        }
        protected virtual void Realize(object sender, CommandReceivedEventArgs<T> e) {
            byte code = e.Code;
            if (!dictionary.ContainsKey(code)) {
                // Strange error
                return;
            }
            dictionary[code].Act(e.Command);
        }
        public event ProgramEventHandler ProgramStateChangeRequested;
        protected virtual void OnProgramStateChangeRequested(ProgramStates state) {
            if (ProgramStateChangeRequested != null)
                ProgramStateChangeRequested(state);
        }
        #region IConnectionActions Members
        public virtual void Connect() {
            toSend.Clear();
            ConsoleWriter.Subscribe(toSend);
            //toSend.Undo += (s, e) => setProgramStateWithoutUndo(pStatePrev);
        }
        public virtual void Disconnect() {
            ConsoleWriter.Unsubscribe(toSend);
            //toSend.Undo -= (s, e) => setProgramStateWithoutUndo(pStatePrev);
            toSend.Clear();
        }
        #endregion
        #region ILog Members
        public event MessageHandler Log;
        protected virtual void OnLog(string msg) {
            // TODO: lock here?
            if (Log != null)
                Log(msg);
        }
        #endregion
        #region IAsyncReplyReceived Members
        public event MessageHandler AsyncReplyReceived;
        protected virtual void OnAsyncReplyReceived(string msg) {
            // TODO: lock here?
            if (AsyncReplyReceived != null)
                AsyncReplyReceived(msg);
            OnLog(msg);
        }
        #endregion
    }
}
