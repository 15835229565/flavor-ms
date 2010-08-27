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
            //this.SuspendLayout();
            // 
            // graphFormMenuStrip
            // 
            graphFormMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            FileMenu,
            viewMenu,
            editMenu});
            graphFormMenuStrip.Location = new System.Drawing.Point(0, 0);
            graphFormMenuStrip.Name = "graphFormMenuStrip";
            graphFormMenuStrip.Size = new System.Drawing.Size(773, 24);
            graphFormMenuStrip.TabIndex = 11;
            graphFormMenuStrip.Text = "menuStrip1";
            graphFormMenuStrip.Visible = false;
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
            FileMenu.Size = new System.Drawing.Size(61, 20);
            FileMenu.Text = "&Главное";
            FileMenu.Visible = false;
            // 
            // closeSpecterFileToolStripMenuItem
            // 
            this.closeSpecterFileToolStripMenuItem.Enabled = false;
            this.closeSpecterFileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.closeSpecterFileToolStripMenuItem.MergeIndex = 1;
            this.closeSpecterFileToolStripMenuItem.Name = "closeSpecterFileToolStripMenuItem";
            this.closeSpecterFileToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.closeSpecterFileToolStripMenuItem.Text = "Закрыть файл спектра";
            this.closeSpecterFileToolStripMenuItem.Click += new System.EventHandler(this.closeSpecterFileToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.saveToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveToolStripMenuItem.MergeIndex = 2;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.saveToolStripMenuItem.Text = "&Сохранить спектр";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.MergeAction = System.Windows.Forms.MergeAction.Insert;
            toolStripSeparator4.MergeIndex = 3;
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(213, 6);
            // 
            // printToolStripMenuItem
            // 
            printToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("printToolStripMenuItem.Image")));
            printToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            printToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            printToolStripMenuItem.MergeIndex = 4;
            printToolStripMenuItem.Name = "printToolStripMenuItem";
            printToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            printToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            printToolStripMenuItem.Text = "&Print";
            printToolStripMenuItem.Visible = false;
            // 
            // printPreviewToolStripMenuItem
            // 
            printPreviewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("printPreviewToolStripMenuItem.Image")));
            printPreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            printPreviewToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            printPreviewToolStripMenuItem.MergeIndex = 5;
            printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            printPreviewToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            printPreviewToolStripMenuItem.Text = "Print Pre&view";
            printPreviewToolStripMenuItem.Visible = false;
            // 
            // printSetupToolStripMenuItem
            // 
            printSetupToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            printSetupToolStripMenuItem.MergeIndex = 6;
            printSetupToolStripMenuItem.Name = "printSetupToolStripMenuItem";
            printSetupToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            printSetupToolStripMenuItem.Text = "Print Setup";
            printSetupToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.MergeAction = System.Windows.Forms.MergeAction.Insert;
            toolStripSeparator5.MergeIndex = 7;
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(213, 6);
            toolStripSeparator5.Visible = false;
            // 
            // viewMenu
            // 
            viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.measurePanelToolStripMenuItem});
            viewMenu.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            viewMenu.Name = "viewMenu";
            viewMenu.Size = new System.Drawing.Size(38, 20);
            viewMenu.Text = "&Вид";
            // 
            // measurePanelToolStripMenuItem
            // 
            this.measurePanelToolStripMenuItem.CheckOnClick = true;
            this.measurePanelToolStripMenuItem.Name = "measurePanelToolStripMenuItem";
            this.measurePanelToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.measurePanelToolStripMenuItem.Text = "Панель режима";
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
            editMenu.Size = new System.Drawing.Size(37, 20);
            editMenu.Text = "&Edit";
            editMenu.Visible = false;
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("undoToolStripMenuItem.Image")));
            undoToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            undoToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            undoToolStripMenuItem.Text = "&Undo";
            undoToolStripMenuItem.Visible = false;
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("redoToolStripMenuItem.Image")));
            redoToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            redoToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            redoToolStripMenuItem.Text = "&Redo";
            redoToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(164, 6);
            toolStripSeparator6.Visible = false;
            // 
            // cutToolStripMenuItem
            // 
            cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
            cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            cutToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            cutToolStripMenuItem.Text = "Cu&t";
            cutToolStripMenuItem.Visible = false;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            copyToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            copyToolStripMenuItem.Text = "&Copy";
            copyToolStripMenuItem.Visible = false;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
            pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            pasteToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            pasteToolStripMenuItem.Text = "&Paste";
            pasteToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new System.Drawing.Size(164, 6);
            toolStripSeparator7.Visible = false;
            // 
            // selectAllToolStripMenuItem
            // 
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            selectAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            selectAllToolStripMenuItem.Text = "Select &All";
            selectAllToolStripMenuItem.Visible = false;
            // 
            // GraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 691);
            this.ControlBox = false;
            this.Controls.Add(graphFormMenuStrip);
            this.MainMenuStrip = graphFormMenuStrip;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GraphForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.GraphForm_Load);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.GraphForm_Validating);
            this.Resize += new System.EventHandler(this.GraphForm_Resize);
            graphFormMenuStrip.ResumeLayout(false);
            graphFormMenuStrip.PerformLayout();
            //this.ResumeLayout(false);
            //this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        protected System.Windows.Forms.ToolStripMenuItem closeSpecterFileToolStripMenuItem;
        protected System.Windows.Forms.ToolStripMenuItem measurePanelToolStripMenuItem;
    }
}