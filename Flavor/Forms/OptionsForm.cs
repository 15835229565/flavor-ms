using System;
using System.Windows.Forms;
using Config = Flavor.Common.Config;
using CommonOptions = Flavor.Common.CommonOptions;
// really here?
using ProgramStates = Flavor.Common.ProgramStates;
using ProgramEventHandler = Flavor.Common.ProgramEventHandler;

namespace Flavor.Forms {
    internal partial class OptionsForm: Form {
        protected OptionsForm() {
            InitializeComponent();
        }

        private void loadCommonData(CommonOptions co) {
            // TODO: remove hard-coded numbers here (use constants in Config)
            setupNumericUpDown(expTimeNumericUpDown, 10, 10000, co.eTimeReal);
            setupNumericUpDown(idleTimeNumericUpDown, 10, 100, co.iTimeReal);

            setupNumericUpDown(iVoltageNumericUpDown, 20, 150, co.iVoltageReal, CommonOptions.iVoltageConvert, CommonOptions.iVoltageConvert);
            setupNumericUpDown(CPNumericUpDown, 10, 12, co.CPReal, CommonOptions.CPConvert, CommonOptions.CPConvert);
            setupNumericUpDown(eCurrentNumericUpDown, 0, 10, co.eCurrentReal, CommonOptions.eCurrentConvert, CommonOptions.eCurrentConvert);
            setupNumericUpDown(hCurrentNumericUpDown, 0, 1, co.hCurrentReal, CommonOptions.hCurrentConvert, CommonOptions.hCurrentConvert);
            setupNumericUpDown(fV1NumericUpDown, 20, 150, co.fV1Real, CommonOptions.fV1Convert, CommonOptions.fV1Convert);
            setupNumericUpDown(fV2NumericUpDown, 20, 150, co.fV2Real, CommonOptions.fV2Convert, CommonOptions.fV2Convert);
        }
        private void setupNumericUpDown(NumericUpDown updown, decimal min, decimal max, double value) {
            updown.Minimum = min;
            updown.Maximum = max;
            decimal temp = (decimal)value;
            if (temp < updown.Minimum)
                temp = updown.Minimum;
            else if (temp > updown.Maximum)
                temp = updown.Maximum;
            updown.Value = temp;
        }
        private delegate ushort ConvertTo(double value);
        private delegate double ConvertFro(ushort value);
        private void setupNumericUpDown(NumericUpDown updown, double min, double max, double value, ConvertTo conv1, ConvertFro conv2) {
            setupNumericUpDown(updown, (decimal)conv2(conv1(min)), (decimal)conv2(conv1(max)), value);
        }

        protected void cancel_butt_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected virtual void ok_butt_Click(object sender, EventArgs e) {
            Config.saveGlobalCommonOptions(
                (ushort)expTimeNumericUpDown.Value,
                (ushort)idleTimeNumericUpDown.Value,
                (double)iVoltageNumericUpDown.Value,
                (double)CPNumericUpDown.Value,
                (double)eCurrentNumericUpDown.Value,
                (double)hCurrentNumericUpDown.Value,
                (double)fV1NumericUpDown.Value,
                (double)fV2NumericUpDown.Value);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected virtual void applyButton_Click(object sender, EventArgs e) {
            Config.saveGlobalCommonOptions(
                (ushort)expTimeNumericUpDown.Value,
                (ushort)idleTimeNumericUpDown.Value,
                (double)iVoltageNumericUpDown.Value,
                (double)CPNumericUpDown.Value,
                (double)eCurrentNumericUpDown.Value,
                (double)hCurrentNumericUpDown.Value,
                (double)fV1NumericUpDown.Value,
                (double)fV2NumericUpDown.Value);
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        protected void saveFileButton_Click(object sender, EventArgs e) {
            if (saveCommonDataFileDialog.ShowDialog() == DialogResult.OK) {
                Config.saveCommonOptions(saveCommonDataFileDialog.FileName, (ushort)expTimeNumericUpDown.Value, (ushort)idleTimeNumericUpDown.Value,
                                         (double)iVoltageNumericUpDown.Value, (double)CPNumericUpDown.Value, (double)eCurrentNumericUpDown.Value, (double)hCurrentNumericUpDown.Value,
                                         (double)fV1NumericUpDown.Value, (double)fV2NumericUpDown.Value);
            }
        }

        protected void loadFileButton_Click(object sender, EventArgs e) {
            if (openCommonDataFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    loadCommonData(Config.loadCommonOptions(openCommonDataFileDialog.FileName));
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
        private void InvokeSetVisibility(ProgramStates state) {
            this.Invoke(new Action(() => {
                // TODO: avoid bringing to front..
                this.Visible = state != ProgramStates.Measure ||
                    state != ProgramStates.BackgroundMeasureReady ||
                    state != ProgramStates.WaitBackgroundMeasure;
            }));
        }
        public class LoadEventArgs: EventArgs {
            public bool Enabled { get; set; }
            public bool NotRareModeRequested { get; set; }
            public ProgramEventHandler Method { get; set; }
        }
        protected override void OnLoad(EventArgs e) {
            var args = e is LoadEventArgs ? e as LoadEventArgs : new LoadEventArgs();
            args.Method += InvokeSetVisibility;
            base.OnLoad(args);
            rareModeCheckBox.Checked = args.NotRareModeRequested;
            if (args.Enabled) {
                applyButton.Enabled = true;
                applyButton.Visible = true;
            } else {
                applyButton.Enabled = false;
                applyButton.Visible = false;
            }
            CommonOptions co = Config.CommonOptions;
            if (co != null)
                loadCommonData(Config.CommonOptions);
        }
        public class ClosingEventArgs: FormClosingEventArgs {
            public bool NotRareModeRequested { get; set; }
            public ProgramEventHandler Method { get; set; }
            public ClosingEventArgs(FormClosingEventArgs args)
                : base(args.CloseReason, args.Cancel) { }
        }
        protected override void OnFormClosing(FormClosingEventArgs e) {
            var args = e is ClosingEventArgs ? e as ClosingEventArgs : new ClosingEventArgs(e);
            args.NotRareModeRequested = rareModeCheckBox.Checked;
            args.Method += InvokeSetVisibility;
            base.OnFormClosing(args);
        }
    }
}