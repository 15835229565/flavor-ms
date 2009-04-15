namespace Flavor
{
    partial class GraphForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphForm));
            this.graphFormMenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.openSpecterFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeSpecterFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.editMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveGraphAsImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveGraphDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphPageSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unZoomAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultScaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSpecterFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openSpecterFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.distractFromCurrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collect2_graph = new Flavor.ZedGraphControlPlus();
            this.collect1_graph = new Flavor.ZedGraphControlPlus();
            this.graphFormMenuStrip.SuspendLayout();
            this.graphContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // graphFormMenuStrip
            // 
            this.graphFormMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.editMenu});
            this.graphFormMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.graphFormMenuStrip.Name = "graphFormMenuStrip";
            this.graphFormMenuStrip.Size = new System.Drawing.Size(773, 24);
            this.graphFormMenuStrip.TabIndex = 11;
            this.graphFormMenuStrip.Text = "menuStrip1";
            this.graphFormMenuStrip.Visible = false;
            // 
            // FileMenu
            // 
            this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSpecterFileToolStripMenuItem,
            this.distractFromCurrentToolStripMenuItem,
            this.closeSpecterFileToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator4,
            this.printToolStripMenuItem,
            this.printPreviewToolStripMenuItem,
            this.printSetupToolStripMenuItem,
            this.toolStripSeparator5});
            this.FileMenu.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.FileMenu.MergeIndex = 0;
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(61, 20);
            this.FileMenu.Text = "&Главное";
            this.FileMenu.Visible = false;
            // 
            // openSpecterFileToolStripMenuItem
            // 
            this.openSpecterFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openSpecterFileToolStripMenuItem.Image")));
            this.openSpecterFileToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.openSpecterFileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.openSpecterFileToolStripMenuItem.MergeIndex = 0;
            this.openSpecterFileToolStripMenuItem.Name = "openSpecterFileToolStripMenuItem";
            this.openSpecterFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openSpecterFileToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.openSpecterFileToolStripMenuItem.Text = "&Открыть файл спектра";
            this.openSpecterFileToolStripMenuItem.Click += new System.EventHandler(this.openSpecterFileToolStripMenuItem_Click);
            // 
            // closeSpecterFileToolStripMenuItem
            // 
            this.closeSpecterFileToolStripMenuItem.Enabled = false;
            this.closeSpecterFileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.closeSpecterFileToolStripMenuItem.MergeIndex = 2;
            this.closeSpecterFileToolStripMenuItem.Name = "closeSpecterFileToolStripMenuItem";
            this.closeSpecterFileToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.closeSpecterFileToolStripMenuItem.Text = "Закрыть файл спектра";
            this.closeSpecterFileToolStripMenuItem.Click += new System.EventHandler(this.closeSpecterFileToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.saveToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveToolStripMenuItem.MergeIndex = 3;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.saveToolStripMenuItem.Text = "&Сохранить спектр";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator4.MergeIndex = 4;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(241, 6);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("printToolStripMenuItem.Image")));
            this.printToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.printToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.printToolStripMenuItem.MergeIndex = 5;
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.printToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.printToolStripMenuItem.Text = "&Print";
            this.printToolStripMenuItem.Visible = false;
            // 
            // printPreviewToolStripMenuItem
            // 
            this.printPreviewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("printPreviewToolStripMenuItem.Image")));
            this.printPreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.printPreviewToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.printPreviewToolStripMenuItem.MergeIndex = 6;
            this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            this.printPreviewToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.printPreviewToolStripMenuItem.Text = "Print Pre&view";
            this.printPreviewToolStripMenuItem.Visible = false;
            // 
            // printSetupToolStripMenuItem
            // 
            this.printSetupToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.printSetupToolStripMenuItem.MergeIndex = 7;
            this.printSetupToolStripMenuItem.Name = "printSetupToolStripMenuItem";
            this.printSetupToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.printSetupToolStripMenuItem.Text = "Print Setup";
            this.printSetupToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator5.MergeIndex = 8;
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(241, 6);
            this.toolStripSeparator5.Visible = false;
            // 
            // editMenu
            // 
            this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator6,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator7,
            this.selectAllToolStripMenuItem});
            this.editMenu.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.editMenu.MergeIndex = 1;
            this.editMenu.Name = "editMenu";
            this.editMenu.Size = new System.Drawing.Size(37, 20);
            this.editMenu.Text = "&Edit";
            this.editMenu.Visible = false;
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("undoToolStripMenuItem.Image")));
            this.undoToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            this.undoToolStripMenuItem.Visible = false;
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("redoToolStripMenuItem.Image")));
            this.redoToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.redoToolStripMenuItem.Text = "&Redo";
            this.redoToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(164, 6);
            this.toolStripSeparator6.Visible = false;
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
            this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            this.cutToolStripMenuItem.Visible = false;
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Visible = false;
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
            this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            this.pasteToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(164, 6);
            this.toolStripSeparator7.Visible = false;
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            this.selectAllToolStripMenuItem.Visible = false;
            // 
            // graphContextMenuStrip
            // 
            this.graphContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyGraphToolStripMenuItem,
            this.saveGraphAsImageToolStripMenuItem,
            this.saveGraphDataToolStripMenuItem,
            this.graphPageSetupToolStripMenuItem,
            this.printGraphToolStripMenuItem,
            this.showValuesToolStripMenuItem,
            this.unZoomToolStripMenuItem,
            this.unZoomAllToolStripMenuItem,
            this.defaultScaleToolStripMenuItem});
            this.graphContextMenuStrip.Name = "graphContextMenuStrip";
            this.graphContextMenuStrip.Size = new System.Drawing.Size(287, 202);
            // 
            // copyGraphToolStripMenuItem
            // 
            this.copyGraphToolStripMenuItem.Name = "copyGraphToolStripMenuItem";
            this.copyGraphToolStripMenuItem.Size = new System.Drawing.Size(286, 22);
            this.copyGraphToolStripMenuItem.Text = "Копировать";
            // 
            // saveGraphAsImageToolStripMenuItem
            // 
            this.saveGraphAsImageToolStripMenuItem.Name = "saveGraphAsImageToolStripMenuItem";
            this.saveGraphAsImageToolStripMenuItem.Size = new System.Drawing.Size(286, 22);
            this.saveGraphAsImageToolStripMenuItem.Text = "Сохранить как изображение...";
            // 
            // saveGraphDataToolStripMenuItem
            // 
            this.saveGraphDataToolStripMenuItem.Name = "saveGraphDataToolStripMenuItem";
            this.saveGraphDataToolStripMenuItem.Size = new System.Drawing.Size(286, 22);
            this.saveGraphDataToolStripMenuItem.Text = "Сохранить данные...";
            // 
            // graphPageSetupToolStripMenuItem
            // 
            this.graphPageSetupToolStripMenuItem.Name = "graphPageSetupToolStripMenuItem";
            this.graphPageSetupToolStripMenuItem.Size = new System.Drawing.Size(286, 22);
            this.graphPageSetupToolStripMenuItem.Text = "Параметры страницы...";
            // 
            // printGraphToolStripMenuItem
            // 
            this.printGraphToolStripMenuItem.Name = "printGraphToolStripMenuItem";
            this.printGraphToolStripMenuItem.Size = new System.Drawing.Size(286, 22);
            this.printGraphToolStripMenuItem.Text = "Печать...";
            // 
            // showValuesToolStripMenuItem
            // 
            this.showValuesToolStripMenuItem.Name = "showValuesToolStripMenuItem";
            this.showValuesToolStripMenuItem.Size = new System.Drawing.Size(286, 22);
            this.showValuesToolStripMenuItem.Text = "Показать значения";
            // 
            // unZoomToolStripMenuItem
            // 
            this.unZoomToolStripMenuItem.Name = "unZoomToolStripMenuItem";
            this.unZoomToolStripMenuItem.Size = new System.Drawing.Size(286, 22);
            this.unZoomToolStripMenuItem.Text = "Отменить последнее масштабирование";
            // 
            // unZoomAllToolStripMenuItem
            // 
            this.unZoomAllToolStripMenuItem.Name = "unZoomAllToolStripMenuItem";
            this.unZoomAllToolStripMenuItem.Size = new System.Drawing.Size(286, 22);
            this.unZoomAllToolStripMenuItem.Text = "Начальный масштаб";
            // 
            // defaultScaleToolStripMenuItem
            // 
            this.defaultScaleToolStripMenuItem.Name = "defaultScaleToolStripMenuItem";
            this.defaultScaleToolStripMenuItem.Size = new System.Drawing.Size(286, 22);
            this.defaultScaleToolStripMenuItem.Text = "Установить масштаб по умолчанию";
            // 
            // saveSpecterFileDialog
            // 
            this.saveSpecterFileDialog.Filter = "Specter data files (*.sdf)|*.sdf|Precise specter files (*.psf)|*.psf";
            this.saveSpecterFileDialog.InitialDirectory = ".";
            // 
            // openSpecterFileDialog
            // 
            this.openSpecterFileDialog.Filter = "Specter data files (*.sdf)|*.sdf|Precise specter files (*.psf)|*.psf";
            this.openSpecterFileDialog.InitialDirectory = ".";
            // 
            // distractFromCurrentToolStripMenuItem
            // 
            this.distractFromCurrentToolStripMenuItem.Enabled = false;
            this.distractFromCurrentToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.distractFromCurrentToolStripMenuItem.MergeIndex = 1;
            this.distractFromCurrentToolStripMenuItem.Name = "distractFromCurrentToolStripMenuItem";
            this.distractFromCurrentToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.distractFromCurrentToolStripMenuItem.Text = "Вычесть из текущего";
            this.distractFromCurrentToolStripMenuItem.Click += new System.EventHandler(this.distractFromCurrentToolStripMenuItem_Click);
            // 
            // collect2_graph
            // 
            this.collect2_graph.EditButtons = System.Windows.Forms.MouseButtons.None;
            this.collect2_graph.EditModifierKeys = System.Windows.Forms.Keys.None;
            this.collect2_graph.IsShowPointValues = true;
            this.collect2_graph.Location = new System.Drawing.Point(12, 210);
            this.collect2_graph.Name = "collect2_graph";
            this.collect2_graph.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.collect2_graph.ScrollGrace = 0;
            this.collect2_graph.ScrollMaxX = 1056;
            this.collect2_graph.ScrollMaxY = 2000000;
            this.collect2_graph.ScrollMaxY2 = 0;
            this.collect2_graph.ScrollMinX = 0;
            this.collect2_graph.ScrollMinY = 0;
            this.collect2_graph.ScrollMinY2 = 0;
            this.collect2_graph.Size = new System.Drawing.Size(281, 192);
            this.collect2_graph.TabIndex = 10;
            // 
            // collect1_graph
            // 
            this.collect1_graph.EditButtons = System.Windows.Forms.MouseButtons.None;
            this.collect1_graph.EditModifierKeys = System.Windows.Forms.Keys.None;
            this.collect1_graph.IsShowPointValues = true;
            this.collect1_graph.Location = new System.Drawing.Point(12, 12);
            this.collect1_graph.Name = "collect1_graph";
            this.collect1_graph.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.collect1_graph.ScrollGrace = 0;
            this.collect1_graph.ScrollMaxX = 1056;
            this.collect1_graph.ScrollMaxY = 2000000;
            this.collect1_graph.ScrollMaxY2 = 0;
            this.collect1_graph.ScrollMinX = 0;
            this.collect1_graph.ScrollMinY = 0;
            this.collect1_graph.ScrollMinY2 = 0;
            this.collect1_graph.Size = new System.Drawing.Size(281, 192);
            this.collect1_graph.TabIndex = 9;
            // 
            // GraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 691);
            this.ControlBox = false;
            this.Controls.Add(this.collect2_graph);
            this.Controls.Add(this.collect1_graph);
            this.Controls.Add(this.graphFormMenuStrip);
            this.MainMenuStrip = this.graphFormMenuStrip;
            this.MinimizeBox = false;
            this.Name = "GraphForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Коллекторы";
            this.Load += new System.EventHandler(this.GraphForm_Load);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.GraphForm_Validating);
            this.Resize += new System.EventHandler(this.GraphForm_Resize);
            this.graphFormMenuStrip.ResumeLayout(false);
            this.graphFormMenuStrip.PerformLayout();
            this.graphContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public ZedGraphControlPlus collect2_graph;
        public ZedGraphControlPlus collect1_graph;
        private System.Windows.Forms.MenuStrip graphFormMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem editMenu;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeSpecterFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printPreviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printSetupToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ContextMenuStrip graphContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveGraphAsImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveGraphDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem graphPageSetupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unZoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unZoomAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultScaleToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveSpecterFileDialog;
        private System.Windows.Forms.ToolStripMenuItem openSpecterFileToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openSpecterFileDialog;
        private System.Windows.Forms.ToolStripMenuItem distractFromCurrentToolStripMenuItem;
    }
}