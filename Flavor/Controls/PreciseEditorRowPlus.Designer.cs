using System.Windows.Forms;
using System;
namespace Flavor.Controls
{
    partial class PreciseEditorRowPlus
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.peakNumberLabel = new System.Windows.Forms.Label();
            this.usePeakCheckBox = new System.Windows.Forms.CheckBox();
            this.clearPeakButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lapsTextBox
            // 
            this.lapsTextBox.Location = new System.Drawing.Point(163, 0);
            // 
            // precTextBox
            // 
            this.precTextBox.Location = new System.Drawing.Point(215, 0);
            // 
            // commentTextBox
            // 
            this.commentTextBox.Location = new System.Drawing.Point(267, 0);
            // 
            // stepTextBox
            // 
            this.stepTextBox.Location = new System.Drawing.Point(37, 0);
            // 
            // colTextBox
            // 
            this.colTextBox.Location = new System.Drawing.Point(89, 0);
            // 
            // widthTextBox
            // 
            this.widthTextBox.Location = new System.Drawing.Point(111, 0);
            // 
            // peakNumberLabel
            // 
            this.peakNumberLabel.AutoSize = true;
            this.peakNumberLabel.BackColor = System.Drawing.SystemColors.Control;
            this.peakNumberLabel.Location = new System.Drawing.Point(0, 0);
            this.peakNumberLabel.Name = "peakNumberLabel";
            this.peakNumberLabel.Size = new System.Drawing.Size(0, 13);
            this.peakNumberLabel.TabIndex = 6;
            this.peakNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // usePeakCheckBox
            // 
            this.usePeakCheckBox.Location = new System.Drawing.Point(22, 0);
            this.usePeakCheckBox.Name = "usePeakCheckBox";
            this.usePeakCheckBox.Size = new System.Drawing.Size(13, 13);
            this.usePeakCheckBox.TabIndex = 7;
            // 
            // clearPeakButton
            // 
            this.clearPeakButton.Location = new System.Drawing.Point(369, 0);
            this.clearPeakButton.Margin = new System.Windows.Forms.Padding(1);
            this.clearPeakButton.Name = "clearPeakButton";
            this.clearPeakButton.Size = new System.Drawing.Size(13, 13);
            this.clearPeakButton.TabIndex = 8;
            this.clearPeakButton.Click += new System.EventHandler(this.clearPeakButton_Click);
            this.clearPeakButton.MouseHover += new System.EventHandler(this.clearPeakButton_MouseHover);
            // 
            // PreciseEditorRowPlus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.peakNumberLabel);
            this.Controls.Add(this.usePeakCheckBox);
            this.Controls.Add(this.clearPeakButton);
            this.Name = "PreciseEditorRowPlus";
            this.Size = new System.Drawing.Size(382, 13);
            this.Controls.SetChildIndex(this.widthTextBox, 0);
            this.Controls.SetChildIndex(this.colTextBox, 0);
            this.Controls.SetChildIndex(this.stepTextBox, 0);
            this.Controls.SetChildIndex(this.lapsTextBox, 0);
            this.Controls.SetChildIndex(this.precTextBox, 0);
            this.Controls.SetChildIndex(this.commentTextBox, 0);
            this.Controls.SetChildIndex(this.clearPeakButton, 0);
            this.Controls.SetChildIndex(this.usePeakCheckBox, 0);
            this.Controls.SetChildIndex(this.peakNumberLabel, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label peakNumberLabel;
        private CheckBox usePeakCheckBox;
        private Button clearPeakButton;
    }
}
