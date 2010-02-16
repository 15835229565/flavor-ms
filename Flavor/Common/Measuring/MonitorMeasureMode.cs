using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Forms;

namespace Flavor.Common.Measuring
{
    class MonitorMeasureMode : PreciseMeasureMode
    {
        private int counter = 4;//test

        internal MonitorMeasureMode()
            : base()
        {
            //TODO: initialization logic
        }
        protected override bool toContinue()
        {
            if (base.toContinue())
            {
                return true;
            }
            --counter;
            if (counter == 0)
            {
                return false;
            }
            base.onExit();
            base.init();
            return true;
        }
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
