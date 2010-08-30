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

        public new bool Enabled {
            get { return base.Enabled; }
            private set { base.Enabled = value; }
        }

        public GraphPanel() {
            InitializeComponent();
        }
        internal void Enable() {
            if (graph == null)
                return;
            CommonOptions commonOpts = graph.CommonOptions;
            if (commonOpts == null)
                return;

            SuspendLayout();

            etime_label.Text = commonOpts.eTimeReal.ToString();
            itime_label.Text = commonOpts.iTimeReal.ToString();
            iVolt_label.Text = commonOpts.iVoltageReal.ToString("f3");
            cp_label.Text = commonOpts.CPReal.ToString("f3");
            emCurLabel.Text = commonOpts.eCurrentReal.ToString("f3");
            heatCurLabel.Text = commonOpts.hCurrentReal.ToString("f3");
            f1_label.Text = commonOpts.fV1Real.ToString("f3");
            f2_label.Text = commonOpts.fV2Real.ToString("f3");

            prepareControls();
            this.Enabled = true;

            ResumeLayout(false);
            PerformLayout();
        }
        protected virtual void prepareControls() {
            // TODO: make abstract
        }
        internal void Disable() {
            this.Enabled = false;
            disableControls();
        }
        protected virtual void disableControls() {
            // TODO: make abstract
        }
    }
}
