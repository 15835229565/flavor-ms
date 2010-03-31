using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Flavor.Common;

namespace Flavor.Forms
{
    internal partial class ScanOptionsForm: OptionsForm
    {
        internal ScanOptionsForm(): base()
        {
            InitializeComponent();
            this.scan_groupBox.Text = string.Format("Интервал сканирования ({0}..{1})", Config.MIN_STEP, Config.MAX_STEP);
            this.endScanNumericUpDown.Minimum = new decimal(new int[] { Config.MIN_STEP, 0, 0, 0 });
            this.endScanNumericUpDown.Maximum = new decimal(new int[] { Config.MAX_STEP, 0, 0, 0 });
            this.startScanNumericUpDown.Minimum = new decimal(new int[] { Config.MIN_STEP, 0, 0, 0 });
            this.startScanNumericUpDown.Maximum = new decimal(new int[] { Config.MAX_STEP, 0, 0, 0 });
            loadStartEndData();
        }

        private void loadStartEndData()
        {
            startScanNumericUpDown.Value = (decimal)(Config.sPoint);
            endScanNumericUpDown.Value = (decimal)(Config.ePoint);
        }

        protected override void ok_butt_Click(object sender, EventArgs e)
        {
            if ((ushort)(startScanNumericUpDown.Value) > (ushort)(endScanNumericUpDown.Value))
            {
                startScanNumericUpDown.BackColor = Color.Red;
                endScanNumericUpDown.BackColor = Color.Red;
                return;
            }
            Config.saveScanOptions((ushort)(startScanNumericUpDown.Value), (ushort)(endScanNumericUpDown.Value));
            base.ok_butt_Click(sender, e);
        }

        protected override void applyButton_Click(object sender, EventArgs e)
        {
            if ((ushort)(startScanNumericUpDown.Value) <= (ushort)(endScanNumericUpDown.Value))
            {
                Config.saveScanOptions((ushort)(startScanNumericUpDown.Value), (ushort)(endScanNumericUpDown.Value));
                base.applyButton_Click(sender, e);
            }
            else
            {
                startScanNumericUpDown.BackColor = Color.Red;
                endScanNumericUpDown.BackColor = Color.Red;
            }
        }
    }
}