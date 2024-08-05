using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using HaumOTH;
using System.Threading;

namespace Instrument.LogicalLayer
{
    internal enum FEAR16v2MeasurementChannelType
    {
        CF = 0, // Currentflow ([C]urrent[F]low)
        UD = 1, // FET Voltagedrop (FET [UD]rop)
        TypeCount = 2 // Amount of channel-values
    }

    //internal class FEAR16v2ChannelRequests
    //{
    //    public bool CurrCtrl = false;
    //    public bool CurrFlow = false;
    //    public bool FETUDrop = false;
    //}



    public class FEAR16v2Layer : InstrumentLogicalLayer
    {
        public InfoBlockFEAR16v2 InfoBlock { get; private set; }
        private List<List<double>> _sweep;
        private double maxVal = 10.0; // Maximum is +10V;
        private double minVal = -10.0; // Minimum is -10V
        private FEAR16v2 _device;
        private HaumChart.HaumChart _chart;
        //private List<List<string>> _seriesNames;
        private List<List<List<string>>> _seriesNames;

        public const string CurrCtrlRequestString = "CC";
        public const string CurrFlowRequestString = "CF";
        public const string FETUDropRequestString = "UD";

        public int RequestDelay_ms; // Delays the request of the measurement data by this time [ms]


        public FEAR16v2Layer(DeviceInfoStructure infoStructure, HaumChart.HaumChart chart)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockFEAR16v2;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Cast failed: infoBlock -> FEAR16v2InfoBlock"));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + "|" + (cName == null || cName == "" ? DeviceType : cName);

            _device = new FEAR16v2(InfoBlock.ComPort.ComPort, InfoBlock.ComPort.Baudrate);
            if (_device == null) throw new NullReferenceException("FEAR16v2 device couldn't be generated.");
            CommunicationPhy = InstrumentCommunicationPHY.COMPort; // Via USB -> FT232

            
            _device.ChangeAdcNMeanPoints(InfoBlock.AdcGeneralSettings.AdcNMean);
            _device.ChangeAdcMDeltaTime(InfoBlock.AdcGeneralSettings.AdcMDelta);

            // Make empty result-lists
            XResults = new List<List<List<List<double>>>>(_device.AmountOfChannels);
            YResults = new List<List<List<double>>>(_device.AmountOfChannels);

            //for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            //{
            //    //XResults.Add(new List<List<List<double>>>()); // Currentflow
            //    //XResults.Add(new List<List<List<double>>>()); // UDrop
            //    //YResults.Add(new List<List<double>>()); // Currentflow
            //    //YResults.Add(new List<List<double>>()); // UDrop
            //}


            // Register channels to chart
            //_seriesNames = new List<List<string>>();
            _seriesNames = new List<List<List<string>>>();
            //yResults[0] = new List<List<List<double>>>();
            FEAR16ADCChannel currentADCChannel = null;
            List<string> chIdents = null;
            List<Color> chColors = null;
            List<string> chDrawnOver = null;
            string chTypeStr = null;
            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                XResults.Add(new List<List<List<double>>>((int)FEAR16v2MeasurementChannelType.TypeCount));
                YResults.Add(new List<List<double>>((int)FEAR16v2MeasurementChannelType.TypeCount));
                _seriesNames.Add(new List<List<string>>());

                for (int chType = 0; chType <= (int)FEAR16v2MeasurementChannelType.UD; chType++)
                {
                    XResults[iCh].Add(new List<List<double>>());
                    YResults[iCh].Add(new List<double>());
                    _seriesNames[iCh].Add(new List<string>());

                    if (chType == (int)FEAR16v2MeasurementChannelType.CF)
                    {
                        currentADCChannel = InfoBlock.CurrFlowChannels[iCh];
                        chTypeStr = CurrFlowRequestString; // CurrentFlow request string
                    }
                    else if (chType == (int)FEAR16v2MeasurementChannelType.UD)
                    {
                        currentADCChannel = InfoBlock.UDropFETChannels[iCh];
                        chTypeStr = FETUDropRequestString; // FET Voltage Drop request string
                    }


                    if (currentADCChannel.MeasureInstantly != GaugeMeasureInstantly.Disabled)
                    {
                        chIdents = currentADCChannel.chartInfo.ChartIdentifiers;
                        chColors = currentADCChannel.chartInfo.ChartColors;
                        chDrawnOver = currentADCChannel.chartInfo.ChartDrawnOvers;

                        if (chIdents != null)
                        {
                            //yResults[iCh].Add(new List<double>());
                            for (var index = 0; index < chIdents.Count; index++)
                            {
                                //_seriesNames[iCh].Add(string.Format("{0}CF{1}-C{2}",
                                _seriesNames[iCh][chType].Add(string.Format("{0}-{1}{2}-C{3}",
                                                           DeviceName,
                                                           chTypeStr,
                                                           iCh,
                                                           index));
                                //chart.AddSeries(chIdents[index], _seriesNames[chType][index], chColors[index]);
                                chart.AddSeries(chIdents[index], _seriesNames[iCh][chType][index], chColors[index]);
                            }

                            for (var iDrawnOver = 0; iDrawnOver < chDrawnOver.Count; iDrawnOver++)
                                XResults[iCh][chType].Add(new List<double>());
                        }
                    }
                }
            }
            _chart = chart;

            RequestDelay_ms = 100; // Default = 100ms
        }

        public void Dispose()
        {
            if (_device != null)
            {
                PowerDownSource();
                _device.Dispose();
            }

            InfoBlock.Dispose();
        }



        #region Getter/Setter
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public InstrumentCommunicationPHY CommunicationPhy { get; private set; }
        public List<List<List<List<double>>>> XResults { get; private set; }
        //public List<List<double>> yResults { get; private set; }
        public List<List<List<double>>> YResults { get; private set; }
        public GaugeMeasureInstantly InstantMeasurement { get { return GaugeMeasureInstantly.CycleEnd; } }
        public List<string> DrawnOverIdentifiers { get { return InfoBlock.Common.ChartDrawnOvers; } }
        #endregion



        #region Common
        public void Init()
        {
            _device.ResetCurrentControl();
        }

        public void DoBeforeStart()
        {
        }

        public void DoAfterFinished()
        {
        }
        #endregion



        #region Gauge | collects the voltages to each iterate!
        //public List<double> GetXResultList(int[] indicies)
        //{
        //    StandardGuardClauses.CheckGaugeResultIndicies(indicies, 3, DeviceIdentifier);

        //    return XResults[indicies[0]][indicies[1]][indicies[2]];
        //}

        //public List<double> GetYResultList(int[] indicies)
        //{
        //    StandardGuardClauses.CheckGaugeResultIndicies(indicies, 2, DeviceIdentifier);

        //    return YResults[indicies[0]][indicies[1]];
        //}



        public void Measure(Func<List<string>, double[]> GetDrawnOver, GaugeMeasureInstantly MeasureCycle)
        {
            // Combine structures for following iterations
            var adcInfoBlocks = new List<FEAR16ADCChannel>[(int)FEAR16v2MeasurementChannelType.TypeCount];
            adcInfoBlocks[(int)FEAR16v2MeasurementChannelType.CF] = InfoBlock.CurrFlowChannels;
            adcInfoBlocks[(int)FEAR16v2MeasurementChannelType.UD] = InfoBlock.UDropFETChannels;

            var adcChnls = new List<FEAR16v2ChannelRequest>[(int)FEAR16v2MeasurementChannelType.TypeCount];
            adcChnls[(int)FEAR16v2MeasurementChannelType.CF] = _device.CurrFlowChannels;
            adcChnls[(int)FEAR16v2MeasurementChannelType.UD] = _device.UFETDropChannels;

            var atLeastOneRequest = new bool[(int)FEAR16v2MeasurementChannelType.TypeCount];
            atLeastOneRequest[(int)FEAR16v2MeasurementChannelType.CF] = false;
            atLeastOneRequest[(int)FEAR16v2MeasurementChannelType.UD] = false;


            // Mark the requested channels and channeltypes
            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                for (int iChType = 0; iChType < (int)FEAR16v2MeasurementChannelType.TypeCount; iChType++)
                {
                    if (adcInfoBlocks[iChType][iCh].MeasureInstantly != MeasureCycle)
                        continue;

                    adcChnls[iChType][iCh].Requested = true;
                    atLeastOneRequest[iChType] = true;
                }
            }

            if (RequestDelay_ms > 0) // Delay request if a waiting time is given!
                Thread.Sleep(RequestDelay_ms);

            // Run the measurements if at least one channel is requested
            if (atLeastOneRequest[(int)FEAR16v2MeasurementChannelType.CF])
                _device.MeasureUFETDropRequests();

            if (atLeastOneRequest[(int)FEAR16v2MeasurementChannelType.UD])
                _device.MeasureCurrentFlowRequests();


            // Put the results into the result-lists
            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                for (int iChType = 0; iChType < (int)FEAR16v2MeasurementChannelType.TypeCount; iChType++)
                {
                    if(adcChnls[iChType][iCh].NewValAvailable)
                    {
                        double[] drawnOver = null;
                        drawnOver = GetDrawnOver(adcInfoBlocks[iChType][iCh].chartInfo.ChartDrawnOvers);


                        lock (XResults[iCh][iChType])
                        {
                            lock (YResults[iCh][iChType])
                            {
                                YResults[iCh][iChType].Add(adcChnls[iChType][iCh].Value);
                                for (var iDrawnOver = 0; iDrawnOver < drawnOver.Length; iDrawnOver++)
                                    XResults[iCh][iChType][iDrawnOver].Add(drawnOver[iDrawnOver]);
                            }
                        }
                    }
                }
            }
        }



        //public void SaveResultsToFolder(string folderPath)
        //{
        //}
        public void SaveResultsToFolder(string folderPath, string filePrefix)
        {
            // Guard clauses
            if (folderPath == null)
                throw new NullReferenceException("No savepath given");
            if (filePrefix == null)
                filePrefix = "";


            List<FEAR16ADCChannel>[] chnlNfoList = new List<FEAR16ADCChannel>[(int)FEAR16v2MeasurementChannelType.TypeCount];
            chnlNfoList[(int)FEAR16v2MeasurementChannelType.CF] = InfoBlock.CurrFlowChannels;
            chnlNfoList[(int)FEAR16v2MeasurementChannelType.UD] = InfoBlock.UDropFETChannels;

            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                for(int iChType = 0; iChType < (int)FEAR16v2MeasurementChannelType.TypeCount; iChType++)
                {
                    var chnlNfo = chnlNfoList[iChType][iCh]; // Get current infoBlock

                    if (chnlNfo.MeasureInstantly != GaugeMeasureInstantly.Disabled) // Hadn't done measurements!
                    {
                        // Grab results of selected channel and type
                        var chnlX = XResults[iCh][iChType];
                        var chnlY = YResults[iCh][iChType];

                        var deviceName = string.Format("{0}_{1}(Ch{2}{3})",
                                                       chnlNfo.chartInfo.DeviceIdentifier,
                                                       (chnlNfo.chartInfo.CustomName == null ? chnlNfo.chartInfo.DeviceType : chnlNfo.chartInfo.CustomName),
                                                       iCh,
                                                       ((FEAR16v2MeasurementChannelType)iChType).ToString()
                                                       );
                        var output = new StringBuilder("# Device: [" + deviceName + "]\n");
                        output.Append("# ");
                        foreach (var drawnOver in chnlNfo.chartInfo.ChartDrawnOvers)
                            output.Append(drawnOver + ", ");
                        output.AppendLine("Y");
                        output.AppendLine(@"# Range: %.4f"); // Currently fixed range, but using the finest (ca. 300µV!)
                        output.AppendLine(@"# Average: 1"); // Has no mean function until now!

                        for (var line = 0; line < chnlY.Count; line++)
                        {
                            for (var xRow = 0; xRow < chnlX.Count; xRow++)
                                output.Append(Convert.ToString(chnlX[xRow][line]) + ", ");
                            output.AppendLine(Convert.ToString(chnlY[line]));
                        }
                        var filename = folderPath + "\\" + filePrefix + deviceName + ".dat";
                        var fileWriter = new StreamWriter(filename, false);
                        fileWriter.Write(output);
                        fileWriter.Dispose();
                    }
                }
            }
        }



        public void ClearResults()
        {
            if (XResults != null)
            {
                foreach (var xxxResult in XResults)
                    foreach (var xxResult in xxxResult)
                        foreach (var xResult in xxResult)
                            xResult.Clear();
            }

            if (YResults != null)
            {
                foreach (var yyResult in YResults)
                    foreach (var yResult in yyResult)
                        yResult.Clear();
            }

            if(_chart !=  null)
            {
                foreach (var ssserName in _seriesNames)
                    foreach (var sserName in ssserName)
                        foreach (var serName in sserName)
                            _chart.ClearXY(serName);
            }

        }
        #endregion



        #region Source
        public double GetSourceValue(string identifier)
        {
            var split = identifier.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var chnlNum = 0; // Default: Ch0
            var srcValType = CurrCtrlRequestString; // Default: CurrentControl

            if (split.Length >= 2)
            {
                if (!split[1].ToUpper().StartsWith("CH"))
                    throw new Exception("No channel as DrawnOver requested (Request was for: " + split[1] + " requested.");
                chnlNum = int.Parse(split[1].Substring(2));
                if (chnlNum < 0 || chnlNum > _device.AmountOfChannels)
                    throw new Exception("A device requests an invalid channel-number.");
            }
            if (split.Length >= 3)
                srcValType = split[2];


            switch (srcValType.ToUpper())
            {
                case CurrFlowRequestString:
                    return _device.CurrFlowChannels[chnlNum].Value;

                case FETUDropRequestString:
                    return _device.UFETDropChannels[chnlNum].Value;

                case CurrCtrlRequestString:
                    return _device.CurrCtrlChannels[chnlNum].Value;

                default:
                    throw new Exception("A device requests an invalid drawnOver.");
            }
        }



        public void SetSourceValues(int sweepLine)
        {
            bool atLeastOneRequested = false;

            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                if (InfoBlock.CurrCtrlChannels[iCh].SourceNode < 0)
                    continue;

                atLeastOneRequested = true;
                _device.CurrCtrlChannels[iCh].Requested = true;
                _device.CurrCtrlChannels[iCh].Value = _sweep[iCh][sweepLine];
            }

            if (atLeastOneRequested)
                _device.UpdateCurrentControlRequests();
        }



        public void PowerDownSource()
        {
            _device.ResetCurrentControl();
        }



        public void AssignSweepColumn(SweepContent sweep)
        {
            _sweep = new List<List<double>>();
            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                _sweep.Add(new List<double>());

                if (InfoBlock.CurrCtrlChannels[iCh].SourceNode >= 0)
                {
                    var sourceNode = CurrCtrlRequestString + Convert.ToString(InfoBlock.CurrCtrlChannels[iCh].SourceNode);
                    //_sweep.Add(new List<double>());
                    _sweep[iCh] = AssignSweep.Assign(sweep, sourceNode);
                    if (_sweep == null) throw new Exception("Can't find " + sourceNode + " in sweep-file.");

                    // Check range of values
                    for (int iValue = 0; iValue < _sweep[iCh].Count; iValue++)
                    {
                        if (_sweep[iCh][iValue] < minVal || _sweep[iCh][iValue] > maxVal)
                        {
                            throw new Exception(string.Format("Value of {0} (Line {1}) out of range: {2} (allowed: {3} <= val <= {4})",
                                sourceNode,
                                iValue.ToString(),
                                _sweep[iCh][iValue],
                                minVal,
                                maxVal));
                        }
                    }
                }
                //else if (InfoBlock.CurrCtrlChannels[iCh].SourceNode < 0)
                //    throw new Exception("Active currentcontrol (Ch" + iCh.ToString() + " has invalid sourceNode number (SN < 0).");
            }
        }
        #endregion



        #region UI
        public void UpdateGraph()
        {
            for(int iCh = 0; iCh <_device.AmountOfChannels;iCh++)
            {
                for (int iChType = 0; iChType < (int)FEAR16v2MeasurementChannelType.TypeCount; iChType++)
                {
                    if (_seriesNames[iCh][iChType] == null)
                        continue;

                    int iLastEntry;
                    double lastYVal;

                    lock(YResults)
                    {
                        iLastEntry = YResults[iCh][iChType].Count - 1;
                        if (iLastEntry < 0)
                            continue;
                        lastYVal = YResults[iCh][iChType][iLastEntry];
                    }

                    lock(XResults)
                    {
                        for (int iChart = 0; iChart < _seriesNames[iCh][iChType].Count; iChart++)
                            _chart.AddXY(_seriesNames[iCh][iChType][iChart], XResults[iCh][iChType][iChart][iLastEntry], lastYVal);
                    }
                }
            }
        }
        #endregion
    }
}
