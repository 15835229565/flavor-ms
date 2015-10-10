using System;
using System.Windows.Forms;
// TODO: move up
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Forms {
    public partial class DelaysOptionsForm: Form {
        public DelaysOptionsForm() {
            InitializeComponent();
            // TODO: move to OnLoad
            var co = Config.CommonOptions;
            standardDelayNumericUpDown.Value = co.iTimeReal;
            beforeTimeNumericUpDown.Value = co.befTimeReal;
            forwardTimeNumericUpDown.Value = co.fTimeReal;
            backwardTimeNumericUpDown.Value = co.bTimeReal;
            forwardAsBeforeCheckBox.Checked = co.ForwardTimeEqualsBeforeTime;
        }

        void forwardAsBeforeCheckBox_CheckedChanged(object sender, EventArgs e) {
            forwardTimeNumericUpDown.ReadOnly = forwardAsBeforeCheckBox.Checked;
        }

        void ok_butt_Click(object sender, EventArgs e) {
            Config.saveGlobalDelaysOptions((ushort)standardDelayNumericUpDown.Value, forwardAsBeforeCheckBox.Checked, (ushort)beforeTimeNumericUpDown.Value,
                                     (ushort)forwardTimeNumericUpDown.Value, (ushort)backwardTimeNumericUpDown.Value);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}