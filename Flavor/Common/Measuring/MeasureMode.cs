using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.UI;
using Flavor.Forms;

namespace Flavor.Common.Measuring
{
    abstract internal class MeasureMode
    {
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

        internal MeasureMode()
        {
            befTime = Config.CommonOptions.befTime;
            eTime = Config.CommonOptions.eTime;
        }
        internal bool onUpdateCounts() 
        {
            customMeasure = null;//ATTENTION! need to be modified if measure mode without waiting for count answer is applied
            //lock here?
            saveData();
            if (Commander.measureCancelRequested)
            {
                stop();
                onCancel();
                return true;
            }
            if (toContinue())
            {
                if (!onNextStep())
                {
                    // TODO: cannot perform step!
                    cancelScan();
                    return false;
                }
            }
            else
            {
                onExit();
                stop();
            }
            return true;
        }
        abstract protected void saveData();
        abstract protected void onCancel();
        abstract protected void onExit();
        abstract protected bool onNextStep();
        abstract protected bool toContinue();
        internal virtual bool start()
        {
            //first measure point with increased idle time
            customMeasure = new sendMeasure(befTime, eTime);
            operating = true;
            return true;
        }
        abstract internal void updateGraph();
        abstract internal void refreshGraphics(mainForm form);
        abstract internal int stepsCount();
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
            Commander.cancelScan();
        }
        protected void stop()
        {
            operating = false;
            Commander.AddToSend(new sendSVoltage(0));//Set ScanVoltage to low limit
            cancelScan();
        }
    }
}
