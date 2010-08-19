using System;
using System.Windows.Forms;
using Flavor.Common;
using Flavor.Controls;

namespace Flavor.Forms {
    internal partial class MonitorOptionsForm: PreciseOptionsForm {
        private MonitorOptionsForm() {
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
            bool enable = Graph.Instance.PointToAdd != null;
            this.checkPeakInsertButton.Enabled = enable;
            if (!enable) {
                Graph.Instance.OnPointAdded += new Graph.PointAddedDelegate(Graph_OnPointAdded);
            }
        }

        private static MonitorOptionsForm instance = null;
        internal new static MonitorOptionsForm getInstance() {
            if (instance == null) instance = new MonitorOptionsForm();
            return instance;
        }

        protected override bool checkTextBoxes() {
            return checkPeakPreciseEditorRowMinus.checkTextBoxes() & base.checkTextBoxes();
        }
        protected override void saveData() {
            base.saveData();
            Config.saveCheckOptions((int)iterationsNumericUpDown.Value, (int)timeLimitNumericUpDown.Value, (ushort)allowedShiftNumericUpDown.Value,
                                    checkPeakPreciseEditorRowMinus.AllFilled?
                                    new Utility.PreciseEditorData(false, 255, Convert.ToUInt16(checkPeakPreciseEditorRowMinus.StepText),
                                                                  Convert.ToByte(checkPeakPreciseEditorRowMinus.ColText), 0,
                                                                  Convert.ToUInt16(checkPeakPreciseEditorRowMinus.WidthText), 0, "checker peak"):
                                    null);
        }

        private void MonitorOptionsForm_FormClosed(object sender, FormClosedEventArgs e) {
            instance = null;
        }

        void Graph_OnPointAdded(bool notNull) {
            checkPeakInsertButton.Enabled = notNull;
            //Graph.Instance.OnPointAdded -= new Graph.PointAddedDelegate(Graph_OnPointAdded);
        }

        private void checkPeakInsertButton_Click(object sender, EventArgs e) {
            if (Graph.Instance.PointToAdd != null) {
                checkPeakPreciseEditorRowMinus.setValues(Graph.Instance.PointToAdd);
            } else {
                MessageBox.Show("Выберите сначала точку на графике спектра", "Ошибка");
            }
        }
    }
}

