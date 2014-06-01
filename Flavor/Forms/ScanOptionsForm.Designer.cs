namespace Flavor.Forms {
    partial class ScanOptionsForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Label startScan;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScanOptionsForm));
            System.Windows.Forms.Label label1;
            this.scan_groupBox = new System.Windows.Forms.GroupBox();
            this.endScanNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.startScanNumericUpDown = new System.Windows.Forms.NumericUpDown();
            startScan = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            this.params_groupBox.SuspendLayout();
            this.scan_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endScanNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startScanNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // startScan
            // 
            resources.ApplyResources(startScan, "startScan");
            startScan.Name = "startScan";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // scan_groupBox
            // 
            this.scan_groupBox.Controls.Add(this.endScanNumericUpDown);
            this.scan_groupBox.Controls.Add(this.startScanNumericUpDown);
            this.scan_groupBox.Controls.Add(label1);
            this.scan_groupBox.Controls.Add(startScan);
            resources.ApplyResources(this.scan_groupBox, "scan_groupBox");
            this.scan_groupBox.Name = "scan_groupBox";
            this.scan_groupBox.TabStop = false;
            // 
            // endScanNumericUpDown
            // 
            resources.ApplyResources(this.endScanNumericUpDown, "endScanNumericUpDown");
            this.endScanNumericUpDown.Name = "endScanNumericUpDown";
            // 
            // startScanNumericUpDown
            // 
            resources.ApplyResources(this.startScanNumericUpDown, "startScanNumericUpDown");
            this.startScanNumericUpDown.Name = "startScanNumericUpDown";
            // 
            // ScanOptionsForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.scan_groupBox);
            this.Name = "ScanOptionsForm";
            this.Controls.SetChildIndex(this.scan_groupBox, 0);
            this.Controls.SetChildIndex(this.params_groupBox, 0);
            this.Controls.SetChildIndex(this.ok_butt, 0);
            this.Controls.SetChildIndex(this.cancel_butt, 0);
            this.Controls.SetChildIndex(this.applyButton, 0);
            this.Controls.SetChildIndex(this.rareModeCheckBox, 0);
            this.params_groupBox.ResumeLayout(false);
            this.params_groupBox.PerformLayout();
            this.scan_groupBox.ResumeLayout(false);
            this.scan_groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endScanNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startScanNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox scan_groupBox;
        private System.Windows.Forms.NumericUpDown startScanNumericUpDown;
        private System.Windows.Forms.NumericUpDown endScanNumericUpDown;
    }
}
