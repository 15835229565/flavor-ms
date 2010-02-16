using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.UI;
using Flavor.Forms;

namespace Flavor.Common.Measuring
{
    internal class PreciseMeasureMode: MeasureMode
    {
        private Utility.PreciseEditorData[] senseModePoints;
        private long[][] senseModeCounts;
        private byte senseModePeak = 0;
        private Utility.PreciseEditorData SenseModePeak
        {
            get { return senseModePoints[senseModePeak]; }
        }
        private ushort[] senseModePeakIterationMax;
        private ushort[] senseModePeakIteration;
        private ushort smpiSumMax;//ushort?
        private ushort smpiSum;//ushort?

        private bool noPoints = true;
        private int stepPoints = 0;

        internal PreciseMeasureMode(): base()
        {
            //Sort in increased order
            //Config.PreciseData.Sort(ComparePreciseEditorDataByPeakValue);
            //Config.PreciseData.Sort(ComparePreciseEditorDataByUseFlagAndPeakValue);
            //senseModePoints = Config.PreciseData.ToArray();
            List<Utility.PreciseEditorData> temp = Config.PreciseData.FindAll(Utility.PeakIsUsed);
            
            if (temp.Count == 0)
            {
                return;
            }

            noPoints = false;
            temp.Sort(Utility.ComparePreciseEditorDataByPeakValue);
            senseModePoints = temp.ToArray();
            senseModePeakIterationMax = new ushort[senseModePoints.Length];
            smpiSumMax = 0;
            senseModeCounts = new long[senseModePoints.Length][];
            for (int i = 0; i < senseModePeakIterationMax.Length; ++i)
            {
                int dimension = 2 * senseModePoints[i].Width + 1;
                stepPoints += dimension;
                senseModeCounts[i] = new long[dimension];
                senseModePeakIterationMax[i] = senseModePoints[i].Iterations;
                smpiSumMax += senseModePoints[i].Iterations; ;
            }
            //senseModePeak = 0;
        }
        protected override void saveData()
        {
            if (senseModePoints[senseModePeak].Collector == 1)
                senseModeCounts[senseModePeak][(pointValue - 1) - senseModePoints[senseModePeak].Step + senseModePoints[senseModePeak].Width] += Device.Detector1;
            else
                senseModeCounts[senseModePeak][(pointValue - 1) - senseModePoints[senseModePeak].Step + senseModePoints[senseModePeak].Width] += Device.Detector2;
        }
        protected override void onCancel()
        {
            finishMeasure();
        }
        protected override void onExit()
        {
            // order is important here: points are saved from graph..
            finishMeasure();
            Config.AutoSavePreciseSpecterFile();
        }
        protected override bool toContinue()
        {
            if ((pointValue > (senseModePoints[senseModePeak].Step + senseModePoints[senseModePeak].Width)))
            {
                // modify pointValue in case of finished iteration
                --(senseModePeakIteration[senseModePeak]);
                --smpiSum;
                if (smpiSum <= 0)
                {
                    // all data acquired
                    return false;
                }
                for (int i = 0; i < senseModePoints.Length; ++i)//Поиск пика с оставшейся ненулевой итерацией. Но не более 1 цикла.
                {
                    ++senseModePeak;
                    if (senseModePeak >= senseModePoints.Length) senseModePeak = 0;
                    if (senseModePeakIteration[senseModePeak] > 0) break;
                }
                ushort nextPoint = (ushort)(senseModePoints[senseModePeak].Step - senseModePoints[senseModePeak].Width);
                if (pointValue > nextPoint)
                {
                    //!!!case of backward voltage change
                    customMeasure = new sendMeasure(Config.CommonOptions.bTime, Config.CommonOptions.eTime);
                }
                else
                {
                    //!!!case of forward voltage change
                    if (Config.CommonOptions.ForwardTimeEqualsBeforeTime)
                    {
                        customMeasure = new sendMeasure(Config.CommonOptions.befTime, Config.CommonOptions.eTime);
                    }
                    else
                    {
                        customMeasure = new sendMeasure(Config.CommonOptions.fTime, Config.CommonOptions.eTime);
                    }
                }
                pointValue = nextPoint;
                //old code:
                //Commander.Point = (ushort)(senseModePoints[senseModePeak].Step - senseModePoints[senseModePeak].Width);
                //Commander.AddToSend(new sendSVoltage(Commander.Point++));
            }
            return true;
        }
        private void finishMeasure()
        {
            Graph.updateGraphAfterPreciseMeasure(senseModeCounts, senseModePoints);
        }

        internal override void start()
        {
            if (noPoints)
            {
                cancelScan();
                return;
            }
            init();
            onNextStep();
        }
        protected void init()
        {
            base.start();
            smpiSum = smpiSumMax;
            senseModePeak = 0;
            senseModePeakIteration = senseModePeakIterationMax.Clone() as ushort[];
            pointValue = (ushort)(senseModePoints[senseModePeak].Step - senseModePoints[senseModePeak].Width);
        }
        internal override void updateGraph()
        {
            ushort pnt = pointValue;
            Graph.updateGraphDuringPreciseMeasure(--pnt, SenseModePeak);
        }
        internal override void refreshGraphics(mainForm form)
        {
            form.refreshGraphicsOnPreciseStep();
        }
        internal override int stepsCount()
        {
            return stepPoints;
        }
    }
}
