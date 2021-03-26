namespace HaumChart
{
    partial class HaumChart
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

        #region Vom Komponenten-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.cDataChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pSidebar = new System.Windows.Forms.Panel();
            this.cbShowLegend = new System.Windows.Forms.CheckBox();
            this.pbPinSidepanel = new System.Windows.Forms.PictureBox();
            this.lChasingSeries = new System.Windows.Forms.Label();
            this.nupAccuracy = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.lbSeries = new System.Windows.Forms.ListBox();
            this.lExpandSidebar = new System.Windows.Forms.Label();
            this.ttCapturedPoint = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cDataChart)).BeginInit();
            this.pSidebar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPinSidepanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupAccuracy)).BeginInit();
            this.SuspendLayout();
            // 
            // cDataChart
            // 
            chartArea1.Name = "ChartArea1";
            this.cDataChart.ChartAreas.Add(chartArea1);
            this.cDataChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.cDataChart.Legends.Add(legend1);
            this.cDataChart.Location = new System.Drawing.Point(0, 0);
            this.cDataChart.MinimumSize = new System.Drawing.Size(50, 20);
            this.cDataChart.Name = "cDataChart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.cDataChart.Series.Add(series1);
            this.cDataChart.Size = new System.Drawing.Size(754, 416);
            this.cDataChart.TabIndex = 0;
            this.cDataChart.Text = "dataChart";
            this.cDataChart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CDataChart_MouseClick);
            this.cDataChart.MouseLeave += new System.EventHandler(this.CDataChart_MouseLeave);
            this.cDataChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CDataChart_MouseMove);
            // 
            // pSidebar
            // 
            this.pSidebar.BackColor = System.Drawing.Color.White;
            this.pSidebar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pSidebar.Controls.Add(this.cbShowLegend);
            this.pSidebar.Controls.Add(this.pbPinSidepanel);
            this.pSidebar.Controls.Add(this.lChasingSeries);
            this.pSidebar.Controls.Add(this.nupAccuracy);
            this.pSidebar.Controls.Add(this.label1);
            this.pSidebar.Controls.Add(this.lbSeries);
            this.pSidebar.Controls.Add(this.lExpandSidebar);
            this.pSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pSidebar.Location = new System.Drawing.Point(0, 0);
            this.pSidebar.Name = "pSidebar";
            this.pSidebar.Size = new System.Drawing.Size(167, 416);
            this.pSidebar.TabIndex = 1;
            this.pSidebar.MouseLeave += new System.EventHandler(this.PChartControls_MouseLeave);
            // 
            // cbShowLegend
            // 
            this.cbShowLegend.AutoSize = true;
            this.cbShowLegend.Location = new System.Drawing.Point(28, 35);
            this.cbShowLegend.Name = "cbShowLegend";
            this.cbShowLegend.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbShowLegend.Size = new System.Drawing.Size(91, 17);
            this.cbShowLegend.TabIndex = 7;
            this.cbShowLegend.Text = ":Show legend";
            this.cbShowLegend.UseVisualStyleBackColor = true;
            this.cbShowLegend.CheckedChanged += new System.EventHandler(this.cbLabelVisible_CheckedChanged);
            // 
            // pbPinSidepanel
            // 
            this.pbPinSidepanel.Image = global::FEMDAQ.Properties.Resources.Unpinned16;
            this.pbPinSidepanel.Location = new System.Drawing.Point(149, 0);
            this.pbPinSidepanel.Margin = new System.Windows.Forms.Padding(0);
            this.pbPinSidepanel.Name = "pbPinSidepanel";
            this.pbPinSidepanel.Size = new System.Drawing.Size(16, 16);
            this.pbPinSidepanel.TabIndex = 6;
            this.pbPinSidepanel.TabStop = false;
            this.pbPinSidepanel.Click += new System.EventHandler(this.PbPinSidepanel_Click);
            this.pbPinSidepanel.MouseEnter += new System.EventHandler(this.PbPinSidepanel_MouseEnter);
            this.pbPinSidepanel.MouseLeave += new System.EventHandler(this.PbPinSidepanel_MouseLeave);
            // 
            // lChasingSeries
            // 
            this.lChasingSeries.AutoSize = true;
            this.lChasingSeries.BackColor = System.Drawing.Color.Transparent;
            this.lChasingSeries.Location = new System.Drawing.Point(25, 55);
            this.lChasingSeries.Name = "lChasingSeries";
            this.lChasingSeries.Size = new System.Drawing.Size(68, 13);
            this.lChasingSeries.TabIndex = 5;
            this.lChasingSeries.Text = "Chasing plot:";
            // 
            // nupAccuracy
            // 
            this.nupAccuracy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nupAccuracy.Location = new System.Drawing.Point(86, 9);
            this.nupAccuracy.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.nupAccuracy.Name = "nupAccuracy";
            this.nupAccuracy.Size = new System.Drawing.Size(35, 20);
            this.nupAccuracy.TabIndex = 4;
            this.nupAccuracy.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(25, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Accuracy:";
            // 
            // lbSeries
            // 
            this.lbSeries.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbSeries.ForeColor = System.Drawing.Color.Black;
            this.lbSeries.FormattingEnabled = true;
            this.lbSeries.Location = new System.Drawing.Point(27, 73);
            this.lbSeries.Margin = new System.Windows.Forms.Padding(5);
            this.lbSeries.Name = "lbSeries";
            this.lbSeries.Size = new System.Drawing.Size(132, 329);
            this.lbSeries.TabIndex = 1;
            this.lbSeries.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.LbSeries_DrawItem);
            // 
            // lExpandSidebar
            // 
            this.lExpandSidebar.BackColor = System.Drawing.Color.Transparent;
            this.lExpandSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.lExpandSidebar.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lExpandSidebar.Location = new System.Drawing.Point(0, 0);
            this.lExpandSidebar.Name = "lExpandSidebar";
            this.lExpandSidebar.Size = new System.Drawing.Size(19, 414);
            this.lExpandSidebar.TabIndex = 0;
            this.lExpandSidebar.Text = ">";
            this.lExpandSidebar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lExpandSidebar.MouseEnter += new System.EventHandler(this.LExpandControls_MouseEnter);
            // 
            // HaumChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pSidebar);
            this.Controls.Add(this.cDataChart);
            this.Name = "HaumChart";
            this.Size = new System.Drawing.Size(754, 416);
            this.Resize += new System.EventHandler(this.HaumChart_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.cDataChart)).EndInit();
            this.pSidebar.ResumeLayout(false);
            this.pSidebar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPinSidepanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupAccuracy)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart cDataChart;
        private System.Windows.Forms.Panel pSidebar;
        private System.Windows.Forms.Label lExpandSidebar;
        private System.Windows.Forms.NumericUpDown nupAccuracy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbSeries;
        private System.Windows.Forms.Label lChasingSeries;
        private System.Windows.Forms.PictureBox pbPinSidepanel;
        private System.Windows.Forms.ToolTip ttCapturedPoint;
        private System.Windows.Forms.CheckBox cbShowLegend;
    }
}
