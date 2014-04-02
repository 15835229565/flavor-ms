using System;
using System.Drawing;
using System.Windows.Forms;
using Graph = Flavor.Common.Graph;
using Config = Flavor.Common.Config;
// remove this reference
using Utility = Flavor.Common.Utility;

namespace Flavor.Forms {
    partial class SetScalingCoeffForm: Form {
        private byte myCol = 0;
        private ushort myStep = 0;
        private Graph graph;
        internal SetScalingCoeffForm()
            : base() {
            InitializeComponent();
            // TODO: better solution, make new CustomTextBoxClass or extension method
            this.massTextBox.KeyPress += Utility.positiveNumericTextbox_TextChanged;
        }

        internal SetScalingCoeffForm(ushort step, byte col, Graph graph)
            : this() {
            myCol = col;
            myStep = step;
            this.graph = graph;
            this.stepTextBox.Text = step.ToString();
            this.Text += " " + col.ToString();
            // move this reference up
            if (graph != Graph.Instance)
                this.Text += " (только для текущего спектра)";
        }

        protected void okButton_Click(object sender, EventArgs e) {
            double mass;
            try {
                mass = Convert.ToDouble(massTextBox.Text);
            } catch (System.FormatException) {
                massTextBox.BackColor = Color.Red;
                return;
            } catch (System.OverflowException) {
                massTextBox.BackColor = Color.Red;
                return;
            }
            //check mass is not near 0
            if (mass < 1) {
                massTextBox.BackColor = Color.Red;
                return;
            }
            bool result = graph == Graph.Instance ? Config.setScalingCoeff(myCol, myStep, mass) : graph.setScalingCoeff(myCol, myStep, mass);
            this.DialogResult = result ? DialogResult.Yes : DialogResult.No;
            this.Close();
        }

        protected void cancelButton_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}