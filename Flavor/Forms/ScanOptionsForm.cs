using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Flavor.Common;

namespace Flavor.Forms
{
    internal class ScanOptionsForm: OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private System.Windows.Forms.GroupBox scan_groupBox;
        private System.Windows.Forms.Label startScan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown startScanNumericUpDown;
        private System.Windows.Forms.NumericUpDown endScanNumericUpDown;
        
        private void InitializeComponent()
        {
            this.scan_groupBox = new System.Windows.Forms.GroupBox();
            this.endScanNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.startScanNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.startScan = new System.Windows.Forms.Label();
            this.scan_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endScanNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startScanNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // scan_groupBox
            // 
            this.scan_groupBox.Controls.Add(this.endScanNumericUpDown);
            this.scan_groupBox.Controls.Add(this.startScanNumericUpDown);
            this.scan_groupBox.Controls.Add(this.label1);
            this.scan_groupBox.Controls.Add(this.startScan);
            this.scan_groupBox.Location = new System.Drawing.Point(10, 10);
            this.scan_groupBox.Margin = new System.Windows.Forms.Padding(0);
            this.scan_groupBox.Name = "scan_groupBox";
            this.scan_groupBox.Padding = new System.Windows.Forms.Padding(0);
            this.scan_groupBox.Size = new System.Drawing.Size(270, 72);
            this.scan_groupBox.TabIndex = 0;
            this.scan_groupBox.TabStop = false;
            this.scan_groupBox.Text = string.Format("Интервал сканирования ({0}..{1})", Config.MIN_STEP, Config.MAX_STEP);
            // 
            // endScanNumericUpDown
            // 
            this.endScanNumericUpDown.Location = new System.Drawing.Point(195, 42);
            this.endScanNumericUpDown.Minimum = new decimal(new int[] {Config.MIN_STEP, 0, 0, 0});
            this.endScanNumericUpDown.Maximum = new decimal(new int[] {Config.MAX_STEP, 0, 0, 0});
            this.endScanNumericUpDown.Name = "endScanNumericUpDown";
            this.endScanNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.endScanNumericUpDown.TabIndex = 4;
            this.endScanNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // startScanNumericUpDown
            // 
            this.startScanNumericUpDown.Location = new System.Drawing.Point(195, 16);
            this.startScanNumericUpDown.Minimum = new decimal(new int[] { Config.MIN_STEP, 0, 0, 0 });
            this.startScanNumericUpDown.Maximum = new decimal(new int[] { Config.MAX_STEP, 0, 0, 0 });
            this.startScanNumericUpDown.Name = "startScanNumericUpDown";
            this.startScanNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.startScanNumericUpDown.TabIndex = 3;
            this.startScanNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Последняя ступенька";
            // 
            // startScan
            // 
            this.startScan.AutoSize = true;
            this.startScan.Location = new System.Drawing.Point(3, 18);
            this.startScan.Name = "startScan";
            this.startScan.Size = new System.Drawing.Size(100, 13);
            this.startScan.TabIndex = 0;
            this.startScan.Text = "Первая ступенька";
            this.Controls.Add(this.scan_groupBox);
            this.Name = "ScanOptionsForm";
            this.Text = "Настройки обзорного режима";
            this.scan_groupBox.ResumeLayout(false);
            this.scan_groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endScanNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startScanNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        internal ScanOptionsForm(): base()
        {
            InitializeComponent();
            loadStartEndData();
        }

        private void loadStartEndData()
        {
            startScanNumericUpDown.Value = (decimal)(Config.sPoint);
            endScanNumericUpDown.Value = (decimal)(Config.ePoint);
        }

        protected override void ok_butt_Click(object sender, EventArgs e)
        {
            if ((ushort)(startScanNumericUpDown.Value) > (ushort)(endScanNumericUpDown.Value))
            {
                startScanNumericUpDown.BackColor = Color.Red;
                endScanNumericUpDown.BackColor = Color.Red;
                return;
            }
            Config.saveScanOptions((ushort)(startScanNumericUpDown.Value), (ushort)(endScanNumericUpDown.Value));
            base.ok_butt_Click(sender, e);
        }

        protected override void applyButton_Click(object sender, EventArgs e)
        {
            if ((ushort)(startScanNumericUpDown.Value) <= (ushort)(endScanNumericUpDown.Value))
            {
                Config.saveScanOptions((ushort)(startScanNumericUpDown.Value), (ushort)(endScanNumericUpDown.Value));
                base.applyButton_Click(sender, e);
            }
            else
            {
                startScanNumericUpDown.BackColor = Color.Red;
                endScanNumericUpDown.BackColor = Color.Red;
            }
        }
    }
}