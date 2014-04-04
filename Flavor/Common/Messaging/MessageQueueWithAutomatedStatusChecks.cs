using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor.Common.Messaging {
    class MessageQueueWithAutomatedStatusChecks<T>: MessageQueue<T> {
        private System.Timers.Timer statusCheckTimer;

        private readonly object locker = new object();

        private bool isRareMode = false;
        internal bool IsRareMode {
            get { return isRareMode; }
            set {
                lock (locker) {
                    if (value != isRareMode) {
                        isRareMode = value;
                        statusCheckTimer.Enabled = false;
                        statusCheckTimer.Elapsed -= StatusCheckTime_Elapsed;
                        statusCheckTimer.Close();
                        Init();
                    }
                }
            }
        }

        private void Init() {
            bool rare = IsRareMode;
            // TODO: move this hard-coded defaults to Config
            double checkInterval = rare ? 10000 : 500;
            statusCheckTimer = new System.Timers.Timer(checkInterval);
            toggleOperation();
            statusCheckTimer.Elapsed += StatusCheckTime_Elapsed;

            //requestEnumerator = StatusRequestEnumerator2();
            requestEnumerator = StatusRequestEnumerator();
        }

        private bool operating = false;
        internal bool IsOperating {
            get { return operating; }
            set {
                lock (locker) {
                    if (value != operating) {
                        operating = value;
                        toggleOperation();
                    }
                }
            }
        }
        private void toggleOperation() {
            statusCheckTimer.Enabled = operating;
        }

        private readonly ServicePacket<T>.UserRequest statusCheck;
        private readonly ServicePacket<T>.UserRequest vacuumCheck;
        [Obsolete]
        internal MessageQueueWithAutomatedStatusChecks(IProtocol<T> protocol, ServicePacket<T>.UserRequest statusCheck, ServicePacket<T>.UserRequest vacuumCheck)
            : base(protocol) {
            this.statusCheck = statusCheck;
            this.vacuumCheck = vacuumCheck;
            Init();
        }
        private readonly IEnumerable<KeyValuePair<T, CommandRecord<T>>> statusCheckers;
        internal MessageQueueWithAutomatedStatusChecks(IProtocol<T> protocol, IEnumerable<KeyValuePair<T, CommandRecord<T>>> statusCheckers)
            : base(protocol) {
            this.statusCheckers = statusCheckers;
            Init();
        }
        private IEnumerator<ServicePacket<T>.UserRequest> StatusRequestEnumerator2() {
            for ( ; ; ) {
                foreach (var x in statusCheckers) {
                    var y = x.Value as StatusUserCommandRecord<T>;
                    if (y == null)
                        continue;
                    for (int i = 0; ; ++i) {
                        if (i < (isRareMode ? y.Priority : y.Priority))//!!!
                            yield return y.Send();
                    }
                }
            }
        }
        [Obsolete]
        private IEnumerator<ServicePacket<T>.UserRequest> StatusRequestEnumerator() {
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
        private IEnumerator<ServicePacket<T>.UserRequest> requestEnumerator;
        private ServicePacket<T>.UserRequest StatusRequest() {
            requestEnumerator.MoveNext();
            return requestEnumerator.Current;
        }
        private void StatusCheckTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            var request = StatusRequest();
            // to prevent queue overflow
            if (Contains(request))
                return;
            Enqueue(request);
        }
    }
}
