using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Cursor = System.Windows.Forms.DataVisualization.Charting.Cursor;
using Files.Parser;

namespace HaumChart
{
    public partial class HaumChart : UserControl
    {
        private int _controlWidth;

        public HaumChart()
        {
            InitializeComponent();
            ChartInfos = new Dictionary<string, AreaInfo>();
            WipeChart();

            _controlWidth = pSidebar.Width;
            //pSidebar.Width = lExpandSidebar.Width;        // Disabled to not fold the sidepanel
            PbPinSidepanel_Click(this, new EventArgs());    // Call to pin sidebar by default
        }


        public new void Dispose()
        {
            WipeChart();
            base.Dispose();
        }



        #region Helper (cleaning, UI-Update, Decimal-Rangechecker, Screenshoter)
        public void WipeChart()
        {
            Areas.Clear();
            ChartInfos.Clear();
            ClearXYAllSeries();
            Series.Clear();
            UpdateSeriesList();
            Titles.Clear();
            Legends.Clear();
        }



        private void UpdateSeriesList()
        {
            lbSeries.Items.Clear();
            foreach (var serie in Series)
                lbSeries.Items.Add(serie.Name);

            if (lbSeries.Items.Count == 0)
                lbSeries.SelectedIndex = -1;
        }


        private void EnsureDoubleInDecimalRange(ref double Val, bool IsLog)
        {
            if (Val > (double)decimal.MaxValue)
                Val = 1e28; // Self-defined max-value!
            else if (Val < (double)decimal.MinValue)
                Val = -1e28; // Self-defined min-value!
            else if ((decimal)Val == 0 && IsLog)
                Val = (double)1e-28m; // minimum Decimal-Delta!

            if (IsLog)
                Val = Math.Abs(Val);
        }

        private void EnsureDoubleInDecimalRange(ref double[] Values, bool IsLog)
        {
            for (var arrIndex = 0; arrIndex < Values.LongLength; arrIndex++)
                EnsureDoubleInDecimalRange(ref Values[arrIndex], IsLog);
        }


        //private void FixBoundariesWhenDoubleExceeds(double Val, AxisInfo AxisInfos)
        //{
        //    if (!double.IsNaN(AxisInfos.Boundaries.LowerBoundary))
        //    {
        //        if (!AxisInfos.Boundaries.LowerIsStatic && Val < AxisInfos.Boundaries.LowerBoundary)
        //            AxisInfos.ChartAxis.Minimum = AxisInfos.Boundaries.LowerBoundary;
        //    }
        //    if (!double.IsNaN(AxisInfos.Boundaries.UpperBoundary))
        //    {
        //        if (!AxisInfos.Boundaries.UpperIsStatic && Val > AxisInfos.Boundaries.UpperBoundary)
        //            AxisInfos.ChartAxis.Maximum = AxisInfos.Boundaries.UpperBoundary;
        //    }
        //}



        //private void FixBoundariesWhenDoubleExceeds(double[] Values, AxisInfo AxisInfos)
        //{
        //    for (var arrIndex = 0; arrIndex < Values.LongLength; arrIndex++)
        //        FixBoundariesWhenDoubleExceeds(Values[arrIndex], AxisInfos);
        //}



        public void SaveChartCapture(string FullFilename, ImageFormat PictureFormat)
        {
            cDataChart.SaveImage(FullFilename, PictureFormat);
        }
        #endregion


        public ChartAreaCollection Areas { get { return cDataChart.ChartAreas; } }
        public SeriesCollection Series { get { return cDataChart.Series; } }
        public TitleCollection Titles { get { return cDataChart.Titles; } }
        public LegendCollection Legends { get { return cDataChart.Legends; } }
        public Dictionary<string, AreaInfo> ChartInfos { get; private set; }



        /*
                 public bool LowerIsStatic;
        public double LowerBoundary;
        public bool UpperIsStatic;
        public double UpperBoundary;
             */
        #region Area-Handling
        public ChartArea AddArea(string AreaName,
                 string AreaTitle,
                 string XAxisLabel,
                 double XAxisLowerBoundary,
                 bool XAxisLowerIsStatic,
                 double XAxisUpperBoundary,
                 bool XAxisUpperIsStatic,
                 int XAxisLogBase,
                 string YAxisLabel,
                 double YAxisLowerBoundary,
                 bool YAxisLowerIsStatic,
                 double YAxisUpperBoundary,
                 bool YAxisUpperIsStatic,
                 int YAxisLogBase,
                 bool ShowLegend)
        {
            // Create axis-boundaries
            // x
            var xBoundaries = new AxisBoundaries();
            xBoundaries.LowerBoundary = XAxisLowerBoundary;
            xBoundaries.LowerIsStatic = XAxisLowerIsStatic;
            xBoundaries.UpperBoundary = XAxisUpperBoundary;
            xBoundaries.UpperIsStatic = XAxisUpperIsStatic;

            // y
            var yBoundaries = new AxisBoundaries();
            yBoundaries.LowerBoundary = YAxisLowerBoundary;
            yBoundaries.LowerIsStatic = YAxisLowerIsStatic;
            yBoundaries.UpperBoundary = YAxisUpperBoundary;
            yBoundaries.UpperIsStatic = YAxisUpperIsStatic;

            // Call executive AddArea-Function
            return AddArea(AreaName, AreaTitle, XAxisLabel, xBoundaries, XAxisLogBase, YAxisLabel, xBoundaries, YAxisLogBase, ShowLegend);
        }



        public ChartArea AddArea(string AreaName,
                         string AreaTitle,
                         string XAxisLabel,
                         AxisBoundaries XAxisBoundaries,
                         int XAxisLogBase,
                         string YAxisLabel,
                         AxisBoundaries YAxisBoundaries,
                         int YAxisLogBase,
                         bool ShowLegend)
        {
            // Create axis infos
            // x
            var xAxisInfo = new AxisInfo();
            xAxisInfo.Label = XAxisLabel;
            xAxisInfo.Boundaries = XAxisBoundaries;
            xAxisInfo.LogBase = XAxisLogBase;

            // y
            var yAxisInfo = new AxisInfo();
            yAxisInfo.Label = YAxisLabel;
            yAxisInfo.Boundaries = YAxisBoundaries;
            yAxisInfo.LogBase = YAxisLogBase;

            // Call executive AddArea-Function
            return AddArea(AreaName, AreaTitle, xAxisInfo, yAxisInfo, ShowLegend);
        }



        public ChartArea AddArea(string AreaName, 
                                 string AreaTitle,
                                 AxisInfo XAxisInfo,
                                 AxisInfo YAxisInfo,
                                 bool ShowLegend)
        {
            var areaInfo = new AreaInfo();
            areaInfo.Name = AreaName;
            areaInfo.Title = AreaTitle;
            areaInfo.XAxis = XAxisInfo;
            areaInfo.YAxis = YAxisInfo;
            areaInfo.ShowLegend = ShowLegend;
            return AddArea(areaInfo);
        }



        public ChartArea AddArea(AreaInfo ChartAreaInfos)
        {
            // Guard clauses
            if (ChartAreaInfos.Name == null || ChartAreaInfos.Name == "") throw new ArgumentException("HaumChart -> AddArea: AreaName Null or \"\"");
            ChartInfos.Add(ChartAreaInfos.Name, ChartAreaInfos);


            // Area search/creation
            var area = cDataChart.ChartAreas.FindByName(ChartInfos[ChartAreaInfos.Name].Name);
            if (area == null)
                area = cDataChart.ChartAreas.Add(ChartInfos[ChartAreaInfos.Name].Name);

            // Cursorsetup
            var xCursor = area.CursorX; // new System.Windows.Forms.DataVisualization.Charting.Cursor();
            xCursor.SelectionColor = Color.DodgerBlue;
            xCursor.IsUserEnabled = true;
            xCursor.IsUserSelectionEnabled = true;
            xCursor.Interval = 0;
            var yCursor = area.CursorY; // new System.Windows.Forms.DataVisualization.Charting.Cursor();
            yCursor.SelectionColor = Color.DodgerBlue;
            yCursor.IsUserEnabled = true;
            yCursor.IsUserSelectionEnabled = true;
            yCursor.Interval = 0;

            // Set scales
            ChartInfos[ChartAreaInfos.Name].XAxis.ChartAxis = area.AxisX;
            ChartInfos[ChartAreaInfos.Name].XAxis.ChartAxis.LabelStyle.Format = "#.##e0";
            if (ChartInfos[ChartAreaInfos.Name].XAxis.Label != null)
                ChartInfos[ChartAreaInfos.Name].XAxis.ChartAxis.Title = ChartInfos[ChartAreaInfos.Name].XAxis.Label;
            if (ChartInfos[ChartAreaInfos.Name].XAxis.LogBase > 1)
            {
                ChartInfos[ChartAreaInfos.Name].XAxis.ChartAxis.LogarithmBase = ChartInfos[ChartAreaInfos.Name].XAxis.LogBase;
                ChartInfos[ChartAreaInfos.Name].XAxis.ChartAxis.IsLogarithmic = true;
            }
            else
                ChartInfos[ChartAreaInfos.Name].XAxis.ChartAxis.IsLogarithmic = false;

            ChartInfos[ChartAreaInfos.Name].YAxis.ChartAxis = area.AxisY;
            ChartInfos[ChartAreaInfos.Name].YAxis.ChartAxis.LabelStyle.Format = "#.##e0";
            if (ChartInfos[ChartAreaInfos.Name].YAxis.Label != null)
                ChartInfos[ChartAreaInfos.Name].YAxis.ChartAxis.Title = ChartInfos[ChartAreaInfos.Name].YAxis.Label;
            if (ChartInfos[ChartAreaInfos.Name].YAxis.LogBase > 1)
            {
                ChartInfos[ChartAreaInfos.Name].YAxis.ChartAxis.LogarithmBase = ChartInfos[ChartAreaInfos.Name].YAxis.LogBase;
                ChartInfos[ChartAreaInfos.Name].YAxis.ChartAxis.IsLogarithmic = true;
            }
            else
                ChartInfos[ChartAreaInfos.Name].YAxis.ChartAxis.IsLogarithmic = false;

            // Check if one of the boundaries is dynamic and turn off exceptions!
            //if ((!ChartInfos[ChartAreaInfos.Name].XAxis.Boundaries.LowerIsStatic && !double.IsNaN(ChartInfos[ChartAreaInfos.Name].XAxis.Boundaries.LowerBoundary))
            // || (!ChartInfos[ChartAreaInfos.Name].XAxis.Boundaries.UpperIsStatic && !double.IsNaN(ChartInfos[ChartAreaInfos.Name].XAxis.Boundaries.UpperBoundary))
            // || (!ChartInfos[ChartAreaInfos.Name].YAxis.Boundaries.LowerIsStatic && !double.IsNaN(ChartInfos[ChartAreaInfos.Name].YAxis.Boundaries.LowerBoundary))
            // || (!ChartInfos[ChartAreaInfos.Name].YAxis.Boundaries.UpperIsStatic && !double.IsNaN(ChartInfos[ChartAreaInfos.Name].YAxis.Boundaries.UpperBoundary)))
            //cDataChart.SuppressExceptions = true;

            // Create boundaries when fixed! (When semi-fixed, they get fixed, during point-insertion
            if (ChartInfos[ChartAreaInfos.Name].XAxis.Boundaries.LowerIsStatic && !double.IsNaN(ChartInfos[ChartAreaInfos.Name].XAxis.Boundaries.LowerBoundary))
                ChartInfos[ChartAreaInfos.Name].XAxis.ChartAxis.Minimum = ChartInfos[ChartAreaInfos.Name].XAxis.Boundaries.LowerBoundary;
            if (ChartInfos[ChartAreaInfos.Name].XAxis.Boundaries.UpperIsStatic && !double.IsNaN(ChartInfos[ChartAreaInfos.Name].XAxis.Boundaries.UpperBoundary))
                ChartInfos[ChartAreaInfos.Name].XAxis.ChartAxis.Maximum = ChartInfos[ChartAreaInfos.Name].XAxis.Boundaries.UpperBoundary;
            if (ChartInfos[ChartAreaInfos.Name].YAxis.Boundaries.LowerIsStatic && !double.IsNaN(ChartInfos[ChartAreaInfos.Name].YAxis.Boundaries.LowerBoundary))
                ChartInfos[ChartAreaInfos.Name].YAxis.ChartAxis.Minimum = ChartInfos[ChartAreaInfos.Name].YAxis.Boundaries.LowerBoundary;
            if (ChartInfos[ChartAreaInfos.Name].YAxis.Boundaries.UpperIsStatic && !double.IsNaN(ChartInfos[ChartAreaInfos.Name].YAxis.Boundaries.UpperBoundary))
                ChartInfos[ChartAreaInfos.Name].YAxis.ChartAxis.Maximum = ChartInfos[ChartAreaInfos.Name].YAxis.Boundaries.UpperBoundary;

            // Grid creation
            ChartInfos[ChartAreaInfos.Name].XAxis.ChartAxis.MajorGrid.LineColor = Color.Gray;
            ChartInfos[ChartAreaInfos.Name].XAxis.ChartAxis.MinorGrid.LineColor = Color.LightGray;
            ChartInfos[ChartAreaInfos.Name].XAxis.ChartAxis.IsStartedFromZero = false;
            ChartInfos[ChartAreaInfos.Name].YAxis.ChartAxis.MajorGrid.LineColor = Color.Gray;
            ChartInfos[ChartAreaInfos.Name].YAxis.ChartAxis.MinorGrid.LineColor = Color.LightGray;
            ChartInfos[ChartAreaInfos.Name].YAxis.ChartAxis.IsStartedFromZero = false;

            // Title creation
            if (ChartInfos[ChartAreaInfos.Name].Title != null)
            {
                var title = cDataChart.Titles.FindByName(ChartInfos[ChartAreaInfos.Name].Title);
                if (title == null)
                    title = cDataChart.Titles.Add(ChartInfos[ChartAreaInfos.Name].Title);
                title.Text = ChartInfos[ChartAreaInfos.Name].Title;
                title.DockedToChartArea = ChartInfos[ChartAreaInfos.Name].Name;
                title.IsDockedInsideChartArea = true;
                title.BackColor = Color.Transparent;
                title.Position.Auto = true;
            }

            // Legend creation
            if (/*ChartInfos[ChartAreaInfos.Name].ShowLegend && */Legends.Count == 0) // One legend is used for all chartAreas! Use only the first one
            {
                var legend = cDataChart.Legends.FindByName(ChartInfos[ChartAreaInfos.Name].Name);
                if (legend == null)
                    legend = cDataChart.Legends.Add(ChartInfos[ChartAreaInfos.Name].Name);
                legend.DockedToChartArea = ChartInfos[ChartAreaInfos.Name].Name;
                legend.Docking = Docking.Top;
                legend.IsDockedInsideChartArea = true;
                legend.LegendStyle = LegendStyle.Table;
                legend.BackColor = Color.Transparent;

                //Set Legend visible checked-property
                cbShowLegend.Checked = ChartInfos[ChartAreaInfos.Name].ShowLegend; // Show or hide legend depending on the first setting
                SetLegendVisible(cbShowLegend.Checked);

            }

            return area;
        }



        public void DeleteArea(string AreaName)
        {
            // Guard clauses
            if (AreaName == null) throw new ArgumentNullException("HaumChart - DeleteArea: AreaName = NULL");

            // Find area
            var area = cDataChart.ChartAreas.FindByName(AreaName);
            if (area == null)
                return;

            // Remove area
            if (area != null)
            {
                cDataChart.ChartAreas.Remove(area);
                area.Dispose();
                //area = null;
                ChartInfos.Remove(area.Name);
            }

            // Remove legend
            var legend = cDataChart.Legends.FindByName(AreaName);
            if (legend != null)
            {
                cDataChart.Legends.Remove(legend);
                legend.Dispose();
                //legend = null;
            }
        }
        #endregion



        #region Series-Handling
        public Series AddSeries(string AreaName, string SeriesName, Color DrawColor, SeriesChartType DrawStyle = SeriesChartType.FastPoint)
        {
            // Guard clauses
            if (AreaName == null) throw new ArgumentNullException("HaumChart - AddSeries: AreaName == NULL");
            if (SeriesName == null || SeriesName == "") throw new ArgumentException("HaumChart - AddSeries: SeriesName == NULL | \"\"");

            // Check for area
            var area = cDataChart.ChartAreas.FindByName(AreaName);
            if (area == null) throw new KeyNotFoundException("HaumChart - AddSeries: Can't find " + AreaName);

            // Series search/creation
            Series series = cDataChart.Series.FindByName(SeriesName);
            if (series != null)
                series = new Series(SeriesName);

            series = cDataChart.Series.Add(SeriesName);
            series.ChartArea = AreaName;
            series.Color = DrawColor;
            series.ChartType = DrawStyle;
            UpdateSeriesList();

            return series;
        }


        public void DeleteSeries(string SeriesName)
        {
            Series series = cDataChart.Series.FindByName(SeriesName);
            if (series == null)
                return;

            series.Points.Clear();
            cDataChart.Series.Remove(series);
            series.Dispose();
            series = null;
            UpdateSeriesList();
        }



        public void AddXY(string SeriesName, double XVal, double YVal)
        {
            // Guard clauses
            if (SeriesName == null) throw new ArgumentNullException("HaumChart - AddXY: SeriesName == NULL");

            // Series search
            var series = cDataChart.Series.FindByName(SeriesName);
            if (series == null) throw new KeyNotFoundException("HaumChart - AddXY: Can't find " + SeriesName);

            // Area search
            var area = cDataChart.ChartAreas.FindByName(series.ChartArea);
            if (area == null) throw new KeyNotFoundException("HaumChart - AddXY: Can't find " + series.ChartArea);

            // Range check for values
            EnsureDoubleInDecimalRange(ref XVal, area.AxisX.IsLogarithmic);
            EnsureDoubleInDecimalRange(ref YVal, area.AxisY.IsLogarithmic);

            // Boundarycheck
            //FixBoundariesWhenDoubleExceeds(XVal, ChartInfos[series.ChartArea].XAxis);
            //FixBoundariesWhenDoubleExceeds(YVal, ChartInfos[series.ChartArea].YAxis);

            // Add point
            series.Points.AddXY(XVal, YVal);
        }



        public void AddXYSet(string SeriesName, List<double> XValues, List<double> YValues)
        {
            AddXYSet(SeriesName, XValues.ToArray(), YValues.ToArray());
        }



        public void AddXYSet(string SeriesName, double[] XValues, double[] YValues)
        {
            // Guard clauses
            if (SeriesName == null) throw new ArgumentNullException("HaumChart - AddXYSet: SeriesName == NULL");
            if (XValues == null) throw new ArgumentNullException("HaumChart - AddXYSet: XValues == NULL");
            if (YValues == null) throw new ArgumentNullException("HaumChart - AddXYSet: YValues == NULL");

            // Series search
            var series = cDataChart.Series.FindByName(SeriesName);
            if (series == null) throw new KeyNotFoundException("HaumChart - AddXY: Can't find " + SeriesName);

            // Area search
            var area = cDataChart.ChartAreas.FindByName(series.ChartArea);
            if (area == null) throw new KeyNotFoundException("HaumChart - AddXY: Can't find " + series.ChartArea);

            for (var valIndex = 0; valIndex < XValues.Length; valIndex++)
            {
                // Range check for values
                EnsureDoubleInDecimalRange(ref XValues[valIndex], area.AxisX.IsLogarithmic);
                EnsureDoubleInDecimalRange(ref XValues[valIndex], area.AxisY.IsLogarithmic);

                // Add point
                series.Points.AddXY(XValues[valIndex], YValues[valIndex]);
            }
        }



        public void DataBindXY(string SeriesName, double[] XValues, double[] YValues)
        {
            // Guard clauses
            if (SeriesName == null) throw new ArgumentNullException("HaumChart - AddXYSet: SeriesName == NULL");
            if (XValues == null) throw new ArgumentNullException("HaumChart - AddXYSet: XValues == NULL");
            if (YValues == null) throw new ArgumentNullException("HaumChart - AddXYSet: YValues == NULL");

            // Series search
            var series = cDataChart.Series.FindByName(SeriesName);
            if (series == null) throw new KeyNotFoundException("HaumChart - DataBindXY: Can't find " + SeriesName);

            // Area search
            var area = cDataChart.ChartAreas.FindByName(series.ChartArea);
            if (area == null) throw new KeyNotFoundException("HaumChart - DataBindXY: Can't find " + series.ChartArea);

            EnsureDoubleInDecimalRange(ref XValues, area.AxisX.IsLogarithmic);
            EnsureDoubleInDecimalRange(ref YValues, area.AxisY.IsLogarithmic);

            series.Points.DataBindXY(XValues, YValues);
        }



        public void ClearXY(string SeriesName)
        {
            var serie = cDataChart.Series.FindByName(SeriesName);
            if (serie == null)
                return;

            serie.Points.Clear();
        }



        public void ClearXYAllSeries()
        {
            foreach (var serie in cDataChart.Series)
                serie.Points.Clear();
        }
        #endregion



        #region UI-Events (Control-Sidepanel, Resize-Chart)
        private void LExpandControls_MouseEnter(object sender, EventArgs e)
        {
            pSidebar.Width = _controlWidth;
            lExpandSidebar.Text = "<";
        }



        private void PChartControls_MouseLeave(object sender, EventArgs e)
        {
            if (IsSidebarPinned)
                return;

            if (!pSidebar.ClientRectangle.Contains(PointToClient(Control.MousePosition)))
            {
                pSidebar.Width = lExpandSidebar.Width;
                lExpandSidebar.Text = ">";
            }
        }




        private void LbSeries_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            var listBox = sender as ListBox;
            var item = listBox.Items[e.Index];

            var itemText = item as string;
            var drawColor = cDataChart.Series[e.Index].Color;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State ^ DrawItemState.Selected, e.ForeColor, Color.Gainsboro);

            e.DrawBackground();
            TextRenderer.DrawText(e.Graphics, itemText, e.Font, e.Bounds, drawColor, e.BackColor, TextFormatFlags.Default);
            e.DrawFocusRectangle();
        }



        public bool IsSidebarPinned;
        private void PbPinSidepanel_Click(object sender, EventArgs e)
        {
            IsSidebarPinned = !IsSidebarPinned;
            if (IsSidebarPinned)
            {
                pbPinSidepanel.Image = FEMDAQ.Properties.Resources.Pinned16;
                cDataChart.Dock = DockStyle.Right;
                cDataChart.Width = Width - pSidebar.Width;
            }
            else
            {
                pbPinSidepanel.Image = FEMDAQ.Properties.Resources.Unpinned16;
                cDataChart.Dock = DockStyle.Fill;
                cDataChart.Width = Width - lExpandSidebar.Width;
            }
        }



        private void PbPinSidepanel_MouseEnter(object sender, EventArgs e)
        {
            pbPinSidepanel.BackColor = Color.LightGray;
        }



        private void PbPinSidepanel_MouseLeave(object sender, EventArgs e)
        {
            pbPinSidepanel.BackColor = Color.Transparent;
        }



        private void HaumChart_Resize(object sender, EventArgs e)
        {
            // Resize the serieslist in height!
            var lbHeight = pSidebar.Height - 70;// 70 is the offset above and below the serieslist (that it looks good)
            if (lbHeight < 0)
                lbHeight = 0;
            lbSeries.Height = lbHeight;

            // Resize the chart in width when the panel is docked!
            if (!IsSidebarPinned)
                return;
            cDataChart.Width = Width - pSidebar.Width;
        }
        #endregion



        #region UI (Chart-Handling)
        private Point _lastMouseLoc;
        private int _lastNearestPointIndex;
        private void CDataChart_MouseMove(object sender, MouseEventArgs e)
        {
            var mouseLoc = e.Location;
            // Check mouse-movement
            if (mouseLoc == _lastMouseLoc)
                return;
            _lastMouseLoc = mouseLoc;

            // Check if a curve for chasing is selected
            if (lbSeries.SelectedIndex < 0)
            {
                ttCapturedPoint.SetToolTip(cDataChart, "No chasing-series selected.");
                return;
            }

            // Series search and getting datapoints
            var serie = cDataChart.Series.FindByName(lbSeries.SelectedItem as string);
            if (serie == null)
            {
                ttCapturedPoint.SetToolTip(cDataChart, "Can't find selected series.");
                return;
            }
            var dataPoints = serie.Points;
            if (dataPoints.Count <= 0)
            {
                ttCapturedPoint.SetToolTip(cDataChart, "No datapoints available");
                return;
            }


            // Area search
            var area = cDataChart.ChartAreas.FindByName(serie.ChartArea);
            if (area == null)
                return;

            // Getting axis and cursors
            var xAxis = area.AxisX;
            var yAxis = area.AxisY;
            if (xAxis == null || yAxis == null)
                return;
            var xCursor = area.CursorX;
            var yCursor = area.CursorY;
            if (xCursor == null || yCursor == null)
                return;

            // Get index nearest data-point to mouselocation on x-axis
            _lastNearestPointIndex = FindNearestPointIndexInDataPointCollection(xAxis, yAxis, mouseLoc, dataPoints);

            // Set cursors in x and y
            SetCapturedPointToolTip(xAxis, yAxis, xCursor, yCursor, dataPoints, _lastNearestPointIndex);
        }



        private void CDataChart_MouseLeave(object sender, EventArgs e)
        {
            ttCapturedPoint.Hide(this);
        }



        private void CDataChart_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                var hitTest = cDataChart.HitTest(e.X, e.Y);
                var hitArea = hitTest.ChartArea;
                if (hitArea == null)
                    return;

                hitArea.AxisX.ScaleView.ZoomReset();
                hitArea.AxisY.ScaleView.ZoomReset();
            }
        }



        private void cbLabelVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (Legends.Count != 0)
                SetLegendVisible(cbShowLegend.Checked);
        }



        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (lbSeries.Items.Count > 0)
            {
                switch (keyData)
                {
                    case Keys.Up:
                        if (lbSeries.SelectedIndex > 0)
                            lbSeries.SelectedIndex--;
                        break;

                    case Keys.Down:
                        if (lbSeries.SelectedIndex < (lbSeries.Items.Count - 1))
                            lbSeries.SelectedIndex++;
                        break;

                    case Keys.Left:
                    case Keys.Right:
                        var chasingSeries = cDataChart.Series.FindByName(lbSeries.SelectedItem as string);
                        if (chasingSeries == null || chasingSeries.Points.Count <= 0)
                            break; // return true! The key was handled but with no action! Otherwise the base.processkey will handle with the standard action!
                        switch (keyData)
                        {
                            case Keys.Left:
                                _lastNearestPointIndex--;
                                if (_lastNearestPointIndex < 0)
                                    _lastNearestPointIndex = 0;
                                break;

                            case Keys.Right:
                                _lastNearestPointIndex++;
                                if (_lastNearestPointIndex >= chasingSeries.Points.Count)
                                    _lastNearestPointIndex = chasingSeries.Points.Count - 1;
                                break;
                        }
                        var chasingArea = cDataChart.ChartAreas.FindByName(chasingSeries.ChartArea);
                        SetCapturedPointToolTip(chasingArea.AxisX, chasingArea.AxisY, chasingArea.CursorX, chasingArea.CursorY, chasingSeries.Points, _lastNearestPointIndex);
                        break;

                    default:
                        break;
                }
            }
            return true; // base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion



        #region UI Helper (Chart-Handling)
        private int FindNearestPointIndexInDataPointCollection(Axis XAxis, Axis YAxis, Point MouseLocation, DataPointCollection DataPoints)
        {
            // The x-range check is necessary, because of the PixelPositionToValue-function. If the x-location is outside of the Chartclient-width it throws an exception
            //  Therefore a check for the mouselocation is needed.
            if (MouseLocation.X < 0)
                MouseLocation.X = 0; // Bring location back into the range of client-width
            if (MouseLocation.X >= cDataChart.Width - 2) // Max-Pixelindex = Width - 2!
                MouseLocation.X = cDataChart.Width - 2;

            // Convert mouseposition to values on axis.
            var xPosAsVal = XAxis.PixelPositionToValue(MouseLocation.X);
            //var yPosAsVal = YAxis.PixelPositionToValue(MouseLocation.Y);

            // Find index nearest data-point to mouselocation on x-axis
            double currentDifference;
            double minimalDifference = 1e30; // Init-Difference has to be bigger than ever other difference
            var nearestPointIndex = 0; // First difference calculated from above!
            for (var pointIndex = 0; pointIndex < DataPoints.Count; pointIndex++)
            {
                currentDifference = Math.Abs(xPosAsVal - DataPoints[pointIndex].XValue);
                if (currentDifference < minimalDifference)
                {
                    minimalDifference = currentDifference;
                    nearestPointIndex = pointIndex;
                }
            }

            return nearestPointIndex;
        }


        // Cursors are specified by using directive on top! (System.Windows.Forms.DataVisualization.Charting.Cursor)
        private void SetCapturedPointToolTip(Axis XAxis, Axis YAxis, Cursor XCursor, Cursor YCursor, DataPointCollection DataPoints, int DataPointIndex)
        {
            if (XAxis.IsLogarithmic)
                XCursor.SetCursorPosition(Math.Log(DataPoints[DataPointIndex].XValue, XAxis.LogarithmBase));
            else
                XCursor.SetCursorPosition(DataPoints[DataPointIndex].XValue);

            if (YAxis.IsLogarithmic)
                YCursor.SetCursorPosition(Math.Log(DataPoints[DataPointIndex].YValues[0], YAxis.LogarithmBase));
            else
                YCursor.SetCursorPosition(DataPoints[DataPointIndex].YValues[0]);

            // Create tooltip with x- and y-values
            ttCapturedPoint.SetToolTip(cDataChart, string.Format("X: {0}; Y: {1}",
                                                                 DataPoints[DataPointIndex].XValue.ToString("E" + nupAccuracy.Value),
                                                                 DataPoints[DataPointIndex].YValues[0].ToString("E" + nupAccuracy.Value)));
        }



        private void SetLegendVisible(bool visible)
        {
            if (Legends.Count != 0)
                Legends[0].Enabled = visible;
        }
        #endregion
    }
}
