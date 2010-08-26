namespace Flavor.Forms {
    partial class mainForm {
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
            System.Windows.Forms.MenuStrip menuStrip;
            System.Windows.Forms.ToolStripMenuItem fileMenu;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem viewMenu;
            System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem openConfigFileToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem saveConfigFileToolStripMenuItem;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripMenuItem delaysToolStripMenuItem;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.Windows.Forms.ToolStripMenuItem overviewToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem senseToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem monitorToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem helpMenu;
            System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
            System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
            this.openSpecterFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ParameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GraphWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.measureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.initSys_butt = new System.Windows.Forms.ToolStripButton();
            this.overview_button = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.measure_StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.sensmeasure_button = new System.Windows.Forms.ToolStripButton();
            this.shutSys_butt = new System.Windows.Forms.ToolStripButton();
            this.unblock_butt = new System.Windows.Forms.ToolStripButton();
            this.controlToolStrip = new System.Windows.Forms.ToolStrip();
            this.connectToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.monitorToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.parameterPanel = new System.Windows.Forms.Panel();
            this.statusTreeView = new System.Windows.Forms.TreeView();
            this.statusTreeViewStateImageList = new System.Windows.Forms.ImageList(this.components);
            this.openSpecterFileDialog = new System.Windows.Forms.OpenFileDialog();
            menuStrip = new System.Windows.Forms.MenuStrip();
            fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openConfigFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveConfigFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            delaysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            overviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            senseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            monitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.controlToolStrip.SuspendLayout();
            this.parameterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileMenu,
            viewMenu,
            optionsToolStripMenuItem,
            helpMenu});
            menuStrip.Location = new System.Drawing.Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new System.Drawing.Size(992, 24);
            menuStrip.TabIndex = 1;
            menuStrip.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSpecterFileToolStripMenuItem,
            exitToolStripMenuItem});
            fileMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            fileMenu.Name = "fileMenu";
            fileMenu.Size = new System.Drawing.Size(61, 20);
            fileMenu.Text = "&Главное";
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
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            exitToolStripMenuItem.Text = "В&ыход";
            exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewMenu
            // 
            viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBarToolStripMenuItem,
            this.ParameterToolStripMenuItem,
            this.statusBarToolStripMenuItem,
            this.GraphWindowToolStripMenuItem});
            viewMenu.Name = "viewMenu";
            viewMenu.Size = new System.Drawing.Size(38, 20);
            viewMenu.Text = "&Вид";
            // 
            // toolBarToolStripMenuItem
            // 
            this.toolBarToolStripMenuItem.Checked = true;
            this.toolBarToolStripMenuItem.CheckOnClick = true;
            this.toolBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolBarToolStripMenuItem.Name = "toolBarToolStripMenuItem";
            this.toolBarToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.toolBarToolStripMenuItem.Text = "Панель &управления прибором";
            this.toolBarToolStripMenuItem.Click += new System.EventHandler(this.ToolBarToolStripMenuItem_Click);
            // 
            // ParameterToolStripMenuItem
            // 
            this.ParameterToolStripMenuItem.Checked = true;
            this.ParameterToolStripMenuItem.CheckOnClick = true;
            this.ParameterToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ParameterToolStripMenuItem.Name = "ParameterToolStripMenuItem";
            this.ParameterToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.ParameterToolStripMenuItem.Text = "&Параметры системы";
            this.ParameterToolStripMenuItem.Click += new System.EventHandler(this.ParameterToolStripMenuItem_Click);
            // 
            // statusBarToolStripMenuItem
            // 
            this.statusBarToolStripMenuItem.Checked = true;
            this.statusBarToolStripMenuItem.CheckOnClick = true;
            this.statusBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            this.statusBarToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.statusBarToolStripMenuItem.Text = "Строка &состояния";
            this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.StatusBarToolStripMenuItem_Click);
            // 
            // GraphWindowToolStripMenuItem
            // 
            this.GraphWindowToolStripMenuItem.Name = "GraphWindowToolStripMenuItem";
            this.GraphWindowToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.GraphWindowToolStripMenuItem.Text = "&Коллекторы";
            this.GraphWindowToolStripMenuItem.Click += new System.EventHandler(this.GraphWindowToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            openConfigFileToolStripMenuItem,
            saveConfigFileToolStripMenuItem,
            toolStripSeparator1,
            this.connectToolStripMenuItem,
            this.measureToolStripMenuItem});
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            optionsToolStripMenuItem.Text = "&Настройки";
            // 
            // openConfigFileToolStripMenuItem
            // 
            openConfigFileToolStripMenuItem.Name = "openConfigFileToolStripMenuItem";
            openConfigFileToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            openConfigFileToolStripMenuItem.Text = "Перечитать файл конфигурации";
            openConfigFileToolStripMenuItem.Click += new System.EventHandler(this.openConfigFileToolStripMenuItem_Click);
            // 
            // saveConfigFileToolStripMenuItem
            // 
            saveConfigFileToolStripMenuItem.Name = "saveConfigFileToolStripMenuItem";
            saveConfigFileToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            saveConfigFileToolStripMenuItem.Text = "Перезаписать файл конфигурации";
            saveConfigFileToolStripMenuItem.Click += new System.EventHandler(this.saveConfigFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(258, 6);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.connectToolStripMenuItem.Text = "Соединение";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // measureToolStripMenuItem
            // 
            this.measureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            delaysToolStripMenuItem,
            toolStripSeparator2,
            overviewToolStripMenuItem,
            senseToolStripMenuItem,
            monitorToolStripMenuItem});
            this.measureToolStripMenuItem.Name = "measureToolStripMenuItem";
            this.measureToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.measureToolStripMenuItem.Text = "Измерение";
            // 
            // delaysToolStripMenuItem
            // 
            delaysToolStripMenuItem.Name = "delaysToolStripMenuItem";
            delaysToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            delaysToolStripMenuItem.Text = "Установка задержек";
            delaysToolStripMenuItem.Click += new System.EventHandler(this.delaysToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(189, 6);
            // 
            // overviewToolStripMenuItem
            // 
            overviewToolStripMenuItem.Name = "overviewToolStripMenuItem";
            overviewToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            overviewToolStripMenuItem.Text = "Обзорный режим";
            overviewToolStripMenuItem.Click += new System.EventHandler(this.overviewToolStripMenuItem_Click);
            // 
            // senseToolStripMenuItem
            // 
            senseToolStripMenuItem.Name = "senseToolStripMenuItem";
            senseToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            senseToolStripMenuItem.Text = "Точный режим";
            senseToolStripMenuItem.Click += new System.EventHandler(this.senseToolStripMenuItem_Click);
            // 
            // monitorToolStripMenuItem
            // 
            monitorToolStripMenuItem.Name = "monitorToolStripMenuItem";
            monitorToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            monitorToolStripMenuItem.Text = "Режим мониторинга";
            monitorToolStripMenuItem.Click += new System.EventHandler(this.monitorToolStripMenuItem_Click);
            // 
            // helpMenu
            // 
            helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            contentsToolStripMenuItem,
            indexToolStripMenuItem,
            searchToolStripMenuItem,
            toolStripSeparator8,
            aboutToolStripMenuItem});
            helpMenu.Name = "helpMenu";
            helpMenu.Size = new System.Drawing.Size(59, 20);
            helpMenu.Text = "&Помощь";
            helpMenu.Visible = false;
            // 
            // contentsToolStripMenuItem
            // 
            contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            contentsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
            contentsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            contentsToolStripMenuItem.Text = "&Contents";
            // 
            // indexToolStripMenuItem
            // 
            indexToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("indexToolStripMenuItem.Image")));
            indexToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            indexToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            indexToolStripMenuItem.Text = "&Index";
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("searchToolStripMenuItem.Image")));
            searchToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            searchToolStripMenuItem.Text = "&Search";
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new System.Drawing.Size(170, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            aboutToolStripMenuItem.Text = "&О программе ...";
            // 
            // initSys_butt
            // 
            this.initSys_butt.BackColor = System.Drawing.Color.White;
            this.initSys_butt.Enabled = false;
            this.initSys_butt.ForeColor = System.Drawing.SystemColors.ControlText;
            this.initSys_butt.Image = ((System.Drawing.Image)(resources.GetObject("initSys_butt.Image")));
            this.initSys_butt.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.initSys_butt.Name = "initSys_butt";
            this.initSys_butt.Size = new System.Drawing.Size(101, 22);
            this.initSys_butt.Text = "Запуск системы";
            this.initSys_butt.Click += new System.EventHandler(this.initSys_butt_Click);
            // 
            // overview_button
            // 
            this.overview_button.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.overview_button.BackColor = System.Drawing.Color.White;
            this.overview_button.Enabled = false;
            this.overview_button.Name = "overview_button";
            this.overview_button.Size = new System.Drawing.Size(97, 22);
            this.overview_button.Text = "Обзорный режим";
            this.overview_button.Click += new System.EventHandler(this.overview_button_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.measure_StatusLabel});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip.Location = new System.Drawing.Point(0, 944);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(992, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 9;
            // 
            // measure_StatusLabel
            // 
            this.measure_StatusLabel.BackColor = System.Drawing.SystemColors.MenuBar;
            this.measure_StatusLabel.Name = "measure_StatusLabel";
            this.measure_StatusLabel.Size = new System.Drawing.Size(96, 17);
            this.measure_StatusLabel.Text = "Info about System";
            // 
            // sensmeasure_button
            // 
            this.sensmeasure_button.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.sensmeasure_button.BackColor = System.Drawing.Color.White;
            this.sensmeasure_button.Enabled = false;
            this.sensmeasure_button.Name = "sensmeasure_button";
            this.sensmeasure_button.Size = new System.Drawing.Size(84, 22);
            this.sensmeasure_button.Text = "Точный режим";
            this.sensmeasure_button.Click += new System.EventHandler(this.sensmeasure_button_Click);
            // 
            // shutSys_butt
            // 
            this.shutSys_butt.BackColor = System.Drawing.Color.White;
            this.shutSys_butt.Enabled = false;
            this.shutSys_butt.Image = ((System.Drawing.Image)(resources.GetObject("shutSys_butt.Image")));
            this.shutSys_butt.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.shutSys_butt.Name = "shutSys_butt";
            this.shutSys_butt.Size = new System.Drawing.Size(123, 22);
            this.shutSys_butt.Text = "Отключить систему";
            this.shutSys_butt.Click += new System.EventHandler(this.shutSys_butt_Click);
            // 
            // unblock_butt
            // 
            this.unblock_butt.BackColor = System.Drawing.Color.White;
            this.unblock_butt.Enabled = false;
            this.unblock_butt.Image = ((System.Drawing.Image)(resources.GetObject("unblock_butt.Image")));
            this.unblock_butt.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.unblock_butt.Name = "unblock_butt";
            this.unblock_butt.Size = new System.Drawing.Size(115, 22);
            this.unblock_butt.Text = "Снять блокировку";
            this.unblock_butt.Click += new System.EventHandler(this.unblock_butt_Click);
            // 
            // controlToolStrip
            // 
            this.controlToolStrip.AllowMerge = false;
            this.controlToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.controlToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripButton,
            this.initSys_butt,
            this.shutSys_butt,
            this.unblock_butt,
            this.monitorToolStripButton,
            this.sensmeasure_button,
            this.overview_button});
            this.controlToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.controlToolStrip.Location = new System.Drawing.Point(0, 24);
            this.controlToolStrip.Name = "controlToolStrip";
            this.controlToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.controlToolStrip.Size = new System.Drawing.Size(992, 25);
            this.controlToolStrip.TabIndex = 14;
            this.controlToolStrip.Text = "Управление масс-спектрометром";
            // 
            // connectToolStripButton
            // 
            this.connectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.connectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.connectToolStripButton.Name = "connectToolStripButton";
            this.connectToolStripButton.Size = new System.Drawing.Size(67, 22);
            this.connectToolStripButton.Text = "Соединить";
            this.connectToolStripButton.Click += new System.EventHandler(this.connectToolStripButton_Click);
            // 
            // monitorToolStripButton
            // 
            this.monitorToolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.monitorToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.monitorToolStripButton.Enabled = false;
            this.monitorToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("monitorToolStripButton.Image")));
            this.monitorToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.monitorToolStripButton.Name = "monitorToolStripButton";
            this.monitorToolStripButton.Size = new System.Drawing.Size(111, 22);
            this.monitorToolStripButton.Text = "Режим мониторинга";
            this.monitorToolStripButton.Click += new System.EventHandler(this.monitorToolStripButton_Click);
            // 
            // parameterPanel
            // 
            this.parameterPanel.BackColor = System.Drawing.SystemColors.Control;
            this.parameterPanel.Controls.Add(this.statusTreeView);
            this.parameterPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.parameterPanel.Location = new System.Drawing.Point(0, 49);
            this.parameterPanel.Name = "parameterPanel";
            this.parameterPanel.Size = new System.Drawing.Size(234, 895);
            this.parameterPanel.TabIndex = 16;
            this.parameterPanel.Text = "Параметры системы";
            // 
            // statusTreeView
            // 
            this.statusTreeView.BackColor = System.Drawing.SystemColors.Control;
            this.statusTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.statusTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusTreeView.Indent = 15;
            this.statusTreeView.Location = new System.Drawing.Point(0, 0);
            this.statusTreeView.Name = "statusTreeView";
            this.statusTreeView.ShowLines = false;
            this.statusTreeView.ShowPlusMinus = false;
            this.statusTreeView.Size = new System.Drawing.Size(234, 895);
            this.statusTreeView.StateImageList = this.statusTreeViewStateImageList;
            this.statusTreeView.TabIndex = 0;
            // 
            // statusTreeViewStateImageList
            // 
            this.statusTreeViewStateImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("statusTreeViewStateImageList.ImageStream")));
            this.statusTreeViewStateImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.statusTreeViewStateImageList.Images.SetKeyName(0, "warning");
            this.statusTreeViewStateImageList.Images.SetKeyName(1, "error");
            // 
            // openSpecterFileDialog
            // 
            this.openSpecterFileDialog.InitialDirectory = ".";
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(992, 966);
            this.Controls.Add(this.parameterPanel);
            this.Controls.Add(this.controlToolStrip);
            this.Controls.Add(menuStrip);
            this.Controls.Add(this.statusStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
            this.Name = "mainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Управление масс-спектрометром";
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.MdiChildActivate += new System.EventHandler(this.mainForm_MdiChildActivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainForm_FormClosing);
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.controlToolStrip.ResumeLayout(false);
            this.controlToolStrip.PerformLayout();
            this.parameterPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripButton initSys_butt;
        private System.Windows.Forms.ToolStripButton shutSys_butt;
        private System.Windows.Forms.ToolStripButton unblock_butt;
        private System.Windows.Forms.ToolStripButton overview_button;
        private System.Windows.Forms.ToolStripButton sensmeasure_button;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel measure_StatusLabel;
        private System.Windows.Forms.ToolStripMenuItem toolBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statusBarToolStripMenuItem;
        private System.Windows.Forms.Panel parameterPanel;
        private System.Windows.Forms.ToolStripMenuItem ParameterToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton connectToolStripButton;

        private System.Windows.Forms.TreeView statusTreeView;
        private System.Windows.Forms.ImageList statusTreeViewStateImageList;
        private System.Windows.Forms.ToolStripButton monitorToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem measureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem GraphWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStrip controlToolStrip;
        private System.Windows.Forms.OpenFileDialog openSpecterFileDialog;
        private System.Windows.Forms.ToolStripMenuItem openSpecterFileToolStripMenuItem;
    }
}

