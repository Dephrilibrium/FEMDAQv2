﻿using System;
using System.Collections.Generic;
using System.Text;
using Files;
using StanfordResearch;
using Files.Parser;
using System.IO;

namespace Instrument.LogicalLayer
{
    class SR785Layer : InstrumentLogicalLayer
    {
        public InfoBlockSR785 InfoBlock { get; private set; }
        private SR785 _device;
        private HaumChart.HaumChart _chart;
        private List<string> _seriesNames;



        public SR785Layer(DeviceInfoStructure infoStructure, HaumChart.HaumChart chart)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockSR785;
            if (InfoBlock == null) throw new InvalidCastException("InfoBlock to InfoBlockSR785 cast failed.");

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + "|" + (cName == null || cName == "" ? DeviceType : cName);

            XResults=new List<List<double>>();
            YResults=new List<List<double>>();

            _device = new SR785(InfoBlock.Gpib.GpibBoardNumber, (byte)InfoBlock.Gpib.GpibPrimaryAdress, (byte)InfoBlock.Gpib.GpibSecondaryAdress);
            if (_device == null) throw new NullReferenceException("SR785 device couldn't be generated.");
            CommunicationPhy = InstrumentCommunicationPHY.GPIB;

            if (InfoBlock.Common.ChartIdentifiers != null && InfoBlock.Common.ChartIdentifiers[0] != "") // Add the possible chart!
            {
                _seriesNames = new List<string>();
                _seriesNames.Add(string.Format("{0}",
                                               DeviceName
                                               ));
                chart.AddSeries(InfoBlock.Common.ChartIdentifiers[0], _seriesNames[0], InfoBlock.Common.ChartColors[0]);
                _chart = chart;
            }
        }



        public void Dispose()
        {
            if (_device != null)
                _device.Dispose();

            ClearResults();
            if (_chart != null)
                foreach (var seriesName in _seriesNames)
                    _chart.DeleteSeries(seriesName);
            _seriesNames.Clear();

            InfoBlock.Dispose();
        }



        #region Getter/Setter
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public InstrumentCommunicationPHY CommunicationPhy { get; private set; }
        public List<List<double>> XResults { get; private set; }
        public List<List<double>> YResults { get; private set; }
        public GaugeMeasureInstantly InstantMeasurement { get { return InfoBlock.Gauge.MeasureInstantly; } }
        public List<string> DrawnOverIdentifiers { get { return InfoBlock.Common.ChartDrawnOvers; } }
        #endregion



        #region Common
        public void Init()
        {
            _device.SetDisplay(sr785Display.A);
            _device.SetDisplayFormat(sr785DisplayFormat.Single);
            _device.SetMeasurement(sr785Measurement.FFT1);
            _device.SetXAxisView(InfoBlock.XAxisView);
            _device.SetFrequencyBorders(InfoBlock.FftLines, InfoBlock.StartFrequency, InfoBlock.FrequencySpan);
            _device.SetYAxisView(InfoBlock.YAxisView, InfoBlock.YdBUnit, InfoBlock.YPeakUnit, InfoBlock.YMin, InfoBlock.YMax);
            _device.SetWindowType(InfoBlock.WindowType, InfoBlock.ExponentialTimeConstant);
            if (InfoBlock.Averages != 0)
            {
                _device.SetupAverage(sr785AveragingType.Exponential_Continuous, InfoBlock.DisplayedAverage, InfoBlock.Averages, InfoBlock.timeRecordIncrement);
                _device.SetAverage(true);
            }
            else
            {
                _device.SetAverage(false);
            }
        }

        public void DoBeforeStart()
        {
        }

        public void DoAfterFinished()
        {
        }
        #endregion



        #region Gauge
        //public List<double> GetXResultList(int[] indicies)
        //{
        //    StandardGuardClauses.CheckGaugeResultIndicies(indicies, 1, DeviceIdentifier);

        //    return XResults[indicies[0]];
        //}
        //public List<double> GetYResultList(int[] indicies)
        //{
        //    StandardGuardClauses.CheckGaugeResultIndicies(indicies, 1, DeviceIdentifier);

        //    return YResults[indicies[0]];
        //}

        //public void Measure(double[] drawnOver)
        public void Measure(Func<List<string>, double[]> GetDrawnOver, GaugeMeasureInstantly MeasureCycle)
        {
            if (MeasureCycle != InfoBlock.Gauge.MeasureInstantly)
                return;

            var result = _device.Measure();
            var start = result.StartFrequency;
            var diff = result.ResolutionBandwidth;

            var valX = new List<double>();
            var valY = new List<double>(result.Values);
            for (var valueCount = valY.Count; valueCount > 0; start += diff, valueCount--)
                valX.Add(start);

            XResults.Add(valX);
            YResults.Add(valY);
        }



        //public void SaveResultsToFolder(string folderPath)
        //{
        //    SaveResultsToFolder(folderPath, DateTime.Now);
        //}

        public void SaveResultsToFolder(string folderPath, string filePrefix = null)
        {
            // Guard clauses
            if (folderPath == null)
                throw new NullReferenceException("No savepath given");
            if (filePrefix == null)
                filePrefix = "";

            if (InfoBlock.Gauge.MeasureInstantly < 0) // Hadn't done measurements
                return;

            var deviceName = string.Format("{0}_{1}",
                                           InfoBlock.Common.DeviceIdentifier,
                                           (InfoBlock.Common.CustomName == null ? InfoBlock.Common.DeviceType : InfoBlock.Common.CustomName)
                                           );
            var output = new StringBuilder("Device: [" + deviceName + "]\n");
            output.AppendFormat("# dX: {0} [Hz]\n", InfoBlock.FrequencySpan / XResults[0].Count);
            output.AppendFormat("# X-View: {0}\n", InfoBlock.XAxisView.ToString());
            output.AppendFormat("# Y-View: {0}\n", InfoBlock.YAxisView.ToString());
            output.AppendFormat("# Y-Unit: {0}, {1}\n", InfoBlock.YdBUnit.ToString(), InfoBlock.YPeakUnit.ToString());
            output.AppendFormat("# DatasetSize: {0}\n", XResults[0].Count);
            double x, y;
            output.AppendLine("# X, Y\n\n");
            for (var dataSetIndex = 0; dataSetIndex < XResults.Count; dataSetIndex++)
            {
                for (var valueIndex = 0; valueIndex < XResults[dataSetIndex].Count; valueIndex++)
                {
                    x = XResults[dataSetIndex][valueIndex];
                    y = YResults[dataSetIndex][valueIndex];
                    output.AppendLine(string.Format("{0}, {1}", Convert.ToString(x), Convert.ToString(y)));
                }
            }
            var filename = folderPath + "\\" + filePrefix + deviceName + ".dat";
            var fileWriter = new StreamWriter(filename, false);
            fileWriter.Write(output);
            fileWriter.Dispose();
        }



        public void ClearResults()
        {
            if(XResults != null)
                //foreach (var result in XResults)
                    XResults.Clear();

            if(YResults!=null)
                //foreach (var result in YResults)
                    YResults.Clear();

            if (_seriesNames != null)
                foreach (var seriesName in _seriesNames)
                    _chart.ClearXY(seriesName);
        }
        #endregion



        #region Source
        public double GetSourceValue(string identifier)
        {
            return 0;
        }



        public void SetSourceValues(int sweepLine)
        {
        }



        public void AssignSweepColumn(SweepContent sweep)
        {
        }



        public void PowerDownSource()
        {
        }
        #endregion



        #region UI
        public void UpdateGraph()
        {
            if (_seriesNames.Count <= 0) // No drawdata
                return;

            var lastDataSetIndex = XResults.Count - 1;
            if (lastDataSetIndex < 0) // No datasets until now
                return;

            lock(XResults)
            {
                lock(YResults)
                {
                    var x = XResults[lastDataSetIndex];
                    var y = YResults[lastDataSetIndex];
                    _chart.AddXYSet(_seriesNames[0], x, y);
                }
            }
        }
        #endregion
    }
}
