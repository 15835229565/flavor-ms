using System;
using System.Windows.Forms;
using Flavor.Controls;
using Flavor.Common.Data.Measure;
// be careful with Config data. use constants only!
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Forms {
    partial class MeasuredCollectorsForm: CollectorsForm2, IMeasured {
        public event EventHandler MeasureCancelRequested;
        protected virtual void OnMeasureCancelRequested() {
            MeasureCancelRequested.Raise(this, EventArgs.Empty);
        }
        public MeasuredCollectorsForm()
            : base(Graph.MeasureGraph.Instance, false) {
            // Init panel before ApplyResources
            // strangely is inited on measure init
            InitializeComponent();
        }
        protected override bool DisableTabPage(Collector collector) {
            // temporary disable first collector with garbage counts
            return Graph.MeasureGraph.Instance.Collectors[0] == collector;
        }

        #region IMeasured Members

        public void initMeasure(int progressMaximum, bool isPrecise) {
            // TODO: different types of panel
            PreciseSpectrumDisplayed = isPrecise;

            var g = Graph.MeasureGraph.Instance;
            MeasureGraphPanel panel;
            if (isPrecise) {
                panel = new PreciseMeasureGraphPanel();
                // search temporary here
                setXScaleLimits(g.PreciseData.GetUsed());
            } else {
                panel = new ScanMeasureGraphPanel(Config.sPoint, Config.ePoint);
                setXScaleLimits();
            }
            panel.MeasureCancelRequested += MeasuredCollectorsForm_MeasureCancelRequested;
            panel.Graph = g;
            panel.ProgressMaximum = progressMaximum;

            Panel = panel;
            Panel.Enable();
            // TODO: and set it visible together with menu item set checked!

            specterSavingEnabled = false;

            g.NewGraphData += InvokeRefreshGraph;
            g.ResetPointListsWithEvent();
            
            Show();
            Activate();
        }
        public void deactivateOnMeasureStop() {
            Panel.Disable();
            specterSavingEnabled = true;
            Graph.MeasureGraph.Instance.NewGraphData -= InvokeRefreshGraph;
        }

        #endregion
        
        protected sealed override bool saveData() {
            saveSpecterFileDialog.FileName = "";
			return base.saveData();        
		}

        void MeasuredCollectorsForm_MeasureCancelRequested(object sender, EventArgs e) {
            // do something local
            ((MeasureGraphPanel)Panel).MeasureCancelRequested -= MeasuredCollectorsForm_MeasureCancelRequested;
            OnMeasureCancelRequested();
        }

        void InvokeRefreshGraph(ushort pnt, uint[] counts, params int[] recreate) {
            Invoke(new Action(() => refreshGraph(pnt, counts, recreate)));
        }
        void refreshGraph(ushort pnt, uint[] counts, params int[] recreate) {
            // TODO: use recreate to refresh only affected collectors
            // TODO: avoid frequent refresh as point tooltips are invalidating too quickly
            base.RefreshGraph();
            if (counts != null)
                ((MeasureGraphPanel)Panel).performStep(pnt, counts);
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

