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
        protected override void OnLoad(EventArgs e) {
            inletRadioButton_CheckedChanged(this, e);
            base.OnLoad(e);
        }
        public class ClosingEventArgs: FormClosingEventArgs {
            public bool UseCapillary { get; set; }
            public decimal[] Parameters { get; set; }
            public ClosingEventArgs(FormClosingEventArgs args)
                : base(args.CloseReason, args.Cancel) { }
        }
        protected override void OnFormClosing(FormClosingEventArgs e) {
            var args = e is ClosingEventArgs ? e as ClosingEventArgs : new ClosingEventArgs(e);
            if (DialogResult == DialogResult.OK) {
                bool useCapillary = capillaryRadioButton.Checked;
                args.UseCapillary = useCapillary;
                if (!useCapillary) {
                    args.Parameters = new decimal[] { voltageТumericUpDown.Value, temperatureNumericUpDown.Value };
                }
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
