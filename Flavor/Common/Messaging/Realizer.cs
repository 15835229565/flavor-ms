using System;

namespace Flavor.Common.Messaging {
    abstract class Realizer<T>: ILog, IAsyncReplyReceived
        where T: struct, IConvertible, IComparable {
        public event EventHandler<EventArgs<bool>> SystemDown;
        protected virtual void OnSystemDown(bool error) {
            SystemDown.Raise(this, new EventArgs<bool>(error));
        }
        public event EventHandler<EventArgs> SystemReady;
        protected virtual void OnSystemReady() {
            SystemReady.Raise(this, EventArgs.Empty);
        }
        public event EventHandler<EventArgs<bool>> OperationBlock;
        protected virtual void OnOperationBlock(bool on) {
            OperationBlock.Raise(this, new EventArgs<bool>(on));
        }
        public event EventHandler<EventArgs<bool>> OperationToggle;
        protected virtual void OnOperationToggle(bool on) {
            OperationToggle.Raise(this, new EventArgs<bool>(on));
        }
        public event EventHandler<EventArgs> MeasurePreconfigured;
        protected virtual void OnMeasurePreconfigured() {
            MeasurePreconfigured.Raise(this, EventArgs.Empty);
        }
        public event EventHandler<EventArgs<Action<ushort, ushort>>> MeasureSend;
        protected virtual void OnMeasureSend(Action<ushort, ushort> doMeasure) {
            MeasureSend.Raise(this, new EventArgs<Action<ushort, ushort>>(doMeasure));
        }
        public event EventHandler<EventArgs> MeasureDone;
        protected virtual void OnMeasureDone() {
            MeasureDone.Raise(this, EventArgs.Empty);
        }
        public event EventHandler<EventArgs<Action>> FirstStatus;
        protected virtual void OnFirstStatus(Action onTheFly) {
            FirstStatus.Raise(this, new EventArgs<Action>(onTheFly));
        }

        readonly IAsyncProtocol<T> protocol;
        readonly MessageQueue<T> toSend;
        readonly PackageDictionary<T> dictionary;
        protected Realizer(IAsyncProtocol<T> protocol, MessageQueue<T> queue) {
            this.protocol = protocol;
            toSend = queue;
            dictionary = GetDictionary();
        }
        protected abstract PackageDictionary<T> GetDictionary();
        public abstract void SetOperationBlock(bool block);
        public abstract void SetOperationToggle(bool on);
        public abstract void SetSettings();
        public abstract void SetMeasureStep(ushort step);
        protected bool Realize<T1>(object sender, CommandReceivedEventArgs<T, T1> e)
            where T1: ServicePacket<T> {
            byte code = e.Code;
            if (!dictionary.ContainsKey(code)) {
                // Strange error
                return false;
            }
            var actor = dictionary[code];
            if (actor != null && actor.Act != null)
                actor.Act(e.Command);
            return true;
        }
        [Obsolete]
        public event ProgramEventHandler ProgramStateChangeRequested;
        protected virtual void OnProgramStateChangeRequested(ProgramStates state) {
            if (ProgramStateChangeRequested != null)
                ProgramStateChangeRequested(state);
        }
        public virtual void Connect(Action undo) {
            toSend.Clear();
            ConsoleWriter.Subscribe(toSend);
            toSend.Undo += (s, e) => undo();
            toSend.CommandApproved += (s, e) => Realize<Sync<T>>(s, e);
            protocol.AsyncCommandReceived += (s, e) => Realize<Async<T>>(s, e);
            protocol.AsyncErrorReceived += protocol_AsyncErrorReceived;
            //protocol.CommandReceived += (s, e) => { };
            protocol.ErrorCommand += (s, e) => OnLog(e.Message);
        }
        public virtual void Disconnect(Action undo) {
            protocol.AsyncCommandReceived -= (s, e) => Realize<Async<T>>(s, e);
            protocol.AsyncErrorReceived += protocol_AsyncErrorReceived;
            //protocol.CommandReceived -= (s, e) => { };
            protocol.ErrorCommand -= (s, e) => OnLog(e.Message);
            toSend.Clear();
            toSend.Undo -= (s, e) => undo();
            toSend.CommandApproved -= (s, e) => Realize<Sync<T>>(s, e);
            ConsoleWriter.Unsubscribe(toSend);
        }
        private void protocol_AsyncErrorReceived(object sender, CommandReceivedEventArgs<T, AsyncError<T>> e) {
            // TODO: check behaviour!
            if (Realize<AsyncError<T>>(sender, e)) {
                string message = string.Format("Device says: {0}", e.Command.Message);
                OnSystemDown(true);
                OnAsyncReplyReceived(message);
                // TODO: subscribe in Config for event
                Config.logCrash(message);
            }
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
        [Obsolete]
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
