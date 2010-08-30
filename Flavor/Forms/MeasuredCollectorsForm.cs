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
            : base(Graph.Instance) {
            InitializeComponent();
            Graph.Instance.DisplayingMode = Graph.Displaying.Measured;
        }
        protected sealed override GraphPanel initPanel() {
            GraphPanel panel = new MeasureGraphPanel();
			return panel;
		}
        internal void startScan() {
            // TODO: different types of panel
            preciseSpecterDisplayed = false;
            (Panel as MeasureGraphPanel).overview_button_Click();
        }
        internal void startPrecise() {
            // TODO: different types of panel
            preciseSpecterDisplayed = true;
            (Panel as MeasureGraphPanel).sensmeasure_button_Click();
        }
        internal void startMonitor() {
            // TODO: move to other class
            (Panel as MeasureGraphPanel).monitorToolStripButton_Click();
        }
        internal void Enable() {
            setXScaleLimits();

            Panel.Enable();

            specterSavingEnabled = false;
            Graph.Instance.ResetPointLists();
        }
        internal void Disable() {
            Panel.Disable();
            specterSavingEnabled = true;
        }
        
        protected sealed override void doSmthMore() {
            // TODO: simplify code below
            (Panel as MeasureGraphPanel).RefreshGraph();
            Commander.CurrentMeasureMode.refreshGraphics(this);
        }
        protected sealed override void saveData() {
            saveSpecterFileDialog.FileName = "";
			base.saveData();        
		}

        internal void refreshGraphicsOnScanStep() {
            yAxisChange();
            (Panel as MeasureGraphPanel).refreshGraphicsOnScanStep();
        }
        internal void refreshGraphicsOnPreciseStep() {
            (Panel as MeasureGraphPanel).refreshGraphicsOnPreciseStep();
        }
        internal void refreshGraphicsOnMonitorStep() {
            //TODO: this is temporary
            refreshGraphicsOnPreciseStep();
        }
    }
}

