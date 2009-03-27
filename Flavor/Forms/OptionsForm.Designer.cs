namespace Flavor
{
    partial class OptionsForm
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
            this.params_groupBox = new System.Windows.Forms.GroupBox();
            this.loadFileButton = new System.Windows.Forms.Button();
            this.saveFileButton = new System.Windows.Forms.Button();
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
            this.openCommonDataFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveCommonDataFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.adjustSettingsCheckBox = new System.Windows.Forms.CheckBox();
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
            // params_groupBox
            // 
            this.params_groupBox.Controls.Add(this.adjustSettingsCheckBox);
            this.params_groupBox.Controls.Add(this.loadFileButton);
            this.params_groupBox.Controls.Add(this.saveFileButton);
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
            this.params_groupBox.Size = new System.Drawing.Size(270, 283);
            this.params_groupBox.TabIndex = 1;
            this.params_groupBox.TabStop = false;
            this.params_groupBox.Text = "����� ���������";
            // 
            // loadFileButton
            // 
            this.loadFileButton.Location = new System.Drawing.Point(135, 250);
            this.loadFileButton.Name = "loadFileButton";
            this.loadFileButton.Size = new System.Drawing.Size(120, 23);
            this.loadFileButton.TabIndex = 34;
            this.loadFileButton.Text = "��������� �� �����";
            this.loadFileButton.UseVisualStyleBackColor = true;
            this.loadFileButton.Click += new System.EventHandler(this.loadFileButton_Click);
            // 
            // saveFileButton
            // 
            this.saveFileButton.Location = new System.Drawing.Point(9, 250);
            this.saveFileButton.Name = "saveFileButton";
            this.saveFileButton.Size = new System.Drawing.Size(112, 23);
            this.saveFileButton.TabIndex = 6;
            this.saveFileButton.Text = "��������� � ����";
            this.saveFileButton.UseVisualStyleBackColor = true;
            this.saveFileButton.Click += new System.EventHandler(this.saveFileButton_Click);
            // 
            // fV2NumericUpDown
            // 
            this.fV2NumericUpDown.DecimalPlaces = 1;
            this.fV2NumericUpDown.Enabled = false;
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
            this.fV1NumericUpDown.Enabled = false;
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
            this.hCurrentNumericUpDown.Enabled = false;
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
            this.CPNumericUpDown.Enabled = false;
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
            this.idleTimeNumericUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
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
            this.label33.Text = "������������ ����. (2) (20-150 �)";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(6, 177);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(184, 13);
            this.label32.TabIndex = 24;
            this.label32.Text = "������������ ����. (1) (20-150 �)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 151);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "��� ������� (0-1 �)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "��� ������� (0-10 ���)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "����������� �������";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(180, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "���������� ��������� (20-150 �)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "����� �������� (10-100 ��)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(174, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "����� ���������� (10-10000 ��)";
            // 
            // ok_butt
            // 
            this.ok_butt.Location = new System.Drawing.Point(89, 397);
            this.ok_butt.Name = "ok_butt";
            this.ok_butt.Size = new System.Drawing.Size(72, 23);
            this.ok_butt.TabIndex = 2;
            this.ok_butt.Text = "���������";
            this.ok_butt.UseVisualStyleBackColor = true;
            this.ok_butt.Click += new System.EventHandler(this.ok_butt_Click);
            // 
            // cancel_butt
            // 
            this.cancel_butt.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel_butt.Location = new System.Drawing.Point(226, 397);
            this.cancel_butt.Name = "cancel_butt";
            this.cancel_butt.Size = new System.Drawing.Size(54, 23);
            this.cancel_butt.TabIndex = 3;
            this.cancel_butt.Text = "������";
            this.cancel_butt.UseVisualStyleBackColor = true;
            this.cancel_butt.Click += new System.EventHandler(this.cancel_butt_Click);
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(10, 397);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(73, 23);
            this.applyButton.TabIndex = 4;
            this.applyButton.Text = "���������";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // rareModeCheckBox
            // 
            this.rareModeCheckBox.AutoSize = true;
            this.rareModeCheckBox.Location = new System.Drawing.Point(10, 374);
            this.rareModeCheckBox.Name = "rareModeCheckBox";
            this.rareModeCheckBox.Size = new System.Drawing.Size(287, 17);
            this.rareModeCheckBox.TabIndex = 5;
            this.rareModeCheckBox.Text = "��� ���������� ������� ������� ������� �������";
            this.rareModeCheckBox.UseVisualStyleBackColor = true;
            // 
            // openCommonDataFileDialog
            // 
            this.openCommonDataFileDialog.DefaultExt = "cdf";
            this.openCommonDataFileDialog.Filter = "Common data files (*.cdf)|*.cdf";
            // 
            // saveCommonDataFileDialog
            // 
            this.saveCommonDataFileDialog.DefaultExt = "cdf";
            this.saveCommonDataFileDialog.Filter = "Common data files (*.cdf)|*.cdf";
            // 
            // adjustSettingsCheckBox
            // 
            this.adjustSettingsCheckBox.AutoSize = true;
            this.adjustSettingsCheckBox.Location = new System.Drawing.Point(9, 227);
            this.adjustSettingsCheckBox.Name = "adjustSettingsCheckBox";
            this.adjustSettingsCheckBox.Size = new System.Drawing.Size(211, 17);
            this.adjustSettingsCheckBox.TabIndex = 35;
            this.adjustSettingsCheckBox.Text = "������������ ��������� ���������";
            this.adjustSettingsCheckBox.UseVisualStyleBackColor = true;
            this.adjustSettingsCheckBox.CheckedChanged += new System.EventHandler(this.adjustSettingsCheckBox_CheckedChanged);
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.ok_butt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel_butt;
            this.ClientSize = new System.Drawing.Size(292, 432);
            this.Controls.Add(this.rareModeCheckBox);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.cancel_butt);
            this.Controls.Add(this.ok_butt);
            this.Controls.Add(this.params_groupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "���������";
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

        protected System.Windows.Forms.GroupBox params_groupBox;
        protected System.Windows.Forms.Label label2;
        protected System.Windows.Forms.Label label3;
        protected System.Windows.Forms.Label label4;
        protected System.Windows.Forms.Label label5;
        protected System.Windows.Forms.Label label6;
        protected System.Windows.Forms.Label label7;
        protected System.Windows.Forms.Button ok_butt;
        protected System.Windows.Forms.Button cancel_butt;
        protected System.Windows.Forms.Label label33;
        protected System.Windows.Forms.Label label32;
        protected System.Windows.Forms.NumericUpDown fV1NumericUpDown;
        protected System.Windows.Forms.NumericUpDown hCurrentNumericUpDown;
        protected System.Windows.Forms.NumericUpDown eCurrentNumericUpDown;
        protected System.Windows.Forms.NumericUpDown CPNumericUpDown;
        protected System.Windows.Forms.NumericUpDown iVoltageNumericUpDown;
        protected System.Windows.Forms.NumericUpDown idleTimeNumericUpDown;
        protected System.Windows.Forms.NumericUpDown expTimeNumericUpDown;
        protected System.Windows.Forms.NumericUpDown fV2NumericUpDown;
        protected System.Windows.Forms.Button applyButton;
        protected System.Windows.Forms.CheckBox rareModeCheckBox;
        protected System.Windows.Forms.Button saveFileButton;
        protected System.Windows.Forms.Button loadFileButton;
        protected System.Windows.Forms.OpenFileDialog openCommonDataFileDialog;
        protected System.Windows.Forms.SaveFileDialog saveCommonDataFileDialog;
        private System.Windows.Forms.CheckBox adjustSettingsCheckBox;
    }
}