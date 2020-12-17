using FEMDAQ.StaticHelper;
using HaumChart;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Files.Parser
{
    public class InfoBlockChart
    {
        public AreaInfo ChartInfo;

        //public string ChartName;
        //public string XAxisTitle;
        //public string YAxisTitle;
        //public AxisInfo XAxisBoundaries;
        //public AxisInfo YAxisBoundaries;
        //public int XAxisLogBase;
        //public int YAxisLogBase;
        //public int ShowLegend;



        public InfoBlockChart(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            ChartInfo = new AreaInfo();
            string lineInfo = null;

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "[Chart");
            ChartInfo.Name = ParseChartIdentifier(lineInfo);
            ChartInfo.Title = ParseChartname(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "XAxisTitle=");
            ChartInfo.XAxis.Label = ParseHelper.ParseStringValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "YAxisTitle=");
            ChartInfo.YAxis.Label = ParseHelper.ParseStringValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "XAxisBoundaries=");
            ChartInfo.XAxis.DimensionDirection = AxisDimensionDirection.x;
            if (lineInfo != null)
                try { ParseAxisBoundaries(ParseHelper.ParseStringValueFromLineInfo(lineInfo), ref ChartInfo.XAxis.Boundaries); }
                catch (Exception e) { throw new FormatException("Can' parse boundaries of x-axis", e); }

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "YAxisBoundaries=");
            ChartInfo.YAxis.DimensionDirection = AxisDimensionDirection.y;
            if (lineInfo != null)
                try { ParseAxisBoundaries(ParseHelper.ParseStringValueFromLineInfo(lineInfo), ref ChartInfo.YAxis.Boundaries); }
                catch (Exception e) { throw new FormatException("Can' parse boundaries of y-axis", e); }

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "XAxisLogBase=");
            ChartInfo.XAxis.LogBase = (int)ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "YAxisLogBase=");
            ChartInfo.YAxis.LogBase = (int)ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "ShowLegend=");
            var booleanHelper = (int)ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);
            ChartInfo.ShowLegend = (booleanHelper != 0 ? true : false);
        }



        private string ParseChartIdentifier(string info)
        {
            var splitInfo = info.Split(new char[] { '[', '|', ']' }, StringSplitOptions.RemoveEmptyEntries);

            if (splitInfo == null || splitInfo.Length < 2)
                throw new FormatException("Line: " + info);

            return splitInfo[0];
        }



        private string ParseChartname(string info)
        {
            var splitInfo = info.Split(new char[] { '[', '|', ']' }, StringSplitOptions.RemoveEmptyEntries);

            if (splitInfo == null || splitInfo.Length < 2)
                throw new FormatException("Line: " + info);

            return splitInfo[1];
        }



        private void ParseAxisBoundaries(string lineInfo, ref AxisBoundaries boundaries)
        {
            var bounds = StringHelper.TrimArray(lineInfo.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));

            if (bounds.Length != 2)
                throw new FormatException("InfoBlockChart - ParseAxisBoundaries: Invalid count of arguments (" + bounds.Length + " of 2)");

            // Lower bound
            try
            {
                // Check if boundary should be static
                if (bounds[0].StartsWith("@"))
                {
                    bounds[0] = bounds[0].Remove(0, 1); // Remove static-indicator
                    boundaries.LowerIsStatic = true;
                }
                boundaries.LowerBoundary = double.Parse(bounds[0]);
                // Use NaN (no limitation) when 0 is given
                if (boundaries.LowerBoundary == 0)
                    boundaries.LowerBoundary = double.NaN;
            }
            catch (Exception e) { throw new FormatException("InfoBlockChart - ParseAxisBoundaries: Invalid format of lower-bound! Valid format: ([@]lower|[@]upper)\r\n\r\n" + e.Message); }


            // Upper bound
            try
            {
                // Check if boundary should be static
                if (bounds[1].StartsWith("@"))
                {
                    bounds[1] = bounds[1].Remove(0, 1); // Remove static-indicator
                    boundaries.UpperIsStatic = true;
                }
                boundaries.UpperBoundary = double.Parse(bounds[1]);
                // Use NaN (no limitation) when 0 is given
                if (boundaries.UpperBoundary == 0)
                    boundaries.UpperBoundary = double.NaN;

            }
            catch (Exception e) { throw new FormatException("InfoBlockChart - ParseAxisBoundaries: Invalid format of upper-bound! Valid format: [@]lower|[@]upper\r\n\r\n" + e.Message); }
        }

    }
}
