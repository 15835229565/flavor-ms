using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.UI;
using Flavor.Forms;

namespace Flavor.Common.Measuring
{
    abstract internal class MeasureMode
    {
        internal event Commander.ProgramEventHandler OnScanCancelled;
        
        protected object locker = new object();
        private bool operating = false;

        internal bool isOperating
        {
            get { return operating; }
        }

        protected ushort pointValue = 0;
        internal ushort Point
        {
            get { return pointValue; }
        }

        protected sendMeasure customMeasure = null;
        private ushort befTime;
        private ushort eTime;

        internal virtual void onUpdateCounts() 
        {
            customMeasure = null;//ATTENTION! need to be modified if measure mode without waiting for count answer is applied
        }
        internal virtual void start()
        {
            befTime = Config.CommonOptions.befTime;
            eTime = Config.CommonOptions.eTime;
            //first measure point with increased idle time
            customMeasure = new sendMeasure(befTime, eTime);
            OnScanCancelled += new Commander.ProgramEventHandler(Commander.cancelScan);

            operating = true;
        }
        abstract internal void updateGraph();
        abstract internal void refreshGraphics(mainForm form);
        internal void autoNextMeasure()
        {
            if (operating)
            {
                if (customMeasure == null)
                {
                    Commander.AddToSend(new sendMeasure());
                }
                else
                {
                    Commander.AddToSend(customMeasure);
                }
            }
        }
        protected void cancelScan()
        {
            OnScanCancelled();
            OnScanCancelled -= new Commander.ProgramEventHandler(Commander.cancelScan);
        }
        protected void stop()
        {
            operating = false;
            Commander.AddToSend(new sendSVoltage(0));//Set ScanVoltage to low limit
            cancelScan();
        }
    }
}
