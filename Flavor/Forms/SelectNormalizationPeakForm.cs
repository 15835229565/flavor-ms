using System;
using System.Windows.Forms;
using Flavor.Controls;

namespace Flavor.Forms {
    partial class SetNormalizationPeakForm: Form {
        public SetNormalizationPeakForm()
            : base() {
            InitializeComponent();
            coeffTextBox.KeyPress += coeffTextBox.PositiveNumericTextChanged();
        }

        public class LoadEventArgs: EventArgs {
            public int NormPeakNumber { get; set; }
            public string[] PeakList { get; set; }
            public double Coeff { get; set; }
        }
        protected override void OnLoad(EventArgs e) {
            var args = e is LoadEventArgs ? (LoadEventArgs)e : new LoadEventArgs();
            base.OnLoad(args);
            peakComboBox.Items.AddRange(args.PeakList);
            peakComboBox.SelectedIndex = args.NormPeakNumber;
            //double coeff = args.Coeff;
        }
        public class ClosingEventArgs: FormClosingEventArgs {
            public int NormPeakNumber { get; set; }
            public double Coeff { get; set; }
            public ClosingEventArgs(FormClosingEventArgs args)
                : base(args.CloseReason, args.Cancel) { }
        }
        protected override void OnFormClosing(FormClosingEventArgs e) {
            var args = e is ClosingEventArgs ? (ClosingEventArgs)e : new ClosingEventArgs(e);
            if (DialogResult == DialogResult.OK) {
                args.NormPeakNumber = peakComboBox.SelectedIndex;
                //args.Coeff = ;
            }
            base.OnFormClosing(args);
        }
    }
}