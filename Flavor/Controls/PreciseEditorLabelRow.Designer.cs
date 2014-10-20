using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Flavor.Controls {
    partial class PreciseEditorLabelRow {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(PreciseEditorLabelRow));
            Label label9;
            Label label11;
            Label commentLabel;
            label9 = new Label();
            label11 = new Label();
            commentLabel = new Label();
            this.SuspendLayout();
            // 
            // peakCenterLabel
            // 
            this.peakCenterLabel.AccessibleDescription = null;
            this.peakCenterLabel.AccessibleName = null;
            resources.ApplyResources(this.peakCenterLabel, "peakCenterLabel");
            this.peakCenterLabel.Font = null;
            // 
            // label9
            // 
            label9.AccessibleDescription = null;
            label9.AccessibleName = null;
            resources.ApplyResources(label9, "label9");
            label9.BackColor = SystemColors.Control;
            label9.Font = null;
            label9.Name = "label9";
            // 
            // label11
            // 
            label11.AccessibleDescription = null;
            label11.AccessibleName = null;
            resources.ApplyResources(label11, "label11");
            label11.BackColor = SystemColors.Control;
            label11.Font = null;
            label11.Name = "label11";
            // 
            // commentLabel
            // 
            commentLabel.AccessibleDescription = null;
            commentLabel.AccessibleName = null;
            resources.ApplyResources(commentLabel, "commentLabel");
            commentLabel.BackColor = SystemColors.Control;
            commentLabel.Font = null;
            commentLabel.Name = "commentLabel";
            // 
            // PreciseEditorLabelRow
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(label9);
            this.Controls.Add(label11);
            this.Controls.Add(commentLabel);
            this.Font = null;
            this.Name = "PreciseEditorLabelRow";
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
