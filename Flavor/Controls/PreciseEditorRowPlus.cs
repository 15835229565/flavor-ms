using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using PreciseEditorData = Flavor.Common.Data.Measure.PreciseEditorData;

namespace Flavor.Controls {
    public partial class PreciseEditorRowPlus: PreciseEditorRow {
        internal string PeakNumber {
            set { peakNumberLabel.Text = value; }
        }
        internal bool UseChecked {
            get { return usePeakCheckBox.Checked; }
        }
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
        private void clearPeakButton_Click(object sender, EventArgs e) {
            this.Clear();
        }
        
        internal void setClearToolTip(ToolTip toolTip) {
            if (toolTip == null) {
                return;
            }
            toolTip.SetToolTip(this.clearPeakButton, "Очистить строку");
        }
        
        internal override bool checkTextBoxes() {
            return base.checkTextBoxes();
        }
        internal override void setValues(PreciseEditorData ped) {
            base.setValues(ped);
            usePeakCheckBox.Checked = ped.Use;
        }
    }
}

