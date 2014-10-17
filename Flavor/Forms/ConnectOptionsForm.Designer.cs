namespace Flavor.Forms {
    partial class ConnectOptionsForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectOptionsForm));
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Button ok_butt;
            System.Windows.Forms.Button cancel_butt;
            this.serialPortComboBox = new System.Windows.Forms.ComboBox();
            this.baudrateComboBox = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            ok_butt = new System.Windows.Forms.Button();
            cancel_butt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // ok_butt
            // 
            ok_butt.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(ok_butt, "ok_butt");
            ok_butt.Name = "ok_butt";
            ok_butt.UseVisualStyleBackColor = true;
            ok_butt.Click += new System.EventHandler(this.ok_butt_Click);
            // 
            // cancel_butt
            // 
            cancel_butt.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(cancel_butt, "cancel_butt");
            cancel_butt.Name = "cancel_butt";
            cancel_butt.UseVisualStyleBackColor = true;
            cancel_butt.Click += new System.EventHandler(this.cancel_butt_Click);
            // 
            // serialPortComboBox
            // 
            this.serialPortComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.serialPortComboBox, "serialPortComboBox");
            this.serialPortComboBox.Name = "serialPortComboBox";
            // 
            // baudrateComboBox
            // 
            this.baudrateComboBox.FormattingEnabled = true;
            this.baudrateComboBox.Items.AddRange(new object[] {
            resources.GetString("baudrateComboBox.Items"),
            resources.GetString("baudrateComboBox.Items1"),
            resources.GetString("baudrateComboBox.Items2"),
            resources.GetString("baudrateComboBox.Items3"),
            resources.GetString("baudrateComboBox.Items4")});
            resources.ApplyResources(this.baudrateComboBox, "baudrateComboBox");
            this.baudrateComboBox.Name = "baudrateComboBox";
            // 
            // ConnectOptionsForm
            // 
            this.AcceptButton = ok_butt;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = cancel_butt;
            this.Controls.Add(cancel_butt);
            this.Controls.Add(ok_butt);
            this.Controls.Add(this.baudrateComboBox);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.Controls.Add(this.serialPortComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectOptionsForm";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox serialPortComboBox;
        private System.Windows.Forms.ComboBox baudrateComboBox;
    }
}