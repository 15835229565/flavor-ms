using Flavor.Controls;

namespace Flavor.Forms {
    partial class GraphForm {
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
            if (disposing && (panel != null)) {
                panel.Dispose();
            }
		}

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.MenuStrip graphFormMenuStrip;
            System.Windows.Forms.ToolStripMenuItem FileMenu;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphForm));
            System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
            System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem printPreviewToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem printSetupToolStripMenuItem;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
            System.Windows.Forms.ToolStripMenuItem viewMenu;
            System.Windows.Forms.ToolStripMenuItem editMenu;
            System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
            System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
            System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
            this.closeSpecterFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.measurePanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            graphFormMenuStrip = new System.Windows.Forms.MenuStrip();
            FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            printSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            editMenu = new System.Windows.Forms.ToolStripMenuItem();
            undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            graphFormMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // graphFormMenuStrip
            // 
            graphFormMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            FileMenu,
            viewMenu,
            editMenu});
            resources.ApplyResources(graphFormMenuStrip, "graphFormMenuStrip");
            graphFormMenuStrip.Name = "graphFormMenuStrip";
            // 
            // FileMenu
            // 
            FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeSpecterFileToolStripMenuItem,
            this.saveToolStripMenuItem,
            toolStripSeparator4,
            printToolStripMenuItem,
            printPreviewToolStripMenuItem,
            printSetupToolStripMenuItem,
            toolStripSeparator5});
            FileMenu.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            FileMenu.MergeIndex = 0;
            FileMenu.Name = "FileMenu";
            resources.ApplyResources(FileMenu, "FileMenu");
            // 
            // closeSpecterFileToolStripMenuItem
            // 
            resources.ApplyResources(this.closeSpecterFileToolStripMenuItem, "closeSpecterFileToolStripMenuItem");
            this.closeSpecterFileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.closeSpecterFileToolStripMenuItem.MergeIndex = 1;
            this.closeSpecterFileToolStripMenuItem.Name = "closeSpecterFileToolStripMenuItem";
            this.closeSpecterFileToolStripMenuItem.Click += new System.EventHandler(this.closeSpecterFileToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveToolStripMenuItem.MergeIndex = 2;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.MergeAction = System.Windows.Forms.MergeAction.Insert;
            toolStripSeparator4.MergeIndex = 3;
            toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(toolStripSeparator4, "toolStripSeparator4");
            // 
            // printToolStripMenuItem
            // 
            resources.ApplyResources(printToolStripMenuItem, "printToolStripMenuItem");
            printToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            printToolStripMenuItem.MergeIndex = 4;
            printToolStripMenuItem.Name = "printToolStripMenuItem";
            // 
            // printPreviewToolStripMenuItem
            // 
            resources.ApplyResources(printPreviewToolStripMenuItem, "printPreviewToolStripMenuItem");
            printPreviewToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            printPreviewToolStripMenuItem.MergeIndex = 5;
            printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            // 
            // printSetupToolStripMenuItem
            // 
            printSetupToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            printSetupToolStripMenuItem.MergeIndex = 6;
            printSetupToolStripMenuItem.Name = "printSetupToolStripMenuItem";
            resources.ApplyResources(printSetupToolStripMenuItem, "printSetupToolStripMenuItem");
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.MergeAction = System.Windows.Forms.MergeAction.Insert;
            toolStripSeparator5.MergeIndex = 7;
            toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(toolStripSeparator5, "toolStripSeparator5");
            // 
            // viewMenu
            // 
            viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.measurePanelToolStripMenuItem});
            viewMenu.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            viewMenu.Name = "viewMenu";
            resources.ApplyResources(viewMenu, "viewMenu");
            // 
            // measurePanelToolStripMenuItem
            // 
            this.measurePanelToolStripMenuItem.Checked = true;
            this.measurePanelToolStripMenuItem.CheckOnClick = true;
            this.measurePanelToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.measurePanelToolStripMenuItem.Name = "measurePanelToolStripMenuItem";
            resources.ApplyResources(this.measurePanelToolStripMenuItem, "measurePanelToolStripMenuItem");
            this.measurePanelToolStripMenuItem.CheckedChanged += new System.EventHandler(this.measurePanelToolStripMenuItem_CheckedChanged);
            // 
            // editMenu
            // 
            editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            undoToolStripMenuItem,
            redoToolStripMenuItem,
            toolStripSeparator6,
            cutToolStripMenuItem,
            copyToolStripMenuItem,
            pasteToolStripMenuItem,
            toolStripSeparator7,
            selectAllToolStripMenuItem});
            editMenu.MergeAction = System.Windows.Forms.MergeAction.Insert;
            editMenu.MergeIndex = 1;
            editMenu.Name = "editMenu";
            resources.ApplyResources(editMenu, "editMenu");
            // 
            // undoToolStripMenuItem
            // 
            resources.ApplyResources(undoToolStripMenuItem, "undoToolStripMenuItem");
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            // 
            // redoToolStripMenuItem
            // 
            resources.ApplyResources(redoToolStripMenuItem, "redoToolStripMenuItem");
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(toolStripSeparator6, "toolStripSeparator6");
            // 
            // cutToolStripMenuItem
            // 
            resources.ApplyResources(cutToolStripMenuItem, "cutToolStripMenuItem");
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            // 
            // copyToolStripMenuItem
            // 
            resources.ApplyResources(copyToolStripMenuItem, "copyToolStripMenuItem");
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            // 
            // pasteToolStripMenuItem
            // 
            resources.ApplyResources(pasteToolStripMenuItem, "pasteToolStripMenuItem");
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(toolStripSeparator7, "toolStripSeparator7");
            // 
            // selectAllToolStripMenuItem
            // 
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            resources.ApplyResources(selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
            // 
            // GraphForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(graphFormMenuStrip);
            this.MainMenuStrip = graphFormMenuStrip;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GraphForm";
            this.ShowIcon = false;
            this.Shown += new System.EventHandler(this.GraphForm_Shown);
            this.Activated += new System.EventHandler(this.GraphForm_Activated);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.GraphForm_Validating);
            this.Resize += new System.EventHandler(this.GraphForm_Resize);
            graphFormMenuStrip.ResumeLayout(false);
            graphFormMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        protected System.Windows.Forms.ToolStripMenuItem closeSpecterFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem measurePanelToolStripMenuItem;
    }
}