using System;
using System.Windows.Forms;
using Flavor.Controls;
using Graph = Flavor.Common.Data.Measure.Graph;
using PreciseEditorData = Flavor.Common.Data.Measure.PreciseEditorData;
// be careful with Config data. use constants only!
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Forms {
    partial class MeasuredCollectorsForm: CollectorsForm2, IMeasured {
        public event EventHandler MeasureCancelRequested;
        protected virtual void OnMeasureCancelRequested() {
            if (MeasureCancelRequested != null)
                MeasureCancelRequested(this, EventArgs.Empty);
        }
        public MeasuredCollectorsForm()
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

        void MeasuredCollectorsForm_MeasureCancelRequested(object sender, EventArgs e) {
            // do something local
            (Panel as MeasureGraphPanel).MeasureCancelRequested -= MeasuredCollectorsForm_MeasureCancelRequested;
            OnMeasureCancelRequested();
        }

        void InvokeRefreshGraph(uint[] counts, params int[] recreate) {
            Invoke(new Graph.GraphEventHandler(refreshGraph), counts, recreate);
        }
        void refreshGraph(uint[] counts, params int[] recreate) {
            // TODO: use recreate to refresh only affected collectors
            refreshGraphicsOnMeasureStep(counts);
        }
        void refreshGraphicsOnMeasureStep(uint[] counts) {
            (Panel as MeasureGraphPanel).performStep(counts);
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

