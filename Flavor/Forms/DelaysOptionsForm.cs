using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Flavor
{
    public partial class DelaysOptionsForm : Form
    {
        public DelaysOptionsForm()
        {
            InitializeComponent();
            beforeTimeNumericUpDown.Value = Config.CommonOptions.befTimeReal;
            forwardTimeNumericUpDown.Value = Config.CommonOptions.fTimeReal;
            backwardTimeNumericUpDown.Value = Config.CommonOptions.bTimeReal;
            forwardAsBeforeCheckBox.Checked = Config.CommonOptions.ForwardTimeEqualsBeforeTime;
        }

        private void forwardAsBeforeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            forwardTimeNumericUpDown.ReadOnly = forwardAsBeforeCheckBox.Checked;
        }

        private void cancel_butt_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ok_butt_Click(object sender, EventArgs e)
        {
            Config.saveDelaysOptions(forwardAsBeforeCheckBox.Checked, (ushort)beforeTimeNumericUpDown.Value,
                                     (ushort)forwardTimeNumericUpDown.Value, (ushort)backwardTimeNumericUpDown.Value);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}