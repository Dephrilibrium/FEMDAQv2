using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace HaumChart
{
    public enum AxisDimensionDirection { x, y, z }



    public class AreaInfo
    {
        public string Name;
        public string Title;
        public AxisInfo XAxis;
        public AxisInfo YAxis;
        public bool ShowLegend;

        public AreaInfo()
        {
            XAxis = new AxisInfo();
            YAxis = new AxisInfo();
        }
    }



    public class AxisInfo
    {
        public Axis ChartAxis;
        public AxisDimensionDirection DimensionDirection;
        public string Label;
        public AxisBoundaries Boundaries;
        public int LogBase;

        public AxisInfo()
        {
            Boundaries = new AxisBoundaries();
        }
    }



    public class AxisBoundaries
    {
        public bool LowerIsStatic;
        public double LowerBoundary;
        public bool UpperIsStatic;
        public double UpperBoundary;

        public AxisBoundaries()
        {
        }
    }
}
