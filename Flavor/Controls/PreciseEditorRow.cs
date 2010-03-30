using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Controls
{
    internal partial class PreciseEditorRow: Flavor.Controls.PreciseEditorRowMinus
    {
        internal string LapsText
        {
            get { return lapsTextBox.Text; }
        }
        internal string PrecText
        {
            get { return precTextBox.Text; }
        }
        internal string CommentText
        {
            get { return commentTextBox.Text; }
        }
        internal PreciseEditorRow()
        {
            InitializeComponent();
        }
        protected override void Clear()
        {
            base.Clear();
            lapsTextBox.Text = "";
            precTextBox.Text = "";
            commentTextBox.Text = "";
            lapsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            precTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            commentTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
        }
        internal override bool checkTextBoxes()
        {
            bool exitFlag = true;
            bool somethingFilled = ((lapsTextBox.Text != "") || (stepTextBox.Text != "") ||
                                    (colTextBox.Text != "") || (widthTextBox.Text != "") /*|| (precTextBox].Text != "")*/);
            this.allFilled = ((lapsTextBox.Text != "") && (stepTextBox.Text != "") && (colTextBox.Text != "") &&
                              (widthTextBox.Text != "")/* && (precTextBox.Text != "")*/);
            stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            lapsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            widthTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            precTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            commentTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            if (somethingFilled && !this.allFilled)
            {
                stepTextBox.BackColor = Color.Gold;
                colTextBox.BackColor = Color.Gold;
                lapsTextBox.BackColor = Color.Gold;
                widthTextBox.BackColor = Color.Gold;
                precTextBox.BackColor = Color.Gold;
                commentTextBox.BackColor = Color.Gold;
                exitFlag = false;
            }
            if ((widthTextBox.Text != "") && (stepTextBox.Text != "") &&
                ((Convert.ToUInt16(stepTextBox.Text) - Convert.ToUInt16(widthTextBox.Text) < Config.MIN_STEP) ||
                (Convert.ToUInt16(stepTextBox.Text) + Convert.ToUInt16(widthTextBox.Text) > Config.MAX_STEP)))
            {
                stepTextBox.BackColor = Color.Green;
                widthTextBox.BackColor = Color.Green;
                exitFlag = false;
            }
            if ((stepTextBox.Text != "") && (Convert.ToInt16(stepTextBox.Text) > Config.MAX_STEP))
            {
                stepTextBox.BackColor = Color.Red;
                exitFlag = false;
            }
            return exitFlag;
        }
        internal override void setValues(Utility.PreciseEditorData p)
        {
            base.setValues(p);
            lapsTextBox.Text = p.Iterations.ToString();
            precTextBox.Text = p.Precision.ToString();
            commentTextBox.Text = p.Comment;
        }
    }
}

