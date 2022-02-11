using System;
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

            xResults = new List<List<List<double>>>();
            yResults = new List<List<List<double>>>();
            xResults.Add(new List<List<double>>());
            yResults.Add(new List<List<double>>());

            _device = new VSH8x.VSH8x(InfoBlock.RS485Address, InfoBlock.ComPort.ComPort, InfoBlock.ComPort.Baudrate);
            if (_device == null) throw new NullReferenceException("VSH8x device couldn't be generated.");

            if(InfoBlock.Common.ChartDrawnOvers != null)
            {
                foreach (var drawnOver in InfoBlock.Common.ChartDrawnOvers)
                    xResults[0].Add(new List<double>());
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

            yResults[0].Add(new List<double>());
        }



        public void Dispose()
        {
            if (_device != null)
                _device.Dispose();

            ClearResults();
            xResults[0].Clear();
            yResults[0].Clear();
            if (_chart != null)
                foreach (var seriesName in _seriesNames)
                    _chart.DeleteSeries(seriesName);
            _seriesNames.Clear();
        }


        #region Getter/Setter
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public List<List<List<double>>> xResults { get; private set; }
        public List<List<List<double>>> yResults { get; private set; }
        public List<string> DrawnOverIdentifiers { get { return InfoBlock.Common.ChartDrawnOvers; } }
        public GaugeMeasureInstantly InstantMeasurement { get { return InfoBlock.Gauge.MeasureInstantly; } }
        #endregion



        #region Common
        public void Init()
        {
        }
        #endregion



        #region Gauge
        //public void Measure(double[] drawnOver)
        public void Measure(Func<List<string>, double[]> GetDrawnOver, GaugeMeasureInstantly MeasureCycle)
        {
            double[] drawnOver = GetDrawnOver(DrawnOverIdentifiers);
            lock(xResults[0])
            {
                lock(yResults[0])
                {
                    for (int index = 0; index < _seriesNames.Count; index++)
                        xResults[0][index].Add(drawnOver[index]);
                    yResults[0][0].Add(_device.GetMeasurementValue());
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
            for (var line = 0; line < yResults[0].Count; line++)
            {
                for (var xRow = 0; xRow < xResults[0].Count; xRow++)
                    output.Append(Convert.ToString(xResults[0][xRow][line]) + ",");
                output.AppendLine(Convert.ToString(yResults[0][0][line]));
            }

            var filename = folderPath + "\\" + filePrefix + deviceName + ".dat";
            var fileWriter = new StreamWriter(filename, false);
            fileWriter.Write(output);
            fileWriter.Dispose();
        }



        public void ClearResults()
        {
            if(xResults != null)
                foreach (var result in xResults[0])
                    result.Clear();

            if (yResults != null)
                foreach (var result in yResults[0])
                    result.Clear();

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
            lock (yResults[0])
            {
                lastLine = yResults[0][0].Count - 1;
                if (lastLine < 0) // Actual no value measured
                    return;
                lastYVal = yResults[0][0][lastLine];
            }

            lock (xResults[0])
            {
                for (var xRowIndex = 0; xRowIndex < _seriesNames.Count; xRowIndex++)
                    _chart.AddXY(_seriesNames[xRowIndex], xResults[0][xRowIndex][lastLine], lastYVal);
            }
        }
        #endregion
    }
}
