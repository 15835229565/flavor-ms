using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.UI;
using Flavor.Forms;
using System.Timers;

namespace Flavor.Common {
    abstract internal class MeasureMode {
        private object locker = new object();

        private bool operating = false;
        internal bool isOperating {
            get { return operating; }
        }

        private ushort pointValue = 0;

        private sendMeasure customMeasure = null;
        private ushort befTime;
        private ushort eTime;

        private MeasureMode() {
            befTime = Config.CommonOptions.befTime;
            eTime = Config.CommonOptions.eTime;
        }
        internal bool onUpdateCounts() {
            customMeasure = null;//ATTENTION! need to be modified if measure mode without waiting for count answer is applied
            //lock here?
            saveData();
            if (toContinue())
            {
                if (Commander.measureCancelRequested)
                {
                    stop();
                    return true;
                }
                if (!onNextStep())
                {
                    // TODO: cannot perform step!
                    Disable();
                    return false;
                }
            }
            else {
                onSuccessfulExit();
                stop();
            }
            return true;
        }
        abstract protected void saveData();
        abstract protected void onSuccessfulExit();
        abstract protected bool onNextStep();
        abstract protected bool toContinue();
        protected virtual void finalize() {}
        internal virtual bool start() {
            //first measure point with increased idle time
            customMeasure = new sendMeasure(befTime, eTime);
            operating = true;
            return true;
        }
        abstract internal void updateGraph();
        abstract internal int stepsCount();
        internal void autoNextMeasure() {
            if (operating) {
                if (customMeasure == null) {
                    Commander.AddToSend(new sendMeasure());
                } else {
                    Commander.AddToSend(customMeasure);
                }
            }
        }
        private void Disable() {
            Commander.Disable();
            Commander.setProgramStateWithoutUndo(Commander.programStates.Ready);//really without undo?
        }
        private void stop() {
            finalize();
            operating = false;
            Commander.AddToSend(new sendSVoltage(0));//Set ScanVoltage to low limit
            Disable();
        }

        internal class Scan: MeasureMode {
            private ushort sPoint;
            private ushort ePoint;
            internal Scan()
                : base() {
                sPoint = Config.sPoint;
                ePoint = Config.ePoint;
            }
            protected override void saveData() { }
            protected override void onSuccessfulExit() {
                Config.autoSaveSpectrumFile();
            }
            protected override bool onNextStep() {
                Commander.AddToSend(new sendSVoltage(pointValue++));
                return true;
            }
            protected override bool toContinue() {
                return pointValue <= ePoint;
            }

            internal override bool start() {
                if (!base.start()) {
                    return false;
                }
                //lock here
                pointValue = sPoint;
                onNextStep();
                return true;
            }
            internal override void updateGraph() {
                ushort pnt = pointValue;
                Graph.updateGraphDuringScanMeasure(Device.Detector1, Device.Detector2, --pnt);
            }
            internal override int stepsCount() {
                return ePoint - sPoint + 1;
            }
        }
        internal class Precise: MeasureMode {
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

            private short shift = 0;

            internal Precise()
                : this(Config.PreciseData.FindAll(Utility.PreciseEditorData.PeakIsUsed)) { }
            private Precise(List<Utility.PreciseEditorData> peaks)
                : base() {
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
                if (senseModePoints[senseModePeak].Collector == 1) {
                    senseModeCounts[senseModePeak][(pointValue - 1) - senseModePoints[senseModePeak].Step + senseModePoints[senseModePeak].Width] += Device.Detector1;
                } else {
                    senseModeCounts[senseModePeak][(pointValue - 1) - senseModePoints[senseModePeak].Step + senseModePoints[senseModePeak].Width] += Device.Detector2;
                }
            }
            protected override void onSuccessfulExit() {
                // order is important here: points are saved from graph..
                Graph.updateGraphAfterPreciseMeasure(senseModeCounts, senseModePoints, shift);
                saveResults();
            }
            protected virtual void saveResults() {
                Config.autoSavePreciseSpectrumFile(shift);
            }

            private long[] peakCounts(Predicate<Utility.PreciseEditorData> isCheckPeak) {
                int index = senseModePoints.FindIndex(isCheckPeak);
                if (index == -1) {
                    return null;
                }
                long[] temp = senseModeCounts[index];
                // auto-reinitialize
                senseModeCounts[index] = new long[senseModeCounts[index].Length];
                return temp;
            }
            protected override bool onNextStep() {
                int realValue = pointValue + shift;
                if (realValue > Config.MAX_STEP || realValue < Config.MIN_STEP) {
                    return false;
                }
                Commander.AddToSend(new sendSVoltage((ushort)realValue));
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
                    if (pointValue > nextPoint) {
                        //!!!case of backward voltage change
                        customMeasure = new sendMeasure(Config.CommonOptions.bTime, Config.CommonOptions.eTime);
                    } else {
                        //!!!case of forward voltage change
                        if (Config.CommonOptions.ForwardTimeEqualsBeforeTime) {
                            customMeasure = new sendMeasure(Config.CommonOptions.befTime, Config.CommonOptions.eTime);
                        } else {
                            customMeasure = new sendMeasure(Config.CommonOptions.fTime, Config.CommonOptions.eTime);
                        }
                    }
                    pointValue = nextPoint;
                }
                return true;
            }
            protected virtual bool isSpectrumValid(Utility.PreciseEditorData curPeak) {
                return true;
            }

            internal override bool start() {
                if (noPoints) {
                    Disable();
                    return false;
                }
                if (!init(false)) {
                    return false;
                }
                onNextStep();
                return true;
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
                return base.start();
            }
            internal override void updateGraph() {
                ushort pnt = pointValue;
                Graph.updateGraphDuringPreciseMeasure(--pnt, SenseModePeak);
            }
            internal override int stepsCount() {
                return stepPoints;
            }
            internal class Monitor: Precise {
                private class MeasureStopper {
                    private int counter;
                    private Timer timer;
                    private bool timerElapsed = false;
                    internal MeasureStopper(int counterLimit, int timeLimit) {
                        counter = counterLimit == 0 ? -1 : counterLimit;
                        if (timeLimit > 0) {
                            timer = new Timer();
                            // time in minutes
                            timer.Interval = timeLimit * 60000;
                            timer.AutoReset = false;
                            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                        }
                    }

                    void timer_Elapsed(object sender, ElapsedEventArgs e) {
                        timerElapsed = true;
                        timer.Elapsed -= new ElapsedEventHandler(timer_Elapsed);
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

                private MeasureStopper stopper;
                private Utility.PreciseEditorData peak;
                private bool spectrumIsValid = true;
                private ushort allowedShift;

                internal Monitor(short initialShift, ushort allowedShift, int timeLimit)
                    : base(Config.PreciseDataWithChecker) {
                    shift = initialShift;
                    this.allowedShift = allowedShift;
                    stopper = new MeasureStopper(Config.Iterations, timeLimit);
                    peak = Config.CheckerPeak;
                }
                protected override void onSuccessfulExit() {
                    throw new NotImplementedException("Modify behaviour according to data drop policy");
					if (spectrumIsValid) {
                        base.onSuccessfulExit();
                    }
                }
                protected override void finalize() {
                    Config.finalizeMonitorFile();
                }
                protected override void saveResults() {
                    // senseModeCounts here?
                    Config.autoSaveMonitorSpectrumFile(shift);
                }

                protected override bool toContinue() {
                    if (base.toContinue()) {
                        return true;
                    }
                    throw new NotImplementedException("Modify behaviour according to data drop policy");
                    if (spectrumIsValid) {
                        stopper.next();
                    }
                    if (stopper.ready()) {
                        return false;
                    }
                    // operations between iterations
                    onSuccessfulExit();
                    init(true);
                    return true;
                }
                protected override bool isSpectrumValid(Utility.PreciseEditorData curPeak) {
                    if (!curPeak.Equals(peak)) {
                        // do not store value here!
                        return true;
                    }
                    long[] counts = peakCounts(isCheckPeak);
                    ushort width = peak.Width;
                    if (counts.Length != 2 * width + 1) {
                        // data mismatch
                        return spectrumIsValid = false;
                    }
                    long max = -1;
                    int index = -1;
                    for (int i = 0; i < counts.Length; ++i) {
                        long temp = counts[i];
                        if (temp > max) {
                            max = temp;
                            index = i;
                        }
                    }

                    short delta = (short)(index - width);
                    if (delta > allowedShift || delta < -allowedShift) {
                        shift += delta;
                        return spectrumIsValid = false;
                    }
                    return spectrumIsValid = true;
                }
                private bool isCheckPeak(Utility.PreciseEditorData ped) {
                    // only in this situation (checkpeak marked with false use)
                    if (ped.Use) {
                        return false;
                    }
                    return true;
                }
                internal override bool start() {
                    if (!base.start()) {
                        return false;
                    }
                    stopper.startTimer();
                    return true;
                }
                internal override int stepsCount() {
                    int stopperTurns = stopper.estimatedTurns();
                    if (stopperTurns <= 0) {
                        return 0;
                    }
                    return base.stepsCount() * stopperTurns;
                }
            }
        }
    }
}
