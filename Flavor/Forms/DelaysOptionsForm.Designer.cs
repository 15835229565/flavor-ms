namespace Flavor
{
    partial class DelaysOptionsForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.beforeTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.forwardAsBeforeCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.forwardTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.backwardTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.ok_butt = new System.Windows.Forms.Button();
            this.cancel_butt = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.beforeTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.forwardTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backwardTimeNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Перед циклом измерений (мс)";
            // 
            // beforeTimeNumericUpDown
            // 
            this.beforeTimeNumericUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.beforeTimeNumericUpDown.Location = new System.Drawing.Point(220, 7);
            this.beforeTimeNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.beforeTimeNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.beforeTimeNumericUpDown.Name = "beforeTimeNumericUpDown";
            this.beforeTimeNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.beforeTimeNumericUpDown.TabIndex = 27;
            this.beforeTimeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.beforeTimeNumericUpDown.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // forwardAsBeforeCheckBox
            // 
            this.forwardAsBeforeCheckBox.AutoSize = true;
            this.forwardAsBeforeCheckBox.Location = new System.Drawing.Point(15, 59);
            this.forwardAsBeforeCheckBox.Name = "forwardAsBeforeCheckBox";
            this.forwardAsBeforeCheckBox.Size = new System.Drawing.Size(224, 17);
            this.forwardAsBeforeCheckBox.TabIndex = 28;
            this.forwardAsBeforeCheckBox.Text = "Одинаково с задержкой перед циклом";
            this.forwardAsBeforeCheckBox.UseVisualStyleBackColor = true;
            this.forwardAsBeforeCheckBox.CheckedChanged += new System.EventHandler(this.forwardAsBeforeCheckBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "При скачке напряжения вперед (мс)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(186, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "При скачке напряжения назад (мс)";
            // 
            // forwardTimeNumericUpDown
            // 
            this.forwardTimeNumericUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.forwardTimeNumericUpDown.Location = new System.Drawing.Point(220, 33);
            this.forwardTimeNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.forwardTimeNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.forwardTimeNumericUpDown.Name = "forwardTimeNumericUpDown";
            this.forwardTimeNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.forwardTimeNumericUpDown.TabIndex = 31;
            this.forwardTimeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.forwardTimeNumericUpDown.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // backwardTimeNumericUpDown
            // 
            this.backwardTimeNumericUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.backwardTimeNumericUpDown.Location = new System.Drawing.Point(220, 82);
            this.backwardTimeNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.backwardTimeNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.backwardTimeNumericUpDown.Name = "backwardTimeNumericUpDown";
            this.backwardTimeNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.backwardTimeNumericUpDown.TabIndex = 32;
            this.backwardTimeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.backwardTimeNumericUpDown.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // ok_butt
            // 
            this.ok_butt.Location = new System.Drawing.Point(12, 108);
            this.ok_butt.Name = "ok_butt";
            this.ok_butt.Size = new System.Drawing.Size(72, 23);
            this.ok_butt.TabIndex = 33;
            this.ok_butt.Text = "Сохранить";
            this.ok_butt.UseVisualStyleBackColor = true;
            this.ok_butt.Click += new System.EventHandler(this.ok_butt_Click);
            // 
            // cancel_butt
            // 
            this.cancel_butt.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel_butt.Location = new System.Drawing.Point(226, 108);
            this.cancel_butt.Name = "cancel_butt";
            this.cancel_butt.Size = new System.Drawing.Size(54, 23);
            this.cancel_butt.TabIndex = 34;
            this.cancel_butt.Text = "Отмена";
            this.cancel_butt.UseVisualStyleBackColor = true;
            this.cancel_butt.Click += new System.EventHandler(this.cancel_butt_Click);
            // 
            // DelaysOptionsForm
            // 
            this.AcceptButton = this.ok_butt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel_butt;
            this.ClientSize = new System.Drawing.Size(292, 143);
            this.Controls.Add(this.cancel_butt);
            this.Controls.Add(this.ok_butt);
            this.Controls.Add(this.backwardTimeNumericUpDown);
            this.Controls.Add(this.forwardTimeNumericUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.forwardAsBeforeCheckBox);
            this.Controls.Add(this.beforeTimeNumericUpDown);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DelaysOptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Установка задержек";
            ((System.ComponentModel.ISupportInitialize)(this.beforeTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.forwardTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backwardTimeNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Label label2;
        protected System.Windows.Forms.NumericUpDown beforeTimeNumericUpDown;
        protected System.Windows.Forms.CheckBox forwardAsBeforeCheckBox;
        protected System.Windows.Forms.Label label1;
        protected System.Windows.Forms.Label label3;
        protected System.Windows.Forms.NumericUpDown forwardTimeNumericUpDown;
        protected System.Windows.Forms.NumericUpDown backwardTimeNumericUpDown;
        protected System.Windows.Forms.Button ok_butt;
        protected System.Windows.Forms.Button cancel_butt;
    }
}