using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;
using System.Drawing;



namespace Files.Parser
{
    public class InfoBlockDmySMU
    {
        public CommonParser Common { get; private set; }
        public SourceParser Source { get; private set; }
        public GaugeParser Gauge { get; private set; }
        public double LowerBound { get; private set; }
        public double UpperBound { get; private set; }


        public InfoBlockDmySMU(IEnumerable<string> infoBlock, string yRangeToken = "Range")
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            Common = new CommonParser(infoBlock);
            Source = new SourceParser(infoBlock, "SourceNode=", null);
            Gauge = new GaugeParser(infoBlock, "MeasureInstantly=", null, null);

            string lineInfo = null;
            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "LowerBound=");
            LowerBound = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);
            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "UpperBound=");
            UpperBound = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);
        }
    }
}
