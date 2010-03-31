namespace Flavor.Forms
{
    partial class OptionsForm
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Button loadFileButton;
            System.Windows.Forms.Button saveFileButton;
            System.Windows.Forms.Label label33;
            System.Windows.Forms.Label label32;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label2;
            this.params_groupBox = new System.Windows.Forms.GroupBox();
            this.adjustSettingsCheckBox = new System.Windows.Forms.CheckBox();
            this.fV2NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.fV1NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.hCurrentNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.eCurrentNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.CPNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.iVoltageNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.idleTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.expTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.ok_butt = new System.Windows.Forms.Button();
            this.cancel_butt = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.rareModeCheckBox = new System.Windows.Forms.CheckBox();
            this.openCommonDataFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveCommonDataFileDialog = new System.Windows.Forms.SaveFileDialog();
            loadFileButton = new System.Windows.Forms.Button();
            saveFileButton = new System.Windows.Forms.Button();
            label33 = new System.Windows.Forms.Label();
            label32 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
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
            this.params_groupBox.Controls.Add(loadFileButton);
            this.params_groupBox.Controls.Add(saveFileButton);
            this.params_groupBox.Controls.Add(this.fV2NumericUpDown);
            this.params_groupBox.Controls.Add(this.fV1NumericUpDown);
            this.params_groupBox.Controls.Add(this.hCurrentNumericUpDown);
            this.params_groupBox.Controls.Add(this.eCurrentNumericUpDown);
            this.params_groupBox.Controls.Add(this.CPNumericUpDown);
            this.params_groupBox.Controls.Add(this.iVoltageNumericUpDown);
            this.params_groupBox.Controls.Add(this.idleTimeNumericUpDown);
            this.params_groupBox.Controls.Add(this.expTimeNumericUpDown);
            this.params_groupBox.Controls.Add(label33);
            this.params_groupBox.Controls.Add(label32);
            this.params_groupBox.Controls.Add(label7);
            this.params_groupBox.Controls.Add(label6);
            this.params_groupBox.Controls.Add(label5);
            this.params_groupBox.Controls.Add(label4);
            this.params_groupBox.Controls.Add(label3);
            this.params_groupBox.Controls.Add(label2);
            this.params_groupBox.Location = new System.Drawing.Point(10, 85);
            this.params_groupBox.Name = "params_groupBox";
            this.params_groupBox.Size = new System.Drawing.Size(270, 283);
            this.params_groupBox.TabIndex = 1;
            this.params_groupBox.TabStop = false;
            this.params_groupBox.Text = "Общие параметры";
            // 
            // adjustSettingsCheckBox
            // 
            this.adjustSettingsCheckBox.AutoSize = true;
            this.adjustSettingsCheckBox.Location = new System.Drawing.Point(9, 227);
            this.adjustSettingsCheckBox.Name = "adjustSettingsCheckBox";
            this.adjustSettingsCheckBox.Size = new System.Drawing.Size(211, 17);
            this.adjustSettingsCheckBox.TabIndex = 35;
            this.adjustSettingsCheckBox.Text = "Активировать настройки юстировки";
            this.adjustSettingsCheckBox.UseVisualStyleBackColor = true;
            this.adjustSettingsCheckBox.CheckedChanged += new System.EventHandler(this.adjustSettingsCheckBox_CheckedChanged);
            // 
            // loadFileButton
            // 
            loadFileButton.Location = new System.Drawing.Point(135, 250);
            loadFileButton.Name = "loadFileButton";
            loadFileButton.Size = new System.Drawing.Size(120, 23);
            loadFileButton.TabIndex = 34;
            loadFileButton.Text = "Загрузить из файла";
            loadFileButton.UseVisualStyleBackColor = true;
            loadFileButton.Click += new System.EventHandler(this.loadFileButton_Click);
            // 
            // saveFileButton
            // 
            saveFileButton.Location = new System.Drawing.Point(9, 250);
            saveFileButton.Name = "saveFileButton";
            saveFileButton.Size = new System.Drawing.Size(112, 23);
            saveFileButton.TabIndex = 6;
            saveFileButton.Text = "Сохранить в файл";
            saveFileButton.UseVisualStyleBackColor = true;
            saveFileButton.Click += new System.EventHandler(this.saveFileButton_Click);
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
            this.fV2NumericUpDown.ReadOnly = true;
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
            this.fV1NumericUpDown.ReadOnly = true;
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
            this.hCurrentNumericUpDown.ReadOnly = true;
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
            this.CPNumericUpDown.ReadOnly = true;
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
            label33.AutoSize = true;
            label33.Location = new System.Drawing.Point(6, 203);
            label33.Name = "label33";
            label33.Size = new System.Drawing.Size(184, 13);
            label33.TabIndex = 25;
            label33.Text = "Фокусирующее напр. (2) (20-150 В)";
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Location = new System.Drawing.Point(6, 177);
            label32.Name = "label32";
            label32.Size = new System.Drawing.Size(184, 13);
            label32.TabIndex = 24;
            label32.Text = "Фокусирующее напр. (1) (20-150 В)";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(6, 151);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(104, 13);
            label7.TabIndex = 11;
            label7.Text = "Ток нагрева (0-1 А)";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(6, 125);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(127, 13);
            label6.TabIndex = 10;
            label6.Text = "Ток эмиссии (0-10 мкА)";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(6, 99);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(122, 13);
            label5.TabIndex = 9;
            label5.Text = "Коэффициент деления";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(6, 73);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(180, 13);
            label4.TabIndex = 8;
            label4.Text = "Напряжение ионизации (20-150 В)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(6, 47);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(149, 13);
            label3.TabIndex = 7;
            label3.Text = "Время перехода (10-100 мс)";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(6, 21);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(174, 13);
            label2.TabIndex = 6;
            label2.Text = "Время экспозиции (10-10000 мс)";
            // 
            // ok_butt
            // 
            this.ok_butt.Location = new System.Drawing.Point(89, 397);
            this.ok_butt.Name = "ok_butt";
            this.ok_butt.Size = new System.Drawing.Size(72, 23);
            this.ok_butt.TabIndex = 2;
            this.ok_butt.Text = "Сохранить";
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
            this.cancel_butt.Text = "Отмена";
            this.cancel_butt.UseVisualStyleBackColor = true;
            this.cancel_butt.Click += new System.EventHandler(this.cancel_butt_Click);
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(10, 397);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(73, 23);
            this.applyButton.TabIndex = 4;
            this.applyButton.Text = "Применить";
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
            this.rareModeCheckBox.Text = "Без уменьшения частоты запроса статуса прибора";
            this.rareModeCheckBox.UseVisualStyleBackColor = true;
            // 
            // openCommonDataFileDialog
            // 
            this.openCommonDataFileDialog.DefaultExt = "cdf";
            this.openCommonDataFileDialog.Filter = "Common data files (*.cdf)|*.cdf|Specter data files (*.sdf)|*.sdf|Precise specter " +
                "files (*.psf)|*.psf";
            // 
            // saveCommonDataFileDialog
            // 
            this.saveCommonDataFileDialog.DefaultExt = "cdf";
            this.saveCommonDataFileDialog.Filter = "Common data files (*.cdf)|*.cdf";
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
            this.Text = "Настройки";
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

        protected System.Windows.Forms.NumericUpDown fV1NumericUpDown;
        protected System.Windows.Forms.NumericUpDown hCurrentNumericUpDown;
        protected System.Windows.Forms.NumericUpDown eCurrentNumericUpDown;
        protected System.Windows.Forms.NumericUpDown CPNumericUpDown;
        protected System.Windows.Forms.NumericUpDown iVoltageNumericUpDown;
        protected System.Windows.Forms.NumericUpDown idleTimeNumericUpDown;
        protected System.Windows.Forms.NumericUpDown expTimeNumericUpDown;
        protected System.Windows.Forms.NumericUpDown fV2NumericUpDown;
        protected System.Windows.Forms.OpenFileDialog openCommonDataFileDialog;
        protected System.Windows.Forms.SaveFileDialog saveCommonDataFileDialog;
        protected System.Windows.Forms.Button cancel_butt;
        protected System.Windows.Forms.Button applyButton;
        protected System.Windows.Forms.Button ok_butt;
        protected System.Windows.Forms.GroupBox params_groupBox;
        private System.Windows.Forms.CheckBox adjustSettingsCheckBox;
        protected System.Windows.Forms.CheckBox rareModeCheckBox;
    }
}