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
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(cancelButton, "cancelButton");
            cancelButton.Name = "cancelButton";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // oneRow
            // 
            resources.ApplyResources(this.oneRow, "oneRow");
            this.oneRow.Name = "oneRow";
            // 
            // oneLabelRow
            // 
            resources.ApplyResources(oneLabelRow, "oneLabelRow");
            oneLabelRow.Name = "oneLabelRow";
            // 
            // AddPointForm
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = cancelButton;
            this.Controls.Add(cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.oneRow);
            this.Controls.Add(oneLabelRow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
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