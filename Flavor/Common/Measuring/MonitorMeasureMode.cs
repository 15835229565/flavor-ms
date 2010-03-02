using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Forms;

namespace Flavor.Common.Measuring
{
    internal class MonitorMeasureMode : PreciseMeasureMode
    {
        internal class MeasureStopper
        {
            private int counter;
            internal MeasureStopper()
            {
                counter = 4;//test
            }
            internal void next()
            {
                --counter;
            }
            internal bool ready()
            {
                return counter == 0;
            }
            internal int estimatedTurns()
            {
                // estimated operation duration
                return counter;
            }
        }
        internal class MeasureChecker
        {
            private ushort shift;
            private Utility.PreciseEditorData peak;
            internal MeasureChecker()
            {
                peak = Config.CheckerPeak;
                shift = 0;
            }
            internal bool isSpectrumValid()
            {
                //TODO: check here for spectrum errors
                return true;
            }
        }
        private MeasureStopper stopper;
        private MeasureChecker checker;

        internal MonitorMeasureMode()
            : base(Config.PreciseDataWithChecker)
        {
            stopper = new MeasureStopper();
            checker = new MeasureChecker();
        }
        protected override bool toContinue()
        {
            if (base.toContinue())
            {
                return true;
            }
            if (checker.isSpectrumValid()) {
                stopper.next();
            }
            if (stopper.ready())
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
            return base.stepsCount() * stopper.estimatedTurns();
        }
    }
}
