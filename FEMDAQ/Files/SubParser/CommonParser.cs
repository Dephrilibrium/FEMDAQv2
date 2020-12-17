using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Files.Parser
{
    public class CommonParser
    {
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string CustomName { get; private set; }

        public List<string> ChartIdentifiers { get; private set; }
        public List<string> ChartDrawnOvers { get; private set; }
        public List<Color> ChartColors { get; private set; }
        public string Comment { get; private set; }



        public CommonParser(IEnumerable<string> infoBlock, string chartIdentifierToken = "ChartIdentifier=", string chartDrawnOverToken = "ChartDrawnOver=", string chartColorToken = "ChartColor=", string customNameToken = "CustomName=", string commentToken = "Comment=")
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            ParseDevice(StringHelper.FindStringWhichStartsWith(infoBlock, "[Dev"));
            ParseCustomName(StringHelper.FindStringWhichStartsWith(infoBlock, customNameToken));
            ParseChartIdentifiers(StringHelper.FindStringWhichStartsWith(infoBlock, chartIdentifierToken));
            ParseChartDrawnOvers(StringHelper.FindStringWhichStartsWith(infoBlock, chartDrawnOverToken));
            ParseChartColors(StringHelper.FindStringWhichStartsWith(infoBlock, chartColorToken));
            ParseComment(StringHelper.FindStringWhichStartsWith(infoBlock, commentToken));


            if (ChartColors.Count < ChartIdentifiers.Count)
                throw new FormatException("There are more charts than given colors for [" + DeviceIdentifier + "|" + DeviceType + "] (Customname: " + CustomName + "). (Colors < ChartIdentifiers)\r\n\r\nPlease correct your settings and try again.");
            if (ChartDrawnOvers.Count < ChartIdentifiers.Count)
                throw new FormatException("There are more charts than measure-curves for [" + DeviceIdentifier + "|" + DeviceType + "] (Customname: " + CustomName + "). (DrawnOvers < ChartIdentifiers)\r\n\r\nPlease correct your settings and try again.");
        }


        private void ParseDevice(string info)
        {
            if (info == null)
                return; // Just ignore a non-existing token

            var parseHelper = info.Split(new char[] { '[', '|', ']' }, StringSplitOptions.RemoveEmptyEntries);
            if (parseHelper == null || parseHelper.Length < 2)
                throw new FormatException("Line: " + info);

            DeviceIdentifier = parseHelper[0];
            DeviceType = parseHelper[1];
        }


        private void ParseCustomName(string info)
        {
            if (info == null)
            {
                CustomName = null;
                return;
            }

            var parseHelper = ParseHelper.ParseStringValueFromLineInfo(info);
            if (parseHelper == "")
                CustomName = null; // No name given!
            else
                CustomName = parseHelper;
        }


        private void ParseChartIdentifiers(string info)
        {
            ChartIdentifiers = new List<string>();

            if (info == null)
                return; // Ignore info
            var lineInfo = ParseHelper.ParseStringValueFromLineInfo(info);
            var chartIdentifiers = lineInfo.Split(new char[] { ',' });
            foreach (var chartIdentifier in chartIdentifiers)
                ChartIdentifiers.Add(StringHelper.TrimString(chartIdentifier));
        }



        private void ParseChartDrawnOvers(string info)
        {
            ChartDrawnOvers = new List<string>();

            if (info == null)
            {
                ChartDrawnOvers.Add("time");  // Necessary for the RTO!
                return; // Ignore info
            }
            var lineInfo = ParseHelper.ParseStringValueFromLineInfo(info);
            var drawnOverValues = lineInfo.Split(new char[] { ',' });
            foreach (var drawnOverValue in drawnOverValues)
                ChartDrawnOvers.Add(StringHelper.TrimString(drawnOverValue));
        }



        private void ParseChartColors(string info)
        {
            ChartColors = new List<Color>();

            if (info == null)
            {
                //ChartColors.Add(Color.SteelBlue);
                return; // Ignore info
            }

            var lineInfo = ParseHelper.ParseStringValueFromLineInfo(info);
            var colors = StringHelper.TrimArray(lineInfo.Split(new char[] { ',' }));
            foreach (var drawColor in colors)
                ChartColors.Add(Color.FromName(drawColor));
        }



        private void ParseComment(string info)
        {
            if (info == null)
                return; // Ignore info

            Comment = ParseHelper.ParseStringValueFromLineInfo(info);
        }
    }
}
