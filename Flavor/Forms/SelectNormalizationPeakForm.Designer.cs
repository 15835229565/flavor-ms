namespace Flavor.Forms {
    partial class SetNormalizationPeakForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Label stepLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetNormalizationPeakForm));
            System.Windows.Forms.Label massLabel;
            System.Windows.Forms.Button okButton;
            System.Windows.Forms.Button cancelButton;
            this.stepTextBox = new System.Windows.Forms.TextBox();
            this.massTextBox = new System.Windows.Forms.TextBox();
            stepLabel = new System.Windows.Forms.Label();
            massLabel = new System.Windows.Forms.Label();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // stepLabel
            // 
            resources.ApplyResources(stepLabel, "stepLabel");
            stepLabel.Name = "stepLabel";
            // 
            // massLabel
            // 
            resources.ApplyResources(massLabel, "massLabel");
            massLabel.Name = "massLabel";
            // 
            // okButton
            // 
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(okButton, "okButton");
            okButton.Name = "okButton";
            okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(cancelButton, "cancelButton");
            cancelButton.Name = "cancelButton";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // stepTextBox
            // 
            resources.ApplyResources(this.stepTextBox, "stepTextBox");
            this.stepTextBox.Name = "stepTextBox";
            this.stepTextBox.ReadOnly = true;
            // 
            // massTextBox
            // 
            resources.ApplyResources(this.massTextBox, "massTextBox");
            this.massTextBox.Name = "massTextBox";
            // 
            // SetNormalizationPeakForm
            // 
            this.AcceptButton = okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = cancelButton;
            this.Controls.Add(massLabel);
            this.Controls.Add(stepLabel);
            this.Controls.Add(this.massTextBox);
            this.Controls.Add(this.stepTextBox);
            this.Controls.Add(cancelButton);
            this.Controls.Add(okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SetNormalizationPeakForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox stepTextBox;
        private System.Windows.Forms.TextBox massTextBox;
    }
}