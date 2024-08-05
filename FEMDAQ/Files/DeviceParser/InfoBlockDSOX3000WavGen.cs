using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Own usings
using System.Drawing;
using FEMDAQ.StaticHelper;

using Ivi.Driver.Interop;
using Agilent.AgInfiniiVision.Interop;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Files.Parser
{
    public class InfoBlockDSOX3000WavGen : InfoBlock
    {
        public CommonParser Common { get; private set; }
        public UsbParser Usb { get; private set; }
        public SourceParser Source { get; private set; }

        public AgInfiniiVisionWaveformGeneratorFunctionEnum Waveform { get; private set; }
        // Common parameter
        public AgInfiniiVisionWaveformGeneratorOutputLoadImpedanceEnum OutputLoad { get; private set; }
        public bool InvertOutput { get; private set; }



        public InfoBlockDSOX3000WavGen(IEnumerable<string> infoBlock)
        {

            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            string lineInfo = null;

            Common = new CommonParser(infoBlock, null, null, null, "CustomName=", "Comment=");

            Usb = new UsbParser(infoBlock);

            Source = new SourceParser(infoBlock, "SourceNode=", null);

            ParseWaveform(StringHelper.FindStringWhichStartsWith(infoBlock, "Waveform="));

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "OutputLoad=");
            OutputLoad = (ParseHelper.ParseDoubleValueFromLineInfo(lineInfo) < 0 ? AgInfiniiVisionWaveformGeneratorOutputLoadImpedanceEnum.AgInfiniiVisionWaveformGeneratorOutputLoadImpedanceOneMeg : AgInfiniiVisionWaveformGeneratorOutputLoadImpedanceEnum.AgInfiniiVisionWaveformGeneratorOutputLoadImpedanceFifty);

            ParseInvertOutput(StringHelper.FindStringWhichStartsWith(infoBlock, "InvertOutput="));
        }

        public void Dispose()
        {
            Common.Dispose();
            Usb.Dispose();
            Source.Dispose();
        }




        private void ParseWaveform(string info)
        {
            if (info == null) throw new FormatException("Error on waveform-info");

            var strVal = ParseHelper.ParseStringValueFromLineInfo(info).ToUpper();

            if(strVal.StartsWith("SIN")) { Waveform = AgInfiniiVisionWaveformGeneratorFunctionEnum.AgInfiniiVisionWaveformGeneratorFunctionSinusoid; }
            else if (strVal.StartsWith("REC") || strVal.StartsWith("SQU"))            { Waveform = AgInfiniiVisionWaveformGeneratorFunctionEnum.AgInfiniiVisionWaveformGeneratorFunctionSquare; }
            else if (strVal.StartsWith("RAM") || strVal.StartsWith("SAW")){ Waveform = AgInfiniiVisionWaveformGeneratorFunctionEnum.AgInfiniiVisionWaveformGeneratorFunctionRamp; }
            else if (strVal.StartsWith("PUL")){ Waveform = AgInfiniiVisionWaveformGeneratorFunctionEnum.AgInfiniiVisionWaveformGeneratorFunctionPulse; }
            else { throw new FormatException("Unknown waveformtype - Line: " + info); }
        }



        private void ParseInvertOutput(string info)
        {
            if (info == null) throw new FormatException("Error on invertoutput-info");

            var strVal = StringHelper.TrimString(ParseHelper.ParseStringValueFromLineInfo(info)).ToUpper();
            switch(strVal)
            {
                case "0":
                case "FALSE":
                    InvertOutput = false;
                    break;
                case "1":
                case "TRUE":
                    InvertOutput = true;
                    break;
                default:
                    throw new FormatException("Wrong bool-type given - Line: " + info);
            }
        }
    }
}
