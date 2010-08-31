using System.Windows.Forms;
using Flavor.Common;
namespace Flavor.Controls {
    partial class PreciseEditorRow {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreciseEditorRow));
            this.lapsTextBox = new System.Windows.Forms.TextBox();
            this.precTextBox = new System.Windows.Forms.TextBox();
            this.commentTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // stepTextBox
            // 
            this.stepTextBox.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.stepTextBox, "stepTextBox");
            this.stepTextBox.ReadOnly = true;
            // 
            // colTextBox
            // 
            this.colTextBox.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.colTextBox, "colTextBox");
            // 
            // lapsTextBox
            // 
            this.lapsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lapsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.lapsTextBox, "lapsTextBox");
            this.lapsTextBox.Name = "lapsTextBox";
            // 
            // precTextBox
            // 
            this.precTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.precTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.precTextBox, "precTextBox");
            this.precTextBox.Name = "precTextBox";
            // 
            // commentTextBox
            // 
            this.commentTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.commentTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.commentTextBox, "commentTextBox");
            this.commentTextBox.Name = "commentTextBox";
            // 
            // PreciseEditorRow
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lapsTextBox);
            this.Controls.Add(this.precTextBox);
            this.Controls.Add(this.commentTextBox);
            this.Name = "PreciseEditorRow";
            this.Controls.SetChildIndex(this.commentTextBox, 0);
            this.Controls.SetChildIndex(this.precTextBox, 0);
            this.Controls.SetChildIndex(this.lapsTextBox, 0);
            this.Controls.SetChildIndex(this.widthTextBox, 0);
            this.Controls.SetChildIndex(this.colTextBox, 0);
            this.Controls.SetChildIndex(this.stepTextBox, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected TextBox lapsTextBox;
        protected TextBox precTextBox;
        protected TextBox commentTextBox;
    }
}
