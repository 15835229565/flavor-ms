using System;
using System.Drawing;
using System.Windows.Forms;

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
        KeyPressEventHandler collectorNumberTextChanged = null;
        public int MaxNumber {
            set {
                if (collectorNumberTextChanged != null)
                    colTextBox.KeyPress -= collectorNumberTextChanged;
                collectorNumberTextChanged = ExtensionMethods.OneDigitTextChanged(value);
                colTextBox.KeyPress += collectorNumberTextChanged;
            }
        }
        public PreciseEditorRowMinus()
            : base() {
            InitializeComponent();
            stepTextBox.KeyPress += ExtensionMethods.IntegralTextChanged;
            widthTextBox.KeyPress += ExtensionMethods.IntegralTextChanged;
        }
        public virtual bool checkTextBoxes(ushort minStep, ushort maxStep) {
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
        public virtual void setValues(PreciseEditorData p) {
            stepTextBox.Text = p.Step.ToString();
            colTextBox.Text = p.Collector.ToString();
            widthTextBox.Text = p.Width.ToString();
        }
    }
}
