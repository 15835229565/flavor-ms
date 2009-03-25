using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Flavor
{
    static class Utility
    {
        public class PreciseEditorData
        {
            public PreciseEditorData(byte pn, ushort st, byte co, ushort it, ushort wi, float pr)
            {
                pointNumber = pn;
                step = st;
                collector = co;
                iterations = it;
                width = wi;
                precision = pr;
            }
            public PreciseEditorData(bool useit, byte pn, ushort st, byte co, ushort it, ushort wi, float pr, string comm)
            {
                usethis = useit;
                pointNumber = pn;
                step = st;
                collector = co;
                iterations = it;
                width = wi;
                precision = pr;
                comment = comm;
            }
            private bool usethis = true;
            private byte pointNumber;
            private ushort step;
            private byte collector;
            private ushort iterations;
            private ushort width;
            private float precision;
            private string comment = "";
            ZedGraph.PointPairList associatedPoints;
            public ZedGraph.PointPairList AssociatedPoints
            {
                get { return associatedPoints; }
                set { associatedPoints = value; }
            }
            public bool Use
            {
                get { return usethis; }
                //set { usethis = value; }
            }
            public byte pNumber
            {
                get { return pointNumber; }
                //set { pointNumber = value; }
            }
            public ushort Step
            {
                get { return step; }
                //set { step = value; }
            }
            public byte Collector
            {
                get { return collector; }
                //set { collector = value; }
            }
            public ushort Iterations
            {
                get { return iterations; }
                //set { iterations = value; }
            }
            public ushort Width
            {
                get { return width; }
                //set { width = value; }
            }
            public float Precision
            {
                get { return precision; }
                //set { precision = value; }
            }
            public string Comment
            {
                get { return comment; }
                //set { comment = value; }
            }
        }

        public class PreciseEditorRow
        {
            protected TextBox stepTextBox;
            public string StepText
            {
                get { return stepTextBox.Text; }
                set { stepTextBox.Text = value; }
            }
            protected TextBox colTextBox;
            public string ColText
            {
                get { return colTextBox.Text; }
                set { colTextBox.Text = value; }
            }
            protected TextBox lapsTextBox;
            public string LapsText
            {
                get { return lapsTextBox.Text; }
                set { lapsTextBox.Text = value; }
            }
            protected TextBox widthTextBox;
            public string WidthText
            {
                get { return widthTextBox.Text; }
                set { widthTextBox.Text = value; }
            }
            protected TextBox precTextBox;
            public string PrecText
            {
                get { return precTextBox.Text; }
                set { precTextBox.Text = value; }
            }
            protected TextBox commentTextBox;
            public string CommentText
            {
                get { return commentTextBox.Text; }
                set { commentTextBox.Text = value; }
            }
            protected bool stepAndColEnabled = false;
            public PreciseEditorRow()
            {
                this.stepTextBox = new TextBox();
                this.stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                this.stepTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.stepTextBox.Location = new System.Drawing.Point(0, 0);
                this.stepTextBox.Margin = new System.Windows.Forms.Padding(1);
                this.stepTextBox.MaxLength = 4;
                this.stepTextBox.Size = new System.Drawing.Size(50, 13);
                this.stepTextBox.Enabled = stepAndColEnabled;

                this.colTextBox = new TextBox();
                this.colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                this.colTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.colTextBox.Location = new System.Drawing.Point(52, 0);
                this.colTextBox.Margin = new System.Windows.Forms.Padding(1);
                this.colTextBox.MaxLength = 1;
                this.colTextBox.Size = new System.Drawing.Size(20, 13);
                this.colTextBox.Enabled = stepAndColEnabled;

                this.lapsTextBox = new TextBox();
                this.lapsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                this.lapsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.lapsTextBox.Location = new System.Drawing.Point(74, 0);
                this.lapsTextBox.Margin = new System.Windows.Forms.Padding(1);
                this.lapsTextBox.Size = new System.Drawing.Size(50, 13);
                this.lapsTextBox.TextChanged += new System.EventHandler(Utility.integralTextbox_TextChanged);

                this.widthTextBox = new TextBox();
                this.widthTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                this.widthTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.widthTextBox.Location = new System.Drawing.Point(126, 0);
                this.widthTextBox.Margin = new System.Windows.Forms.Padding(1);
                this.widthTextBox.MaxLength = 4;
                this.widthTextBox.Size = new System.Drawing.Size(50, 13);
                this.widthTextBox.TextChanged += new System.EventHandler(Utility.integralTextbox_TextChanged);

                this.precTextBox = new TextBox();
                this.precTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                this.precTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.precTextBox.Location = new System.Drawing.Point(178, 0);
                this.precTextBox.Margin = new System.Windows.Forms.Padding(1);
                this.precTextBox.Size = new System.Drawing.Size(50, 13);
                this.precTextBox.TextChanged += new System.EventHandler(Utility.positiveNumericTextbox_TextChanged);

                this.commentTextBox = new TextBox();
                this.commentTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                this.commentTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.commentTextBox.Location = new System.Drawing.Point(230, 0);
                this.commentTextBox.Margin = new System.Windows.Forms.Padding(1);
                this.commentTextBox.Size = new System.Drawing.Size(100, 13);
            }
            public PreciseEditorRow(int x, int y): this()
            {
                moveTo(x, y);
            }
            protected virtual void moveTo(int x, int y)
            {
                this.stepTextBox.Location = new System.Drawing.Point(this.stepTextBox.Location.X + x, this.stepTextBox.Location.Y + y);
                this.colTextBox.Location = new System.Drawing.Point(this.colTextBox.Location.X + x, this.colTextBox.Location.Y + y);
                this.lapsTextBox.Location = new System.Drawing.Point(this.lapsTextBox.Location.X + x, this.lapsTextBox.Location.Y + y);
                this.widthTextBox.Location = new System.Drawing.Point(this.widthTextBox.Location.X + x, this.widthTextBox.Location.Y + y);
                this.precTextBox.Location = new System.Drawing.Point(this.precTextBox.Location.X + x, this.precTextBox.Location.Y + y);
                this.commentTextBox.Location = new System.Drawing.Point(this.commentTextBox.Location.X + x, this.commentTextBox.Location.Y + y);
            }
            public virtual Control[] getControls()
            {
                return new Control[] { stepTextBox, colTextBox, lapsTextBox, widthTextBox, precTextBox, commentTextBox };
            }
            protected virtual void Clear()
            {
                stepTextBox.Text = "";
                colTextBox.Text = "";
                lapsTextBox.Text = "";
                widthTextBox.Text = "";
                precTextBox.Text = "";
                commentTextBox.Text = "";
                stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                lapsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                widthTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                precTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                commentTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            }
            protected virtual bool checkTextBoxes()
            {
                bool exitFlag = true;
                bool somethingFilled = ((lapsTextBox.Text != "") || (stepTextBox.Text != "") ||
                                        (colTextBox.Text != "") || (widthTextBox.Text != "") /*|| (precTextBox].Text != "")*/);
                bool allFilled = ((lapsTextBox.Text != "") && (stepTextBox.Text != "") && (colTextBox.Text != "") &&
                                  (widthTextBox.Text != "")/* && (precTextBox.Text != "")*/);
                stepTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                colTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                lapsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                widthTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                precTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                commentTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
                if (somethingFilled & !allFilled)
                {
                    stepTextBox.BackColor = Color.Gold;
                    colTextBox.BackColor = Color.Gold;
                    lapsTextBox.BackColor = Color.Gold;
                    widthTextBox.BackColor = Color.Gold;
                    precTextBox.BackColor = Color.Gold;
                    commentTextBox.BackColor = Color.Gold;
                    exitFlag = false;
                }
                if ((widthTextBox.Text != "") && (stepTextBox.Text != "") &&
                    ((Convert.ToUInt16(stepTextBox.Text) - Convert.ToUInt16(widthTextBox.Text) < 0) ||
                    (Convert.ToUInt16(stepTextBox.Text) + Convert.ToUInt16(widthTextBox.Text) > 1056)))
                {
                    stepTextBox.BackColor = Color.Green;
                    widthTextBox.BackColor = Color.Green;
                    exitFlag = false;
                }
                if ((stepTextBox.Text != "") && (Convert.ToInt16(stepTextBox.Text) > 1056))
                {
                    stepTextBox.BackColor = Color.Red;
                    exitFlag = false;
                }
                //Integral checkbox => (>0)
                /*
                if ((lapsTextBox.Text != "") && (Convert.ToInt16(lapsTextBox.Text) <= 0))
                {
                    lapsTextBox.BackColor = Color.Red;
                    exitFlag = false;
                }
                if ((widthTextBox.Text != "") && (Convert.ToInt16(widthTextBox.Text) <= 0))
                {
                    widthTextBox.BackColor = Color.Red;
                    exitFlag = false;
                }
                */
                return (exitFlag && allFilled);
            }
        }
        public class PreciseEditorRowPlus: PreciseEditorRow
        {
            private Label peakNumberLabel;
            public string PeakNumber
            {
                set { peakNumberLabel.Text = value; }
            }
            private CheckBox usePeakCheckBox;
            public bool UseChecked
            {
                get { return usePeakCheckBox.Checked; }
                set { usePeakCheckBox.Checked = value; }
            }
            private Button clearPeakButton;
            private static ToolTip clearRowToolTip = new ToolTip();
            public PreciseEditorRowPlus(): base()
            {
                base.moveTo(37, 0);
                this.peakNumberLabel = new Label();
                this.peakNumberLabel.AutoSize = true;
                this.peakNumberLabel.BackColor = System.Drawing.SystemColors.Control;
                this.peakNumberLabel.Location = new System.Drawing.Point(0, 0);
                this.peakNumberLabel.Size = new System.Drawing.Size(16, 13);
                this.peakNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                this.usePeakCheckBox = new CheckBox();
                this.usePeakCheckBox.Location = new System.Drawing.Point(22, 0);
                this.usePeakCheckBox.Size = new System.Drawing.Size(13, 13);
                this.usePeakCheckBox.Text = "";

                this.clearPeakButton = new Button();
                this.clearPeakButton.Location = new System.Drawing.Point(369, 0);
                this.clearPeakButton.Size = new System.Drawing.Size(13, 13);
                this.clearPeakButton.Margin = new Padding(1);
                this.clearPeakButton.MouseHover += new EventHandler(clearPeakButton_MouseHover);
                this.clearPeakButton.Click += new EventHandler(clearPeakButton_Click);

                this.stepAndColEnabled = true;
                this.stepTextBox.TextChanged += new System.EventHandler(Utility.integralTextbox_TextChanged);
                this.colTextBox.TextChanged += new System.EventHandler(Utility.oneDigitTextbox_TextChanged);
            }
            public PreciseEditorRowPlus(int x, int y): this()
            {
                moveTo(x, y);
            }
            protected override void moveTo(int x, int y)
            {
                base.moveTo(x, y);
                this.peakNumberLabel.Location = new System.Drawing.Point(this.peakNumberLabel.Location.X + x, this.peakNumberLabel.Location.Y + y);
                this.usePeakCheckBox.Location = new System.Drawing.Point(this.usePeakCheckBox.Location.X + x, this.usePeakCheckBox.Location.Y + y);
                this.clearPeakButton.Location = new System.Drawing.Point(this.clearPeakButton.Location.X + x, this.clearPeakButton.Location.Y + y);
            }
            public override Control[] getControls()
            {
                return new Control[] { peakNumberLabel, usePeakCheckBox, stepTextBox, colTextBox, lapsTextBox, widthTextBox, precTextBox, commentTextBox, clearPeakButton };
            }
            public new void Clear()
            {
                base.Clear();
                usePeakCheckBox.Checked = false;
            }
            private void clearPeakButton_MouseHover(object sender, EventArgs e)
            {
                PreciseEditorRowPlus.clearRowToolTip.Show("Очистить строку", (IWin32Window)sender);
            }
            private void clearPeakButton_Click(object sender, EventArgs e)
            {
                this.Clear();
            }
            public new bool checkTextBoxes() 
            {
                return base.checkTextBoxes();
            }
        }
        //Comparers and predicate for sorting and finding Utility.PreciseEditorData objects in List
        internal static int ComparePreciseEditorDataByPeakValue(PreciseEditorData ped1, PreciseEditorData ped2)
        {
            //Forward sort
            if (ped1 == null)
            {
                if (ped2 == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (ped2 == null)
                    return 1;
                else
                    return (int)(ped1.Step - ped2.Step);
            }
        }
        internal static int ComparePreciseEditorDataByUseFlagAndPeakValue(PreciseEditorData ped1, PreciseEditorData ped2)
        {
            //Forward sort
            if ((ped1 == null) || !ped1.Use)
            {
                if ((ped2 == null) || !ped2.Use)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if ((ped2 == null) || !ped2.Use)
                    return 1;
                else
                    return (int)(ped1.Step - ped2.Step);
            }
        }
        internal static bool PeakIsUsed(PreciseEditorData ped)
        {
            return ped.Use;
        }
        //Textbox charset limitations
        internal static void oneDigitTextbox_TextChanged(object sender, EventArgs e)
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
        internal static void integralTextbox_TextChanged(object sender, EventArgs e)
        {
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
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
        internal static void positiveNumericTextbox_TextChanged(object sender, EventArgs e)
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
    }
}
