using System;
using PreciseEditorData = Flavor.Common.Data.Measure.PreciseEditorData;

namespace Flavor.Forms {
    partial class PlacePointForm: AddPointForm {
        int pNum = -1;
        public int PointNumber {
            get { return pNum; }
        }
        PlacePointForm()
            : base() {
            InitializeComponent();
            for (int i = 1; i <= 20; ++i)
                pNumComboBox.Items.Add(i.ToString());
        }
        public PlacePointForm(PreciseEditorData ped)
            : this() {
            oneRow.setValues(ped);
        }

        void pNumComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            pNum = pNumComboBox.SelectedIndex;
            okButton.Enabled = true;
        }
        new void okButton_Click(object sender, EventArgs e) {
            pNum = pNumComboBox.SelectedIndex;
            base.okButton_Click(sender, e);
        }
    }
}
