using Flavor.Controls;
namespace Flavor.Forms {
    partial class CollectorsForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectorsForm));
            this.saveSpecterFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openSpecterFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.collect2_graph = new Flavor.Controls.ZedGraphControlPlus();
            this.collect1_graph = new Flavor.Controls.ZedGraphControlPlus();
            this.distractFromCurrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SuspendLayout();
            // 
            // saveSpecterFileDialog
            // 
            this.saveSpecterFileDialog.InitialDirectory = ".";
            // 
            // openSpecterFileDialog
            // 
            this.openSpecterFileDialog.InitialDirectory = ".";
            // 
            // collect2_graph
            // 
            this.collect2_graph.EditButtons = System.Windows.Forms.MouseButtons.None;
            this.collect2_graph.EditModifierKeys = System.Windows.Forms.Keys.None;
            this.collect2_graph.IsEnableSelection = true;
            this.collect2_graph.IsShowPointValues = true;
            resources.ApplyResources(this.collect2_graph, "collect2_graph");
            this.collect2_graph.Name = "collect2_graph";
            this.collect2_graph.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.collect2_graph.ScrollGrace = 0;
            this.collect2_graph.ScrollMaxX = 0;
            this.collect2_graph.ScrollMaxY = 2000000;
            this.collect2_graph.ScrollMaxY2 = 0;
            this.collect2_graph.ScrollMinX = 0;
            this.collect2_graph.ScrollMinY = 0;
            this.collect2_graph.ScrollMinY2 = 0;
            this.collect2_graph.SelectModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)));
            this.collect2_graph.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.ZedGraphControlPlus_PointValueEvent);
            this.collect2_graph.OnPoint += new Flavor.Controls.ZedGraphControlPlus.PointEventHandler(this.GraphForm_OnPoint);
            this.collect2_graph.OnDiffOnPoint += new Flavor.Controls.ZedGraphControlPlus.DiffOnPointEventHandler(this.GraphForm_OnDiffOnPoint);
            this.collect2_graph.ContextMenuBuilder += new Flavor.Controls.ZedGraphControlPlus.ContextMenuBuilderEventHandler(this.ZedGraphControlPlus_ContextMenuBuilder);
            // 
            // collect1_graph
            // 
            this.collect1_graph.EditButtons = System.Windows.Forms.MouseButtons.None;
            this.collect1_graph.EditModifierKeys = System.Windows.Forms.Keys.None;
            this.collect1_graph.IsEnableSelection = true;
            this.collect1_graph.IsShowPointValues = true;
            resources.ApplyResources(this.collect1_graph, "collect1_graph");
            this.collect1_graph.Name = "collect1_graph";
            this.collect1_graph.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.collect1_graph.ScrollGrace = 0;
            this.collect1_graph.ScrollMaxX = 0;
            this.collect1_graph.ScrollMaxY = 2000000;
            this.collect1_graph.ScrollMaxY2 = 0;
            this.collect1_graph.ScrollMinX = 0;
            this.collect1_graph.ScrollMinY = 0;
            this.collect1_graph.ScrollMinY2 = 0;
            this.collect1_graph.SelectModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)));
            this.collect1_graph.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.ZedGraphControlPlus_PointValueEvent);
            this.collect1_graph.OnPoint += new Flavor.Controls.ZedGraphControlPlus.PointEventHandler(this.GraphForm_OnPoint);
            this.collect1_graph.OnDiffOnPoint += new Flavor.Controls.ZedGraphControlPlus.DiffOnPointEventHandler(this.GraphForm_OnDiffOnPoint);
            this.collect1_graph.ContextMenuBuilder += new Flavor.Controls.ZedGraphControlPlus.ContextMenuBuilderEventHandler(this.ZedGraphControlPlus_ContextMenuBuilder);
            // 
            // distractFromCurrentToolStripMenuItem
            // 
            resources.ApplyResources(this.distractFromCurrentToolStripMenuItem, "distractFromCurrentToolStripMenuItem");
            this.distractFromCurrentToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.distractFromCurrentToolStripMenuItem.MergeIndex = 1;
            this.distractFromCurrentToolStripMenuItem.Name = "distractFromCurrentToolStripMenuItem";
            this.distractFromCurrentToolStripMenuItem.Click += new System.EventHandler(this.distractFromCurrentToolStripMenuItem_Click);
            // 
            // CollectorsForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.collect2_graph);
            this.Controls.Add(this.collect1_graph);
            this.Name = "CollectorsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraphControlPlus collect1_graph;
        private ZedGraphControlPlus collect2_graph;
        protected System.Windows.Forms.SaveFileDialog saveSpecterFileDialog;
        private System.Windows.Forms.OpenFileDialog openSpecterFileDialog;
        private System.Windows.Forms.ToolStripMenuItem distractFromCurrentToolStripMenuItem;
    }
}
