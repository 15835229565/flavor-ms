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

        protected readonly IProtocol<T> protocol;
        readonly PackageDictionary<T> dictionary;
        protected Realizer(IProtocol<T> protocol) {
            this.protocol = protocol;
            toSend = GetQueue();
            dictionary = GetDictionary();
        }
        protected abstract MessageQueue<T> GetQueue();
        protected abstract PackageDictionary<T> GetDictionary();
        protected readonly MessageQueue<T> toSend;
        public abstract void SetOperationBlock(bool block);
        public abstract void SetOperationToggle(bool on);
        public abstract void SetSettings();
        public abstract void SetMeasureStep(ushort step);
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
            protocol.CommandReceived += Realize;
        }
        public virtual void Disconnect(Action undo) {
            protocol.CommandReceived -= Realize;
            toSend.Clear();
            toSend.Undo -= (s, e) => undo();
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
