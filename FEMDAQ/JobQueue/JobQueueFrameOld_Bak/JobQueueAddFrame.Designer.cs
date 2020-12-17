namespace FEMDAQ.JobQueue
{
    partial class JobQueueAddFrame
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JobQueueAddFrame));
            this.tbIniFile = new System.Windows.Forms.TextBox();
            this.lIniFile = new System.Windows.Forms.Label();
            this.lSweepFile = new System.Windows.Forms.Label();
            this.tbSweepFile = new System.Windows.Forms.TextBox();
            this.lSaveFolder = new System.Windows.Forms.Label();
            this.tbSaveFolder = new System.Windows.Forms.TextBox();
            this.bAdd = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbIniFile
            // 
            this.tbIniFile.Location = new System.Drawing.Point(12, 25);
            this.tbIniFile.Name = "tbIniFile";
            this.tbIniFile.Size = new System.Drawing.Size(288, 20);
            this.tbIniFile.TabIndex = 0;
            this.tbIniFile.DoubleClick += new System.EventHandler(this.tbIniFile_DoubleClick);
            // 
            // lIniFile
            // 
            this.lIniFile.AutoSize = true;
            this.lIniFile.Location = new System.Drawing.Point(12, 9);
            this.lIniFile.Name = "lIniFile";
            this.lIniFile.Size = new System.Drawing.Size(37, 13);
            this.lIniFile.TabIndex = 1;
            this.lIniFile.Text = "Ini-file:";
            // 
            // lSweepFile
            // 
            this.lSweepFile.AutoSize = true;
            this.lSweepFile.Location = new System.Drawing.Point(12, 48);
            this.lSweepFile.Name = "lSweepFile";
            this.lSweepFile.Size = new System.Drawing.Size(59, 13);
            this.lSweepFile.TabIndex = 3;
            this.lSweepFile.Text = "Sweep-file:";
            // 
            // tbSweepFile
            // 
            this.tbSweepFile.Location = new System.Drawing.Point(12, 64);
            this.tbSweepFile.Name = "tbSweepFile";
            this.tbSweepFile.Size = new System.Drawing.Size(288, 20);
            this.tbSweepFile.TabIndex = 2;
            this.tbSweepFile.DoubleClick += new System.EventHandler(this.tbSweepFile_DoubleClick);
            // 
            // lSaveFolder
            // 
            this.lSaveFolder.AutoSize = true;
            this.lSaveFolder.Location = new System.Drawing.Point(12, 87);
            this.lSaveFolder.Name = "lSaveFolder";
            this.lSaveFolder.Size = new System.Drawing.Size(64, 13);
            this.lSaveFolder.TabIndex = 5;
            this.lSaveFolder.Text = "Save-folder:";
            // 
            // tbSaveFolder
            // 
            this.tbSaveFolder.Location = new System.Drawing.Point(12, 103);
            this.tbSaveFolder.Name = "tbSaveFolder";
            this.tbSaveFolder.Size = new System.Drawing.Size(288, 20);
            this.tbSaveFolder.TabIndex = 4;
            this.tbSaveFolder.DoubleClick += new System.EventHandler(this.tbSaveFolder_DoubleClick);
            // 
            // bAdd
            // 
            this.bAdd.Location = new System.Drawing.Point(12, 129);
            this.bAdd.Name = "bAdd";
            this.bAdd.Size = new System.Drawing.Size(75, 23);
            this.bAdd.TabIndex = 6;
            this.bAdd.Text = "Add job";
            this.bAdd.UseVisualStyleBackColor = true;
            this.bAdd.Click += new System.EventHandler(this.bAdd_Click);
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(93, 129);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 7;
            this.bCancel.Text = "Abort";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // JobQueueAddFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 160);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bAdd);
            this.Controls.Add(this.lSaveFolder);
            this.Controls.Add(this.tbSaveFolder);
            this.Controls.Add(this.lSweepFile);
            this.Controls.Add(this.tbSweepFile);
            this.Controls.Add(this.lIniFile);
            this.Controls.Add(this.tbIniFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "JobQueueAddFrame";
            this.Text = "Add job...";
            this.Shown += new System.EventHandler(this.JobQueueAddFrame_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbIniFile;
        private System.Windows.Forms.Label lIniFile;
        private System.Windows.Forms.Label lSweepFile;
        private System.Windows.Forms.TextBox tbSweepFile;
        private System.Windows.Forms.Label lSaveFolder;
        private System.Windows.Forms.TextBox tbSaveFolder;
        private System.Windows.Forms.Button bAdd;
        private System.Windows.Forms.Button bCancel;
    }
}