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
        private TextBox stepTextBox;
        private TextBox colTextBox;
        private TextBox lapsTextBox;
        private TextBox widthTextBox;
        private TextBox precTextBox;
        private TextBox commentTextBox;
        
        public AddPointForm()
        {
            InitializeComponent();
            this.stepTextBox = new TextBox();
            this.stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.stepTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.stepTextBox.Location = new System.Drawing.Point(58, 42);
            this.stepTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.stepTextBox.MaxLength = 4;
            this.stepTextBox.Size = new System.Drawing.Size(50, 13);
            this.stepTextBox.Enabled = false;

            this.colTextBox = new TextBox();
            this.colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.colTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.colTextBox.Location = new System.Drawing.Point(110, 42);
            this.colTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.colTextBox.MaxLength = 1;
            this.colTextBox.Size = new System.Drawing.Size(20, 13);
            this.colTextBox.Enabled = false;

            this.lapsTextBox = new TextBox();
            this.lapsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lapsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lapsTextBox.Location = new System.Drawing.Point(132, 42);
            this.lapsTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.lapsTextBox.Size = new System.Drawing.Size(50, 13);
            this.lapsTextBox.TextChanged += new System.EventHandler(Utility.integralTextbox_TextChanged);

            this.widthTextBox = new TextBox();
            this.widthTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.widthTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.widthTextBox.Location = new System.Drawing.Point(184, 42);
            this.widthTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.widthTextBox.MaxLength = 4;
            this.widthTextBox.Size = new System.Drawing.Size(50, 13);
            this.widthTextBox.TextChanged += new System.EventHandler(Utility.integralTextbox_TextChanged);

            this.precTextBox = new TextBox();
            this.precTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.precTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.precTextBox.Location = new System.Drawing.Point(236, 42);
            this.precTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.precTextBox.Size = new System.Drawing.Size(50, 13);
            this.precTextBox.TextChanged += new System.EventHandler(Utility.positiveNumericTextbox_TextChanged);

            this.commentTextBox = new TextBox();
            this.commentTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.commentTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.commentTextBox.Location = new System.Drawing.Point(288, 42);
            this.commentTextBox.Margin = new System.Windows.Forms.Padding(1);
            this.commentTextBox.Size = new System.Drawing.Size(100, 13);
        }
    }
}