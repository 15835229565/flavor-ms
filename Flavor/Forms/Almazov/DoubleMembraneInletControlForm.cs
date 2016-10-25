using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Flavor.Forms.Almazov {
    partial class DoubleMembraneInletControlForm: Form {
        public DoubleMembraneInletControlForm() {
            InitializeComponent();
        }
        void sendButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }
        void cancelButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        protected override void OnLoad(EventArgs e) {
            inletRadioButton_CheckedChanged(this, e);
            temperatureCheckBox_CheckedChanged(this, e);

            base.OnLoad(e);
            var data = e.As<ParamsEventArgs<decimal>>().Parameters;
            if (data == null || data.Length != 3)
                return;
            temperatureNumericUpDown.Minimum = data[0];
            temperatureNumericUpDown.Maximum = data[1];
            // TODO: move to traslatable resources
            temperatureCheckBox.Text = string.Format("Температура ({0:F0}-{1:F0}C)", data[0], data[1]);
            temperatureNumericUpDown.Value = data[2];
        }
        public class ClosingEventArgs: FormClosingEventArgs {
            public bool Open{ get; set; }
            public decimal[] Parameters { get; set; }
            public ClosingEventArgs(FormClosingEventArgs args)
                : base(args.CloseReason, args.Cancel) { }
        }
        protected override void OnFormClosing(FormClosingEventArgs e) {
            var args = e is ClosingEventArgs ? (ClosingEventArgs)e : new ClosingEventArgs(e);
            if (DialogResult == DialogResult.OK) {
                args.Open = inletRadioButton.Checked;
                var ps = new List<decimal>(1);
                if (temperatureCheckBox.Checked) {
                    ps.Add(temperatureNumericUpDown.Value);
                }
                args.Parameters = ps.ToArray();
            }
            base.OnFormClosing(args);
        }
        void inletRadioButton_CheckedChanged(object sender, EventArgs e) {
            bool chked = inletRadioButton.Checked;
            temperatureCheckBox.Checked = chked;
        }
        void temperatureCheckBox_CheckedChanged(object sender, EventArgs e) {
            temperatureNumericUpDown.Enabled = temperatureCheckBox.Checked;
        }
    }
}
