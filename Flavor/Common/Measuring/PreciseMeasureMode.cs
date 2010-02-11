using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.UI;

namespace Flavor.Common.Measuring
{
    class PreciseMeasureMode: MeasureMode
    {
        private static Utility.PreciseEditorData[] senseModePoints;

        private static int[][] senseModeCounts;

        private static byte senseModePeak = 0;

        public static Utility.PreciseEditorData SenseModePeak
        {
            get { return senseModePoints[senseModePeak]; }
        }

        private static ushort[] senseModePeakIteration;

        private static ushort smpiSum;

        public override void onUpdateCounts()
        {
            base.onUpdateCounts();
            if (!Commander.measureCancelRequested)
            {
                if (senseModePoints[senseModePeak].Collector == 1)
                    senseModeCounts[senseModePeak][(pointValue - 1) - senseModePoints[senseModePeak].Step + senseModePoints[senseModePeak].Width] += Device.Detector1;
                else
                    senseModeCounts[senseModePeak][(pointValue - 1) - senseModePoints[senseModePeak].Step + senseModePoints[senseModePeak].Width] += Device.Detector2;
                if ((pointValue <= (senseModePoints[senseModePeak].Step + senseModePoints[senseModePeak].Width)))
                {
                    Commander.AddToSend(new sendSVoltage(pointValue++));
                }
                else
                {
                    --(senseModePeakIteration[senseModePeak]);
                    --smpiSum;
                    if (smpiSum > 0)
                    {
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
                        Commander.AddToSend(new sendSVoltage(pointValue++));
                        //old code:
                        //Commander.Point = (ushort)(senseModePoints[senseModePeak].Step - senseModePoints[senseModePeak].Width);
                        //Commander.AddToSend(new sendSVoltage(Commander.Point++));
                    }
                    else
                    {
                        //TODO extract method
                        stop();
                        toggleStatusCheck(Commander.notRareModeRequested);//?

                        Commander.pStatePrev = Commander.pState;
                        Commander.pState = Commander.programStates.Ready;
                        Commander.pStatePrev = Commander.pState;
                        cancelScan();//!!!
                        Graph.updateGraphAfterPreciseMeasure(senseModeCounts, senseModePoints);
                        Config.AutoSavePreciseSpecterFile();
                    }
                }
            }
            else
            {
                //TODO extract method
                stop();
                cancelScan();//!!!
                Graph.updateGraphAfterPreciseMeasure(senseModeCounts, senseModePoints);
                toggleStatusCheck(Commander.notRareModeRequested);//?
                Commander.AddToSend(new sendSVoltage(0));//Set ScanVoltage to low limit
                Commander.pStatePrev = Commander.pState;
                Commander.pState = Commander.programStates.Ready;
                Commander.pStatePrev = Commander.pState;
                Commander.measureCancelRequested = false;
            }
        }
        public override void start()
        {
            base.start();
            if (Config.PreciseData.Count > 0)
            {
                //Sort in increased order
                //Config.PreciseData.Sort(ComparePreciseEditorDataByPeakValue);
                //Config.PreciseData.Sort(ComparePreciseEditorDataByUseFlagAndPeakValue);
                //senseModePoints = Config.PreciseData.ToArray();
                List<Utility.PreciseEditorData> temp = Config.PreciseData.FindAll(Utility.PeakIsUsed);
                temp.Sort(Utility.ComparePreciseEditorDataByPeakValue);
                senseModePoints = temp.ToArray();
                senseModePeakIteration = new ushort[senseModePoints.Length];
                smpiSum = 0;
                senseModeCounts = new int[senseModePoints.Length][];
                for (int i = 0; i < senseModePeakIteration.Length; ++i)
                {
                    senseModeCounts[i] = new int[2 * senseModePoints[i].Width + 1];
                    senseModePeakIteration[i] = senseModePoints[i].Iterations;
                    smpiSum += senseModePoints[i].Iterations; ;
                }
                senseModePeak = 0;
                pointValue = (ushort)(senseModePoints[senseModePeak].Step - senseModePoints[senseModePeak].Width);
                Commander.AddToSend(new sendSVoltage(pointValue++));
            }
            else
            {
                toggleStatusCheck(Commander.notRareModeRequested);//?
                Commander.pStatePrev = Commander.pState;
                Commander.pState = Commander.programStates.Ready;// ATTENTION!
                Commander.pStatePrev = Commander.pState;
            }
        }
        public override void updateGraph()
        {
            ushort pnt = pointValue;
            Graph.updateGraphDuringPreciseMeasure(--pnt, SenseModePeak);
        }
    }
}
