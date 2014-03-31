using System;
using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Controls {
    public partial class PreciseMeasureGraphPanel: MeasureGraphPanel {
        public PreciseMeasureGraphPanel() {
            InitializeComponent();
        }
        private void refreshGraphicsOnPreciseStep() {
            var g = Graph as Graph.MeasureGraph;
            label37.Visible = true;
            peakNumberLabel.Text = (g.CurrentPeak.pNumber + 1).ToString();
            peakNumberLabel.Visible = true;
            label39.Visible = true;
            peakCenterLabel.Text = g.CurrentPeak.Step.ToString();
            peakCenterLabel.Visible = true;
            label41.Visible = true;
            peakWidthLabel.Text = g.CurrentPeak.Width.ToString();
            peakWidthLabel.Visible = true;

            scanRealTimeLabel.Visible = true;
            stepNumberLabel.Visible = true;
            label35.Visible = true;
            label36.Visible = true;

            if (g.CurrentPeak.Collector == 1) {
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
        internal override void performStep() {
            base.performStep();
            refreshGraphicsOnPreciseStep();
        }
    }
}
