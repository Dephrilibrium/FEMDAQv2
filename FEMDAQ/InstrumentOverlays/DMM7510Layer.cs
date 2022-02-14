using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using Keithley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace Instrument.LogicalLayer
{
    public class DMM7510Layer : InstrumentLogicalLayer
    {
        public InfoBlockDMM7510 InfoBlock { get; private set; }
        private DMM7510 _device;
        private HaumChart.HaumChart _chart;
        private List<string> _seriesNames;



        public DMM7510Layer(DeviceInfoStructure infoStructure, HaumChart.HaumChart chart)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockDMM7510;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Wrong argument: {0} instead of DMM7510", DeriveFromObject.DeriveNameFromStructure(infoStructure.InfoBlock)));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + "|" + (cName == null || cName == "" ? DeviceType : cName);

            xResults = new List<List<List<double>>>();
            yResults = new List<List<List<double>>>();
            xResults .Add( new List<List<double>>());
            yResults .Add( new List<List<double>>());

            _device = new DMM7510(InfoBlock.Gpib.GpibBoardNumber, (byte)InfoBlock.Gpib.GpibPrimaryAdress, (byte)InfoBlock.Gpib.GpibSecondaryAdress);
            if (_device == null) throw new NullReferenceException("KE6487 device couldn't be generated.");

            if (DrawnOverIdentifiers != null)
            {
                foreach (var drawnOver in DrawnOverIdentifiers)
                    xResults[0].Add(new List<double>());
            }

            if (InfoBlock.Common.ChartIdentifiers != null)
            {
                _seriesNames = new List<string>();
                for (int index = 0; index < InfoBlock.Common.ChartIdentifiers.Count; index++)
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
        public List<List<List<double>>> xResults { get; private set; }
        public List<List<List<double>>> yResults { get; private set; }
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public GaugeMeasureInstantly InstantMeasurement { get { return InfoBlock.Gauge.MeasureInstantly; } }
        public List<string> DrawnOverIdentifiers { get { return InfoBlock.Common.ChartDrawnOvers; } }
        #endregion



        #region Common
        public void Init()
        {
            var couplingAC = DMM7510.ConvertCoupling(InfoBlock.Coupling);
            if (InfoBlock.MeasurementType == "VOLT")
            {
                DMM7510_VoltageRange vRange = DMM7510.ConvertVoltageRange(InfoBlock.Gauge.Range);
                _device.SetVoltageMeasurement(couplingAC, vRange, InfoBlock.Gauge.Nplc, InfoBlock.AutoZero);
            }
            else if (InfoBlock.MeasurementType == "CURR")
            {
                DMM7510_CurrentRange cRange = DMM7510.ConvertCurrentRange(InfoBlock.Gauge.Range);
                _device.SetCurrentMeasurement(couplingAC, cRange, InfoBlock.Gauge.Nplc, InfoBlock.AutoZero);
            }
            else throw new ArgumentOutOfRangeException("Unsupported measurementtype.");
        }
        #endregion



        #region Gauge
        //public void Measure(double[] drawnOver)
        public void Measure(Func<List<string>, double[]> GetDrawnOver, GaugeMeasureInstantly MeasureCycle)
        {
            double[] drawnOver = GetDrawnOver(DrawnOverIdentifiers);

            lock (xResults[0])
            {
                lock (yResults[0])
                {
                    yResults[0][0].Add(_device.Measure());
                    for (var index = 0; index < DrawnOverIdentifiers.Count; index++)
                        xResults[0][index].Add(drawnOver[index]);
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

            if (InfoBlock.Gauge.MeasureInstantly < 0) // Hadn't done measurements!
                return;

            var deviceName = string.Format("{0}_{1}",
                                           InfoBlock.Common.DeviceIdentifier,
                                           (InfoBlock.Common.CustomName == null ? InfoBlock.Common.DeviceType : InfoBlock.Common.CustomName)
                                           );
            var output = new StringBuilder("# Device: [" + deviceName + "]\n");
            output.Append("# ");
            foreach (var drawnOver in DrawnOverIdentifiers)
                output.Append(drawnOver + ", ");
            output.AppendLine("Y");
            output.AppendLine("# Range: " + InfoBlock.Gauge.Range.ToString());

            for (var line = 0; line < yResults[0][0].Count; line++)
            {
                for (var xRow = 0; xRow < DrawnOverIdentifiers.Count; xRow++)
                    output.Append(Convert.ToString(xResults[0][xRow][line]) + ", ");
                output.AppendLine(Convert.ToString(yResults[0][0][line]));
            }
            var filename = folderPath + "\\" + filePrefix + deviceName + ".dat";
            var fileWriter = new StreamWriter(filename, false);
            fileWriter.Write(output);
            fileWriter.Dispose();
        }



        public void ClearResults()
        {
            if (xResults[0] != null)
                foreach (var xResult in xResults[0])
                    xResult.Clear();

            if (yResults[0] != null)
                yResults[0][0].Clear();

            if (_chart != null)
                foreach (var seriesName in _seriesNames)
                    _chart.ClearXY(seriesName);

            if (_chart != null)
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



        public void PowerDownSource()
        {
        }



        public void AssignSweepColumn(SweepContent sweep)
        {
        }
        #endregion



        #region UI
        // Invoke this when using threads!
        public void UpdateGraph()
        {
            if (_seriesNames.Count <= 0) // No drawdata
                return;

            int lastLine;
            double lastYVal;
            lock (yResults[0])
            {
                lastLine = yResults[0][0].Count - 1;
                if (lastLine < 0) // Actual is no value measured
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
