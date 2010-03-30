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
    internal partial class PreciseEditorRowPlus : Flavor.Controls.PreciseEditorRow
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
        internal PreciseEditorRowPlus(): base()
        {
            InitializeComponent();

            this.stepAndColModifiable = true;
            this.stepTextBox.ReadOnly = !stepAndColModifiable;
            this.colTextBox.ReadOnly = !stepAndColModifiable;
            if (stepAndColModifiable)
                this.stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            else
                this.stepTextBox.BackColor = System.Drawing.SystemColors.Control;
            if (stepAndColModifiable)
                this.colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            else
                this.colTextBox.BackColor = System.Drawing.SystemColors.Control;

        }
        internal new void Clear()
        {
            base.Clear();
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

