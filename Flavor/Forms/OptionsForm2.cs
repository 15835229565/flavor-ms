using System;
using System.Windows.Forms;
using Config = Flavor.Common.Settings.Config;
using CommonOptions = Flavor.Common.Settings.CommonOptions;

namespace Flavor.Forms {
    partial class OptionsForm2: Form {
        protected OptionsForm2() {
            InitializeComponent();
        }

        void loadCommonData(CommonOptions co) {
            // TODO: remove hard-coded numbers here (use constants in Config)
            setupNumericUpDown(expTimeNumericUpDown, 1, uint.MaxValue, co.eTimeReal);
            //setupNumericUpDown(idleTimeNumericUpDown, 10, 100, co.iTimeReal);

            setupNumericUpDown(d1VoltageNumericUpDown, 0, 3000, co.d1VReal, CommonOptions.dVConvert, CommonOptions.dVConvert);
            setupNumericUpDown(d2VoltageNumericUpDown, 0, 3000, co.d2VReal, CommonOptions.dVConvert, CommonOptions.dVConvert);
            setupNumericUpDown(d3VoltageNumericUpDown, 0, 3000, co.d3VReal, CommonOptions.dVConvert, CommonOptions.dVConvert);
            setupNumericUpDown(CPNumericUpDown, (decimal)0.01, (decimal)0.09, co.C);
            setupNumericUpDown(kNumericUpDown, (decimal)0.6, (decimal)0.9, co.K);
            setupNumericUpDown(iVoltageNumericUpDown, 50, 100, co.iVoltageReal, CommonOptions.iVoltageConvert, CommonOptions.iVoltageConvert);
            setupNumericUpDown(eCurrentNumericUpDown, 0, 20, co.eCurrentReal, CommonOptions.eCurrentConvert, CommonOptions.eCurrentConvert);
            setupNumericUpDown(fV1NumericUpDown, 50, 100, co.fV1Real, CommonOptions.fV1Convert, CommonOptions.fV1Convert);
            setupNumericUpDown(fV2NumericUpDown, 50, 100, co.fV2Real, CommonOptions.fV2Convert, CommonOptions.fV2Convert);
        }
        protected void setupNumericUpDown(NumericUpDown updown, decimal min, decimal max, decimal value) {
            updown.Minimum = min;
            updown.Maximum = max;
            if (value < updown.Minimum)
                value = updown.Minimum;
            else if (value > updown.Maximum)
                value = updown.Maximum;
            updown.Value = value;
        }
        void setupNumericUpDown(NumericUpDown updown, decimal min, decimal max, double value) {
            setupNumericUpDown(updown, min, max, (decimal)value);
        }
        void setupNumericUpDown(NumericUpDown updown, decimal min, decimal max, ushort value) {
            setupNumericUpDown(updown, min, max, (decimal)value);
        }
        void setupNumericUpDown(NumericUpDown updown, double min, double max, double value, Converter<double, ushort> conv1, Converter<ushort, double> conv2) {
            setupNumericUpDown(updown, (decimal)conv2(conv1(min)), (decimal)conv2(conv1(max)), value);
        }

        protected void cancel_butt_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected virtual void ok_butt_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        protected virtual void applyButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Yes;
            Close();
        }

        public class SaveFileButtonClickEventArgs: EventArgs {
            public string FileName { get; private set; }
            public decimal[] Parameters { get; private set; }
            public SaveFileButtonClickEventArgs(string filename, params decimal[] parameters) {
                FileName = filename;
                Parameters = parameters;
            }
        }
        public event EventHandler<SaveFileButtonClickEventArgs> SaveFileButtonClick;
        void OnSaveFileButtonClick(string filename, params decimal[] parameters)        {
            var args = new SaveFileButtonClickEventArgs(filename, parameters);
            SaveFileButtonClick.Raise(this, args);
        }
        void saveFileButton_Click(object sender, EventArgs e) {
            if (saveCommonDataFileDialog.ShowDialog() == DialogResult.OK) {
                OnSaveFileButtonClick(saveCommonDataFileDialog.FileName,
                d1VoltageNumericUpDown.Value,
                d2VoltageNumericUpDown.Value,
                d3VoltageNumericUpDown.Value,
                iVoltageNumericUpDown.Value,
                eCurrentNumericUpDown.Value,
                fV1NumericUpDown.Value,
                fV2NumericUpDown.Value,
                CPNumericUpDown.Value,
                kNumericUpDown.Value,
                expTimeNumericUpDown.Value);
            }
        }

        void loadFileButton_Click(object sender, EventArgs e) {
            if (openCommonDataFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    loadCommonData(Config.loadCommonOptions(openCommonDataFileDialog.FileName));
                } catch (Config.ConfigLoadException cle) {
                    cle.visualise();
                }
            }
        }

        void adjustSettingsCheckBox_CheckedChanged(object sender, EventArgs e) {
            // TODO: enumerate collection and modify only proper controls
            bool ro = !adjustSettingsCheckBox.Checked;
            CPNumericUpDown.ReadOnly = ro;
            kNumericUpDown.ReadOnly = ro;
            fV1NumericUpDown.ReadOnly = ro;
            fV2NumericUpDown.ReadOnly = ro;
            d1VoltageNumericUpDown.ReadOnly = ro;
            d2VoltageNumericUpDown.ReadOnly = ro;
            d3VoltageNumericUpDown.ReadOnly = ro;
        }
        void InvokeSetVisibility(bool enabled, bool canApply) {
            Invoke(new Action(() => {
                // TODO: avoid bringing to front..
                Visible = enabled;
                params_groupBox.Enabled = enabled;
                rareModeCheckBox.Enabled = enabled;
                applyButton.Enabled = canApply;
                applyButton.Visible = canApply;
            }));
        }
        public class LoadEventArgs: EventArgs {
            public bool NotRareModeRequested { get; set; }
            public Action<bool, bool> Method { get; set; }
            public CommonOptions CommonOptions { get; set; }
        }
        protected override void OnLoad(EventArgs e) {
            var args = e.As<LoadEventArgs>();
            args.Method += InvokeSetVisibility;
            base.OnLoad(args);
            rareModeCheckBox.Checked = args.NotRareModeRequested;
            var co = args.CommonOptions;
            if (co != null)
                loadCommonData(co);
        }
        public class ClosingEventArgs: FormClosingEventArgs {
            public bool NotRareModeRequested { get; set; }
            public Action<bool, bool> Method { get; set; }
            public decimal[] Parameters { get; set; }
            public ClosingEventArgs(FormClosingEventArgs args)
                : base(args.CloseReason, args.Cancel) { }
        }
        protected override void OnFormClosing(FormClosingEventArgs e) {
            var args = e is ClosingEventArgs ? (ClosingEventArgs)e  : new ClosingEventArgs(e);
            args.Method += InvokeSetVisibility;
            if (DialogResult == DialogResult.OK || DialogResult == DialogResult.Yes) {
                args.NotRareModeRequested = rareModeCheckBox.Checked;
                args.Parameters = new[] { d1VoltageNumericUpDown.Value,
                    d2VoltageNumericUpDown.Value,
                    d3VoltageNumericUpDown.Value,
                    iVoltageNumericUpDown.Value,
                    eCurrentNumericUpDown.Value,
                    fV1NumericUpDown.Value,
                    fV2NumericUpDown.Value,
                    CPNumericUpDown.Value,
                    kNumericUpDown.Value,
                    expTimeNumericUpDown.Value,
                };
            }
            base.OnFormClosing(args);
        }
    }
}