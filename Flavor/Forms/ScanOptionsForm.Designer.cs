namespace Flavor.Forms {
    partial class ScanOptionsForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Label startScan;
            System.Windows.Forms.Label label1;
            this.scan_groupBox = new System.Windows.Forms.GroupBox();
            this.endScanNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.startScanNumericUpDown = new System.Windows.Forms.NumericUpDown();
            startScan = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.fV1NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hCurrentNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eCurrentNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CPNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iVoltageNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.idleTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fV2NumericUpDown)).BeginInit();
            this.params_groupBox.SuspendLayout();
            this.scan_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endScanNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startScanNumericUpDown)).BeginInit();
            this.SuspendLayout();
            this.params_groupBox.Controls.SetChildIndex(this.expTimeNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.idleTimeNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.iVoltageNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.CPNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.eCurrentNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.hCurrentNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.fV1NumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.fV2NumericUpDown, 0);
            // 
            // startScan
            // 
            startScan.AutoSize = true;
            startScan.Location = new System.Drawing.Point(3, 18);
            startScan.Name = "startScan";
            startScan.Size = new System.Drawing.Size(100, 13);
            startScan.TabIndex = 0;
            startScan.Text = "Первая ступенька";
            // 
            // scan_groupBox
            // 
            this.scan_groupBox.Controls.Add(this.endScanNumericUpDown);
            this.scan_groupBox.Controls.Add(this.startScanNumericUpDown);
            this.scan_groupBox.Controls.Add(label1);
            this.scan_groupBox.Controls.Add(startScan);
            this.scan_groupBox.Location = new System.Drawing.Point(10, 10);
            this.scan_groupBox.Margin = new System.Windows.Forms.Padding(0);
            this.scan_groupBox.Name = "scan_groupBox";
            this.scan_groupBox.Padding = new System.Windows.Forms.Padding(0);
            this.scan_groupBox.Size = new System.Drawing.Size(270, 72);
            this.scan_groupBox.TabIndex = 0;
            this.scan_groupBox.TabStop = false;
            // 
            // endScanNumericUpDown
            // 
            this.endScanNumericUpDown.Location = new System.Drawing.Point(195, 42);
            this.endScanNumericUpDown.Name = "endScanNumericUpDown";
            this.endScanNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.endScanNumericUpDown.TabIndex = 4;
            this.endScanNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // startScanNumericUpDown
            // 
            this.startScanNumericUpDown.Location = new System.Drawing.Point(195, 16);
            this.startScanNumericUpDown.Name = "startScanNumericUpDown";
            this.startScanNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.startScanNumericUpDown.TabIndex = 3;
            this.startScanNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 44);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(118, 13);
            label1.TabIndex = 2;
            label1.Text = "Последняя ступенька";
            // 
            // ScanOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(292, 432);
            this.Controls.Add(this.scan_groupBox);
            this.Name = "ScanOptionsForm";
            this.Text = "Настройки обзорного режима";
            this.Controls.SetChildIndex(this.scan_groupBox, 0);
            this.Controls.SetChildIndex(this.params_groupBox, 0);
            this.Controls.SetChildIndex(this.ok_butt, 0);
            this.Controls.SetChildIndex(this.cancel_butt, 0);
            this.Controls.SetChildIndex(this.applyButton, 0);
            this.Controls.SetChildIndex(this.rareModeCheckBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.fV1NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hCurrentNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eCurrentNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CPNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iVoltageNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.idleTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.expTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fV2NumericUpDown)).EndInit();
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
