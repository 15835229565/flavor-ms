using Flavor.Controls;

namespace Flavor.Forms {
    partial class MonitorForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitorForm));
            this.graph = new Flavor.Controls.ZedGraphControlMonitor();
            this.SuspendLayout();
            // 
            // graph
            // 
            this.graph.EditButtons = System.Windows.Forms.MouseButtons.None;
            this.graph.EditModifierKeys = System.Windows.Forms.Keys.None;
            this.graph.IsEnableSelection = true;
            this.graph.IsShowPointValues = true;
            resources.ApplyResources(this.graph, "graph");
            this.graph.Name = "graph";
            this.graph.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.graph.ScrollGrace = 0;
            this.graph.ScrollMaxX = 1056;
            this.graph.ScrollMaxY = 2000000;
            this.graph.ScrollMaxY2 = 0;
            this.graph.ScrollMinX = 0;
            this.graph.ScrollMinY = 0;
            this.graph.ScrollMinY2 = 0;
            this.graph.SelectModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)));
            this.graph.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.ZedGraphControlMonitor_PointValueEvent);
            this.graph.ContextMenuBuilder += new ZedGraph.ZedGraphControl.ContextMenuBuilderEventHandler(this.ZedGraphControlMonitor_ContextMenuBuilder);
            // 
            // MonitorForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.graph);
            this.Name = "MonitorForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraphControlMonitor graph;
        private System.ComponentModel.IContainer components;
    }
}
