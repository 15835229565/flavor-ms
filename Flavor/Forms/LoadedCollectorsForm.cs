using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Forms {
    internal partial class LoadedCollectorsForm: CollectorsForm, ILoaded {
        private string displayedFileName;
        public LoadedCollectorsForm(bool isPrecise, Graph graph)
            : base(isPrecise, graph) {
            InitializeComponent();
            graph.DisplayingMode = Graph.Displaying.Loaded;
            specterClosingEnabled = true;
        }
        public void DisplayLoadedSpectrum() {
            DisplayLoadedSpectrum(displayedFileName);
        }
        public void DisplayLoadedSpectrum(string fileName) {
            displayedFileName = fileName;
            this.Text = displayedFileName;
			CreateGraph();
        }
        protected sealed override bool Graph_OnAxisModeChanged() {
            if (base.Graph_OnAxisModeChanged()) {
				return false;
			}
            DisplayLoadedSpectrum(displayedFileName);
			return true;
		}
        protected sealed override void saveData() {
			saveSpecterFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(displayedFileName);
			base.saveData();
		}
    }
}

