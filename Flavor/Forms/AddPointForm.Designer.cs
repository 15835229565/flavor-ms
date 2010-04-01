namespace Flavor.Forms {
    partial class AddPointForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Button cancelButton;
            Flavor.Controls.PreciseEditorLabelRow oneLabelRow;
            this.okButton = new System.Windows.Forms.Button();
            this.oneRow = new Flavor.Controls.PreciseEditorRow();
            cancelButton = new System.Windows.Forms.Button();
            oneLabelRow = new Flavor.Controls.PreciseEditorLabelRow();
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
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(93, 64);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Отмена";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // oneRow
            // 
            this.oneRow.Location = new System.Drawing.Point(13, 42);
            this.oneRow.Name = "oneRow";
            this.oneRow.Size = new System.Drawing.Size(330, 13);
            this.oneRow.TabIndex = 2;
            // 
            // oneLabelRow
            // 
            oneLabelRow.Location = new System.Drawing.Point(12, 8);
            oneLabelRow.Name = "oneLabelRow";
            oneLabelRow.Size = new System.Drawing.Size(310, 26);
            oneLabelRow.TabIndex = 3;
            // 
            // AddPointForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = cancelButton;
            this.ClientSize = new System.Drawing.Size(354, 99);
            this.Controls.Add(cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.oneRow);
            this.Controls.Add(oneLabelRow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AddPointForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Добавление точки";
            this.ResumeLayout(false);

        }

        #endregion

        protected Flavor.Controls.PreciseEditorRow oneRow;
        protected System.Windows.Forms.Button okButton;
    }
}