using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Controls
{
    partial class PreciseEditorRowMinus
    {
        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.stepTextBox = new System.Windows.Forms.TextBox();
            this.colTextBox = new System.Windows.Forms.TextBox();
            this.widthTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // stepTextBox
            // 
            this.stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.stepTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.stepTextBox.Location = new System.Drawing.Point(0, 0);
            this.stepTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.stepTextBox.MaxLength = 4;
            this.stepTextBox.Name = "stepTextBox";
            this.stepTextBox.Size = new System.Drawing.Size(50, 13);
            this.stepTextBox.TabIndex = 0;
            // 
            // colTextBox
            // 
            this.colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.colTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.colTextBox.Location = new System.Drawing.Point(52, 0);
            this.colTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.colTextBox.MaxLength = 1;
            this.colTextBox.Name = "colTextBox";
            this.colTextBox.Size = new System.Drawing.Size(20, 13);
            this.colTextBox.TabIndex = 1;
            // 
            // widthTextBox
            // 
            this.widthTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.widthTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.widthTextBox.Location = new System.Drawing.Point(74, 0);
            this.widthTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.widthTextBox.MaxLength = 4;
            this.widthTextBox.Name = "widthTextBox";
            this.widthTextBox.Size = new System.Drawing.Size(50, 13);
            this.widthTextBox.TabIndex = 2;
            // 
            // PreciseEditorRowMinus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stepTextBox);
            this.Controls.Add(this.colTextBox);
            this.Controls.Add(this.widthTextBox);
            this.Name = "PreciseEditorRowMinus";
            this.Size = new System.Drawing.Size(124, 13);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected TextBox stepTextBox;
        protected TextBox colTextBox;
        protected TextBox widthTextBox;
    }
}
