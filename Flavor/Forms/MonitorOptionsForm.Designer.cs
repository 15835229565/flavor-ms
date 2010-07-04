using Flavor.Controls;
namespace Flavor.Forms {
    partial class MonitorOptionsForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.GroupBox groupBox2;
            System.Windows.Forms.Label label2;
            Flavor.Controls.PreciseEditorLabelRowMinus controlPeakLabelRow;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label1;
            this.allowedShiftNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.timeLimitNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.checkPeakInsertButton = new System.Windows.Forms.Button();
            this.checkPeakPreciseEditorRowMinus = new Flavor.Controls.PreciseEditorRowMinus();
            this.iterationsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            groupBox2 = new System.Windows.Forms.GroupBox();
            label2 = new System.Windows.Forms.Label();
            controlPeakLabelRow = new Flavor.Controls.PreciseEditorLabelRowMinus();
            label8 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.fV1NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hCurrentNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eCurrentNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CPNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iVoltageNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.idleTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fV2NumericUpDown)).BeginInit();
            this.params_groupBox.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.allowedShiftNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeLimitNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iterationsNumericUpDown)).BeginInit();
            this.SuspendLayout();
            this.params_groupBox.Controls.SetChildIndex(this.expTimeNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.idleTimeNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.iVoltageNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.CPNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.eCurrentNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.hCurrentNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.fV1NumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.fV2NumericUpDown, 0);
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(this.allowedShiftNumericUpDown);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(controlPeakLabelRow);
            groupBox2.Controls.Add(this.timeLimitNumericUpDown);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(this.checkPeakInsertButton);
            groupBox2.Controls.Add(this.checkPeakPreciseEditorRowMinus);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(this.iterationsNumericUpDown);
            groupBox2.Location = new System.Drawing.Point(12, 403);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(690, 70);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "Параметры режима мониторинга";
            // 
            // allowedShiftNumericUpDown
            // 
            this.allowedShiftNumericUpDown.Location = new System.Drawing.Point(624, 14);
            this.allowedShiftNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.allowedShiftNumericUpDown.Name = "allowedShiftNumericUpDown";
            this.allowedShiftNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.allowedShiftNumericUpDown.TabIndex = 8;
            this.allowedShiftNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(408, 16);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(180, 13);
            label2.TabIndex = 7;
            label2.Text = "Разрешенная отстройка (ступени)";
            // 
            // controlPeakLabelRow
            // 
            controlPeakLabelRow.Location = new System.Drawing.Point(194, 14);
            controlPeakLabelRow.Name = "controlPeakLabelRow";
            controlPeakLabelRow.Size = new System.Drawing.Size(124, 26);
            controlPeakLabelRow.TabIndex = 6;
            // 
            // timeLimitNumericUpDown
            // 
            this.timeLimitNumericUpDown.Location = new System.Drawing.Point(128, 40);
            this.timeLimitNumericUpDown.Name = "timeLimitNumericUpDown";
            this.timeLimitNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.timeLimitNumericUpDown.TabIndex = 5;
            this.timeLimitNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(6, 42);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(106, 13);
            label8.TabIndex = 4;
            label8.Text = "Общее время (мин)";
            // 
            // checkPeakInsertButton
            // 
            this.checkPeakInsertButton.Location = new System.Drawing.Point(324, 11);
            this.checkPeakInsertButton.Name = "checkPeakInsertButton";
            this.checkPeakInsertButton.Size = new System.Drawing.Size(78, 49);
            this.checkPeakInsertButton.TabIndex = 3;
            this.checkPeakInsertButton.Text = "Вставка контрольного пика";
            this.checkPeakInsertButton.UseVisualStyleBackColor = true;
            this.checkPeakInsertButton.Click += new System.EventHandler(this.checkPeakInsertButton_Click);
            // 
            // checkPeakPreciseEditorRowMinus
            // 
            this.checkPeakPreciseEditorRowMinus.Location = new System.Drawing.Point(194, 42);
            this.checkPeakPreciseEditorRowMinus.Name = "checkPeakPreciseEditorRowMinus";
            this.checkPeakPreciseEditorRowMinus.Size = new System.Drawing.Size(124, 13);
            this.checkPeakPreciseEditorRowMinus.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(116, 13);
            label1.TabIndex = 1;
            label1.Text = "Количество итераций";
            // 
            // iterationsNumericUpDown
            // 
            this.iterationsNumericUpDown.Location = new System.Drawing.Point(128, 14);
            this.iterationsNumericUpDown.Name = "iterationsNumericUpDown";
            this.iterationsNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.iterationsNumericUpDown.TabIndex = 0;
            this.iterationsNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // MonitorOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(711, 482);
            this.Controls.Add(groupBox2);
            this.Name = "MonitorOptionsForm";
            this.Text = "Настройки  режима мониторинга";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MonitorOptionsForm_FormClosed);
            this.Controls.SetChildIndex(groupBox2, 0);
            this.Controls.SetChildIndex(this.params_groupBox, 0);
            this.Controls.SetChildIndex(this.ok_butt, 0);
            this.Controls.SetChildIndex(this.cancel_butt, 0);
            this.Controls.SetChildIndex(this.applyButton, 0);
            this.Controls.SetChildIndex(this.rareModeCheckBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.fV1NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hCurrentNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eCurrentNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CPNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iVoltageNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.idleTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.expTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fV2NumericUpDown)).EndInit();
            this.params_groupBox.ResumeLayout(false);
            this.params_groupBox.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.allowedShiftNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeLimitNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iterationsNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown iterationsNumericUpDown;
        private Flavor.Controls.PreciseEditorRowMinus checkPeakPreciseEditorRowMinus;
        private System.Windows.Forms.Button checkPeakInsertButton;
        private System.Windows.Forms.NumericUpDown timeLimitNumericUpDown;
        private System.Windows.Forms.NumericUpDown allowedShiftNumericUpDown;
    }
}
