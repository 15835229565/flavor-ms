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
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileMenu,
            viewMenu,
            optionsToolStripMenuItem,
            windowsMenu,
            helpMenu});
            resources.ApplyResources(menuStrip, "menuStrip");
            menuStrip.MdiWindowListItem = windowsMenu;
            menuStrip.Name = "menuStrip";
            // 
            // fileMenu
            // 
            fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSpecterFileToolStripMenuItem,
            exitToolStripMenuItem});
            resources.ApplyResources(fileMenu, "fileMenu");
            fileMenu.Name = "fileMenu";
            // 
            // openSpecterFileToolStripMenuItem
            // 
            resources.ApplyResources(this.openSpecterFileToolStripMenuItem, "openSpecterFileToolStripMenuItem");
            this.openSpecterFileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.openSpecterFileToolStripMenuItem.MergeIndex = 0;
            this.openSpecterFileToolStripMenuItem.Name = "openSpecterFileToolStripMenuItem";
            this.openSpecterFileToolStripMenuItem.Click += new System.EventHandler(this.openSpecterFileToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(exitToolStripMenuItem, "exitToolStripMenuItem");
            exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewMenu
            // 
            viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBarToolStripMenuItem,
            this.ParameterToolStripMenuItem,
            this.statusBarToolStripMenuItem});
            viewMenu.Name = "viewMenu";
            resources.ApplyResources(viewMenu, "viewMenu");
            // 
            // toolBarToolStripMenuItem
            // 
            this.toolBarToolStripMenuItem.Checked = true;
            this.toolBarToolStripMenuItem.CheckOnClick = true;
            this.toolBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolBarToolStripMenuItem.Name = "toolBarToolStripMenuItem";
            resources.ApplyResources(this.toolBarToolStripMenuItem, "toolBarToolStripMenuItem");
            this.toolBarToolStripMenuItem.Click += new System.EventHandler(this.ToolBarToolStripMenuItem_Click);
            // 
            // ParameterToolStripMenuItem
            // 
            this.ParameterToolStripMenuItem.Checked = true;
            this.ParameterToolStripMenuItem.CheckOnClick = true;
            this.ParameterToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ParameterToolStripMenuItem.Name = "ParameterToolStripMenuItem";
            resources.ApplyResources(this.ParameterToolStripMenuItem, "ParameterToolStripMenuItem");
            this.ParameterToolStripMenuItem.Click += new System.EventHandler(this.ParameterToolStripMenuItem_Click);
            // 
            // statusBarToolStripMenuItem
            // 
            this.statusBarToolStripMenuItem.Checked = true;
            this.statusBarToolStripMenuItem.CheckOnClick = true;
            this.statusBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            resources.ApplyResources(this.statusBarToolStripMenuItem, "statusBarToolStripMenuItem");
            this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.StatusBarToolStripMenuItem_Click);
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
            resources.ApplyResources(optionsToolStripMenuItem, "optionsToolStripMenuItem");
            // 
            // openConfigFileToolStripMenuItem
            // 
            openConfigFileToolStripMenuItem.Name = "openConfigFileToolStripMenuItem";
            resources.ApplyResources(openConfigFileToolStripMenuItem, "openConfigFileToolStripMenuItem");
            openConfigFileToolStripMenuItem.Click += new System.EventHandler(this.openConfigFileToolStripMenuItem_Click);
            // 
            // saveConfigFileToolStripMenuItem
            // 
            saveConfigFileToolStripMenuItem.Name = "saveConfigFileToolStripMenuItem";
            resources.ApplyResources(saveConfigFileToolStripMenuItem, "saveConfigFileToolStripMenuItem");
            saveConfigFileToolStripMenuItem.Click += new System.EventHandler(this.saveConfigFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            resources.ApplyResources(this.connectToolStripMenuItem, "connectToolStripMenuItem");
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
            resources.ApplyResources(this.measureToolStripMenuItem, "measureToolStripMenuItem");
            // 
            // delaysToolStripMenuItem
            // 
            delaysToolStripMenuItem.Name = "delaysToolStripMenuItem";
            resources.ApplyResources(delaysToolStripMenuItem, "delaysToolStripMenuItem");
            delaysToolStripMenuItem.Click += new System.EventHandler(this.delaysToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            // 
            // overviewToolStripMenuItem
            // 
            overviewToolStripMenuItem.Name = "overviewToolStripMenuItem";
            resources.ApplyResources(overviewToolStripMenuItem, "overviewToolStripMenuItem");
            overviewToolStripMenuItem.Click += new System.EventHandler(this.overviewToolStripMenuItem_Click);
            // 
            // senseToolStripMenuItem
            // 
            senseToolStripMenuItem.Name = "senseToolStripMenuItem";
            resources.ApplyResources(senseToolStripMenuItem, "senseToolStripMenuItem");
            senseToolStripMenuItem.Click += new System.EventHandler(this.senseToolStripMenuItem_Click);
            // 
            // monitorToolStripMenuItem
            // 
            monitorToolStripMenuItem.Name = "monitorToolStripMenuItem";
            resources.ApplyResources(monitorToolStripMenuItem, "monitorToolStripMenuItem");
            monitorToolStripMenuItem.Click += new System.EventHandler(this.monitorToolStripMenuItem_Click);
            // 
            // windowsMenu
            // 
            windowsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            closeAllToolStripMenuItem,
            toolStripSeparator3});
            windowsMenu.Name = "windowsMenu";
            resources.ApplyResources(windowsMenu, "windowsMenu");
            // 
            // closeAllToolStripMenuItem
            // 
            closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            resources.ApplyResources(closeAllToolStripMenuItem, "closeAllToolStripMenuItem");
            closeAllToolStripMenuItem.Click += new System.EventHandler(this.closeAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
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
            resources.ApplyResources(helpMenu, "helpMenu");
            // 
            // contentsToolStripMenuItem
            // 
            contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            resources.ApplyResources(contentsToolStripMenuItem, "contentsToolStripMenuItem");
            // 
            // indexToolStripMenuItem
            // 
            resources.ApplyResources(indexToolStripMenuItem, "indexToolStripMenuItem");
            indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            // 
            // searchToolStripMenuItem
            // 
            resources.ApplyResources(searchToolStripMenuItem, "searchToolStripMenuItem");
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(toolStripSeparator8, "toolStripSeparator8");
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(aboutToolStripMenuItem, "aboutToolStripMenuItem");
            // 
            // initSys_butt
            // 
            this.initSys_butt.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.initSys_butt, "initSys_butt");
            this.initSys_butt.ForeColor = System.Drawing.SystemColors.ControlText;
            this.initSys_butt.Name = "initSys_butt";
            this.initSys_butt.Click += new System.EventHandler(this.initSys_butt_Click);
            // 
            // overview_button
            // 
            this.overview_button.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.overview_button.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.overview_button, "overview_button");
            this.overview_button.Name = "overview_button";
            this.overview_button.Click += new System.EventHandler(this.overview_button_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.measure_StatusLabel});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.SizingGrip = false;
            // 
            // measure_StatusLabel
            // 
            this.measure_StatusLabel.BackColor = System.Drawing.SystemColors.MenuBar;
            this.measure_StatusLabel.Name = "measure_StatusLabel";
            resources.ApplyResources(this.measure_StatusLabel, "measure_StatusLabel");
            // 
            // sensmeasure_button
            // 
            this.sensmeasure_button.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.sensmeasure_button.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.sensmeasure_button, "sensmeasure_button");
            this.sensmeasure_button.Name = "sensmeasure_button";
            this.sensmeasure_button.Click += new System.EventHandler(this.sensmeasure_button_Click);
            // 
            // shutSys_butt
            // 
            this.shutSys_butt.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.shutSys_butt, "shutSys_butt");
            this.shutSys_butt.Name = "shutSys_butt";
            this.shutSys_butt.Click += new System.EventHandler(this.shutSys_butt_Click);
            // 
            // unblock_butt
            // 
            this.unblock_butt.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.unblock_butt, "unblock_butt");
            this.unblock_butt.Name = "unblock_butt";
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
            resources.ApplyResources(this.controlToolStrip, "controlToolStrip");
            this.controlToolStrip.Name = "controlToolStrip";
            this.controlToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // connectToolStripButton
            // 
            this.connectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.connectToolStripButton, "connectToolStripButton");
            this.connectToolStripButton.Name = "connectToolStripButton";
            this.connectToolStripButton.Click += new System.EventHandler(this.connectToolStripButton_Click);
            // 
            // monitorToolStripButton
            // 
            this.monitorToolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.monitorToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.monitorToolStripButton, "monitorToolStripButton");
            this.monitorToolStripButton.Name = "monitorToolStripButton";
            this.monitorToolStripButton.Click += new System.EventHandler(this.monitorToolStripButton_Click);
            // 
            // parameterPanel
            // 
            this.parameterPanel.BackColor = System.Drawing.SystemColors.Control;
            this.parameterPanel.Controls.Add(this.statusTreeView);
            resources.ApplyResources(this.parameterPanel, "parameterPanel");
            this.parameterPanel.Name = "parameterPanel";
            // 
            // statusTreeView
            // 
            this.statusTreeView.BackColor = System.Drawing.SystemColors.Control;
            this.statusTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.statusTreeView, "statusTreeView");
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
            this.openSpecterFileDialog.InitialDirectory = ".";
            // 
            // mainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.parameterPanel);
            this.Controls.Add(this.controlToolStrip);
            this.Controls.Add(menuStrip);
            this.Controls.Add(this.statusStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
            this.Name = "mainForm";
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
    }
}

