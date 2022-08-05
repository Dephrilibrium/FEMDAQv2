using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;
using HaumOTH;
using System.IO;
using System.Linq;

namespace Files.Parser
{
    public class InfoBlockPiCam
    {
        public CommonParser Common { get; private set; }
        public IpParser Ip { get; private set; }
        public GaugeParser Gauge { get; private set; }
        public string PyCamScriptPath { get; private set; }
        public string TempDownloadDir{ get; private set; }
        public uint ISO { get; private set; }
        //public (uint width, uint height) Resolution { get; private set; }
        public uint ResolutionWidth { get; private set; }
        public uint ResolutionHeight { get; private set; }
        public PiCamExposureMode ExposureMode { get; private set; }
        public uint[] ShutterSpeeds { get; private set; }
        public double PictureInterval { get; private set; }

        public PiCamAwbMode AwbMode { get; private set; }
        //public (double rBalance, double bBalance) Awb { get; private set; }
        public double AwbGainRBalance { get; private set; }
        public double AwbGainBBalance { get; private set; }

        public InfoBlockPiCam(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            string lineInfo = null;

            Common = new CommonParser(infoBlock);

            Ip= new IpParser(infoBlock);

            Gauge = new GaugeParser(infoBlock);

            PyCamScriptPath = StringHelper.TrimString(ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "PyCamPath=")));
            if (PyCamScriptPath == null || PyCamScriptPath == "")
                throw new Exception("No PyCamScriptPath given.");
            TempDownloadDir = StringHelper.TrimString(ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "TempDownloadDir=")));
            if (TempDownloadDir == null || TempDownloadDir == "")
                TempDownloadDir = Path.Combine(Directory.GetCurrentDirectory(), "_PiCamTemp");

            lineInfo = ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "ISO="));
            ISO = uint.Parse(lineInfo);

            ParseResolution(infoBlock);

            ParseExposuremode(infoBlock);
            ParseShutterspeed(infoBlock);
            ParsePictureInterval(infoBlock);

            ParseAutoWhiteBalance(infoBlock);
        }


        private void ParseResolution(IEnumerable<string> infoBlock)
        {
            var lineInfo = ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "Resolution="));
            var lineSplit = lineInfo.Split(new char[] { ',' });
            lineSplit = StringHelper.TrimArray(lineSplit);
            if (lineSplit.Length <= 0 || lineSplit.Length > 2)
                throw new Exception("Non-valid resolution on: Resolution=" + lineInfo);

            ResolutionWidth = uint.Parse(lineSplit[0]);
            ResolutionHeight = uint.Parse(lineSplit[1]);
            //Resolution = (uint.Parse(lineSplit[0]), uint.Parse(lineSplit[1]));
            ExposureMode = PiCamExposureMode.off; // Currently always off, maybe added in future
        }


        private void ParseExposuremode(IEnumerable<string> infoBlock)
        {
            var lineInfo = ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "ExposureMode="));
            ExposureMode = (PiCamExposureMode)Enum.Parse(typeof(PiCamExposureMode), lineInfo);
        }


        private void ParseShutterspeed(IEnumerable<string> infoBlock)
        {
            var lineInfo = ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "ShutterSpeed="));
            var lineSplit = lineInfo.Split(new char[] { ',' });
            lineSplit = StringHelper.TrimArray(lineSplit);
            if (lineSplit.Length <= 0)
                throw new Exception("No ShutterSpeed given: ShutterSpeed=" + lineInfo);

            ShutterSpeeds = new uint[lineSplit.Length];
            for (int iSS = 0; iSS < lineSplit.Length; iSS++)
                ShutterSpeeds[iSS] = uint.Parse(lineSplit[iSS]);

            // Sort from lowest to highest!
            //  -> Cam an switch from short SS to longer SS fastly, but not inverse!
            //  -> The shortest SS is set as initial SS in PiCam-Overlay
            //  -> During measurement after all SS-Pictures the shortest SS is reset to not waste time for the next cycle
            Array.Sort(ShutterSpeeds);
        }


        private void ParsePictureInterval(IEnumerable<string> infoBlock)
        {
            PictureInterval = ParseHelper.ParseDoubleValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "PictureInterval="));
        }


        private void ParseAutoWhiteBalance(IEnumerable<string> infoBlock)
        {
            var lineInfo = ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "Awb="));
            var lineSplit = lineInfo.Split(new char[] { ',' });
            lineSplit = StringHelper.TrimArray(StringHelper.TrimArray(lineSplit));
            if (lineSplit.Length <= 0)
                throw new Exception("No ShutterSpeed given: Awb=" + lineInfo);

            switch (lineSplit.Length)
            {
                case 1:
                    AwbMode = (PiCamAwbMode)Enum.Parse(typeof(PiCamAwbMode), lineInfo);
                    // 1.2 for both if anothermode than off
                    AwbGainRBalance = 1.2;
                    AwbGainBBalance = 1.2;
                    //Awb = (1.2, 1.2);
                    break;

                case 2:
                    AwbMode = PiCamAwbMode.off;
                    AwbGainRBalance = double.Parse(lineSplit[0]);
                    AwbGainBBalance = double.Parse(lineSplit[1]);
                    //Awb = (double.Parse(lineSplit[0]), double.Parse(lineSplit[1])); // If a mode other than 
                    break;

                default:
                    throw new Exception("Invalid Awb-Argument: Awb=" + lineInfo);
            }
        }
    }
}
