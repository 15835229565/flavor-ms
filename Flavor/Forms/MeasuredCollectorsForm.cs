using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Flavor.Common;
using Flavor.Controls;

namespace Flavor.Forms {
    internal partial class MeasuredCollectorsForm: CollectorsForm, IMeasured {
        /*private static MeasuredCollectorsForm instance = null;
        internal MeasuredCollectorsForm getInstance(bool isPrecise) {
            if (instance == null) {
                instance = new MeasuredCollectorsForm(isPrecise);
            }
            return instance;
        }*/

        internal MeasuredCollectorsForm()
            : base(Graph.Instance, false) {
            InitializeComponent();
        }
        protected sealed override GraphPanel initPanel() {
            return new MeasureGraphPanel();
		}

        #region IMeasured Members

        public void initMeasure(bool isPrecise) {
            // TODO: different types of panel
            Graph.Instance.OnNewGraphData += new Graph.GraphEventHandler(InvokeRefreshGraph);
            PreciseSpectrumDisplayed = isPrecise;
            if (isPrecise) {
                //(Panel as MeasureGraphPanel).sensmeasure_button_Click();
            } else
                (Panel as MeasureGraphPanel).overview_button_Click(Config.sPoint, Config.ePoint);
            Show();
            Activate();
        }
        public void prepareControlsOnMeasureStart() {
            // not so good..
            if (PreciseSpectrumDisplayed)
                // search temporary here
                setXScaleLimits(Config.PreciseData.FindAll(Utility.PreciseEditorData.PeakIsUsed));
            else
                setXScaleLimits();

            Panel.Enable();
            // TODO: and set it visible together with menu item set checked!

            specterSavingEnabled = false;
            Graph.ResetPointListsWithEvent();
        }
        public void deactivateOnMeasureStop() {
            Panel.Disable();
            specterSavingEnabled = true;
            Graph.Instance.OnNewGraphData -= new Graph.GraphEventHandler(InvokeRefreshGraph);
        }

        #endregion
        
        protected sealed override bool saveData() {
            saveSpecterFileDialog.FileName = "";
			return base.saveData();        
		}

        private void InvokeRefreshGraph(Graph.Recreate recreate) {
            if (this.InvokeRequired) {
                // TODO: NullPointerException here..
                this.Invoke(new Graph.GraphEventHandler(refreshGraph), recreate);
                return;
            }
            refreshGraph(recreate);
        }
        private void refreshGraph(Graph.Recreate recreate) {
            // not trivial value..
            if (recreate == Graph.Recreate.Both)
                return;
            refreshGraphicsOnMeasureStep();
        }
        private void refreshGraphicsOnMeasureStep() {
            MeasureGraphPanel panel = Panel as MeasureGraphPanel;
            panel.performStep();
            if (PreciseSpectrumDisplayed)
                panel.refreshGraphicsOnPreciseStep();
            else {
                yAxisChange();
                panel.refreshGraphicsOnScanStep();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            if (e.CloseReason != CloseReason.UserClosing)
                base.OnFormClosing(e);
        }
    }
}

