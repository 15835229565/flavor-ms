﻿using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging {
    class MessageQueueWithAutomatedStatusChecks<T>: MessageQueue<T>
        where T: struct, IConvertible, IComparable {
        readonly System.Timers.Timer statusCheckTimer = new System.Timers.Timer();

        readonly object locker = new object();

        public void Start() {
            statusCheckTimer.Interval = interval();
            statusCheckTimer.Start();
        }
        public void Stop() {
            statusCheckTimer.Stop();
            requestSequence.Reset();
        }

        readonly IStatusRequestGenerator<T> requestSequence;
        readonly Func<double> interval;
        public MessageQueueWithAutomatedStatusChecks(ISyncProtocol<T> protocol, byte attempts, IStatusRequestGenerator<T> requestSequence, Func<double> interval)
            : this(protocol, attempts, requestSequence, interval, EqualityComparer<Sync<T>>.Default) { }
        public MessageQueueWithAutomatedStatusChecks(ISyncProtocol<T> protocol, byte attempts, IStatusRequestGenerator<T> requestSequence, Func<double> interval, IEqualityComparer<Sync<T>> comparer)
            : base(protocol, comparer, attempts) {
            this.requestSequence = requestSequence;
            this.interval = interval;
            statusCheckTimer.Elapsed += StatusCheckTime_Elapsed;
        }
        void StatusCheckTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            var request = requestSequence.Next;
            // to prevent queue overflow
            if (Contains(request))
                return;
            Enqueue(request);
        }
    }
}
