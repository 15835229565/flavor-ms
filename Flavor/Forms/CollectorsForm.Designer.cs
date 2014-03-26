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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectorsForm2));
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
            resources.ApplyResources(this.collect2_graph, "collect2_graph");
            this.collect2_graph.Name = "collect2_graph";
            this.collect2_graph.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.ZedGraphControlPlus_PointValueEvent);
            this.collect2_graph.ContextMenuBuilder += new System.EventHandler<ZedGraphControlPlus.ContextMenuBuilderEventArgs>(this.ZedGraphControlPlus_ContextMenuBuilder);
            // 
            // collect1_graph
            // 
            resources.ApplyResources(this.collect1_graph, "collect1_graph");
            this.collect1_graph.Name = "collect1_graph";
            this.collect1_graph.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.ZedGraphControlPlus_PointValueEvent);
            this.collect1_graph.ContextMenuBuilder += new System.EventHandler<ZedGraphControlPlus.ContextMenuBuilderEventArgs>(this.ZedGraphControlPlus_ContextMenuBuilder);
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
