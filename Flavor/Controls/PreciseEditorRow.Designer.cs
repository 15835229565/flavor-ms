using System.Windows.Forms;
using Flavor.Common;
namespace Flavor.Controls
{
    partial class PreciseEditorRow
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
            this.lapsTextBox = new System.Windows.Forms.TextBox();
            this.precTextBox = new System.Windows.Forms.TextBox();
            this.commentTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // stepTextBox
            // 
            this.stepTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.stepTextBox.ReadOnly = true;
            // 
            // colTextBox
            // 
            this.colTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.colTextBox.Enabled = false;
            // 
            // lapsTextBox
            // 
            this.lapsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lapsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lapsTextBox.Location = new System.Drawing.Point(126, 0);
            this.lapsTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.lapsTextBox.Name = "lapsTextBox";
            this.lapsTextBox.Size = new System.Drawing.Size(50, 13);
            this.lapsTextBox.TabIndex = 3;
            // 
            // precTextBox
            // 
            this.precTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.precTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.precTextBox.Location = new System.Drawing.Point(178, 0);
            this.precTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.precTextBox.Name = "precTextBox";
            this.precTextBox.Size = new System.Drawing.Size(50, 13);
            this.precTextBox.TabIndex = 4;
            // 
            // commentTextBox
            // 
            this.commentTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.commentTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.commentTextBox.Location = new System.Drawing.Point(230, 0);
            this.commentTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.commentTextBox.Name = "commentTextBox";
            this.commentTextBox.Size = new System.Drawing.Size(100, 13);
            this.commentTextBox.TabIndex = 5;
            // 
            // PreciseEditorRow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.lapsTextBox);
            this.Controls.Add(this.precTextBox);
            this.Controls.Add(this.commentTextBox);
            this.Name = "PreciseEditorRow";
            this.Size = new System.Drawing.Size(330, 13);
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
