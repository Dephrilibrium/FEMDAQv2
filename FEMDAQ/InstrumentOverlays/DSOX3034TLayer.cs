using Files;
using Files.Parser;
using Keysight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Instrument.LogicalLayer
{
    public class DSOX3034TLayer : InstrumentLogicalLayer
    {
        public InfoBlockDSOX3034T InfoBlock { get; private set; }
        private DSOX3034T _device;
        private List<string> _seriesNames;
        private HaumChart.HaumChart _chart;

        private int _setsPerMeasure = 0;
        private List<int> _waveformIndicies;


        public DSOX3034TLayer(DeviceInfoStructure infoStructure, HaumChart.HaumChart chart)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockDSOX3034T;
            if (InfoBlock == null) throw new ArgumentException("InfoBlock-Convertion to InfoBlockRTO2034 failed.");

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + "|" + (cName == null || cName == "" ? DeviceType : cName);
            
            DrawnOverIdentifiers = new List<string>();
            XResults = new List<List<double>>();
            YResults = new List<List<double>>();

            _seriesNames = new List<string>();
            if (InfoBlock.Common != null) // is null when ReadWavefrom = 0
            {
                _setsPerMeasure += 2;
                _waveformIndicies = new List<int>();
                DrawnOverIdentifiers.Add("TIME");
                if (InfoBlock.Common.ChartIdentifiers != null && InfoBlock.Common.ChartIdentifiers[0] != "") // Add one chart!
                {
                    _seriesNames.Add(string.Format("{0}-Waveform",
                                                   DeviceName
                                                   ));
                    chart.AddSeries(InfoBlock.Common.ChartIdentifiers[0], _seriesNames[_seriesNames.Count - 1], InfoBlock.Common.ChartColors[0]);
                }
                _chart = chart;
            }

            _device = new DSOX3034T(InfoBlock.Usb.USBAddress, InfoBlock.Channel);
            if (_device == null) throw new NullReferenceException("DSXO3034T device couldn't be generated.");
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
        }



        #region Getter/Setter
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public List<int> WaveformIndiciesInResults { get; private set; }
        public List<List<double>> XResults { get; private set; }
        public List<List<double>> YResults { get; private set; }
        public GaugeMeasureInstantly InstantMeasurement { get { return InfoBlock.Gauge.MeasureInstantly; } }
        public List<string> DrawnOverIdentifiers { get; private set; }
        #endregion



        #region Common
        public void Init()
        {
            _device.YDivScale = InfoBlock.Gauge.Range;
            _device.XDivScale = InfoBlock.XDivScale;
            _device.TriggerSetup(InfoBlock.TriggerLevel,
                                 InfoBlock.TriggerSource,
                                 InfoBlock.TriggerMode,
                                 InfoBlock.TriggerType,
                                 InfoBlock.TriggerSlope,
                                 InfoBlock.ForceTrigger);
        }
        #endregion



        #region Gauge
        public List<double> GetXResultList(int[] indicies)
        {
            StandardGuardClauses.CheckGaugeResultIndicies(indicies, 1, DeviceIdentifier);

            return XResults[indicies[0]];
        }

        public List<double> GetYResultList(int[] indicies)
        {
            StandardGuardClauses.CheckGaugeResultIndicies(indicies, 1, DeviceIdentifier);

            return YResults[indicies[0]];
        }


        //public void Measure(double[] drawnOver)
        public void Measure(Func<List<string>, double[]> GetDrawnOver, GaugeMeasureInstantly MeasureCycle)
        {
            // Get drawnOver-values
            double[] drawnOver = GetDrawnOver(DrawnOverIdentifiers);

            // Read waveform
            var waveform = new DSOX2034TWaveformResult();
            List<double> xWaveVals = null;
            List<double> yWaveVals = null;
            if (InfoBlock.Common != null) // is null when ReadWaveform = 0
            {
                waveform = _device.GetWaveform(InfoBlock.TriggerOnWaveform);
                if (waveform.ErrorState == -1)
                    return;
                var waveStart = drawnOver[0];

                // Create waveform-results
                xWaveVals = new List<double>();
                yWaveVals = new List<double>(waveform.Values);
                for (int valueCount = yWaveVals.Count; valueCount > 0; valueCount--, waveStart += waveform.SampleInterval)
                    xWaveVals.Add(waveStart);
            }


            lock (XResults)
            {
                lock (YResults)
                {
                    if (InfoBlock.Common != null) // Is null when ReadWaveform = 0
                    {
                        _waveformIndicies.Add(XResults.Count);
                        XResults.Add(xWaveVals);
                        YResults.Add(yWaveVals);
                    }
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

            if (InfoBlock.Gauge.MeasureInstantly < 0) // No measurements
                return;

            SaveWaveform(folderPath, filePrefix);
        }



        private void SaveWaveform(string folderPath, string filePrefix = null)
        {
            // Guard clauses
            if (folderPath == null)
                throw new NullReferenceException("No savepath given");
            if (filePrefix == null)
                filePrefix = "";

            var deviceName = string.Format("{0}_{1}",
                                           InfoBlock.Common.DeviceIdentifier,
                                           (InfoBlock.Common.CustomName == null ? InfoBlock.Common.DeviceType : InfoBlock.Common.CustomName)
                                           );
            //var deviceName = string.Format("{0}_{1}", InfoBlock.Common.DeviceIdentifier, InfoBlock.Common.DeviceType);
            var output = new StringBuilder("Device: [" + deviceName + "]\n");
            output.AppendFormat("# DivX: {0}\n", InfoBlock.XDivScale);
            output.AppendFormat("# DivY: {0}\n", InfoBlock.Gauge.Range);
            output.AppendFormat("# Samplerate: {0}\n", _device.SampleRate);
            output.AppendFormat("# DatasetSize: {0}\n", XResults[0].Count);
            double x, y;
            output.AppendLine("# X, Y");

            for (var dataSetIndex = 0; dataSetIndex < _waveformIndicies.Count; dataSetIndex++)
            {
                for (var valueIndex = 0; valueIndex < XResults[_waveformIndicies[dataSetIndex]].Count; valueIndex++)
                {
                    x = XResults[_waveformIndicies[dataSetIndex]][valueIndex];
                    y = YResults[_waveformIndicies[dataSetIndex]][valueIndex];
                    output.AppendLine(string.Format("{0}, {1}", Convert.ToString(x), Convert.ToString(y)));
                }
            }
            var filename = folderPath + "\\" + filePrefix + deviceName + "_Waveform.dat";
            var fileWriter = new StreamWriter(filename, false);
            fileWriter.Write(output);
            fileWriter.Dispose();
            fileWriter = null;
            output = null;
        }



        public void ClearResults()
        {
            if (XResults != null)
            {
                foreach (var result in XResults)
                    result.Clear();
                XResults.Clear();
            }
            if (YResults != null)
            {
                foreach (var result in YResults)
                    result.Clear();
                YResults.Clear();
            }
            if (_waveformIndicies != null)
                _waveformIndicies.Clear();
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
            if (InfoBlock.Common != null)
            {
                var lastDataSetIndex = _waveformIndicies.Count - 1;
                if (lastDataSetIndex > 0)
                {
                    lastDataSetIndex = _waveformIndicies[lastDataSetIndex];
                    lock (XResults)
                    {
                        lock (YResults)
                        {
                            var x = XResults[lastDataSetIndex].ToArray();
                            var y = YResults[lastDataSetIndex].ToArray();
                            _chart.DataBindXY(_seriesNames[0], x, y);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
