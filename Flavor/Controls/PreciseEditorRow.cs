using System;
using System.Drawing;
using Utility = Flavor.Common.Utility;
using Config = Flavor.Common.Config;
using PreciseEditorData = Flavor.Common.Utility.PreciseEditorData;

namespace Flavor.Controls {
    public partial class PreciseEditorRow: PreciseEditorRowMinus {
        internal string LapsText {
            get { return lapsTextBox.Text; }
        }
        internal string PrecText {
            get { return precTextBox.Text; }
        }
        internal string CommentText {
            get { return commentTextBox.Text; }
        }
        public PreciseEditorRow()
            : base() {
            InitializeComponent();
            this.lapsTextBox.KeyPress += Utility.integralTextbox_TextChanged;
            this.precTextBox.KeyPress += Utility.positiveNumericTextbox_TextChanged;
        }
        internal override bool checkTextBoxes() {
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
            if (somethingFilled && !this.allFilled) {
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
                (Convert.ToUInt16(stepTextBox.Text) + Convert.ToUInt16(widthTextBox.Text) > Config.MAX_STEP))) {
                stepTextBox.BackColor = Color.Green;
                widthTextBox.BackColor = Color.Green;
                exitFlag = false;
            }
            if ((stepTextBox.Text != "") && (Convert.ToInt16(stepTextBox.Text) > Config.MAX_STEP)) {
                stepTextBox.BackColor = Color.Red;
                exitFlag = false;
            }
            return exitFlag;
        }
        internal override void setValues(PreciseEditorData p) {
            base.setValues(p);
            lapsTextBox.Text = p.Iterations.ToString();
            precTextBox.Text = p.Precision.ToString();
            commentTextBox.Text = p.Comment;
        }
    }
}

