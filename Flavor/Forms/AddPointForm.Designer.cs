namespace Flavor.Forms {
    partial class AddPointForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Button cancelButton;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddPointForm));
            Flavor.Controls.PreciseEditorLabelRow oneLabelRow;
            this.okButton = new System.Windows.Forms.Button();
            this.oneRow = new Flavor.Controls.PreciseEditorRow();
            cancelButton = new System.Windows.Forms.Button();
            oneLabelRow = new Flavor.Controls.PreciseEditorLabelRow();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            cancelButton.AccessibleDescription = null;
            cancelButton.AccessibleName = null;
            resources.ApplyResources(cancelButton, "cancelButton");
            cancelButton.BackgroundImage = null;
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Font = null;
            cancelButton.Name = "cancelButton";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // oneLabelRow
            // 
            oneLabelRow.AccessibleDescription = null;
            oneLabelRow.AccessibleName = null;
            resources.ApplyResources(oneLabelRow, "oneLabelRow");
            oneLabelRow.BackgroundImage = null;
            oneLabelRow.Font = null;
            oneLabelRow.Name = "oneLabelRow";
            // 
            // okButton
            // 
            this.okButton.AccessibleDescription = null;
            this.okButton.AccessibleName = null;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackgroundImage = null;
            this.okButton.Font = null;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // oneRow
            // 
            this.oneRow.AccessibleDescription = null;
            this.oneRow.AccessibleName = null;
            resources.ApplyResources(this.oneRow, "oneRow");
            this.oneRow.BackgroundImage = null;
            this.oneRow.Font = null;
            this.oneRow.Name = "oneRow";
            // 
            // AddPointForm
            // 
            this.AcceptButton = this.okButton;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = cancelButton;
            this.Controls.Add(cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.oneRow);
            this.Controls.Add(oneLabelRow);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.Name = "AddPointForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        protected Flavor.Controls.PreciseEditorRow oneRow;
        protected System.Windows.Forms.Button okButton;
    }
}