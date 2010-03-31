namespace Flavor.Forms
{
    partial class ConnectOptionsForm
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
            System.Windows.Forms.Label label1;
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
            // serialPort_comboBox
            // 
            this.serialPort_comboBox.FormattingEnabled = true;
            this.serialPort_comboBox.Location = new System.Drawing.Point(15, 50);
            this.serialPort_comboBox.Name = "serialPort_comboBox";
            this.serialPort_comboBox.Size = new System.Drawing.Size(121, 21);
            this.serialPort_comboBox.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            label1.Location = new System.Drawing.Point(25, 20);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(111, 15);
            label1.TabIndex = 1;
            label1.Text = "Доступные порты";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            label2.Location = new System.Drawing.Point(179, 20);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(134, 15);
            label2.TabIndex = 2;
            label2.Text = "Скорость соединения";
            // 
            // baudrate_comboBox
            // 
            this.baudrate_comboBox.FormattingEnabled = true;
            this.baudrate_comboBox.Items.AddRange(new object[] {
            "115200",
            "57600",
            "38400",
            "19200",
            "9600"});
            this.baudrate_comboBox.Location = new System.Drawing.Point(192, 50);
            this.baudrate_comboBox.Name = "baudrate_comboBox";
            this.baudrate_comboBox.Size = new System.Drawing.Size(121, 21);
            this.baudrate_comboBox.TabIndex = 3;
            // 
            // ok_butt
            // 
            ok_butt.DialogResult = System.Windows.Forms.DialogResult.OK;
            ok_butt.Location = new System.Drawing.Point(157, 93);
            ok_butt.Name = "ok_butt";
            ok_butt.Size = new System.Drawing.Size(75, 23);
            ok_butt.TabIndex = 4;
            ok_butt.Text = "Сохранить";
            ok_butt.UseVisualStyleBackColor = true;
            ok_butt.Click += new System.EventHandler(this.ok_butt_Click);
            // 
            // cancel_butt
            // 
            cancel_butt.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancel_butt.Location = new System.Drawing.Point(238, 93);
            cancel_butt.Name = "cancel_butt";
            cancel_butt.Size = new System.Drawing.Size(75, 23);
            cancel_butt.TabIndex = 5;
            cancel_butt.Text = "Отмена";
            cancel_butt.UseVisualStyleBackColor = true;
            cancel_butt.Click += new System.EventHandler(this.cancel_butt_Click);
            // 
            // ConnectOptionsForm
            // 
            this.AcceptButton = ok_butt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = cancel_butt;
            this.ClientSize = new System.Drawing.Size(337, 128);
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
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки соединения";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox serialPort_comboBox;
        private System.Windows.Forms.ComboBox baudrate_comboBox;
    }
}