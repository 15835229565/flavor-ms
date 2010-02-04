using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor.Common.Measuring
{
    public class ScanMeasureMode: MeasureMode
    {
        public override void onUpdateCounts()
        {
            if (!Commander.measureCancelRequested && (Commander.Point <= Config.ePoint))
            {
                Commander.AddToSend(new sendSVoltage(Commander.Point++));
            }
            else
            {
                //TODO
                /*if (scanning)
                {
                    if (!Commander.notRareModeRequested) Commander.StopScanStatusCheck();
                    scanning = false;
                    Commander.AddToSend(new sendSVoltage(0, false));//Set ScanVoltage to low limit
                    Commander.OnScanCancelled();
                    Commander.pStatePrev = Commander.pState;
                    Commander.pState = Commander.programStates.Ready;
                    Commander.pStatePrev = Commander.pState;
                    Commander.measureCancelRequested = false;
                    Config.AutoSaveSpecterFile();
                }*/
            }
        }
        public override void start()
        {
            Commander.Point = Config.sPoint;
            Commander.AddToSend(new sendSVoltage(Commander.Point++));
        }
    }
}
