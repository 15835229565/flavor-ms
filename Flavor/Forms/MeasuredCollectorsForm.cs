using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Flavor.Common;

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
            : base(isPrecise) {
            InitializeComponent();
            Graph.Instance.DisplayingMode = Graph.Displaying.Measured;
            specterClosingEnabled = false;
        }
        protected sealed override void Graph_OnAxisModeChanged() {
            switch (Graph.Instance.DisplayingMode) {
                case Graph.Displaying.Loaded:
                    throw new InvalidOperationException("!");
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
                    throw new InvalidOperationException("!");
                /*case Graph.Displaying.Measured:
                    saveSpecterFileDialog.FileName = "";
                    break;
                case Graph.Displaying.Diff:
                    if (displayedFileName == "") {
                        saveSpecterFileDialog.FileName = "";
                        break;
                    }
                    saveSpecterFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName("");//?
                    saveSpecterFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(displayedFileName) + "~diff";
                    break;*/
            }
            saveSpecterFileDialog.FileName = "";
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

