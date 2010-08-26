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
        internal MeasuredCollectorsForm(bool isPrecise)
            : base(isPrecise, Graph.Instance) {
            InitializeComponent();
            Graph.Instance.DisplayingMode = Graph.Displaying.Measured;
        }
        internal void startScan() {
            // TODO: different types of panel
            //Panel = new MeasurePanel();
            preciseSpecterDisplayed = false;
            Panel.overview_button_Click();
        }
        internal void startPrecise() {
            // TODO: different types of panel
            //Panel = new MeasurePanel();
            preciseSpecterDisplayed = true;
            Panel.sensmeasure_button_Click();
        }
        internal void startMonitor() {
            // TODO: move to other class
            Panel.monitorToolStripButton_Click();
        }
        internal void prepareControlsOnMeasureStart() {
            setXScaleLimits();
            // TODO: make private to GraphForm
            //measurePanelToolStripMenuItem.Enabled = true;
            measurePanelToolStripMenuItem.Checked = true;

            Panel.prepareControlsOnMeasureStart();

            specterSavingEnabled = false;
            Graph.Instance.ResetPointLists();
            //CreateGraph();
        }
        internal void cancelScan() {
            Panel.cancelScan();
            //mandatory?
            measurePanelToolStripMenuItem.Checked = false;
            specterSavingEnabled = true;
        }
        
        protected sealed override bool axisModeChange() {
            if (base.axisModeChange()) {
				return false;
			}
            CreateGraph();
			return true;
        }
        protected sealed override bool refreshGraph(bool recreate) {
            if (base.refreshGraph(recreate))
                return false;
            if (recreate) {
                CreateGraph();
            } else {
                RefreshGraph();
                // TODO: simplify code below
                Panel.RefreshGraph();
                Commander.CurrentMeasureMode.refreshGraphics(this);
            }
            return true;
        }
        protected sealed override void saveData() {
            saveSpecterFileDialog.FileName = "";
			base.saveData();        
		}

        internal void refreshGraphicsOnScanStep() {
            yAxisChange();
            Panel.refreshGraphicsOnScanStep();
        }
        internal void refreshGraphicsOnPreciseStep() {
            Panel.refreshGraphicsOnPreciseStep();
        }
        internal void refreshGraphicsOnMonitorStep() {
            //TODO: this is temporary
            refreshGraphicsOnPreciseStep();
        }
    }
}

