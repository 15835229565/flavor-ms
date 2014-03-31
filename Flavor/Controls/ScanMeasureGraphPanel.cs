namespace Flavor.Controls {
    public partial class ScanMeasureGraphPanel: MeasureGraphPanel {
        public ScanMeasureGraphPanel(ushort start, ushort end) {
            InitializeComponent();
            setScanBounds(start, end);
        }

        private void refreshGraphicsOnScanStep() {
            detector1CountsLabel.Visible = true;
            label15.Visible = true;
            detector2CountsLabel.Visible = true;
            label16.Visible = true;

            scanRealTimeLabel.Visible = true;
            stepNumberLabel.Visible = true;
            label35.Visible = true;
            label36.Visible = true;
        }
        internal override void performStep() {
            base.performStep();
            refreshGraphicsOnScanStep();
        }
    }
}
