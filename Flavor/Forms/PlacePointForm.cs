using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Flavor
{
    class PlacePointForm : AddPointForm
    {
        private ComboBox pNumComboBox;
        private int pNum = -1;
        public int PointNumber
        {
            get { return pNum; }
        }
        public PlacePointForm(): base()
        {
            this.Text = "Вставка точки";
            this.pNumComboBox = new ComboBox();
            this.pNumComboBox.Location = new Point(174, 64);
            this.pNumComboBox.Size = new Size(40, 23);
            for (int i = 1; i <= 20; ++i )
                this.pNumComboBox.Items.Add(i.ToString());
            this.pNumComboBox.SelectedIndexChanged += new EventHandler(pNumComboBox_SelectedIndexChanged);
            this.Controls.Add(this.pNumComboBox);
            this.okButton.Enabled = false;
        }
        public PlacePointForm(Utility.PreciseEditorData ped)
            : this()
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
 