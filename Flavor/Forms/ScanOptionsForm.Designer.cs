namespace Flavor
{
    partial class ScanOptionsForm
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
            this.scan_groupBox = new System.Windows.Forms.GroupBox();
            this.endScanNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.startScanNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.startScan = new System.Windows.Forms.Label();
            this.params_groupBox = new System.Windows.Forms.GroupBox();
            this.fV2NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.fV1NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.hCurrentNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.eCurrentNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.CPNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.iVoltageNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.idleTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.expTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label33 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ok_butt = new System.Windows.Forms.Button();
            this.cancel_butt = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.rareModeCheckBox = new System.Windows.Forms.CheckBox();
            this.scan_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endScanNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startScanNumericUpDown)).BeginInit();
            this.params_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fV2NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fV1NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hCurrentNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eCurrentNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CPNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iVoltageNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.idleTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expTimeNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // scan_groupBox
            // 
            this.scan_groupBox.Controls.Add(this.endScanNumericUpDown);
            this.scan_groupBox.Controls.Add(this.startScanNumericUpDown);
            this.scan_groupBox.Controls.Add(this.label1);
            this.scan_groupBox.Controls.Add(this.startScan);
            this.scan_groupBox.Location = new System.Drawing.Point(10, 10);
            this.scan_groupBox.Margin = new System.Windows.Forms.Padding(0);
            this.scan_groupBox.Name = "scan_groupBox";
            this.scan_groupBox.Padding = new System.Windows.Forms.Padding(0);
            this.scan_groupBox.Size = new System.Drawing.Size(270, 72);
            this.scan_groupBox.TabIndex = 0;
            this.scan_groupBox.TabStop = false;
            this.scan_groupBox.Text = "Интервал сканирования (0...1056)";
            // 
            // endScanNumericUpDown
            // 
            this.endScanNumericUpDown.Location = new System.Drawing.Point(195, 42);
            this.endScanNumericUpDown.Maximum = new decimal(new int[] {
            1056,
            0,
            0,
            0});
            this.endScanNumericUpDown.Name = "endScanNumericUpDown";
            this.endScanNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.endScanNumericUpDown.TabIndex = 4;
            this.endScanNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.endScanNumericUpDown.Value = new decimal(new int[] {
            1056,
            0,
            0,
            0});
            // 
            // startScanNumericUpDown
            // 
            this.startScanNumericUpDown.Location = new System.Drawing.Point(195, 16);
            this.startScanNumericUpDown.Maximum = new decimal(new int[] {
            1056,
            0,
            0,
            0});
            this.startScanNumericUpDown.Name = "startScanNumericUpDown";
            this.startScanNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.startScanNumericUpDown.TabIndex = 3;
            this.startScanNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Последняя ступенька";
            // 
            // startScan
            // 
            this.startScan.AutoSize = true;
            this.startScan.Location = new System.Drawing.Point(3, 18);
            this.startScan.Name = "startScan";
            this.startScan.Size = new System.Drawing.Size(100, 13);
            this.startScan.TabIndex = 0;
            this.startScan.Text = "Первая ступенька";
            // 
            // params_groupBox
            // 
            this.params_groupBox.Controls.Add(this.fV2NumericUpDown);
            this.params_groupBox.Controls.Add(this.fV1NumericUpDown);
            this.params_groupBox.Controls.Add(this.hCurrentNumericUpDown);
            this.params_groupBox.Controls.Add(this.eCurrentNumericUpDown);
            this.params_groupBox.Controls.Add(this.CPNumericUpDown);
            this.params_groupBox.Controls.Add(this.iVoltageNumericUpDown);
            this.params_groupBox.Controls.Add(this.idleTimeNumericUpDown);
            this.params_groupBox.Controls.Add(this.expTimeNumericUpDown);
            this.params_groupBox.Controls.Add(this.label33);
            this.params_groupBox.Controls.Add(this.label32);
            this.params_groupBox.Controls.Add(this.label7);
            this.params_groupBox.Controls.Add(this.label6);
            this.params_groupBox.Controls.Add(this.label5);
            this.params_groupBox.Controls.Add(this.label4);
            this.params_groupBox.Controls.Add(this.label3);
            this.params_groupBox.Controls.Add(this.label2);
            this.params_groupBox.Location = new System.Drawing.Point(10, 85);
            this.params_groupBox.Name = "params_groupBox";
            this.params_groupBox.Size = new System.Drawing.Size(270, 233);
            this.params_groupBox.TabIndex = 1;
            this.params_groupBox.TabStop = false;
            this.params_groupBox.Text = "Общие параметры";
            // 
            // fV2NumericUpDown
            // 
            this.fV2NumericUpDown.DecimalPlaces = 1;
            this.fV2NumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.fV2NumericUpDown.Location = new System.Drawing.Point(195, 201);
            this.fV2NumericUpDown.Maximum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.fV2NumericUpDown.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.fV2NumericUpDown.Name = "fV2NumericUpDown";
            this.fV2NumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.fV2NumericUpDown.TabIndex = 33;
            this.fV2NumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.fV2NumericUpDown.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // fV1NumericUpDown
            // 
            this.fV1NumericUpDown.DecimalPlaces = 1;
            this.fV1NumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.fV1NumericUpDown.Location = new System.Drawing.Point(195, 175);
            this.fV1NumericUpDown.Maximum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.fV1NumericUpDown.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.fV1NumericUpDown.Name = "fV1NumericUpDown";
            this.fV1NumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.fV1NumericUpDown.TabIndex = 32;
            this.fV1NumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.fV1NumericUpDown.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // hCurrentNumericUpDown
            // 
            this.hCurrentNumericUpDown.DecimalPlaces = 2;
            this.hCurrentNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.hCurrentNumericUpDown.Location = new System.Drawing.Point(195, 149);
            this.hCurrentNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.hCurrentNumericUpDown.Name = "hCurrentNumericUpDown";
            this.hCurrentNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.hCurrentNumericUpDown.TabIndex = 31;
            this.hCurrentNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.hCurrentNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // eCurrentNumericUpDown
            // 
            this.eCurrentNumericUpDown.DecimalPlaces = 1;
            this.eCurrentNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.eCurrentNumericUpDown.Location = new System.Drawing.Point(195, 123);
            this.eCurrentNumericUpDown.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.eCurrentNumericUpDown.Name = "eCurrentNumericUpDown";
            this.eCurrentNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.eCurrentNumericUpDown.TabIndex = 30;
            this.eCurrentNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.eCurrentNumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // CPNumericUpDown
            // 
            this.CPNumericUpDown.DecimalPlaces = 3;
            this.CPNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.CPNumericUpDown.Location = new System.Drawing.Point(195, 97);
            this.CPNumericUpDown.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.CPNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.CPNumericUpDown.Name = "CPNumericUpDown";
            this.CPNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.CPNumericUpDown.TabIndex = 29;
            this.CPNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.CPNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // iVoltageNumericUpDown
            // 
            this.iVoltageNumericUpDown.DecimalPlaces = 1;
            this.iVoltageNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.iVoltageNumericUpDown.Location = new System.Drawing.Point(195, 71);
            this.iVoltageNumericUpDown.Maximum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.iVoltageNumericUpDown.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.iVoltageNumericUpDown.Name = "iVoltageNumericUpDown";
            this.iVoltageNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.iVoltageNumericUpDown.TabIndex = 28;
            this.iVoltageNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.iVoltageNumericUpDown.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // idleTimeNumericUpDown
            // 
            this.idleTimeNumericUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.idleTimeNumericUpDown.Location = new System.Drawing.Point(195, 45);
            this.idleTimeNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.idleTimeNumericUpDown.Name = "idleTimeNumericUpDown";
            this.idleTimeNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.idleTimeNumericUpDown.TabIndex = 27;
            this.idleTimeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.idleTimeNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // expTimeNumericUpDown
            // 
            this.expTimeNumericUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.expTimeNumericUpDown.Location = new System.Drawing.Point(195, 19);
            this.expTimeNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.expTimeNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.expTimeNumericUpDown.Name = "expTimeNumericUpDown";
            this.expTimeNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.expTimeNumericUpDown.TabIndex = 26;
            this.expTimeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.expTimeNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(6, 203);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(184, 13);
            this.label33.TabIndex = 25;
            this.label33.Text = "Фокусирующее напр. (2) (20-150 В)";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(6, 177);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(184, 13);
            this.label32.TabIndex = 24;
            this.label32.Text = "Фокусирующее напр. (1) (20-150 В)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 151);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Ток нагрева (0-1 А)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Ток эмиссии (0-50 мкА)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Коэффициент деления";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(180, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Напряжение ионизации (20-150 В)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Время перехода (10-100 мс)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(174, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Время экспозиции (10-10000 мс)";
            // 
            // ok_butt
            // 
            this.ok_butt.Location = new System.Drawing.Point(125, 367);
            this.ok_butt.Name = "ok_butt";
            this.ok_butt.Size = new System.Drawing.Size(75, 23);
            this.ok_butt.TabIndex = 2;
            this.ok_butt.Text = "Сохранить";
            this.ok_butt.UseVisualStyleBackColor = true;
            this.ok_butt.Click += new System.EventHandler(this.ok_butt_Click);
            // 
            // cancel_butt
            // 
            this.cancel_butt.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel_butt.Location = new System.Drawing.Point(206, 367);
            this.cancel_butt.Name = "cancel_butt";
            this.cancel_butt.Size = new System.Drawing.Size(75, 23);
            this.cancel_butt.TabIndex = 3;
            this.cancel_butt.Text = "Отмена";
            this.cancel_butt.UseVisualStyleBackColor = true;
            this.cancel_butt.Click += new System.EventHandler(this.cancel_butt_Click);
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(44, 367);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 4;
            this.applyButton.Text = "Применить";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // rareModeCheckBox
            // 
            this.rareModeCheckBox.AutoSize = true;
            this.rareModeCheckBox.Location = new System.Drawing.Point(10, 324);
            this.rareModeCheckBox.Name = "rareModeCheckBox";
            this.rareModeCheckBox.Size = new System.Drawing.Size(287, 17);
            this.rareModeCheckBox.TabIndex = 5;
            this.rareModeCheckBox.Text = "Без уменьшения частоты запроса статуса прибора";
            this.rareModeCheckBox.UseVisualStyleBackColor = true;
            // 
            // ScanOptionsForm
            // 
            this.AcceptButton = this.ok_butt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel_butt;
            this.ClientSize = new System.Drawing.Size(292, 402);
            this.Controls.Add(this.rareModeCheckBox);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.cancel_butt);
            this.Controls.Add(this.ok_butt);
            this.Controls.Add(this.params_groupBox);
            this.Controls.Add(this.scan_groupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScanOptionsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки обзорного режима";
            this.scan_groupBox.ResumeLayout(false);
            this.scan_groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.endScanNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startScanNumericUpDown)).EndInit();
            this.params_groupBox.ResumeLayout(false);
            this.params_groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fV2NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fV1NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hCurrentNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eCurrentNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CPNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iVoltageNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.idleTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.expTimeNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox scan_groupBox;
        private System.Windows.Forms.Label startScan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox params_groupBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button ok_butt;
        private System.Windows.Forms.Button cancel_butt;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.NumericUpDown startScanNumericUpDown;
        private System.Windows.Forms.NumericUpDown endScanNumericUpDown;
        private System.Windows.Forms.NumericUpDown fV1NumericUpDown;
        private System.Windows.Forms.NumericUpDown hCurrentNumericUpDown;
        private System.Windows.Forms.NumericUpDown eCurrentNumericUpDown;
        private System.Windows.Forms.NumericUpDown CPNumericUpDown;
        private System.Windows.Forms.NumericUpDown iVoltageNumericUpDown;
        private System.Windows.Forms.NumericUpDown idleTimeNumericUpDown;
        private System.Windows.Forms.NumericUpDown expTimeNumericUpDown;
        private System.Windows.Forms.NumericUpDown fV2NumericUpDown;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.CheckBox rareModeCheckBox;
    }
}