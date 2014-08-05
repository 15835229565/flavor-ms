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
            this.voltageТumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.sendButton = new System.Windows.Forms.Button();
            this.closeInletRadioButton = new System.Windows.Forms.RadioButton();
            this.inletGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.temperatureNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.voltageТumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // capillaryRadioButton
            // 
            this.capillaryRadioButton.AutoSize = true;
            this.capillaryRadioButton.Checked = true;
            this.capillaryRadioButton.Location = new System.Drawing.Point(12, 12);
            this.capillaryRadioButton.Name = "capillaryRadioButton";
            this.capillaryRadioButton.Size = new System.Drawing.Size(74, 17);
            this.capillaryRadioButton.TabIndex = 0;
            this.capillaryRadioButton.TabStop = true;
            this.capillaryRadioButton.Text = "Капилляр";
            this.capillaryRadioButton.UseVisualStyleBackColor = true;
            // 
            // inletRadioButton
            // 
            this.inletRadioButton.AutoSize = true;
            this.inletRadioButton.Location = new System.Drawing.Point(12, 35);
            this.inletRadioButton.Name = "inletRadioButton";
            this.inletRadioButton.Size = new System.Drawing.Size(85, 17);
            this.inletRadioButton.TabIndex = 1;
            this.inletRadioButton.Text = "Натекатель";
            this.inletRadioButton.UseVisualStyleBackColor = true;
            this.inletRadioButton.CheckedChanged += new System.EventHandler(this.inletRadioButton_CheckedChanged);
            // 
            // inletGroupBox
            // 
            this.inletGroupBox.Controls.Add(this.temperatureNumericUpDown);
            this.inletGroupBox.Controls.Add(this.voltageТumericUpDown);
            this.inletGroupBox.Controls.Add(this.label2);
            this.inletGroupBox.Controls.Add(this.label1);
            this.inletGroupBox.Location = new System.Drawing.Point(12, 58);
            this.inletGroupBox.Name = "inletGroupBox";
            this.inletGroupBox.Size = new System.Drawing.Size(268, 73);
            this.inletGroupBox.TabIndex = 2;
            this.inletGroupBox.TabStop = false;
            this.inletGroupBox.Text = "Параметры натекателя";
            // 
            // temperatureNumericUpDown
            // 
            this.temperatureNumericUpDown.DecimalPlaces = 1;
            this.temperatureNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.temperatureNumericUpDown.Location = new System.Drawing.Point(142, 45);
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
            this.temperatureNumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.temperatureNumericUpDown.TabIndex = 3;
            this.temperatureNumericUpDown.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            // 
            // voltageТumericUpDown
            // 
            this.voltageТumericUpDown.Location = new System.Drawing.Point(142, 19);
            this.voltageТumericUpDown.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.voltageТumericUpDown.Minimum = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            this.voltageТumericUpDown.Name = "voltageТumericUpDown";
            this.voltageТumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.voltageТumericUpDown.TabIndex = 2;
            this.voltageТumericUpDown.Value = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Температура (40-50C)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Напряжение (2500-3000В)";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(205, 231);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(12, 231);
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
            this.closeInletRadioButton.Location = new System.Drawing.Point(12, 137);
            this.closeInletRadioButton.Name = "closeInletRadioButton";
            this.closeInletRadioButton.Size = new System.Drawing.Size(69, 17);
            this.closeInletRadioButton.TabIndex = 5;
            this.closeInletRadioButton.TabStop = true;
            this.closeInletRadioButton.Text = "Закрыть";
            this.closeInletRadioButton.UseVisualStyleBackColor = true;
            // 
            // InletControlForm
            // 
            this.AcceptButton = this.sendButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.closeInletRadioButton);
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
            this.Text = "Система ввода";
            this.inletGroupBox.ResumeLayout(false);
            this.inletGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.temperatureNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.voltageТumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton capillaryRadioButton;
        private System.Windows.Forms.RadioButton inletRadioButton;
        private System.Windows.Forms.GroupBox inletGroupBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown temperatureNumericUpDown;
        private System.Windows.Forms.NumericUpDown voltageТumericUpDown;
        private System.Windows.Forms.RadioButton closeInletRadioButton;
    }
}