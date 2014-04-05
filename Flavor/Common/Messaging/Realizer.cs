using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flavor.Common.Messaging {
    abstract class Realizer<T>: ILog, IAsyncReplyReceived, IConnectionActions
        where T: struct, IConvertible, IComparable {
        readonly CommandDictionary<T> dictionary;
        IProtocol<T> protocol;
        protected Realizer(IProtocol<T> protocol, CommandDictionary<T> dictionary, MessageQueue<T> queue) {
            this.dictionary = dictionary;
            this.protocol = protocol;
            ConsoleWriter.Subscribe(this);
            ConsoleWriter.Subscribe(protocol);
            protocol.ErrorCommand += (s, e) => {
                // TODO: more accurate
                OnLog(e.Message);
            };
            toSend = queue;
        }
        protected interface IActor {
            void Act(ServicePacket<T> command);
        }
        readonly MessageQueue<T> toSend;
        protected void Enqueue(ServicePacket<T>.UserRequest command) {
            toSend.Enqueue(command);
        }
        protected virtual void Realize(object sender, CommandReceivedEventArgs<T> e) {
            byte code = e.Code;
            var command = e.Command;
            if (!dictionary.ContainsKey(code)) {
                // Strange error
                return;
            }
            var actor = dictionary[code] as IActor;
            if (actor != null)
                actor.Act(command);
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
