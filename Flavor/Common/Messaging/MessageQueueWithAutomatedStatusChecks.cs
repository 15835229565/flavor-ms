using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor.Common.Messaging
{
    class MessageQueueWithAutomatedStatusChecks: MessageQueue
    {
        private System.Timers.Timer DeviceStatusCheckTimer;
        private System.Timers.Timer TurboPumpCheckTimer;

        private System.Timers.ElapsedEventHandler statusElapsed;
        private System.Timers.ElapsedEventHandler turboElapsed;

        private object locker = new object();

        private bool isRareMode = false;
        public bool IsRareMode
        {
            get { return isRareMode; }
            set
            {
                lock (locker)
                {
                    if (value != isRareMode)
                    {
                        isRareMode = value;
                        stopTimers();
                        toggleRareMode();
                    }
                }
            }
        }

        private void stopTimers()
        {
            if (operating)
            {
                DeviceStatusCheckTimer.Enabled = false;
                TurboPumpCheckTimer.Enabled = false;
            }
        }
        private void toggleRareMode()
        {
            double deviceCheckInterval = isRareMode? 10000: 500;
            double turboCheckInterval = isRareMode ? 20000 : 2000;
            
            DeviceStatusCheckTimer = new System.Timers.Timer(deviceCheckInterval);
            TurboPumpCheckTimer = new System.Timers.Timer(turboCheckInterval);

            toggleOperation();

            DeviceStatusCheckTimer.Elapsed += statusElapsed;
            TurboPumpCheckTimer.Elapsed += turboElapsed;
        }

        private bool operating = false;
        public bool IsOperating
        {
            get { return operating; }
            set
            {
                lock (locker)
                {
                    if (value != operating)
                    {
                        operating = value;
                        toggleOperation();
                    }
                }
            }
        }
        private void toggleOperation()
        {
            DeviceStatusCheckTimer.Enabled = operating;
            TurboPumpCheckTimer.Enabled = operating;
        }

        public MessageQueueWithAutomatedStatusChecks()
            : base()
        {
            statusElapsed = new System.Timers.ElapsedEventHandler(StatusCheckTime_Elapsed);
            turboElapsed = new System.Timers.ElapsedEventHandler(TurboPumpCheckTime_Elapsed);

            lock (locker)
            {
                toggleRareMode();
            }
        }

        private void StatusCheckTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            addStatusRequest();
        }
        private void TurboPumpCheckTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            addTurboPumpStatusRequest();
        }
    }
}
