namespace Flavor.Controls {
    partial class PreciseMeasureGraphPanel {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreciseMeasureGraphPanel));
            this.ratioLabel = new System.Windows.Forms.Label();
            this.ratioTextLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ratioLabel
            // 
            resources.ApplyResources(this.ratioLabel, "ratioLabel");
            this.ratioLabel.Name = "ratioLabel";
            // 
            // ratioTextLabel
            // 
            resources.ApplyResources(this.ratioTextLabel, "ratioTextLabel");
            this.ratioTextLabel.Name = "ratioTextLabel";
            // 
            // PreciseMeasureGraphPanel
            // 
            this.AutoScroll = true;
            this.Controls.SetChildIndex(this.label15, 0);
            this.Controls.SetChildIndex(this.detector1CountsLabel, 0);
            this.Controls.SetChildIndex(this.label16, 0);
            this.Controls.SetChildIndex(this.detector2CountsLabel, 0);
            this.Controls.SetChildIndex(this.label0, 0);
            this.Controls.SetChildIndex(this.detector3CountsLabel, 0);
            this.Controls.SetChildIndex(this.label35, 0);
            this.Controls.SetChildIndex(this.scanRealTimeLabel, 0);
            this.Controls.SetChildIndex(this.label36, 0);
            this.Controls.SetChildIndex(this.stepNumberLabel, 0);
            this.Controls.SetChildIndex(this.label37, 0);
            this.Controls.SetChildIndex(this.peakNumberLabel, 0);
            this.Controls.SetChildIndex(this.label39, 0);
            this.Controls.SetChildIndex(this.peakCenterLabel, 0);
            this.Controls.SetChildIndex(this.label41, 0);
            this.Controls.SetChildIndex(this.peakWidthLabel, 0);
            this.Controls.Add(this.ratioLabel);
            this.Controls.Add(this.ratioTextLabel);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.Label ratioLabel;
        private System.Windows.Forms.Label ratioTextLabel;

        #endregion
    }
}
