using System;
using System.Drawing;
using System.Windows.Forms;
using PreciseEditorData = Flavor.Common.Data.Measure.PreciseEditorData;

namespace Flavor.Controls {
    partial class PreciseEditorRowPlus: PreciseEditorRow {
        public string PeakNumber {
            set { peakNumberLabel.Text = value; }
        }
        public bool UseChecked {
            get { return usePeakCheckBox.Checked; }
        }
        public PreciseEditorRowPlus()
            : base() {
            InitializeComponent();
        }
        public void Clear() {
            var color = SystemColors.ControlDark;
            stepTextBox.Text = "";
            colTextBox.Text = "";
            stepTextBox.BackColor = color;
            colTextBox.BackColor = color;

            widthTextBox.Text = "";
            widthTextBox.BackColor = color;

            lapsTextBox.Text = "";
            precTextBox.Text = "";
            commentTextBox.Text = "";
            lapsTextBox.BackColor = color;
            precTextBox.BackColor = color;
            commentTextBox.BackColor = color;

            usePeakCheckBox.Checked = false;
        }
        void clearPeakButton_Click(object sender, EventArgs e) {
            this.Clear();
        }
        
        public void setClearToolTip(ToolTip toolTip) {
            if (toolTip == null) {
                return;
            }
            toolTip.SetToolTip(this.clearPeakButton, "Очистить строку");
        }
        
        public override bool checkTextBoxes() {
            return base.checkTextBoxes();
        }
        public override void setValues(PreciseEditorData ped) {
            base.setValues(ped);
            usePeakCheckBox.Checked = ped.Use;
        }
    }
}

