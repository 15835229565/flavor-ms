using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.UI;
using Flavor.Forms;

namespace Flavor.Common.Measuring
{
    internal class ScanMeasureMode: MeasureMode
    {
        internal override void onUpdateCounts()
        {
            base.onUpdateCounts();
            //lock here?
            if (!Commander.measureCancelRequested && (pointValue <= Config.ePoint))
            {
                Commander.AddToSend(new sendSVoltage(pointValue++));
            }
            else
            {
                stop();
                Config.AutoSaveSpecterFile();
            }
        }
        internal override void start()
        {
            base.start();
            //lock here
            pointValue = Config.sPoint;
            Commander.AddToSend(new sendSVoltage(pointValue++));
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
    }
}
