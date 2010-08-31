using System.Windows.Forms;
using System;
namespace Flavor.Controls {
    partial class PreciseEditorRowPlus {

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreciseEditorRowPlus));
            this.clearPeakButton = new System.Windows.Forms.Button();
            this.peakNumberLabel = new System.Windows.Forms.Label();
            this.usePeakCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lapsTextBox
            // 
            resources.ApplyResources(this.lapsTextBox, "lapsTextBox");
            // 
            // precTextBox
            // 
            resources.ApplyResources(this.precTextBox, "precTextBox");
            // 
            // commentTextBox
            // 
            resources.ApplyResources(this.commentTextBox, "commentTextBox");
            // 
            // stepTextBox
            // 
            this.stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            resources.ApplyResources(this.stepTextBox, "stepTextBox");
            this.stepTextBox.ReadOnly = false;
            // 
            // colTextBox
            // 
            this.colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            resources.ApplyResources(this.colTextBox, "colTextBox");
            // 
            // widthTextBox
            // 
            resources.ApplyResources(this.widthTextBox, "widthTextBox");
            // 
            // clearPeakButton
            // 
            resources.ApplyResources(this.clearPeakButton, "clearPeakButton");
            this.clearPeakButton.Name = "clearPeakButton";
            this.clearPeakButton.Click += new System.EventHandler(this.clearPeakButton_Click);
            // 
            // peakNumberLabel
            // 
            resources.ApplyResources(this.peakNumberLabel, "peakNumberLabel");
            this.peakNumberLabel.BackColor = System.Drawing.SystemColors.Control;
            this.peakNumberLabel.Name = "peakNumberLabel";
            // 
            // usePeakCheckBox
            // 
            resources.ApplyResources(this.usePeakCheckBox, "usePeakCheckBox");
            this.usePeakCheckBox.Name = "usePeakCheckBox";
            // 
            // PreciseEditorRowPlus
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.peakNumberLabel);
            this.Controls.Add(this.usePeakCheckBox);
            this.Controls.Add(this.clearPeakButton);
            this.Name = "PreciseEditorRowPlus";
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
