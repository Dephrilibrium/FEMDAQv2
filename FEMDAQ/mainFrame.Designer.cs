namespace FEMDAQ
{
    partial class FEMDAQ
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FEMDAQ));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.miFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileOpenIni = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileOpenSweep = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileSaveLastResults = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileClose = new System.Windows.Forms.ToolStripMenuItem();
            this.miMsrmt = new System.Windows.Forms.ToolStripMenuItem();
            this.miStart = new System.Windows.Forms.ToolStripMenuItem();
            this.miPause = new System.Windows.Forms.ToolStripMenuItem();
            this.miStop = new System.Windows.Forms.ToolStripMenuItem();
            this.miJobQueue = new System.Windows.Forms.ToolStripMenuItem();
            this.miJobQueueCreateOrShow = new System.Windows.Forms.ToolStripMenuItem();
            this.miJobQueueDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.miInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.miChangelog = new System.Windows.Forms.ToolStripMenuItem();
            this.miUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.splitInfoData = new System.Windows.Forms.SplitContainer();
            this.gbSweepInfo = new System.Windows.Forms.GroupBox();
            this.lvSweepData = new System.Windows.Forms.ListView();
            this.chSweepNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chU1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chU2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chU3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chI1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chI2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chI3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chI4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbIniInfo = new System.Windows.Forms.GroupBox();
            this.tbLog = new System.Windows.Forms.RichTextBox();
            this.Chart = new HaumChart.HaumChart();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.sbProgressLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.sbIterates = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbRemaining = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbOperator = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbDescription = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbSpacer = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.InitialTimer = new System.Windows.Forms.Timer(this.components);
            this.IterativeTimer = new System.Windows.Forms.Timer(this.components);
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitInfoData)).BeginInit();
            this.splitInfoData.Panel1.SuspendLayout();
            this.splitInfoData.Panel2.SuspendLayout();
            this.splitInfoData.SuspendLayout();
            this.gbSweepInfo.SuspendLayout();
            this.gbIniInfo.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miMsrmt,
            this.miJobQueue,
            this.miHelp});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(1304, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "mainMenu";
            // 
            // miFile
            // 
            this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileOpen,
            this.miFileSaveLastResults,
            this.miFileClose});
            this.miFile.Name = "miFile";
            this.miFile.Size = new System.Drawing.Size(37, 20);
            this.miFile.Text = "File";
            // 
            // miFileOpen
            // 
            this.miFileOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileOpenIni,
            this.miFileOpenSweep});
            this.miFileOpen.Name = "miFileOpen";
            this.miFileOpen.Size = new System.Drawing.Size(198, 22);
            this.miFileOpen.Text = "Open";
            // 
            // miFileOpenIni
            // 
            this.miFileOpenIni.Name = "miFileOpenIni";
            this.miFileOpenIni.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.I)));
            this.miFileOpenIni.ShowShortcutKeys = false;
            this.miFileOpenIni.Size = new System.Drawing.Size(200, 22);
            this.miFileOpenIni.Text = "Ini-File (Ctrl+Shift+I)";
            this.miFileOpenIni.Click += new System.EventHandler(this.miFileOpenIni_Click);
            // 
            // miFileOpenSweep
            // 
            this.miFileOpenSweep.Name = "miFileOpenSweep";
            this.miFileOpenSweep.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.miFileOpenSweep.ShowShortcutKeys = false;
            this.miFileOpenSweep.Size = new System.Drawing.Size(200, 22);
            this.miFileOpenSweep.Text = "Sweep-File (Ctrl+Shift+S)";
            this.miFileOpenSweep.Click += new System.EventHandler(this.miFileOpenSweep_Click);
            // 
            // miFileSaveLastResults
            // 
            this.miFileSaveLastResults.Name = "miFileSaveLastResults";
            this.miFileSaveLastResults.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.miFileSaveLastResults.Size = new System.Drawing.Size(198, 22);
            this.miFileSaveLastResults.Text = "Save last results";
            this.miFileSaveLastResults.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // miFileClose
            // 
            this.miFileClose.Name = "miFileClose";
            this.miFileClose.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.miFileClose.ShowShortcutKeys = false;
            this.miFileClose.Size = new System.Drawing.Size(198, 22);
            this.miFileClose.Text = "Close (Alt+F4)";
            this.miFileClose.Click += new System.EventHandler(this.miFileClose_Click);
            // 
            // miMsrmt
            // 
            this.miMsrmt.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miStart,
            this.miPause,
            this.miStop});
            this.miMsrmt.Name = "miMsrmt";
            this.miMsrmt.Size = new System.Drawing.Size(92, 20);
            this.miMsrmt.Text = "Measurement";
            // 
            // miStart
            // 
            this.miStart.Name = "miStart";
            this.miStart.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.miStart.ShowShortcutKeys = false;
            this.miStart.Size = new System.Drawing.Size(180, 22);
            this.miStart.Text = "Start (F5)";
            this.miStart.Click += new System.EventHandler(this.msStart_Click);
            // 
            // miPause
            // 
            this.miPause.Enabled = false;
            this.miPause.Name = "miPause";
            this.miPause.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.miPause.ShowShortcutKeys = false;
            this.miPause.Size = new System.Drawing.Size(180, 22);
            this.miPause.Text = "Pausing (F6)";
            this.miPause.Click += new System.EventHandler(this.msPause_Click);
            // 
            // miStop
            // 
            this.miStop.Enabled = false;
            this.miStop.Name = "miStop";
            this.miStop.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.miStop.ShowShortcutKeys = false;
            this.miStop.Size = new System.Drawing.Size(180, 22);
            this.miStop.Text = "Stop (F7)";
            this.miStop.Click += new System.EventHandler(this.msStop_Click);
            // 
            // miJobQueue
            // 
            this.miJobQueue.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miJobQueueCreateOrShow,
            this.miJobQueueDelete});
            this.miJobQueue.Name = "miJobQueue";
            this.miJobQueue.Size = new System.Drawing.Size(70, 20);
            this.miJobQueue.Text = "Jobqueue";
            // 
            // miJobQueueCreateOrShow
            // 
            this.miJobQueueCreateOrShow.Name = "miJobQueueCreateOrShow";
            this.miJobQueueCreateOrShow.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
            this.miJobQueueCreateOrShow.Size = new System.Drawing.Size(206, 22);
            this.miJobQueueCreateOrShow.Text = "Create queue";
            this.miJobQueueCreateOrShow.Click += new System.EventHandler(this.miJobQueueCreateOrShow_Click);
            // 
            // miJobQueueDelete
            // 
            this.miJobQueueDelete.Name = "miJobQueueDelete";
            this.miJobQueueDelete.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.J)));
            this.miJobQueueDelete.Size = new System.Drawing.Size(206, 22);
            this.miJobQueueDelete.Text = "Delete queue";
            this.miJobQueueDelete.Click += new System.EventHandler(this.miJobQueueDelete_Click);
            // 
            // miHelp
            // 
            this.miHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miInfo,
            this.miChangelog,
            this.miUpdate});
            this.miHelp.Name = "miHelp";
            this.miHelp.Size = new System.Drawing.Size(44, 20);
            this.miHelp.Text = "Help";
            // 
            // miInfo
            // 
            this.miInfo.Name = "miInfo";
            this.miInfo.Size = new System.Drawing.Size(180, 22);
            this.miInfo.Text = "Info";
            this.miInfo.Click += new System.EventHandler(this.infoToolStripMenuItem_Click);
            // 
            // miChangelog
            // 
            this.miChangelog.Name = "miChangelog";
            this.miChangelog.Size = new System.Drawing.Size(180, 22);
            this.miChangelog.Text = "Changelog";
            this.miChangelog.Click += new System.EventHandler(this.ChangelogToolStripMenuItem_Click);
            // 
            // miUpdate
            // 
            this.miUpdate.Name = "miUpdate";
            this.miUpdate.Size = new System.Drawing.Size(180, 22);
            this.miUpdate.Text = "Check for updates";
            this.miUpdate.Click += new System.EventHandler(this.autoUpdaterToolStripMenuItem_Click);
            // 
            // splitInfoData
            // 
            this.splitInfoData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitInfoData.Location = new System.Drawing.Point(0, 24);
            this.splitInfoData.Name = "splitInfoData";
            this.splitInfoData.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitInfoData.Panel1
            // 
            this.splitInfoData.Panel1.Controls.Add(this.gbSweepInfo);
            this.splitInfoData.Panel1.Controls.Add(this.gbIniInfo);
            // 
            // splitInfoData.Panel2
            // 
            this.splitInfoData.Panel2.Controls.Add(this.Chart);
            this.splitInfoData.Panel2.Controls.Add(this.statusBar);
            this.splitInfoData.Size = new System.Drawing.Size(1304, 701);
            this.splitInfoData.SplitterDistance = 140;
            this.splitInfoData.SplitterWidth = 3;
            this.splitInfoData.TabIndex = 1;
            // 
            // gbSweepInfo
            // 
            this.gbSweepInfo.Controls.Add(this.lvSweepData);
            this.gbSweepInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbSweepInfo.Location = new System.Drawing.Point(343, 0);
            this.gbSweepInfo.Name = "gbSweepInfo";
            this.gbSweepInfo.Size = new System.Drawing.Size(961, 140);
            this.gbSweepInfo.TabIndex = 0;
            this.gbSweepInfo.TabStop = false;
            this.gbSweepInfo.Text = "Sweepfile (None):";
            // 
            // lvSweepData
            // 
            this.lvSweepData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSweepNo,
            this.chU1,
            this.chU2,
            this.chU3,
            this.chI1,
            this.chI2,
            this.chI3,
            this.chI4});
            this.lvSweepData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSweepData.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvSweepData.HideSelection = false;
            this.lvSweepData.Location = new System.Drawing.Point(3, 16);
            this.lvSweepData.Name = "lvSweepData";
            this.lvSweepData.Size = new System.Drawing.Size(955, 121);
            this.lvSweepData.TabIndex = 0;
            this.lvSweepData.UseCompatibleStateImageBehavior = false;
            this.lvSweepData.View = System.Windows.Forms.View.Details;
            // 
            // chSweepNo
            // 
            this.chSweepNo.Text = "#";
            this.chSweepNo.Width = 30;
            // 
            // chU1
            // 
            this.chU1.Text = "U1";
            this.chU1.Width = 100;
            // 
            // chU2
            // 
            this.chU2.Text = "U2";
            this.chU2.Width = 100;
            // 
            // chU3
            // 
            this.chU3.Text = "U3";
            this.chU3.Width = 100;
            // 
            // chI1
            // 
            this.chI1.Text = "I1";
            this.chI1.Width = 100;
            // 
            // chI2
            // 
            this.chI2.Text = "I2";
            this.chI2.Width = 100;
            // 
            // chI3
            // 
            this.chI3.Text = "I3";
            this.chI3.Width = 100;
            // 
            // chI4
            // 
            this.chI4.Text = "I4";
            this.chI4.Width = 100;
            // 
            // gbIniInfo
            // 
            this.gbIniInfo.Controls.Add(this.tbLog);
            this.gbIniInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.gbIniInfo.Location = new System.Drawing.Point(0, 0);
            this.gbIniInfo.Name = "gbIniInfo";
            this.gbIniInfo.Size = new System.Drawing.Size(343, 140);
            this.gbIniInfo.TabIndex = 3;
            this.gbIniInfo.TabStop = false;
            this.gbIniInfo.Text = "Log-Window:";
            // 
            // tbLog
            // 
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLog.Location = new System.Drawing.Point(3, 16);
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(337, 121);
            this.tbLog.TabIndex = 1;
            this.tbLog.Text = "";
            // 
            // Chart
            // 
            this.Chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Chart.Location = new System.Drawing.Point(0, 0);
            this.Chart.Name = "Chart";
            this.Chart.Size = new System.Drawing.Size(1304, 536);
            this.Chart.TabIndex = 1;
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sbProgressLabel,
            this.sbProgress,
            this.sbIterates,
            this.sbRemaining,
            this.sbOperator,
            this.sbDescription,
            this.sbSpacer,
            this.sbStatusLabel,
            this.sbStatus});
            this.statusBar.Location = new System.Drawing.Point(0, 536);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1304, 22);
            this.statusBar.TabIndex = 0;
            this.statusBar.Text = "Statusbar";
            // 
            // sbProgressLabel
            // 
            this.sbProgressLabel.Name = "sbProgressLabel";
            this.sbProgressLabel.Size = new System.Drawing.Size(55, 17);
            this.sbProgressLabel.Text = "Progress:";
            // 
            // sbProgress
            // 
            this.sbProgress.Name = "sbProgress";
            this.sbProgress.Size = new System.Drawing.Size(150, 16);
            this.sbProgress.Step = 1;
            // 
            // sbIterates
            // 
            this.sbIterates.Name = "sbIterates";
            this.sbIterates.Size = new System.Drawing.Size(68, 17);
            this.sbIterates.Text = "Iterates: 0/0";
            // 
            // sbRemaining
            // 
            this.sbRemaining.Margin = new System.Windows.Forms.Padding(15, 3, 0, 2);
            this.sbRemaining.Name = "sbRemaining";
            this.sbRemaining.Size = new System.Drawing.Size(140, 17);
            this.sbRemaining.Text = "Remaining: --:--:-- (--:--)";
            // 
            // sbOperator
            // 
            this.sbOperator.Margin = new System.Windows.Forms.Padding(15, 3, 0, 2);
            this.sbOperator.Name = "sbOperator";
            this.sbOperator.Size = new System.Drawing.Size(65, 17);
            this.sbOperator.Text = "Operator: -";
            // 
            // sbDescription
            // 
            this.sbDescription.Margin = new System.Windows.Forms.Padding(15, 3, 0, 2);
            this.sbDescription.Name = "sbDescription";
            this.sbDescription.Size = new System.Drawing.Size(78, 17);
            this.sbDescription.Text = "Description: -";
            // 
            // sbSpacer
            // 
            this.sbSpacer.Name = "sbSpacer";
            this.sbSpacer.Size = new System.Drawing.Size(590, 17);
            this.sbSpacer.Spring = true;
            // 
            // sbStatusLabel
            // 
            this.sbStatusLabel.BackColor = System.Drawing.SystemColors.Control;
            this.sbStatusLabel.Name = "sbStatusLabel";
            this.sbStatusLabel.Size = new System.Drawing.Size(45, 17);
            this.sbStatusLabel.Text = "Status: ";
            this.sbStatusLabel.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // sbStatus
            // 
            this.sbStatus.BackColor = System.Drawing.Color.Red;
            this.sbStatus.Name = "sbStatus";
            this.sbStatus.Size = new System.Drawing.Size(51, 17);
            this.sbStatus.Text = "Stopped";
            // 
            // InitialTimer
            // 
            this.InitialTimer.Tick += new System.EventHandler(this.InitialTimer_TicksElapsed);
            // 
            // IterativeTimer
            // 
            this.IterativeTimer.Tick += new System.EventHandler(this.IterativeTimer_TicksElapsed);
            // 
            // FEMDAQ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1304, 725);
            this.Controls.Add(this.splitInfoData);
            this.Controls.Add(this.mainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Name = "FEMDAQ";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FEMDAQ V2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FEMDAQ_FormClosing);
            this.SizeChanged += new System.EventHandler(this.FEMDAQ_SizeChanged);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.splitInfoData.Panel1.ResumeLayout(false);
            this.splitInfoData.Panel2.ResumeLayout(false);
            this.splitInfoData.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitInfoData)).EndInit();
            this.splitInfoData.ResumeLayout(false);
            this.gbSweepInfo.ResumeLayout(false);
            this.gbIniInfo.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.MenuStrip mainMenu;
        public System.Windows.Forms.ToolStripMenuItem miFile;
        public System.Windows.Forms.ToolStripMenuItem miFileOpen;
        public System.Windows.Forms.ToolStripMenuItem miMsrmt;
        public System.Windows.Forms.ToolStripMenuItem miStart;
        public System.Windows.Forms.ToolStripMenuItem miHelp;
        public System.Windows.Forms.ToolStripMenuItem miInfo;
        public System.Windows.Forms.ToolStripMenuItem miFileOpenIni;
        public System.Windows.Forms.ToolStripMenuItem miFileOpenSweep;
        public System.Windows.Forms.ToolStripMenuItem miFileClose;
        public System.Windows.Forms.SplitContainer splitInfoData;
        public System.Windows.Forms.GroupBox gbIniInfo;
        public System.Windows.Forms.RichTextBox tbLog;
        public System.Windows.Forms.GroupBox gbSweepInfo;
        public System.Windows.Forms.StatusStrip statusBar;
        public System.Windows.Forms.ToolStripProgressBar sbProgress;
        public System.Windows.Forms.ListView lvSweepData;
        public System.Windows.Forms.ColumnHeader chSweepNo;
        public System.Windows.Forms.ColumnHeader chU1;
        public System.Windows.Forms.ColumnHeader chU2;
        public System.Windows.Forms.ColumnHeader chU3;
        public System.Windows.Forms.ColumnHeader chI1;
        public System.Windows.Forms.ColumnHeader chI2;
        public System.Windows.Forms.ColumnHeader chI3;
        public System.Windows.Forms.ColumnHeader chI4;
        public System.Windows.Forms.ToolStripStatusLabel sbOperator;
        public System.Windows.Forms.ToolStripStatusLabel sbDescription;
        public System.Windows.Forms.ToolStripMenuItem miPause;
        public System.Windows.Forms.ToolStripMenuItem miStop;
        public System.Windows.Forms.ToolStripStatusLabel sbProgressLabel;
        public System.Windows.Forms.ToolStripStatusLabel sbStatusLabel;
        public System.Windows.Forms.ToolStripStatusLabel sbStatus;
        public System.Windows.Forms.ToolStripStatusLabel sbSpacer;
        public System.Windows.Forms.Timer InitialTimer;
        public System.Windows.Forms.Timer IterativeTimer;
        private System.Windows.Forms.ToolStripMenuItem miFileSaveLastResults;
        private System.Windows.Forms.ToolStripMenuItem miJobQueue;
        private System.Windows.Forms.ToolStripMenuItem miJobQueueCreateOrShow;
        private System.Windows.Forms.ToolStripMenuItem miJobQueueDelete;
        private System.Windows.Forms.ToolStripStatusLabel sbIterates;
        private System.Windows.Forms.ToolStripStatusLabel sbRemaining;
        private HaumChart.HaumChart Chart;
        private System.Windows.Forms.ToolStripMenuItem miChangelog;
        private System.Windows.Forms.ToolStripMenuItem miUpdate;
    }
}

