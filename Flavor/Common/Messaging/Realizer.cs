using System;

namespace Flavor.Common.Messaging {
    abstract class Realizer<T>: IRealizer, IAsyncReplyReceived
        where T: struct, IConvertible, IComparable {
        public event EventHandler<EventArgs<bool>> SystemDown;
        protected virtual void OnSystemDown(bool error) {
            SystemDown.Raise(this, new EventArgs<bool>(error));
        }
        public event EventHandler<EventArgs> SystemReady;
        protected virtual void OnSystemReady() {
            SystemReady.Raise(this, EventArgs.Empty);
        }
        public event EventHandler<EventArgs<IUpdateDevice>> UpdateDevice;
        protected virtual void OnUpdateDevice(IUpdateDevice packet) {
            UpdateDevice.Raise(this, new EventArgs<IUpdateDevice>(packet));
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
        public event EventHandler Undo;
        protected virtual void OnUndo() {
            Undo.Raise(this, EventArgs.Empty);
        }

        readonly IAsyncProtocol<T> protocol;
        readonly MessageQueue<T> toSend;
        readonly PackageDictionary<T> dictionary;
        protected Realizer(IAsyncProtocol<T> protocol, MessageQueue<T> queue) {
            this.protocol = protocol;
            toSend = queue;
            realizeSync = (s, e) => Realize<Sync<T>>(s, e);
            realizeAsync = (s, e) => Realize<Async<T>>(s, e);

            this.dictionary = new PackageDictionary<T>();
            PopulateDictionary(dictionary);
        }
        protected void updateDevice(ServicePacket<T> p) {
            OnUpdateDevice((IUpdateDevice)p);
        }
        protected void Add<T1>(params Action<ServicePacket<T>>[] actions)
            where T1: ServicePacket<T>, new() {
            this.dictionary[new T1()] = new PackageRecord<T>(actions);
        }
        protected void AutoSend<T1>(ServicePacket<T> p)
            where T1: UserRequest<T>, new() {
            toSend.Enqueue(new T1());
        }
        protected abstract void PopulateDictionary(PackageDictionary<T> d);

        void SendUndoable(UserRequest<T> packet) {
            EventHandler<EventArgs<UserRequest<T>>> undo = new EventHandler<EventArgs<UserRequest<T>>>(delegate { });
            EventHandler<CommandReceivedEventArgs<T, Sync<T>>> discard = new EventHandler<CommandReceivedEventArgs<T, Sync<T>>>(delegate { });
            undo += (s, e) => {
                if (Equals(e.Value, packet)) {
                    toSend.NotAnsweringTo -= undo;
                    toSend.CommandApproved -= discard;
                    OnUndo();
                }
            };
            discard += (s, e) => {
                if (Equals(e.Command, packet)) {
                    toSend.NotAnsweringTo -= undo;
                    toSend.CommandApproved -= discard;
                }
            };
            toSend.NotAnsweringTo += undo;
            toSend.CommandApproved += discard;
            toSend.Enqueue(packet);
        }
        public void SetOperationBlock(bool block) {
            SendUndoable(Block(block));
        }
        protected abstract UserRequest<T> Block(bool block);
        public void SetOperationToggle(bool on) {
            SendUndoable(OperationOnOff(on));
        }
        protected abstract UserRequest<T> OperationOnOff(bool on);
        public void SetSettings() {
            toSend.Enqueue(Settings());
            //SendUndoable(Settings());
        }
        protected abstract UserRequest<T> Settings();
        public void SetMeasureStep(ushort step) {
            toSend.Enqueue(MeasureStep(step));
            //SendUndoable(MeasureStep(step));
        }
        protected abstract UserRequest<T> MeasureStep(ushort step);
        protected bool Realize<T1>(object sender, CommandReceivedEventArgs<T, T1> e)
            where T1: ServicePacket<T> {
            //byte code = e.Code;
            var command = e.Command;
            if (!dictionary.ContainsKey(command)) {
                // Strange error
                return false;
            }
            var actor = dictionary[command];
            if (actor == null || actor.Act == null) {
                return false;
            }
            actor.Act(command);
            return true;
        }
        [Obsolete]
        public event ProgramEventHandler ProgramStateChangeRequested;
        protected virtual void OnProgramStateChangeRequested(ProgramStates state) {
            if (ProgramStateChangeRequested != null)
                ProgramStateChangeRequested(state);
        }
        readonly EventHandler<CommandReceivedEventArgs<T, Sync<T>>> realizeSync;
        readonly EventHandler<CommandReceivedEventArgs<T, Async<T>>> realizeAsync;
        public virtual void Connect() {
            toSend.Clear();
            ConsoleWriter.Subscribe(toSend);
            toSend.CommandApproved += realizeSync;
            protocol.AsyncCommandReceived += realizeAsync;
            protocol.AsyncErrorReceived += realizeAsyncError;
            //protocol.CommandReceived +=;
            protocol.ErrorCommand += OnLog;
        }
        public virtual void Disconnect() {
            protocol.AsyncCommandReceived -= realizeAsync;
            protocol.AsyncErrorReceived -= realizeAsyncError;
            //protocol.CommandReceived -=;
            protocol.ErrorCommand -= OnLog;
            toSend.Clear();
            toSend.CommandApproved -= realizeSync;
            ConsoleWriter.Unsubscribe(toSend);
        }
        public abstract void Reset();
        void realizeAsyncError(object sender, CommandReceivedEventArgs<T, AsyncError<T>> e) {
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
        void OnLog(object sender, ErrorCommandEventArgs e) {
            var evt = Log;
            if (evt != null)
                evt(e.Message);
        }
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
