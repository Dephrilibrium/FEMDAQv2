using FEMDAQ.StaticHelper;
using Keysight;
using System;
using System.Collections.Generic;
using System.Drawing;

// own usings
using Ivi.Driver.Interop;
using Agilent.AgInfiniiVision.Interop;

namespace Files.Parser
{
    public class InfoBlockDSOX3034T
    {
        public CommonParser Common { get; private set; }
        public UsbParser Usb { get; private set; }
        public GaugeParser Gauge { get; private set; }


        public DSOX3034TChannel Channel { get; private set;}
        public double XDivScale { get; private set; }
        public bool TriggerOnWaveform { get; private set; }
        public bool ForceTrigger { get; private set; }
        public double TriggerLevel { get; private set; }
        public DSOX3034TTriggerSource TriggerSource { get; private set; }
        public AgInfiniiVisionTriggerModifierEnum TriggerMode { get; private set; }
        public AgInfiniiVisionTriggerTypeEnum TriggerType { get; private set; }
        public AgInfiniiVisionTriggerSlopeEnum TriggerSlope { get; private set; }




        public InfoBlockDSOX3034T(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");
            string lineInfo = null;

            Common = new CommonParser(infoBlock);

            Usb = new UsbParser(infoBlock);

            Gauge = new GaugeParser(infoBlock, "MeasureInstantly=", "QuantPerDivY=", null);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "QuantPerDivX=");
            XDivScale = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "TriggerOnWaveform=");
            TriggerOnWaveform = ParseHelper.ParseBooLValueFromLineInfo(lineInfo, true);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "ForceTrigger=");
            ForceTrigger = ParseHelper.ParseBooLValueFromLineInfo(lineInfo, false);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "TriggerLevel=");
            TriggerLevel = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            ParseChannel(StringHelper.FindStringWhichStartsWith(infoBlock, "[Dev"));
            ParseTriggerSource(StringHelper.FindStringWhichStartsWith(infoBlock, "TriggerSource="));
            ParseTriggerMode(StringHelper.FindStringWhichStartsWith(infoBlock, "TriggerMode="));
            ParseTriggerType(StringHelper.FindStringWhichStartsWith(infoBlock, "TriggerType="));
        }



        private void ParseChannel(string info)
        {
            var splitString = info.Split(new string[] { "[", "|", "]" }, StringSplitOptions.RemoveEmptyEntries);

            switch(splitString[2].ToUpper())
            {
                case "CHANNEL1":
                    Channel = DSOX3034TChannel.Channel1;
                    break;

                case "CHANNEL2":
                    Channel = DSOX3034TChannel.Channel2;
                    break;

                case "CHANNEL3":
                    Channel = DSOX3034TChannel.Channel3;
                    break;

                case "CHANNEL4":
                    Channel = DSOX3034TChannel.Channel4;
                    break;

                default:
                    throw new FormatException("Uknown channelname: " + info);
            }
        }



        private void ParseTriggerSource(string info)
        {
            if (info == null) throw new FormatException("Triggersource-info is NULL");

            var stringVal = ParseHelper.ParseStringValueFromLineInfo(info).ToUpper();
            switch(stringVal)
            {
                case "CHANNEL1":
                    TriggerSource = DSOX3034TTriggerSource.Channel1;
                    break;

                case "CHANNEL2":
                    TriggerSource = DSOX3034TTriggerSource.Channel2;
                    break;

                case "CHANNEL3":
                    TriggerSource = DSOX3034TTriggerSource.Channel3;
                    break;

                case "CHANNEL4":
                    TriggerSource = DSOX3034TTriggerSource.Channel4;
                    break;

                case "EXTERNAL":
                    TriggerSource = DSOX3034TTriggerSource.External;
                    break;

                default:
                    throw new FormatException("Unknown triggersource: " + info);
            }
        }



        private void ParseTriggerMode(string info)
        {
            if (info == null) throw new FormatException("Triggermode-info is NULL");

            var stringVal = ParseHelper.ParseStringValueFromLineInfo(info).ToUpper();
            switch(stringVal)
            {
                case "NORMAL":
                    TriggerMode = AgInfiniiVisionTriggerModifierEnum.AgInfiniiVisionTriggerModifierNone;
                    break;

                case "AUTO":
                    TriggerMode = AgInfiniiVisionTriggerModifierEnum.AgInfiniiVisionTriggerModifierAuto;
                    break;

                default:
                    throw new FormatException("Unknown triggermode: " + info);
            }
        }



        private void ParseTriggerType(string info)
        {
            if (info == null) throw new FormatException("Triggertype-info is NULL");

            var stringVal = ParseHelper.ParseStringValueFromLineInfo(info).ToUpper();
            var splitString = StringHelper.TrimArray(stringVal.Split(new char[] { '|' }));

            if (splitString == null || splitString.Length < 2) throw new FormatException("Unknown triggertype: Current only Edge|<Slope> available - Line: " + info);

            switch(splitString[0].ToUpper())
            {
                case "EDGE":
                    TriggerType = AgInfiniiVisionTriggerTypeEnum.AgInfiniiVisionTriggerTypeEdge;
                    break;

                default:
                    throw new FormatException("Unknown triggertype - Line: " + info);
            }

            switch(splitString[1].ToUpper())
            {
                case "FALLING":
                    TriggerSlope = AgInfiniiVisionTriggerSlopeEnum.AgInfiniiVisionTriggerSlopeNegative;
                    break;
                case "RISING":
                    TriggerSlope = AgInfiniiVisionTriggerSlopeEnum.AgInfiniiVisionTriggerSlopePositive;
                    break;
                case "EITHER":
                    TriggerSlope = AgInfiniiVisionTriggerSlopeEnum.AgInfiniiVisionTriggerSlopeEither;
                    break;
                case "ALTERNATE":
                    TriggerSlope = AgInfiniiVisionTriggerSlopeEnum.AgInfiniiVisionTriggerSlopeAlternate;
                    break;
                default:
                    throw new FormatException("Unknown slope - Line: " + info);
            }
        }
    }
}
