using System.Windows.Forms;
using Flavor.Common;
using System.ComponentModel;

namespace Flavor.Controls {
    [DefaultEvent("DoubleClick")]
    public partial class PreciseEditorLabelRowMinus: UserControl {
        public PreciseEditorLabelRowMinus()
            : base() {
            InitializeComponent();
            this.peakCenterLabel.Text = string.Format("Ступенька\r\n(<={0})", Config.MAX_STEP);
        }
    }
}
