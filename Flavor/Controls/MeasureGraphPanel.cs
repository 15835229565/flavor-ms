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

        internal override void prepareControlsOnMeasureStart() {
            base.prepareControlsOnMeasureStart();

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
        }
        internal void RefreshGraph() {
            if (scanProgressBar.Style != ProgressBarStyle.Marquee) {
                if (scanProgressBar.Value == scanProgressBar.Maximum) {
                    // if already full line - reinit
                    scanProgressBar.Value = 0;
                    scanProgressBar.Maximum = Commander.CurrentMeasureMode.stepsCount();
                }
                scanProgressBar.PerformStep();
            }
            stepNumberLabel.Text = Graph.LastPoint.ToString();
            scanRealTimeLabel.Text = Graph.CommonOptions.scanVoltageReal(Graph.LastPoint).ToString("f1");
            detector1CountsLabel.Text = Device.Detector1.ToString();
            detector2CountsLabel.Text = Device.Detector2.ToString();
        }

        internal void overview_button_Click() {
            startScanTextLabel.Visible = true;
            label18.Visible = true;
            firstStepLabel.Visible = true;
            lastStepLabel.Visible = true;
            label37.Visible = false;
            peakNumberLabel.Visible = false;

            //TODO:!
            firstStepLabel.Text = Config.sPoint.ToString();
            lastStepLabel.Text = Config.ePoint.ToString();
        }
        internal void sensmeasure_button_Click() {
            startScanTextLabel.Visible = false;
            label18.Visible = false;
            firstStepLabel.Visible = false;
            lastStepLabel.Visible = false;
            label37.Visible = true;
            peakNumberLabel.Visible = true;
        }
        internal void monitorToolStripButton_Click() {
            startScanTextLabel.Visible = false;
            label18.Visible = false;
            firstStepLabel.Visible = false;
            lastStepLabel.Visible = false;
            label37.Visible = true;
            peakNumberLabel.Visible = true;
        }
        internal void refreshGraphicsOnScanStep() {
            detector1CountsLabel.Visible = true;
            label15.Visible = true;
            detector2CountsLabel.Visible = true;
            label16.Visible = true;
            peakNumberLabel.Visible = false;
            label39.Visible = false;
            peakCenterLabel.Visible = false;
            label41.Visible = false;
            peakWidthLabel.Visible = false;
        }
        internal void refreshGraphicsOnPreciseStep() {
            peakNumberLabel.Text = (Graph.CurrentPeak.pNumber + 1).ToString();
            peakNumberLabel.Visible = true;
            label39.Visible = true;
            peakCenterLabel.Text = Graph.CurrentPeak.Step.ToString();
            peakCenterLabel.Visible = true;
            label41.Visible = true;
            peakWidthLabel.Text = Graph.CurrentPeak.Width.ToString();
            peakWidthLabel.Visible = true;
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
        internal void cancelScan() {
            cancelScanButton.Visible = false;
        }
    }
}

