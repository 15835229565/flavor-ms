using System;
using System.Windows.Forms;
using Flavor.Common.Data.Measure;

namespace Flavor.Controls {
    partial class PreciseMeasureGraphPanel: MeasureGraphPanel {
        public PreciseMeasureGraphPanel() {
            InitializeComponent();
        }

        void refreshGraphicsOnPreciseStep(PreciseEditorData peak) {
            // TODO: move logic up
            long? carbonDioxideCounts = null;
            long? oxygenCounts = null;
            if (peak.IsCarbonDioxide()) {
                // TODO: simplify
                if (peak.AssociatedPoints != null && peak.AssociatedPoints.PLSreference != null) {
                    carbonDioxideCounts = peak.AssociatedPoints.PLSreference.PeakSum;
                }
            } else if (peak.IsOxygen()) {
                if (peak.AssociatedPoints != null && peak.AssociatedPoints.PLSreference != null) {
                    oxygenCounts = peak.AssociatedPoints.PLSreference.PeakSum;
                }
            }
            if (carbonDioxideCounts != null && oxygenCounts != null && oxygenCounts != 0) {
                double ratio = (double)carbonDioxideCounts / (double)oxygenCounts;
                ratioTextLabel.Visible = true;
                ratioLabel.Visible = true;
                ratioLabel.Text = ratio.ToString("F2");
                ratioLabel.ForeColor = ratio > 1 ? System.Drawing.Color.Red : System.Drawing.Color.Green;
            }
            
            label37.Visible = true;
            peakNumberLabel.Text = (peak.pNumber + 1).ToString();
            peakNumberLabel.Visible = true;
            label39.Visible = true;
            peakCenterLabel.Text = peak.Step.ToString();
            peakCenterLabel.Visible = true;
            label41.Visible = true;
            peakWidthLabel.Text = peak.Width.ToString();
            peakWidthLabel.Visible = true;

            //scanRealTimeLabel.Visible = true;
            stepNumberLabel.Visible = true;
            //label35.Visible = true;
            label36.Visible = true;
            // TODO: modify to display any collector counts!
            if (peak.Collector == 1) {
                detector1CountsLabel.Visible = true;
                label15.Visible = true;
                detector2CountsLabel.Visible = false;
                label16.Visible = false;
                detector3CountsLabel.Visible = false;
                label0.Visible = false;
            } else if (peak.Collector == 2) {
                detector1CountsLabel.Visible = false;
                label15.Visible = false;
                detector2CountsLabel.Visible = true;
                label16.Visible = true;
                detector3CountsLabel.Visible = false;
                label0.Visible = false;
            } else {
                detector1CountsLabel.Visible = false;
                label15.Visible = false;
                detector2CountsLabel.Visible = false;
                label16.Visible = false;
                detector3CountsLabel.Visible = true;
                label0.Visible = true;
            }
        }
        protected override void prepareControls() {
            ratioTextLabel.Visible = false;
            ratioLabel.Visible = false;
            base.prepareControls();
        }
        public override void performStep(ushort pnt, uint[] counts) {
            base.performStep(pnt, counts);
            var peak = ((Graph.MeasureGraph)Graph).CurrentPeak;
            refreshGraphicsOnPreciseStep(peak);
        }
    }
}
