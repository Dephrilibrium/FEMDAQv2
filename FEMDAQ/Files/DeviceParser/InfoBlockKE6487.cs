﻿using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;
using System.Drawing;



namespace Files.Parser
{
    public class InfoBlockKE6487 : InfoBlock
    {
        public CommonParser Common { get; private set; }
        public GpibParser Gpib { get; private set; }
        public SourceParser Source { get; private set; }
        public GaugeParser Gauge { get; private set; }
        public bool ZeroCheck { get; private set; }
        public bool AutoZero { get; private set; }


        public InfoBlockKE6487(IEnumerable<string> infoBlock, string yRangeToken = "Range")
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            Common = new CommonParser(infoBlock);
            Gpib = new GpibParser(infoBlock);
            Source = new SourceParser(infoBlock);
            Gauge = new GaugeParser(infoBlock);

            string lineInfo = null;
            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "ZeroCheck=");
            if (lineInfo == null)
                ZeroCheck = false;
            else
                ZeroCheck = bool.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "AutoZero=");
            if (lineInfo == null)
                AutoZero = false;
            else
                AutoZero = bool.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));
        }

        public void Dispose()
        {
            Common.Dispose();
            Gpib.Dispose();
            Source.Dispose();
            Gauge.Dispose();
        }
    }
}
