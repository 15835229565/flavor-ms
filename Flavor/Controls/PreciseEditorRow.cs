using System;
using System.Drawing;
using PreciseEditorData = Flavor.Common.Data.Measure.PreciseEditorData;

namespace Flavor.Controls {
    partial class PreciseEditorRow: PreciseEditorRowMinus {
        public string LapsText {
            get { return lapsTextBox.Text; }
        }
        public string PrecText {
            get { return precTextBox.Text; }
        }
        public string CommentText {
            get { return commentTextBox.Text; }
        }
        public PreciseEditorRow()
            : base() {
            InitializeComponent();
            lapsTextBox.KeyPress += ExtensionMethods.IntegralTextChanged;
            // temporarily disabled
            precTextBox.Enabled = false;
            precTextBox.Visible = false;
            //precTextBox.KeyPress += precTextBox.PositiveNumericTextChanged();
        }
        public override bool checkTextBoxes(ushort minStep, ushort maxStep) {
            bool somethingFilled = lapsTextBox.Text != "" || stepTextBox.Text != "" ||
                                    colTextBox.Text != "" || widthTextBox.Text != "" /*|| precTextBox].Text != ""*/;
            allFilled = lapsTextBox.Text != "" && stepTextBox.Text != "" && colTextBox.Text != "" &&
                              widthTextBox.Text != ""/* && (precTextBox.Text != "")*/;
            Color color;
            bool exitFlag;
            if (somethingFilled && !allFilled) {
                color = Color.Gold;
                exitFlag = false;
            } else {
                color = SystemColors.ControlDark;
                exitFlag = true;
            }
            stepTextBox.BackColor = color;
            colTextBox.BackColor = color;
            lapsTextBox.BackColor = color;
            widthTextBox.BackColor = color;
            precTextBox.BackColor = color;
            commentTextBox.BackColor = color;
            if (widthTextBox.Text != "" && stepTextBox.Text != "" &&
                (Convert.ToUInt16(stepTextBox.Text) - Convert.ToUInt16(widthTextBox.Text) < minStep ||
                Convert.ToUInt16(stepTextBox.Text) + Convert.ToUInt16(widthTextBox.Text) > maxStep)) {
                stepTextBox.BackColor = Color.Green;
                widthTextBox.BackColor = Color.Green;
                exitFlag = false;
            }
            if (stepTextBox.Text != "" && Convert.ToInt16(stepTextBox.Text) > maxStep) {
                stepTextBox.BackColor = Color.Red;
                exitFlag = false;
            }
            return exitFlag;
        }
        public override void setValues(PreciseEditorData p) {
            base.setValues(p);
            lapsTextBox.Text = p.Iterations.ToString();
            precTextBox.Text = p.Precision.ToString();
            commentTextBox.Text = p.Comment;
        }
    }
}

