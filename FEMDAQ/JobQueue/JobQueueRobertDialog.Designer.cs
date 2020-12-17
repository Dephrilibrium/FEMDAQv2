namespace FEMDAQ.JobQueue
{
    partial class JobQueueRobertDialog
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
            this.bNextJob = new System.Windows.Forms.Button();
            this.tbQuestion = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bRepeatJob = new System.Windows.Forms.Button();
            this.bAbortQueue = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // bNextJob
            // 
            this.bNextJob.Location = new System.Drawing.Point(12, 50);
            this.bNextJob.Name = "bNextJob";
            this.bNextJob.Size = new System.Drawing.Size(76, 61);
            this.bNextJob.TabIndex = 0;
            this.bNextJob.Text = "Next job";
            this.bNextJob.UseVisualStyleBackColor = true;
            // 
            // tbQuestion
            // 
            this.tbQuestion.BackColor = System.Drawing.SystemColors.Control;
            this.tbQuestion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbQuestion.Cursor = System.Windows.Forms.Cursors.No;
            this.tbQuestion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbQuestion.Location = new System.Drawing.Point(50, 18);
            this.tbQuestion.Multiline = true;
            this.tbQuestion.Name = "tbQuestion";
            this.tbQuestion.Size = new System.Drawing.Size(158, 26);
            this.tbQuestion.TabIndex = 3;
            this.tbQuestion.Text = "Select your next step.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // bRepeatJob
            // 
            this.bRepeatJob.Location = new System.Drawing.Point(94, 50);
            this.bRepeatJob.Name = "bRepeatJob";
            this.bRepeatJob.Size = new System.Drawing.Size(76, 61);
            this.bRepeatJob.TabIndex = 5;
            this.bRepeatJob.Text = "Repeat job";
            this.bRepeatJob.UseVisualStyleBackColor = true;
            // 
            // bAbortQueue
            // 
            this.bAbortQueue.Location = new System.Drawing.Point(176, 50);
            this.bAbortQueue.Name = "bAbortQueue";
            this.bAbortQueue.Size = new System.Drawing.Size(76, 61);
            this.bAbortQueue.TabIndex = 6;
            this.bAbortQueue.Text = "Stop queue";
            this.bAbortQueue.UseVisualStyleBackColor = true;
            // 
            // JobQueueRobertDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 120);
            this.ControlBox = false;
            this.Controls.Add(this.bAbortQueue);
            this.Controls.Add(this.bRepeatJob);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tbQuestion);
            this.Controls.Add(this.bNextJob);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "JobQueueRobertDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Job-Queue (Robert-Mode dialog)";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bNextJob;
        private System.Windows.Forms.TextBox tbQuestion;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button bRepeatJob;
        private System.Windows.Forms.Button bAbortQueue;
    }
}