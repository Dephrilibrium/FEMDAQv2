using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Instrument.LogicalLayer
{
    internal enum FEAR16v2MeasurementChannelType
    {
        CF = 0, // Currentflow
        Drp = 1, // FET Voltagedrop
    }



    public class FEAR16v2Layer : InstrumentLogicalLayer
    {
        public InfoBlockFEAR16v2 InfoBlock { get; private set; }
        private List<double> _sweep;
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

            xResults = new List<List<double>>();
            yResults = new List<List<List<double>>>();
            yResults.Add(new List<List<double>>()); // CurrentFlow
            yResults.Add(new List<List<double>>()); // FET Dropvoltage

            _device = new HaumOTH.FEAR16v2(InfoBlock.ComPort.ComPort, InfoBlock.ComPort.Baudrate);
            if (_device == null) throw new NullReferenceException("FEAR16v2 device couldn't be generated.");



            if (DrawnOverIdentifiers != null)
            {
                foreach (var drawnOver in DrawnOverIdentifiers)
                    xResults.Add(new List<double>());
            }

            //_seriesNames = new List<List<string>>();
            _seriesNames = new List<List<List<string>>>();
            yResults = new List<List<List<double>>>();
            List<string> chIdents = null;
            List<Color> chColors = null;
            string chTypeStr = null;
            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                yResults.Add(new List<List<double>>());
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
                            yResults[iCh].Add(new List<double>());
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
        public List<List<double>> xResults { get; private set; }
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

            for(int iCh = 0; iCh <_device.AmountOfChannels; iCh++)
            {
                if (InfoBlock.CurrFlowChannels[iCh].IsActive && 
                    InfoBlock.CurrFlowChannels[iCh].MeasureInstantly == MeasureCycle)
                    _device.CurrFlowChannels[iCh].Requested = true;

                if (InfoBlock.UDropFETChannels[iCh].IsActive &&
                    InfoBlock.UDropFETChannels[iCh].MeasureInstantly == MeasureCycle)
                    _device.UFETDropChannels[iCh].Requested = true;
            }
            _device.MeasureUFETDropRequests();
            _device.MeasureCurrentFlowRequests();

            double[] drawnOver = GetDrawnOver(DrawnOverIdentifiers);
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
            var voltage = _sweep[sweepLine];

            //_device.SetVoltage(voltage);
            //_device.SetOutput(true); // Output now enabled in Init() instead of each time a value is sent
        }



        public void PowerDownSource()
        {
            _device.ResetCurrentControl();
        }



        public void AssignSweepColumn(SweepContent sweep)
        {
            //if (InfoBlock.Source.SourceNode < 0)
            //    return;

            //var voltageSourceNode = "U" + Convert.ToString(InfoBlock.Source.SourceNode);
            //_sweep = AssignSweep.Assign(sweep, voltageSourceNode);
            //if (_sweep == null) throw new MissingFieldException("Can't find " + voltageSourceNode + " in sweep-file.");

            //// MCP140 can't work with negative voltages -> Check if there are some negative values within sweep-values
            //int numberOfNegativeVoltages = 0;
            //foreach (double voltage in _sweep)
            //{
            //    if (voltage < 0)
            //        numberOfNegativeVoltages++;
            //}

            //if (numberOfNegativeVoltages > 0)
            //    throw new ArgumentOutOfRangeException("MCP140 can't handle negative voltages (found " + numberOfNegativeVoltages.ToString() + " in " + voltageSourceNode + " sweep values)");

        }
        #endregion



        #region UI
        public void UpdateGraph()
        {
        }
        #endregion
    }
}
