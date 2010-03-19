using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Forms
{
    internal partial class MonitorOptionsForm : PreciseOptionsForm
    {
        private MonitorOptionsForm()
        {
            InitializeComponent();
            iterationsNumericUpDown.Maximum = new decimal(new int[] {int.MaxValue, 0, 0, 0});
            iterationsNumericUpDown.Value = (decimal)Config.Iterations;
        }

        private static MonitorOptionsForm instance = null;
        internal new static MonitorOptionsForm getInstance()
        {
            if (instance == null) instance = new MonitorOptionsForm();
            return instance;
        }

        protected override void ok_butt_Click(object sender, EventArgs e)
        {
            // TODO:
            Config.saveCheckOptions((int)iterationsNumericUpDown.Value);
            base.ok_butt_Click(sender, e);
        }

        private void MonitorOptionsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            instance = null;
        }
    }
}

