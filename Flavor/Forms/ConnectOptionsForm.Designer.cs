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
            this.serialPort_comboBox = new System.Windows.Forms.ComboBox();
            this.baudrate_comboBox = new System.Windows.Forms.ComboBox();
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
            // serialPort_comboBox
            // 
            this.serialPort_comboBox.FormattingEnabled = true;
            resources.ApplyResources(this.serialPort_comboBox, "serialPort_comboBox");
            this.serialPort_comboBox.Name = "serialPort_comboBox";
            // 
            // baudrate_comboBox
            // 
            this.baudrate_comboBox.FormattingEnabled = true;
            this.baudrate_comboBox.Items.AddRange(new object[] {
            resources.GetString("baudrate_comboBox.Items"),
            resources.GetString("baudrate_comboBox.Items1"),
            resources.GetString("baudrate_comboBox.Items2"),
            resources.GetString("baudrate_comboBox.Items3"),
            resources.GetString("baudrate_comboBox.Items4")});
            resources.ApplyResources(this.baudrate_comboBox, "baudrate_comboBox");
            this.baudrate_comboBox.Name = "baudrate_comboBox";
            // 
            // ConnectOptionsForm
            // 
            this.AcceptButton = ok_butt;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = cancel_butt;
            this.Controls.Add(cancel_butt);
            this.Controls.Add(ok_butt);
            this.Controls.Add(this.baudrate_comboBox);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.Controls.Add(this.serialPort_comboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectOptionsForm";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox serialPort_comboBox;
        private System.Windows.Forms.ComboBox baudrate_comboBox;
    }
}