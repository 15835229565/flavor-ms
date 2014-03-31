using System;
using System.Collections.Generic;
using System.Timers;

namespace Flavor.Common {
    // TODO: remove dependencies from Commander, Config
    abstract internal class MeasureMode {
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
            // TODO: lock here?
            if (SuccessfulExit != null)
                SuccessfulExit(this, args);
        }
        public event EventHandler Disable;
        protected virtual void OnDisable() {
            // TODO: lock here?
            if (Disable != null)
                Disable(this, EventArgs.Empty);
        }
        public event EventHandler Finalize;
        protected virtual void OnFinalize() {
            // TODO: lock here?
            if (Finalize != null)
                Finalize(this, EventArgs.Empty);
        }

        private readonly object locker = new object();

        private bool operating = false;
        internal bool isOperating {
            get { return operating; }
        }

        private ushort pointValue = 0;

        private SingleMeasureEventArgs customMeasureEventArgs = null;

        private readonly SingleMeasureEventArgs firstMeasureEventArgs;
        private readonly SingleMeasureEventArgs generalMeasureEventArgs;

        public bool CancelRequested { private get; set; }
        private MeasureMode(ushort befTime, ushort eTime) {
            this.firstMeasureEventArgs = new SingleMeasureEventArgs(befTime, eTime);
            this.generalMeasureEventArgs = new SingleMeasureEventArgs(Config.CommonOptions.iTime, Config.CommonOptions.eTime);
        }
        internal bool onUpdateCounts() {
            customMeasureEventArgs = null;//ATTENTION! need to be modified if measure mode without waiting for count answer is applied
            //lock here?
            saveData();
            if (toContinue())
            {
                // TODO:!
                if (CancelRequested)
                {
                    stop();
                    return true;
                }
                if (!onNextStep())
                {
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
        private void stop() {
            OnFinalize();
            operating = false;
            OnVoltageStepChangeRequested(0);//Set ScanVoltage to low limit
            OnDisable();
        }
        // TODO: move to Commander!
        // internal usage only
        abstract protected void saveData();
        abstract protected bool onNextStep();
        abstract protected bool toContinue();
        internal virtual bool Start() {
            //first measure point with increased idle time
            customMeasureEventArgs = firstMeasureEventArgs;
            operating = true;
            return true;
        }
        // external usage only
        public Action<ushort, Utility.PreciseEditorData> GraphUpdateDelegate { get; set; }
        abstract internal void UpdateGraph();
        // external usage only
        abstract internal int StepsCount { get; }
        internal void NextMeasure(Action<ushort, ushort> send) {
            var args = customMeasureEventArgs == null ? generalMeasureEventArgs : customMeasureEventArgs;
            send(args.IdleTime, args.ExpositionTime);
        }

        internal class Scan: MeasureMode {
            private readonly ushort sPoint;
            private readonly ushort ePoint;

            internal Scan()
                : base(Config.CommonOptions.befTime, Config.CommonOptions.eTime) {
                sPoint = Config.sPoint;
                ePoint = Config.ePoint;
            }
            protected override void saveData() { }
            protected override bool onNextStep() {
                OnVoltageStepChangeRequested(pointValue);
                ++pointValue;
                return true;
            }
            protected override bool toContinue() {
                return pointValue <= ePoint;
            }

            internal override bool Start() {
                if (!base.Start()) {
                    return false;
                }
                //lock here
                pointValue = sPoint;
                return onNextStep();
            }
            internal override void UpdateGraph() {
                ushort pnt = pointValue;
                GraphUpdateDelegate(--pnt, null);
            }
            internal override int StepsCount {
                get { return ePoint - sPoint + 1; }
            }
        }
        internal class Precise: MeasureMode {
            public class SaveResultsEventArgs: EventArgs {
                public short? Shift { get; private set; }
                public SaveResultsEventArgs(short? shift) {
                    Shift = shift;
                }
            }
            public event EventHandler<SaveResultsEventArgs> SaveResults;
            protected virtual void OnSaveResults(short? shift) {
                // TODO: lock here?
                if (SaveResults != null)
                    SaveResults(this, new SaveResultsEventArgs(shift));
                // senseModeCounts here?
            }
            private List<Utility.PreciseEditorData> senseModePoints;
            private long[][] senseModeCounts;
            private byte senseModePeak = 0;
            private Utility.PreciseEditorData SenseModePeak {
                get { return senseModePoints[senseModePeak]; }
            }
            private ushort[] senseModePeakIterationMax;
            private ushort[] senseModePeakIteration;
            private ushort smpiSumMax;//ushort?
            private ushort smpiSum;//ushort?

            private bool noPoints = true;
            private int stepPoints = 0;

            private short? shift = null;

            private readonly SingleMeasureEventArgs forwardMeasureEventArgs;
            private readonly SingleMeasureEventArgs backwardMeasureEventArgs;

            internal Precise()
                : this(Config.PreciseData.getUsed(), 0) { }
            private Precise(List<Utility.PreciseEditorData> peaks, short? shift)
                : base(Config.CommonOptions.befTime, Config.CommonOptions.eTime) {
                forwardMeasureEventArgs = Config.CommonOptions.ForwardTimeEqualsBeforeTime ? firstMeasureEventArgs : new SingleMeasureEventArgs(Config.CommonOptions.fTime, Config.CommonOptions.eTime);
                backwardMeasureEventArgs = new SingleMeasureEventArgs(Config.CommonOptions.bTime, Config.CommonOptions.eTime);

                this.shift = shift;
                senseModePoints = peaks;
                //Sort in increased order
                if (senseModePoints.Count == 0) {
                    // nothing to do... strange. throw smth?
                    return;
                }

                noPoints = false;
                // only peak value?
                senseModePoints.Sort(Utility.PreciseEditorData.ComparePreciseEditorDataByPeakValue);
                senseModePeakIterationMax = new ushort[senseModePoints.Count];
                smpiSumMax = 0;
                senseModeCounts = new long[senseModePoints.Count][];
                for (int i = 0; i < senseModePeakIterationMax.Length; ++i) {
                    int dimension = 2 * senseModePoints[i].Width + 1;
                    stepPoints += dimension;
                    senseModeCounts[i] = new long[dimension];
                    senseModePeakIterationMax[i] = senseModePoints[i].Iterations;
                    smpiSumMax += senseModePoints[i].Iterations; ;
                }
            }
            protected override void saveData() {
                Utility.PreciseEditorData peak = senseModePoints[senseModePeak];
                senseModeCounts[senseModePeak][(pointValue - 1) - peak.Step + peak.Width] += peak.Collector == 1 ? Device.Detector1 : Device.Detector2;
            }
            public class SuccessfulExitEventArgs: EventArgs {
                public long[][] Counts { get; private set; }
                public List<Utility.PreciseEditorData> Points { get; private set; }
                public short? Shift { get; private set; }
                public SuccessfulExitEventArgs(long[][] counts, List<Utility.PreciseEditorData> points, short? shift) {
                    Counts = counts;
                    Points = points;
                    Shift = shift;
                }
            }
            protected override void OnSuccessfulExit(EventArgs args) {
                // order is important here: points are saved from graph..
                base.OnSuccessfulExit(new SuccessfulExitEventArgs(senseModeCounts, senseModePoints, shift));
                OnSaveResults(shift);
            }

            protected override bool onNextStep() {
                int realValue = pointValue + (shift ?? 0);
                if (realValue > Config.MAX_STEP || realValue < Config.MIN_STEP) {
                    return false;
                }
                OnVoltageStepChangeRequested((ushort)realValue);
                ++pointValue;
                return true;
            }
            protected override bool toContinue() {
                Utility.PreciseEditorData peak = senseModePoints[senseModePeak];
                if ((pointValue > (peak.Step + peak.Width))) {
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
                        ++senseModePeak;
                        if (senseModePeak >= senseModePoints.Count) senseModePeak = 0;
                        if (senseModePeakIteration[senseModePeak] > 0) break;
                    }
                    ushort nextPoint = (ushort)(senseModePoints[senseModePeak].Step - senseModePoints[senseModePeak].Width);
                    customMeasureEventArgs = (pointValue > nextPoint) ? backwardMeasureEventArgs : forwardMeasureEventArgs;
                    pointValue = nextPoint;
                }
                return true;
            }
            protected virtual bool isSpectrumValid(Utility.PreciseEditorData curPeak) {
                return true;
            }

            internal override bool Start() {
                if (noPoints) {
                    OnDisable();
                    return false;
                }
                if (!init(false)) {
                    return false;
                }
                return onNextStep();
            }
            private bool init(bool initCounts) {
                smpiSum = smpiSumMax;
                senseModePeak = 0;
                senseModePeakIteration = senseModePeakIterationMax.Clone() as ushort[];
                pointValue = (ushort)(senseModePoints[senseModePeak].Step - senseModePoints[senseModePeak].Width);
                if (initCounts) {
                    for (int i = 0; i < senseModeCounts.GetLength(0); ++i) {
                        senseModeCounts[i] = new long[senseModeCounts[i].Length];
                    }
                }
                return base.Start();
            }
            internal override void UpdateGraph() {
                ushort pnt = pointValue;
                GraphUpdateDelegate(--pnt, SenseModePeak);
            }
            internal override int StepsCount {
                get { return stepPoints; }
            }
            internal class Monitor: Precise {
                private class MeasureStopper {
                    private int counter;
                    private readonly Timer timer;
                    private bool timerElapsed = false;
                    internal MeasureStopper(int counterLimit, int timeLimit) {
                        counter = counterLimit == 0 ? -1 : counterLimit;
                        if (timeLimit > 0) {
                            timer = new Timer();
                            // time in minutes
                            timer.Interval = timeLimit * 60000;
                            timer.AutoReset = false;
                            timer.Elapsed += timer_Elapsed;
                        }
                    }

                    private void timer_Elapsed(object sender, ElapsedEventArgs e) {
                        timerElapsed = true;
                        timer.Elapsed -= timer_Elapsed;
                    }
                    internal void next() {
                        if (counter == -1) {
                            // produce infinite loop
                            return;
                        }
                        --counter;
                    }
                    internal bool ready() {
                        return timerElapsed || counter == 0;
                    }
                    internal int estimatedTurns() {
                        // estimated operation duration
                        return counter;
                    }

                    internal void startTimer() {
                        if (timer == null) {
                            return;
                        }
                        timer.Start();
                    }
                }

                private readonly MeasureStopper stopper;
                
                private readonly Utility.PreciseEditorData peak;
                private readonly int checkerIndex;
                
                private bool spectrumIsValid = true;
                private readonly ushort allowedShift;
                private long[] prevIteration = null;

                internal Monitor(short? initialShift, ushort allowedShift, int timeLimit)
                    : base(Graph.Instance.PreciseData.getUsed(), initialShift) {
                    // TODO: getWithId()
                    this.allowedShift = allowedShift;
                    stopper = new MeasureStopper(Config.Iterations, timeLimit);
                    if (initialShift.HasValue) {
                        peak = Config.CheckerPeak;
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
                protected override bool isSpectrumValid(Utility.PreciseEditorData curPeak) {
                    // TODO: use options-specific delegate
                    // specially do not stop cycle
                    return isSpectrumValid2(curPeak, true);
                }
                private bool isSpectrumValid2(Utility.PreciseEditorData curPeak, bool ignoreInvalidity) {
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
                internal override bool Start() {
                    if (!base.Start()) {
                        return false;
                    }
                    stopper.startTimer();
                    return true;
                }
                internal override int StepsCount {
                    get {
                        int stopperTurns = stopper.estimatedTurns();
                        if (stopperTurns <= 0) {
                            return 0;
                        }
                        return base.StepsCount * stopperTurns;
                    }
                }
            }
        }
    }
}