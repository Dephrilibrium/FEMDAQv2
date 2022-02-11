using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Instrument.LogicalLayer
{
    public class FEAR16v2Layer : InstrumentLogicalLayer
    {
        public InfoBlockFEAR16v2 InfoBlock { get; private set; }
        private List<double> _sweep;
        private HaumOTH.FEAR16v2 _device;
        private HaumChart.HaumChart _chart;
        private List<List<string>> _seriesNames;



        public FEAR16v2Layer(DeviceInfoStructure infoStructure, HaumChart.HaumChart chart)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockFEAR16v2;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Cast failed: infoBlock -> FEAR16v2InfoBlock"));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + (cName == null || cName == "" ? DeviceType : cName);

            //_device = new HaumOTH.FEAR16v2(InfoBlock.ComPort.ComPort, InfoBlock.ComPort.Baudrate);
            //if (_device == null) throw new NullReferenceException("FEAR16v2 device couldn't be generated.");

            if (DrawnOverIdentifiers != null)
            {
                foreach (var drawnOver in DrawnOverIdentifiers)
                    xResults.Add(new List<double>());
            }

            _seriesNames = new List<List<string>>();
            for (int iCh = 0; iCh < _device.AmountOfChannels; iCh++)
            {
                _seriesNames.Add(new List<string>());
                if (InfoBlock.Common.ChartIdentifiers != null)
                {
                    
                    for (var index = 0; index < InfoBlock.Common.ChartIdentifiers.Count; index++)
                    {
                        _seriesNames[iCh].Add(string.Format("{0}Ch{1}-C{2}",
                                                       DeviceName,
                                                       iCh,
                                                       index));
                        chart.AddSeries(InfoBlock.Common.ChartIdentifiers[index], _seriesNames[iCh][index], InfoBlock.Common.ChartColors[index]);
                    }
                    _chart = chart;
                }
            }
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
        public List<List<double>> yResults { get; private set; }
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
        void Measure(Func<List<string>, double[]> GetDrawnOver, GaugeMeasureInstantly MeasureCycle)
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
            _device.MeasureCurrentFlowRequests();
            _device.MeasureUFETDropRequests();
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
            var split = identifier.Split(new char[] { '|' });
            var iCh = int.Parse(split[1].Remove(0, 2));
            return _device.CurrCtrlChannels[iCh].value;
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
