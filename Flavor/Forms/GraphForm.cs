using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Flavor.Controls;
using Flavor.Common;

namespace Flavor.Forms {
    internal abstract partial class GraphForm: Form {
        private MeasurePanel panel;
        public MeasurePanel Panel {
            get {
                return panel;
            }
            protected set {
                panel = value;
            }
        }
		
		internal GraphForm() {
            this.InitializeComponent();
        }
        private void GraphForm_Load(object sender, EventArgs e) {
            CreateGraph();
            SetSize();
        }
        private void GraphForm_Resize(object sender, EventArgs e) {
            SetSize();
        }
        private void GraphForm_Validating(object sender, CancelEventArgs e) {
            RefreshGraph();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            saveData();
        }
        private void closeSpecterFileToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        internal abstract void RefreshGraph();
        internal abstract void CreateGraph();
        protected abstract void SetSize();
        protected abstract void saveData();
    }
}