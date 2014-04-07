using System;

namespace Flavor.Common.Messaging {
    abstract class Realizer<T>: ILog, IAsyncReplyReceived
        where T: struct, IConvertible, IComparable {
        readonly PackageDictionary<T> dictionary;
        protected Realizer(MessageQueue<T> queue) {
            toSend = queue;
            dictionary = GetDictionary();
        }
        protected abstract PackageDictionary<T> GetDictionary();
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
            var actor = dictionary[code];
            if (actor != null)
                actor.Act(e.Command);
        }
        public event ProgramEventHandler ProgramStateChangeRequested;
        protected virtual void OnProgramStateChangeRequested(ProgramStates state) {
            if (ProgramStateChangeRequested != null)
                ProgramStateChangeRequested(state);
        }
        public virtual void Connect() {
            toSend.Clear();
            ConsoleWriter.Subscribe(toSend);
            //toSend.Undo += (s, e) => setProgramStateWithoutUndo(pStatePrev);
        }
        public virtual void Disconnect() {
            toSend.Clear();
            //toSend.Undo -= (s, e) => setProgramStateWithoutUndo(pStatePrev);
            ConsoleWriter.Unsubscribe(toSend);
        }
        #region ILog Members
        public event MessageHandler Log;
        protected virtual void OnLog(string msg) {
            var evt = Log;
            if (evt != null)
                evt(msg);
        }
        #endregion
        #region IAsyncReplyReceived Members
        public event MessageHandler AsyncReplyReceived;
        protected virtual void OnAsyncReplyReceived(string msg) {
            var evt = AsyncReplyReceived;
            if (evt != null)
                evt(msg);
            OnLog(msg);
        }
        #endregion
    }
}
