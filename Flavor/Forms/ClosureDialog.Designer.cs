namespace Flavor.Forms {
    partial class ClosureDialog {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Button okButton;
            System.Windows.Forms.Button cancelButton;
            System.Windows.Forms.Label label1;
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Location = new System.Drawing.Point(12, 67);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.TabIndex = 0;
            okButton.Text = "Отключить!";
            okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(205, 67);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Дождаться";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(282, 26);
            label1.TabIndex = 2;
            label1.Text = "Следует дождаться отключения системы.\nОтключить программу несмотря на предупрежде" +
                "ние?";
            // 
            // ClosureDialog
            // 
            this.AcceptButton = cancelButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = cancelButton;
            this.ClientSize = new System.Drawing.Size(292, 101);
            this.Controls.Add(label1);
            this.Controls.Add(cancelButton);
            this.Controls.Add(okButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClosureDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Предупреждение об отключении";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    }
}