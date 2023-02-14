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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.sbbCopyJob = new System.Windows.Forms.ToolStripSplitButton();
            this.sbbRemoveJob = new System.Windows.Forms.ToolStripSplitButton();
            this.sbbClearAllJobs = new System.Windows.Forms.ToolStripSplitButton();
            this.sbbMoveUp = new System.Windows.Forms.ToolStripSplitButton();
            this.sbbMoveDown = new System.Windows.Forms.ToolStripSplitButton();
            this.sblJobCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbb_RobertMode = new System.Windows.Forms.ToolStripSplitButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.queueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startQueueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseQueueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopQueueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadJobQueueListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveJobQueueListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvJobQueue = new System.Windows.Forms.DataGridView();
            this.dgvRuns = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvRunsFinished = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvIniPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvSweepPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvSavePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvJobQueue)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sbbCopyJob,
            this.sbbRemoveJob,
            this.sbbClearAllJobs,
            this.sbbMoveUp,
            this.sbbMoveDown,
            this.sblJobCount,
            this.toolStripStatusLabel1,
            this.sbb_RobertMode});
            this.statusStrip1.Location = new System.Drawing.Point(0, 193);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(955, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // sbbCopyJob
            // 
            this.sbbCopyJob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sbbCopyJob.DropDownButtonWidth = 0;
            this.sbbCopyJob.Image = ((System.Drawing.Image)(resources.GetObject("sbbCopyJob.Image")));
            this.sbbCopyJob.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sbbCopyJob.Name = "sbbCopyJob";
            this.sbbCopyJob.Size = new System.Drawing.Size(65, 20);
            this.sbbCopyJob.Text = "Copy jobs";
            this.sbbCopyJob.ButtonClick += new System.EventHandler(this.sbbCopyJob_ButtonClick);
            // 
            // sbbRemoveJob
            // 
            this.sbbRemoveJob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sbbRemoveJob.DropDownButtonWidth = 0;
            this.sbbRemoveJob.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sbbRemoveJob.Name = "sbbRemoveJob";
            this.sbbRemoveJob.Size = new System.Drawing.Size(80, 20);
            this.sbbRemoveJob.Text = "Remove jobs";
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
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(457, 17);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // sbb_RobertMode
            // 
            this.sbb_RobertMode.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.sbb_RobertMode.BackColor = System.Drawing.Color.Red;
            this.sbb_RobertMode.BackgroundImage = global::FEMDAQ.Properties.Resources.DummyBMP_Pixel;
            this.sbb_RobertMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.sbb_RobertMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sbb_RobertMode.DropDownButtonWidth = 0;
            this.sbb_RobertMode.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.sbb_RobertMode.Name = "sbb_RobertMode";
            this.sbb_RobertMode.Size = new System.Drawing.Size(83, 20);
            this.sbb_RobertMode.Text = "Robert-Mode";
            this.sbb_RobertMode.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.sbb_RobertMode.ButtonClick += new System.EventHandler(this.sbb_RobertMode_ButtonClick);
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
            this.loadJobQueueListToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.loadJobQueueListToolStripMenuItem.Text = "Load";
            this.loadJobQueueListToolStripMenuItem.Click += new System.EventHandler(this.loadJobQueueListToolStripMenuItem_Click);
            // 
            // saveJobQueueListToolStripMenuItem
            // 
            this.saveJobQueueListToolStripMenuItem.Enabled = false;
            this.saveJobQueueListToolStripMenuItem.Name = "saveJobQueueListToolStripMenuItem";
            this.saveJobQueueListToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveJobQueueListToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.saveJobQueueListToolStripMenuItem.Text = "Save";
            this.saveJobQueueListToolStripMenuItem.Click += new System.EventHandler(this.saveJobQueueListToolStripMenuItem_Click);
            // 
            // dgvJobQueue
            // 
            this.dgvJobQueue.BackgroundColor = System.Drawing.Color.White;
            this.dgvJobQueue.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvJobQueue.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvJobQueue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvJobQueue.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvRuns,
            this.dgvRunsFinished,
            this.dgvIniPath,
            this.dgvSweepPath,
            this.dgvSavePath});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvJobQueue.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvJobQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvJobQueue.EnableHeadersVisualStyles = false;
            this.dgvJobQueue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dgvJobQueue.Location = new System.Drawing.Point(0, 24);
            this.dgvJobQueue.Name = "dgvJobQueue";
            this.dgvJobQueue.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvJobQueue.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.Format = "N0";
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvJobQueue.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvJobQueue.RowHeadersWidth = 50;
            dataGridViewCellStyle4.NullValue = null;
            this.dgvJobQueue.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvJobQueue.Size = new System.Drawing.Size(955, 169);
            this.dgvJobQueue.TabIndex = 4;
            this.dgvJobQueue.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvJobQueue_CellEndEdit);
            this.dgvJobQueue.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvJobQueue_RowsAdded);
            this.dgvJobQueue.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvJobQueue_RowsRemoved);
            this.dgvJobQueue.SelectionChanged += new System.EventHandler(this.dgvJobQueue_SelectionChanged);
            this.dgvJobQueue.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dgvJobQueue_MouseDoubleClick);
            // 
            // dgvRuns
            // 
            this.dgvRuns.HeaderText = "Runs";
            this.dgvRuns.Name = "dgvRuns";
            this.dgvRuns.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRuns.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvRuns.Width = 55;
            // 
            // dgvRunsFinished
            // 
            this.dgvRunsFinished.HeaderText = "Finished";
            this.dgvRunsFinished.Name = "dgvRunsFinished";
            this.dgvRunsFinished.ReadOnly = true;
            this.dgvRunsFinished.ToolTipText = "Current run of this job";
            this.dgvRunsFinished.Width = 55;
            // 
            // dgvIniPath
            // 
            this.dgvIniPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvIniPath.HeaderText = "Ini-Path";
            this.dgvIniPath.Name = "dgvIniPath";
            this.dgvIniPath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvSweepPath
            // 
            this.dgvSweepPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvSweepPath.HeaderText = "Sweep-Path";
            this.dgvSweepPath.Name = "dgvSweepPath";
            this.dgvSweepPath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvSavePath
            // 
            this.dgvSavePath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvSavePath.HeaderText = "Savepath";
            this.dgvSavePath.Name = "dgvSavePath";
            this.dgvSavePath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // JobQueueFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 215);
            this.Controls.Add(this.dgvJobQueue);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "JobQueueFrame";
            this.Text = "FEMDAQ - Job Queue";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JobQueueFrame_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvJobQueue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripSplitButton sbbRemoveJob;
        private System.Windows.Forms.ToolStripSplitButton sbbClearAllJobs;
        private System.Windows.Forms.ToolStripStatusLabel sblJobCount;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem queueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startQueueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseQueueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopQueueToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton sbbMoveUp;
        private System.Windows.Forms.ToolStripSplitButton sbbMoveDown;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadJobQueueListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveJobQueueListToolStripMenuItem;
        private System.Windows.Forms.DataGridView dgvJobQueue;
        private System.Windows.Forms.ToolStripSplitButton sbbCopyJob;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripSplitButton sbb_RobertMode;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvRuns;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvRunsFinished;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvIniPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvSweepPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvSavePath;
    }
}