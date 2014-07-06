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
    internal partial class PreciseOptionsForm: OptionsForm2 {
        PreciseEditorRowPlus[] PErows = new PreciseEditorRowPlus[Config.PEAK_NUMBER];
        PreciseSpectrum data = new PreciseSpectrum();

        public PreciseOptionsForm()
            : base() {
            InitializeComponent();
        }

        void InvokeEnableForm(bool enabled, bool canApply) {
            Invoke(new Action(() => setControls(enabled, canApply)));
        }

        protected virtual void setControls(bool enabled, bool canApply) {
            this.preciseEditorGroupBox.Enabled = enabled;
            this.params_groupBox.Enabled = enabled;
            this.savePreciseEditorToFileButton.Enabled = enabled;
            this.loadPreciseEditorFromFileButton.Enabled = enabled;
            this.clearButton.Enabled = enabled;
            this.ok_butt.Enabled = enabled;
            this.rareModeCheckBox.Enabled = enabled;
        }

        void loadPreciseEditorData(List<PreciseEditorData> ped) {
            if (ped != null) {
                clearPreciseEditorData();
                foreach (PreciseEditorData p in ped) {
                    PErows[p.pNumber].setValues(p);
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
            for (int i = 0; i < Config.PEAK_NUMBER; ++i) {
                if (exitFlag &= PErows[i].checkTextBoxes())
                    if (PErows[i].AllFilled)
                        data.Add(new PreciseEditorData(PErows[i].UseChecked, (byte)i,
                                                               Convert.ToUInt16(PErows[i].StepText),
                                                               Convert.ToByte(PErows[i].ColText),
                                                               Convert.ToUInt16(PErows[i].LapsText),
                                                               Convert.ToUInt16(PErows[i].WidthText),
                                                               (float)0, PErows[i].CommentText));
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
                    loadPreciseEditorData(Config.loadPreciseOptions(loadPreciseEditorFromFileDialog.FileName));
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
            if (Graph.PointToAdd != null) {
                PlacePointForm ppForm = new PlacePointForm(Graph.PointToAdd);
                if (ppForm.ShowDialog() == DialogResult.OK) {
                    if (ppForm.PointNumber != -1)
                        PErows[ppForm.PointNumber].setValues(Graph.PointToAdd);
                }
            } else {
                MessageBox.Show("Выберите сначала точку на графике спектра", "Ошибка");
            }
        }

        void clearPreciseEditorData() {
            for (int i = 0; i < Config.PEAK_NUMBER; ++i)
                PErows[i].Clear();
        }

        protected override void OnLoad(EventArgs e) {
            var args = e is LoadEventArgs ? e as LoadEventArgs : new LoadEventArgs();
            args.Method += InvokeEnableForm;
            base.OnLoad(args);
            //Commander.ProgramStateChanged += InvokeEnableForm;

            bool enable = Graph.PointToAdd != null;

            this.SuspendLayout();
            this.preciseEditorGroupBox.SuspendLayout();
            this.insertPointButton.Enabled = enable;

            for (int i = 0; i < Config.PEAK_NUMBER; ++i) {
                this.PErows[i] = new PreciseEditorRowPlus();
                this.PErows[i].Location = new Point(21, 42 + 15 * i);
                this.PErows[i].PeakNumber = string.Format("{0}", i + 1);
                this.PErows[i].setClearToolTip(this.formToolTip);
                this.preciseEditorGroupBox.Controls.Add(PErows[i]);
            }
            // TODO: set form dimensions accordingly to number of peak lines..
            loadPreciseEditorData(Config.PreciseData);
            this.preciseEditorGroupBox.ResumeLayout(false);
            this.preciseEditorGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            if (!enable) {
                Graph.OnPointAdded += Graph_OnPointAdded;
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e) {
            var args = e is ClosingEventArgs ? e as ClosingEventArgs : new ClosingEventArgs(e);
            args.Method += InvokeEnableForm;
            Graph.OnPointAdded -= Graph_OnPointAdded;
            base.OnFormClosing(args);
        }
    }
}