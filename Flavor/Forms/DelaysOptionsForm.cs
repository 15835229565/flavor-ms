using System;
using System.Windows.Forms;
// TODO: move up
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Forms {
    public partial class DelaysOptionsForm: Form {
        public DelaysOptionsForm() {
            InitializeComponent();
            beforeTimeNumericUpDown.Value = Config.CommonOptions.befTimeReal;
            forwardTimeNumericUpDown.Value = Config.CommonOptions.fTimeReal;
            backwardTimeNumericUpDown.Value = Config.CommonOptions.bTimeReal;
            forwardAsBeforeCheckBox.Checked = Config.CommonOptions.ForwardTimeEqualsBeforeTime;
        }

        void forwardAsBeforeCheckBox_CheckedChanged(object sender, EventArgs e) {
            forwardTimeNumericUpDown.ReadOnly = forwardAsBeforeCheckBox.Checked;
        }

        void cancel_butt_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        void ok_butt_Click(object sender, EventArgs e) {
            Config.saveGlobalDelaysOptions(forwardAsBeforeCheckBox.Checked, (ushort)beforeTimeNumericUpDown.Value,
                                     (ushort)forwardTimeNumericUpDown.Value, (ushort)backwardTimeNumericUpDown.Value);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}