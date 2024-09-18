using Instrument.HP4145B;
using System;
using System.Collections.Generic;
using FEMDAQ.StaticHelper;
using System.Drawing;
using Instrument.LogicalLayer;
using System.IO;
using RohdeSchwarz.RsScope;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Files.Parser
{
    public class FEAR16DACChannel
    {
        //public bool IsActive = false;
        public int SourceNode = -1;
    }

    public class FEAR16ADCChannel
    {
        //public bool IsActive = false;
        public GaugeMeasureInstantly MeasureInstantly = 0;
        public CommonParser chartInfo;
    }

    public class FEAR16ADCGeneralSettings
    {
        public uint AdcNMean = 10;
        public uint AdcMDelta = 10;         // [ms]
        public int RequestDelay_ms = 100; // default: AdcNMean * AdcMDelta = RequestDelay_ms
        public int nSubMeasurements = 1;
        public int deltatimeSubMeasurements = 0; // [ms]
    }


    public class InfoBlockFEAR16v2 : InfoBlock
    {
        private int _amountOfChannels { get; set; }
        public CommonParser Common { get; private set; }
        //public List<CommonParser> ChChartCommons { get; private set; }
        public ComParser ComPort { get; private set; }
        public FEAR16ADCGeneralSettings AdcGeneralSettings { get; private set; }
        public List<FEAR16DACChannel> CurrCtrlChannels { get; private set; }
        public List<FEAR16ADCChannel> ShuntCurrChannels { get; private set; }
        public List<FEAR16ADCChannel> DropVoltChannels { get; private set; }

        // Devicespecific
        //public Channel SMUChannel { get; private set; }
        //public Mode MeasureMode { get; private set; }
        //public IntegrationTime IntegrationTime { get; private set; }
        //public double Compliance { get; private set; }
        //public Mode SourceMode { get; private set; }


        public InfoBlockFEAR16v2(IEnumerable<string> infoBlock, string yRangeToken = "Range")
        {
            if (infoBlock == null) throw new NullReferenceException("infoBlock");

            Common = new CommonParser(infoBlock, null, null, null, null);
            ComPort = new ComParser(infoBlock);

            _amountOfChannels = 16;
            //ChChartCommons = new List<CommonParser>(_amountOfChannels);
            // Make Ctrl and Gauge lists and fill with instances
            CurrCtrlChannels = new List<FEAR16DACChannel>(_amountOfChannels);
            ShuntCurrChannels = new List<FEAR16ADCChannel>(_amountOfChannels);
            DropVoltChannels = new List<FEAR16ADCChannel>(_amountOfChannels);


            int detectedChEntries = 0;
            for (int iCh = 0; iCh < _amountOfChannels; iCh++)
            {
                CurrCtrlChannels.Add(new FEAR16DACChannel());
                ShuntCurrChannels.Add(new FEAR16ADCChannel());
                DropVoltChannels.Add(new FEAR16ADCChannel());
                ParseChannelNum(infoBlock, iCh);

                if (CurrCtrlChannels[iCh].SourceNode != -1 || ShuntCurrChannels[iCh].MeasureInstantly != GaugeMeasureInstantly.Disabled || DropVoltChannels[iCh].MeasureInstantly != GaugeMeasureInstantly.Disabled)
                    detectedChEntries++;
            }

            if (detectedChEntries <= 0)
                throw new ArgumentException("InfoBlockParser-FEAR16v2: No valid channel-entries found! (" + ComPort.ComPort + ")\n\n" +
                                            "!!! Note !!! \nINI-Keys changed recently:\n" + 
                                            "Ch<x>CC -> Ch<x>Ctrl\n" + 
                                            "Ch<x>Shnt -> Ch<x>Shnt\n" +
                                            "Ch<x>Drop -> Ch<x>Drop");


            AdcGeneralSettings = new FEAR16ADCGeneralSettings();
            parseADCGeneralSettings(infoBlock);

            parseRequestDelay(infoBlock);

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

        public void Dispose()
        {
            Common.Dispose();
            ComPort.Dispose();
            for (int iChADC = 0; iChADC < _amountOfChannels; iChADC++)
            {
                if (ShuntCurrChannels[iChADC].chartInfo != null) { ShuntCurrChannels[iChADC].chartInfo.Dispose(); }
                if (DropVoltChannels[iChADC].chartInfo != null) { DropVoltChannels[iChADC].chartInfo.Dispose(); }
            }
        }

        private void parseRequestDelay(IEnumerable<string> infoBlock)
        {
            var lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "RequestDelay=");
            if (lineInfo == null)
            {
                AdcGeneralSettings.RequestDelay_ms = 100; // Default = 100ms
                return;
            }
            
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
            AdcGeneralSettings.RequestDelay_ms = int.Parse(lineInfo);
        }

        private void ParseChannelNum(IEnumerable<string> infoBlock, int iCh)
        {
            //string lineInfo;
            //string lineValue;
            string chBase = string.Format("Ch{0}", iCh);

            parseControlChannel(infoBlock, chBase+"Ctrl", CurrCtrlChannels[iCh]);

            parseMeasurementChannel(infoBlock, chBase + "Shnt", ShuntCurrChannels[iCh]);
            parseMeasurementChannel(infoBlock, chBase + "Drop", DropVoltChannels[iCh]);
        }

 

        private void parseControlChannel(IEnumerable<string> infoBlock, string chPrefix, FEAR16DACChannel dacChannel)
        {
            string lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, chPrefix + "=");
            if (lineInfo != null)
            {
                //dacChannel.IsActive = true;
                dacChannel.SourceNode = int.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));
            }
            else
                //dacChannel.IsActive = false;
                dacChannel.SourceNode = -1;
        }


        private void parseMeasurementChannel(IEnumerable<string> infoBlock, string chPrefix, FEAR16ADCChannel adcChannel)
        {
            string lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, chPrefix + "=");
            if (lineInfo != null)
            {
                //adcChannel.IsActive = true;
                adcChannel.MeasureInstantly = (GaugeMeasureInstantly)int.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));
                if (adcChannel.MeasureInstantly < 0)
                    adcChannel.MeasureInstantly = GaugeMeasureInstantly.Disabled;
                else
                {
                    adcChannel.chartInfo = new CommonParser(infoBlock,
                        chPrefix + "ChartIdentifier=",
                        chPrefix + "ChartDrawnOver=",
                        chPrefix + "ChartColor",
                        chPrefix + "CustomName",
                        null);
                }

            }
            else
            {
                //adcChannel.IsActive = false;
                adcChannel.MeasureInstantly = GaugeMeasureInstantly.Disabled;
                //adcChannel.chartInfo = new CommonParser(infoBlock, null, null, null, null, null); // Create instance no content
            }
        }


        private void parseADCGeneralSettings(IEnumerable<string> infoBlock)
        {
            string lineInfo = null;

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "AdcNMean=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
            AdcGeneralSettings.AdcNMean = uint.Parse(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "AdcMDelta=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
            AdcGeneralSettings.AdcMDelta = uint.Parse(lineInfo);


            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "nSubMeasurements=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
            AdcGeneralSettings.nSubMeasurements = int.Parse(lineInfo);
            if (AdcGeneralSettings.nSubMeasurements <= 1) // value <= 1 --> Only one datapoint!
                AdcGeneralSettings.nSubMeasurements = 1;

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "deltatimeSubmeasurements=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
            AdcGeneralSettings.deltatimeSubMeasurements = int.Parse(lineInfo);
            if (AdcGeneralSettings.nSubMeasurements <= 1)        // When only one datapoint is requested
                AdcGeneralSettings.deltatimeSubMeasurements = 0; //  turn off delay (submeastimer-class does not use an internal timer then!)

        }
    }
}
