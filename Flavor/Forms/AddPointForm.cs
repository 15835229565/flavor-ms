using System;
using System.Windows.Forms;

using Config = Flavor.Common.Settings.Config;
using PreciseEditorData = Flavor.Common.Data.Measure.PreciseEditorData;

namespace Flavor.Forms {
    partial class AddPointForm: Form {
        protected AddPointForm()
            : base() {
            InitializeComponent();
        }
        public AddPointForm(ushort step, byte col)
            : this() {
            oneRow.MaxNumber = Config.COLLECTOR_COEFFS.Length;
            oneRow.StepText = step.ToString();
            oneRow.ColText = col.ToString();
        }
        public PreciseEditorData PointToAdd { get; protected set; }
        protected void okButton_Click(object sender, EventArgs e) {
            if (oneRow.checkTextBoxes(Config.MIN_STEP, Config.MAX_STEP) && oneRow.AllFilled) {
                PointToAdd = new PreciseEditorData((byte)0, Convert.ToUInt16(oneRow.StepText),
                                       Convert.ToByte(oneRow.ColText), Convert.ToUInt16(oneRow.LapsText),
                                       Convert.ToUInt16(oneRow.WidthText), 0f, oneRow.CommentText);
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}