using System;
using System.Windows.Forms;
using Flavor.Common;
using Flavor.Controls;

namespace Flavor.Forms {
    internal partial class MonitorOptionsForm: PreciseOptionsForm {
        public MonitorOptionsForm() {
            InitializeComponent();
            iterationsNumericUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            iterationsNumericUpDown.Value = (decimal)Config.Iterations;
            timeLimitNumericUpDown.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            timeLimitNumericUpDown.Value = (decimal)Config.TimeLimit;
            allowedShiftNumericUpDown.Value = (decimal)Config.AllowedShift;
            Utility.PreciseEditorData peak = Config.CheckerPeak;
            if (peak != null) {
                checkPeakPreciseEditorRowMinus.setValues(peak);
            }
            bool enable = Graph.PointToAdd != null;
            this.checkPeakInsertButton.Enabled = enable;
            if (!enable) {
                Graph.OnPointAdded += new Graph.PointAddedDelegate(Graph_OnPointAdded);
            }
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
                                    null);
        }

        private void Graph_OnPointAdded(bool notNull) {
            checkPeakInsertButton.Enabled = notNull;
            //Graph.OnPointAdded -= new Graph.PointAddedDelegate(Graph_OnPointAdded);
        }

        private void checkPeakInsertButton_Click(object sender, EventArgs e) {
            if (Graph.PointToAdd != null) {
                checkPeakPreciseEditorRowMinus.setValues(Graph.PointToAdd);
            } else {
                MessageBox.Show("Выберите сначала точку на графике спектра", "Ошибка");
            }
        }

        protected sealed override void OnFormClosing(FormClosingEventArgs e) {
            Graph.OnPointAdded -= Graph_OnPointAdded;
            base.OnFormClosing(e);
        }
    }
}

