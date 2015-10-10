using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Flavor.Controls;
using Config = Flavor.Common.Settings.Config;

using PreciseSpectrum = Flavor.Common.Data.Measure.PreciseSpectrum;
using PreciseEditorData = Flavor.Common.Data.Measure.PreciseEditorData;
// remove this reference from here..
using Graph = Flavor.Common.Data.Measure.Graph;

namespace Flavor.Forms {
    partial class PreciseOptionsForm: OptionsForm2 {
        readonly PreciseEditorRowPlus[] _rows = new PreciseEditorRowPlus[Config.PEAK_NUMBER];
        PreciseSpectrum data;

        public PreciseOptionsForm()
            : base() {
            InitializeComponent();
            SuspendLayout();
            preciseEditorGroupBox.SuspendLayout();

            int max = Config.COLLECTOR_COEFFS.Length;
            for (int i = 0; i < _rows.Length; ++i) {
                _rows[i] = new PreciseEditorRowPlus {
                    MaxNumber = max,
                    Location = new Point(21, 42 + 15 * i),
                    PeakNumber = (i + 1).ToString(),
                    ToolTip = formToolTip
                };
                preciseEditorGroupBox.Controls.Add(_rows[i]);
            }
            // TODO: set form dimensions accordingly to number of peak lines..
            preciseEditorGroupBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        void InvokeEnableForm(bool enabled, bool canApply) {
            Invoke(new Action(() => setControls(enabled, canApply)));
        }

        protected virtual void setControls(bool enabled, bool canApply) {
            preciseEditorGroupBox.Enabled = enabled;
            savePreciseEditorToFileButton.Enabled = enabled;
            loadPreciseEditorFromFileButton.Enabled = enabled;
            clearButton.Enabled = enabled;
        }

        void loadPreciseEditorData(List<PreciseEditorData> ped) {
            if (ped != null) {
                foreach (var p in ped) {
                    _rows[p.pNumber].setValues(p);
                }
            }
        }

        protected override void ok_butt_Click(object sender, EventArgs e) {
            if (!checkTextBoxes()) {
                return;
            }
            saveData();
            base.ok_butt_Click(sender, e);
        }
        protected virtual bool checkTextBoxes() {
            bool exitFlag = true;
            data = new PreciseSpectrum();
            byte n = 0;
            foreach (var row in _rows) {
                if (exitFlag &= row.checkTextBoxes(Config.MIN_STEP, Config.MAX_STEP))
                    if (row.AllFilled)
                        data.Add(new PreciseEditorData(row.UseChecked, n,
                            Convert.ToUInt16(row.StepText),
                            Convert.ToByte(row.ColText),
                            Convert.ToUInt16(row.LapsText),
                            Convert.ToUInt16(row.WidthText),
                            0f, row.CommentText));
                ++n;
            }
            return exitFlag;
        }
        protected virtual void saveData() {
            Config.saveGlobalPreciseOptions(data);
        }

        protected override void applyButton_Click(object sender, EventArgs e) {
            if (checkTextBoxes()) {
                Config.saveGlobalPreciseOptions(data);
                base.applyButton_Click(sender, e);
            }
        }

        void savePreciseEditorToFileButton_Click(object sender, EventArgs e) {
            if (checkTextBoxes()) {
                if (savePreciseEditorToFileDialog.ShowDialog() == DialogResult.OK) {
                    Config.savePreciseOptions(data, savePreciseEditorToFileDialog.FileName, false);
                }
            }
        }

        void loadPreciseEditorFromFileButton_Click(object sender, EventArgs e) {
            if (loadPreciseEditorFromFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    var data = Config.loadPreciseOptions(loadPreciseEditorFromFileDialog.FileName);
                    clearPreciseEditorData();
                    loadPreciseEditorData(data);
                } catch (Config.ConfigLoadException cle) {
                    cle.visualise();
                }
            }
        }

        void clearButton_Click(object sender, EventArgs e) {
            clearPreciseEditorData();
        }

        void Graph_OnPointAdded(bool notNull) {
            insertPointButton.Enabled = notNull;
        }

        void insertPointButton_Click(object sender, EventArgs e) {
            var cachedPoint = Graph.PointToAdd;
            if (cachedPoint != null) {
                var form = new PlacePointForm(cachedPoint, Config.PEAK_NUMBER);
                if (form.ShowDialog() == DialogResult.OK) {
                    int i = form.PointNumber;
                    if (i != -1)
                        _rows[i].setValues(form.PointToAdd);
                }
            } else {
                MessageBox.Show("Выберите сначала точку на графике спектра", "Ошибка");
            }
        }

        void clearPreciseEditorData() {
            foreach (var row in _rows) {
                row.Clear();
            }
        }

        protected override void OnLoad(EventArgs e) {
            var args = e.As<LoadEventArgs>();
            args.Method += InvokeEnableForm;
            base.OnLoad(args);

            // TODO: transmit Config and Graph data in LoadEventArgs
            bool enable = Graph.PointToAdd != null;
            insertPointButton.Enabled = enable;
            loadPreciseEditorData(Config.PreciseData);
            if (!enable) {
                Graph.OnPointAdded += Graph_OnPointAdded;
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e) {
            var args = e is ClosingEventArgs ? (ClosingEventArgs)e : new ClosingEventArgs(e);
            args.Method += InvokeEnableForm;
            Graph.OnPointAdded -= Graph_OnPointAdded;
            base.OnFormClosing(args);
        }
    }
}