using System;
using System.Collections.Generic;
using System.Collections;

namespace Flavor.Common.Messaging {
    class MessageQueue<T>: ILog
        where T: struct, IConvertible, IComparable {
        byte Try = 0;

        Queue<UserRequest<T>> queue = new Queue<UserRequest<T>>();
        // TODO: configurable
        readonly System.Timers.Timer sendTimer = new System.Timers.Timer(1000);

        object syncObj = null;
        object SyncRoot {
            get {
                return syncObj == null ? (syncObj = (queue as ICollection).SyncRoot) : syncObj; 
            }
        }
        public event EventHandler Undo;
        protected virtual void OnUndo() {
            if (Undo != null)
                Undo(this, EventArgs.Empty);
        }
        IProtocol<T> protocol = null;
        public MessageQueue(IProtocol<T> protocol) {
            this.protocol = protocol;
            protocol.ErrorCommand += (s, e) => {
                // TODO: more accurate
                OnLog(e.Message);
            };
            ConsoleWriter.Subscribe(protocol);
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
        public UserRequest<T> Dequeue() {
            UserRequest<T> packet = null;
            lock (SyncRoot) {
                dequeueToSendInsideLock(ref packet);
                StopSending();
            }
            trySend();
            return packet;
        }
        public UserRequest<T> Peek(Sync<T> command) {
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
                if (!packet.Id.Equals(command.Id)) {
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
                if (packet != null)
                    OnLog(string.Format("Device not answering to {0}", packet.Id));
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
