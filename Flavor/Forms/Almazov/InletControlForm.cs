using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            // TODO:
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
    }
}
