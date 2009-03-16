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
        private System.Windows.Forms.Label colNumLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;

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
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();

            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.loadPreciseEditorFromFileButton = new System.Windows.Forms.Button();
            this.savePreciseEditorToFileButton = new System.Windows.Forms.Button();
            this.savePreciseEditorToFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.loadPreciseEditorFromFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.colNumLabel = new System.Windows.Forms.Label();

            this.groupBox1.SuspendLayout();
            this.params_groupBox.SuspendLayout();
            this.SuspendLayout();
            // label1
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Номер\r\nпика";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // label8
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.SystemColors.Control;
            this.label8.Location = new System.Drawing.Point(55, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 26);
            this.label8.TabIndex = 1;
            this.label8.Text = "Ступенька\r\n(<=1056)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // label9
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.SystemColors.Control;
            this.label9.Location = new System.Drawing.Point(130, 29);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Проходы";
            // label10
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.SystemColors.Control;
            this.label10.Location = new System.Drawing.Point(182, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "Ширина";
            // label11
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.SystemColors.Control;
            this.label11.Location = new System.Drawing.Point(232, 29);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(54, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "Точность";
            // label12
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.SystemColors.Control;
            this.label12.Location = new System.Drawing.Point(21, 42);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(13, 13);
            this.label12.TabIndex = 106;
            this.label12.Text = "1";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // label13
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.SystemColors.Control;
            this.label13.Location = new System.Drawing.Point(21, 57);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(13, 13);
            this.label13.TabIndex = 107;
            this.label13.Text = "2";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // label14
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.SystemColors.Control;
            this.label14.Location = new System.Drawing.Point(21, 72);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(13, 13);
            this.label14.TabIndex = 108;
            this.label14.Text = "3";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // label15
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.SystemColors.Control;
            this.label15.Location = new System.Drawing.Point(21, 87);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(13, 13);
            this.label15.TabIndex = 109;
            this.label15.Text = "4";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // label16
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.SystemColors.Control;
            this.label16.Location = new System.Drawing.Point(21, 102);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(13, 13);
            this.label16.TabIndex = 110;
            this.label16.Text = "5";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // label17
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.SystemColors.Control;
            this.label17.Location = new System.Drawing.Point(21, 117);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(13, 13);
            this.label17.TabIndex = 111;
            this.label17.Text = "6";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // label18
            this.label18.AutoSize = true;
            this.label18.BackColor = System.Drawing.SystemColors.Control;
            this.label18.Location = new System.Drawing.Point(21, 132);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(13, 13);
            this.label18.TabIndex = 112;
            this.label18.Text = "7";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // label19
            this.label19.AutoSize = true;
            this.label19.BackColor = System.Drawing.SystemColors.Control;
            this.label19.Location = new System.Drawing.Point(21, 147);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(13, 13);
            this.label19.TabIndex = 113;
            this.label19.Text = "8";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // label20
            this.label20.AutoSize = true;
            this.label20.BackColor = System.Drawing.SystemColors.Control;
            this.label20.Location = new System.Drawing.Point(21, 162);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(13, 13);
            this.label20.TabIndex = 114;
            this.label20.Text = "9";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // label21
            this.label21.AutoSize = true;
            this.label21.BackColor = System.Drawing.SystemColors.Control;
            this.label21.Location = new System.Drawing.Point(21, 177);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(19, 13);
            this.label21.TabIndex = 115;
            this.label21.Text = "10";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // label22
            this.label22.AutoSize = true;
            this.label22.BackColor = System.Drawing.SystemColors.Control;
            this.label22.Location = new System.Drawing.Point(21, 192);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(19, 13);
            this.label22.TabIndex = 116;
            this.label22.Text = "11";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // label23
            this.label23.AutoSize = true;
            this.label23.BackColor = System.Drawing.SystemColors.Control;
            this.label23.Location = new System.Drawing.Point(21, 207);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(19, 13);
            this.label23.TabIndex = 117;
            this.label23.Text = "12";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // label24
            this.label24.AutoSize = true;
            this.label24.BackColor = System.Drawing.SystemColors.Control;
            this.label24.Location = new System.Drawing.Point(21, 222);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(19, 13);
            this.label24.TabIndex = 118;
            this.label24.Text = "13";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // label25
            this.label25.AutoSize = true;
            this.label25.BackColor = System.Drawing.SystemColors.Control;
            this.label25.Location = new System.Drawing.Point(21, 237);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(19, 13);
            this.label25.TabIndex = 119;
            this.label25.Text = "14";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // label26
            this.label26.AutoSize = true;
            this.label26.BackColor = System.Drawing.SystemColors.Control;
            this.label26.Location = new System.Drawing.Point(21, 252);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(19, 13);
            this.label26.TabIndex = 120;
            this.label26.Text = "15";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // label27
            this.label27.AutoSize = true;
            this.label27.BackColor = System.Drawing.SystemColors.Control;
            this.label27.Location = new System.Drawing.Point(21, 267);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(19, 13);
            this.label27.TabIndex = 121;
            this.label27.Text = "16";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // label28
            this.label28.AutoSize = true;
            this.label28.BackColor = System.Drawing.SystemColors.Control;
            this.label28.Location = new System.Drawing.Point(21, 282);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(19, 13);
            this.label28.TabIndex = 122;
            this.label28.Text = "17";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // label29
            this.label29.AutoSize = true;
            this.label29.BackColor = System.Drawing.SystemColors.Control;
            this.label29.Location = new System.Drawing.Point(21, 297);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(19, 13);
            this.label29.TabIndex = 123;
            this.label29.Text = "18";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // label30
            this.label30.AutoSize = true;
            this.label30.BackColor = System.Drawing.SystemColors.Control;
            this.label30.Location = new System.Drawing.Point(21, 312);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(19, 13);
            this.label30.TabIndex = 124;
            this.label30.Text = "19";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // groupBox1
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.colNumLabel);
            this.groupBox1.Controls.Add(this.label31);
            this.groupBox1.Controls.Add(this.clearButton);
            this.groupBox1.Controls.Add(this.label30);
            this.groupBox1.Controls.Add(this.loadPreciseEditorFromFileButton);
            this.groupBox1.Controls.Add(this.label29);
            this.groupBox1.Controls.Add(this.savePreciseEditorToFileButton);
            this.groupBox1.Controls.Add(this.label28);
            this.groupBox1.Controls.Add(this.label27);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312, 385);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Редактор областей сканирования";
            // label31
            this.label31.AutoSize = true;
            this.label31.BackColor = System.Drawing.SystemColors.Control;
            this.label31.Location = new System.Drawing.Point(21, 327);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(19, 13);
            this.label31.TabIndex = 125;
            this.label31.Text = "20";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.params_groupBox.Location = new System.Drawing.Point(330, 12);
            // loadFileButton
            this.loadFileButton.Location = new System.Drawing.Point(126, 228);
            // saveFileButton
            this.saveFileButton.Location = new System.Drawing.Point(9, 228);
            // fV2NumericUpDown
            this.fV2NumericUpDown.Location = new System.Drawing.Point(195, 201);
            // fV1NumericUpDown
            this.fV1NumericUpDown.Location = new System.Drawing.Point(195, 175);
            // hCurrentNumericUpDown
            this.hCurrentNumericUpDown.Location = new System.Drawing.Point(195, 149);
            // eCurrentNumericUpDown
            this.eCurrentNumericUpDown.Location = new System.Drawing.Point(195, 123);
            // CPNumericUpDown
            this.CPNumericUpDown.Location = new System.Drawing.Point(195, 97);
            // iVoltageNumericUpDown
            this.iVoltageNumericUpDown.Location = new System.Drawing.Point(195, 71);
            // idleTimeNumericUpDown
            this.idleTimeNumericUpDown.Location = new System.Drawing.Point(195, 45);
            // expTimeNumericUpDown
            this.expTimeNumericUpDown.Location = new System.Drawing.Point(195, 19);
            // label34
            this.label33.Location = new System.Drawing.Point(6, 203);
            // label35
            this.label32.Location = new System.Drawing.Point(6, 177);
            // label36
            this.label7.Location = new System.Drawing.Point(6, 151);
            // label37
            this.label6.Location = new System.Drawing.Point(6, 125);
            // label38
            this.label5.Location = new System.Drawing.Point(6, 99);
            // label39
            this.label4.Location = new System.Drawing.Point(6, 73);
            // label40
            this.label3.Location = new System.Drawing.Point(6, 47);
            // label41
            this.label2.Location = new System.Drawing.Point(6, 21);
            // rareModeCheckBox
            this.rareModeCheckBox.Location = new System.Drawing.Point(330, 275);
            // applyButton
            this.applyButton.Location = new System.Drawing.Point(330, 374);
            // cancel_butt
            this.cancel_butt.Location = new System.Drawing.Point(540, 374);
            // ok_butt
            this.ok_butt.Location = new System.Drawing.Point(408, 374);
            // savePreciseEditorToFileDialog
            this.savePreciseEditorToFileDialog.DefaultExt = "ped";
            this.savePreciseEditorToFileDialog.Filter = "Precise Editor Data Files (*.ped)|*.ped";
            // loadPreciseEditorFromFileDialog
            this.loadPreciseEditorFromFileDialog.DefaultExt = "ped";
            this.loadPreciseEditorFromFileDialog.Filter = "Precise editor data files (*.ped)|*.ped|Precise specter files (*.psf)|*.psf";
            // colNumLabel
            this.colNumLabel.AutoSize = true;
            this.colNumLabel.Location = new System.Drawing.Point(105, 29);
            this.colNumLabel.Name = "colNumLabel";
            this.colNumLabel.Size = new System.Drawing.Size(29, 13);
            this.colNumLabel.TabIndex = 126;
            this.colNumLabel.Text = "Кол.";
            // PreciseOptionsForm
            this.ClientSize = new System.Drawing.Size(609, 409);
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
        
        protected override void applyButton_Click(object sender, EventArgs e)
        {
            if (checkTextBoxes())
            {
                Config.SavePreciseOptions(data);
                base.applyButton_Click(sender, e);
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
    }
}