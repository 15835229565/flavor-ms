using System;
using System.Windows.Forms;
using Flavor.Common;
using Flavor.Controls;

namespace Flavor.Forms {
    internal partial class MonitorOptionsForm: PreciseOptionsForm {
        public MonitorOptionsForm() {
            InitializeComponent();
        }

        protected override bool checkTextBoxes() {
            return checkPeakPreciseEditorRowMinus.checkTextBoxes() & base.checkTextBoxes();
        }
        protected override void saveData() {
            base.saveData();
            Config.saveGlobalCheckOptions((int)iterationsNumericUpDown.Value, (int)timeLimitNumericUpDown.Value, (ushort)allowedShiftNumericUpDown.Value,
                                    checkPeakPreciseEditorRowMinus.AllFilled?
                                    new Utility.PreciseEditorData(false, 255, Convert.ToUInt16(checkPeakPreciseEditorRowMinus.StepText),
                                                                  Convert.ToByte(checkPeakPreciseEditorRowMinus.ColText), 0,
                                                                  Convert.ToUInt16(checkPeakPreciseEditorRowMinus.WidthText), 0, "checker peak"):
                                    null, (int)checkPeakNumberNumericUpDown.Value);
        }

        private void Graph_OnPointAdded(bool notNull) {
            checkPeakInsertButton.Enabled = notNull;
            //Graph.OnPointAdded -= Graph_OnPointAdded;
        }

        private void checkPeakInsertButton_Click(object sender, EventArgs e) {
            if (Graph.PointToAdd != null) {
                checkPeakPreciseEditorRowMinus.setValues(Graph.PointToAdd);
            } else {
                MessageBox.Show("Выберите сначала точку на графике спектра", "Ошибка");
            }
        }

        protected sealed override void OnLoad(EventArgs e) {
            iterationsNumericUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            iterationsNumericUpDown.Value = (decimal)Config.Iterations;
            timeLimitNumericUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            timeLimitNumericUpDown.Value = (decimal)Config.TimeLimit;
            allowedShiftNumericUpDown.Value = (decimal)Config.AllowedShift;
            checkPeakNumberNumericUpDown.Maximum = (decimal)Config.PEAK_NUMBER;
            checkPeakNumberNumericUpDown.Value = (decimal)Config.CheckerPeakIndex;
            Utility.PreciseEditorData peak = Config.CustomCheckerPeak;
            if (peak != null)
                checkPeakPreciseEditorRowMinus.setValues(peak);
            // TODO: more accurate options...
            useCheckPeakCheckBox.Checked = peak != null && Config.PEAK_NUMBER != 0;
            bool enable = Graph.PointToAdd != null;
            checkPeakInsertButton.Enabled = enable;
            if (!enable) {
                Graph.OnPointAdded += new Graph.PointAddedDelegate(Graph_OnPointAdded);
            }
            base.OnLoad(e);
        }
        protected sealed override void OnFormClosing(FormClosingEventArgs e) {
            Graph.OnPointAdded -= Graph_OnPointAdded;
            base.OnFormClosing(e);
        }
    }
}

