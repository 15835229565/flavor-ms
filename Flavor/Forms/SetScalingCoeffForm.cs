using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Flavor
{
    partial class SetScalingCoeffForm : Form
    {
        private byte myCol = 0;
        private ushort myStep = 0;
        public SetScalingCoeffForm()
            : base()
        {
            InitializeComponent();

            this.massTextBox.TextChanged += new System.EventHandler(Utility.positiveNumericTextbox_TextChanged);
        }
        public SetScalingCoeffForm(ushort step, byte col)
            : this()
        {
            myCol = col;
            myStep = step;
            this.stepTextBox.Text = step.ToString();
            this.Name += " " + col;
        }

        protected void okButton_Click(object sender, EventArgs e)
        {
            double mass;
            try
            {
                mass = Convert.ToDouble(massTextBox.Text);
            }
            catch(System.InvalidCastException) 
            {
                massTextBox.BackColor = Color.Red;
                return;
            }
            Config.setScalingCoeff(myCol, myStep, mass);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}