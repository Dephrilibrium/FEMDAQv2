using FEMDAQ.StaticHelper;
using Instrument.LogicalLayer;
using System;
using System.Collections.Generic;
using System.Threading;

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
        public int nSubMeasurements { get; private set; }
        public int deltatimeSubMeasurements { get; private set; }
        public double Range { get; private set; }
        public double Nplc { get; private set; }


        public GaugeParser(IEnumerable<string> infoBlock,
                        string measureInstantlyToken = "MeasureInstantly=",
                        string nSubMeasurementsToken = "nSubMeasurements",
                        string deltatimeSubMeasurementsToken = "deltatimeSubmeasurements",
                        string yRangeToken = "Range=", string nplcToken = "Nplc="
            )
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            ParseMeasureInstantly(StringHelper.FindStringWhichStartsWith(infoBlock, measureInstantlyToken));
            Parse_nSubMeasurements(StringHelper.FindStringWhichStartsWith(infoBlock, nSubMeasurementsToken));
            Parse_deltatimeSubMeasurements(StringHelper.FindStringWhichStartsWith(infoBlock, deltatimeSubMeasurementsToken));
            if (nSubMeasurements <= 1)          // When only one datapoint is requested
                deltatimeSubMeasurements = 0;   //  turn off delay (submeastimer-class does not use an internal timer then!)

            ParseRange(StringHelper.FindStringWhichStartsWith(infoBlock, yRangeToken));
            ParseNplc(StringHelper.FindStringWhichStartsWith(infoBlock, nplcToken));
        }



        public void Dispose()
        {
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



        private void Parse_nSubMeasurements(string info)
        {
            if (info == null)
            {
                nSubMeasurements = 1; // Default = 1 datapoint
                return; // Ignore the rest
            }
            nSubMeasurements= (int)ParseHelper.ParseDoubleValueFromLineInfo(info);
            if (nSubMeasurements <= 0)
                nSubMeasurements = 1;
        }



        private void Parse_deltatimeSubMeasurements(string info)
        {
            if (info == null)
            {
                deltatimeSubMeasurements = 0; // Default
                return; // Ignore the rest

            }
            deltatimeSubMeasurements = (int)ParseHelper.ParseDoubleValueFromLineInfo(info);
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
