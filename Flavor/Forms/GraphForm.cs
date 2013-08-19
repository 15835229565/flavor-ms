using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Flavor.Controls;

namespace Flavor.Forms {
    internal partial class GraphForm: Form {
        protected const int HORIZ_GRAPH_INDENT = 12;
        protected const int VERT_GRAPH_INDENT = 12;
        protected readonly Color[] rowsColors = 
            { Color.Blue, Color.Red, Color.Green, Color.DarkOrange, Color.DarkViolet,
            Color.DeepPink, Color.Black, Color.Magenta, Color.Teal, Color.Tomato,
            Color.Indigo, Color.Fuchsia, Color.DarkKhaki, Color.Goldenrod, Color.Navy,
            Color.Lime, Color.SaddleBrown, Color.Maroon, Color.DeepSkyBlue, Color.DimGray,};
        protected readonly string Y_AXIS_TITLE = Resources.GraphForm_YAxisTitle;

        private GraphPanel panel = null;
        protected GraphPanel Panel {
            get {
                if (panel == null)
                    initPanel();
				return panel;
            }
        }

        protected void initPanel() {
            if (panel != null) {
                panel.EnabledChanged -= toggleMeasurePanel;
                this.Controls.Remove(panel);
            }
            panel = newPanel();
            // 
            // panel
            // 
            panel.BackColor = System.Drawing.SystemColors.Control;
            panel.Dock = System.Windows.Forms.DockStyle.Right;
            panel.Location = new System.Drawing.Point(493, 24);
            panel.Size = new System.Drawing.Size(280, 667);
            panel.Visible = panel.Enabled && this.measurePanelToolStripMenuItem.Checked;
            panel.EnabledChanged += toggleMeasurePanel;

            this.Controls.Add(panel);
        }
        protected virtual GraphPanel newPanel() {
            return null;
        }
        
        internal protected GraphForm() {
            InitializeComponent();
        }
        protected sealed override void OnShown(EventArgs e) {
            CreateGraph();
            SetSize();
            base.OnShown(e);
        }
        protected sealed override void OnResize(EventArgs e) {
            SetSize();
            base.OnResize(e);
        }
        protected sealed override void OnValidating(CancelEventArgs e) {
            // here is null reference exception on exit under linux
			RefreshGraph();
            base.OnValidating(e);
        }
        protected sealed override void OnActivated(EventArgs e) {
            WindowState = FormWindowState.Maximized;
            base.OnActivated(e);
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

        protected virtual void RefreshGraph() {}
        protected virtual void CreateGraph() {}
        protected virtual void SetSize() {}
        protected virtual bool saveData() { return false; }
    }
}