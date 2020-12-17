using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Own usings
using System.Drawing;
using FEMDAQ.StaticHelper;

namespace Files.Parser
{
    public class InfoBlockWavGen33511B
    {
        public CommonParser Common { get; private set; }
        public GpibParser Gpib { get; private set; }
        public SourceParser Source { get; private set; }

        public string Waveform { get; private set; }
        // Common parameter
        public int OutputLoad { get; private set; }
        public double Offset { get; private set; }
        public double Phase { get; private set; }

        // Ramp-specific parameter
        public double PulseLeadEdge { get; private set; }
        public double PulseTrailEdge { get; private set; }
        // Burst
        public int UseBurst { get; private set; }
        public int BurstCycles { get; private set; }
        public double BurstPeriod { get; private set; }



        public InfoBlockWavGen33511B(IEnumerable<string> infoBlock)
        {

            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            string lineInfo = null;

            Common = new CommonParser(infoBlock, null, null, null, "CustomName=", "Comment=");

            Gpib = new GpibParser(infoBlock);

            Source = new SourceParser(infoBlock, "SourceNode=", null);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Waveform=");
            if (lineInfo == null)
                Waveform = "SIN";
            else
                Waveform = ParseHelper.ParseStringValueFromLineInfo(lineInfo)
                                  .ToUpper();

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "OutputLoad=");
            if (lineInfo == null)
                OutputLoad = 50;
            else
                OutputLoad = (int)ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Offset=");
            if (lineInfo == null)
                Offset = 0;
            else
                Offset = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Phase=");
            if (lineInfo == null)
                Phase = 0;
            else
                Phase = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);


            // Parse pulse specific: Rise- and falling times will be static. Puls width can vary (-> swp-File)
            if (Waveform == "PULS")
            {
                lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "LeadEdge=");
                PulseLeadEdge = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

                lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "TrailEdge=");
                PulseTrailEdge = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);
            }

            // Parse burst specific
            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "UseBurst=");
            if (lineInfo == null)
                UseBurst = 0;
            else
                UseBurst = (int)ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            if (UseBurst != 0)
            {
                lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "BurstCycles=");
                if (lineInfo == null)
                    BurstCycles = 1;
                else
                    BurstCycles = (int)ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

                lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "BurstPeriod=");
                if (lineInfo == null)
                    BurstPeriod = (int)ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);
            }
        }
    }
}
