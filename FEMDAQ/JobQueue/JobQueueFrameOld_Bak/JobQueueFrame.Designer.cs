namespace FEMDAQ.JobQueue
{
    partial class JobQueueFrame
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JobQueueFrame));
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("");
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.sbbAddJob = new System.Windows.Forms.ToolStripSplitButton();
            this.sbbRemoveJob = new System.Windows.Forms.ToolStripSplitButton();
            this.sbbClearAllJobs = new System.Windows.Forms.ToolStripSplitButton();
            this.sbbMoveUp = new System.Windows.Forms.ToolStripSplitButton();
            this.sbbMoveDown = new System.Windows.Forms.ToolStripSplitButton();
            this.sblJobCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lvJobQueue = new System.Windows.Forms.ListView();
            this.chActiveJob = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chIniFiles = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSwpFiles = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSaveFolder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.queueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startQueueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseQueueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopQueueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadJobQueueListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveJobQueueListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sbbAddJob,
            this.sbbRemoveJob,
            this.sbbClearAllJobs,
            this.sbbMoveUp,
            this.sbbMoveDown,
            this.sblJobCount});
            this.statusStrip1.Location = new System.Drawing.Point(0, 193);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(955, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // sbbAddJob
            // 
            this.sbbAddJob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sbbAddJob.DropDownButtonWidth = 0;
            this.sbbAddJob.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sbbAddJob.Name = "sbbAddJob";
            this.sbbAddJob.Size = new System.Drawing.Size(54, 20);
            this.sbbAddJob.Text = "Add job";
            this.sbbAddJob.ButtonClick += new System.EventHandler(this.sbbAddJob_ButtonClick);
            // 
            // sbbRemoveJob
            // 
            this.sbbRemoveJob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sbbRemoveJob.DropDownButtonWidth = 0;
            this.sbbRemoveJob.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sbbRemoveJob.Name = "sbbRemoveJob";
            this.sbbRemoveJob.Size = new System.Drawing.Size(75, 20);
            this.sbbRemoveJob.Text = "Remove job";
            this.sbbRemoveJob.ButtonClick += new System.EventHandler(this.sbRemoveJob_ButtonClick);
            // 
            // sbbClearAllJobs
            // 
            this.sbbClearAllJobs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sbbClearAllJobs.DropDownButtonWidth = 0;
            this.sbbClearAllJobs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sbbClearAllJobs.Name = "sbbClearAllJobs";
            this.sbbClearAllJobs.Size = new System.Drawing.Size(79, 20);
            this.sbbClearAllJobs.Text = "Clear all jobs";
            this.sbbClearAllJobs.ButtonClick += new System.EventHandler(this.sbClearAllJobs_ButtonClick);
            // 
            // sbbMoveUp
            // 
            this.sbbMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sbbMoveUp.DropDownButtonWidth = 0;
            this.sbbMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("sbbMoveUp.Image")));
            this.sbbMoveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sbbMoveUp.Name = "sbbMoveUp";
            this.sbbMoveUp.Size = new System.Drawing.Size(59, 20);
            this.sbbMoveUp.Text = "Move up";
            this.sbbMoveUp.ButtonClick += new System.EventHandler(this.sbbMoveUp_ButtonClick);
            // 
            // sbbMoveDown
            // 
            this.sbbMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sbbMoveDown.DropDownButtonWidth = 0;
            this.sbbMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("sbbMoveDown.Image")));
            this.sbbMoveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sbbMoveDown.Name = "sbbMoveDown";
            this.sbbMoveDown.Size = new System.Drawing.Size(75, 20);
            this.sbbMoveDown.Text = "Move down";
            this.sbbMoveDown.ButtonClick += new System.EventHandler(this.sbbMoveDown_ButtonClick);
            // 
            // sblJobCount
            // 
            this.sblJobCount.Name = "sblJobCount";
            this.sblJobCount.Size = new System.Drawing.Size(42, 17);
            this.sblJobCount.Text = "Jobs: 0";
            this.sblJobCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lvJobQueue
            // 
            this.lvJobQueue.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chActiveJob,
            this.chIniFiles,
            this.chSwpFiles,
            this.chSaveFolder});
            this.lvJobQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvJobQueue.GridLines = true;
            this.lvJobQueue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lvJobQueue.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem2});
            this.lvJobQueue.Location = new System.Drawing.Point(0, 24);
            this.lvJobQueue.Name = "lvJobQueue";
            this.lvJobQueue.Size = new System.Drawing.Size(955, 169);
            this.lvJobQueue.TabIndex = 2;
            this.lvJobQueue.UseCompatibleStateImageBehavior = false;
            this.lvJobQueue.View = System.Windows.Forms.View.Details;
            this.lvJobQueue.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvJobQueue_ItemSelectionChanged);
            this.lvJobQueue.DoubleClick += new System.EventHandler(this.lvJobQueue_DoubleClick);
            // 
            // chActiveJob
            // 
            this.chActiveJob.Text = "O";
            this.chActiveJob.Width = 44;
            // 
            // chIniFiles
            // 
            this.chIniFiles.Text = "Ini-Files";
            this.chIniFiles.Width = 184;
            // 
            // chSwpFiles
            // 
            this.chSwpFiles.Text = "Sweep-Files";
            this.chSwpFiles.Width = 190;
            // 
            // chSaveFolder
            // 
            this.chSaveFolder.Text = "Save-Folder";
            this.chSaveFolder.Width = 178;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.queueToolStripMenuItem,
            this.listToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(955, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // queueToolStripMenuItem
            // 
            this.queueToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startQueueToolStripMenuItem,
            this.pauseQueueToolStripMenuItem,
            this.stopQueueToolStripMenuItem});
            this.queueToolStripMenuItem.Name = "queueToolStripMenuItem";
            this.queueToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.queueToolStripMenuItem.Text = "Queue";
            // 
            // startQueueToolStripMenuItem
            // 
            this.startQueueToolStripMenuItem.Name = "startQueueToolStripMenuItem";
            this.startQueueToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.startQueueToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.startQueueToolStripMenuItem.Text = "Start queue";
            this.startQueueToolStripMenuItem.Click += new System.EventHandler(this.startQueueToolStripMenuItem_Click);
            // 
            // pauseQueueToolStripMenuItem
            // 
            this.pauseQueueToolStripMenuItem.Name = "pauseQueueToolStripMenuItem";
            this.pauseQueueToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F6)));
            this.pauseQueueToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.pauseQueueToolStripMenuItem.Text = "Pause queue";
            // 
            // stopQueueToolStripMenuItem
            // 
            this.stopQueueToolStripMenuItem.Name = "stopQueueToolStripMenuItem";
            this.stopQueueToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F7)));
            this.stopQueueToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.stopQueueToolStripMenuItem.Text = "Stop queue";
            this.stopQueueToolStripMenuItem.Click += new System.EventHandler(this.stopQueueToolStripMenuItem_Click);
            // 
            // listToolStripMenuItem
            // 
            this.listToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadJobQueueListToolStripMenuItem,
            this.saveJobQueueListToolStripMenuItem});
            this.listToolStripMenuItem.Name = "listToolStripMenuItem";
            this.listToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.listToolStripMenuItem.Text = "List";
            // 
            // loadJobQueueListToolStripMenuItem
            // 
            this.loadJobQueueListToolStripMenuItem.Name = "loadJobQueueListToolStripMenuItem";
            this.loadJobQueueListToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.loadJobQueueListToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadJobQueueListToolStripMenuItem.Text = "Load";
            this.loadJobQueueListToolStripMenuItem.Click += new System.EventHandler(this.loadJobQueueListToolStripMenuItem_Click);
            // 
            // saveJobQueueListToolStripMenuItem
            // 
            this.saveJobQueueListToolStripMenuItem.Enabled = false;
            this.saveJobQueueListToolStripMenuItem.Name = "saveJobQueueListToolStripMenuItem";
            this.saveJobQueueListToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveJobQueueListToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveJobQueueListToolStripMenuItem.Text = "Save";
            this.saveJobQueueListToolStripMenuItem.Click += new System.EventHandler(this.saveJobQueueListToolStripMenuItem_Click);
            // 
            // JobQueueFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 215);
            this.Controls.Add(this.lvJobQueue);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "JobQueueFrame";
            this.Text = "Job Queue";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JobQueueFrame_FormClosing);
            this.SizeChanged += new System.EventHandler(this.JobQueueFrame_SizeChanged);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripSplitButton sbbAddJob;
        private System.Windows.Forms.ToolStripSplitButton sbbRemoveJob;
        private System.Windows.Forms.ToolStripSplitButton sbbClearAllJobs;
        private System.Windows.Forms.ToolStripStatusLabel sblJobCount;
        private System.Windows.Forms.ListView lvJobQueue;
        private System.Windows.Forms.ColumnHeader chIniFiles;
        private System.Windows.Forms.ColumnHeader chSwpFiles;
        private System.Windows.Forms.ColumnHeader chSaveFolder;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem queueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startQueueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseQueueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopQueueToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton sbbMoveUp;
        private System.Windows.Forms.ToolStripSplitButton sbbMoveDown;
        private System.Windows.Forms.ColumnHeader chActiveJob;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadJobQueueListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveJobQueueListToolStripMenuItem;
    }
}