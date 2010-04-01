using System;
using System.Drawing;
using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Controls {
    public partial class PreciseEditorRowPlus: PreciseEditorRow {
        internal string PeakNumber {
            set { peakNumberLabel.Text = value; }
        }
        internal bool UseChecked {
            get { return usePeakCheckBox.Checked; }
        }
        private static ToolTip clearRowToolTip = new ToolTip();
        public PreciseEditorRowPlus()
            : base() {
            InitializeComponent();
        }
        internal void Clear() {
            stepTextBox.Text = "";
            colTextBox.Text = "";
            stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;

            widthTextBox.Text = "";
            widthTextBox.BackColor = System.Drawing.SystemColors.ControlDark;

            lapsTextBox.Text = "";
            precTextBox.Text = "";
            commentTextBox.Text = "";
            lapsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            precTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            commentTextBox.BackColor = System.Drawing.SystemColors.ControlDark;

            usePeakCheckBox.Checked = false;
        }
        private void clearPeakButton_MouseHover(object sender, EventArgs e) {
            PreciseEditorRowPlus.clearRowToolTip.Show("Очистить строку", (IWin32Window)sender);
        }
        private void clearPeakButton_Click(object sender, EventArgs e) {
            this.Clear();
        }
        internal override bool checkTextBoxes() {
            return base.checkTextBoxes();
        }
        internal override void setValues(Utility.PreciseEditorData ped) {
            base.setValues(ped);
            usePeakCheckBox.Checked = ped.Use;
        }
    }
}

