using System;
using System.Collections.Generic;
using System.Timers;

namespace Flavor.Common.Data.Measure {
    abstract class MeasureMode: MeasureModeBase {
        //readonly object locker = new object();
        ushort _step = 0;

        readonly ushort _min, _max;
        SingleMeasureEventArgs customMeasureEventArgs = null;
        readonly SingleMeasureEventArgs instantMeasureEventArgs;
        readonly SingleMeasureEventArgs generalMeasureEventArgs;
        readonly SingleMeasureEventArgs firstMeasureEventArgs;
        MeasureMode(ushort min, ushort max, ushort befTime, ushort iTime, ushort eTime, EventHandler successfulExit)
            : base(successfulExit) {
            _min = min;
            _max = max;
            instantMeasureEventArgs = new SingleMeasureEventArgs(0, eTime);
            generalMeasureEventArgs = new SingleMeasureEventArgs(iTime, eTime);
            firstMeasureEventArgs = new SingleMeasureEventArgs(befTime, eTime);
        }
        public override bool Start() {
            //first measure point with increased idle time
            customMeasureEventArgs = firstMeasureEventArgs;
            return base.Start();
        }
        public sealed override void NextMeasure(Action<ushort, ushort> send) {
            var args = customMeasureEventArgs == null ? generalMeasureEventArgs : customMeasureEventArgs;
            send(args.IdleTime, args.ExpositionTime);
        }
        public sealed override bool onUpdateCounts(uint[] counts) {
            customMeasureEventArgs = null;//ATTENTION! need to be modified if measure mode without waiting for count answer is applied
            //lock here?
            saveData(counts);
            if (toContinue()) {
                // TODO:!
                if (CancelRequested) {
                    stop();
                    return true;
                }
                if (!onNextStep()) {
                    // TODO: cannot perform step!
                    OnDisable();
                    return false;
                }
            } else {
                OnSuccessfulExit(EventArgs.Empty);
                stop();
            }
            return true;
        }
        abstract protected void saveData(uint[] counts);
        abstract protected bool onNextStep();
        abstract protected bool toContinue();
        void stop() {
            var e = new CancelEventArgs(false);
            OnFinalize(e);
            if (e.Cancel)
                return;
            isOperating = false;
            OnVoltageStepChangeRequested(0);//Set ScanVoltage to low limit
            OnDisable();
        }
        public class Scan: MeasureMode {
            // is used to sparse data
            readonly ushort _ratio;

            readonly Action<ushort> _graphUpdater;
            public Scan(ushort min, ushort max, ushort befTime, ushort iTime, ushort eTime, Action<ushort> graphUpdater, Action successfulExit, ushort ratio)
                : base(min, max, befTime, iTime, eTime, (s, e) => successfulExit()) {
                _graphUpdater = graphUpdater;
                _ratio = ratio;
            }
            public Scan(ushort min, ushort max, ushort befTime, ushort iTime, ushort eTime, Action<ushort> graphUpdater, Action successfulExit)
                : this(min, max, befTime, iTime, eTime, graphUpdater, successfulExit, 1) { }
            protected override void saveData(uint[] counts) { }
            protected override bool onNextStep() {
                OnVoltageStepChangeRequested(_step);
                _step += _ratio;
                return true;
            }
            protected override bool toContinue() {
                return _step <= _max;
            }

            public override bool Start() {
                if (!base.Start()) {
                    return false;
                }
                //lock here
                _step = _min;
                return onNextStep();
            }
            public override void UpdateGraph() {
                ushort pnt = _step;
                pnt -= _ratio;
                for (int i = 0; i < _ratio; ++i) {
                    // temporary solution. fake points to prevent spectrum save format
                    _graphUpdater(pnt);
                    ++pnt;
                    if (pnt > _max)
                        break;
                }
            }
            public override int StepsCount {
                get { return (_max - _min + 1)/* / _ratio*/; }
            }
        }
        public class Precise: MeasureMode {
            public event EventHandler SaveResults;
            protected virtual void OnSaveResults() {
                SaveResults.Raise(this, EventArgs.Empty);
                // send data here?
            }
            readonly List<PreciseEditorData> _peaks;
            long[][] _counts;
            byte _peakIndex = 0;
            PreciseEditorData Peak {
                get { return _peaks[_peakIndex]; }
            }
            readonly ushort[] _maxIterations;
            ushort[] _iterations;
            readonly ushort smpiSumMax;//ushort?
            ushort smpiSum;//ushort?

            readonly int _overallSteps;
            readonly int _overallExposition;
            readonly int _overallDelay;

            bool noPoints = true;

            short? shift = null;

            readonly SingleMeasureEventArgs forwardMeasureEventArgs;
            readonly SingleMeasureEventArgs backwardMeasureEventArgs;
            readonly Action<PreciseEditorData> _checkOnPeakEnd;
            readonly Action<ushort, PreciseEditorData> _graphUpdater;
            public Precise(ushort min, ushort max, List<PreciseEditorData> peaks, ushort startDelay, ushort stepDelay, ushort exposition, ushort forwardDelay, ushort backwardDelay, Action<ushort, PreciseEditorData> graphUpdater, Action<long[][], List<PreciseEditorData>, short?> successfulExit)
                : this(min, max, peaks, startDelay, stepDelay, exposition, forwardDelay, backwardDelay, graphUpdater, successfulExit, 0, p => { }) { }
            Precise(ushort min, ushort max, List<PreciseEditorData> peaks, ushort startDelay, ushort stepDelay, ushort exposition, ushort forwardDelay, ushort backwardDelay, Action<ushort, PreciseEditorData> graphUpdater, Action<long[][], List<PreciseEditorData>, short?> successfulExit, short? shift, Action<PreciseEditorData> checkOnPeakEnd)
                : base(min, max, startDelay, stepDelay, exposition, (s, e) => {
                    var ee = (SuccessfulExitEventArgs)e;
                    successfulExit(ee.Counts, ee.Points, ee.Shift);
                }) {
                _graphUpdater = graphUpdater;
                _checkOnPeakEnd = checkOnPeakEnd;
                forwardMeasureEventArgs = new SingleMeasureEventArgs(forwardDelay, exposition);
                backwardMeasureEventArgs = new SingleMeasureEventArgs(backwardDelay, exposition);

                this.shift = shift;
                //Sort in increased order
                int count = peaks.Count;
                if (count == 0) {
                    // nothing to do... strange. throw smth?
                    return;
                }

                noPoints = false;
                // only peak value?
                peaks.Sort(PreciseEditorData.ComparePreciseEditorDataByPeakValue);
                _maxIterations = new ushort[count];
                smpiSumMax = 0;
                _counts = new long[count][];
                for (int i = 0; i < _maxIterations.Length; ++i) {
                    var peak = peaks[i];
                    int dimension = 2 * peak.Width + 1;
                    _counts[i] = new long[dimension];
                    ushort iterations = peak.Iterations;
                    _maxIterations[i] = iterations;
                    smpiSumMax += iterations; ;
                    _overallSteps += dimension * iterations;
                }
                _peaks = peaks;

                _overallExposition = _overallSteps * exposition;
                _iterations = (ushort[])_maxIterations.Clone();
                ushort delay = startDelay;
                int s = 0;
                // TODO: unify with toContinue()
                for (int i = smpiSumMax, j = 0; i > 0; --i) {
                    for (int k = 0; k < count; ++k) {
                        ++j;
                        j %= count;
                        if (_iterations[j] > 0)
                            break;
                    }
                    var p = peaks[j];
                    int start = p.Step - p.Width;
                    int end = p.Step + p.Width;
                    int diff = start - s;
                    switch (diff) {
                        case 0:
                            delay = 0;
                            break;
                        case 1:
                        case -1:
                            delay = stepDelay;
                            break;
                        default:
                            delay = diff > 0 ? forwardDelay : backwardDelay;
                            break;
                    }
                    _overallDelay += delay;
                    _overallDelay += (end - start) * stepDelay;
                    s = end;
                    --_iterations[j];
                }
            }
            public int OverallTime() {
                return _overallExposition + _overallDelay;
            }
            protected override void saveData(uint[] counts) {
                var peak = Peak;
                // be careful!
                _counts[_peakIndex][_step - 1 - peak.Step + peak.Width] += counts[peak.Collector - 1];
            }
            class SuccessfulExitEventArgs: EventArgs {
                public long[][] Counts { get; private set; }
                public List<PreciseEditorData> Points { get; private set; }
                public short? Shift { get; private set; }
                public SuccessfulExitEventArgs(long[][] counts, List<PreciseEditorData> points, short? shift) {
                    Counts = counts;
                    Points = points;
                    Shift = shift;
                }
            }
            protected override void OnSuccessfulExit(EventArgs args) {
                // order is important here: points are saved from graph..
                base.OnSuccessfulExit(new SuccessfulExitEventArgs(_counts, _peaks, shift));
                OnSaveResults();
            }

            protected override bool onNextStep() {
                int realValue = _step + (shift ?? 0);
                if (realValue > _max || realValue < _min) {
                    return false;
                }
                // TODO: transform NextMeasure to event
                //if (realValue == _step) {
                //    NextMeasure();
                //} else {
                    OnVoltageStepChangeRequested((ushort)realValue);
                //}
                ++_step;
                return true;
            }
            protected override bool toContinue() {
                var peak = Peak;
                if (_step > (peak.Step + peak.Width)) {
                    _checkOnPeakEnd(peak);
                    // modify pointValue in case of finished iteration
                    --(_iterations[_peakIndex]);
                    --smpiSum;
                    if (smpiSum <= 0) {
                        // all data acquired
                        return false;
                    }
                    int count = _peaks.Count;
                    for (int i = 0; i < count; ++i) {
                        // TODO: remove finished peaks from temp work list instead of search
                        ++_peakIndex;
                        _peakIndex %= (byte)count;
                        if (_iterations[_peakIndex] > 0) break;
                    }
                    peak = Peak;
                    ushort nextPoint = (ushort)(peak.Step - peak.Width);
                    // _step is 1 more than current point
                    int diff = _step - nextPoint;
                    if (diff <= 2 && diff >= 0) {
                        if (diff == 1) {
                            // special case when next step is the same
                            customMeasureEventArgs = instantMeasureEventArgs;
                        } else {
                            // special case when next step is close to current
                            customMeasureEventArgs = null;
                        }
                    } else {
                        customMeasureEventArgs = diff > 0 ? backwardMeasureEventArgs : forwardMeasureEventArgs;
                    }
                    _step = nextPoint;
                }
                return true;
            }

            public override bool Start() {
                if (noPoints) {
                    OnDisable();
                    return false;
                }
                if (!init(false)) {
                    return false;
                }
                return onNextStep();
            }
            bool init(bool initCounts) {
                smpiSum = smpiSumMax;
                _peakIndex = 0;
                _iterations = (ushort[])_maxIterations.Clone();
                var peak = Peak;
                _step = (ushort)(peak.Step - peak.Width);
                if (initCounts) {
                    for (int i = 0; i < _counts.GetLength(0); ++i) {
                        _counts[i] = new long[_counts[i].Length];
                    }
                }
                //first measure settings are used inside! not good
                return base.Start();
            }
            public override void UpdateGraph() {
                ushort pnt = _step;
                _graphUpdater(--pnt, Peak);
            }
            public override int StepsCount {
                get { return _overallSteps; }
            }
            public class Monitor: MeasureModeBase {
                public event EventHandler SaveResults {
                    add { cycle.SaveResults += value; }
                    remove { cycle.SaveResults -= value; }
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
                readonly Precise cycle;
                readonly PreciseEditorData peak;
                readonly int checkerIndex;
                
                readonly bool ignoreInvalidity = true;
                readonly ushort allowedShift;
                readonly long[] prevIteration;

                public Monitor(ushort min, ushort max, List<PreciseEditorData> peaks, ushort startDelay, ushort stepDelay, ushort exposition, ushort forwardDelay, ushort backwardDelay, Action<ushort, PreciseEditorData> graphUpdater, Action<long[][], List<PreciseEditorData>, short?> successfulExit, int iterations, int timeLimit, PreciseEditorData checkerPeak, short? initialShift, ushort allowedShift)
                    : base((s, e) => {
                        var ee = (SuccessfulExitEventArgs)e;
                        successfulExit(ee.Counts, ee.Points, ee.Shift);
                    }) {
                    // TODO: checker peak received by index, after sort found by equality again
                    cycle = new Precise(min, max, peaks, startDelay, stepDelay, exposition, forwardDelay, backwardDelay, graphUpdater, (ps, peds, shift) => OnSuccessfulExit(new SuccessfulExitEventArgs(ps, peds, shift)), initialShift, CheckShift);
                    cycle.VoltageStepChangeRequested += Cycle_VoltageStepRequested;
                    cycle.Finalize += Cycle_Finalize;
                    cycle.Disable += cycle_Disable;

                    this.allowedShift = allowedShift;
                    stopper = new MeasureStopper(iterations, timeLimit);
                    // redundant data
                    if (initialShift.HasValue && checkerPeak != null) {
                        // TODO: move up to Commander. only index here
                        peak = checkerPeak;
                        checkerIndex = peaks.FindIndex(peak.Equals);
                        if (checkerIndex != -1)
                            prevIteration = new long[cycle._counts[checkerIndex].Length];
                    }
                }

                void cycle_Disable(object sender, EventArgs e) {
                    cycle.Finalize -= Cycle_Finalize;
                    cycle.VoltageStepChangeRequested -= Cycle_VoltageStepRequested;
                    cycle.Disable -= cycle_Disable;
                    stop();                
                }
                void Cycle_Finalize(object sender, CancelEventArgs e) {
                    OnFinalize(e);
                }
                void Cycle_VoltageStepRequested(object sender, VoltageStepEventArgs e) {
                    OnVoltageStepChangeRequested(e.Step);
                }

                void CheckShift(PreciseEditorData curPeak) {
                    if (!cycle.shift.HasValue || !curPeak.Equals(peak) || prevIteration == null) {
                        // if peak is null also exit here
                        // do not store value here!
                        return;
                    }
                    long[] counts = cycle._counts[checkerIndex];
                    ushort width = peak.Width;
                    if (counts.Length != 2 * width + 1) {
                        // data mismatch. strange.
                        return;
                    }
                    long max = -1;
                    int index = -1;
                    for (int i = 0; i < counts.Length; ++i) {
                        // TODO: better solution. now not optimal. reverse work.
                        long temp = counts[i] - prevIteration[i];
                        if (temp > max) {
                            max = temp;
                            index = i;
                        }
                        prevIteration[i] = counts[i];
                    }

                    short delta = (short)(index - width);
                    if (delta > allowedShift || delta < -allowedShift) {
                        cycle.shift += delta;
                        if (ignoreInvalidity) {
                            return;
                        }
                        throw new ShiftException();
                    }
                }
                public override bool onUpdateCounts(uint[] counts) {
                    if (CancelRequested) {
                        cycle.CancelRequested = true;
                    }
                    try {
                        return cycle.onUpdateCounts(counts);
                    } catch (ShiftException) {
                        if (CancelRequested) {
                            cycle.stop();
                            stop();
                            return true;
                        }
                        // restart cycle
                        cycle.init(true);
                        return cycle.onNextStep();
                    }
                }
                protected override void OnSuccessfulExit(EventArgs args) {
                    base.OnSuccessfulExit(args);
                    stopper.next();
                }
                protected override void OnFinalize(CancelEventArgs e) {
                    if (CancelRequested || stopper.ready()) {
                        // measure mode end
                        //stop();
                    } else {
                        e.Cancel = true;
                        // next cycle
                        cycle.init(true);
                        cycle.onNextStep();
                    }
                }
                public override bool Start() {
                    if (!cycle.Start()) {
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
                        return cycle.StepsCount * stopperTurns;
                    }
                }

                public override void UpdateGraph() {
                    cycle.UpdateGraph();
                }

                public sealed override void NextMeasure(Action<ushort, ushort> send) {
                    cycle.NextMeasure(send);
                }
                public int CycleTime() {
                    return cycle.OverallTime();
                }
            }
        }
    }
}