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
            : base(isPrecise, Graph.Instance) {
            InitializeComponent();
            Graph.Instance.DisplayingMode = Graph.Displaying.Measured;
            specterClosingEnabled = false;
        }
        protected sealed override bool Graph_OnAxisModeChanged() {
            if (base.Graph_OnAxisModeChanged()) {
				return false;
			}
            CreateGraph();
			return true;
        }
        
        protected sealed override void saveData() {
            saveSpecterFileDialog.FileName = "";
			base.saveData();        
		}
    }
}

