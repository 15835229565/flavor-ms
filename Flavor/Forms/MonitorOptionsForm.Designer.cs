using Flavor.Controls;
namespace Flavor.Forms {
    partial class MonitorOptionsForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Label backgroundMeasureCycleCountLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitorOptionsForm));
            System.Windows.Forms.Label label2;
            Flavor.Controls.PreciseEditorLabelRowMinus controlPeakLabelRow;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label1;
            this.checkPeakInsertButton = new System.Windows.Forms.Button();
            this.monitorOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.backroundMeasureCycleCountNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.checkPeakNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.useCheckPeakCheckBox = new System.Windows.Forms.CheckBox();
            this.allowedShiftNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.timeLimitNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.checkPeakPreciseEditorRowMinus = new Flavor.Controls.PreciseEditorRowMinus();
            this.iterationsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            backgroundMeasureCycleCountLabel = new System.Windows.Forms.Label();
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
            this.monitorOptionsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backroundMeasureCycleCountNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkPeakNumberNumericUpDown)).BeginInit();
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
            // backgroundMeasureCycleCountLabel
            // 
            resources.ApplyResources(backgroundMeasureCycleCountLabel, "backgroundMeasureCycleCountLabel");
            backgroundMeasureCycleCountLabel.Name = "backgroundMeasureCycleCountLabel";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // controlPeakLabelRow
            // 
            resources.ApplyResources(controlPeakLabelRow, "controlPeakLabelRow");
            controlPeakLabelRow.Name = "controlPeakLabelRow";
            // 
            // label8
            // 
            resources.ApplyResources(label8, "label8");
            label8.Name = "label8";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // checkPeakInsertButton
            // 
            resources.ApplyResources(this.checkPeakInsertButton, "checkPeakInsertButton");
            this.checkPeakInsertButton.Name = "checkPeakInsertButton";
            this.formToolTip.SetToolTip(this.checkPeakInsertButton, resources.GetString("checkPeakInsertButton.ToolTip"));
            this.checkPeakInsertButton.UseVisualStyleBackColor = true;
            this.checkPeakInsertButton.Click += new System.EventHandler(this.checkPeakInsertButton_Click);
            // 
            // monitorOptionsGroupBox
            // 
            this.monitorOptionsGroupBox.Controls.Add(backgroundMeasureCycleCountLabel);
            this.monitorOptionsGroupBox.Controls.Add(this.backroundMeasureCycleCountNumericUpDown);
            this.monitorOptionsGroupBox.Controls.Add(this.checkPeakInsertButton);
            this.monitorOptionsGroupBox.Controls.Add(this.checkPeakNumberNumericUpDown);
            this.monitorOptionsGroupBox.Controls.Add(this.useCheckPeakCheckBox);
            this.monitorOptionsGroupBox.Controls.Add(this.allowedShiftNumericUpDown);
            this.monitorOptionsGroupBox.Controls.Add(label2);
            this.monitorOptionsGroupBox.Controls.Add(controlPeakLabelRow);
            this.monitorOptionsGroupBox.Controls.Add(this.timeLimitNumericUpDown);
            this.monitorOptionsGroupBox.Controls.Add(label8);
            this.monitorOptionsGroupBox.Controls.Add(this.checkPeakPreciseEditorRowMinus);
            this.monitorOptionsGroupBox.Controls.Add(label1);
            this.monitorOptionsGroupBox.Controls.Add(this.iterationsNumericUpDown);
            resources.ApplyResources(this.monitorOptionsGroupBox, "monitorOptionsGroupBox");
            this.monitorOptionsGroupBox.Name = "monitorOptionsGroupBox";
            this.monitorOptionsGroupBox.TabStop = false;
            // 
            // backroundMeasureCycleCountNumericUpDown
            // 
            resources.ApplyResources(this.backroundMeasureCycleCountNumericUpDown, "backroundMeasureCycleCountNumericUpDown");
            this.backroundMeasureCycleCountNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.backroundMeasureCycleCountNumericUpDown.Name = "backroundMeasureCycleCountNumericUpDown";
            // 
            // checkPeakNumberNumericUpDown
            // 
            resources.ApplyResources(this.checkPeakNumberNumericUpDown, "checkPeakNumberNumericUpDown");
            this.checkPeakNumberNumericUpDown.Name = "checkPeakNumberNumericUpDown";
            // 
            // useCheckPeakCheckBox
            // 
            resources.ApplyResources(this.useCheckPeakCheckBox, "useCheckPeakCheckBox");
            this.useCheckPeakCheckBox.Name = "useCheckPeakCheckBox";
            this.useCheckPeakCheckBox.UseVisualStyleBackColor = true;
            // 
            // allowedShiftNumericUpDown
            // 
            resources.ApplyResources(this.allowedShiftNumericUpDown, "allowedShiftNumericUpDown");
            this.allowedShiftNumericUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.allowedShiftNumericUpDown.Name = "allowedShiftNumericUpDown";
            // 
            // timeLimitNumericUpDown
            // 
            resources.ApplyResources(this.timeLimitNumericUpDown, "timeLimitNumericUpDown");
            this.timeLimitNumericUpDown.Name = "timeLimitNumericUpDown";
            // 
            // checkPeakPreciseEditorRowMinus
            // 
            resources.ApplyResources(this.checkPeakPreciseEditorRowMinus, "checkPeakPreciseEditorRowMinus");
            this.checkPeakPreciseEditorRowMinus.Name = "checkPeakPreciseEditorRowMinus";
            // 
            // iterationsNumericUpDown
            // 
            resources.ApplyResources(this.iterationsNumericUpDown, "iterationsNumericUpDown");
            this.iterationsNumericUpDown.Name = "iterationsNumericUpDown";
            // 
            // MonitorOptionsForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.monitorOptionsGroupBox);
            this.Name = "MonitorOptionsForm";
            this.Controls.SetChildIndex(this.monitorOptionsGroupBox, 0);
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
            this.monitorOptionsGroupBox.ResumeLayout(false);
            this.monitorOptionsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backroundMeasureCycleCountNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkPeakNumberNumericUpDown)).EndInit();
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
        private System.Windows.Forms.CheckBox useCheckPeakCheckBox;
        private System.Windows.Forms.NumericUpDown checkPeakNumberNumericUpDown;
        private System.Windows.Forms.NumericUpDown backroundMeasureCycleCountNumericUpDown;
        private System.Windows.Forms.GroupBox monitorOptionsGroupBox;
    }
}
