using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Flavor.Controls;
using Flavor.Common;

namespace Flavor.Forms {
    internal partial class GraphForm: Form {
        protected const int HORIZ_GRAPH_INDENT = 12;
        protected const int VERT_GRAPH_INDENT = 12;
        protected readonly Color[] rowsColors = { Color.Blue, Color.Red, Color.Green, Color.Orange, Color.DarkViolet, Color.DeepPink,
        Color.Black, Color.Magenta,};
        protected const string Y_AXIS_TITLE = "Интенсивность";

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
                    panel.Size = new System.Drawing.Size(280, 667);
                    panel.Visible = panel.Enabled && this.measurePanelToolStripMenuItem.Checked;
                    panel.EnabledChanged += new EventHandler(toggleMeasurePanel);
                
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

        /*public new FormWindowState WindowState {
            get { return base.WindowState; }
            set { base.WindowState = FormWindowState.Maximized; }
        }*/
        
        internal protected GraphForm() {
            InitializeComponent();
            //Dock = DockStyle.Fill;
            //FormBorderStyle = FormBorderStyle.None;
        }
        /*protected override void OnCreateControl() {
            ControlBox = false;
            base.OnCreateControl();
        }*/
        private void GraphForm_Shown(object sender, EventArgs e) {
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
			Close();
        }
        private void measurePanelToolStripMenuItem_CheckedChanged(object sender, EventArgs e) {
            Panel.Visible = Panel.Enabled && measurePanelToolStripMenuItem.Checked;
            SetSize();
        }

        private void toggleMeasurePanel(object sender, EventArgs e) {
            measurePanelToolStripMenuItem.Visible = Panel.Enabled;
            measurePanelToolStripMenuItem_CheckedChanged(sender, e);
        }

        protected virtual GraphPanel initPanel() { return null; }

        protected virtual void RefreshGraph() {}
        protected virtual void CreateGraph() {}
        protected virtual void SetSize() {}
        protected virtual bool saveData() { return false; }

        private void GraphForm_Activated(object sender, EventArgs e) {
            WindowState = FormWindowState.Maximized;
        }
    }
}