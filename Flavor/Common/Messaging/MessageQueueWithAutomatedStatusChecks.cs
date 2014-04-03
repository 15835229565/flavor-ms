using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor.Common.Messaging {
    class MessageQueueWithAutomatedStatusChecks<T>: MessageQueue<T> {
        // TODO: use 1 timer
        private System.Timers.Timer DeviceStatusCheckTimer;
        private System.Timers.Timer TurboPumpCheckTimer;

        private readonly object locker = new object();

        private bool isRareMode = false;
        internal bool IsRareMode {
            get { return isRareMode; }
            set {
                lock (locker) {
                    if (value != isRareMode) {
                        isRareMode = value;
                        stopTimers();
                        toggleRareMode();
                    }
                }
            }
        }

        private void stopTimers() {
            if (operating) {
                DeviceStatusCheckTimer.Enabled = false;
                TurboPumpCheckTimer.Enabled = false;
            }
        }
        private void toggleRareMode() {
            DeviceStatusCheckTimer.Elapsed -= StatusCheckTime_Elapsed;
            TurboPumpCheckTimer.Elapsed -= TurboPumpCheckTime_Elapsed;
            DeviceStatusCheckTimer.Close();
            TurboPumpCheckTimer.Close();
            initTimers();
        }
        private void initTimers() {
            // TODO: move this hard-coded defaults to Config
            // TODO: one timer with 1:2 or 1:4 package frequence ratio
            bool rare = isRareMode;
            double deviceCheckInterval = rare ? 10000 : 500;
            double turboCheckInterval = rare ? 20000 : 2000;

            DeviceStatusCheckTimer = new System.Timers.Timer(deviceCheckInterval);
            TurboPumpCheckTimer = new System.Timers.Timer(turboCheckInterval);

            toggleOperation();

            DeviceStatusCheckTimer.Elapsed += StatusCheckTime_Elapsed;
            TurboPumpCheckTimer.Elapsed += TurboPumpCheckTime_Elapsed;
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
            DeviceStatusCheckTimer.Enabled = operating;
            TurboPumpCheckTimer.Enabled = operating;
        }

        internal MessageQueueWithAutomatedStatusChecks(IProtocol<T> protocol)
            : base(protocol) {
            initTimers();
        }

        // TODO: use Enumerator to get next status request package
        private void StatusCheckTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            addStatusRequest();
        }
        private void TurboPumpCheckTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            addTurboPumpStatusRequest();
        }
    }
}
