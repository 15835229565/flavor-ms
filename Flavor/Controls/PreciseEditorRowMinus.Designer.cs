using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Controls {
    partial class PreciseEditorRowMinus {
        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreciseEditorRowMinus));
            this.stepTextBox = new System.Windows.Forms.TextBox();
            this.colTextBox = new System.Windows.Forms.TextBox();
            this.widthTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // stepTextBox
            // 
            this.stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.stepTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.stepTextBox, "stepTextBox");
            this.stepTextBox.Name = "stepTextBox";
            // 
            // colTextBox
            // 
            this.colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.colTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.colTextBox, "colTextBox");
            this.colTextBox.Name = "colTextBox";
            // 
            // widthTextBox
            // 
            this.widthTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.widthTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.widthTextBox, "widthTextBox");
            this.widthTextBox.Name = "widthTextBox";
            // 
            // PreciseEditorRowMinus
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stepTextBox);
            this.Controls.Add(this.colTextBox);
            this.Controls.Add(this.widthTextBox);
            this.Name = "PreciseEditorRowMinus";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected TextBox stepTextBox;
        protected TextBox colTextBox;
        protected TextBox widthTextBox;
    }
}
