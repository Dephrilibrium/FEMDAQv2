namespace FEMDAQ.SplashScreen
{
    partial class SplashScreenFrame
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
            this.lToolname = new System.Windows.Forms.Label();
            this.lToolbuild = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pbSplashImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSplashImage)).BeginInit();
            this.SuspendLayout();
            // 
            // lToolname
            // 
            this.lToolname.AutoSize = true;
            this.lToolname.BackColor = System.Drawing.Color.White;
            this.lToolname.Font = new System.Drawing.Font("Lucida Console", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lToolname.Location = new System.Drawing.Point(12, 9);
            this.lToolname.Name = "lToolname";
            this.lToolname.Size = new System.Drawing.Size(213, 36);
            this.lToolname.TabIndex = 2;
            this.lToolname.Text = "FEMDAQ V2";
            // 
            // lToolbuild
            // 
            this.lToolbuild.AutoSize = true;
            this.lToolbuild.BackColor = System.Drawing.Color.White;
            this.lToolbuild.Font = new System.Drawing.Font("Lucida Console", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lToolbuild.Location = new System.Drawing.Point(12, 45);
            this.lToolbuild.Name = "lToolbuild";
            this.lToolbuild.Size = new System.Drawing.Size(158, 16);
            this.lToolbuild.TabIndex = 3;
            this.lToolbuild.Text = "Build: VA.B.C.D";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::FEMDAQ.Properties.Resources.FakANK_Logo;
            this.pictureBox1.Location = new System.Drawing.Point(0, 428);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(512, 172);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // pbSplashImage
            // 
            this.pbSplashImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbSplashImage.Image = global::FEMDAQ.Properties.Resources.SplashScreen;
            this.pbSplashImage.Location = new System.Drawing.Point(0, 0);
            this.pbSplashImage.Name = "pbSplashImage";
            this.pbSplashImage.Size = new System.Drawing.Size(512, 600);
            this.pbSplashImage.TabIndex = 1;
            this.pbSplashImage.TabStop = false;
            // 
            // SplashScreenFrame
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(512, 600);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lToolbuild);
            this.Controls.Add(this.lToolname);
            this.Controls.Add(this.pbSplashImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreenFrame";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSplashImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbSplashImage;
        private System.Windows.Forms.Label lToolname;
        private System.Windows.Forms.Label lToolbuild;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}