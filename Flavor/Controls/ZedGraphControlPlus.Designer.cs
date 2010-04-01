namespace Flavor.Controls
{
    partial class ZedGraphControlPlus
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ZedGraphControlPlus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.IsShowPointValues = true;
            this.Name = "ZedGraphControlPlus";
            this.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.ZedGraphControlPlus_PointValueEvent);
            this.ContextMenuBuilder += new ZedGraph.ZedGraphControl.ContextMenuBuilderEventHandler(this.ZedGraphControlPlus_ContextMenuBuilder);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
