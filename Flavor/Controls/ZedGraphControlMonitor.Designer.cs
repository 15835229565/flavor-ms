namespace Flavor.Controls {
    partial class ZedGraphControlMonitor {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZedGraphControlMonitor));
            this.SuspendLayout();
            // 
            // ZedGraphControlMonitor
            // 
            resources.ApplyResources(this, "$this");
            this.IsShowPointValues = true;
            this.Name = "ZedGraphControlMonitor";
            this.ContextMenuBuilder += new ZedGraph.ZedGraphControl.ContextMenuBuilderEventHandler(this.ZedGraphControlMonitor_ContextMenuBuilder);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
