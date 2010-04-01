using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Forms
{
    internal partial class PlacePointForm: AddPointForm
    {
        private int pNum = -1;
        internal int PointNumber
        {
            get { return pNum; }
        }
        internal PlacePointForm(): base()
        {
            InitializeComponent();
            for (int i = 1; i <= 20; ++i )
                this.pNumComboBox.Items.Add(i.ToString());
        }
        internal PlacePointForm(Utility.PreciseEditorData ped): this()
        {
            oneRow.setValues(ped);
        }

        private void pNumComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            pNum = pNumComboBox.SelectedIndex;
            this.okButton.Enabled = true;
        }
        private new void okButton_Click(object sender, EventArgs e)
        {
            pNum = pNumComboBox.SelectedIndex;
            base.okButton_Click(sender, e);
        }
    }
}
 