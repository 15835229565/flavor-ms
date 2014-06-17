using System;
using System.Drawing;
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Forms {
    partial class ScanOptionsForm: OptionsForm2 {
        public ScanOptionsForm()
            : base() {
            InitializeComponent();
            this.scan_groupBox.Text = string.Format("Интервал сканирования ({0}..{1})", Config.MIN_STEP, Config.MAX_STEP);
            setupNumericUpDown(startScanNumericUpDown, Config.MIN_STEP, Config.MAX_STEP, Config.sPoint);
            setupNumericUpDown(endScanNumericUpDown, Config.MIN_STEP, Config.MAX_STEP, Config.ePoint);
        }

        protected override void ok_butt_Click(object sender, EventArgs e) {
            checkAndSave(base.ok_butt_Click, sender, e);
        }
        protected override void applyButton_Click(object sender, EventArgs e) {
            checkAndSave(base.applyButton_Click, sender, e);
        }
        void checkAndSave(EventHandler action, object sender, EventArgs e) {
            if (startScanNumericUpDown.Value > endScanNumericUpDown.Value) {
                startScanNumericUpDown.BackColor = Color.Red;
                endScanNumericUpDown.BackColor = Color.Red;
                return;
            }
            Config.saveGlobalScanOptions((ushort)startScanNumericUpDown.Value, (ushort)endScanNumericUpDown.Value);
            action(sender, e);
        }
    }
}