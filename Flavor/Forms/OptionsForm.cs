using System;
using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Forms {
    internal partial class OptionsForm: Form {
        protected OptionsForm() {
            InitializeComponent();
            rareModeCheckBox.Checked = Commander.notRareModeRequested;
            if ((Commander.pState == Commander.programStates.Ready) || (Commander.pState == Commander.programStates.WaitHighVoltage) || (Commander.pState == Commander.programStates.Measure)) {
                applyButton.Enabled = true;
                applyButton.Visible = true;
            } else {
                applyButton.Enabled = false;
                applyButton.Visible = false;
            }
            loadCommonData(Config.CommonOptions);
        }

        private void loadCommonData(string fn) {
            loadCommonData(Config.loadCommonOptions(fn));
        }

        private void loadCommonData(CommonOptions co) {
            // TODO: remove hard-coded numbers here
            decimal temp;

            temp = (decimal)(co.eTimeReal);
            if (temp < expTimeNumericUpDown.Minimum) temp = expTimeNumericUpDown.Minimum;
            if (temp > expTimeNumericUpDown.Maximum) temp = expTimeNumericUpDown.Maximum;
            expTimeNumericUpDown.Value = temp;

            temp = (decimal)(co.iTimeReal);
            if (temp < idleTimeNumericUpDown.Minimum) temp = idleTimeNumericUpDown.Minimum;
            if (temp > idleTimeNumericUpDown.Maximum) temp = idleTimeNumericUpDown.Maximum;
            idleTimeNumericUpDown.Value = temp;

            iVoltageNumericUpDown.Minimum = (decimal)(CommonOptions.iVoltageConvert(CommonOptions.iVoltageConvert((double)20)));
            iVoltageNumericUpDown.Maximum = (decimal)(CommonOptions.iVoltageConvert(CommonOptions.iVoltageConvert((double)150)));
            temp = (decimal)(co.iVoltageReal);
            if (temp < iVoltageNumericUpDown.Minimum) temp = iVoltageNumericUpDown.Minimum;
            if (temp > iVoltageNumericUpDown.Maximum) temp = iVoltageNumericUpDown.Maximum;
            iVoltageNumericUpDown.Value = temp;

            CPNumericUpDown.Minimum = (decimal)(CommonOptions.CPConvert(CommonOptions.CPConvert((double)10)));
            CPNumericUpDown.Maximum = (decimal)(CommonOptions.CPConvert(CommonOptions.CPConvert((double)12)));
            temp = (decimal)(co.CPReal);
            if (temp < CPNumericUpDown.Minimum) temp = CPNumericUpDown.Minimum;
            if (temp > CPNumericUpDown.Maximum) temp = CPNumericUpDown.Maximum;
            CPNumericUpDown.Value = temp;

            eCurrentNumericUpDown.Minimum = (decimal)(CommonOptions.eCurrentConvert(CommonOptions.eCurrentConvert((double)0)));
            eCurrentNumericUpDown.Maximum = (decimal)(CommonOptions.eCurrentConvert(CommonOptions.eCurrentConvert((double)10)));
            temp = (decimal)(co.eCurrentReal);
            if (temp < eCurrentNumericUpDown.Minimum) temp = eCurrentNumericUpDown.Minimum;
            if (temp > eCurrentNumericUpDown.Maximum) temp = eCurrentNumericUpDown.Maximum;
            eCurrentNumericUpDown.Value = temp;

            hCurrentNumericUpDown.Minimum = (decimal)(CommonOptions.hCurrentConvert(CommonOptions.hCurrentConvert((double)0)));
            hCurrentNumericUpDown.Maximum = (decimal)(CommonOptions.hCurrentConvert(CommonOptions.hCurrentConvert((double)1)));
            temp = (decimal)(co.hCurrentReal);
            if (temp < hCurrentNumericUpDown.Minimum) temp = hCurrentNumericUpDown.Minimum;
            if (temp > hCurrentNumericUpDown.Maximum) temp = hCurrentNumericUpDown.Maximum;
            hCurrentNumericUpDown.Value = temp;

            fV1NumericUpDown.Minimum = (decimal)(CommonOptions.fV1Convert(CommonOptions.fV1Convert((double)20)));
            fV1NumericUpDown.Maximum = (decimal)(CommonOptions.fV1Convert(CommonOptions.fV1Convert((double)150)));
            temp = (decimal)(co.fV1Real);
            if (temp < fV1NumericUpDown.Minimum) temp = fV1NumericUpDown.Minimum;
            if (temp > fV1NumericUpDown.Maximum) temp = fV1NumericUpDown.Maximum;
            fV1NumericUpDown.Value = temp;

            fV2NumericUpDown.Minimum = (decimal)(CommonOptions.fV2Convert(CommonOptions.fV2Convert((double)20)));
            fV2NumericUpDown.Maximum = (decimal)(CommonOptions.fV2Convert(CommonOptions.fV2Convert((double)150)));
            temp = (decimal)(co.fV2Real);
            if (temp < fV2NumericUpDown.Minimum) temp = fV2NumericUpDown.Minimum;
            if (temp > fV2NumericUpDown.Maximum) temp = fV2NumericUpDown.Maximum;
            fV2NumericUpDown.Value = temp;
        }

        protected void cancel_butt_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected virtual void ok_butt_Click(object sender, EventArgs e) {
            Config.saveGlobalCommonOptions((ushort)expTimeNumericUpDown.Value, (ushort)(idleTimeNumericUpDown.Value),
                                   (double)(iVoltageNumericUpDown.Value), (double)(CPNumericUpDown.Value), (double)(eCurrentNumericUpDown.Value), (double)(hCurrentNumericUpDown.Value), (double)(fV1NumericUpDown.Value), (double)(fV2NumericUpDown.Value));
            Commander.notRareModeRequested = rareModeCheckBox.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected virtual void applyButton_Click(object sender, EventArgs e) {
            Commander.sendSettings();
            ok_butt_Click(sender, e);
        }

        protected void saveFileButton_Click(object sender, EventArgs e) {
            if (saveCommonDataFileDialog.ShowDialog() == DialogResult.OK) {
                Config.saveCommonOptions(saveCommonDataFileDialog.FileName, (ushort)(expTimeNumericUpDown.Value), (ushort)(idleTimeNumericUpDown.Value),
                                         (double)(iVoltageNumericUpDown.Value), (double)(CPNumericUpDown.Value), (double)(eCurrentNumericUpDown.Value), (double)(hCurrentNumericUpDown.Value), (double)(fV1NumericUpDown.Value), (double)(fV2NumericUpDown.Value));
            }
        }

        protected void loadFileButton_Click(object sender, EventArgs e) {
            if (openCommonDataFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    loadCommonData(openCommonDataFileDialog.FileName);
                } catch (Config.ConfigLoadException cle) {
                    cle.visualise();
                }
            }
        }

        private void adjustSettingsCheckBox_CheckedChanged(object sender, EventArgs e) {
            CPNumericUpDown.ReadOnly = !adjustSettingsCheckBox.Checked;
            fV1NumericUpDown.ReadOnly = !adjustSettingsCheckBox.Checked;
            fV2NumericUpDown.ReadOnly = !adjustSettingsCheckBox.Checked;
            hCurrentNumericUpDown.ReadOnly = !adjustSettingsCheckBox.Checked;
        }
    }
}