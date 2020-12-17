using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Instrument.LogicalLayer
{
    public class KE6485Layer : InstrumentLogicalLayer
    {
        public InfoBlockKE6485 InfoBlock { get; private set; }
        private KE6485.KE6485 _device;
        private HaumChart.HaumChart _chart;
        private List<string> _seriesNames;



        public KE6485Layer(DeviceInfoStructure infoStructure, HaumChart.HaumChart chart)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockKE6485;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Wrong argument: {0} instead of KE6485", DeriveFromObject.DeriveNameFromStructure(infoStructure.InfoBlock)));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + (cName == null || cName == "" ? DeviceType : cName);

            xResults = new List<List<double>>();
            yResults = new List<List<double>>();

            _device = new KE6485.KE6485(InfoBlock.Gpib.GpibBoardNumber, (byte)InfoBlock.Gpib.GpibPrimaryAdress, (byte)InfoBlock.Gpib.GpibSecondaryAdress);
            if (_device == null) throw new NullReferenceException("KE6485 device couldn't be generated.");

            if(DrawnOverIdentifiers != null)
            {
                foreach (var drawnOver in DrawnOverIdentifiers)
                    xResults.Add(new List<double>());
            }

            if(InfoBlock.Common.ChartIdentifiers != null)
            {
                _seriesNames = new List<string>();
                for(int index = 0; index < InfoBlock.Common.ChartIdentifiers.Count; index++)
                {
                    _seriesNames.Add(string.Format("{0}-C{1}",
                                                   DeviceName,
                                                   index));
                    chart.AddSeries(InfoBlock.Common.ChartIdentifiers[index], _seriesNames[index], InfoBlock.Common.ChartColors[index]);
                }
                _chart = chart;
            }

            yResults.Add(new List<double>()); // Be sure to have an y-vector when no drawnover is given
        }



        public void Dispose()
        {
            if (_device != null)
                _device.Dispose();

            ClearResults();
            xResults.Clear();
            yResults.Clear();
            if (_chart != null)
                foreach (var seriesName in _seriesNames)
                    _chart.DeleteSeries(seriesName);
            _seriesNames.Clear();
        }



        #region Getter/Setter
        public List<List<double>> xResults { get; private set; }
        public List<List<double>> yResults { get; private set; }
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public int InstantMeasurement { get { return InfoBlock.Gauge.MeasureInstantly; } }
        public List<string> DrawnOverIdentifiers { get { return InfoBlock.Common.ChartDrawnOvers; } }
        #endregion



        #region Common
        public void Init()
        {
            _device.Init();
            _device.SetZeroCheck(InfoBlock.ZeroCheck);
            _device.SetAutoZero(InfoBlock.AutoZero);
            if (InfoBlock.Gauge.Range <= 0)
            {
                _device.SetRange(2e-9);
                _device.SetAutoRange(true);
            }
            else
            {
                _device.SetAutoRange(false);
                _device.SetRange(InfoBlock.Gauge.Range);
            }
            _device.SetRate(InfoBlock.Gauge.Nplc);
        }
        #endregion



        #region Gauge
        public void Measure(double[] drawnOver)
        {
            lock (xResults)
            {
                lock (yResults)
                {
                    var current = _device.GetCurrent();
                    yResults[0].Add(current);
                    for (var index = 0; index < DrawnOverIdentifiers.Count; index++)
                        xResults[index].Add(drawnOver[index]);
                }
            }
        }



        public void SaveResultsToFolder(string folderPath)
        {
            SaveResultsToFolder(folderPath, DateTime.Now);
        }

        public void SaveResultsToFolder(string folderPath, DateTime timeStamp)
        {
            if (InfoBlock.Gauge.MeasureInstantly < 0) // Hadn't done measurements!
                return; // Do nothing


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
            output.AppendLine("# NPLC: " + InfoBlock.Gauge.Nplc.ToString());

            for (var line = 0; line < yResults[0].Count; line++)
            {
                for (var xIndex = 0; xIndex < xResults.Count; xIndex++)
                    output.Append(xResults[xIndex][line] + ", ");
                output.AppendLine(yResults[0][line].ToString());
            }
            var filename = folderPath + "\\" + timeStamp.ToString("yyMMdd_HHmm") + "_" + deviceName + ".dat";
            var fileWriter = new StreamWriter(filename, false);
            fileWriter.Write(output);
            fileWriter.Dispose();
        }



        public void ClearResults()
        {
            if (xResults != null)
                foreach (var xResult in xResults)
                    xResult.Clear();

            if (yResults != null)
                yResults[0].Clear();

            if (_chart != null)
                foreach(var seriesName in _seriesNames)
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
        public void UpdateGraph()
        {
            if (_seriesNames.Count <= 0) // No drawdata
                return;

            int lastLine;
            double lastYVal;
            lock (yResults)
            {
                lastLine = yResults[0].Count - 1;
                if (lastLine < 0) // Actual is no value measured
                    return;
                lastYVal = yResults[0][lastLine];
            }

            lock (xResults)
            {
                for (var xRowIndex = 0; xRowIndex < _seriesNames.Count; xRowIndex++)
                    _chart.AddXY(_seriesNames[xRowIndex], xResults[xRowIndex][lastLine], lastYVal);
            }
        }
        #endregion
    }
}