﻿using System.Windows.Forms;

namespace Flavor.Controls {
    partial class PreciseEditorLabelRowMinus {
        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Label colNumLabel;
            System.Windows.Forms.Label label10;
            this.peakCenterLabel = new System.Windows.Forms.Label();
            colNumLabel = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // peakCenterLabel
            // 
            this.peakCenterLabel.AutoSize = true;
            this.peakCenterLabel.BackColor = System.Drawing.SystemColors.Control;
            this.peakCenterLabel.Location = new System.Drawing.Point(0, 0);
            this.peakCenterLabel.Name = "peakCenterLabel";
            this.peakCenterLabel.Size = new System.Drawing.Size(0, 13);
            this.peakCenterLabel.TabIndex = 0;
            this.peakCenterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // colNumLabel
            // 
            colNumLabel.AutoSize = true;
            colNumLabel.Location = new System.Drawing.Point(50, 13);
            colNumLabel.Name = "colNumLabel";
            colNumLabel.Size = new System.Drawing.Size(29, 13);
            colNumLabel.TabIndex = 1;
            colNumLabel.Text = "Кол.";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.BackColor = System.Drawing.SystemColors.Control;
            label10.Location = new System.Drawing.Point(75, 13);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(46, 13);
            label10.TabIndex = 2;
            label10.Text = "Ширина";
            // 
            // PreciseEditorLabelRowMinus
            // 
            this.Controls.Add(colNumLabel);
            this.Controls.Add(label10);
            this.Controls.Add(this.peakCenterLabel);
            this.Name = "PreciseEditorLabelRowMinus";
            this.Size = new System.Drawing.Size(124, 26);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        protected System.Windows.Forms.Label peakCenterLabel;
    }
}