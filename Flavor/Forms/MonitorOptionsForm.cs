using System;
using System.Windows.Forms;
using Flavor.Controls;
using Config = Flavor.Common.Settings.Config;

using PreciseEditorData = Flavor.Common.Data.Measure.PreciseEditorData;
// remove this reference from here..
using Graph = Flavor.Common.Data.Measure.Graph;

namespace Flavor.Forms {
    partial class MonitorOptionsForm: PreciseOptionsForm {
        public MonitorOptionsForm() {
            InitializeComponent();
            checkPeakPreciseEditorRowMinus.MaxNumber = Config.COLLECTOR_COEFFS.Length;
        }

        protected override bool checkTextBoxes() {
            return checkPeakPreciseEditorRowMinus.checkTextBoxes(Config.MIN_STEP, Config.MAX_STEP) & base.checkTextBoxes();
        }
        protected override void saveData() {
            base.saveData();
            Config.saveGlobalCheckOptions((int)iterationsNumericUpDown.Value, (int)timeLimitNumericUpDown.Value, (ushort)allowedShiftNumericUpDown.Value,
                                    checkPeakPreciseEditorRowMinus.AllFilled?
                                    new PreciseEditorData(false, byte.MaxValue, Convert.ToUInt16(checkPeakPreciseEditorRowMinus.StepText),
                                                                  Convert.ToByte(checkPeakPreciseEditorRowMinus.ColText), 0,
                                                                  Convert.ToUInt16(checkPeakPreciseEditorRowMinus.WidthText), 0, "checker peak"):
                                    null, (int)checkPeakNumberNumericUpDown.Value, 
                                    (byte)backroundMeasureCycleCountNumericUpDown.Value);
        }

        void Graph_OnPointAdded(bool notNull) {
            checkPeakInsertButton.Enabled = notNull;
        }

        void checkPeakInsertButton_Click(object sender, EventArgs e) {
            if (Graph.PointToAdd != null) {
                checkPeakPreciseEditorRowMinus.setValues(Graph.PointToAdd);
            } else {
                MessageBox.Show("Выберите сначала точку на графике спектра", "Ошибка");
            }
        }

        protected sealed override void OnLoad(EventArgs e) {
            iterationsNumericUpDown.Maximum = int.MaxValue;
            iterationsNumericUpDown.Value = Config.Iterations;
            timeLimitNumericUpDown.Maximum = int.MaxValue;
            timeLimitNumericUpDown.Value = Config.TimeLimit;
            allowedShiftNumericUpDown.Value = Config.AllowedShift;
            checkPeakNumberNumericUpDown.Maximum = Config.PEAK_NUMBER;
            checkPeakNumberNumericUpDown.Value = Config.CheckerPeakIndex;
            backroundMeasureCycleCountNumericUpDown.Value = Config.BackgroundCycles;

            var peak = Config.CustomCheckerPeak;
            if (peak != null)
                checkPeakPreciseEditorRowMinus.setValues(peak);
            // TODO: more accurate options...
            useCheckPeakCheckBox.Checked = peak != null || Config.CheckerPeakIndex != 0;
            bool enable = Graph.PointToAdd != null;
            checkPeakInsertButton.Enabled = enable;
            if (!enable) {
                Graph.OnPointAdded += Graph_OnPointAdded;
            }
            base.OnLoad(e);
        }
        protected sealed override void OnFormClosing(FormClosingEventArgs e) {
            Graph.OnPointAdded -= Graph_OnPointAdded;
            base.OnFormClosing(e);
        }
        protected override void setControls(bool enabled, bool canApply) {
            base.setControls(enabled, canApply);
            monitorOptionsGroupBox.Enabled = enabled;
        }
    }
}

