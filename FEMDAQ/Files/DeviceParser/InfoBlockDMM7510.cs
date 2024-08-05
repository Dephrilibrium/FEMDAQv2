using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;


namespace Files.Parser
{
    public class InfoBlockDMM7510 : InfoBlockInterface
    {
        public CommonParser Common { get; private set; }
        public GpibParser Gpib { get; private set; }
        public GaugeParser Gauge { get; private set; }
        public string MeasurementType { get; private set; }
        public string Coupling { get; private set; }
        public bool AutoZero { get; private set; }

        public InfoBlockDMM7510(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            string lineInfo = null;

            Common = new CommonParser(infoBlock);

            Gpib = new GpibParser(infoBlock);

            Gauge = new GaugeParser(infoBlock);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "MeasurementType=");
            MeasurementType = ParseHelper.ParseStringValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Coupling=");
            if (lineInfo == null)
                Coupling = "DC";
            else
                Coupling = ParseHelper.ParseStringValueFromLineInfo(lineInfo);

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
            Gauge.Dispose();
        }
    }
}
