using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Flavor.Common;

namespace Flavor.Forms {
    internal partial class LoadedCollectorsForm: CollectorsForm, ILoaded {
        private string displayedFileName;
        public LoadedCollectorsForm(bool isPrecise)
            : base(isPrecise) {
            InitializeComponent();
            Graph.Instance.DisplayingMode = Graph.Displaying.Loaded;
            specterClosingEnabled = true;
        }
        public void DisplayLoadedSpectrum() {
            DisplayLoadedSpectrum(displayedFileName);
        }
        public void DisplayLoadedSpectrum(string fileName) {
            displayedFileName = fileName;
            this.Text = displayedFileName;
            string modeText = preciseSpecterDisplayed ? " (прециз.)" : " (скан.)";

            ZedGraphRebirth(0, Graph.Instance.DisplayedRows1, COL1_TITLE + modeText);
            ZedGraphRebirth(1, Graph.Instance.DisplayedRows2, COL2_TITLE + modeText);
            RefreshGraph();
        }
        protected sealed override void Graph_OnAxisModeChanged() {
            switch (Graph.Instance.DisplayingMode) {
                case Graph.Displaying.Loaded:
                    DisplayLoadedSpectrum(displayedFileName);
                    break;
                case Graph.Displaying.Measured:
                    CreateGraph();
                    break;
                case Graph.Displaying.Diff:
                    DisplayDiff();
                    break;
            }
        }
        protected sealed override void saveData() {
            switch (Graph.Instance.DisplayingMode) {
                case Graph.Displaying.Loaded:
                    if (displayedFileName == "") {
                        throw new InvalidOperationException("!");
                    }
                    saveSpecterFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(displayedFileName);
                    saveSpecterFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(displayedFileName);
                    break;
                case Graph.Displaying.Measured:
                    throw new InvalidOperationException("!");
                case Graph.Displaying.Diff:
                    if (displayedFileName == "") {
                        throw new InvalidOperationException("!");
                    }
                    saveSpecterFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(displayedFileName);
                    saveSpecterFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(displayedFileName) + "~diff";
                    break;
            }
            if (preciseSpecterDisplayed) {
                saveSpecterFileDialog.Filter = Config.preciseSpectrumFileDialogFilter;
                saveSpecterFileDialog.DefaultExt = Config.PRECISE_SPECTRUM_EXT;
                if (saveSpecterFileDialog.ShowDialog() == DialogResult.OK) {
                    Config.SavePreciseSpecterFile(saveSpecterFileDialog.FileName, Graph.Instance.DisplayingMode);
                }
            } else {
                saveSpecterFileDialog.Filter = Config.spectrumFileDialogFilter;
                saveSpecterFileDialog.DefaultExt = Config.SPECTRUM_EXT;
                if (saveSpecterFileDialog.ShowDialog() == DialogResult.OK) {
                    Config.SaveSpecterFile(saveSpecterFileDialog.FileName, Graph.Instance.DisplayingMode);
                }
            }
        }
    }
}

