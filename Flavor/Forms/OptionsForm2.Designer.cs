namespace Flavor.Forms {
    partial class OptionsForm2 {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Button loadFileButton;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm2));
            System.Windows.Forms.Button saveFileButton;
            this.params_groupBox = new System.Windows.Forms.GroupBox();
            this.adjustSettingsCheckBox = new System.Windows.Forms.CheckBox();
            this.ok_butt = new System.Windows.Forms.Button();
            this.cancel_butt = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.rareModeCheckBox = new System.Windows.Forms.CheckBox();
            this.openCommonDataFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveCommonDataFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.treeView1 = new System.Windows.Forms.TreeView();
            loadFileButton = new System.Windows.Forms.Button();
            saveFileButton = new System.Windows.Forms.Button();
            this.params_groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // loadFileButton
            // 
            resources.ApplyResources(loadFileButton, "loadFileButton");
            loadFileButton.Name = "loadFileButton";
            loadFileButton.UseVisualStyleBackColor = true;
            loadFileButton.Click += new System.EventHandler(this.loadFileButton_Click);
            // 
            // saveFileButton
            // 
            resources.ApplyResources(saveFileButton, "saveFileButton");
            saveFileButton.Name = "saveFileButton";
            saveFileButton.UseVisualStyleBackColor = true;
            saveFileButton.Click += new System.EventHandler(this.saveFileButton_Click);
            // 
            // params_groupBox
            // 
            this.params_groupBox.Controls.Add(this.treeView1);
            this.params_groupBox.Controls.Add(this.adjustSettingsCheckBox);
            this.params_groupBox.Controls.Add(loadFileButton);
            this.params_groupBox.Controls.Add(saveFileButton);
            resources.ApplyResources(this.params_groupBox, "params_groupBox");
            this.params_groupBox.Name = "params_groupBox";
            this.params_groupBox.TabStop = false;
            // 
            // adjustSettingsCheckBox
            // 
            resources.ApplyResources(this.adjustSettingsCheckBox, "adjustSettingsCheckBox");
            this.adjustSettingsCheckBox.Name = "adjustSettingsCheckBox";
            this.adjustSettingsCheckBox.UseVisualStyleBackColor = true;
            this.adjustSettingsCheckBox.CheckedChanged += new System.EventHandler(this.adjustSettingsCheckBox_CheckedChanged);
            // 
            // ok_butt
            // 
            resources.ApplyResources(this.ok_butt, "ok_butt");
            this.ok_butt.Name = "ok_butt";
            this.ok_butt.UseVisualStyleBackColor = true;
            this.ok_butt.Click += new System.EventHandler(this.ok_butt_Click);
            // 
            // cancel_butt
            // 
            this.cancel_butt.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancel_butt, "cancel_butt");
            this.cancel_butt.Name = "cancel_butt";
            this.cancel_butt.UseVisualStyleBackColor = true;
            this.cancel_butt.Click += new System.EventHandler(this.cancel_butt_Click);
            // 
            // applyButton
            // 
            resources.ApplyResources(this.applyButton, "applyButton");
            this.applyButton.Name = "applyButton";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // rareModeCheckBox
            // 
            resources.ApplyResources(this.rareModeCheckBox, "rareModeCheckBox");
            this.rareModeCheckBox.Name = "rareModeCheckBox";
            this.rareModeCheckBox.UseVisualStyleBackColor = true;
            // 
            // openCommonDataFileDialog
            // 
            this.openCommonDataFileDialog.DefaultExt = "cdf";
            resources.ApplyResources(this.openCommonDataFileDialog, "openCommonDataFileDialog");
            // 
            // saveCommonDataFileDialog
            // 
            this.saveCommonDataFileDialog.DefaultExt = "cdf";
            resources.ApplyResources(this.saveCommonDataFileDialog, "saveCommonDataFileDialog");
            // 
            // treeView1
            // 
            resources.ApplyResources(this.treeView1, "treeView1");
            this.treeView1.Name = "treeView1";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("treeView1.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("treeView1.Nodes1")))});
            // 
            // OptionsForm2
            // 
            this.AcceptButton = this.ok_butt;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel_butt;
            this.Controls.Add(this.rareModeCheckBox);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.cancel_butt);
            this.Controls.Add(this.ok_butt);
            this.Controls.Add(this.params_groupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm2";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.params_groupBox.ResumeLayout(false);
            this.params_groupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        protected System.Windows.Forms.OpenFileDialog openCommonDataFileDialog;
        protected System.Windows.Forms.SaveFileDialog saveCommonDataFileDialog;
        protected System.Windows.Forms.Button cancel_butt;
        protected System.Windows.Forms.Button applyButton;
        protected System.Windows.Forms.Button ok_butt;
        protected System.Windows.Forms.GroupBox params_groupBox;
        private System.Windows.Forms.CheckBox adjustSettingsCheckBox;
        protected System.Windows.Forms.CheckBox rareModeCheckBox;
        private System.Windows.Forms.TreeView treeView1;
    }
}