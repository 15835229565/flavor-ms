namespace Flavor
{
    partial class SetScalingCoeffForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.stepTextBox = new System.Windows.Forms.TextBox();
            this.massTextBox = new System.Windows.Forms.TextBox();
            this.stepLabel = new System.Windows.Forms.Label();
            this.massLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(12, 64);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(93, 64);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
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
            // stepLabel
            // 
            this.stepLabel.AutoSize = true;
            this.stepLabel.Location = new System.Drawing.Point(12, 9);
            this.stepLabel.Name = "stepLabel";
            this.stepLabel.Size = new System.Drawing.Size(60, 13);
            this.stepLabel.TabIndex = 4;
            this.stepLabel.Text = "Ступенька";
            // 
            // massLabel
            // 
            this.massLabel.AutoSize = true;
            this.massLabel.Location = new System.Drawing.Point(115, 9);
            this.massLabel.Name = "massLabel";
            this.massLabel.Size = new System.Drawing.Size(40, 13);
            this.massLabel.TabIndex = 5;
            this.massLabel.Text = "Масса";
            // 
            // SetScalingCoeffForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(354, 99);
            this.Controls.Add(this.massLabel);
            this.Controls.Add(this.stepLabel);
            this.Controls.Add(this.massTextBox);
            this.Controls.Add(this.stepTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
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

        protected System.Windows.Forms.Button okButton;
        protected System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox stepTextBox;
        private System.Windows.Forms.TextBox massTextBox;
        private System.Windows.Forms.Label stepLabel;
        private System.Windows.Forms.Label massLabel;
    }
}