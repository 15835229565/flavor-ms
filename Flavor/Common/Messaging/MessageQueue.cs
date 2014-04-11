using System;
using System.Collections.Generic;
using System.Collections;

namespace Flavor.Common.Messaging {
    class MessageQueue<T>: ILog
        where T: struct, IConvertible, IComparable {
        byte Try = 0;

        Queue<UserRequest<T>> queue = new Queue<UserRequest<T>>();
        // TODO: configurable time interval
        readonly System.Timers.Timer sendTimer = new System.Timers.Timer(1000);

        object syncObj = null;
        object SyncRoot {
            get {
                return syncObj == null ? (syncObj = (queue as ICollection).SyncRoot) : syncObj; 
            }
        }
        public event EventHandler Undo;
        protected virtual void OnUndo() {
            Undo.Raise(this, EventArgs.Empty);
        }
        public event EventHandler<EventArgs<UserRequest<T>>> NotAnsweringTo;
        protected virtual void OnNotAnsweringTo(UserRequest<T> packet) {
            NotAnsweringTo.Raise(this, new EventArgs<UserRequest<T>>(packet));
        }
        public event EventHandler<CommandReceivedEventArgs<T, Sync<T>>> CommandApproved;
        protected virtual void OnCommandApproved(byte code, Sync<T> command) {
            CommandApproved.Raise(this, new CommandReceivedEventArgs<T,Sync<T>>(code, command));
        }
        readonly ISyncProtocol<T> protocol;
        readonly IEqualityComparer<Sync<T>> comparer;
        public MessageQueue(ISyncProtocol<T> protocol)
            : this(protocol, EqualityComparer<Sync<T>>.Default) { }
        public MessageQueue(ISyncProtocol<T> protocol, IEqualityComparer<Sync<T>> comparer) {
            this.protocol = protocol;
            protocol.SyncCommandReceived += (s, e) => {
                var command = e.Command;
                if (null != Peek(command))
                    OnCommandApproved(e.Code, command);
            };
            protocol.SyncErrorReceived += (s, e) => { 
                Dequeue();
                OnCommandApproved(e.Code, e.Command);
            };
            ConsoleWriter.Subscribe(protocol);
            this.comparer = comparer;
            sendTimer.Enabled = false;
        }
        public void Clear() {
            lock (SyncRoot) {
                lock (sendTimer) {
                    if (sendTimer.Enabled) {
                        StopSending();
                    }
                }
                queue.Clear();
            }
        }
        public void Enqueue(UserRequest<T> command)
        {
            lock (SyncRoot) {
                queue.Enqueue(command);
                trySend();
            }
        }
        UserRequest<T> Dequeue() {
            UserRequest<T> packet = null;
            lock (SyncRoot) {
                dequeueToSendInsideLock(ref packet);
                StopSending();
            }
            trySend();
            return packet;
        }
        UserRequest<T> Peek(Sync<T> command) {
            UserRequest<T> packet = null;
            lock (SyncRoot) {
                if (queue.Count == 0) {
                    OnLog(string.Format("Received {0}. While waiting for nothing.", command));
                    return null;
                }
                peekToSendInsideLock(ref packet);
                if (packet == null) {
                    dequeueToSendInsideLock(ref packet);
                    return null;
                }
                if (!comparer.Equals(command, packet)) {
                    OnLog(string.Format("Received {0}. While waiting for {1}.", command, packet));
                    return null;
                }
                StopSending();
                if (!dequeueToSendInsideLock(ref packet))
                    return null;
            }
            trySend();
            return packet;
        }
        protected bool Contains(UserRequest<T> item) {
            return queue.Contains(item);
        }
        void StartSending() {
            lock (sendTimer) {
                if (sendTimer.Enabled || Try != 0) {
                    OnLog("Error. SendTimer already started.");
                    return;
                }
                Try = 1;
                sendTimer.Elapsed += SendTime_Elapsed;
                sendTimer.Enabled = true;
            }
        }
        void StopSending() {
            lock (sendTimer) {
                if (!sendTimer.Enabled || Try == 0) {
                    OnLog("Error. SendTimer already stopped.");
                    return;
                }
                sendTimer.Elapsed -= SendTime_Elapsed;
                sendTimer.Enabled = false;
                Try = 0;
            }
        }

        void SendTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            lock (SyncRoot) {
                lock (sendTimer) {
                    ++Try;
                    if (Try <= Config.Try) {
                        Send();
                        return;
                    }
                    StopSending();
                }
                UserRequest<T> packet = null;
                if (queue.Count == 0)
                    OnLog("Error. Packet queue is empty but sending counter is not zero.");
                else {
                    if (dequeueToSendInsideLock(ref packet)) {
                        if (packet == null)
                            OnLog("Error. In message queue null found.");
                    }
                }
                if (packet != null) {
                    OnNotAnsweringTo(packet);
                    OnLog(string.Format("Device not answering to {0}", packet.Id));
                }
                OnUndo();
            }
        }

        void trySend() {
            lock (sendTimer) {
                if (!sendTimer.Enabled) {
                    Send();
                }
            }
        }
        void Send() {
            lock (SyncRoot) {
                while (queue.Count > 0) {
                    UserRequest<T> packet = null;
                    peekToSendInsideLock(ref packet);
                    if (packet != null) {
                        if (0 == Try) {
                            StartSending();
                        }
                        protocol.Send(packet.Data);
                        break;
                    }
                    if (dequeueToSendInsideLock(ref packet))
                        continue;
                    break;
                }
            }
        }

        bool dequeueToSendInsideLock(ref UserRequest<T> packet) {
            try {
                packet = queue.Dequeue();
                return true;
            } catch (InvalidOperationException) {
                OnLog("Error. Dequeue failed though someting must be in queue.");
            }
            try {
                queue.Clear();
                return false;
            } catch (InvalidOperationException) {
                OnLog("Error. Cannot clear message queue.");
            }
            OnLog("Message queue recreation.");
            queue = new Queue<UserRequest<T>>();
            return false;
        }
        void peekToSendInsideLock(ref UserRequest<T> packet) {
            try {
                packet = queue.Peek();
                if (packet == null)
                    OnLog("Error. In message queue null found.");
            } catch (InvalidOperationException) {
                OnLog("Error. Peek failed though someting must be in queue.");
            }
        }
        #region ILog Members
        public event MessageHandler Log;
        protected virtual void OnLog(string message) {
            if (Log != null)
                Log(message);
        }
        #endregion
    }
}
