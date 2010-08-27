using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Flavor.Controls;
using Flavor.Common;

namespace Flavor.Forms {
    internal abstract partial class GraphForm: Form {
        private MeasurePanel panel = null;
        public MeasurePanel Panel {
            get {
                return panel;
            }
            protected set {
                this.Controls.Remove(panel);
                panel = value;
                // 
                // panel
                // 
                panel.BackColor = System.Drawing.SystemColors.Control;
                panel.Dock = System.Windows.Forms.DockStyle.Right;
                panel.Location = new System.Drawing.Point(493, 24);
                panel.Name = "panel";
                panel.Size = new System.Drawing.Size(280, 667);
                panel.TabIndex = 18;
                panel.Visible = this.measurePanelToolStripMenuItem.Checked;
                
                this.Controls.Add(panel);
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
        private void measurePanelToolStripMenuItem_CheckedChanged(object sender, EventArgs e) {
            panel.Visible = measurePanelToolStripMenuItem.Checked;
            SetSize();
        }

        internal abstract void RefreshGraph();
        internal abstract void CreateGraph();
        protected abstract void SetSize();
        protected abstract void saveData();
    }
}