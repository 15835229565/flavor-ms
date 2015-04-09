using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Flavor.Common.Data.Measure {
    class CyclingMeasureMode: MeasureModeBase {
        public event EventHandler SaveResults {
            add { _cycle.SaveResults += value; }
            remove { _cycle.SaveResults -= value; }
        }
        class MeasureStopper {
            int counter;
            readonly Timer timer;
            bool timerElapsed = false;
            public MeasureStopper(int counterLimit, int timeLimit) {
                counter = counterLimit == 0 ? -timeLimit : counterLimit;
                if (timeLimit > 0) {
                    timer = new Timer();
                    // time in minutes
                    timer.Interval = timeLimit * 60000;
                    timer.AutoReset = false;
                    timer.Elapsed += timer_Elapsed;
                }
            }

            void timer_Elapsed(object sender, ElapsedEventArgs e) {
                timerElapsed = true;
                timer.Elapsed -= timer_Elapsed;
            }
            public void next() {
                if (counter < 0) {
                    // produce infinite loop
                    return;
                }
                --counter;
            }
            public bool ready() {
                return timerElapsed || counter == 0;
            }
            public int estimatedTurns() {
                // estimated operation duration in iterations or time limit in minutes (negative)
                return timerElapsed ? 0 : counter;
            }

            public void startTimer() {
                if (timer == null) {
                    return;
                }
                timer.Start();
            }
        }
        class ShiftException: Exception { }

        readonly MeasureStopper stopper;
        readonly Flavor.Common.Data.Measure.MeasureMode.Precise _cycle;
        readonly PreciseEditorData peak;
        readonly int _checkerIndex;

        readonly bool ignoreInvalidity = true;
        readonly ushort allowedShift;
        public CyclingMeasureMode(ushort startDelay, Flavor.Common.Data.Measure.MeasureMode.Precise cycle, Action<long[][], List<PreciseEditorData>, short?> successfulExit, int iterations, int timeLimit, PreciseEditorData checkerPeak, int checkerIndex, short? initialShift, ushort allowedShift)
            : base((s, e) => {
                var ee = (Flavor.Common.Data.Measure.MeasureMode.Precise.SuccessfulExitEventArgs)e;
                successfulExit(ee.Counts, ee.Points, ee.Shift);
            }) {
            // TODO: checker peak received by index, after sort found by equality again
            //_cycle = new Flavor.Common.Data.Measure.MeasureMode.Precise(min, max, peaks, startDelay, stepDelay, exposition, forwardDelay, backwardDelay, graphUpdater, (ps, peds, shift) => OnSuccessfulExit(new Flavor.Common.Data.Measure.MeasureMode.Precise.SuccessfulExitEventArgs(ps, peds, shift)), initialShift, CheckShift);
            _cycle = cycle;
            _cycle.VoltageStepChangeRequested += Cycle_VoltageStepRequested;
            _cycle.Finalize += Cycle_Finalize;
            _cycle.Disable += cycle_Disable;

            this.allowedShift = allowedShift;
            stopper = new MeasureStopper(iterations, timeLimit);
            _checkerIndex = checkerIndex;
            peak = checkerPeak;
        }
        void cycle_Disable(object sender, EventArgs e) {
            _cycle.Finalize -= Cycle_Finalize;
            _cycle.VoltageStepChangeRequested -= Cycle_VoltageStepRequested;
            _cycle.Disable -= cycle_Disable;
            stop();
        }
        void Cycle_Finalize(object sender, CancelEventArgs e) {
            OnFinalize(e);
        }
        void Cycle_VoltageStepRequested(object sender, VoltageStepEventArgs e) {
            OnVoltageStepChangeRequested(e.Step);
        }

        // TODO: move checker logic to cycle: cycle provides mechanism to check as cycle can be different
        //void CheckShift(PreciseEditorData curPeak) {
        //    if (!cycle.shift.HasValue || curPeak == null || !curPeak.Equals(peak)) {
        //        // do not store value here!
        //        return;
        //    }
        //    long[] counts = cycle._counts[checkerIndex];
        //    ushort width = peak.Width;
        //    if (counts.Length != 2 * width + 1) {
        //        // data mismatch. strange.
        //        return;
        //    }
        //    long max = -1;
        //    int index = -1;
        //    for (int i = 0; i < counts.Length; ++i) {
        //        if (counts[i] > max) {
        //            max = counts[i];
        //            index = i;
        //        }
        //    }
        //    if (max == counts[width]) {
        //        index = width;
        //    }
        //    // delta from peak center
        //    short delta = (short)(index - width);
        //    if (delta > allowedShift || delta < -allowedShift) {
        //        cycle.shift += delta;
        //        if (ignoreInvalidity) {
        //            return;
        //        }
        //        throw new ShiftException();
        //    }
        //}
        public override bool onUpdateCounts(uint[] counts) {
            if (CancelRequested) {
                _cycle.CancelRequested = true;
            }
            //try {
                return _cycle.onUpdateCounts(counts);
            //} catch (ShiftException) {
            //    if (CancelRequested) {
            //        _cycle.stop();
            //        stop();
            //        return true;
            //    }
            //    // restart cycle
            //    // TODO: new method in cycle to clone..
            //    _cycle.init(true);
            //    return _cycle.onNextStep();
            //}
        }
        protected override void OnSuccessfulExit(EventArgs args) {
            base.OnSuccessfulExit(args);
            stopper.next();
        }
        //protected override void OnFinalize(CancelEventArgs e) {
        //    if (CancelRequested || stopper.ready()) {
        //        // measure mode end
        //        //stop();
        //    } else {
        //        e.Cancel = true;
        //        // next cycle
        //        _cycle.init(true);
        //        _cycle.onNextStep();
        //    }
        //}
        public override bool Start() {
            if (!_cycle.Start()) {
                return false;
            }
            stopper.startTimer();
            return base.Start();
        }
        void stop() {
            // cycle is already stopped anyway.. cancellation is bad
            var e = new CancelEventArgs(false);
            base.OnFinalize(e);
            if (e.Cancel)
                return;
            isOperating = false;
            base.OnDisable();
        }
        public override int StepsCount {
            get {
                // actually is used only on measure mode start..
                int stopperTurns = stopper.estimatedTurns();
                if (stopperTurns < 0) {
                    // minutes
                    return stopperTurns;
                }
                return _cycle.StepsCount * stopperTurns;
            }
        }

        public override void UpdateGraph() {
            _cycle.UpdateGraph();
        }

        public sealed override void NextMeasure(Action<ushort, ushort> send) {
            _cycle.NextMeasure(send);
        }
        public int CycleTime() {
            return _cycle.OverallTime();
        }
    }
}
