using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Flavor.Common;
using Flavor.Controls;

namespace Flavor.Forms {
    internal partial class LoadedCollectorsForm: CollectorsForm, ILoaded {
        private string displayedFileName;
        public LoadedCollectorsForm(Graph graph, string fileName, bool hint)
            : base(!Config.openSpectrumFile(fileName, hint, graph), graph) {
            InitializeComponent();
            this.Text = displayedFileName = fileName;
            // TODO: may be Diff!
            graph.DisplayingMode = Graph.Displaying.Loaded;

            if (preciseSpecterDisplayed) {
                setXScaleLimits(Config.PreciseDataLoaded);
            } else {
                ushort minX = (ushort)(graph.Displayed1Steps[0][0].X);
                ushort maxX = (ushort)(minX - 1 + graph.Displayed1Steps[0].Count);
                setXScaleLimits(minX, maxX, minX, maxX);
            }
        }
        /*protected override MeasurePanel initPanel() {
            MeasurePanel panel = new MeasurePanel();
			//panel.Visible = true;
			return panel;
		}*/
        public void DisplayLoadedSpectrum() {
            DisplayLoadedSpectrum(displayedFileName);
        }
        private void DisplayLoadedSpectrum(string fileName) {
			CreateGraph();
        }
        protected sealed override bool axisModeChange() {
            if (base.axisModeChange()) {
				return false;
			}
            DisplayLoadedSpectrum(displayedFileName);
			return true;
		}
        protected sealed override bool refreshGraph(bool recreate) {
            if (base.refreshGraph(recreate))
                return false;
            if (recreate) {
                DisplayLoadedSpectrum();
            } else {
                RefreshGraph();
            }
            return true;
        }
        protected sealed override void saveData() {
			saveSpecterFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(displayedFileName);
			base.saveData();
		}
    }
}

