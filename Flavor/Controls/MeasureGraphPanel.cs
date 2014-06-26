using System;
using System.Windows.Forms;
using Graph = Flavor.Common.Data.Measure.Graph;
using CommonOptions = Flavor.Common.Settings.CommonOptions;

namespace Flavor.Controls {
    partial class MeasureGraphPanel: GraphPanel/*, IMeasured*/ {
        public event EventHandler MeasureCancelRequested;
        protected virtual void OnMeasureCancelRequested() {
            MeasureCancelRequested.Raise(this, EventArgs.Empty);
        }
        public MeasureGraphPanel() {
            InitializeComponent();
            // TODO: populate array of detectors labels. Move from ctor!
            //int count = base.Graph.Collectors.Count;
        }
        public int ProgressMaximum { get; set; }
        void cancelScanButton_Click(object sender, EventArgs e) {
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
            scanProgressBar.Maximum = ProgressMaximum;
            if (scanProgressBar.Maximum == 0) {
                scanProgressBar.Style = ProgressBarStyle.Marquee;
            } else {
                scanProgressBar.Style = ProgressBarStyle.Blocks;
                scanProgressBar.Step = 1;
            }

            scanProgressBar.Cursor = System.Windows.Forms.Cursors.WaitCursor;
        
            // TODO: use collectors count to display proper data!
        }
        public virtual void performStep(uint[] counts) {
            if (scanProgressBar.Style != ProgressBarStyle.Marquee) {
                if (scanProgressBar.Value == scanProgressBar.Maximum) {
                    // if already full line - reinit
                    scanProgressBar.Value = 0;
                    scanProgressBar.Maximum = ProgressMaximum;
                }
                scanProgressBar.PerformStep();
            }
            stepNumberLabel.Text = Graph.Instance.LastPoint.ToString();
            
            scanRealTimeLabel.Text = base.Graph.CommonOptions.scanVoltageNew(Graph.Instance.LastPoint).ToString("f1");
            
            detector1CountsLabel.Text = counts[0].ToString();
            detector2CountsLabel.Text = counts[1].ToString();
            // TODO: 3rd detector counts (and variable collectors number)
        }

        protected sealed override void disableControls() {
            // what about other controls?
            scanProgressBar.Cursor = System.Windows.Forms.Cursors.Default;
            cancelScanButton.Visible = false;
        }
    }
}

