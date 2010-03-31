using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Controls
{
    public partial class PreciseEditorRowMinus: UserControl
    {
        internal string StepText
        {
            get { return stepTextBox.Text; }
            set { stepTextBox.Text = value; }
        }
        internal string ColText
        {
            get { return colTextBox.Text; }
            set { colTextBox.Text = value; }
        }
        internal string WidthText
        {
            get { return widthTextBox.Text; }
        }
        protected bool stepAndColModifiable = false;
        protected bool allFilled = false;
        internal bool AllFilled
        {
            get { return allFilled; }
        }
        public PreciseEditorRowMinus(): base()
        {
            InitializeComponent();
            this.widthTextBox.TextChanged += new System.EventHandler(Utility.integralTextbox_TextChanged);
        }
        internal virtual bool checkTextBoxes()
        {
            bool exitFlag = true;
            bool somethingFilled = (stepTextBox.Text != "") || (colTextBox.Text != "") || (widthTextBox.Text != "");
            this.allFilled = (stepTextBox.Text != "") && (colTextBox.Text != "") && (widthTextBox.Text != "");
            stepTextBox.BackColor = System.Drawing.SystemColors.Control;
            colTextBox.BackColor = System.Drawing.SystemColors.Control;
            widthTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            if (somethingFilled && !this.allFilled)
            {
                stepTextBox.BackColor = Color.Gold;
                colTextBox.BackColor = Color.Gold;
                widthTextBox.BackColor = Color.Gold;
                exitFlag = false;
            }
            if ((widthTextBox.Text != "") && (stepTextBox.Text != "") &&
                ((Convert.ToUInt16(stepTextBox.Text) - Convert.ToUInt16(widthTextBox.Text) < Config.MIN_STEP) ||
                (Convert.ToUInt16(stepTextBox.Text) + Convert.ToUInt16(widthTextBox.Text) > Config.MAX_STEP)))
            {
                stepTextBox.BackColor = Color.Green;
                widthTextBox.BackColor = Color.Green;
                exitFlag = false;
            }
            if ((stepTextBox.Text != "") && (Convert.ToInt16(stepTextBox.Text) > Config.MAX_STEP))
            {
                stepTextBox.BackColor = Color.Red;
                exitFlag = false;
            }
            return exitFlag;
        }
        internal virtual void setValues(Utility.PreciseEditorData p)
        {
            stepTextBox.Text = p.Step.ToString();
            colTextBox.Text = p.Collector.ToString();
            widthTextBox.Text = p.Width.ToString();
        }
    }
}
