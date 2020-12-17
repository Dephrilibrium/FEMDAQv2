namespace FEMDAQ
{
    partial class SavingPopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SavingPopup));
            this.lSavingFiles = new System.Windows.Forms.Label();
            this.pbSaveStatus = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // lSavingFiles
            // 
            this.lSavingFiles.AutoSize = true;
            this.lSavingFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lSavingFiles.Location = new System.Drawing.Point(12, 9);
            this.lSavingFiles.Name = "lSavingFiles";
            this.lSavingFiles.Size = new System.Drawing.Size(115, 17);
            this.lSavingFiles.TabIndex = 0;
            this.lSavingFiles.Text = "Saving files (x/y):";
            // 
            // pbSaveStatus
            // 
            this.pbSaveStatus.Location = new System.Drawing.Point(12, 29);
            this.pbSaveStatus.Name = "pbSaveStatus";
            this.pbSaveStatus.Size = new System.Drawing.Size(134, 23);
            this.pbSaveStatus.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbSaveStatus.TabIndex = 1;
            // 
            // SavingPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(158, 63);
            this.Controls.Add(this.pbSaveStatus);
            this.Controls.Add(this.lSavingFiles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SavingPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FEMDAQ - Saving";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lSavingFiles;
        private System.Windows.Forms.ProgressBar pbSaveStatus;
    }
}