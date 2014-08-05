using System;
using System.Drawing;
using System.Windows.Forms;
// remove this reference
using Utility = Flavor.Common.Utility;

namespace Flavor.Forms {
    partial class SetScalingCoeffForm: Form {
        readonly byte _col = 0;
        readonly ushort _step = 0;
        readonly bool _isLoaded;
        readonly Func<byte, ushort, double, bool> _setScalingCoeff;
        public SetScalingCoeffForm()
            : base() {
            InitializeComponent();
            // TODO: better solution, make new CustomTextBoxClass or extension method
            massTextBox.KeyPress += Utility.positiveNumericTextbox_TextChanged;
        }

        public SetScalingCoeffForm(ushort step, byte col, bool isLoaded, Func<byte, ushort, double, bool> setScalingCoeff)
            : this() {
            _col = col;
            _step = step;
            _isLoaded = isLoaded;
            stepTextBox.Text = step.ToString();
            Text += " " + col.ToString();
            if (isLoaded)
                Text += " (только для текущего спектра)";
            _setScalingCoeff = setScalingCoeff;
        }

        void okButton_Click(object sender, EventArgs e) {
            double mass;
            try {
                mass = Convert.ToDouble(massTextBox.Text);
            } catch (FormatException) {
                massTextBox.BackColor = Color.Red;
                return;
            } catch (OverflowException) {
                massTextBox.BackColor = Color.Red;
                return;
            }
            //check mass is not near 0
            if (mass < 1) {
                massTextBox.BackColor = Color.Red;
                return;
            }
            DialogResult = _setScalingCoeff(_col, _step, mass) ? DialogResult.Yes : DialogResult.No;
            Close();
        }
    }
}