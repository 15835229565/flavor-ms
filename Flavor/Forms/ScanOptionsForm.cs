using System;
using System.Drawing;
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Forms {
    partial class ScanOptionsForm: OptionsForm2 {
        public ScanOptionsForm()
            : base() {
            InitializeComponent();
            this.scan_groupBox.Text = string.Format("Интервал сканирования ({0}..{1})", Config.MIN_STEP, Config.MAX_STEP);
            this.endScanNumericUpDown.Minimum = Config.MIN_STEP;
            this.endScanNumericUpDown.Maximum = Config.MAX_STEP;
            this.startScanNumericUpDown.Minimum = Config.MIN_STEP;
            this.startScanNumericUpDown.Maximum = Config.MAX_STEP;
            loadStartEndData();
        }

        void loadStartEndData() {
            startScanNumericUpDown.Value = Config.sPoint >= Config.MIN_STEP ? Config.sPoint : Config.MIN_STEP;
            endScanNumericUpDown.Value = Config.ePoint <= Config.MAX_STEP ? Config.ePoint : Config.MAX_STEP;
        }

        protected override void ok_butt_Click(object sender, EventArgs e) {
            if (startScanNumericUpDown.Value > endScanNumericUpDown.Value) {
                startScanNumericUpDown.BackColor = Color.Red;
                endScanNumericUpDown.BackColor = Color.Red;
                return;
            }
            Config.saveGlobalScanOptions((ushort)startScanNumericUpDown.Value, (ushort)endScanNumericUpDown.Value);
            base.ok_butt_Click(sender, e);
        }

        protected override void applyButton_Click(object sender, EventArgs e) {
            if (startScanNumericUpDown.Value > endScanNumericUpDown.Value) {
                startScanNumericUpDown.BackColor = Color.Red;
                endScanNumericUpDown.BackColor = Color.Red;
                return;
            }
            Config.saveGlobalScanOptions((ushort)startScanNumericUpDown.Value, (ushort)endScanNumericUpDown.Value);
            base.applyButton_Click(sender, e);
        }
    }
}