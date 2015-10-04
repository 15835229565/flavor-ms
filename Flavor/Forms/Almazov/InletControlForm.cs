using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Flavor.Forms.Almazov {
    public partial class InletControlForm: Form {
        public InletControlForm() {
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
        public class LoadEventArgs: EventArgs {
            //public bool? UseCapillary { get; set; }
            public decimal[] Parameters { get; set; }
            public LoadEventArgs(EventArgs args)
                : base() { }
        }
        protected override void OnLoad(EventArgs e) {
            inletRadioButton_CheckedChanged(this, e);
            temperatureCheckBox_CheckedChanged(this, e);

            var args = e is LoadEventArgs ? (LoadEventArgs)e : new LoadEventArgs(e);
            base.OnLoad(args);
            var data = args.Parameters;
            if (data == null)
                return;
            voltageNumericUpDown.Minimum = data[0];
            voltageNumericUpDown.Maximum = data[1];
            // TODO: move to traslatable resources
            inletRadioButton.Text = string.Format("Напряжение ({0:F0}-{1:F0}В)", data[0], data[1]);
            voltageNumericUpDown.Value = data[2];
            temperatureNumericUpDown.Minimum = data[3];
            temperatureNumericUpDown.Maximum = data[4];
            // TODO: move to traslatable resources
            temperatureCheckBox.Text = string.Format("Температура ({0:F0}-{1:F0}C)", data[3], data[4]);
            temperatureNumericUpDown.Value = data[5];
        }
        public class ClosingEventArgs: FormClosingEventArgs {
            public bool? UseCapillary { get; set; }
            public decimal[] Parameters { get; set; }
            public ClosingEventArgs(FormClosingEventArgs args)
                : base(args.CloseReason, args.Cancel) { }
        }
        protected override void OnFormClosing(FormClosingEventArgs e) {
            var args = e is ClosingEventArgs ? (ClosingEventArgs)e : new ClosingEventArgs(e);
            if (DialogResult == DialogResult.OK) {
                bool? useCapillary;
                var ps = new List<decimal>(2);
                if (temperatureCheckBox.Checked) {
                    ps.Add(temperatureNumericUpDown.Value);
                }
                if (inletRadioButton.Checked) {
                    ps.Add(voltageNumericUpDown.Value);
                    useCapillary = false;
                } else if (capillaryRadioButton.Checked) {
                    useCapillary = true;
                } else {
                    useCapillary = null;
                }
                args.UseCapillary = useCapillary;
                args.Parameters = ps.ToArray();
            }
            base.OnFormClosing(args);
        }
        void inletRadioButton_CheckedChanged(object sender, EventArgs e) {
            temperatureCheckBox.Checked = inletRadioButton.Checked;
        }
        void temperatureCheckBox_CheckedChanged(object sender, EventArgs e) {
            temperatureNumericUpDown.Enabled = temperatureCheckBox.Checked;
        }
    }
}
