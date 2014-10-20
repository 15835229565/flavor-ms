using System;
using System.Drawing;
using System.Windows.Forms;
using Utility = Flavor.Common.Utility;
using Config = Flavor.Common.Settings.Config;
using PreciseEditorData = Flavor.Common.Data.Measure.PreciseEditorData;

namespace Flavor.Controls {
    partial class PreciseEditorRowMinus: UserControl {
        public string StepText {
            get { return stepTextBox.Text; }
            set { stepTextBox.Text = value; }
        }
        public string ColText {
            get { return colTextBox.Text; }
            set { colTextBox.Text = value; }
        }
        public string WidthText {
            get { return widthTextBox.Text; }
        }
        protected bool allFilled = false;
        public bool AllFilled {
            get { return allFilled; }
        }
        public PreciseEditorRowMinus()
            : base() {
            InitializeComponent();

            stepTextBox.KeyPress += Utility.integralTextbox_TextChanged;
            colTextBox.KeyPress += Utility.oneDigitTextbox_TextChanged;
            widthTextBox.KeyPress += Utility.integralTextbox_TextChanged;
        }
        public virtual bool checkTextBoxes() {
            bool somethingFilled = (stepTextBox.Text != "") || (colTextBox.Text != "") || (widthTextBox.Text != "");
            allFilled = (stepTextBox.Text != "") && (colTextBox.Text != "") && (widthTextBox.Text != "");
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
            widthTextBox.BackColor = color;
            if (widthTextBox.Text != "" && stepTextBox.Text != "" &&
                (Convert.ToUInt16(stepTextBox.Text) - Convert.ToUInt16(widthTextBox.Text) < Config.MIN_STEP ||
                Convert.ToUInt16(stepTextBox.Text) + Convert.ToUInt16(widthTextBox.Text) > Config.MAX_STEP)) {
                stepTextBox.BackColor = Color.Green;
                widthTextBox.BackColor = Color.Green;
                exitFlag = false;
            }
            if (stepTextBox.Text != "" && Convert.ToInt16(stepTextBox.Text) > Config.MAX_STEP) {
                stepTextBox.BackColor = Color.Red;
                exitFlag = false;
            }
            return exitFlag;
        }
        public virtual void setValues(PreciseEditorData p) {
            stepTextBox.Text = p.Step.ToString();
            colTextBox.Text = p.Collector.ToString();
            widthTextBox.Text = p.Width.ToString();
        }
    }
}
