using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Flavor.Controls;
using Flavor.Common;

namespace Flavor.Forms {
    internal abstract partial class GraphForm: Form {
        private GraphPanel panel = null;
        protected GraphPanel Panel {
            get {
                if (panel == null) {
					panel = initPanel();
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
				return panel;
            }
            /*set {
                this.Controls.Remove(panel);
                panel = value;
                this.Controls.Add(panel);
			}*/
        }
		protected abstract GraphPanel initPanel();

		internal GraphForm() {
            InitializeComponent();
        }
        private void GraphForm_Load(object sender, EventArgs e) {
            CreateGraph();
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
            // TODO: subclass-dependent on closing! also mini/max preventing!
			this.Close();
        }
        private void measurePanelToolStripMenuItem_CheckedChanged(object sender, EventArgs e) {
            Panel.Visible = measurePanelToolStripMenuItem.Checked;
            SetSize();
        }

        protected abstract void RefreshGraph();
        protected abstract void CreateGraph();
        protected abstract void SetSize();
        protected abstract void saveData();
    }
}