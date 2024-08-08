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
//using System.Diagnostics;  // Contains "StopWatch"-Class


using Instrument.LogicalLayer.SubClasses;
using System.Security.AccessControl;

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

            // nSubMeasurementsDone = 0; // Done multiple times during Measure()
            _subMeasTimer = new SubMeasurementTimer(InfoBlock.AdcGeneralSettings.deltatimeSubMeasurements);

            // Make empty result-lists
            XResults = new List<List<List<List<List<double>>>>>(_device.AmountOfChannels);
            YResults = new List<List<List<List<double>>>>(_device.AmountOfChannels);

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

            // XResults[Channels][CF/UD][drawnOver][SubMeasDP]
            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                XResults.Add(new List<List<List<List<double>>>>((int)FEAR16v2MeasurementChannelType.TypeCount));
                YResults.Add(new List<List<List<double>>>((int)FEAR16v2MeasurementChannelType.TypeCount));
                _seriesNames.Add(new List<List<string>>());

                for (int chType = 0; chType <= (int)FEAR16v2MeasurementChannelType.UD; chType++)
                {
                    XResults[iCh].Add(new List<List<List<double>>>());
                    YResults[iCh].Add(new List<List<double>>());
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
                                XResults[iCh][chType].Add(new List<List<double>>());
                        }
                    }
                }
            }
            _chart = chart;

            //RequestDelay_ms = 100; // Already setup by InfoBlockFEAR16v2
        }

        public void Dispose()
        {
            if (_device != null)
            {
                PowerDownSource();
                _device.Dispose();
            }

            InfoBlock.Dispose();

            if (_chart != null && _seriesNames != null)
            {
                foreach (var s1SeriesName in _seriesNames)
                    foreach (var s2SeriesName in s1SeriesName)
                        foreach (var s3SeriesName in s2SeriesName)
                            _chart.ClearXY(s3SeriesName);
            }
            ResultListsHelper.DisposeArbitraryResultList(_seriesNames);
        }



        #region Getter/Setter
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public InstrumentCommunicationPHY CommunicationPhy { get; private set; }
        public List<List<List<List<List<double>>>>> XResults { get; private set; }
        //public List<List<double>> yResults { get; private set; }
        public List<List<List<List<double>>>> YResults { get; private set; }
        public GaugeMeasureInstantly InstantMeasurement { get { return GaugeMeasureInstantly.CycleEnd; } }

        public int nSubMeasurements => InfoBlock.AdcGeneralSettings.nSubMeasurements;
        public int nSubMeasurementsDone { get; private set; }
        public int deltatimeSubMeasurements => InfoBlock.AdcGeneralSettings.deltatimeSubMeasurements;
        private SubMeasurementTimer _subMeasTimer = null;

        public int RequestDelay_ms => InfoBlock.AdcGeneralSettings.RequestDelay_ms; // Delays the request of the measurement data by this time [ms]


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


            // XResults[Channels][CF/UD][drawnOver][GlobalDatapoints][SubMeasDatapoints]
            // --> x/yCapture[Channels][CF/UD][drawnOver][SubMeasDP]
            var xCapture = new List<List<List<List<double>>>>(_device.AmountOfChannels);
            var yCapture = new List<List<List<double>>>(_device.AmountOfChannels);

            bool hasValues2Copy = false;

            for (nSubMeasurementsDone = 0; nSubMeasurementsDone < nSubMeasurements; nSubMeasurementsDone++)
            {
                // Start Timer here to not have "Interval-Time + Measurement-Time" (of the device)
                _subMeasTimer.ResetElapsed();
                _subMeasTimer.Start(); // One-Shot-Timer!


                // Mark the requested channels and channeltypes
                for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
                {
                    for (int iChType = 0; iChType < (int)FEAR16v2MeasurementChannelType.TypeCount; iChType++)
                    {
                        if (adcInfoBlocks[iChType][iCh].MeasureInstantly != MeasureCycle)
                            continue;

                        adcChnls[iChType][iCh].Requested = true;
                        atLeastOneRequest[iChType] = true;
                        hasValues2Copy = true;
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
                    if (nSubMeasurementsDone == 0) // Create nested list for first SubMeas-Datapoint
                    {
                        xCapture.Add(new List<List<List<double>>>((int)FEAR16v2MeasurementChannelType.TypeCount));
                        yCapture.Add(new List<List<double>>((int)FEAR16v2MeasurementChannelType.TypeCount));
                    }

                    for (int iChType = 0; iChType < (int)FEAR16v2MeasurementChannelType.TypeCount; iChType++)
                    {
                        if (nSubMeasurementsDone == 0) // Create nested list for first SubMeas-Datapoint
                        {
                            xCapture[iCh].Add(new List<List<double>>());       // Nested Lists can have variable length!        
                            yCapture[iCh].Add(new List<double>(nSubMeasurements));
                        }

                        if (adcChnls[iChType][iCh].NewValAvailable)
                        {
                            List<string> drawnOverIdentifiers = null;
                            drawnOverIdentifiers = adcInfoBlocks[iChType][iCh].chartInfo.ChartDrawnOvers;

                            double[] drawnOver = null;
                            drawnOver = GetDrawnOver(adcInfoBlocks[iChType][iCh].chartInfo.ChartDrawnOvers);


                            //lock (XResults[iCh][iChType])
                            //{
                            //    lock (YResults[iCh][iChType])
                            //    {
                            //        //YResults[iCh][iChType].Add(adcChnls[iChType][iCh].Value);
                            //        //for (var iDrawnOver = 0; iDrawnOver < drawnOver.Length; iDrawnOver++)
                            //        //    XResults[iCh][iChType][iDrawnOver].Add(drawnOver[iDrawnOver]);
                            //    }
                            //}
                            yCapture[iCh][iChType].Add(adcChnls[iChType][iCh].Value);

                            //var _tmpCurrentDrawnOver = 0.0;
                            for (var _nDrawnOver = 0; _nDrawnOver < drawnOverIdentifiers.Count; _nDrawnOver++) // Take drawnOver of channel! Not the global one of InfoBlock.Gauge!
                            {
                                if (nSubMeasurementsDone == 0) // Create nested list for first SubMeas-Datapoint
                                {
                                    xCapture[iCh][iChType].Add(new List<double>(nSubMeasurements));       // Nested Lists can have variable length!        
                                }

                                /////// Not necessary as FEAR re-captures drawnOver!
                                //if (adcInfoBlocks[iChType][iCh].chartInfo.ChartDrawnOvers[_nDrawnOver] == "Time")                               // If current drawnOver is is time (only time vary due to submeasurements!)
                                //    _tmpCurrentDrawnOver = drawnOver[_nDrawnOver] + nSubMeasurementsDone * (deltatimeSubMeasurements / 1000);   //  adjust global timestamp --> drawnOver + nSubmeasurements * Interval[ms] / 1000[ms/s] = [s]
                                //else                                                                                                            // otherwise
                                //_tmpCurrentDrawnOver = drawnOver[_nDrawnOver];                                                              //  take the raw drawnOver-Value

                                xCapture[iCh][iChType][_nDrawnOver].Add(drawnOver[_nDrawnOver]);
                            }

                        }
                    }
                }

                if (nSubMeasurementsDone < (nSubMeasurements - 1)) // Is there a next submeasurement-datapoint?
                    while (!_subMeasTimer.IsElapsed)               // If yes, then await the subMeas-interval
                        ; // Wait
                else                      // When measured the last subMeas-datapoint
                    _subMeasTimer.Stop(); // Stop (one-shot) timer precautionary to avoid unwanted effects for next global measurement-interval

            }
            if (hasValues2Copy)
            {
                hasValues2Copy = false;
                lock (XResults)
                {
                    lock (YResults)
                    {
                        List<string> drawnOverIdentifiers = null;
                        for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
                        {
                            for (int iChType = 0; iChType < (int)FEAR16v2MeasurementChannelType.TypeCount; iChType++)
                            {
                                if (adcInfoBlocks[iChType][iCh].MeasureInstantly != MeasureCycle) // Skip if Ch is unused or not requested!
                                    continue;

                                drawnOverIdentifiers = adcInfoBlocks[iChType][iCh].chartInfo.ChartDrawnOvers;
                                for (var _nDrawnOver = 0; _nDrawnOver < drawnOverIdentifiers.Count; _nDrawnOver++) // Take drawnOver of channel! Not the global one of InfoBlock.Gauge!
                                {
                                    XResults[iCh][iChType][_nDrawnOver].Add(xCapture[iCh][iChType][_nDrawnOver]);
                                }

                                YResults[iCh][iChType].Add(yCapture[iCh][iChType]);
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
                for (int iChType = 0; iChType < (int)FEAR16v2MeasurementChannelType.TypeCount; iChType++)
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
                            for (int nSubMeasDP = 1; nSubMeasDP <= nSubMeasurements; nSubMeasDP++)
                                output.Append(string.Format("{0}_{1}, ", drawnOver, nSubMeasDP));

                        for (int nSubMeasDP = 1; nSubMeasDP <= nSubMeasurements; nSubMeasDP++)
                            if (nSubMeasDP < nSubMeasurements)
                                output.Append(string.Format("Y_{0}, ", nSubMeasDP));
                            else
                                output.AppendLine(string.Format("Y_{0}", nSubMeasDP));

                        output.AppendLine(@"# Range: %.4f"); // Currently fixed range, but using the finest (ca. 300µV!)
                        output.AppendLine(@"# Average: 1"); // Has no mean function until now!

                        for (var line = 0; line < chnlY.Count; line++)
                        {
                            // Append X-Values
                            for (var xRow = 0; xRow < chnlX.Count; xRow++)
                                for (int nSubMeasDP = 0; nSubMeasDP < nSubMeasurements; nSubMeasDP++)
                                    output.Append(string.Format("{0}, ", chnlX[xRow][line][nSubMeasDP]));
                            //output.Append(string.Format("{0}, ", Convert.ToString(chnlX[xRow][line][nSubMeasDP]) + ", ");

                            // Append Y-Values
                            for (int nSubMeasDP = 0; nSubMeasDP < nSubMeasurements; nSubMeasDP++)
                                if (nSubMeasDP < (nSubMeasurements - 1))
                                    output.Append(string.Format("{0}, ", chnlY[line][nSubMeasDP]));
                                // output.Append(string.Format("{0}, ", Convert.ToString(chnlY[line][nSubMeasDP])));
                                else
                                    output.AppendLine(string.Format("{0}", chnlY[line][nSubMeasDP]));
                            //output.AppendLine(Convert.ToString(chnlY[line][nSubMeasDP]));
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
            // List<List<double>> are the lists containing the global and submeasurement datapoints
            ResultListsHelper.ClearArbitraryNestedResultList<List<List<double>>>(XResults);
            ResultListsHelper.ClearArbitraryNestedResultList<List<List<double>>>(YResults);
            /**************
             * Old code - Can be removed after further ResultListHelper-Tests
             * //if (XResults != null)
             * //{
             * //    foreach(var x1Result in XResults)
             * //        foreach (var x2Result in x1Result)
             * //            foreach (var x3Result in x2Result)
             * //                foreach (var x4Result in x3Result)
             * //                    x4Result.Clear();
             * //}
             * 
             * //if (YResults != null)
             * //{
             * //    foreach (var y1Result in YResults)
             * //        foreach (var y2Result in y1Result)
             * //            foreach(var y3Result in y2Result)
             * //                y3Result.Clear();
             * //}
             * 
            *******************************/
            // Global and submeasurement datapoints are assigned to the same seriesname --> List<string> instead of List<List<string>>
            if (_chart != null && _seriesNames != null)
            {
                foreach (var s1SeriesName in _seriesNames)
                    foreach (var s2SeriesName in s1SeriesName)
                        foreach (var s3SeriesName in s2SeriesName)
                            _chart.ClearXY(s3SeriesName);
            }
            //ResultListsHelper.ClearArbitraryNestedResultList<List<string>>(_seriesNames);
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
            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                for (int iChType = 0; iChType < (int)FEAR16v2MeasurementChannelType.TypeCount; iChType++)
                {
                    if (_seriesNames[iCh][iChType] == null)
                        continue;

                    int iLastEntry;
                    List<double> lastYVal;

                    lock (YResults)
                    {
                        iLastEntry = YResults[iCh][iChType].Count - 1;
                        if (iLastEntry < 0)
                            continue;
                        lastYVal = YResults[iCh][iChType][iLastEntry];

                        lock (XResults)
                        {
                            for (int iChart = 0; iChart < _seriesNames[iCh][iChType].Count; iChart++)
                                _chart.AddXYSet(_seriesNames[iCh][iChType][iChart], XResults[iCh][iChType][iChart][iLastEntry], lastYVal);
                            //_chart.AddXY(_seriesNames[iCh][iChType][iChart], XResults[iCh][iChType][iChart][iLastEntry], lastYVal);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
