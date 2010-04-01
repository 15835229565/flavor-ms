using Flavor.Common;
namespace Flavor.Forms
{
    partial class PreciseOptionsForm
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.preciseEditorGroupBox = new System.Windows.Forms.GroupBox();
            this.insertPointButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.loadPreciseEditorFromFileButton = new System.Windows.Forms.Button();
            this.savePreciseEditorToFileButton = new System.Windows.Forms.Button();
            this.savePreciseEditorToFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.loadPreciseEditorFromFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.fV1NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hCurrentNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eCurrentNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CPNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iVoltageNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.idleTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fV2NumericUpDown)).BeginInit();
            this.params_groupBox.SuspendLayout();
            this.preciseEditorGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancel_butt
            // 
            this.cancel_butt.Location = new System.Drawing.Point(642, 374);
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(432, 374);
            // 
            // ok_butt
            // 
            this.ok_butt.Location = new System.Drawing.Point(510, 374);
            // 
            // params_groupBox
            // 
            this.params_groupBox.Location = new System.Drawing.Point(432, 12);
            this.params_groupBox.Controls.SetChildIndex(this.expTimeNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.idleTimeNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.iVoltageNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.CPNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.eCurrentNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.hCurrentNumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.fV1NumericUpDown, 0);
            this.params_groupBox.Controls.SetChildIndex(this.fV2NumericUpDown, 0);
            // 
            // rareModeCheckBox
            // 
            this.rareModeCheckBox.Location = new System.Drawing.Point(432, 301);
            // 
            // groupBox1
            // 
            this.preciseEditorGroupBox.BackColor = System.Drawing.SystemColors.Control;
            this.preciseEditorGroupBox.Controls.Add(this.insertPointButton);
            this.preciseEditorGroupBox.Controls.Add(this.clearButton);
            this.preciseEditorGroupBox.Controls.Add(this.loadPreciseEditorFromFileButton);
            this.preciseEditorGroupBox.Controls.Add(this.savePreciseEditorToFileButton);
            this.preciseEditorGroupBox.Location = new System.Drawing.Point(12, 12);
            this.preciseEditorGroupBox.Name = "groupBox1";
            this.preciseEditorGroupBox.Size = new System.Drawing.Size(414, 385);
            this.preciseEditorGroupBox.TabIndex = 3;
            this.preciseEditorGroupBox.TabStop = false;
            this.preciseEditorGroupBox.Text = "Редактор областей сканирования";
            // 
            // insertPointButton
            // 
            this.insertPointButton.Location = new System.Drawing.Point(312, 356);
            this.insertPointButton.Name = "insertPointButton";
            this.insertPointButton.Size = new System.Drawing.Size(90, 23);
            this.insertPointButton.TabIndex = 0;
            this.insertPointButton.Text = "Вставка точки";
            this.insertPointButton.UseVisualStyleBackColor = true;
            this.insertPointButton.Click += new System.EventHandler(this.insertPointButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(244, 356);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(62, 23);
            this.clearButton.TabIndex = 12;
            this.clearButton.Text = "Очистить";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // loadPreciseEditorFromFileButton
            // 
            this.loadPreciseEditorFromFileButton.Location = new System.Drawing.Point(118, 356);
            this.loadPreciseEditorFromFileButton.Name = "loadPreciseEditorFromFileButton";
            this.loadPreciseEditorFromFileButton.Size = new System.Drawing.Size(120, 23);
            this.loadPreciseEditorFromFileButton.TabIndex = 11;
            this.loadPreciseEditorFromFileButton.Text = "Загрузить из файла";
            this.loadPreciseEditorFromFileButton.UseVisualStyleBackColor = true;
            this.loadPreciseEditorFromFileButton.Click += new System.EventHandler(this.loadPreciseEditorFromFileButton_Click);
            // 
            // savePreciseEditorToFileButton
            // 
            this.savePreciseEditorToFileButton.Location = new System.Drawing.Point(6, 356);
            this.savePreciseEditorToFileButton.Name = "savePreciseEditorToFileButton";
            this.savePreciseEditorToFileButton.Size = new System.Drawing.Size(106, 23);
            this.savePreciseEditorToFileButton.TabIndex = 10;
            this.savePreciseEditorToFileButton.Text = "Сохранить в файл";
            this.savePreciseEditorToFileButton.UseVisualStyleBackColor = true;
            this.savePreciseEditorToFileButton.Click += new System.EventHandler(this.savePreciseEditorToFileButton_Click);
            // 
            // savePreciseEditorToFileDialog
            // 
            this.savePreciseEditorToFileDialog.DefaultExt = "ped";
            this.savePreciseEditorToFileDialog.Filter = "Precise Editor Data Files (*.ped)|*.ped";
            // 
            // loadPreciseEditorFromFileDialog
            // 
            this.loadPreciseEditorFromFileDialog.DefaultExt = "ped";
            this.loadPreciseEditorFromFileDialog.Filter = "Precise editor data files (*.ped)|*.ped|Precise specter files (*.psf)|*.psf";
            // 
            // PreciseOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(711, 409);
            this.Controls.Add(this.preciseEditorGroupBox);
            this.MinimizeBox = true;
            this.Name = "PreciseOptionsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки точного режима";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PreciseOptionsForm_FormClosed);
            this.Controls.SetChildIndex(this.preciseEditorGroupBox, 0);
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
            this.preciseEditorGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog savePreciseEditorToFileDialog;
        private System.Windows.Forms.Button loadPreciseEditorFromFileButton;
        private System.Windows.Forms.OpenFileDialog loadPreciseEditorFromFileDialog;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button insertPointButton;
        private System.Windows.Forms.Button savePreciseEditorToFileButton;
        private System.Windows.Forms.GroupBox preciseEditorGroupBox;
    }
}