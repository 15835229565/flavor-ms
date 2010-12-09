using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.UI;
using Flavor.Forms;

namespace Flavor.Common.Measuring {
    internal class PreciseMeasureMode: MeasureMode {
        protected internal List<Utility.PreciseEditorData> senseModePoints;
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

        protected short shift = 0;

        internal PreciseMeasureMode(): this(Config.PreciseData.FindAll(Utility.PeakIsUsed)) { }
        internal PreciseMeasureMode(List<Utility.PreciseEditorData> peaks)
            : base() {
            senseModePoints = peaks;
            //Sort in increased order
            if (senseModePoints.Count == 0) {
                // nothing to do... strange. throw smth?
                return;
            }

            noPoints = false;
            senseModePoints.Sort(Utility.ComparePreciseEditorDataByPeakValue);
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
        protected override void onExit() {
            // order is important here: points are saved from graph..
            Graph.updateGraphAfterPreciseMeasure(senseModeCounts, senseModePoints, shift);
            saveResults();
        }
        protected virtual void saveResults() {
            Config.autoSavePreciseSpectrumFile(shift);
        }

        protected long[] peakCounts(Predicate<Utility.PreciseEditorData> isCheckPeak) {
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
        protected bool init(bool initCounts) {
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
    }
}
