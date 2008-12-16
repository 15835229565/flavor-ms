using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Flavor
{
    public partial class PreciseOptionsForm : Form
    {
        private mainForm upLevel;
        public mainForm UpLevel
        {
            //get { return upLevel; }
            set { upLevel = value; }
        }
        public TextBox[] stepTextBoxes = new TextBox[20];
        public TextBox[] colTextBoxes = new TextBox[20];
        public TextBox[] lapsTextBoxes = new TextBox[20];
        public TextBox[] widthTextBoxes = new TextBox[20];
        public TextBox[] precTextBoxes = new TextBox[20];
        private List<PreciseEditorData> data = new List<PreciseEditorData>();

        private static PreciseOptionsForm instance = null;
        
        public static PreciseOptionsForm getInstance(){
            if (instance == null) instance = new PreciseOptionsForm();
            return instance;
        }
        
        private PreciseOptionsForm()
        {
            InitializeComponent();
            this.SuspendLayout();
            this.groupBox1.SuspendLayout();
            for (int i = 0; i < 20; ++i) 
            {
                this.stepTextBoxes[i] = new TextBox();
                this.stepTextBoxes[i].BackColor = System.Drawing.SystemColors.ControlDark;
                this.stepTextBoxes[i].BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.stepTextBoxes[i].Location = new System.Drawing.Point(58, 42 + 15 * i);
                this.stepTextBoxes[i].Margin = new System.Windows.Forms.Padding(1);
                this.stepTextBoxes[i].MaxLength = 4;
                this.stepTextBoxes[i].Size = new System.Drawing.Size(50, 13);
                this.stepTextBoxes[i].TabIndex = 26 + i;
                this.stepTextBoxes[i].TextChanged += new System.EventHandler(this.integralTextbox_TextChanged);

                this.colTextBoxes[i] = new TextBox();
                this.colTextBoxes[i].BackColor = System.Drawing.SystemColors.ControlDark;
                this.colTextBoxes[i].BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.colTextBoxes[i].Location = new System.Drawing.Point(110, 42 + 15 * i);
                this.colTextBoxes[i].Margin = new System.Windows.Forms.Padding(1);
                this.colTextBoxes[i].MaxLength = 1;
                this.colTextBoxes[i].Size = new System.Drawing.Size(20, 13);
                this.colTextBoxes[i].TabIndex = 46 + i;
                this.colTextBoxes[i].TextChanged += new System.EventHandler(this.oneDigitTextbox_TextChanged);

                this.lapsTextBoxes[i] = new TextBox();
                this.lapsTextBoxes[i].BackColor = System.Drawing.SystemColors.ControlDark;
                this.lapsTextBoxes[i].BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.lapsTextBoxes[i].Location = new System.Drawing.Point(132, 42 + 15 * i);
                this.lapsTextBoxes[i].Margin = new System.Windows.Forms.Padding(1);
                this.lapsTextBoxes[i].Size = new System.Drawing.Size(50, 13);
                this.lapsTextBoxes[i].TabIndex = 66 + i;
                this.lapsTextBoxes[i].TextChanged += new System.EventHandler(this.integralTextbox_TextChanged);

                this.widthTextBoxes[i] = new TextBox();
                this.widthTextBoxes[i].BackColor = System.Drawing.SystemColors.ControlDark;
                this.widthTextBoxes[i].BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.widthTextBoxes[i].Location = new System.Drawing.Point(184, 42 + 15 * i);
                this.widthTextBoxes[i].Margin = new System.Windows.Forms.Padding(1);
                this.widthTextBoxes[i].MaxLength = 4;
                this.widthTextBoxes[i].Size = new System.Drawing.Size(50, 13);
                this.widthTextBoxes[i].TabIndex = 86 + i;
                this.widthTextBoxes[i].TextChanged += new System.EventHandler(this.integralTextbox_TextChanged);

                this.precTextBoxes[i] = new TextBox();
                this.precTextBoxes[i].BackColor = System.Drawing.SystemColors.ControlDark;
                this.precTextBoxes[i].BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.precTextBoxes[i].Location = new System.Drawing.Point(236, 42 + 15 * i);
                this.precTextBoxes[i].Margin = new System.Windows.Forms.Padding(1);
                this.precTextBoxes[i].Size = new System.Drawing.Size(50, 13);
                this.precTextBoxes[i].TabIndex = 106 + i;
                this.precTextBoxes[i].TextChanged += new System.EventHandler(this.positiveNumericTextbox_TextChanged);

                this.groupBox1.Controls.Add(stepTextBoxes[i]);
                this.groupBox1.Controls.Add(colTextBoxes[i]);
                this.groupBox1.Controls.Add(lapsTextBoxes[i]);
                this.groupBox1.Controls.Add(widthTextBoxes[i]);
                this.groupBox1.Controls.Add(precTextBoxes[i]);
            }
            loadPreciseEditorData(Config.PreciseData);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            rareModeCheckBox.Checked = Commander.notRareModeRequested;
            if ((Commander.pState == Commander.programStates.Ready) || (Commander.pState == Commander.programStates.WaitHighVoltage))
            {
                applyButton.Enabled = true;
                applyButton.Visible = true;
            }
            else
            {
                applyButton.Enabled = false;
                applyButton.Visible = false;
            }

            loadCommonData();
            Commander.OnProgramStateChanged += new ProgramEventHandler(InvokeEnableForm);
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
                    this.groupBox2.Enabled = true;
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
                    this.groupBox2.Enabled = true;
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
                    this.groupBox2.Enabled = true;
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
                    this.groupBox2.Enabled = true;
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
                    this.groupBox2.Enabled = true;
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
                    this.groupBox2.Enabled = false;
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
                    this.groupBox2.Enabled = true;
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
                    this.groupBox2.Enabled = true;
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

        private void loadPreciseEditorData(List<PreciseEditorData> ped)
        {
            if (ped != null)
            {
                clearPreciseEditorData();
                foreach (PreciseEditorData p in ped)
                {
                    stepTextBoxes[p.pNumber].Text = p.Step.ToString();
                    colTextBoxes[p.pNumber].Text = p.Collector.ToString();
                    lapsTextBoxes[p.pNumber].Text = p.Iterations.ToString();
                    widthTextBoxes[p.pNumber].Text = p.Width.ToString();
                    precTextBoxes[p.pNumber].Text = p.Precision.ToString();
                }
            }
        }

        private void loadCommonData(string fn)
        {
            Config.loadCommonOptions(fn);
            loadCommonData();
        }
        
        private void loadCommonData()
        {
            decimal temp;

            temp = (decimal)(Config.eTimeReal);
            if (temp < expTimeNumericUpDown.Minimum) temp = expTimeNumericUpDown.Minimum;
            if (temp > expTimeNumericUpDown.Maximum) temp = expTimeNumericUpDown.Maximum;
            expTimeNumericUpDown.Value = temp;

            temp = (decimal)(Config.iTimeReal);
            if (temp < idleTimeNumericUpDown.Minimum) temp = idleTimeNumericUpDown.Minimum;
            if (temp > idleTimeNumericUpDown.Maximum) temp = idleTimeNumericUpDown.Maximum;
            idleTimeNumericUpDown.Value = temp;

            iVoltageNumericUpDown.Minimum = (decimal)(Config.iVoltageConvert(Config.iVoltageConvert((double)20)));
            iVoltageNumericUpDown.Maximum = (decimal)(Config.iVoltageConvert(Config.iVoltageConvert((double)150)));
            temp = (decimal)(Config.iVoltageReal);
            if (temp < iVoltageNumericUpDown.Minimum) temp = iVoltageNumericUpDown.Minimum;
            if (temp > iVoltageNumericUpDown.Maximum) temp = iVoltageNumericUpDown.Maximum;
            iVoltageNumericUpDown.Value = temp;

            CPNumericUpDown.Minimum = (decimal)(Config.CPConvert(Config.CPConvert((double)10)));
            CPNumericUpDown.Maximum = (decimal)(Config.CPConvert(Config.CPConvert((double)12)));
            temp = (decimal)(Config.CPReal);
            if (temp < CPNumericUpDown.Minimum) temp = CPNumericUpDown.Minimum;
            if (temp > CPNumericUpDown.Maximum) temp = CPNumericUpDown.Maximum;
            CPNumericUpDown.Value = temp;

            eCurrentNumericUpDown.Minimum = (decimal)(Config.eCurrentConvert(Config.eCurrentConvert((double)0)));
            eCurrentNumericUpDown.Maximum = (decimal)(Config.eCurrentConvert(Config.eCurrentConvert((double)50)));
            temp = (decimal)(Config.eCurrentReal);
            if (temp < eCurrentNumericUpDown.Minimum) temp = eCurrentNumericUpDown.Minimum;
            if (temp > eCurrentNumericUpDown.Maximum) temp = eCurrentNumericUpDown.Maximum;
            eCurrentNumericUpDown.Value = temp;

            hCurrentNumericUpDown.Minimum = (decimal)(Config.hCurrentConvert(Config.hCurrentConvert((double)0)));
            hCurrentNumericUpDown.Maximum = (decimal)(Config.hCurrentConvert(Config.hCurrentConvert((double)1)));
            temp = (decimal)(Config.hCurrentReal);
            if (temp < hCurrentNumericUpDown.Minimum) temp = hCurrentNumericUpDown.Minimum;
            if (temp > hCurrentNumericUpDown.Maximum) temp = hCurrentNumericUpDown.Maximum;
            hCurrentNumericUpDown.Value = temp;

            fV1NumericUpDown.Minimum = (decimal)(Config.fV1Convert(Config.fV1Convert((double)20)));
            fV1NumericUpDown.Maximum = (decimal)(Config.fV1Convert(Config.fV1Convert((double)150)));
            temp = (decimal)(Config.fV1Real);
            if (temp < fV1NumericUpDown.Minimum) temp = fV1NumericUpDown.Minimum;
            if (temp > fV1NumericUpDown.Maximum) temp = fV1NumericUpDown.Maximum;
            fV1NumericUpDown.Value = temp;

            fV2NumericUpDown.Minimum = (decimal)(Config.fV2Convert(Config.fV2Convert((double)20)));
            fV2NumericUpDown.Maximum = (decimal)(Config.fV2Convert(Config.fV2Convert((double)150)));
            temp = (decimal)(Config.fV2Real);
            if (temp < fV2NumericUpDown.Minimum) temp = fV2NumericUpDown.Minimum;
            if (temp > fV2NumericUpDown.Maximum) temp = fV2NumericUpDown.Maximum;
            fV2NumericUpDown.Value = temp;
        }

        private void cancel_butt_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ok_butt_Click(object sender, EventArgs e)
        {
            if (checkTextBoxes())
            {
                Config.SavePreciseOptions(data, (ushort)(expTimeNumericUpDown.Value), (ushort)(idleTimeNumericUpDown.Value),
                       (double)(iVoltageNumericUpDown.Value), (double)(CPNumericUpDown.Value), (double)(eCurrentNumericUpDown.Value), (double)(hCurrentNumericUpDown.Value), (double)(fV1NumericUpDown.Value), (double)(fV2NumericUpDown.Value));
                Commander.notRareModeRequested = rareModeCheckBox.Checked;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool checkTextBoxes()
        {
            bool exitFlag = true;
            data = new List<PreciseEditorData>();
            for (int i = 0; i < 20; ++i)
            {
                bool somethingFilled = ((lapsTextBoxes[i].Text != "") || (stepTextBoxes[i].Text != "") || (colTextBoxes[i].Text != "") || (widthTextBoxes[i].Text != "") /*|| (precTextBoxes[i].Text != "")*/);
                bool allFilled = ((lapsTextBoxes[i].Text != "") && (stepTextBoxes[i].Text != "") && (colTextBoxes[i].Text != "") && (widthTextBoxes[i].Text != "")/* && (precTextBoxes[i].Text != "")*/);
                stepTextBoxes[i].BackColor = System.Drawing.SystemColors.ControlDark;
                colTextBoxes[i].BackColor = System.Drawing.SystemColors.ControlDark;
                lapsTextBoxes[i].BackColor = System.Drawing.SystemColors.ControlDark;
                widthTextBoxes[i].BackColor = System.Drawing.SystemColors.ControlDark;
                precTextBoxes[i].BackColor = System.Drawing.SystemColors.ControlDark;
                if (somethingFilled & !allFilled)
                {
                    stepTextBoxes[i].BackColor = Color.Gold;
                    colTextBoxes[i].BackColor = Color.Gold;
                    lapsTextBoxes[i].BackColor = Color.Gold;
                    widthTextBoxes[i].BackColor = Color.Gold;
                    precTextBoxes[i].BackColor = Color.Gold;
                    exitFlag = false;
                }
                if ((widthTextBoxes[i].Text != "") && (stepTextBoxes[i].Text != "") && ((Convert.ToUInt16(stepTextBoxes[i].Text) - Convert.ToUInt16(widthTextBoxes[i].Text) < 0) || (Convert.ToUInt16(stepTextBoxes[i].Text) + Convert.ToUInt16(widthTextBoxes[i].Text) > 1056)))
                {
                    stepTextBoxes[i].BackColor = Color.Green;
                    widthTextBoxes[i].BackColor = Color.Green;
                    exitFlag = false;
                }
                if ((stepTextBoxes[i].Text != "") && (Convert.ToInt16(stepTextBoxes[i].Text) > 1056))
                {
                    stepTextBoxes[i].BackColor = Color.Red;
                    exitFlag = false;
                }
                if ((lapsTextBoxes[i].Text != "") && (Convert.ToInt16(lapsTextBoxes[i].Text) <= 0))
                {
                    lapsTextBoxes[i].BackColor = Color.Red;
                    exitFlag = false;
                }
                if ((widthTextBoxes[i].Text != "") && (Convert.ToInt16(widthTextBoxes[i].Text) <= 0))
                {
                    widthTextBoxes[i].BackColor = Color.Red;
                    exitFlag = false;
                }
                if (allFilled & exitFlag)
                {
                    data.Add(new PreciseEditorData((byte)i, Convert.ToUInt16(stepTextBoxes[i].Text), Convert.ToByte(colTextBoxes[i].Text), Convert.ToUInt16(lapsTextBoxes[i].Text), Convert.ToUInt16(widthTextBoxes[i].Text), (float)0/*Convert.ToSingle(precTextBoxes[i].Text)*/));
                }
            }
            return exitFlag;
        }
        
        private void applyButton_Click(object sender, EventArgs e)
        {
            if (checkTextBoxes())
            {
                Config.SavePreciseOptions(data, (ushort)(expTimeNumericUpDown.Value), (ushort)(idleTimeNumericUpDown.Value),
                       (double)(iVoltageNumericUpDown.Value), (double)(CPNumericUpDown.Value), (double)(eCurrentNumericUpDown.Value), (double)(hCurrentNumericUpDown.Value), (double)(fV1NumericUpDown.Value), (double)(fV2NumericUpDown.Value));
                Commander.notRareModeRequested = rareModeCheckBox.Checked;
                Commander.AddToSend(new sendIVoltage());
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void oneDigitTextbox_TextChanged(object sender, EventArgs e)
        {
            char[] numbers = { '1', '2' };
            char[] tempCharArray = ((TextBox)sender).Text.ToCharArray();
            string outputString = "";
            foreach (char ch in tempCharArray)
            {
                foreach (char compareChar in numbers)
                {
                    if (ch == compareChar)
                    {
                        outputString += ch;
                        ((TextBox)sender).Text = outputString;
                        return;
                    }
                }
            }
            ((TextBox)sender).Text = outputString;
        }

        private void integralTextbox_TextChanged(object sender, EventArgs e)
        {
            char[] numbers = {'0','1','2','3','4','5','6','7','8','9'};
            char[] tempCharArray = ((TextBox)sender).Text.ToCharArray();
            string outputString = "";
            foreach (char ch in tempCharArray) 
            {
                foreach (char compareChar in numbers)
                {
                    if (ch == compareChar)
                    {
                        outputString += ch;
                        break;
                    }
                }
            }
            ((TextBox)sender).Text = outputString;
        }

        private void positiveNumericTextbox_TextChanged(object sender, EventArgs e)
        {
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            bool waitFirstDot = true;
            char[] tempCharArray = ((TextBox)sender).Text.ToCharArray();
            string outputString = "";
            foreach (char ch in tempCharArray)
            {
                if (waitFirstDot && (ch == '.'))
                {
                    waitFirstDot = false;
                    outputString += ch;
                    continue;
                }
                foreach (char compareChar in numbers)
                {
                    if (ch == compareChar)
                    {
                        outputString += ch;
                        break;
                    }
                }
            }
            ((TextBox)sender).Text = outputString;
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
            for (int i = 0; i < 20; ++i) {
                stepTextBoxes[i].Text = "";
                colTextBoxes[i].Text = "";
                lapsTextBoxes[i].Text = "";
                widthTextBoxes[i].Text = "";
                precTextBoxes[i].Text = "";
            }
        }

        private void PreciseOptionsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            instance = null;
            upLevel.InvokeRefreshButtons();
        }

        private void saveFileButton_Click(object sender, EventArgs e)
        {
            if (saveCommonDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                Config.saveCommonOptions(saveCommonDataFileDialog.FileName, (ushort)(expTimeNumericUpDown.Value), (ushort)(idleTimeNumericUpDown.Value),
                                         (double)(iVoltageNumericUpDown.Value), (double)(CPNumericUpDown.Value), (double)(eCurrentNumericUpDown.Value), (double)(hCurrentNumericUpDown.Value), (double)(fV1NumericUpDown.Value), (double)(fV2NumericUpDown.Value));
            }
        }

        private void loadFileButton_Click(object sender, EventArgs e)
        {
            if (openCommonDataFileDialog.ShowDialog() == DialogResult.OK)
            {
                loadCommonData(openCommonDataFileDialog.FileName);
            }
        }
    }
}