using System;
using System.Windows.Forms;
using Flavor.Controls;
using Graph = Flavor.Common.Graph;
using PreciseEditorData = Flavor.Common.Utility.PreciseEditorData;
// be careful with Config data. use constants only!
using Config = Flavor.Common.Config;

namespace Flavor.Forms {
    internal partial class MeasuredCollectorsForm: CollectorsForm2, IMeasured {
        public event EventHandler MeasureCancelRequested;
        protected virtual void OnMeasureCancelRequested() {
            if (MeasureCancelRequested != null)
                MeasureCancelRequested(this, EventArgs.Empty);
        }
        internal MeasuredCollectorsForm()
            : base(Graph.Instance, false) {
            InitializeComponent();
        }

        #region IMeasured Members

        public void initMeasure(int progressMaximum, bool isPrecise) {
            // TODO: different types of panel
            PreciseSpectrumDisplayed = isPrecise;
            
            MeasureGraphPanel panel;
            if (isPrecise) {
                panel = new PreciseMeasureGraphPanel();
                // search temporary here
                // TODO: use extension method getUsed()
                setXScaleLimits(Config.PreciseData.FindAll(PreciseEditorData.PeakIsUsed));
            } else {
                panel = new ScanMeasureGraphPanel(Config.sPoint, Config.ePoint);
                setXScaleLimits();
            }
            panel.MeasureCancelRequested += MeasuredCollectorsForm_MeasureCancelRequested;
            Graph.MeasureGraph g = Graph.Instance;
            panel.Graph = g;
            panel.ProgressMaximum = progressMaximum;

            Panel = panel;
            Panel.Enable();
            // TODO: and set it visible together with menu item set checked!

            specterSavingEnabled = false;

            g.ResetPointListsWithEvent();
            g.NewGraphData += InvokeRefreshGraph;
            
            Show();
            Activate();
        }
        public void deactivateOnMeasureStop() {
            Panel.Disable();
            specterSavingEnabled = true;
            Graph.Instance.NewGraphData -= InvokeRefreshGraph;
        }

        #endregion
        
        protected sealed override bool saveData() {
            saveSpecterFileDialog.FileName = "";
			return base.saveData();        
		}

        private void MeasuredCollectorsForm_MeasureCancelRequested(object sender, EventArgs e) {
            // do something local
            (Panel as MeasureGraphPanel).MeasureCancelRequested -= MeasuredCollectorsForm_MeasureCancelRequested;
            OnMeasureCancelRequested();
        }

        private void InvokeRefreshGraph(int[] recreate) {
            if (this.InvokeRequired) {
                this.Invoke(new Graph.GraphEventHandler(refreshGraph), recreate);
                return;
            }
            refreshGraph(recreate);
        }
        private void refreshGraph(int[] recreate) {
            // TODO: use recreate to refresh only affected collectors
            refreshGraphicsOnMeasureStep();
        }
        private void refreshGraphicsOnMeasureStep() {
            (Panel as MeasureGraphPanel).performStep();
            if (!PreciseSpectrumDisplayed)
                yAxisChange();
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
            else
                base.OnFormClosing(e);
        }
    }
}

