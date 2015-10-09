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
        }
        public PlacePointForm(PreciseEditorData ped, int rowsCount)
            : this() {
            for (int i = 1; i <= rowsCount; ++i)
                pNumComboBox.Items.Add(i.ToString());
            oneRow.setValues(ped);
        }

        void pNumComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            pNum = pNumComboBox.SelectedIndex;
            okButton.Enabled = true;
        }
    }
}
