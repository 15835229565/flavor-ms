namespace Flavor.Controls {
    partial class PreciseEditorLabelRow {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Label label9;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreciseEditorLabelRow));
            System.Windows.Forms.Label label11;
            System.Windows.Forms.Label commentLabel;
            label9 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            commentLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label9
            // 
            resources.ApplyResources(label9, "label9");
            label9.BackColor = System.Drawing.SystemColors.Control;
            label9.Name = "label9";
            // 
            // label11
            // 
            resources.ApplyResources(label11, "label11");
            label11.BackColor = System.Drawing.SystemColors.Control;
            label11.Name = "label11";
            // 
            // commentLabel
            // 
            resources.ApplyResources(commentLabel, "commentLabel");
            commentLabel.BackColor = System.Drawing.SystemColors.Control;
            commentLabel.Name = "commentLabel";
            // 
            // PreciseEditorLabelRow
            // 
            this.Controls.Add(label9);
            this.Controls.Add(label11);
            this.Controls.Add(commentLabel);
            this.Name = "PreciseEditorLabelRow";
            resources.ApplyResources(this, "$this");
            this.Controls.SetChildIndex(this.peakCenterLabel, 0);
            this.Controls.SetChildIndex(commentLabel, 0);
            this.Controls.SetChildIndex(label11, 0);
            this.Controls.SetChildIndex(label9, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
