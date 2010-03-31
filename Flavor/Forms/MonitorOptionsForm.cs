using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Flavor.Common;
using Flavor.Controls;

namespace Flavor.Forms
{
    internal partial class MonitorOptionsForm: PreciseOptionsForm
    {
        private MonitorOptionsForm()
        {
            InitializeComponent();
            iterationsNumericUpDown.Maximum = new decimal(new int[] {int.MaxValue, 0, 0, 0});
            iterationsNumericUpDown.Value = (decimal)Config.Iterations;
            bool enable = Graph.PointToAdd != null;
            this.checkPeakInsertButton.Enabled = enable;
            if (!enable)
            {
                Graph.OnPointAdded += new Graph.PointAddedDelegate(Graph_OnPointAdded);
            }
        }

        private static MonitorOptionsForm instance = null;
        internal new static MonitorOptionsForm getInstance()
        {
            if (instance == null) instance = new MonitorOptionsForm();
            return instance;
        }

        protected override bool checkTextBoxes()
        {
            return checkPeakPreciseEditorRowMinus.checkTextBoxes() & base.checkTextBoxes();
        }
        protected override void saveData()
        {
            base.saveData();
            Config.saveCheckOptions((int)iterationsNumericUpDown.Value,
                                    new Utility.PreciseEditorData(false, 255, Convert.ToUInt16(checkPeakPreciseEditorRowMinus.StepText),
                                                                  Convert.ToByte(checkPeakPreciseEditorRowMinus.ColText), 0,
                                                                  Convert.ToUInt16(checkPeakPreciseEditorRowMinus.WidthText), 0, "checker peak"));
        }

        private void MonitorOptionsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            instance = null;
        }

        void Graph_OnPointAdded(bool notNull)
        {
            checkPeakInsertButton.Enabled = notNull;
            //Graph.OnPointAdded -= new Graph.PointAddedDelegate(Graph_OnPointAdded);
        }

        private void checkPeakInsertButton_Click(object sender, EventArgs e)
        {
            if (Graph.PointToAdd != null)
            {
                checkPeakPreciseEditorRowMinus.setValues(Graph.PointToAdd);
            }
            else
            {
                MessageBox.Show("Выберите сначала точку на графике спектра", "Ошибка");
            }
        }
    }
}

