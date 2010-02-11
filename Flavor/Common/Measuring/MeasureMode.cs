using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.UI;

namespace Flavor.Common.Measuring
{
    abstract public class MeasureMode
    {
        public delegate void StatusCheckToggleEventHandler(bool isRareMode);
        event Commander.ProgramEventHandler OnScanCancelled;
        public event StatusCheckToggleEventHandler OnStatusCheckToggled;
        
        protected object locker = new object();
        private bool operating = false;

        protected ushort pointValue = 0;
        public ushort Point
        {
            get { return pointValue; }
            //set { pointValue = value; }
        }
        
        public bool isOperating
        {
            get { return operating; }
            /*protected set
            {
                lock (locker)
                {
                    if (value != operating)
                    {
                        operating = value;
                        //toggleOperation();
                    }
                }
            }*/
        }
        protected sendMeasure customMeasure = null;
        private ushort befTime;
        private ushort eTime;

        public virtual void onUpdateCounts() 
        {
            customMeasure = null;//ATTENTION! need to be modified if measure mode without waiting for count answer is applied
        }
        public virtual void start()
        {
            befTime = Config.CommonOptions.befTime;
            eTime = Config.CommonOptions.eTime;
            //first measure point with increased idle time
            customMeasure = new sendMeasure(befTime, eTime);
            operating = true;
        }
        abstract public void updateGraph();
        public void autoNextMeasure()
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
        }
        protected void toggleStatusCheck(bool isRareMode)
        {
            OnStatusCheckToggled(isRareMode);
        }
        protected void stop()
        {
            operating = false;
            Commander.AddToSend(new sendSVoltage(0));//Set ScanVoltage to low limit
        }
    }
}
