using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Flavor.Common;
using Flavor.Controls;

namespace Flavor.Forms
{
    partial class AddPointForm: Form
    {
        protected PreciseEditorRow oneRow;
        protected Utility.PreciseEditorLabelRow oneLabelRow;

        internal AddPointForm(): base()
        {
            InitializeComponent();

            this.oneLabelRow = new Utility.PreciseEditorLabelRow(12, 8);
            this.Controls.AddRange(oneLabelRow.getControls());

            this.oneRow = new PreciseEditorRow();
            this.oneRow.Location = new Point(13, 42);
            this.Controls.Add(oneRow);
        }
        internal AddPointForm(ushort step, byte col): this()
        {
            this.oneRow.StepText = step.ToString();
            this.oneRow.ColText = col.ToString();
        }

        protected void okButton_Click(object sender, EventArgs e)
        {
            if (oneRow.checkTextBoxes() && oneRow.AllFilled)
            {
                //Saving of point
                Graph.PointToAdd = new Utility.PreciseEditorData((byte)0, Convert.ToUInt16(oneRow.StepText),
                                       Convert.ToByte(oneRow.ColText), Convert.ToUInt16(oneRow.LapsText),
                                       Convert.ToUInt16(oneRow.WidthText), (float)0, oneRow.CommentText);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        protected void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}