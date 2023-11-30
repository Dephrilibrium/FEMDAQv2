using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;
using HaumOTH;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Renci.SshNet.Compression;

namespace Files.Parser
{
    public class InfoBlockPiCam2
    {
        public CommonParser Common { get; private set; }
        public IpParser Ip { get; private set; }
        public GaugeParser Gauge { get; private set; }
        public string PyCamScriptPath { get; private set; }
        public string PyCamLogPath { get; private set; }
        public string TempDownloadDir { get; private set; }


        public double AnalogGain { get; private set; }
        public double FrameRate { get; private set; }
        public int ShutterSpeedsColumn {  get; private set; }
        public uint[] ShutterSpeeds { get; private set; }
        public uint PicsPerShutterSpeed { get; private set; }

        //public (double rBalance, double bBalance) Awb { get; private set; }
        public double AwbGainRBalance { get; private set; }
        public double AwbGainBBalance { get; private set; }
        public uint[] ScalerCrop { get; private set; }


        public uint[] BayerClipWin { get; private set; }
        public bool DeBayerClipWindow { get; private set; }
        public uint ShrinkDebayeredByBinPow { get; private set; }
        public uint SaveSSLog { get; private set; }
        public uint Compress2TarGz { get; private set; }
        public uint CompressMulticore { get; private set; }
        public uint CompressSuppressParents { get; private set; }



        public InfoBlockPiCam2(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            string lineInfo = null;

            Common = new CommonParser(infoBlock);

            Ip = new IpParser(infoBlock);

            Gauge = new GaugeParser(infoBlock);

            PyCamScriptPath = StringHelper.TrimString(ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "PyCam2Path=")));
            if (PyCamScriptPath == null || PyCamScriptPath == "")
                throw new Exception("No PyCamScriptPath given.");

            TempDownloadDir = StringHelper.TrimString(ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "TempDownloadDir=")));
            if (TempDownloadDir == null || TempDownloadDir == "")
                TempDownloadDir = Path.Combine(Directory.GetCurrentDirectory(), "_PiCam2Temp");

            ParseShutterspeed(infoBlock);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "PicsPerSS=");
            if (lineInfo == null || lineInfo == "")
                PicsPerShutterSpeed = 1;
            else
            {
                int parsedVal = int.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));
                if (parsedVal < 1)
                    parsedVal = 1;
                PicsPerShutterSpeed = (uint)parsedVal;
            }

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "FrameRate=");
            if (lineInfo == null)
                FrameRate = 10;
            else
                FrameRate = double.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "AnalogGain=");
            if (lineInfo == null)
                AnalogGain = -1;
            else
                AnalogGain = double.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));

            ParseAutoWhiteBalance(infoBlock);

            //ParseScalerCrop(infoBlock);

            ParseBayerClipWindow(infoBlock);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "DeBayerClippedBayer=");
            if (lineInfo == null)
                throw new Exception("No DeBayerClip-Flag given: DeBayerClippedBayer=");
            else
                DeBayerClipWindow = (uint.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo)) == 0 ? false : true);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "ShrinkDebayeredByBinPow=");
            if (lineInfo == null)
                throw new Exception("No Shrink-Iterator given: ShrinkDebayeredByBinPow=");
            else
                ShrinkDebayeredByBinPow = uint.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "SaveSSLog=");
            if (lineInfo == null)
                SaveSSLog = 1;
            else
                SaveSSLog = uint.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Compress2TarGz=");
            if (lineInfo == null)
                Compress2TarGz = 1;
            else
                Compress2TarGz = uint.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "CompressMulticore=");
            if (lineInfo == null)
                CompressMulticore = 1;
            else
                CompressMulticore = uint.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "CompressSuppressParents=");
            if (lineInfo == null)
                CompressSuppressParents = 1;
            else
                CompressSuppressParents = uint.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));
        }



        private void ParseShutterspeed(IEnumerable<string> infoBlock)
        {
            var str4SSColumn = "swpCol".ToLower();

            var lineInfo = ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "ShutterSpeed="));
            var lineSplit = lineInfo.Split(new char[] { ',', '|' });
            lineSplit = StringHelper.TrimArray(lineSplit);
            if (lineSplit.Length <= 0)  // Check if a shutterspeed parameterlist is given
                throw new Exception("No ShutterSpeed given: ShutterSpeed=" + lineInfo);
            if (lineSplit.Length <= 1 && lineSplit[0].ToLower() == str4SSColumn) // Check if a shutterspeed-column is given
                throw new Exception("No ShutterSpeedColumn given (swpCol|x; x missing): ShutterSpeed=" + lineInfo);

            if (lineSplit[0].ToLower() == str4SSColumn)
            {
                ShutterSpeedsColumn = int.Parse(lineSplit[1]);
                if (int.Parse(lineSplit[1]) < 0) // Check if a valid shutterspeed-column is given
                    throw new Exception("Invalid ShutterSpeedColumn given (swpCol|x; x < 0 not allowed): ShutterSpeed=" + lineInfo);

                ShutterSpeeds = null; // Columns is the source of Shutterspeeds -> No shutterspeedlist
            }
            else
            {
                ShutterSpeedsColumn = -1; // No Column -> Use invalid index
                ShutterSpeeds = new uint[lineSplit.Length];
                for (int iSS = 0; iSS < lineSplit.Length; iSS++)
                    ShutterSpeeds[iSS] = uint.Parse(lineSplit[iSS]);

                // Sort from lowest to highest!
                //  -> Cam an switch from short SS to longer SS fastly, but not inverse!
                //  -> The shortest SS is set as initial SS in PiCam-Overlay
                //  -> During measurement after all SS-Pictures the shortest SS is reset to not waste time for the next cycle
                Array.Sort(ShutterSpeeds);
            }
        }


        public void OverrideShutterSpeeds(uint[] shutterspeeds)
        {
            ShutterSpeeds = shutterspeeds;
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
                    AwbGainRBalance = double.Parse(lineSplit[0]);
                    AwbGainBBalance = double.Parse(lineSplit[0]);
                    break;

                case 2:
                    AwbGainRBalance = double.Parse(lineSplit[0]);
                    AwbGainBBalance = double.Parse(lineSplit[1]);
                    break;

                default:
                    throw new Exception("Invalid Awb-Argument: Awb=" + lineInfo);
            }
        }


        // !!! ScalerCrop is NOT WORKING for the RAW-Stream we are using !!!
        //private void ParseScalerCrop(IEnumerable<string> infoBlock)
        //{
        //    var lineInfo = ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "ScalerCrop="));
        //    var lineSplit = lineInfo.Split(new char[] { ',' });
        //    lineSplit = StringHelper.TrimArray(StringHelper.TrimArray(lineSplit));

        //    if (lineSplit == null)
        //        throw new Exception("No ScalerCrop value given: ScalerCrop=");

        //    if (lineSplit.Length != 4)
        //        throw new Exception("Unsupported amount of values given: ScalerCrop=" + lineInfo);

        //    ScalerCrop = new uint[lineSplit.Length]; // Always 4 values
        //    for(int i = 0; i < lineSplit.Length; i++)
        //        ScalerCrop[i] = uint.Parse(lineSplit[i]);
        //}


        private void ParseBayerClipWindow(IEnumerable<string> infoBlock)
        {
            var lineInfo = ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "BayerClipWindow="));
            var lineSplit = lineInfo.Split(new char[] { ',' });
            lineSplit = StringHelper.TrimArray(StringHelper.TrimArray(lineSplit));
            if (lineSplit.Length != 2 && lineSplit.Length != 4)
                throw new Exception("Wrong amount of values given: ScalerCrop=" + lineInfo);

            BayerClipWin = new uint[lineSplit.Length];
            for (int i = 0; i < lineSplit.Length; i++)
                BayerClipWin[i] = uint.Parse(lineSplit[i]);
        }
    }
}
