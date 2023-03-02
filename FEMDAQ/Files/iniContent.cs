using FEMDAQ.StaticHelper;
using Files.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Files
{
    public class IniContent
    {
        // Globals
        public string Filepath
        {
            get { return Path.GetDirectoryName(Fullfilename); }
        }
        public string Filename
        {
            get { return Path.GetFileName(Fullfilename); }
        }
        public string Fullfilename { get; private set; }

        // Infoblocks
        private List<List<string>> _infoBlocks = null;
        public InfoBlockToolInfo ToolInfo { get; private set; }
        public InfoBlockToolSettings ToolSettings { get; private set; }
        public InfoBlockSweepInfo SweepInfo { get; private set; }
        public InfoBlockTiming TimingInfo { get; private set; }
        public List<DeviceInfoStructure> DeviceInfoStructures { get; private set; }
        public List<InfoBlockChart> ChartInfo { get; private set; }



        public IniContent(string filename)
        {
            if (filename == null) throw new ArgumentNullException("Filename");
            Fullfilename = filename;

            // Decoding configuration-blocks from ini and try to instanciate the InfoBlocks for later use on devices!
            DecodeIni();
            ParseInfoBlocks();

           // Error-handling
            CheckForMissingBasicBlocks(); // Can throw exceptions
            CheckForDuplicateDeviceEntries(); // Can throw exceptions
            CheckForDuplicateChartEntries(); // Can throw exceptions
        }



        private void DecodeIni()
        {
            // Opening, Reading, closing and quitting ini-File
            StreamReader file = new StreamReader(Fullfilename);
            if (file == null) throw new FileNotFoundException("Filename: " + Fullfilename);

            string[] iniLineByLine = file.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (iniLineByLine.Length < 1)
                return;

            file.Dispose();

            _infoBlocks = new List<List<string>>();
            List<string> infoBlock = null;
            string line = null;
            for (int lineIndex = 0; lineIndex < iniLineByLine.Length; lineIndex++)
            {
                line = iniLineByLine[lineIndex];
                if (line.StartsWith("["))
                {
                    if (infoBlock != null)
                        _infoBlocks.Add(infoBlock);

                    infoBlock = new List<string>();
                }
                if (line.StartsWith("#")) // Remove comments
                    continue;

                infoBlock.Add(line);
            }
            // Add the last infoBlock too!
            if (infoBlock != null)
                _infoBlocks.Add(infoBlock);
        }




        private void ParseInfoBlocks()
        {
            DeviceInfoStructures = new List<DeviceInfoStructure>();
            ChartInfo = new List<InfoBlockChart>();
            string blockIdentifier = null;

            foreach (var infoBlock in _infoBlocks)
            {
                blockIdentifier = infoBlock[0];
                if (blockIdentifier == "[ToolInfo]")
                    ToolInfo = new InfoBlockToolInfo(infoBlock);

                else if (blockIdentifier == "[ToolSettings]")
                    ToolSettings = new InfoBlockToolSettings(infoBlock);

                else if (blockIdentifier == "[SweepInfo]")
                    SweepInfo = new InfoBlockSweepInfo(infoBlock);

                else if (blockIdentifier == "[Timing]")
                    TimingInfo = new InfoBlockTiming(infoBlock);

                else if (blockIdentifier.StartsWith("[Dev"))
                {
                    var splitIdentifier = (string[])ParseHelper.ParseBlockIdentifier(blockIdentifier);
                    DeviceInfoStructure deviceParameterStructure = null;
                    switch (splitIdentifier[1])
                    {
                        // Instanciation of dummydevices-InfoBlocks for test purposes
                        case "DmyMU":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockDmyMU(infoBlock));
                            break;

                        case "DmySU":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockDmySU(infoBlock));
                            break;

                        case "DmySMU":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockDmySMU(infoBlock));
                            break;


                        // Instanciation of real devices-InfoBlocks
                        case "PiCam":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockPiCam(infoBlock));
                            break;

                        case "PiCam2":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockPiCam2(infoBlock));
                            break;

                        case "FEAR16v2":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockFEAR16v2(infoBlock));
                            break;

                        case "KE6485":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockKE6485(infoBlock));
                            break;

                        case "KE6487":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockKE6487(infoBlock));
                            break;

                        case "DMM7510":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockDMM7510(infoBlock));
                            break;

                        case "RTO2034":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockRTO2034(infoBlock));
                            break;

                        case "SR785":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockSR785(infoBlock));
                            break;

                        case "FUGMCP140":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockFUGMCP140(infoBlock));
                            break;

                        case "FUGHCP350":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockFUGHCP350(infoBlock));
                            break;

                        case "WavGen33511B":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockWavGen33511B(infoBlock));
                            break;

                        case "MOVE1250":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockMOVE1250(infoBlock));
                            break;

                        case "VSH8x":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockVSH8x(infoBlock));
                            break;

                        case "VD9x":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockVD9x(infoBlock));
                            break;

                        case "HP4145B":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockHP4145B(infoBlock));
                            break;

                        case "KE6517B":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockKE6517B(infoBlock));
                            break;

                        case "DSOX3034T":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockDSOX3034T(infoBlock));
                            break;

                        case "DSOX3000WavGen":
                            deviceParameterStructure = new DeviceInfoStructure(splitIdentifier[0], splitIdentifier[1], new InfoBlockDSOX3000WavGen(infoBlock));
                            break;

                        default:
                            continue;
                    }
                    DeviceInfoStructures.Add(deviceParameterStructure);
                }
                else if (blockIdentifier.StartsWith("[Chart"))
                {
                    ChartInfo.Add(new InfoBlockChart(infoBlock));
                }
            }
        }

        private void CheckForMissingBasicBlocks()
        {
            if (ToolInfo == null)
                throw new FormatException("No [ToolInfo] block were found");
            if (ToolSettings == null)
                ToolSettings = new InfoBlockToolSettings(); // Don't throw exception. Use default-settings instead!
            if (SweepInfo == null)
                throw new FormatException("No [SweepInfo] block were found");
            if (TimingInfo == null)
                throw new FormatException("No [Timing] block were found");

        }

        private void CheckForDuplicateDeviceEntries()
        {
            var devIdentifierCollection = new List<string>();

            foreach (var info in DeviceInfoStructures)
            {
                devIdentifierCollection.Add(info.DeviceIdentifier);
            }

            if (devIdentifierCollection.Count == 0)
                MessageBox.Show("No [Dev...] blocks were found. The process will continue, but maybe you should check your ini", "No [Dev...] blocks", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CheckForDuplicatesInStringList(devIdentifierCollection);
        }

        private void CheckForDuplicateChartEntries()
        {
            var devIdentifierCollection = new List<string>();

            foreach (var info in ChartInfo)
            {
                devIdentifierCollection.Add(info.ChartInfo.Name);
            }

            if (devIdentifierCollection.Count == 0)
                MessageBox.Show("No [Chart...] blocks were found. The process will continue, but maybe you should check your ini", "No [Chart...] blocks", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CheckForDuplicatesInStringList(devIdentifierCollection);
        }

        private void CheckForDuplicatesInStringList(List<string> stringListToCheck)
        {
            var duplicateStrings = new List<string>();
            var duplicateCounters = new List<int>();

            string currentIdentifier = null;
            string comparismIdentifier = null;
            int duplicateCounter = 0;
            bool duplicateAlreadyInList = false;
            for (int devCompIndex1 = 0; devCompIndex1 < stringListToCheck.Count - 1; devCompIndex1++)
            {
                currentIdentifier = stringListToCheck[devCompIndex1];

                // Look if that identifier was already marked as duplicate
                duplicateAlreadyInList = false;
                foreach (var duplicateIdentifier in duplicateStrings)
                {
                    if (duplicateIdentifier == currentIdentifier)
                    {
                        duplicateAlreadyInList = true; // Mark as already found+counted
                        break;
                    }
                }
                if (duplicateAlreadyInList)
                    continue;

                // Search for duplicates
                duplicateCounter = 0;
                for (int devCompIndex2 = devCompIndex1 + 1; devCompIndex2 < stringListToCheck.Count; devCompIndex2++)
                {
                    comparismIdentifier = stringListToCheck[devCompIndex2];
                    if (currentIdentifier == comparismIdentifier)
                        duplicateCounter++;
                }


                if (duplicateCounter != 0) // Check if duplicate of currentIdentifier were found
                {
                    duplicateStrings.Add(currentIdentifier);
                    duplicateCounters.Add(duplicateCounter + 1); // 1 duplicate found -> 2x the same identifier!
                }
            }

            if (duplicateStrings.Count != 0) // Check if duplicates of any device were found
            {
                // Build message and throw exception
                var message = "Identifier-duplicates found: " + Environment.NewLine;
                for (var listIndex = 0; listIndex < duplicateStrings.Count; listIndex++)
                    message += "- " + duplicateCounters[listIndex] + "x " + duplicateStrings[listIndex] + Environment.NewLine;

                throw new FormatException(message);
            }
        }
    }
}
