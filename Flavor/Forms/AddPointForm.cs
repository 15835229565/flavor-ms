using System;
using System.Windows.Forms;
using PreciseEditorData = Flavor.Common.Data.Measure.PreciseEditorData;
// ?
using Graph = Flavor.Common.Data.Measure.Graph;

namespace Flavor.Forms {
    partial class AddPointForm: Form {
        protected AddPointForm()
            : base() {
            InitializeComponent();
        }
        public AddPointForm(ushort step, byte col)
            : this() {
            oneRow.StepText = step.ToString();
            oneRow.ColText = col.ToString();
        }

        protected void okButton_Click(object sender, EventArgs e) {
            if (oneRow.checkTextBoxes() && oneRow.AllFilled) {
                Graph.PointToAdd = new PreciseEditorData((byte)0, Convert.ToUInt16(oneRow.StepText),
                                       Convert.ToByte(oneRow.ColText), Convert.ToUInt16(oneRow.LapsText),
                                       Convert.ToUInt16(oneRow.WidthText), (float)0, oneRow.CommentText);
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}