using System;
using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Controls {
    public partial class MeasureGraphPanel: GraphPanel/*, IMeasured*/ {
        public event EventHandler MeasureCancelRequested;
        protected virtual void OnMeasureCancelRequested() {
            if (MeasureCancelRequested != null)
                MeasureCancelRequested(this, EventArgs.Empty);
        }
        public MeasureGraphPanel() {
            InitializeComponent();
        }
        private void cancelScanButton_Click(object sender, EventArgs e) {
            cancelScanButton.Enabled = false;
            OnMeasureCancelRequested();
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
            // TODO: set as property
            scanProgressBar.Maximum = Commander.CurrentMeasureMode.stepsCount();
            if (scanProgressBar.Maximum == 0) {
                scanProgressBar.Style = ProgressBarStyle.Marquee;
            } else {
                scanProgressBar.Style = ProgressBarStyle.Blocks;
                scanProgressBar.Step = 1;
            }

            scanProgressBar.Cursor = System.Windows.Forms.Cursors.WaitCursor;
        }
        internal virtual void performStep() {
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

        protected sealed override void disableControls() {
            // what about other controls?
            scanProgressBar.Cursor = System.Windows.Forms.Cursors.Default;
            cancelScanButton.Visible = false;
        }
    }
}

