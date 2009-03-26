using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Flavor
{
    public class PreciseOptionsForm : OptionsForm
    {
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label colNumLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label commentLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button savePreciseEditorToFileButton;
        private System.Windows.Forms.SaveFileDialog savePreciseEditorToFileDialog;
        private System.Windows.Forms.Button loadPreciseEditorFromFileButton;
        private System.Windows.Forms.OpenFileDialog loadPreciseEditorFromFileDialog;
        private System.Windows.Forms.Button clearButton;
        
        private mainForm upLevel;
        public mainForm UpLevel
        {
            //get { return upLevel; }
            set { upLevel = value; }
        }

        private Utility.PreciseEditorRowPlus[] PErows = new Utility.PreciseEditorRowPlus[20];
        private List<Utility.PreciseEditorData> data = new List<Utility.PreciseEditorData>();

        private static PreciseOptionsForm instance = null;
        public static PreciseOptionsForm getInstance(){
            if (instance == null) instance = new PreciseOptionsForm();
            return instance;
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.colNumLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.commentLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.loadPreciseEditorFromFileButton = new System.Windows.Forms.Button();
            this.savePreciseEditorToFileButton = new System.Windows.Forms.Button();
            this.savePreciseEditorToFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.loadPreciseEditorFromFileDialog = new System.Windows.Forms.OpenFileDialog();

            this.groupBox1.SuspendLayout();
            this.params_groupBox.SuspendLayout();
            this.SuspendLayout();
            // label1
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 26);
            this.label1.Text = "Номер\r\nпика";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // label8
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(55, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 26);
            this.label8.Text = "Ступенька\r\n(<=1056)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // colNumLabel
            this.colNumLabel.AutoSize = true;
            this.colNumLabel.Location = new System.Drawing.Point(105, 29);
            this.colNumLabel.Name = "colNumLabel";
            this.colNumLabel.Size = new System.Drawing.Size(29, 13);
            this.colNumLabel.Text = "Кол.";
            // label9
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.SystemColors.Control;
            this.label9.Location = new System.Drawing.Point(130, 29);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.Text = "Проходы";
            // label10
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.SystemColors.Control;
            this.label10.Location = new System.Drawing.Point(182, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.Text = "Ширина";
            // label11
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.SystemColors.Control;
            this.label11.Location = new System.Drawing.Point(232, 29);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(54, 13);
            this.label11.Text = "Точность";
            // commentLabel
            this.commentLabel.AutoSize = true;
            this.commentLabel.BackColor = System.Drawing.SystemColors.Control;
            this.commentLabel.Location = new System.Drawing.Point(288, 29);
            this.commentLabel.Name = "commentLabel";
            this.commentLabel.Size = new System.Drawing.Size(54, 13);
            this.commentLabel.Text = "Комментарий";
            // groupBox1
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.colNumLabel);
            this.groupBox1.Controls.Add(this.clearButton);
            this.groupBox1.Controls.Add(this.loadPreciseEditorFromFileButton);
            this.groupBox1.Controls.Add(this.savePreciseEditorToFileButton);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.commentLabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312 + 2 + 100, 385);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Редактор областей сканирования";
            // clearButton
            this.clearButton.Location = new System.Drawing.Point(244, 356);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(62, 23);
            this.clearButton.TabIndex = 12;
            this.clearButton.Text = "Очистить";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // loadPreciseEditorFromFileButton
            this.loadPreciseEditorFromFileButton.Location = new System.Drawing.Point(118, 356);
            this.loadPreciseEditorFromFileButton.Name = "loadPreciseEditorFromFileButton";
            this.loadPreciseEditorFromFileButton.Size = new System.Drawing.Size(120, 23);
            this.loadPreciseEditorFromFileButton.TabIndex = 11;
            this.loadPreciseEditorFromFileButton.Text = "Загрузить из файла";
            this.loadPreciseEditorFromFileButton.UseVisualStyleBackColor = true;
            this.loadPreciseEditorFromFileButton.Click += new System.EventHandler(this.loadPreciseEditorFromFileButton_Click);
            // savePreciseEditorToFileButton
            this.savePreciseEditorToFileButton.Location = new System.Drawing.Point(6, 356);
            this.savePreciseEditorToFileButton.Name = "savePreciseEditorToFileButton";
            this.savePreciseEditorToFileButton.Size = new System.Drawing.Size(106, 23);
            this.savePreciseEditorToFileButton.TabIndex = 10;
            this.savePreciseEditorToFileButton.Text = "Сохранить в файл";
            this.savePreciseEditorToFileButton.UseVisualStyleBackColor = true;
            this.savePreciseEditorToFileButton.Click += new System.EventHandler(this.savePreciseEditorToFileButton_Click);
            // params_groupBox
            this.params_groupBox.Location = new System.Drawing.Point(this.params_groupBox.Location.X + 320 + 2 + 100, this.params_groupBox.Location.Y - 73);
            // rareModeCheckBox
            this.rareModeCheckBox.Location = new System.Drawing.Point(this.rareModeCheckBox.Location.X + 320 + 2 + 100, this.rareModeCheckBox.Location.Y - 73);
            // applyButton
            this.applyButton.Location = new System.Drawing.Point(330 + 2 + 100, 374);
            // cancel_butt
            this.cancel_butt.Location = new System.Drawing.Point(540 + 2 + 100, 374);
            // ok_butt
            this.ok_butt.Location = new System.Drawing.Point(408 + 2 + 100, 374);
            // savePreciseEditorToFileDialog
            this.savePreciseEditorToFileDialog.DefaultExt = "ped";
            this.savePreciseEditorToFileDialog.Filter = "Precise Editor Data Files (*.ped)|*.ped";
            // loadPreciseEditorFromFileDialog
            this.loadPreciseEditorFromFileDialog.DefaultExt = "ped";
            this.loadPreciseEditorFromFileDialog.Filter = "Precise editor data files (*.ped)|*.ped|Precise specter files (*.psf)|*.psf";
            // PreciseOptionsForm
            this.ClientSize = new System.Drawing.Size(609 + 2 + 100, 409);
            this.Controls.Add(this.groupBox1);
            this.MinimizeBox = true;
            this.Name = "PreciseOptionsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки точного режима";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PreciseOptionsForm_FormClosed);
            this.params_groupBox.ResumeLayout(false);
            this.params_groupBox.PerformLayout();

            for (int i = 0; i < 20; ++i)
            {
                this.PErows[i] = new Utility.PreciseEditorRowPlus(21, 42 + 15 * i);
                this.PErows[i].PeakNumber = string.Format("{0}", i + 1);
                this.groupBox1.Controls.AddRange(PErows[i].getControls());
            }
            
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private PreciseOptionsForm(): base()
        {
            InitializeComponent();
            this.SuspendLayout();
            this.groupBox1.SuspendLayout();
            loadPreciseEditorData(Config.PreciseData);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
            Commander.OnProgramStateChanged += new Commander.ProgramEventHandler(InvokeEnableForm);
        }

        private void InvokeEnableForm()
        {
            if (this.InvokeRequired)
            {
                DeviceEventHandler InvokeDelegate = new DeviceEventHandler(EnableForm);
                this.Invoke(InvokeDelegate);
            }
            else
            {
                EnableForm();
            }
        }

        private void EnableForm()
        {
            switch (Commander.pState)
            {
                case Commander.programStates.Start:
                    this.groupBox1.Enabled = true;
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
                    this.groupBox1.Enabled = true;
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
                    this.groupBox1.Enabled = true;
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
                    this.groupBox1.Enabled = true;
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
                    this.groupBox1.Enabled = true;
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
                    this.groupBox1.Enabled = false;
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
                    this.groupBox1.Enabled = true;
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
                    this.groupBox1.Enabled = true;
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

        private void loadPreciseEditorData(List<Utility.PreciseEditorData> ped)
        {
            if (ped != null)
            {
                clearPreciseEditorData();
                foreach (Utility.PreciseEditorData p in ped)
                {
                    PErows[p.pNumber].UseChecked = p.Use;
                    PErows[p.pNumber].StepText = p.Step.ToString();
                    PErows[p.pNumber].ColText = p.Collector.ToString();
                    PErows[p.pNumber].LapsText = p.Iterations.ToString();
                    PErows[p.pNumber].WidthText = p.Width.ToString();
                    PErows[p.pNumber].PrecText = p.Precision.ToString();
                    PErows[p.pNumber].CommentText = p.Comment;
                }
            }
        }

        protected override void ok_butt_Click(object sender, EventArgs e)
        {
            if (checkTextBoxes())
            {
                Config.SavePreciseOptions(data);
                base.ok_butt_Click(sender, e);
            }
        }

        private bool checkTextBoxes()
        {
            bool exitFlag = true;
            data = new List<Utility.PreciseEditorData>();
            for (int i = 0; i < 20; ++i)
            {
                if (exitFlag &= PErows[i].checkTextBoxes())
                    if (PErows[i].AllFilled == true)
                        data.Add(new Utility.PreciseEditorData(PErows[i].UseChecked, (byte)i,
                                                               Convert.ToUInt16(PErows[i].StepText),
                                                               Convert.ToByte(PErows[i].ColText),
                                                               Convert.ToUInt16(PErows[i].LapsText),
                                                               Convert.ToUInt16(PErows[i].WidthText),
                                                               (float)0, PErows[i].CommentText));
            }
            return exitFlag;
        }
        
        protected override void applyButton_Click(object sender, EventArgs e)
        {
            if (checkTextBoxes())
            {
                Config.SavePreciseOptions(data);
                base.applyButton_Click(sender, e);
            }
        }

        private void savePreciseEditorToFileButton_Click(object sender, EventArgs e)
        {
            if (checkTextBoxes())
            {
                if (savePreciseEditorToFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Config.SavePreciseOptions(data, savePreciseEditorToFileDialog.FileName);
                }
            }
        }

        private void loadPreciseEditorFromFileButton_Click(object sender, EventArgs e)
        {
            if (loadPreciseEditorFromFileDialog.ShowDialog() == DialogResult.OK)
            {
                loadPreciseEditorData(Config.LoadPreciseEditorData(loadPreciseEditorFromFileDialog.FileName));
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            clearPreciseEditorData();
        }

        private void clearPreciseEditorData()
        {
            for (int i = 0; i < 20; ++i)
                PErows[i].Clear();
        }
        private void PreciseOptionsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            instance = null;
            upLevel.InvokeRefreshButtons();
        }
    }
}