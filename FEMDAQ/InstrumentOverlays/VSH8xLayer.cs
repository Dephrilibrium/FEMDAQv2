﻿using System;
using System.Collections.Generic;
using System.Text;
using Files;
using Files.Parser;
using System.IO;

namespace Instrument.LogicalLayer
{



    class VSH8xLayer : InstrumentLogicalLayer
    {
        public InfoBlockVSH8x InfoBlock { get; private set; }
        private VSH8x.VSH8x _device;
        private HaumChart.HaumChart _chart;
        private List<string> _seriesNames;



        public VSH8xLayer(DeviceInfoStructure infoStructure, HaumChart.HaumChart chart)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            if (chart == null) throw new ArgumentNullException("chart");
            InfoBlock = infoStructure.InfoBlock as InfoBlockVSH8x;
            if (InfoBlock == null) throw new InvalidCastException("Cast failed: infoBlock -> InfoBlockVSH8x");

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + (cName == null || cName == "" ? DeviceType : cName);

            //XResults = new List<List<List<double>>>();
            //YResults = new List<List<List<double>>>();
            XResults = new List<List<double>>();
            //YResults = new List<List<double>>();

            _device = new VSH8x.VSH8x(InfoBlock.RS485Address, InfoBlock.ComPort.ComPort, InfoBlock.ComPort.Baudrate);
            if (_device == null) throw new NullReferenceException("VSH8x device couldn't be generated.");
            CommunicationPhy = InstrumentCommunicationPHY.COMPort;

            if(InfoBlock.Common.ChartDrawnOvers != null)
            {
                foreach (var drawnOver in InfoBlock.Common.ChartDrawnOvers)
                    XResults.Add(new List<double>());
            }

            if(InfoBlock.Common.ChartIdentifiers != null)
            {
                _seriesNames = new List<string>();
                for (var index = 0; index < InfoBlock.Common.ChartIdentifiers.Count; index++)
                {
                    _seriesNames.Add(string.Format("{0}-C{1}",
                                                   DeviceName,
                                                   index));
                    chart.AddSeries(InfoBlock.Common.ChartIdentifiers[index], _seriesNames[index], InfoBlock.Common.ChartColors[index]);
                }
                _chart = chart;
            }

            YResults = new List<double>();
        }



        public void Dispose()
        {
            if (_device != null)
                _device.Dispose();

            ClearResults();
            XResults.Clear();
            YResults.Clear();
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
        public List<double> YResults { get; private set; }
        public List<string> DrawnOverIdentifiers { get { return InfoBlock.Common.ChartDrawnOvers; } }
        public GaugeMeasureInstantly InstantMeasurement { get { return InfoBlock.Gauge.MeasureInstantly; } }
        #endregion



        #region Common
        public void Init()
        {
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
        //    StandardGuardClauses.CheckGaugeResultIndicies(indicies, 1, DeviceIdentifier); // Throws exceptions

        //    return XResults[indicies[0]];
        //}
        //public List<double> GetYResultList(int[] indicies)
        //{
        //    //StandardGuardClauses.CheckGaugeResultIndicies(indicies, 0, DeviceIdentifier); // Throws exceptions

        //    return YResults;
        //}

        //public void Measure(double[] drawnOver)
        public void Measure(Func<List<string>, double[]> GetDrawnOver, GaugeMeasureInstantly MeasureCycle)
        {
            if (MeasureCycle != InfoBlock.Gauge.MeasureInstantly)
                return;

            double[] drawnOver = GetDrawnOver(DrawnOverIdentifiers);
            lock(XResults)
            {
                lock(YResults)
                {
                    for (int index = 0; index < _seriesNames.Count; index++)
                        XResults[index].Add(drawnOver[index]);
                    YResults.Add(_device.GetMeasurementValue());
                }
            }
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
            output.Append("# ");
            foreach (var drawnOver in DrawnOverIdentifiers)
                output.Append(drawnOver + ", ");
            output.AppendLine("Y");
            for (var line = 0; line < YResults.Count; line++)
            {
                for (var xRow = 0; xRow < XResults.Count; xRow++)
                    output.Append(Convert.ToString(XResults[xRow][line]) + ",");
                output.AppendLine(Convert.ToString(YResults[line]));
            }

            var filename = folderPath + "\\" + filePrefix + deviceName + ".dat";
            var fileWriter = new StreamWriter(filename, false);
            fileWriter.Write(output);
            fileWriter.Dispose();
        }



        public void ClearResults()
        {
            if(XResults != null)
                foreach (var result in XResults)
                    result.Clear();

            if (YResults != null)
                //foreach (var result in YResults)
                    YResults.Clear();

            if(_seriesNames != null)
                foreach (var seriesName in _seriesNames)
                    _chart.ClearXY(seriesName);
        }
        #endregion



        #region Source
        public double GetSourceValue(string identifier)
        {
            return _device.GetMeasurementValue();
        }



        public void SetSourceValues(int sweepLine)
        {
        }



        public void PowerDownSource()
        {
        }



        public void AssignSweepColumn(SweepContent sweep)
        {
        }
        #endregion


        
        #region UI
        public void UpdateGraph()
        {
            if (_seriesNames == null) // No drawdata
                return;

            int lastLine;
            double lastYVal;
            lock (YResults)
            {
                lastLine = YResults.Count - 1;
                if (lastLine < 0) // Actual no value measured
                    return;
                lastYVal = YResults[lastLine];
            }

            lock (XResults)
            {
                for (var xRowIndex = 0; xRowIndex < _seriesNames.Count; xRowIndex++)
                    _chart.AddXY(_seriesNames[xRowIndex], XResults[xRowIndex][lastLine], lastYVal);
            }
        }
        #endregion
    }
}
