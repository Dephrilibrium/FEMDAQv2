using Instrument.HP4145B;
using System;
using System.Collections.Generic;
using FEMDAQ.StaticHelper;
using System.Drawing;

namespace Files.Parser
{
    public class FEAR16DACChannel
    {
        public bool IsActive = false;
        public int SourceNode = -1;
    }

    public class FEAR16ADCChannel
    {
        public bool IsActive = false;
        public GaugeMeasureInstantly MeasureInstantly = 0;
        public CommonParser chartInfo;
    }


    public class InfoBlockFEAR16v2
    {
        private int _amountOfChannels { get; set; }
        public CommonParser Common { get; private set; }
        public List<CommonParser> ChChartCommons { get; private set; }
        public ComParser ComPort { get; private set; }
        public List<FEAR16DACChannel> CurrCtrlChannels { get; private set; }
        public List<FEAR16ADCChannel> CurrFlowChannels { get; private set; }
        public List<FEAR16ADCChannel> UDropFETChannels { get; private set; }

        // Devicespecific
        //public Channel SMUChannel { get; private set; }
        //public Mode MeasureMode { get; private set; }
        //public IntegrationTime IntegrationTime { get; private set; }
        //public double Compliance { get; private set; }
        //public Mode SourceMode { get; private set; }


        public InfoBlockFEAR16v2(IEnumerable<string> infoBlock, string yRangeToken = "Range")
        {
            if (infoBlock == null) throw new NullReferenceException("infoBlock");

            Common = new CommonParser(infoBlock, null, null, null);
            ComPort = new ComParser(infoBlock);

            _amountOfChannels = 16;
            ChChartCommons = new List<CommonParser>(_amountOfChannels);
            // Make Ctrl and Gauge lists and fill with instances
            CurrCtrlChannels = new List<FEAR16DACChannel>(_amountOfChannels);
            CurrFlowChannels = new List<FEAR16ADCChannel>(_amountOfChannels);
            UDropFETChannels = new List<FEAR16ADCChannel>(_amountOfChannels);

            for (int iCh = 0; iCh < _amountOfChannels; iCh++)
            {
                CurrCtrlChannels.Add(new FEAR16DACChannel());
                CurrFlowChannels.Add(new FEAR16ADCChannel());
                UDropFETChannels.Add(new FEAR16ADCChannel());
                ParseChannelNum(infoBlock, iCh);
            }


            //Source = new SourceParser(infoBlock, "SourceNode=", "Dummy=");
            //Gauge = new GaugeParser(infoBlock);

            //string lineInfo = null;
            //ParseSMUChannel(infoBlock);

            //lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "MeasureMode=");
            //lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
            //MeasureMode = lineInfo.StartsWith("VOLT") ? Mode.Voltage : Mode.Current;

            //lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "IntegrationTime=");
            //lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
            //IntegrationTime = (lineInfo == "LONG" ? IntegrationTime.Long :
            //                   lineInfo == "SHORT" ? IntegrationTime.Short : IntegrationTime.Medium);

            //ParseCompliance(infoBlock);
        }


        private void ParseChannelNum(IEnumerable<string> infoBlock, int iCh)
        {
            string lineInfo;
            string lineValue;
            string chBase = string.Format("Ch{0}", iCh);

            parseControlChannel(infoBlock, chBase+"CC", CurrCtrlChannels[iCh]);

            parseMeasurementChannel(infoBlock, chBase + "CF", CurrFlowChannels[iCh]);
            parseMeasurementChannel(infoBlock, chBase + "UDrp", CurrFlowChannels[iCh]);
        }

 

        private void parseControlChannel(IEnumerable<string> infoBlock, string chPrefix, FEAR16DACChannel dacChannel)
        {
            string lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, chPrefix + "=");
            if (lineInfo != null)
            {
                dacChannel.IsActive = true;
                dacChannel.SourceNode = int.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));
            }
            else
                dacChannel.IsActive = false;
        }


        private void parseMeasurementChannel(IEnumerable<string> infoBlock, string chPrefix, FEAR16ADCChannel adcChannel)
        {
            string lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, chPrefix+"=");
            if (lineInfo != null)
            {
                adcChannel.IsActive = true;
                adcChannel.MeasureInstantly = (GaugeMeasureInstantly)int.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));
                adcChannel.chartInfo = new CommonParser(infoBlock,
                    chPrefix + "ChartIdentifier=",
                    chPrefix + "ChartDrawnOver=",
                    chPrefix + "ChartColor", null, null);
            }
            else
                adcChannel.IsActive = false;
        }
    }
}
