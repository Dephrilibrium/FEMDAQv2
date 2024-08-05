using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using Keithley;



namespace Files.Parser
{

    class InfoBlockKE6517B : InfoBlockInterface
    {
        public CommonParser Common { get; private set; }
        public GpibParser Gpib { get; private set; }
        public SourceParser Source { get; private set; }
        public GaugeParser Gauge { get; private set; }
        public KE6517B_MeasurementType MeasurementType { get; private set; }
        public bool ZeroCheck { get; private set; }
        public bool ZeroCorrection { get; private set; }



        public InfoBlockKE6517B(IEnumerable<string> infoBlock, string yRangeToken = "Range")
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            Common = new CommonParser(infoBlock);
            Gpib = new GpibParser(infoBlock);
            Source = new SourceParser(infoBlock);
            Gauge = new GaugeParser(infoBlock);

            string lineInfo = null;
            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "MeasureType=");
            MeasurementType = ParseMeasurementType(ParseHelper.ParseStringValueFromLineInfo(lineInfo));

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "ZeroCheck=");
            if (lineInfo == null)
                ZeroCheck = true;
            else
                ZeroCheck = bool.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "ZeroCorrection=");
            if (lineInfo == null)
                ZeroCorrection = true;
            else
                ZeroCorrection = bool.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));
        }

        private KE6517B_MeasurementType ParseMeasurementType(string lineValue)
        {
            lineValue = lineValue.ToUpper();
            if (lineValue.StartsWith("VOLT")) // Voltage
                return KE6517B_MeasurementType.Voltage;
            else if (lineValue.StartsWith("CHAR")) // Charge
                return KE6517B_MeasurementType.Charge;
            else if (lineValue.StartsWith("RESI")) // Resistance
                return KE6517B_MeasurementType.Resistance;
            else // Current (default)
                return KE6517B_MeasurementType.Current;
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
