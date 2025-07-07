namespace FEMDAQ.Instrumentwindows.PyCam2Statuswindow
{
    partial class PyCam2Statuswindow
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsPicam2Downloads = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsPiCam2ActiveDownloads = new System.Windows.Forms.ToolStripStatusLabel();
            this.rtbPiCam2Status = new System.Windows.Forms.RichTextBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsPicam2Downloads,
            this.tsPiCam2ActiveDownloads});
            this.statusStrip1.Location = new System.Drawing.Point(0, 188);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(588, 26);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsPicam2Downloads
            // 
            this.tsPicam2Downloads.Name = "tsPicam2Downloads";
            this.tsPicam2Downloads.Size = new System.Drawing.Size(196, 20);
            this.tsPicam2Downloads.Text = "Downloads (Active/Started):";
            // 
            // tsPiCam2ActiveDownloads
            // 
            this.tsPiCam2ActiveDownloads.Name = "tsPiCam2ActiveDownloads";
            this.tsPiCam2ActiveDownloads.Size = new System.Drawing.Size(144, 20);
            this.tsPiCam2ActiveDownloads.Text = "<Active>/<Started>";
            // 
            // rtbPiCam2Status
            // 
            this.rtbPiCam2Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbPiCam2Status.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.rtbPiCam2Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbPiCam2Status.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbPiCam2Status.Location = new System.Drawing.Point(0, 0);
            this.rtbPiCam2Status.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rtbPiCam2Status.MaxLength = 65535;
            this.rtbPiCam2Status.Name = "rtbPiCam2Status";
            this.rtbPiCam2Status.ReadOnly = true;
            this.rtbPiCam2Status.Size = new System.Drawing.Size(588, 188);
            this.rtbPiCam2Status.TabIndex = 3;
            this.rtbPiCam2Status.Text = "Testtext\n\num zu sehen\nwie das\nso ist\n\nYYMMDD - HH:MM:SS:\n";
            // 
            // PyCam2Statuswindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 214);
            this.ControlBox = false;
            this.Controls.Add(this.rtbPiCam2Status);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "PyCam2Statuswindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "PyCam2 Status of <User>@<IP:Port>";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsPicam2Downloads;
        private System.Windows.Forms.ToolStripStatusLabel tsPiCam2ActiveDownloads;
        private System.Windows.Forms.RichTextBox rtbPiCam2Status;
    }
}