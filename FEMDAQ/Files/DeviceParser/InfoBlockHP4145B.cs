using Instrument.HP4145B;
using System;
using System.Collections.Generic;
using FEMDAQ.StaticHelper;

namespace Files.Parser
{
    public class InfoBlockHP4145B : InfoBlock
    {
        public CommonParser Common { get; private set; }
        public GpibParser Gpib { get; private set; }
        public SourceParser Source { get; private set; }
        public GaugeParser Gauge { get; private set; }

        // Devicespecific
        public Channel SMUChannel { get; private set; }
        public Mode MeasureMode { get; private set; }
        public IntegrationTime IntegrationTime { get; private set; }
        public double Compliance { get; private set; }
        public Mode SourceMode { get; private set; }


        public InfoBlockHP4145B(IEnumerable<string> infoBlock, string yRangeToken = "Range")
        {
            if (infoBlock == null) throw new NullReferenceException("infoBlock");

            Common = new CommonParser(infoBlock);
            Gpib = new GpibParser(infoBlock);
            Source = new SourceParser(infoBlock, "SourceNode=", "Dummy=");
            Gauge = new GaugeParser(infoBlock);

            string lineInfo = null;
            ParseSMUChannel(infoBlock);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "MeasureMode=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
            MeasureMode = lineInfo.StartsWith("VOLT") ? Mode.Voltage : Mode.Current;

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "IntegrationTime=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
            IntegrationTime = (lineInfo == "LONG" ? IntegrationTime.Long :
                               lineInfo == "SHORT" ? IntegrationTime.Short : IntegrationTime.Medium);

            ParseCompliance(infoBlock);
        }


        public void Dispose()
        {
            Common.Dispose();
            Gpib.Dispose();
            Source.Dispose();
            Gauge.Dispose();
        }



        private void ParseSMUChannel(IEnumerable<string> infoBlock)
        {
            var blockIdentifier = StringHelper.FindStringWhichStartsWith(infoBlock, "[Dev");
            var splitBlockIdentifier = blockIdentifier.Split(new char[] { '[', '|', ']' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitBlockIdentifier.Length < 3) throw new MissingMemberException("infoBlock - Missing Channelidentifier");
            if (splitBlockIdentifier[2] == null) throw new NullReferenceException("infoBlock - Channelidentifier is NULL");

            var channelIdentifier = splitBlockIdentifier[2].ToUpper();
            SMUChannel = (channelIdentifier == "SMU4" ? Channel.SMU4 :
                          channelIdentifier == "SMU3" ? Channel.SMU3 :
                          channelIdentifier == "SMU2" ? Channel.SMU2 : Channel.SMU1);
        }



        private void ParseCompliance(IEnumerable<string> infoBlock)
        {
            var lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Compliance=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo);
            var splitLineInfo = lineInfo.Split(new char[] { '|' });

            if (splitLineInfo.Length < 2) throw new FormatException("infoBlock - Compliance=");
            if (splitLineInfo[1] == null) throw new NullReferenceException("infoBlock - ComplianceType is NULL");

            Compliance = double.Parse(splitLineInfo[0]);
            splitLineInfo[1] = splitLineInfo[1].ToUpper();
            SourceMode = (splitLineInfo[1] == "V" ? Mode.Voltage : Mode.Current);
        }
    }
}
