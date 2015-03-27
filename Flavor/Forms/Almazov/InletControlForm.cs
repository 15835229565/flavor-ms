using System;
using System.Windows.Forms;

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
            var args = e is LoadEventArgs ? e as LoadEventArgs : new LoadEventArgs(e);
            base.OnLoad(args);
            var data = args.Parameters;
            if (data == null)
                return;
            voltageТumericUpDown.Minimum = data[0];
            voltageТumericUpDown.Maximum = data[1];
            // TODO: move to traslatable resources
            label1.Text = string.Format("Напряжение ({0:F0}-{1:F0}В)", data[0], data[1]);
            voltageТumericUpDown.Value = data[2];
            temperatureNumericUpDown.Minimum = data[3];
            temperatureNumericUpDown.Maximum = data[4];
            // TODO: move to traslatable resources
            label2.Text = string.Format("Температура ({0:F0}-{1:F0}C)", data[3], data[4]);
            temperatureNumericUpDown.Value = data[5];
        }
        public class ClosingEventArgs: FormClosingEventArgs {
            public bool? UseCapillary { get; set; }
            public decimal[] Parameters { get; set; }
            public ClosingEventArgs(FormClosingEventArgs args)
                : base(args.CloseReason, args.Cancel) { }
        }
        protected override void OnFormClosing(FormClosingEventArgs e) {
            var args = e is ClosingEventArgs ? e as ClosingEventArgs : new ClosingEventArgs(e);
            if (DialogResult == DialogResult.OK) {
                bool? useCapillary;
                if (closeInletRadioButton.Checked) {
                    useCapillary = null;
                } else if (capillaryRadioButton.Checked) {
                    useCapillary = true;
                } else {
                    useCapillary = false;
                    args.Parameters = new decimal[] { voltageТumericUpDown.Value, temperatureNumericUpDown.Value };
                }
                args.UseCapillary = useCapillary;
            }
            base.OnFormClosing(args);
        }
        void inletRadioButton_CheckedChanged(object sender, EventArgs e) {
            // mandatory off
            //inletGroupBox.Enabled = false;
            inletGroupBox.Enabled = inletRadioButton.Checked;
        }
    }
}
