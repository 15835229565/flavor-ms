namespace Flavor.Forms {
    partial class SetNormalizationPeakForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Label peakLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetNormalizationPeakForm));
            System.Windows.Forms.Label coeffLabel;
            System.Windows.Forms.Button okButton;
            System.Windows.Forms.Button cancelButton;
            this.coeffTextBox = new System.Windows.Forms.TextBox();
            this.peakComboBox = new System.Windows.Forms.ComboBox();
            peakLabel = new System.Windows.Forms.Label();
            coeffLabel = new System.Windows.Forms.Label();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // peakLabel
            // 
            resources.ApplyResources(peakLabel, "peakLabel");
            peakLabel.Name = "peakLabel";
            // 
            // coeffLabel
            // 
            resources.ApplyResources(coeffLabel, "coeffLabel");
            coeffLabel.Name = "coeffLabel";
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
            // coeffTextBox
            // 
            resources.ApplyResources(this.coeffTextBox, "coeffTextBox");
            this.coeffTextBox.Name = "coeffTextBox";
            // 
            // peakComboBox
            // 
            resources.ApplyResources(this.peakComboBox, "peakComboBox");
            this.peakComboBox.Name = "peakComboBox";
            // 
            // SetNormalizationPeakForm
            // 
            this.AcceptButton = okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = cancelButton;
            this.Controls.Add(this.peakComboBox);
            this.Controls.Add(coeffLabel);
            this.Controls.Add(peakLabel);
            this.Controls.Add(this.coeffTextBox);
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

        private System.Windows.Forms.TextBox coeffTextBox;
        private System.Windows.Forms.ComboBox peakComboBox;
    }
}