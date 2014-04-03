using System;
using System.Collections.Generic;
using UserRequest = Flavor.Common.Messaging.Commands.UserRequest;
using SyncReply = Flavor.Common.Messaging.Commands.SyncReply;
using System.Collections;

namespace Flavor.Common.Messaging {
    class MessageQueue: ILog {
        private byte Try = 0;
        private bool statusToSend = false;
        private bool turboToSend = false;

        private Queue<UserRequest> ToSend = new Queue<UserRequest>();
        private System.Timers.Timer sendTimer;

        private object syncObj = null;
        private object SyncRoot {
            get {
                return syncObj == null ? (syncObj = (ToSend as ICollection).SyncRoot) : syncObj; 
            }
        }
        public event EventHandler Undo;
        protected virtual void OnUndo() {
            if (Undo != null)
                Undo(this, EventArgs.Empty);
        }
        private IProtocol protocol = null;
        internal MessageQueue(IProtocol protocol) {
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
                statusToSend = false;
                turboToSend = false;
                ToSend.Clear();
            }
        }
        internal void AddToSend(UserRequest Command)//Enqueue
        {
            lock (SyncRoot) {
                if (Command is UserRequest.requestStatus) {
                    if (statusToSend) {
                        return;
                    }
                    statusToSend = true;
                } else if (Command is UserRequest.getTurboPumpStatus) {
                    if (turboToSend) {
                        return;
                    }
                    turboToSend = true;
                }

                ToSend.Enqueue(Command);
                trySend();
            }
        }
        internal UserRequest Dequeue() {
            UserRequest packet = null;
            lock (SyncRoot) {
                dequeueToSendInsideLock(ref packet);
                StopSending();
            }
            trySend();
            return packet;
        }
        internal UserRequest Peek(SyncReply command) {
            UserRequest packet = null;
            lock (SyncRoot) {
                if (ToSend.Count == 0) {
                    OnLog(string.Format("Received {0}. While waiting for nothing.", command));
                    return null;
                }
                peekToSendInsideLock(ref packet);
                if (packet == null) {
                    dequeueToSendInsideLock(ref packet);
                    return null;
                }
                if (packet.Id != ((SyncReply)command).Id) {
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
            lock (SyncRoot) {
                if (!statusToSend) {
                    ToSend.Enqueue(new UserRequest.requestStatus());
                    statusToSend = true;
                    trySend();
                }
            }
        }
        protected void addTurboPumpStatusRequest() {
            lock (SyncRoot) {
                if (!turboToSend) {
                    ToSend.Enqueue(new UserRequest.getTurboPumpStatus());
                    turboToSend = true;
                }
                trySend();
            }
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
                UserRequest packet = null;
                if (ToSend.Count == 0)
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
                while (ToSend.Count > 0) {
                    UserRequest packet = null;
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

        private bool dequeueToSendInsideLock(ref UserRequest packet) {
            try {
                packet = ToSend.Dequeue();
                if (packet is UserRequest.requestStatus) {
                    statusToSend = false;
                } else if (packet is UserRequest.getTurboPumpStatus) {
                    turboToSend = false;
                }
                return true;
            } catch (InvalidOperationException) {
                OnLog("Error. Dequeue failed though someting must be in queue.");
            }
            try {
                ToSend.Clear();
                statusToSend = false;
                turboToSend = false;
                return false;
            } catch (InvalidOperationException) {
                OnLog("Error. Cannot clear message queue.");
            }
            OnLog("Message queue recreation.");
            ToSend = new Queue<UserRequest>();
            statusToSend = false;
            turboToSend = false;
            return false;
        }
        private void peekToSendInsideLock(ref UserRequest packet) {
            try {
                packet = ToSend.Peek();
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
