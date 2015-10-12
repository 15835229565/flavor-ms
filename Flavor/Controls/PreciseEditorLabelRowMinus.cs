using System.Windows.Forms;
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Controls {
    partial class PreciseEditorLabelRowMinus: UserControl {
        public PreciseEditorLabelRowMinus()
            : base() {
            InitializeComponent();
            peakCenterLabel.Text = string.Format(Resources.PreciseEditorLabelRowMinus_peakCenterLabel_Text_Format, Config.MAX_STEP);
            colNumLabel.BringToFront();
        }
    }
}
