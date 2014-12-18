namespace Flavor.Forms {
    partial class MainForm2 {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm2));
            System.Windows.Forms.ToolStripMenuItem fileMenu;
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
            System.Windows.Forms.ToolStripMenuItem windowsMenu;
            System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
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
            this.inletToolStripButton = new System.Windows.Forms.ToolStripButton();
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
            windowsMenu = new System.Windows.Forms.ToolStripMenuItem();
            closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
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
            menuStrip.AccessibleDescription = null;
            menuStrip.AccessibleName = null;
            resources.ApplyResources(menuStrip, "menuStrip");
            menuStrip.BackgroundImage = null;
            menuStrip.Font = null;
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileMenu,
            viewMenu,
            optionsToolStripMenuItem,
            windowsMenu,
            helpMenu});
            menuStrip.MdiWindowListItem = windowsMenu;
            menuStrip.Name = "menuStrip";
            // 
            // fileMenu
            // 
            fileMenu.AccessibleDescription = null;
            fileMenu.AccessibleName = null;
            resources.ApplyResources(fileMenu, "fileMenu");
            fileMenu.BackgroundImage = null;
            fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSpecterFileToolStripMenuItem,
            exitToolStripMenuItem});
            fileMenu.Name = "fileMenu";
            fileMenu.ShortcutKeyDisplayString = null;
            // 
            // openSpecterFileToolStripMenuItem
            // 
            this.openSpecterFileToolStripMenuItem.AccessibleDescription = null;
            this.openSpecterFileToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.openSpecterFileToolStripMenuItem, "openSpecterFileToolStripMenuItem");
            this.openSpecterFileToolStripMenuItem.BackgroundImage = null;
            this.openSpecterFileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.openSpecterFileToolStripMenuItem.MergeIndex = 0;
            this.openSpecterFileToolStripMenuItem.Name = "openSpecterFileToolStripMenuItem";
            this.openSpecterFileToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.openSpecterFileToolStripMenuItem.Click += new System.EventHandler(this.openSpecterFileToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.AccessibleDescription = null;
            exitToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(exitToolStripMenuItem, "exitToolStripMenuItem");
            exitToolStripMenuItem.BackgroundImage = null;
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.ShortcutKeyDisplayString = null;
            exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewMenu
            // 
            viewMenu.AccessibleDescription = null;
            viewMenu.AccessibleName = null;
            resources.ApplyResources(viewMenu, "viewMenu");
            viewMenu.BackgroundImage = null;
            viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBarToolStripMenuItem,
            this.ParameterToolStripMenuItem,
            this.statusBarToolStripMenuItem});
            viewMenu.Name = "viewMenu";
            viewMenu.ShortcutKeyDisplayString = null;
            // 
            // toolBarToolStripMenuItem
            // 
            this.toolBarToolStripMenuItem.AccessibleDescription = null;
            this.toolBarToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.toolBarToolStripMenuItem, "toolBarToolStripMenuItem");
            this.toolBarToolStripMenuItem.BackgroundImage = null;
            this.toolBarToolStripMenuItem.Checked = true;
            this.toolBarToolStripMenuItem.CheckOnClick = true;
            this.toolBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolBarToolStripMenuItem.Name = "toolBarToolStripMenuItem";
            this.toolBarToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.toolBarToolStripMenuItem.Click += new System.EventHandler(this.ToolBarToolStripMenuItem_Click);
            // 
            // ParameterToolStripMenuItem
            // 
            this.ParameterToolStripMenuItem.AccessibleDescription = null;
            this.ParameterToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.ParameterToolStripMenuItem, "ParameterToolStripMenuItem");
            this.ParameterToolStripMenuItem.BackgroundImage = null;
            this.ParameterToolStripMenuItem.Checked = true;
            this.ParameterToolStripMenuItem.CheckOnClick = true;
            this.ParameterToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ParameterToolStripMenuItem.Name = "ParameterToolStripMenuItem";
            this.ParameterToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.ParameterToolStripMenuItem.Click += new System.EventHandler(this.ParameterToolStripMenuItem_Click);
            // 
            // statusBarToolStripMenuItem
            // 
            this.statusBarToolStripMenuItem.AccessibleDescription = null;
            this.statusBarToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.statusBarToolStripMenuItem, "statusBarToolStripMenuItem");
            this.statusBarToolStripMenuItem.BackgroundImage = null;
            this.statusBarToolStripMenuItem.Checked = true;
            this.statusBarToolStripMenuItem.CheckOnClick = true;
            this.statusBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            this.statusBarToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.StatusBarToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.AccessibleDescription = null;
            optionsToolStripMenuItem.AccessibleName = null;
            optionsToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(optionsToolStripMenuItem, "optionsToolStripMenuItem");
            optionsToolStripMenuItem.BackgroundImage = null;
            optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            openConfigFileToolStripMenuItem,
            saveConfigFileToolStripMenuItem,
            toolStripSeparator1,
            this.connectToolStripMenuItem,
            this.measureToolStripMenuItem});
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.ShortcutKeyDisplayString = null;
            // 
            // openConfigFileToolStripMenuItem
            // 
            openConfigFileToolStripMenuItem.AccessibleDescription = null;
            openConfigFileToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(openConfigFileToolStripMenuItem, "openConfigFileToolStripMenuItem");
            openConfigFileToolStripMenuItem.BackgroundImage = null;
            openConfigFileToolStripMenuItem.Name = "openConfigFileToolStripMenuItem";
            openConfigFileToolStripMenuItem.ShortcutKeyDisplayString = null;
            openConfigFileToolStripMenuItem.Click += new System.EventHandler(this.openConfigFileToolStripMenuItem_Click);
            // 
            // saveConfigFileToolStripMenuItem
            // 
            saveConfigFileToolStripMenuItem.AccessibleDescription = null;
            saveConfigFileToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(saveConfigFileToolStripMenuItem, "saveConfigFileToolStripMenuItem");
            saveConfigFileToolStripMenuItem.BackgroundImage = null;
            saveConfigFileToolStripMenuItem.Name = "saveConfigFileToolStripMenuItem";
            saveConfigFileToolStripMenuItem.ShortcutKeyDisplayString = null;
            saveConfigFileToolStripMenuItem.Click += new System.EventHandler(this.saveConfigFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.AccessibleDescription = null;
            toolStripSeparator1.AccessibleName = null;
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.AccessibleDescription = null;
            this.connectToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.connectToolStripMenuItem, "connectToolStripMenuItem");
            this.connectToolStripMenuItem.BackgroundImage = null;
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // measureToolStripMenuItem
            // 
            this.measureToolStripMenuItem.AccessibleDescription = null;
            this.measureToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.measureToolStripMenuItem, "measureToolStripMenuItem");
            this.measureToolStripMenuItem.BackgroundImage = null;
            this.measureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            delaysToolStripMenuItem,
            toolStripSeparator2,
            overviewToolStripMenuItem,
            senseToolStripMenuItem,
            monitorToolStripMenuItem});
            this.measureToolStripMenuItem.Name = "measureToolStripMenuItem";
            this.measureToolStripMenuItem.ShortcutKeyDisplayString = null;
            // 
            // delaysToolStripMenuItem
            // 
            delaysToolStripMenuItem.AccessibleDescription = null;
            delaysToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(delaysToolStripMenuItem, "delaysToolStripMenuItem");
            delaysToolStripMenuItem.BackgroundImage = null;
            delaysToolStripMenuItem.Name = "delaysToolStripMenuItem";
            delaysToolStripMenuItem.ShortcutKeyDisplayString = null;
            delaysToolStripMenuItem.Click += new System.EventHandler(this.delaysToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.AccessibleDescription = null;
            toolStripSeparator2.AccessibleName = null;
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // overviewToolStripMenuItem
            // 
            overviewToolStripMenuItem.AccessibleDescription = null;
            overviewToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(overviewToolStripMenuItem, "overviewToolStripMenuItem");
            overviewToolStripMenuItem.BackgroundImage = null;
            overviewToolStripMenuItem.Name = "overviewToolStripMenuItem";
            overviewToolStripMenuItem.ShortcutKeyDisplayString = null;
            overviewToolStripMenuItem.Click += new System.EventHandler(this.overviewToolStripMenuItem_Click);
            // 
            // senseToolStripMenuItem
            // 
            senseToolStripMenuItem.AccessibleDescription = null;
            senseToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(senseToolStripMenuItem, "senseToolStripMenuItem");
            senseToolStripMenuItem.BackgroundImage = null;
            senseToolStripMenuItem.Name = "senseToolStripMenuItem";
            senseToolStripMenuItem.ShortcutKeyDisplayString = null;
            senseToolStripMenuItem.Click += new System.EventHandler(this.senseToolStripMenuItem_Click);
            // 
            // monitorToolStripMenuItem
            // 
            monitorToolStripMenuItem.AccessibleDescription = null;
            monitorToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(monitorToolStripMenuItem, "monitorToolStripMenuItem");
            monitorToolStripMenuItem.BackgroundImage = null;
            monitorToolStripMenuItem.Name = "monitorToolStripMenuItem";
            monitorToolStripMenuItem.ShortcutKeyDisplayString = null;
            monitorToolStripMenuItem.Click += new System.EventHandler(this.monitorToolStripMenuItem_Click);
            // 
            // windowsMenu
            // 
            windowsMenu.AccessibleDescription = null;
            windowsMenu.AccessibleName = null;
            resources.ApplyResources(windowsMenu, "windowsMenu");
            windowsMenu.BackgroundImage = null;
            windowsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            closeAllToolStripMenuItem,
            toolStripSeparator3});
            windowsMenu.Name = "windowsMenu";
            windowsMenu.ShortcutKeyDisplayString = null;
            // 
            // closeAllToolStripMenuItem
            // 
            closeAllToolStripMenuItem.AccessibleDescription = null;
            closeAllToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(closeAllToolStripMenuItem, "closeAllToolStripMenuItem");
            closeAllToolStripMenuItem.BackgroundImage = null;
            closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            closeAllToolStripMenuItem.ShortcutKeyDisplayString = null;
            closeAllToolStripMenuItem.Click += new System.EventHandler(this.closeAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.AccessibleDescription = null;
            toolStripSeparator3.AccessibleName = null;
            resources.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
            toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // helpMenu
            // 
            helpMenu.AccessibleDescription = null;
            helpMenu.AccessibleName = null;
            resources.ApplyResources(helpMenu, "helpMenu");
            helpMenu.BackgroundImage = null;
            helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            contentsToolStripMenuItem,
            indexToolStripMenuItem,
            searchToolStripMenuItem,
            toolStripSeparator8,
            aboutToolStripMenuItem});
            helpMenu.Name = "helpMenu";
            helpMenu.ShortcutKeyDisplayString = null;
            // 
            // contentsToolStripMenuItem
            // 
            contentsToolStripMenuItem.AccessibleDescription = null;
            contentsToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(contentsToolStripMenuItem, "contentsToolStripMenuItem");
            contentsToolStripMenuItem.BackgroundImage = null;
            contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            contentsToolStripMenuItem.ShortcutKeyDisplayString = null;
            // 
            // indexToolStripMenuItem
            // 
            indexToolStripMenuItem.AccessibleDescription = null;
            indexToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(indexToolStripMenuItem, "indexToolStripMenuItem");
            indexToolStripMenuItem.BackgroundImage = null;
            indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            indexToolStripMenuItem.ShortcutKeyDisplayString = null;
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.AccessibleDescription = null;
            searchToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(searchToolStripMenuItem, "searchToolStripMenuItem");
            searchToolStripMenuItem.BackgroundImage = null;
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.ShortcutKeyDisplayString = null;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.AccessibleDescription = null;
            toolStripSeparator8.AccessibleName = null;
            resources.ApplyResources(toolStripSeparator8, "toolStripSeparator8");
            toolStripSeparator8.Name = "toolStripSeparator8";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.AccessibleDescription = null;
            aboutToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(aboutToolStripMenuItem, "aboutToolStripMenuItem");
            aboutToolStripMenuItem.BackgroundImage = null;
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.ShortcutKeyDisplayString = null;
            // 
            // initSys_butt
            // 
            this.initSys_butt.AccessibleDescription = null;
            this.initSys_butt.AccessibleName = null;
            resources.ApplyResources(this.initSys_butt, "initSys_butt");
            this.initSys_butt.BackColor = System.Drawing.Color.White;
            this.initSys_butt.BackgroundImage = null;
            this.initSys_butt.ForeColor = System.Drawing.SystemColors.ControlText;
            this.initSys_butt.Name = "initSys_butt";
            this.initSys_butt.Click += new System.EventHandler(this.initSys_butt_Click);
            // 
            // overview_button
            // 
            this.overview_button.AccessibleDescription = null;
            this.overview_button.AccessibleName = null;
            this.overview_button.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.overview_button, "overview_button");
            this.overview_button.BackColor = System.Drawing.Color.White;
            this.overview_button.BackgroundImage = null;
            this.overview_button.Name = "overview_button";
            this.overview_button.Click += new System.EventHandler(this.overview_button_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.AccessibleDescription = null;
            this.statusStrip.AccessibleName = null;
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.BackgroundImage = null;
            this.statusStrip.Font = null;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.measure_StatusLabel});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.SizingGrip = false;
            // 
            // measure_StatusLabel
            // 
            this.measure_StatusLabel.AccessibleDescription = null;
            this.measure_StatusLabel.AccessibleName = null;
            resources.ApplyResources(this.measure_StatusLabel, "measure_StatusLabel");
            this.measure_StatusLabel.BackColor = System.Drawing.SystemColors.MenuBar;
            this.measure_StatusLabel.BackgroundImage = null;
            this.measure_StatusLabel.Name = "measure_StatusLabel";
            // 
            // sensmeasure_button
            // 
            this.sensmeasure_button.AccessibleDescription = null;
            this.sensmeasure_button.AccessibleName = null;
            this.sensmeasure_button.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.sensmeasure_button, "sensmeasure_button");
            this.sensmeasure_button.BackColor = System.Drawing.Color.White;
            this.sensmeasure_button.BackgroundImage = null;
            this.sensmeasure_button.Name = "sensmeasure_button";
            this.sensmeasure_button.Click += new System.EventHandler(this.sensmeasure_button_Click);
            // 
            // shutSys_butt
            // 
            this.shutSys_butt.AccessibleDescription = null;
            this.shutSys_butt.AccessibleName = null;
            resources.ApplyResources(this.shutSys_butt, "shutSys_butt");
            this.shutSys_butt.BackColor = System.Drawing.Color.White;
            this.shutSys_butt.BackgroundImage = null;
            this.shutSys_butt.Name = "shutSys_butt";
            this.shutSys_butt.Click += new System.EventHandler(this.shutSys_butt_Click);
            // 
            // unblock_butt
            // 
            this.unblock_butt.AccessibleDescription = null;
            this.unblock_butt.AccessibleName = null;
            resources.ApplyResources(this.unblock_butt, "unblock_butt");
            this.unblock_butt.BackColor = System.Drawing.Color.White;
            this.unblock_butt.BackgroundImage = null;
            this.unblock_butt.ForeColor = System.Drawing.Color.Green;
            this.unblock_butt.Name = "unblock_butt";
            this.unblock_butt.Click += new System.EventHandler(this.unblock_butt_Click);
            // 
            // controlToolStrip
            // 
            this.controlToolStrip.AccessibleDescription = null;
            this.controlToolStrip.AccessibleName = null;
            this.controlToolStrip.AllowMerge = false;
            resources.ApplyResources(this.controlToolStrip, "controlToolStrip");
            this.controlToolStrip.BackgroundImage = null;
            this.controlToolStrip.Font = null;
            this.controlToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.controlToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripButton,
            this.initSys_butt,
            this.shutSys_butt,
            this.unblock_butt,
            this.monitorToolStripButton,
            this.sensmeasure_button,
            this.overview_button,
            this.inletToolStripButton});
            this.controlToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.controlToolStrip.Name = "controlToolStrip";
            this.controlToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // connectToolStripButton
            // 
            this.connectToolStripButton.AccessibleDescription = null;
            this.connectToolStripButton.AccessibleName = null;
            resources.ApplyResources(this.connectToolStripButton, "connectToolStripButton");
            this.connectToolStripButton.BackgroundImage = null;
            this.connectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.connectToolStripButton.Name = "connectToolStripButton";
            this.connectToolStripButton.Click += new System.EventHandler(this.connectToolStripButton_Click);
            // 
            // monitorToolStripButton
            // 
            this.monitorToolStripButton.AccessibleDescription = null;
            this.monitorToolStripButton.AccessibleName = null;
            this.monitorToolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.monitorToolStripButton, "monitorToolStripButton");
            this.monitorToolStripButton.BackgroundImage = null;
            this.monitorToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.monitorToolStripButton.Name = "monitorToolStripButton";
            this.monitorToolStripButton.Click += new System.EventHandler(this.monitorToolStripButton_Click);
            // 
            // inletToolStripButton
            // 
            this.inletToolStripButton.AccessibleDescription = null;
            this.inletToolStripButton.AccessibleName = null;
            this.inletToolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.inletToolStripButton, "inletToolStripButton");
            this.inletToolStripButton.BackgroundImage = null;
            this.inletToolStripButton.Name = "inletToolStripButton";
            this.inletToolStripButton.Click += new System.EventHandler(this.inletToolStripButton_Click);
            // 
            // parameterPanel
            // 
            this.parameterPanel.AccessibleDescription = null;
            this.parameterPanel.AccessibleName = null;
            resources.ApplyResources(this.parameterPanel, "parameterPanel");
            this.parameterPanel.BackColor = System.Drawing.SystemColors.Control;
            this.parameterPanel.BackgroundImage = null;
            this.parameterPanel.Controls.Add(this.statusTreeView);
            this.parameterPanel.Font = null;
            this.parameterPanel.Name = "parameterPanel";
            // 
            // statusTreeView
            // 
            this.statusTreeView.AccessibleDescription = null;
            this.statusTreeView.AccessibleName = null;
            resources.ApplyResources(this.statusTreeView, "statusTreeView");
            this.statusTreeView.BackColor = System.Drawing.SystemColors.Control;
            this.statusTreeView.BackgroundImage = null;
            this.statusTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.statusTreeView.Font = null;
            this.statusTreeView.Name = "statusTreeView";
            this.statusTreeView.ShowLines = false;
            this.statusTreeView.ShowPlusMinus = false;
            this.statusTreeView.StateImageList = this.statusTreeViewStateImageList;
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
            resources.ApplyResources(this.openSpecterFileDialog, "openSpecterFileDialog");
            this.openSpecterFileDialog.InitialDirectory = ".";
            // 
            // MainForm2
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = null;
            this.Controls.Add(this.parameterPanel);
            this.Controls.Add(this.controlToolStrip);
            this.Controls.Add(menuStrip);
            this.Controls.Add(this.statusStrip);
            this.Font = null;
            this.Icon = null;
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
            this.Name = "MainForm2";
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
        private System.Windows.Forms.ToolStrip controlToolStrip;
        private System.Windows.Forms.OpenFileDialog openSpecterFileDialog;
        private System.Windows.Forms.ToolStripMenuItem openSpecterFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton inletToolStripButton;
    }
}

