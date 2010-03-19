using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Forms;

namespace Flavor.Common.Measuring
{
    internal class MonitorMeasureMode: PreciseMeasureMode
    {
        internal class MeasureStopper
        {
            private int counter;
            internal MeasureStopper(int counterLimit)
            {
                counter = counterLimit;
            }
            internal void next()
            {
                if (counter == -1) 
                {
                    // produce infinite loop
                    return; 
                }
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

        private MeasureStopper stopper;
        private Utility.PreciseEditorData peak;

        internal MonitorMeasureMode(short initialShift): base(Config.PreciseDataWithChecker)
        {
            shift = initialShift;
            stopper = new MeasureStopper(Config.Iterations);
            peak = Config.CheckerPeak;
        }
        protected override bool toContinue()
        {
            if (base.toContinue())
            {
                return true;
            }
            if (isSpectrumValid()) {
                stopper.next();
            }
            if (stopper.ready())
            {
                return false;
            }
            onExit();
            init();
            return true;
        }
        private bool isSpectrumValid()
        {
            if (peak == null)
            {
                return true;
            }
            long[] counts = peakCounts(isCheckPeak);
            ushort width = peak.Width;
            if (counts.Length != 2 * width + 1)
            {
                // data mismatch
                return false;
            }
            long max = -1;
            int index = -1;
            for (int i = 0; i < counts.Length; ++i)
            {
                long temp = counts[i];
                if (temp > max)
                {
                    max = temp;
                    index = i;
                }
            }
            if (index != width)
            {
                shift += (short)(index - width);
                return false;
            }
            return true;
        }
        private bool isCheckPeak(Utility.PreciseEditorData ped)
        {
            // only in this situation (checkpeak marked with false use)
            if (ped.Use)
            {
                return false;
            }
            return true;
        }
        internal override void refreshGraphics(mainForm form)
        {
            form.refreshGraphicsOnMonitorStep();
        }
        internal override int stepsCount()
        {
            int stopperTurns = stopper.estimatedTurns();
            if (stopperTurns <= 0)
            {
                return 0;
            }
            return base.stepsCount() * stopperTurns;
        }
    }
}
