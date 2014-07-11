using System.Windows.Forms;
using Graph = Flavor.Common.Data.Measure.Graph;

namespace Flavor.Controls {
    public partial class GraphPanel: Panel {
        Graph graph = null;
        internal Graph Graph {
            get { return graph; }
            set { graph = value; }
        }

        public new bool Enabled {
            get { return base.Enabled; }
            protected set { base.Enabled = value; }
        }

        public GraphPanel() {
            InitializeComponent();
        }
        internal void Enable() {
            if (graph == null)
                return;
            var commonOpts = graph.CommonOptions;
            if (commonOpts == null)
                return;

            SuspendLayout();

            etime_label.Text = commonOpts.eTimeReal.ToString();
            //itime_label.Text = commonOpts.iTimeReal.ToString();
            iVolt_label.Text = commonOpts.iVoltageReal.ToString("f3");
            //cp_label.Text = commonOpts.CPReal.ToString("f3");
            cp_label.Text = commonOpts.C.ToString("f5");
            emCurLabel.Text = commonOpts.eCurrentReal.ToString("f3");
            //heatCurLabel.Text = commonOpts.hCurrentReal.ToString("f3");
            f1_label.Text = commonOpts.fV1Real.ToString("f3");
            f2_label.Text = commonOpts.fV2Real.ToString("f3");

            prepareControls();
            this.Enabled = true;

            ResumeLayout(false);
            PerformLayout();
        }
        protected virtual void prepareControls() {
            //TODO: move up
            firstStepLabel.Visible = false;
            startScanTextLabel.Visible = false;
            lastStepLabel.Visible = false;
            label18.Visible = false;
            if (graph.isPreciseSpectrum)
                return;
            // TODO: can be null on empty spectrum..
            setScanBounds((ushort)((graph.Displayed1Steps[0])[0].X), (ushort)((graph.Displayed1Steps[0])[graph.Displayed1Steps[0].Count - 1].X));
        }
        protected void setScanBounds(ushort start, ushort end) {
            //TODO: move up
            startScanTextLabel.Visible = true;
            label18.Visible = true;

            firstStepLabel.Text = start.ToString();
            lastStepLabel.Text = end.ToString();
            firstStepLabel.Visible = true;
            lastStepLabel.Visible = true;
        }
            
        internal void Disable() {
            this.Enabled = false;
            disableControls();
        }
        protected virtual void disableControls() {}
    }
}
