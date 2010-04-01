namespace Flavor.Controls {
    partial class PreciseEditorLabelRow {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Label label9;
            System.Windows.Forms.Label label11;
            System.Windows.Forms.Label commentLabel;
            label9 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            commentLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = System.Drawing.SystemColors.Control;
            label9.Location = new System.Drawing.Point(126, 13);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(52, 13);
            label9.TabIndex = 3;
            label9.Text = "Проходы";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.BackColor = System.Drawing.SystemColors.Control;
            label11.Location = new System.Drawing.Point(177, 13);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(54, 13);
            label11.TabIndex = 4;
            label11.Text = "Точность";
            // 
            // commentLabel
            // 
            commentLabel.AutoSize = true;
            commentLabel.BackColor = System.Drawing.SystemColors.Control;
            commentLabel.Location = new System.Drawing.Point(233, 13);
            commentLabel.Name = "commentLabel";
            commentLabel.Size = new System.Drawing.Size(77, 13);
            commentLabel.TabIndex = 5;
            commentLabel.Text = "Комментарий";
            // 
            // PreciseEditorLabelRow
            // 
            this.Controls.Add(label9);
            this.Controls.Add(label11);
            this.Controls.Add(commentLabel);
            this.Name = "PreciseEditorLabelRow";
            this.Size = new System.Drawing.Size(310, 26);
            this.Controls.SetChildIndex(commentLabel, 0);
            this.Controls.SetChildIndex(label11, 0);
            this.Controls.SetChildIndex(label9, 0);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
