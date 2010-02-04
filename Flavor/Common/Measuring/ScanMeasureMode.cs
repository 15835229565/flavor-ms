using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor
{
    class ScanMeasureMode: MeasureMode
    {
        public delegate void StatusCheckToggleEventHandler(bool isRareMode);

        //private bool scanning = false;
        public event Commander.ProgramEventHandler OnScanCancelled;
        public event StatusCheckToggleEventHandler OnStatusCheckToggled;

        public override void onUpdateCounts()
        {
            //lock here?
            if (!Commander.measureCancelRequested && (Commander.Point <= Config.ePoint))
            {
                Commander.AddToSend(new sendSVoltage(Commander.Point++));
            }
            else
            {
                if (!Commander.notRareModeRequested) OnStatusCheckToggled(!Commander.notRareModeRequested);//?
                Commander.AddToSend(new sendSVoltage(0, false));//Set ScanVoltage to low limit

                base.onUpdateCounts();
                
                OnScanCancelled();
                Commander.pStatePrev = Commander.pState;
                Commander.pState = Commander.programStates.Ready;
                Commander.pStatePrev = Commander.pState;
                Commander.measureCancelRequested = false;
                Config.AutoSaveSpecterFile();
            }
        }
        public override void start()
        {
            base.start();
            //lock here
            OnStatusCheckToggled(Commander.notRareModeRequested);//?
            Commander.Point = Config.sPoint;
            Commander.AddToSend(new sendSVoltage(Commander.Point++));
        }
    }
}
