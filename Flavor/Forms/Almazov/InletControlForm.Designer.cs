namespace Flavor.Forms.Almazov {
    partial class InletControlForm {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent() {
            this.capillaryRadioButton = new System.Windows.Forms.RadioButton();
            this.inletRadioButton = new System.Windows.Forms.RadioButton();
            this.inletGroupBox = new System.Windows.Forms.GroupBox();
            this.temperatureNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.voltageNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.cancelButton = new System.Windows.Forms.Button();
            this.sendButton = new System.Windows.Forms.Button();
            this.closeInletRadioButton = new System.Windows.Forms.RadioButton();
            this.temperatureCheckBox = new System.Windows.Forms.CheckBox();
            this.inletGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.temperatureNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.voltageNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // capillaryRadioButton
            // 
            this.capillaryRadioButton.AutoSize = true;
            this.capillaryRadioButton.Location = new System.Drawing.Point(12, 35);
            this.capillaryRadioButton.Name = "capillaryRadioButton";
            this.capillaryRadioButton.Size = new System.Drawing.Size(74, 17);
            this.capillaryRadioButton.TabIndex = 0;
            this.capillaryRadioButton.Text = "Капилляр";
            this.capillaryRadioButton.UseVisualStyleBackColor = true;
            // 
            // inletRadioButton
            // 
            this.inletRadioButton.AutoSize = true;
            this.inletRadioButton.Location = new System.Drawing.Point(12, 58);
            this.inletRadioButton.Name = "inletRadioButton";
            this.inletRadioButton.Size = new System.Drawing.Size(217, 17);
            this.inletRadioButton.TabIndex = 1;
            this.inletRadioButton.Text = "Напряжение натекателя (2500-3000В)";
            this.inletRadioButton.UseVisualStyleBackColor = true;
            this.inletRadioButton.CheckedChanged += new System.EventHandler(this.inletRadioButton_CheckedChanged);
            // 
            // inletGroupBox
            // 
            this.inletGroupBox.Controls.Add(this.temperatureCheckBox);
            this.inletGroupBox.Controls.Add(this.temperatureNumericUpDown);
            this.inletGroupBox.Location = new System.Drawing.Point(12, 81);
            this.inletGroupBox.Name = "inletGroupBox";
            this.inletGroupBox.Size = new System.Drawing.Size(268, 44);
            this.inletGroupBox.TabIndex = 2;
            this.inletGroupBox.TabStop = false;
            this.inletGroupBox.Text = "Нагрев натекателя";
            // 
            // temperatureNumericUpDown
            // 
            this.temperatureNumericUpDown.DecimalPlaces = 1;
            this.temperatureNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.temperatureNumericUpDown.Location = new System.Drawing.Point(148, 18);
            this.temperatureNumericUpDown.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.temperatureNumericUpDown.Minimum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.temperatureNumericUpDown.Name = "temperatureNumericUpDown";
            this.temperatureNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.temperatureNumericUpDown.TabIndex = 3;
            this.temperatureNumericUpDown.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // voltageNumericUpDown
            // 
            this.voltageNumericUpDown.Location = new System.Drawing.Point(235, 58);
            this.voltageNumericUpDown.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.voltageNumericUpDown.Minimum = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            this.voltageNumericUpDown.Name = "voltageNumericUpDown";
            this.voltageNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.voltageNumericUpDown.TabIndex = 2;
            this.voltageNumericUpDown.Value = new decimal(new int[] {
            2700,
            0,
            0,
            0});
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(205, 131);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(12, 131);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.TabIndex = 4;
            this.sendButton.Text = "Послать";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // closeInletRadioButton
            // 
            this.closeInletRadioButton.AutoSize = true;
            this.closeInletRadioButton.Checked = true;
            this.closeInletRadioButton.Location = new System.Drawing.Point(12, 12);
            this.closeInletRadioButton.Name = "closeInletRadioButton";
            this.closeInletRadioButton.Size = new System.Drawing.Size(69, 17);
            this.closeInletRadioButton.TabIndex = 5;
            this.closeInletRadioButton.TabStop = true;
            this.closeInletRadioButton.Text = "Закрыть";
            this.closeInletRadioButton.UseVisualStyleBackColor = true;
            // 
            // temperatureCheckBox
            // 
            this.temperatureCheckBox.AutoSize = true;
            this.temperatureCheckBox.Location = new System.Drawing.Point(6, 19);
            this.temperatureCheckBox.Name = "temperatureCheckBox";
            this.temperatureCheckBox.Size = new System.Drawing.Size(136, 17);
            this.temperatureCheckBox.TabIndex = 4;
            this.temperatureCheckBox.Text = "Температура (40-50C)";
            this.temperatureCheckBox.UseVisualStyleBackColor = true;
            this.temperatureCheckBox.CheckedChanged += new System.EventHandler(this.temperatureCheckBox_CheckedChanged);
            // 
            // InletControlForm
            // 
            this.AcceptButton = this.sendButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(292, 162);
            this.Controls.Add(this.closeInletRadioButton);
            this.Controls.Add(this.voltageNumericUpDown);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.inletGroupBox);
            this.Controls.Add(this.inletRadioButton);
            this.Controls.Add(this.capillaryRadioButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "InletControlForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Система напуска пробы";
            this.inletGroupBox.ResumeLayout(false);
            this.inletGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.temperatureNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.voltageNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton capillaryRadioButton;
        private System.Windows.Forms.RadioButton inletRadioButton;
        private System.Windows.Forms.GroupBox inletGroupBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.NumericUpDown temperatureNumericUpDown;
        private System.Windows.Forms.NumericUpDown voltageNumericUpDown;
        private System.Windows.Forms.RadioButton closeInletRadioButton;
        private System.Windows.Forms.CheckBox temperatureCheckBox;
    }
}