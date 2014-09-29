using System;
using System.Windows.Forms;
using MeasureGraph = Flavor.Common.Data.Measure.Graph.MeasureGraph;

namespace Flavor.Controls {
    partial class MeasureGraphPanel: GraphPanel/*, IMeasured*/ {
        public event EventHandler MeasureCancelRequested;
        protected virtual void OnMeasureCancelRequested() {
            MeasureCancelRequested.Raise(this, EventArgs.Empty);
        }
        //readonly int count;
        public MeasureGraphPanel() {
            InitializeComponent();
            // TODO: populate array of detectors labels. Move from ctor!
            //count = base.Graph.Collectors.Count;
        }
        public int ProgressMaximum { get; set; }
        void cancelScanButton_Click(object sender, EventArgs e) {
            cancelScanButton.Enabled = false;
            OnMeasureCancelRequested();
        }
        protected override void prepareControls() {
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
            detector3CountsLabel.Visible = false;
            label0.Visible = false;
            scanRealTimeLabel.Visible = false;
            stepNumberLabel.Visible = false;
            label35.Visible = false;
            label36.Visible = false;
            label37.Visible = false;

            cancelScanButton.Enabled = true;
            cancelScanButton.Visible = true;

            scanProgressBar.Value = 0;
            scanProgressBar.Cursor = Cursors.WaitCursor;
            if (ProgressMaximum == 0) {
                // complicated limit
                scanProgressBar.Maximum = 0;
                scanProgressBar.Style = ProgressBarStyle.Marquee;
            } else if (ProgressMaximum < 0) {
                // time limit (minutes)
                scanProgressBar.Maximum = -ProgressMaximum * 60;
                current = DateTime.Now;
            } else {
                // steps limit
                scanProgressBar.Maximum = ProgressMaximum;
                scanProgressBar.Style = ProgressBarStyle.Blocks;
                scanProgressBar.Step = 1;
            }
        
            // TODO: use collectors count to display proper data!
        }
        DateTime current;
        public virtual void performStep(ushort pnt, uint[] counts) {
            if (scanProgressBar.Style != ProgressBarStyle.Marquee) {
                if (ProgressMaximum > 0) {
                    if (scanProgressBar.Value >= scanProgressBar.Maximum) {
                        // if already full line - reinit
                        scanProgressBar.Value = 0;
                        scanProgressBar.Maximum = ProgressMaximum;
                    }
                } else {
                    // time limit
                    var now = DateTime.Now;
                    int step = (int)((now - current).TotalSeconds);
                    if (step > 0) {
                        current = now;
                        scanProgressBar.Step = step;
                    }
                }
                scanProgressBar.PerformStep();
            }
            stepNumberLabel.Text = pnt.ToString();
            
            //scanRealTimeLabel.Text = graph.CommonOptions.scanVoltageRealNew(pnt).ToString("f1");

            if (counts != null) {
                detector1CountsLabel.Text = counts[0].ToString();
                detector2CountsLabel.Text = counts[1].ToString();
                detector3CountsLabel.Text = counts[2].ToString();
                // TODO: variable collectors number
            }
        }

        protected sealed override void disableControls() {
            // what about other controls?
            scanProgressBar.Cursor = Cursors.Default;
            cancelScanButton.Visible = false;
        }
    }
}

