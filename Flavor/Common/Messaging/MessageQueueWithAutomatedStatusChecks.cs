using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor.Common.Messaging {
    class MessageQueueWithAutomatedStatusChecks<T>: MessageQueue<T>
        where T: struct, IConvertible, IComparable {
        System.Timers.Timer statusCheckTimer;

        readonly object locker = new object();

        bool isRareMode = false;
        public bool IsRareMode {
            get { return isRareMode; }
            set {
                lock (locker) {
                    if (value != isRareMode) {
                        isRareMode = value;
                        bool state = statusCheckTimer.Enabled;
                        IsOperating = false;
                        statusCheckTimer.Elapsed -= StatusCheckTime_Elapsed;
                        statusCheckTimer.Close();
                        OnInit(state);
                    }
                }
            }
        }

        protected override void OnInit(bool start) {
            bool rare = IsRareMode;
            // TODO: move this hard-coded defaults to Config
            double checkInterval = rare ? 10000 : 500;
            statusCheckTimer = new System.Timers.Timer(checkInterval);
            statusCheckTimer.Elapsed += StatusCheckTime_Elapsed;
            IsOperating = start;

            requestEnumerator = StatusRequestEnumerator();
        }

        bool operating = false;
        public bool IsOperating {
            set {
                lock (locker) {
                    if (value != operating) {
                        operating = value;
                        statusCheckTimer.Enabled = operating;
                    }
                }
            }
        }
        protected override void OnClear() {
            IsOperating = false;
        }

        readonly ServicePacket<T>.UserRequest statusCheck;
        readonly ServicePacket<T>.UserRequest vacuumCheck;
        public MessageQueueWithAutomatedStatusChecks(IProtocol<T> protocol, ServicePacket<T>.UserRequest statusCheck, ServicePacket<T>.UserRequest vacuumCheck)
            : base(protocol) {
            this.statusCheck = statusCheck;
            this.vacuumCheck = vacuumCheck;
            //OnInit();
        }
        IEnumerator<ServicePacket<T>.UserRequest> StatusRequestEnumerator() {
            bool rare = IsRareMode;
            // TODO: move this hard-coded defaults to Config
            int factor = rare ? 3 : 5;
            for (int i = 0; ; ) {
                if (i == 0)
                    yield return vacuumCheck;
                else 
                    yield return statusCheck;
                ++i;
                i %= factor;
            }
        }
        IEnumerator<ServicePacket<T>.UserRequest> requestEnumerator;
        ServicePacket<T>.UserRequest StatusRequest() {
            requestEnumerator.MoveNext();
            return requestEnumerator.Current;
        }
        void StatusCheckTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            var request = StatusRequest();
            // to prevent queue overflow
            if (Contains(request))
                return;
            Enqueue(request);
        }
    }
}
