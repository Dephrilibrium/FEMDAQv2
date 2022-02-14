using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using HaumOTH;

namespace Instrument.LogicalLayer
{
    internal enum FEAR16v2MeasurementChannelType
    {
        CF = 0, // Currentflow
        Drp = 1, // FET Voltagedrop
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
        private HaumOTH.FEAR16v2 _device;
        private HaumChart.HaumChart _chart;
        //private List<List<string>> _seriesNames;
        private List<List<List<string>>> _seriesNames;



        public FEAR16v2Layer(DeviceInfoStructure infoStructure, HaumChart.HaumChart chart)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockFEAR16v2;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Cast failed: infoBlock -> FEAR16v2InfoBlock"));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + "|" + (cName == null || cName == "" ? DeviceType : cName);
            
            _device = new HaumOTH.FEAR16v2(InfoBlock.ComPort.ComPort, InfoBlock.ComPort.Baudrate);
            if (_device == null) throw new NullReferenceException("FEAR16v2 device couldn't be generated.");

             // Make empty result-lists
            xResults = new List<List<List<double>>>();
            xResults.Add(new List<List<double>>());
            xResults.Add(new List<List<double>>());

            yResults = new List<List<List<double>>>();
            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                yResults.Add(new List<List<double>>());
                for (int iChType = 0; iChType < (int)FEAR16v2MeasurementChannelType.TypeCount; iChType++)
                {
                    yResults[iCh].Add(new List<double>());
                }
            }


            // Register channels to chart
            //_seriesNames = new List<List<string>>();
            _seriesNames = new List<List<List<string>>>();
            //yResults[0] = new List<List<List<double>>>();
            List<string> chIdents = null;
            List<Color> chColors = null;
            string chTypeStr = null;
            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                //yResults.Add(new List<List<double>>());
                //_seriesNames.Add(new List<string>());
                _seriesNames.Add(new List<List<string>>());

                for (int chType = 0; chType <= (int)FEAR16v2MeasurementChannelType.Drp; chType++)
                {
                    if (chType == (int)FEAR16v2MeasurementChannelType.CF)
                    {
                        if (InfoBlock.CurrFlowChannels[iCh].IsActive)
                        {
                            chIdents = InfoBlock.CurrFlowChannels[iCh].chartInfo.ChartIdentifiers;
                            chColors = InfoBlock.CurrFlowChannels[iCh].chartInfo.ChartColors;
                            chTypeStr = "CF";
                        }
                        else
                            chIdents = null;

                    }
                    else if (chType == (int)FEAR16v2MeasurementChannelType.Drp
                        )
                    {
                        if (InfoBlock.UDropFETChannels[iCh].IsActive)
                        {
                            chIdents = InfoBlock.UDropFETChannels[iCh].chartInfo.ChartIdentifiers;
                            chColors = InfoBlock.UDropFETChannels[iCh].chartInfo.ChartColors;
                            chTypeStr = "Drp";
                        }
                        else
                            chIdents = null;
                    }


                    if (InfoBlock.CurrFlowChannels[iCh].IsActive)
                    {
                        if (chIdents != null)
                        {
                            //yResults[iCh].Add(new List<double>());
                            _seriesNames[iCh].Add(new List<string>());
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
                        }
                    }
                }
            }
            _chart = chart;

        }

        public void Dispose()
        {
            if (_device != null)
            {
                PowerDownSource();
                _device.Dispose();
            }
        }



        #region Getter/Setter
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public List<List<List<double>>> xResults { get; private set; }
        //public List<List<double>> yResults { get; private set; }
        public List<List<List<double>>> yResults { get; private set; }
        public GaugeMeasureInstantly InstantMeasurement { get { return GaugeMeasureInstantly.CycleEnd; } }
        public List<string> DrawnOverIdentifiers { get { return InfoBlock.Common.ChartDrawnOvers; } }
        #endregion



        #region Common
        public void Init()
        {
            _device.ResetCurrentControl();
        }
        #endregion



        #region Gauge | collects the voltages to each iterate!
        public void Measure(Func<List<string>, double[]> GetDrawnOver, GaugeMeasureInstantly MeasureCycle)
        {
            var curr = _device.CurrFlowChannels;
            var drop = _device.UFETDropChannels;
            bool oneCFRequest = false;
            bool oneUDRequest = false;

            for(int iCh = 0; iCh <_device.AmountOfChannels; iCh++)
            {
                if (InfoBlock.CurrFlowChannels[iCh].IsActive &&
                    InfoBlock.CurrFlowChannels[iCh].MeasureInstantly == MeasureCycle)
                {
                    oneCFRequest = true;
                    _device.CurrFlowChannels[iCh].Requested = true;
                }

                if (InfoBlock.UDropFETChannels[iCh].IsActive &&
                    InfoBlock.UDropFETChannels[iCh].MeasureInstantly == MeasureCycle)
                {
                    oneUDRequest = true;
                    _device.UFETDropChannels[iCh].Requested = true;
                }
            }
            if(oneUDRequest)
                _device.MeasureUFETDropRequests();

            if (oneCFRequest)
                _device.MeasureCurrentFlowRequests();


            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                double[] drawnOver = GetDrawnOver(DrawnOverIdentifiers);
                for (int iChType = 0; iChType < (int)FEAR16v2MeasurementChannelType.TypeCount; iChType++)
                {
                    lock (xResults[iCh][iChType])
                    {
                        lock (yResults[iCh][iChType])
                        {
                            double value = 0.0;
                            switch((FEAR16v2MeasurementChannelType)iChType)
                            {
                                case FEAR16v2MeasurementChannelType.CF:
                                    yResults[iCh][iChType] = _device.CurrFlowChannels[iCh].value;
                                    xResults[iCh][iCh]
                                    break;

                                case FEAR16v2MeasurementChannelType.Drp:
                                    value = _device.UFETDropChannels[iCh].value;
                                    break;
                            }
                            
                            yResults[iCh][iChType].Add(value);
                            for (var index = 0; index < DrawnOverIdentifiers.Count; index++)
                                xResults[0][index].Add(drawnOver[index]);
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
        }



        public void ClearResults()
        {
        }
        #endregion



        #region Source
        public double GetSourceValue(string identifier)
        {
            Random rand = new Random();
            return rand.NextDouble() * 10;
            //return _device.CurrCtrlChannels[iCh].value;
        }



        public void SetSourceValues(int sweepLine)
        {
            var srcValues = _sweep[sweepLine];
            bool oneRequest = false;

            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                if (!InfoBlock.CurrCtrlChannels[iCh].IsActive)
                    continue;

                oneRequest = true;
                _device.CurrCtrlChannels[iCh].Requested = true;
                _device.CurrCtrlChannels[iCh].value = srcValues[iCh];
            }

            if (oneRequest)
                _device.UpdateCurrentControlRequests();
        }



        public void PowerDownSource()
        {
            _device.ResetCurrentControl();
        }



        public void AssignSweepColumn(SweepContent sweep)
        {
            _sweep = new List<List<double>>();
            for(int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                _sweep.Add(new List<double>());

                if (InfoBlock.CurrCtrlChannels[iCh].IsActive && InfoBlock.CurrCtrlChannels[iCh].SourceNode >= 0)
                {
                    var sourceNode = "CC" + Convert.ToString(InfoBlock.CurrCtrlChannels[iCh].SourceNode);
                    _sweep.Add(new List<double>());
                    _sweep[iCh] = AssignSweep.Assign(sweep, sourceNode);
                    if (_sweep == null) throw new MissingFieldException("Can't find " + sourceNode + " in sweep-file.");

                    // Check range of values
                    for (int iValue = 0; iValue < _sweep.Count; iValue++)
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
                else if (InfoBlock.CurrCtrlChannels[iCh].IsActive && InfoBlock.CurrCtrlChannels[iCh].SourceNode < 0)
                    throw new Exception("Active currentcontrol (Ch" + iCh.ToString() + " has invalid sourceNode number (SN < 0).");
            }
        }
        #endregion



        #region UI
        public void UpdateGraph()
        {
        }
        #endregion
    }
}
