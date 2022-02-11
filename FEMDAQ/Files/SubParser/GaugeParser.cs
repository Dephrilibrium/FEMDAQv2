using FEMDAQ.StaticHelper;
using Instrument.LogicalLayer;
using System;
using System.Collections.Generic;

namespace Files.Parser
{
    public enum GaugeMeasureInstantly
    {
        Disabled = -1,
        CycleStart = 1,
        CycleEnd = 0,
    };


    public class GaugeParser
    {
        public GaugeMeasureInstantly MeasureInstantly { get; private set; }
        public double Range { get; private set; }
        public double Nplc { get; private set; }


        public GaugeParser(IEnumerable<string> infoBlock, string measureInstantlyToken = "MeasureInstantly=", string yRangeToken = "Range=", string nplcToken = "Nplc=")
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            ParseMeasureInstantly(StringHelper.FindStringWhichStartsWith(infoBlock, measureInstantlyToken));
            ParseRange(StringHelper.FindStringWhichStartsWith(infoBlock, yRangeToken));
            ParseNplc(StringHelper.FindStringWhichStartsWith(infoBlock, nplcToken));
        }



        private void ParseMeasureInstantly(string info)
        {
            if (info == null)
            {
                MeasureInstantly = 0; // Default
                return; // Ignore range
            }
            MeasureInstantly = (GaugeMeasureInstantly)ParseHelper.ParseDoubleValueFromLineInfo(info);
        }



        private void ParseRange(string info)
        {
            if (info == null)
            {
                Range = 0; // Default
                return; // Ignore range, if it is not given
            }
            Range = ParseHelper.ParseDoubleValueFromLineInfo(info);
        }



        private void ParseNplc(string info)
        {
            if (info == null)
            {
                Nplc = 1; // Default
                return; // Ignore this
            }

            Nplc = ParseHelper.ParseDoubleValueFromLineInfo(info);
        }
    }
}
