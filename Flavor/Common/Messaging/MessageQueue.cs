using System;
using System.Collections.Generic;
using System.Collections;

namespace Flavor.Common.Messaging {
    class MessageQueue<T>: ILog
        where T: struct, IConvertible, IComparable {
        byte Try = 0;

        Queue<ServicePacket<T>.UserRequest> queue = new Queue<ServicePacket<T>.UserRequest>();
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
            sendTimer.Enabled = false;
            OnInit(false);
        }
        protected virtual void OnInit(bool start) { }
        public void Clear() {
            lock (SyncRoot) {
                OnClear();
                lock (sendTimer) {
                    if (sendTimer.Enabled) {
                        StopSending();
                    }
                }
                queue.Clear();
            }
        }
        protected virtual void OnClear() { }
        public void Enqueue(ServicePacket<T>.UserRequest command)
        {
            lock (SyncRoot) {
                queue.Enqueue(command);
                trySend();
            }
        }
        public ServicePacket<T>.UserRequest Dequeue() {
            ServicePacket<T>.UserRequest packet = null;
            lock (SyncRoot) {
                dequeueToSendInsideLock(ref packet);
                StopSending();
            }
            trySend();
            return packet;
        }
        public ServicePacket<T>.UserRequest Peek(ServicePacket<T>.Sync command) {
            ServicePacket<T>.UserRequest packet = null;
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
        protected bool Contains(ServicePacket<T>.UserRequest item) {
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
                ServicePacket<T>.UserRequest packet = null;
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
                    ServicePacket<T>.UserRequest packet = null;
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

        bool dequeueToSendInsideLock(ref ServicePacket<T>.UserRequest packet) {
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
            queue = new Queue<ServicePacket<T>.UserRequest>();
            return false;
        }
        void peekToSendInsideLock(ref ServicePacket<T>.UserRequest packet) {
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
