using System;
using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Controls {
    public partial class MeasureGraphPanel: GraphPanel {
        public MeasureGraphPanel() {
            InitializeComponent();
        }
        private void cancelScanButton_Click(object sender, EventArgs e) {
            cancelScanButton.Enabled = false;
            Commander.measureCancelRequested = true;
        }

        protected sealed override void prepareControls() {
            // put here code that only changes data source and refreshes
            //Elements are not visible until first real information is ready
            peakNumberLabel.Visible = false;
            label39.Visible = false;
            peakCenterLabel.Visible = false;
            label41.Visible = false;
            peakWidthLabel.Visible = false;
            detector1CountsLabel.Visible = false;
            label15.Visible = false;
            detector2CountsLabel.Visible = false;
            label16.Visible = false;
            scanRealTimeLabel.Visible = false;
            stepNumberLabel.Visible = false;
            label35.Visible = false;
            label36.Visible = false;
            label37.Visible = false;

            cancelScanButton.Enabled = true;
            cancelScanButton.Visible = true;

            scanProgressBar.Value = 0;
            scanProgressBar.Maximum = Commander.CurrentMeasureMode.stepsCount();
            if (scanProgressBar.Maximum == 0) {
                scanProgressBar.Style = ProgressBarStyle.Marquee;
            } else {
                scanProgressBar.Style = ProgressBarStyle.Blocks;
                scanProgressBar.Step = 1;
            }

            scanProgressBar.Cursor = System.Windows.Forms.Cursors.WaitCursor;
        }
        internal void performStep() {
            if (scanProgressBar.Style != ProgressBarStyle.Marquee) {
                if (scanProgressBar.Value == scanProgressBar.Maximum) {
                    // if already full line - reinit
                    scanProgressBar.Value = 0;
                    scanProgressBar.Maximum = Commander.CurrentMeasureMode.stepsCount();
                }
                scanProgressBar.PerformStep();
            }
            stepNumberLabel.Text = Graph.LastPoint.ToString();
            scanRealTimeLabel.Text = CommonOptions.scanVoltageReal(Graph.LastPoint).ToString("f1");
            detector1CountsLabel.Text = Device.Detector1.ToString();
            detector2CountsLabel.Text = Device.Detector2.ToString();
        }

        internal void overview_button_Click(ushort start, ushort end) {
            setScanBounds(start, end);
        }
        /*internal void sensmeasure_button_Click() {
            peakNumberLabel.Visible = true;
        }
        internal void monitorToolStripButton_Click() {
            peakNumberLabel.Visible = true;
        }*/
        internal void refreshGraphicsOnScanStep() {
            detector1CountsLabel.Visible = true;
            label15.Visible = true;
            detector2CountsLabel.Visible = true;
            label16.Visible = true;
            
            scanRealTimeLabel.Visible = true;
            stepNumberLabel.Visible = true;
            label35.Visible = true;
            label36.Visible = true;
        }
        internal void refreshGraphicsOnPreciseStep() {
            label37.Visible = true;
            peakNumberLabel.Text = (Graph.CurrentPeak.pNumber + 1).ToString();
            peakNumberLabel.Visible = true;
            label39.Visible = true;
            peakCenterLabel.Text = Graph.CurrentPeak.Step.ToString();
            peakCenterLabel.Visible = true;
            label41.Visible = true;
            peakWidthLabel.Text = Graph.CurrentPeak.Width.ToString();
            peakWidthLabel.Visible = true;
            
            scanRealTimeLabel.Visible = true;
            stepNumberLabel.Visible = true;
            label35.Visible = true;
            label36.Visible = true;
            
            if (Graph.CurrentPeak.Collector == 1) {
                detector1CountsLabel.Visible = true;
                label15.Visible = true;
                detector2CountsLabel.Visible = false;
                label16.Visible = false;
            } else {
                detector1CountsLabel.Visible = false;
                label15.Visible = false;
                detector2CountsLabel.Visible = true;
                label16.Visible = true;
            }
        }
        protected sealed override void disableControls() {
            // what about other controls?
            scanProgressBar.Cursor = System.Windows.Forms.Cursors.Default;
            cancelScanButton.Visible = false;
        }
    }
}

