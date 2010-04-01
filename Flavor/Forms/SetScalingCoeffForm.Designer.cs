namespace Flavor.Forms {
    partial class SetScalingCoeffForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Label stepLabel;
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
            stepLabel.AutoSize = true;
            stepLabel.Location = new System.Drawing.Point(12, 9);
            stepLabel.Name = "stepLabel";
            stepLabel.Size = new System.Drawing.Size(60, 13);
            stepLabel.TabIndex = 4;
            stepLabel.Text = "Ступенька";
            // 
            // massLabel
            // 
            massLabel.AutoSize = true;
            massLabel.Location = new System.Drawing.Point(115, 9);
            massLabel.Name = "massLabel";
            massLabel.Size = new System.Drawing.Size(40, 13);
            massLabel.TabIndex = 5;
            massLabel.Text = "Масса";
            // 
            // okButton
            // 
            okButton.Location = new System.Drawing.Point(12, 64);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.TabIndex = 0;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(93, 64);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Отмена";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // stepTextBox
            // 
            this.stepTextBox.Location = new System.Drawing.Point(12, 28);
            this.stepTextBox.Name = "stepTextBox";
            this.stepTextBox.ReadOnly = true;
            this.stepTextBox.Size = new System.Drawing.Size(100, 20);
            this.stepTextBox.TabIndex = 2;
            // 
            // massTextBox
            // 
            this.massTextBox.Location = new System.Drawing.Point(118, 28);
            this.massTextBox.Name = "massTextBox";
            this.massTextBox.Size = new System.Drawing.Size(100, 20);
            this.massTextBox.TabIndex = 3;
            // 
            // SetScalingCoeffForm
            // 
            this.AcceptButton = okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = cancelButton;
            this.ClientSize = new System.Drawing.Size(354, 99);
            this.Controls.Add(massLabel);
            this.Controls.Add(stepLabel);
            this.Controls.Add(this.massTextBox);
            this.Controls.Add(this.stepTextBox);
            this.Controls.Add(cancelButton);
            this.Controls.Add(okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SetScalingCoeffForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Установка коэффициента";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox stepTextBox;
        private System.Windows.Forms.TextBox massTextBox;
    }
}