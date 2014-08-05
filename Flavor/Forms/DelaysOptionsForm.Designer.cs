﻿namespace Flavor.Forms {
    partial class DelaysOptionsForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Label label2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DelaysOptionsForm));
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Button ok_butt;
            System.Windows.Forms.Button cancel_butt;
            this.beforeTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.forwardAsBeforeCheckBox = new System.Windows.Forms.CheckBox();
            this.forwardTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.backwardTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            ok_butt = new System.Windows.Forms.Button();
            cancel_butt = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.beforeTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.forwardTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backwardTimeNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // ok_butt
            // 
            ok_butt.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(ok_butt, "ok_butt");
            ok_butt.Name = "ok_butt";
            ok_butt.UseVisualStyleBackColor = true;
            ok_butt.Click += new System.EventHandler(this.ok_butt_Click);
            // 
            // cancel_butt
            // 
            cancel_butt.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(cancel_butt, "cancel_butt");
            cancel_butt.Name = "cancel_butt";
            cancel_butt.UseVisualStyleBackColor = true;
            // 
            // beforeTimeNumericUpDown
            // 
            resources.ApplyResources(this.beforeTimeNumericUpDown, "beforeTimeNumericUpDown");
            this.beforeTimeNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.beforeTimeNumericUpDown.Name = "beforeTimeNumericUpDown";
            this.beforeTimeNumericUpDown.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // forwardAsBeforeCheckBox
            // 
            resources.ApplyResources(this.forwardAsBeforeCheckBox, "forwardAsBeforeCheckBox");
            this.forwardAsBeforeCheckBox.Name = "forwardAsBeforeCheckBox";
            this.forwardAsBeforeCheckBox.UseVisualStyleBackColor = true;
            this.forwardAsBeforeCheckBox.CheckedChanged += new System.EventHandler(this.forwardAsBeforeCheckBox_CheckedChanged);
            // 
            // forwardTimeNumericUpDown
            // 
            resources.ApplyResources(this.forwardTimeNumericUpDown, "forwardTimeNumericUpDown");
            this.forwardTimeNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.forwardTimeNumericUpDown.Name = "forwardTimeNumericUpDown";
            this.forwardTimeNumericUpDown.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // backwardTimeNumericUpDown
            // 
            resources.ApplyResources(this.backwardTimeNumericUpDown, "backwardTimeNumericUpDown");
            this.backwardTimeNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.backwardTimeNumericUpDown.Name = "backwardTimeNumericUpDown";
            this.backwardTimeNumericUpDown.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // DelaysOptionsForm
            // 
            this.AcceptButton = ok_butt;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = cancel_butt;
            this.Controls.Add(cancel_butt);
            this.Controls.Add(ok_butt);
            this.Controls.Add(this.backwardTimeNumericUpDown);
            this.Controls.Add(this.forwardTimeNumericUpDown);
            this.Controls.Add(label3);
            this.Controls.Add(label1);
            this.Controls.Add(this.forwardAsBeforeCheckBox);
            this.Controls.Add(this.beforeTimeNumericUpDown);
            this.Controls.Add(label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DelaysOptionsForm";
            ((System.ComponentModel.ISupportInitialize)(this.beforeTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.forwardTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backwardTimeNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.NumericUpDown beforeTimeNumericUpDown;
        protected System.Windows.Forms.NumericUpDown forwardTimeNumericUpDown;
        protected System.Windows.Forms.NumericUpDown backwardTimeNumericUpDown;
        protected System.Windows.Forms.CheckBox forwardAsBeforeCheckBox;
    }
}