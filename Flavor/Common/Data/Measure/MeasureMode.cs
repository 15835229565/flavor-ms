using System;
using System.Collections.Generic;
using System.Timers;

namespace Flavor.Common.Data.Measure {
    abstract class MeasureMode {
        public class NoListenersException: Exception { }
        public class VoltageStepEventArgs: EventArgs {
            public ushort Step { get; private set; }
            public VoltageStepEventArgs(ushort step) {
                Step = step;
            }
        }
        public event EventHandler<VoltageStepEventArgs> VoltageStepChangeRequested;
        protected virtual void OnVoltageStepChangeRequested(ushort step) {
            // TODO: lock here?
            if (VoltageStepChangeRequested == null)
                throw new NoListenersException();
            VoltageStepChangeRequested(this, new VoltageStepEventArgs(step));
        }
        public class SingleMeasureEventArgs: EventArgs {
            public ushort IdleTime { get; private set; }
            public ushort ExpositionTime { get; private set; }
            public SingleMeasureEventArgs(ushort idleTime, ushort expositionTime) {
                IdleTime = idleTime;
                ExpositionTime = expositionTime;
            }
        }

        public event EventHandler SuccessfulExit;
        protected virtual void OnSuccessfulExit(EventArgs args) {
            SuccessfulExit.Raise(this, args);
        }
        public event EventHandler Disable;
        protected virtual void OnDisable() {
            Disable.Raise(this, EventArgs.Empty);
        }
        public event EventHandler Finalize;
        protected virtual void OnFinalize() {
            Finalize.Raise(this, EventArgs.Empty);
        }

        readonly object locker = new object();

        public bool isOperating { get; private set; }

        readonly ushort _min, _max;
        ushort pointValue = 0;

        SingleMeasureEventArgs customMeasureEventArgs = null;

        readonly SingleMeasureEventArgs firstMeasureEventArgs;
        readonly SingleMeasureEventArgs generalMeasureEventArgs;

        public bool CancelRequested { private get; set; }
        MeasureMode(ushort min, ushort max, ushort befTime, ushort iTime, ushort eTime) {
            isOperating = false;
            _min = min;
            _max = max;
            firstMeasureEventArgs = new SingleMeasureEventArgs(befTime, eTime);
            generalMeasureEventArgs = new SingleMeasureEventArgs(iTime, eTime);
        }
        public bool onUpdateCounts(uint[] counts) {
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
            }
            else {
                OnSuccessfulExit(EventArgs.Empty);
                stop();
            }
            return true;
        }
        void stop() {
            OnFinalize();
            isOperating = false;
            OnVoltageStepChangeRequested(0);//Set ScanVoltage to low limit
            OnDisable();
        }
        // TODO: move to Commander!
        // internal usage only
        abstract protected void saveData(uint[] counts);
        abstract protected bool onNextStep();
        abstract protected bool toContinue();
        public virtual bool Start() {
            //first measure point with increased idle time
            customMeasureEventArgs = firstMeasureEventArgs;
            isOperating = true;
            return true;
        }
        // external usage only
        public Action<ushort, PreciseEditorData> GraphUpdateDelegate { get; set; }
        abstract public void UpdateGraph();
        // external usage only
        abstract public int StepsCount { get; }
        public void NextMeasure(Action<ushort, ushort> send) {
            var args = customMeasureEventArgs == null ? generalMeasureEventArgs : customMeasureEventArgs;
            send(args.IdleTime, args.ExpositionTime);
        }

        public class Scan: MeasureMode {
            // is used to sparse data
            readonly ushort _ratio;

            public Scan(ushort min, ushort max, ushort befTime, ushort iTime, ushort eTime, ushort ratio)
                : base(min, max, befTime, iTime, eTime) {
                _ratio = ratio;
            }
            public Scan(ushort min, ushort max, ushort befTime, ushort iTime, ushort eTime)
                : this(min, max, befTime, iTime, eTime, 1) { }
            protected override void saveData(uint[] counts) { }
            protected override bool onNextStep() {
                OnVoltageStepChangeRequested(pointValue);
                pointValue += _ratio;
                return true;
            }
            protected override bool toContinue() {
                return pointValue <= _max;
            }

            public override bool Start() {
                if (!base.Start()) {
                    return false;
                }
                //lock here
                pointValue = _min;
                return onNextStep();
            }
            public override void UpdateGraph() {
                ushort pnt = pointValue;
                pnt -= _ratio;
                for (int i = 0; i < _ratio; ++i) {
                    // temporary solution. fake points to prevent spectrum save format
                    GraphUpdateDelegate(pnt, null);
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
            List<PreciseEditorData> senseModePoints;
            long[][] senseModeCounts;
            byte senseModePeak = 0;
            PreciseEditorData SenseModePeak {
                get { return senseModePoints[senseModePeak]; }
            }
            ushort[] senseModePeakIterationMax;
            ushort[] senseModePeakIteration;
            ushort smpiSumMax;//ushort?
            ushort smpiSum;//ushort?

            bool noPoints = true;
            int stepPoints = 0;

            short? shift = null;

            readonly SingleMeasureEventArgs forwardMeasureEventArgs;
            readonly SingleMeasureEventArgs backwardMeasureEventArgs;

            public Precise(ushort min, ushort max, List<PreciseEditorData> peaks, ushort startDelay, ushort stepDelay, ushort exposition, ushort forwardDelay, ushort backwardDelay)
                : this(min, max, peaks, startDelay, stepDelay, exposition, forwardDelay, backwardDelay, 0) { }
            Precise(ushort min, ushort max, List<PreciseEditorData> peaks, ushort startDelay, ushort stepDelay, ushort exposition, ushort forwardDelay, ushort backwardDelay, short? shift)
                : base(min, max, startDelay, stepDelay, exposition) {
                forwardMeasureEventArgs = new SingleMeasureEventArgs(forwardDelay, exposition);
                backwardMeasureEventArgs = new SingleMeasureEventArgs(backwardDelay, exposition);

                this.shift = shift;
                senseModePoints = peaks;
                //Sort in increased order
                if (senseModePoints.Count == 0) {
                    // nothing to do... strange. throw smth?
                    return;
                }

                noPoints = false;
                // only peak value?
                senseModePoints.Sort(PreciseEditorData.ComparePreciseEditorDataByPeakValue);
                senseModePeakIterationMax = new ushort[senseModePoints.Count];
                smpiSumMax = 0;
                senseModeCounts = new long[senseModePoints.Count][];
                // TODO: count cycle time
                for (int i = 0; i < senseModePeakIterationMax.Length; ++i) {
                    int dimension = 2 * senseModePoints[i].Width + 1;
                    senseModeCounts[i] = new long[dimension];
                    ushort iterations = senseModePoints[i].Iterations;
                    senseModePeakIterationMax[i] = iterations;
                    smpiSumMax += iterations; ;
                    stepPoints += dimension * iterations;
                }
            }
            protected override void saveData(uint[] counts) {
                var peak = SenseModePeak;
                // be careful!
                senseModeCounts[senseModePeak][pointValue - 1 - peak.Step + peak.Width] += counts[peak.Collector - 1];
            }
            public class SuccessfulExitEventArgs: EventArgs {
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
                base.OnSuccessfulExit(new SuccessfulExitEventArgs(senseModeCounts, senseModePoints, shift));
                OnSaveResults();
            }

            protected override bool onNextStep() {
                int realValue = pointValue + shift ?? 0;
                // TODO: move up
                if (realValue > _max || realValue < _min) {
                    return false;
                }
                OnVoltageStepChangeRequested((ushort)realValue);
                ++pointValue;
                return true;
            }
            protected override bool toContinue() {
                var peak = SenseModePeak;
                if (pointValue > peak.Step + peak.Width) {
                    if (!isSpectrumValid(peak)) {
                        // check spectrum validity after any iteration over checker peak
                        return false;
                    }
                    // modify pointValue in case of finished iteration
                    --(senseModePeakIteration[senseModePeak]);
                    --smpiSum;
                    if (smpiSum <= 0) {
                        // all data acquired
                        return false;
                    }
                    for (int i = 0; i < senseModePoints.Count; ++i)//Поиск пика с оставшейся ненулевой итерацией. Но не более 1 цикла.
                    {
                        // TODO: remove finished peaks from temp work list instead of search
                        ++senseModePeak;
                        if (senseModePeak >= senseModePoints.Count) senseModePeak = 0;
                        if (senseModePeakIteration[senseModePeak] > 0) break;
                    }
                    peak = SenseModePeak;
                    ushort nextPoint = (ushort)(peak.Step - peak.Width);
                    int diff = pointValue - nextPoint;
                    if (diff <= 2 && diff >= 0) {
                        // special case when next step is close to current
                        customMeasureEventArgs = null;
                    } else {
                        customMeasureEventArgs = diff > 0 ? backwardMeasureEventArgs : forwardMeasureEventArgs;
                    }
                    pointValue = nextPoint;
                }
                return true;
            }
            protected virtual bool isSpectrumValid(PreciseEditorData curPeak) {
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
                senseModePeak = 0;
                senseModePeakIteration = (ushort[])senseModePeakIterationMax.Clone();
                var peak = SenseModePeak;
                pointValue = (ushort)(peak.Step - peak.Width);
                if (initCounts) {
                    for (int i = 0; i < senseModeCounts.GetLength(0); ++i) {
                        senseModeCounts[i] = new long[senseModeCounts[i].Length];
                    }
                }
                return base.Start();
            }
            public override void UpdateGraph() {
                ushort pnt = pointValue;
                GraphUpdateDelegate(--pnt, SenseModePeak);
            }
            public override int StepsCount {
                get { return stepPoints; }
            }
            public class Monitor: Precise {
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

                readonly MeasureStopper stopper;
                
                readonly PreciseEditorData peak;
                readonly int checkerIndex;
                
                bool spectrumIsValid = true;
                readonly ushort allowedShift;
                long[] prevIteration = null;

                public Monitor(ushort min, ushort max, List<PreciseEditorData> peaks, ushort startDelay, ushort stepDelay, ushort exposition, ushort forwardDelay, ushort backwardDelay, PreciseEditorData checkerPeak, short? initialShift, ushort allowedShift, int iterations, int timeLimit)
                    : base(min, max, peaks, startDelay, stepDelay, exposition, forwardDelay, backwardDelay, initialShift) {
                    this.allowedShift = allowedShift;
                    stopper = new MeasureStopper(iterations, timeLimit);
                    if (initialShift.HasValue) {
                        peak = checkerPeak;
                        if (peak != null) {
                            checkerIndex = senseModePoints.FindIndex(peak.Equals);
                            if (checkerIndex != -1)
                                prevIteration = new long[senseModeCounts[checkerIndex].Length];
                        }
                    }
                }
                protected override void OnSuccessfulExit(EventArgs args) {
                    //TODO: option-dependent behaviour: drop or save data on shift situation. See similar comment in toContinue()
					if (true || spectrumIsValid) {
                        base.OnSuccessfulExit(args);
                    }
                }

                protected override bool toContinue() {
                    if (base.toContinue()) {
                        return true;
                    }
                    //TODO: option-dependent behaviour: transition to next cycle on shift situation. See similar comment in onSuccessfulExit()
                    if (true || spectrumIsValid) {
                        stopper.next();
                    }
                    if (stopper.ready()) {
                        return false;
                    }
                    // operations between iterations
                    OnSuccessfulExit(EventArgs.Empty);
                    init(true);
                    prevIteration = prevIteration == null ? null : new long[senseModeCounts[checkerIndex].Length];
                    return true;
                }
                protected override bool isSpectrumValid(PreciseEditorData curPeak) {
                    // TODO: use options-specific delegate
                    // specially do not stop cycle
                    return isSpectrumValid2(curPeak, true);
                }
                bool isSpectrumValid2(PreciseEditorData curPeak, bool ignoreInvalidity) {
                    if (!shift.HasValue || !curPeak.Equals(peak) || prevIteration == null) {
                        // if peak is null also exit here
                        // do not store value here!
                        return true;
                    }
                    long[] counts = senseModeCounts[checkerIndex];
                    ushort width = peak.Width;
                    if (counts.Length != 2 * width + 1) {
                        // data mismatch. strange.
                        return spectrumIsValid = false;
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
                        // here shift must not be null!
                        shift += delta;
                        spectrumIsValid = false;
                        return ignoreInvalidity && spectrumIsValid;
                    }
                    return spectrumIsValid = true;
                }
                public override bool Start() {
                    if (!base.Start()) {
                        return false;
                    }
                    stopper.startTimer();
                    return true;
                }
                public override int StepsCount {
                    get {
                        // actually is used only on measure mode start..
                        int stopperTurns = stopper.estimatedTurns();
                        if (stopperTurns < 0) {
                            // minutes
                            return stopperTurns;
                        }
                        return base.StepsCount * stopperTurns;
                    }
                }
            }
        }
    }
}