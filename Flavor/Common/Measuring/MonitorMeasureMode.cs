using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Forms;
using System.Timers;

namespace Flavor.Common.Measuring {
    internal class MonitorMeasureMode: PreciseMeasureMode {
        internal class MeasureStopper {
            private int counter;
            private Timer timer;
            private bool timerElapsed = false;
            internal MeasureStopper(int counterLimit, int timeLimit) {
                counter = counterLimit;
                if (timeLimit > 0) {
                    timer = new Timer();
                    // time in minutes
                    timer.Interval = timeLimit * 60000;
                    timer.AutoReset = false;
                    timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                }
            }

            void timer_Elapsed(object sender, ElapsedEventArgs e) {
                timerElapsed = true;
            }
            internal void next() {
                if (counter == -1) {
                    // produce infinite loop
                    return;
                }
                --counter;
            }
            internal bool ready() {
                return timerElapsed || counter == 0;
            }
            internal int estimatedTurns() {
                // estimated operation duration
                return counter;
            }

            internal void startTimer() {
                if (timer == null) {
                    return;
                }
                timer.Start();
            }
        }

        private MeasureStopper stopper;
        private Utility.PreciseEditorData peak;
        private bool spectrumIsValid = true;

        internal MonitorMeasureMode(short initialShift)
            : base(Config.PreciseDataWithChecker) {
            shift = initialShift;
            stopper = new MeasureStopper(Config.Iterations, 0);
            peak = Config.CheckerPeak;
        }
        protected override void onExit() {
            if (spectrumIsValid) {
                base.onExit();
            } else {
                base.onCancel();
            }
        }
        protected override bool toContinue() {
            if (base.toContinue()) {
                return true;
            }
            if (spectrumIsValid) {
                stopper.next();
            }
            if (stopper.ready()) {
                return false;
            }
            onExit();
            init();
            return true;
        }
        protected override bool isSpectrumValid(Utility.PreciseEditorData curPeak) {
            if (!curPeak.Equals(peak)) {
                // do not store value here!
                return true;
            }
            long[] counts = peakCounts(isCheckPeak);
            ushort width = peak.Width;
            if (counts.Length != 2 * width + 1) {
                // data mismatch
                return spectrumIsValid = false;
            }
            long max = -1;
            int index = -1;
            for (int i = 0; i < counts.Length; ++i) {
                long temp = counts[i];
                if (temp > max) {
                    max = temp;
                    index = i;
                }
            }
            
            counts.Initialize(); //!!!

            if (index != width) {
                shift += (short)(index - width);
                return spectrumIsValid = false;
            }
            return spectrumIsValid = true;
        }
        private bool isCheckPeak(Utility.PreciseEditorData ped) {
            // only in this situation (checkpeak marked with false use)
            if (ped.Use) {
                return false;
            }
            return true;
        }
        internal override bool start() {
            if (!base.start()) {
                return false;
            }
            stopper.startTimer();
            return true;
        }
        internal override void refreshGraphics(mainForm form) {
            form.refreshGraphicsOnMonitorStep();
        }
        internal override int stepsCount() {
            int stopperTurns = stopper.estimatedTurns();
            if (stopperTurns <= 0) {
                return 0;
            }
            return base.stepsCount() * stopperTurns;
        }
    }
}
