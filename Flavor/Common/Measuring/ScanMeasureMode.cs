using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.UI;
using Flavor.Forms;

namespace Flavor.Common.Measuring
{
    internal class ScanMeasureMode: MeasureMode
    {
        private ushort sPoint;
        private ushort ePoint;
        internal ScanMeasureMode(): base()
        {
            sPoint = Config.sPoint;
            ePoint = Config.ePoint;
        }
        protected override void saveData() { }
        protected override void onCancel() { }
        protected override void onExit()
        {
            Config.AutoSaveSpecterFile();
        }
        protected override bool onNextStep()
        {
            Commander.AddToSend(new sendSVoltage(pointValue++));
            return true;
        }
        protected override bool toContinue()
        {
            return pointValue <= ePoint;
        }
        
        internal override void start()
        {
            base.start();
            //lock here
            pointValue = sPoint;
            onNextStep();
        }
        internal override void updateGraph() 
        {
            ushort pnt = pointValue;
            Graph.updateGraph(Device.Detector1, Device.Detector2, --pnt);
        }
        internal override void refreshGraphics(mainForm form)
        {
            form.refreshGraphicsOnScanStep();
        }
        internal override int stepsCount()
        {
            return ePoint - sPoint;
        }
    }
}
