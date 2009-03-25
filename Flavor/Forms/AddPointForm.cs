using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Flavor
{
    public partial class AddPointForm : Form
    {
        private Utility.PreciseEditorRow oneRow;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label colNumLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label commentLabel;

        public AddPointForm(ushort step, byte col): base()
        {
            InitializeComponent();
            //Move all this stuff to new PreciseEditorLabelRow class
            this.label8 = new System.Windows.Forms.Label();
            this.colNumLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.commentLabel = new System.Windows.Forms.Label();
            // label8
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(13, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 26);
            this.label8.Text = "Ступенька\r\n(<=1056)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // colNumLabel
            this.colNumLabel.AutoSize = true;
            this.colNumLabel.Location = new System.Drawing.Point(63, 29);
            this.colNumLabel.Name = "colNumLabel";
            this.colNumLabel.Size = new System.Drawing.Size(29, 13);
            this.colNumLabel.Text = "Кол.";
            // label9
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.SystemColors.Control;
            this.label9.Location = new System.Drawing.Point(88, 29);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.Text = "Проходы";
            // label10
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.SystemColors.Control;
            this.label10.Location = new System.Drawing.Point(140, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.Text = "Ширина";
            // label11
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.SystemColors.Control;
            this.label11.Location = new System.Drawing.Point(190, 29);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(54, 13);
            this.label11.Text = "Точность";
            // commentLabel
            this.commentLabel.AutoSize = true;
            this.commentLabel.BackColor = System.Drawing.SystemColors.Control;
            this.commentLabel.Location = new System.Drawing.Point(246, 29);
            this.commentLabel.Name = "commentLabel";
            this.commentLabel.Size = new System.Drawing.Size(54, 13);
            this.commentLabel.Text = "Комментарий";

            this.Controls.AddRange(new Control[] { label8, colNumLabel, label9, label10, label11, commentLabel});
            
            this.oneRow = new Utility.PreciseEditorRow(13, 50);
            this.oneRow.StepText = step.ToString();
            this.oneRow.ColText = col.ToString();
            this.Controls.AddRange(oneRow.getControls());
        }
    }
}