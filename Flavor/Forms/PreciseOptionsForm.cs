using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Flavor.Common;
using Flavor.Controls;

namespace Flavor.Forms {
    internal partial class PreciseOptionsForm: OptionsForm {
        private mainForm upLevel;
        internal mainForm UpLevel {
            set { upLevel = value; }
        }

        private PreciseEditorRowPlus[] PErows = new PreciseEditorRowPlus[20];
        private List<Utility.PreciseEditorData> data = new List<Utility.PreciseEditorData>();

        private static PreciseOptionsForm instance = null;
        internal static PreciseOptionsForm getInstance() {
            if (instance == null) instance = new PreciseOptionsForm();
            return instance;
        }

        internal PreciseOptionsForm()
            : base() {
            InitializeComponent();
            bool enable = Graph.PointToAdd != null;

            this.SuspendLayout();
            this.preciseEditorGroupBox.SuspendLayout();
            this.insertPointButton.Enabled = enable;

            for (int i = 0; i < 20; ++i) {
                this.PErows[i] = new PreciseEditorRowPlus();
                this.PErows[i].Location = new Point(21, 42 + 15 * i);
                this.PErows[i].PeakNumber = string.Format("{0}", i + 1);
                this.PErows[i].setClearToolTip(this.formToolTip);
                this.preciseEditorGroupBox.Controls.Add(PErows[i]);
            }

            loadPreciseEditorData(Config.PreciseData);
            this.preciseEditorGroupBox.ResumeLayout(false);
            this.preciseEditorGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            if (!enable) {
                Graph.OnPointAdded += new Graph.PointAddedDelegate(Graph_OnPointAdded);
            }
            Commander.OnProgramStateChanged += new Commander.ProgramEventHandler(InvokeEnableForm);
        }

        private void InvokeEnableForm() {
            if (this.InvokeRequired) {
                DeviceEventHandler InvokeDelegate = new DeviceEventHandler(EnableForm);
                this.Invoke(InvokeDelegate);
            } else {
                EnableForm();
            }
        }

        private void EnableForm() {
            switch (Commander.pState) {
                case Commander.programStates.Start:
                    this.preciseEditorGroupBox.Enabled = true;
                    this.params_groupBox.Enabled = true;
                    this.savePreciseEditorToFileButton.Enabled = true;
                    this.loadPreciseEditorFromFileButton.Enabled = true;
                    this.clearButton.Enabled = true;
                    this.applyButton.Enabled = false;
                    this.applyButton.Visible = false;
                    this.ok_butt.Enabled = true;
                    this.rareModeCheckBox.Enabled = true;
                    //this.cancel_butt.Enabled = true;
                    break;
                case Commander.programStates.WaitInit:
                    this.preciseEditorGroupBox.Enabled = true;
                    this.params_groupBox.Enabled = true;
                    this.savePreciseEditorToFileButton.Enabled = true;
                    this.loadPreciseEditorFromFileButton.Enabled = true;
                    this.clearButton.Enabled = true;
                    this.applyButton.Enabled = false;
                    this.applyButton.Visible = false;
                    this.ok_butt.Enabled = true;
                    this.rareModeCheckBox.Enabled = true;
                    break;
                case Commander.programStates.Init:
                    this.preciseEditorGroupBox.Enabled = true;
                    this.params_groupBox.Enabled = true;
                    this.savePreciseEditorToFileButton.Enabled = true;
                    this.loadPreciseEditorFromFileButton.Enabled = true;
                    this.clearButton.Enabled = true;
                    this.applyButton.Enabled = false;
                    this.applyButton.Visible = false;
                    this.ok_butt.Enabled = true;
                    this.rareModeCheckBox.Enabled = true;
                    break;
                case Commander.programStates.WaitHighVoltage:
                    this.preciseEditorGroupBox.Enabled = true;
                    this.params_groupBox.Enabled = true;
                    this.savePreciseEditorToFileButton.Enabled = true;
                    this.loadPreciseEditorFromFileButton.Enabled = true;
                    this.clearButton.Enabled = true;
                    this.applyButton.Enabled = true;
                    this.applyButton.Visible = true;
                    this.ok_butt.Enabled = true;
                    this.rareModeCheckBox.Enabled = true;
                    break;
                case Commander.programStates.Ready:
                    this.preciseEditorGroupBox.Enabled = true;
                    this.params_groupBox.Enabled = true;
                    this.savePreciseEditorToFileButton.Enabled = true;
                    this.loadPreciseEditorFromFileButton.Enabled = true;
                    this.clearButton.Enabled = true;
                    this.applyButton.Enabled = true;
                    this.applyButton.Visible = true;
                    this.ok_butt.Enabled = true;
                    this.rareModeCheckBox.Enabled = true;
                    break;
                case Commander.programStates.Measure:
                    this.preciseEditorGroupBox.Enabled = false;
                    this.params_groupBox.Enabled = false;
                    this.savePreciseEditorToFileButton.Enabled = false;
                    this.loadPreciseEditorFromFileButton.Enabled = false;
                    this.clearButton.Enabled = false;
                    this.applyButton.Enabled = false;
                    this.applyButton.Visible = true;
                    this.ok_butt.Enabled = false;
                    this.rareModeCheckBox.Enabled = false;
                    break;
                case Commander.programStates.WaitShutdown:
                    this.preciseEditorGroupBox.Enabled = true;
                    this.params_groupBox.Enabled = true;
                    this.savePreciseEditorToFileButton.Enabled = true;
                    this.loadPreciseEditorFromFileButton.Enabled = true;
                    this.clearButton.Enabled = true;
                    this.applyButton.Enabled = false;
                    this.applyButton.Visible = false;
                    this.ok_butt.Enabled = true;
                    this.rareModeCheckBox.Enabled = true;
                    break;
                case Commander.programStates.Shutdown:
                    this.preciseEditorGroupBox.Enabled = true;
                    this.params_groupBox.Enabled = true;
                    this.savePreciseEditorToFileButton.Enabled = true;
                    this.loadPreciseEditorFromFileButton.Enabled = true;
                    this.clearButton.Enabled = true;
                    this.applyButton.Enabled = false;
                    this.applyButton.Visible = false;
                    this.ok_butt.Enabled = true;
                    this.rareModeCheckBox.Enabled = true;
                    break;
            }
        }

        private void loadPreciseEditorData(List<Utility.PreciseEditorData> ped) {
            if (ped != null) {
                clearPreciseEditorData();
                foreach (Utility.PreciseEditorData p in ped) {
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
            data = new List<Utility.PreciseEditorData>();
            for (int i = 0; i < 20; ++i) {
                if (exitFlag &= PErows[i].checkTextBoxes())
                    if (PErows[i].AllFilled)
                        data.Add(new Utility.PreciseEditorData(PErows[i].UseChecked, (byte)i,
                                                               Convert.ToUInt16(PErows[i].StepText),
                                                               Convert.ToByte(PErows[i].ColText),
                                                               Convert.ToUInt16(PErows[i].LapsText),
                                                               Convert.ToUInt16(PErows[i].WidthText),
                                                               (float)0, PErows[i].CommentText));
            }
            return exitFlag;
        }
        protected virtual void saveData() {
            Config.SavePreciseOptions(data);
        }

        protected override void applyButton_Click(object sender, EventArgs e) {
            if (checkTextBoxes()) {
                Config.SavePreciseOptions(data);
                base.applyButton_Click(sender, e);
            }
        }

        private void savePreciseEditorToFileButton_Click(object sender, EventArgs e) {
            if (checkTextBoxes()) {
                if (savePreciseEditorToFileDialog.ShowDialog() == DialogResult.OK) {
                    Config.SavePreciseOptions(data, savePreciseEditorToFileDialog.FileName, Config.PRECISE_OPTIONS_HEADER, false, false);
                }
            }
        }

        private void loadPreciseEditorFromFileButton_Click(object sender, EventArgs e) {
            if (loadPreciseEditorFromFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    loadPreciseEditorData(Config.LoadPreciseEditorData(loadPreciseEditorFromFileDialog.FileName));
                } catch (Config.ConfigLoadException cle) {
                    cle.visualise();
                }
            }
        }

        private void clearButton_Click(object sender, EventArgs e) {
            clearPreciseEditorData();
        }

        void Graph_OnPointAdded(bool notNull) {
            insertPointButton.Enabled = notNull;
            //Graph.OnPointAdded -= new Graph.PointAddedDelegate(Graph_OnPointAdded);
        }

        private void insertPointButton_Click(object sender, EventArgs e) {
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

        private void clearPreciseEditorData() {
            for (int i = 0; i < 20; ++i)
                PErows[i].Clear();
        }
        private void PreciseOptionsForm_FormClosed(object sender, FormClosedEventArgs e) {
            instance = null;
            upLevel.InvokeRefreshButtons();
        }
    }
}