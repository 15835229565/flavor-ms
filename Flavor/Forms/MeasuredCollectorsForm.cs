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
    internal partial class MeasuredCollectorsForm: CollectorsForm {
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
            GraphPanel panel = new MeasureGraphPanel();
			return panel;
		}
        internal void startScan() {
            // TODO: different types of panel
            PreciseSpectrumDisplayed = false;
            (Panel as MeasureGraphPanel).overview_button_Click(Config.sPoint, Config.ePoint);
        }
        internal void startPrecise() {
            // TODO: different types of panel
            PreciseSpectrumDisplayed = true;
            (Panel as MeasureGraphPanel).sensmeasure_button_Click();
        }
        internal void startMonitor() {
            // TODO: move to other class
            PreciseSpectrumDisplayed = true;
            (Panel as MeasureGraphPanel).monitorToolStripButton_Click();
        }
        internal void prepareControlsOnMeasureStart() {
            // not so good..
            if (PreciseSpectrumDisplayed)
                setXScaleLimits(Config.PreciseData);
            else
                setXScaleLimits();

            Panel.Enable();
            // TODO: and set it visible together with menu item set checked!

            specterSavingEnabled = false;
            Graph.ResetPointListsWithEvent();
        }
        internal void deactivateOnMeasureStop() {
            Panel.Disable();
            specterSavingEnabled = true;
        }
        
        protected sealed override void doSmthMore() {
            // TODO: simplify code below
            (Panel as MeasureGraphPanel).performStep();
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

        void MeasuredCollectorsForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }
    }
}

