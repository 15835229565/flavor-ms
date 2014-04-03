using System;
using System.Collections.Generic;
using System.Collections;

namespace Flavor.Common.Messaging {
    class MessageQueue<T>: ILog {
        private byte Try = 0;

        private Queue<ServicePacket<T>.UserRequest> toSend = new Queue<ServicePacket<T>.UserRequest>();
        private System.Timers.Timer sendTimer;

        private object syncObj = null;
        private object SyncRoot {
            get {
                return syncObj == null ? (syncObj = (toSend as ICollection).SyncRoot) : syncObj; 
            }
        }
        public event EventHandler Undo;
        protected virtual void OnUndo() {
            if (Undo != null)
                Undo(this, EventArgs.Empty);
        }
        private IProtocol<T> protocol = null;
        internal MessageQueue(IProtocol<T> protocol) {
            this.protocol = protocol;
            sendTimer = new System.Timers.Timer(1000);
            sendTimer.Enabled = false;
        }
        internal void Clear() {
            lock (SyncRoot) {
                lock (sendTimer) {
                    if (sendTimer.Enabled) {
                        StopSending();
                    }
                }
                toSend.Clear();
            }
        }
        internal void AddToSend(ServicePacket<T>.UserRequest command)//Enqueue
        {
            lock (SyncRoot) {
                if (command is IStatusRequest) {
                    if (toSend.Contains(command))
                        return;
                }
                toSend.Enqueue(command);
                trySend();
            }
        }
        internal ServicePacket<T>.UserRequest Dequeue() {
            ServicePacket<T>.UserRequest packet = null;
            lock (SyncRoot) {
                dequeueToSendInsideLock(ref packet);
                StopSending();
            }
            trySend();
            return packet;
        }
        internal ServicePacket<T>.UserRequest Peek(ServicePacket<T>.Sync command) {
            ServicePacket<T>.UserRequest packet = null;
            lock (SyncRoot) {
                if (toSend.Count == 0) {
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

        protected void addStatusRequest() {
            throw new NotImplementedException();
            /*lock (SyncRoot) {
                if (!statusToSend) {
                    toSend.Enqueue(new ServicePacket<T>.User.requestStatus());
                    trySend();
                }
            }*/
        }
        protected void addTurboPumpStatusRequest() {
            throw new NotImplementedException();
            /*lock (SyncRoot) {
                if (!turboToSend) {
                    toSend.Enqueue(new ServicePacket<T>.User.getTurboPumpStatus());
                }
                trySend();
            }*/
        }

        private void StartSending() {
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
        private void StopSending() {
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

        private void SendTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
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
                if (toSend.Count == 0)
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

        private void trySend() {
            lock (sendTimer) {
                if (!sendTimer.Enabled) {
                    Send();
                }
            }
        }
        private void Send() {
            lock (SyncRoot) {
                while (toSend.Count > 0) {
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

        private bool dequeueToSendInsideLock(ref ServicePacket<T>.UserRequest packet) {
            try {
                packet = toSend.Dequeue();
                return true;
            } catch (InvalidOperationException) {
                OnLog("Error. Dequeue failed though someting must be in queue.");
            }
            try {
                toSend.Clear();
                return false;
            } catch (InvalidOperationException) {
                OnLog("Error. Cannot clear message queue.");
            }
            OnLog("Message queue recreation.");
            toSend = new Queue<ServicePacket<T>.UserRequest>();
            return false;
        }
        private void peekToSendInsideLock(ref ServicePacket<T>.UserRequest packet) {
            try {
                packet = toSend.Peek();
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
