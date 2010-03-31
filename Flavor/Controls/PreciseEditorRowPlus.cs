using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Controls
{
    public partial class PreciseEditorRowPlus: PreciseEditorRow
    {
        internal string PeakNumber
        {
            set { peakNumberLabel.Text = value; }
        }
        internal bool UseChecked
        {
            get { return usePeakCheckBox.Checked; }
        }
        private static ToolTip clearRowToolTip = new ToolTip();
        public PreciseEditorRowPlus(): base()
        {
            InitializeComponent();

            this.stepAndColModifiable = true;

            this.SuspendLayout();

            this.stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.stepTextBox.TextChanged += new System.EventHandler(Utility.integralTextbox_TextChanged);
            this.colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.colTextBox.TextChanged += new System.EventHandler(Utility.oneDigitTextbox_TextChanged);

            this.stepTextBox.ReadOnly = false;
            this.colTextBox.ReadOnly = false;

            this.ResumeLayout(false);
            this.PerformLayout();
        }
        internal void Clear()
        {
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
        private void clearPeakButton_MouseHover(object sender, EventArgs e)
        {
            PreciseEditorRowPlus.clearRowToolTip.Show("Очистить строку", (IWin32Window)sender);
        }
        private void clearPeakButton_Click(object sender, EventArgs e)
        {
            this.Clear();
        }
        internal override bool checkTextBoxes()
        {
            return base.checkTextBoxes();
        }
        internal override void setValues(Utility.PreciseEditorData ped)
        {
            base.setValues(ped);
            usePeakCheckBox.Checked = ped.Use;
        }
    }
}

