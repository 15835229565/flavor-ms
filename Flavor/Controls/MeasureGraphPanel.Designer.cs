namespace Flavor.Controls {
    partial class MeasureGraphPanel {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MeasureGraphPanel));
            this.label35 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.scanRealTimeLabel = new System.Windows.Forms.Label();
            this.stepNumberLabel = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.peakNumberLabel = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.peakCenterLabel = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.peakWidthLabel = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.detector1CountsLabel = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.detector2CountsLabel = new System.Windows.Forms.Label();
            this.scanProgressBar = new System.Windows.Forms.ProgressBar();
            this.cancelScanButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label35
            // 
            resources.ApplyResources(this.label35, "label35");
            this.label35.Name = "label35";
            // 
            // label36
            // 
            resources.ApplyResources(this.label36, "label36");
            this.label36.Name = "label36";
            // 
            // scanRealTimeLabel
            // 
            this.scanRealTimeLabel.BackColor = System.Drawing.SystemColors.Control;
            this.scanRealTimeLabel.ForeColor = System.Drawing.Color.Green;
            resources.ApplyResources(this.scanRealTimeLabel, "scanRealTimeLabel");
            this.scanRealTimeLabel.Name = "scanRealTimeLabel";
            // 
            // stepNumberLabel
            // 
            this.stepNumberLabel.BackColor = System.Drawing.SystemColors.Control;
            this.stepNumberLabel.ForeColor = System.Drawing.Color.Green;
            resources.ApplyResources(this.stepNumberLabel, "stepNumberLabel");
            this.stepNumberLabel.Name = "stepNumberLabel";
            // 
            // label37
            // 
            resources.ApplyResources(this.label37, "label37");
            this.label37.Name = "label37";
            // 
            // peakNumberLabel
            // 
            this.peakNumberLabel.BackColor = System.Drawing.SystemColors.Control;
            this.peakNumberLabel.ForeColor = System.Drawing.Color.Green;
            resources.ApplyResources(this.peakNumberLabel, "peakNumberLabel");
            this.peakNumberLabel.Name = "peakNumberLabel";
            // 
            // label39
            // 
            resources.ApplyResources(this.label39, "label39");
            this.label39.Name = "label39";
            // 
            // peakCenterLabel
            // 
            this.peakCenterLabel.BackColor = System.Drawing.SystemColors.Control;
            this.peakCenterLabel.ForeColor = System.Drawing.Color.Green;
            resources.ApplyResources(this.peakCenterLabel, "peakCenterLabel");
            this.peakCenterLabel.Name = "peakCenterLabel";
            // 
            // label41
            // 
            resources.ApplyResources(this.label41, "label41");
            this.label41.Name = "label41";
            // 
            // peakWidthLabel
            // 
            this.peakWidthLabel.BackColor = System.Drawing.SystemColors.Control;
            this.peakWidthLabel.ForeColor = System.Drawing.Color.Green;
            resources.ApplyResources(this.peakWidthLabel, "peakWidthLabel");
            this.peakWidthLabel.Name = "peakWidthLabel";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // detector1CountsLabel
            // 
            this.detector1CountsLabel.BackColor = System.Drawing.SystemColors.Control;
            this.detector1CountsLabel.ForeColor = System.Drawing.Color.Green;
            resources.ApplyResources(this.detector1CountsLabel, "detector1CountsLabel");
            this.detector1CountsLabel.Name = "detector1CountsLabel";
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // detector2CountsLabel
            // 
            this.detector2CountsLabel.BackColor = System.Drawing.SystemColors.Control;
            this.detector2CountsLabel.ForeColor = System.Drawing.Color.Green;
            resources.ApplyResources(this.detector2CountsLabel, "detector2CountsLabel");
            this.detector2CountsLabel.Name = "detector2CountsLabel";
            // 
            // scanProgressBar
            // 
            resources.ApplyResources(this.scanProgressBar, "scanProgressBar");
            this.scanProgressBar.Name = "scanProgressBar";
            // 
            // cancelScanButton
            // 
            resources.ApplyResources(this.cancelScanButton, "cancelScanButton");
            this.cancelScanButton.Name = "cancelScanButton";
            this.cancelScanButton.UseVisualStyleBackColor = true;
            this.cancelScanButton.Click += new System.EventHandler(this.cancelScanButton_Click);
            // 
            // MeasureGraphPanel
            // 
            this.Controls.Add(this.peakWidthLabel);
            this.Controls.Add(this.label41);
            this.Controls.Add(this.peakCenterLabel);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.peakNumberLabel);
            this.Controls.Add(this.label37);
            this.Controls.Add(this.stepNumberLabel);
            this.Controls.Add(this.label36);
            this.Controls.Add(this.scanRealTimeLabel);
            this.Controls.Add(this.label35);
            this.Controls.Add(this.scanProgressBar);
            this.Controls.Add(this.cancelScanButton);
            this.Controls.Add(this.detector2CountsLabel);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.detector1CountsLabel);
            this.Controls.Add(this.label15);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar scanProgressBar;
        private System.Windows.Forms.Button cancelScanButton;
        protected System.Windows.Forms.Label detector1CountsLabel;
        protected System.Windows.Forms.Label detector2CountsLabel;
        protected System.Windows.Forms.Label label15;
        protected System.Windows.Forms.Label label16;
        protected System.Windows.Forms.Label scanRealTimeLabel;
        protected System.Windows.Forms.Label stepNumberLabel;
        protected System.Windows.Forms.Label label37;
        protected System.Windows.Forms.Label label35;
        protected System.Windows.Forms.Label label36;
        protected System.Windows.Forms.Label peakNumberLabel;
        protected System.Windows.Forms.Label peakWidthLabel;
        protected System.Windows.Forms.Label label39;
        protected System.Windows.Forms.Label label41;
        protected System.Windows.Forms.Label peakCenterLabel;
    }
}
