using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Flavor.Common;
using Flavor.Controls;

namespace Flavor.Forms {
    internal partial class LoadedCollectorsForm: CollectorsForm {
        private string displayedFileName;
        public LoadedCollectorsForm(Graph graph, string fileName, bool hint)
            : base(graph, hint) {
            InitializeComponent();
            this.Text = displayedFileName = fileName;

            if (PreciseSpectrumDisplayed) {
                // TODO:!
                setXScaleLimits(graph.PreciseData);
            } else {
                ushort minX = (ushort)(graph.Displayed1Steps[0][0].X);
                ushort maxX = (ushort)(minX - 1 + graph.Displayed1Steps[0].Count);
                setXScaleLimits(minX, maxX, minX, maxX);
            }

            Panel.Enable();
        }
        /*protected override MeasurePanel initPanel() {
            MeasurePanel panel = new MeasurePanel();
			return panel;
		}*/
        protected sealed override void saveData() {
			saveSpecterFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(displayedFileName);
			base.saveData();
		}
    }
}

