using Flavor.Controls;
namespace Flavor.Forms
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
            System.Windows.Forms.MenuStrip graphFormMenuStrip;
            System.Windows.Forms.ToolStripMenuItem FileMenu;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphForm));
            System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
            System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem printPreviewToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem printSetupToolStripMenuItem;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
            System.Windows.Forms.ToolStripMenuItem editMenu;
            System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
            System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
            System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
            System.Windows.Forms.ContextMenuStrip graphContextMenuStrip;
            this.openSpecterFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distractFromCurrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeSpecterFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.collect2_graph = new Flavor.Controls.ZedGraphControlPlus();
            this.collect1_graph = new Flavor.Controls.ZedGraphControlPlus();
            graphFormMenuStrip = new System.Windows.Forms.MenuStrip();
            FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            printSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            editMenu = new System.Windows.Forms.ToolStripMenuItem();
            undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            graphContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            graphFormMenuStrip.SuspendLayout();
            graphContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // graphFormMenuStrip
            // 
            graphFormMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            FileMenu,
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
            this.openSpecterFileToolStripMenuItem,
            this.distractFromCurrentToolStripMenuItem,
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
            toolStripSeparator4.MergeAction = System.Windows.Forms.MergeAction.Insert;
            toolStripSeparator4.MergeIndex = 4;
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(241, 6);
            // 
            // printToolStripMenuItem
            // 
            printToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("printToolStripMenuItem.Image")));
            printToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            printToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            printToolStripMenuItem.MergeIndex = 5;
            printToolStripMenuItem.Name = "printToolStripMenuItem";
            printToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            printToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            printToolStripMenuItem.Text = "&Print";
            printToolStripMenuItem.Visible = false;
            // 
            // printPreviewToolStripMenuItem
            // 
            printPreviewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("printPreviewToolStripMenuItem.Image")));
            printPreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            printPreviewToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            printPreviewToolStripMenuItem.MergeIndex = 6;
            printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            printPreviewToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            printPreviewToolStripMenuItem.Text = "Print Pre&view";
            printPreviewToolStripMenuItem.Visible = false;
            // 
            // printSetupToolStripMenuItem
            // 
            printSetupToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            printSetupToolStripMenuItem.MergeIndex = 7;
            printSetupToolStripMenuItem.Name = "printSetupToolStripMenuItem";
            printSetupToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            printSetupToolStripMenuItem.Text = "Print Setup";
            printSetupToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.MergeAction = System.Windows.Forms.MergeAction.Insert;
            toolStripSeparator5.MergeIndex = 8;
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(241, 6);
            toolStripSeparator5.Visible = false;
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
            // graphContextMenuStrip
            // 
            graphContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyGraphToolStripMenuItem,
            this.saveGraphAsImageToolStripMenuItem,
            this.saveGraphDataToolStripMenuItem,
            this.graphPageSetupToolStripMenuItem,
            this.printGraphToolStripMenuItem,
            this.showValuesToolStripMenuItem,
            this.unZoomToolStripMenuItem,
            this.unZoomAllToolStripMenuItem,
            this.defaultScaleToolStripMenuItem});
            graphContextMenuStrip.Name = "graphContextMenuStrip";
            graphContextMenuStrip.Size = new System.Drawing.Size(287, 202);
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
            // collect2_graph
            // 
            this.collect2_graph.EditButtons = System.Windows.Forms.MouseButtons.None;
            this.collect2_graph.EditModifierKeys = System.Windows.Forms.Keys.None;
            this.collect2_graph.IsEnableSelection = true;
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
            this.collect2_graph.SelectModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)));
            this.collect2_graph.Size = new System.Drawing.Size(281, 192);
            this.collect2_graph.TabIndex = 10;
            // 
            // collect1_graph
            // 
            this.collect1_graph.EditButtons = System.Windows.Forms.MouseButtons.None;
            this.collect1_graph.EditModifierKeys = System.Windows.Forms.Keys.None;
            this.collect1_graph.IsEnableSelection = true;
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
            this.collect1_graph.SelectModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)));
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
            this.Controls.Add(graphFormMenuStrip);
            this.MainMenuStrip = graphFormMenuStrip;
            this.MinimizeBox = false;
            this.Name = "GraphForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Коллекторы";
            this.Load += new System.EventHandler(this.GraphForm_Load);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.GraphForm_Validating);
            this.Resize += new System.EventHandler(this.GraphForm_Resize);
            graphFormMenuStrip.ResumeLayout(false);
            graphFormMenuStrip.PerformLayout();
            graphContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraphControlPlus collect2_graph;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeSpecterFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveGraphAsImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveGraphDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem graphPageSetupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unZoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unZoomAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultScaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem distractFromCurrentToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveSpecterFileDialog;
        private System.Windows.Forms.OpenFileDialog openSpecterFileDialog;
        private ZedGraphControlPlus collect1_graph;
        private System.Windows.Forms.ToolStripMenuItem openSpecterFileToolStripMenuItem;
    }
}