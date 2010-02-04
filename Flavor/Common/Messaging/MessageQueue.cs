using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor
{
    class MessageQueue
    {
        private byte Try = 0;
        private bool statusToSend = false;
        private bool turboToSend = false;

        private Queue<UserRequest> ToSend = new Queue<UserRequest>();
        private System.Timers.Timer SendTimer;
        private System.Timers.ElapsedEventHandler elapsed;

        public MessageQueue(): base()
        {
            elapsed = new System.Timers.ElapsedEventHandler(SendTime_Elapsed);
            SendTimer = new System.Timers.Timer(1000);
            SendTimer.Enabled = false;
        }
        public void Clear()
        {
            lock (ToSend)
            {
                ToSend.Clear();
                statusToSend = false;
                turboToSend = false;
                lock (SendTimer)
                {
                    if (SendTimer.Enabled)
                    {
                        StopSending();
                    }
                }
            }
        }
        public void AddToSend(UserRequest Command)//Enqueue
        {
            lock (ToSend)
            {
                if (Command is requestStatus)
                {
                    if (statusToSend)
                    {
                        return;
                    }
                    statusToSend = true;
                }
                else if (Command is getTurboPumpStatus)
                {
                    if (turboToSend)
                    {
                        return;
                    }
                    turboToSend = true;
                }

                ToSend.Enqueue(Command);
                trySend();
            }
        }
        public UserRequest Dequeue()
        {
            UserRequest packet = null;
            lock (ToSend)
            {
                dequeueToSendInsideLock(ref packet);
                StopSending();
            }
            trySend();
            return packet;
        }
        public UserRequest Peek(SyncReply command)
        {
            UserRequest packet = null;
            lock (ToSend)
            {
                if (ToSend.Count == 0)
                {
                    Console.WriteLine("Received {0}. While waiting for nothing.", command);
                    return null;
                }
                peekToSendInsideLock(ref packet);
                if (packet == null)
                {
                    dequeueToSendInsideLock(ref packet);
                    return null;
                }
                if (packet.Id != ((SyncReply)command).Id)
                {
                    Console.WriteLine("Received {0}. While waiting for {1}.", command, packet);
                    return null;
                }
                StopSending();
                if (!dequeueToSendInsideLock(ref packet))
                    return null;
            }
            trySend();
            return packet;
        }

        protected void addStatusRequest()
        {
            lock (ToSend)
            {
                if (!statusToSend)
                {
                    ToSend.Enqueue(new requestStatus());
                    statusToSend = true;
                    trySend();
                }
            }
        }
        protected void addTurboPumpStatusRequest()
        {
            lock (ToSend)
            {
                if (!turboToSend)
                {
                    ToSend.Enqueue(new getTurboPumpStatus());
                    turboToSend = true;
                }
                trySend();
            }
        }

        private void StartSending()
        {
            lock (SendTimer)
            {
                if (SendTimer.Enabled || Try != 0)
                {
                    Console.WriteLine("Error. SendTimer already started.");
                    return;
                }
                Try = 1;
                SendTimer.Elapsed += elapsed;
                SendTimer.Enabled = true;
            }
        }
        private void StopSending()
        {
            lock (SendTimer)
            {
                if (!SendTimer.Enabled || Try == 0)
                {
                    Console.WriteLine("Error. SendTimer already stopped.");
                    return;
                }
                SendTimer.Elapsed -= elapsed;
                SendTimer.Enabled = false;
                Try = 0;
            }
        }

        private void SendTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (ToSend)
            {
                lock (SendTimer)
                {
                    ++Try;
                    if (Try <= Config.Try)
                    {
                        Send();
                        return;
                    }
                    StopSending();
                }
                UserRequest packet = null;
                if (ToSend.Count == 0)
                    Console.WriteLine("Error. Packet queue is empty but sending counter is not zero.");
                else
                {
                    if (dequeueToSendInsideLock(ref packet))
                    {
                        if (packet == null)
                            Console.WriteLine("Error. In message queue null found.");
                    }
                }
                if (packet != null)
                    Console.WriteLine("Device not answering to {0}", packet.Id);
                if (Commander.pState != Commander.pStatePrev)
                    Commander.pState = Commander.pStatePrev;
            }
        }

        private void trySend()
        {
            lock (SendTimer)
            {
                if (!SendTimer.Enabled)
                {
                    Send();
                }
            }
        }
        private void Send()
        {
            lock (ToSend)
            {
                while (ToSend.Count > 0)
                {
                    UserRequest packet = null;
                    peekToSendInsideLock(ref packet);
                    if (packet != null)
                    {
                        if (0 == Try)
                        {
                            StartSending();
                        }
                        packet.Send();
                        break;
                    }
                    if (dequeueToSendInsideLock(ref packet))
                        continue;
                    break;
                }
            }
        }

        private bool dequeueToSendInsideLock(ref UserRequest packet)
        {
            try
            {
                packet = ToSend.Dequeue();
                if (packet is requestStatus)
                {
                    statusToSend = false;
                }
                else if (packet is getTurboPumpStatus)
                {
                    turboToSend = false;
                }
                return true;
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Error. Dequeue failed though someting must be in queue.");
            }
            try
            {
                ToSend.Clear();
                statusToSend = false;
                turboToSend = false;
                return false;
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Error. Cannot clear message queue.");
            }
            Console.WriteLine("Message queue recreation.");
            ToSend = new Queue<UserRequest>();
            statusToSend = false;
            turboToSend = false;
            return false;
        }
        private void peekToSendInsideLock(ref UserRequest packet)
        {
            try
            {
                packet = ToSend.Peek();
                if (packet == null)
                    Console.WriteLine("Error. In message queue null found.");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Error. Peek failed though someting must be in queue.");
            }
        }
    }
}
