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
            GraphPanel panel = new MeasureGraphPanel();
            return panel;
		}
        internal void startMonitor() {
            // TODO: move to other class
            PreciseSpectrumDisplayed = true;
            (Panel as MeasureGraphPanel).monitorToolStripButton_Click();
        }
        #region IMeasured Members

        public void initMeasure(bool isPrecise) {
            // TODO: different types of panel
            PreciseSpectrumDisplayed = isPrecise;
            if (isPrecise)
                (Panel as MeasureGraphPanel).sensmeasure_button_Click();
            else
                (Panel as MeasureGraphPanel).overview_button_Click(Config.sPoint, Config.ePoint);
            Show();
            Activate();
        }
        public void prepareControlsOnMeasureStart() {
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
        public void refreshGraphicsOnMeasureStep() {
            (Panel as MeasureGraphPanel).performStep();
            if (PreciseSpectrumDisplayed)
                (Panel as MeasureGraphPanel).refreshGraphicsOnPreciseStep();
            else {
                yAxisChange();
                (Panel as MeasureGraphPanel).refreshGraphicsOnScanStep();
            }
        }
        public void deactivateOnMeasureStop() {
            Panel.Disable();
            specterSavingEnabled = true;
        }

        #endregion
        
        protected sealed override bool saveData() {
            saveSpecterFileDialog.FileName = "";
			return base.saveData();        
		}

        void MeasuredCollectorsForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }
    }
}

