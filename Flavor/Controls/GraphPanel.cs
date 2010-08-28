using System;
using System.Windows.Forms;
using Flavor.Common;
using Flavor.Forms;

namespace Flavor.Controls {
    public partial class GraphPanel: Panel {
        private Graph graph = null;

        internal Graph Graph {
            get { return graph; }
            set { graph = value; }
        }
        public GraphPanel() {
            InitializeComponent();
        }
        internal virtual void prepareControlsOnMeasureStart() {
            if (graph == null)
                return;
            CommonOptions commonOpts = graph.CommonOptions;

            SuspendLayout();

            etime_label.Text = commonOpts.eTimeReal.ToString();
            itime_label.Text = commonOpts.iTimeReal.ToString();
            iVolt_label.Text = commonOpts.iVoltageReal.ToString("f3");
            cp_label.Text = commonOpts.CPReal.ToString("f3");
            emCurLabel.Text = commonOpts.eCurrentReal.ToString("f3");
            heatCurLabel.Text = commonOpts.hCurrentReal.ToString("f3");
            f1_label.Text = commonOpts.fV1Real.ToString("f3");
            f2_label.Text = commonOpts.fV2Real.ToString("f3");

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
