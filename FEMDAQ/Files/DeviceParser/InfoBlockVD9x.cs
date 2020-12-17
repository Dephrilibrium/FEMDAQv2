using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;
using System.Drawing;



namespace Files.Parser
{
    public class InfoBlockVD9x
    {
        public CommonParser Common { get; private set; }
        public ComParser Com { get; private set; }
        public GaugeParser Gauge { get; private set; }

        public int Baudrate { get; private set; }



        public InfoBlockVD9x(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            string lineInfo = null;

            Common = new CommonParser(infoBlock);
            Com = new ComParser(infoBlock);
            Gauge = new GaugeParser(infoBlock, null, null, null); // Has no range!

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Baudrate");
            Baudrate = (int)ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);
        }
    }
}
