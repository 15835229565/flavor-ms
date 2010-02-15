using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Forms;

namespace Flavor.Common.Measuring
{
    class MonitorMeasureMode : PreciseMeasureMode
    {
        private int counter = 10;//test

        internal MonitorMeasureMode()
            : base()
        {
            //TODO: initialization logic
        }
        protected override void finishMeasure()
        {
            --counter;
            if (counter == 0)
            {
                stop();
                return;
            }
            start();
        }
        /*internal override void updateGraph()
        {
            
            //Graph.updateGraphDuringMonitorMeasure();
        }*/
        internal override void refreshGraphics(mainForm form)
        {
            form.refreshGraphicsOnMonitorStep();
        }
        internal override int stepsCount()
        {
            return base.stepsCount() * counter;
        }
    }
}
