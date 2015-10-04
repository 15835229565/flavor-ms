using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Flavor.Controls;

namespace Flavor.Forms {
    partial class GraphForm: Form {
        const int PANEL_WIDTH = 280;
        protected readonly Color[] rowsColors = 
            { Color.Blue, Color.Red, Color.Green, Color.DarkOrange, Color.DarkViolet,
            Color.DeepPink, Color.Black, Color.Magenta, Color.Teal, Color.Tomato,
            Color.Indigo, Color.Fuchsia, Color.DarkKhaki, Color.Goldenrod, Color.Navy,
            Color.Lime, Color.SaddleBrown, Color.Maroon, Color.DeepSkyBlue, Color.DimGray,};
        protected readonly string Y_AXIS_TITLE = Resources.GraphForm_YAxisTitle;

        GraphPanel panel;
        protected GraphPanel Panel {
            get {
				return panel;
            }
            set {
                if (panel == value)
                    return;
                if (panel != null) {
                    panel.EnabledChanged -= toggleMeasurePanel;
                    Controls.Remove(panel);
                }
                
                panel = value;
                Controls.Add(panel);
                panel.Width = PANEL_WIDTH;
                panel.Dock = DockStyle.Right;
                panel.Visible = panel.Enabled && measurePanelToolStripMenuItem.Checked;
                panel.EnabledChanged += toggleMeasurePanel;
            }
        }

        internal protected GraphForm()
            : base() {
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
        void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            saveData();
        }
        void closeSpecterFileToolStripMenuItem_Click(object sender, EventArgs e) {
			Close();
        }
        void measurePanelToolStripMenuItem_CheckedChanged(object sender, EventArgs e) {
            Panel.Visible = Panel.Enabled && measurePanelToolStripMenuItem.Checked;
            SetSize();
        }

        void toggleMeasurePanel(object sender, EventArgs e) {
            measurePanelToolStripMenuItem.Visible = Panel.Enabled;
            measurePanelToolStripMenuItem_CheckedChanged(sender, e);
        }

        protected virtual void RefreshGraph() {}
        protected virtual void CreateGraph() {}
        protected virtual void SetSize() {}
        protected virtual bool saveData() { return false; }
    }
}