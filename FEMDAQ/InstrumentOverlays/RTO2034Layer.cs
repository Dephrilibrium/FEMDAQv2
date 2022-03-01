using Files;
using Files.Parser;
using RohdeUndSchwarz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Instrument.LogicalLayer
{
    public class RTO2034Layer : InstrumentLogicalLayer
    {
        public InfoBlockRTO2034 InfoBlock { get; private set; }
        private RTO2034 _device;
        private List<string> _seriesNames;
        private HaumChart.HaumChart _chart;

        private int _setsPerMeasure = 0;
        private List<int> _waveformIndicies;
        private List<int> _fftIndicies;



        public RTO2034Layer(DeviceInfoStructure infoStructure, HaumChart.HaumChart chart)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockRTO2034;
            if (InfoBlock == null) throw new ArgumentException("InfoBlock-Convertion to InfoBlockRTO2034 failed.");

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.CommonWaveform.CustomName;
            DeviceName = DeviceIdentifier + "|" + (cName == null || cName == "" ? DeviceType : cName);

            DrawnOverIdentifiers = new List<string>();
            XResults=new List<List<double>>();
            YResults=new List<List<double>>();

            _seriesNames = new List<string>();
            if (InfoBlock.CommonWaveform != null) // is null when ReadWavefrom = 0
            {
                _setsPerMeasure += 2;
                _waveformIndicies = new List<int>();
                DrawnOverIdentifiers.Add("TIME");
                if (InfoBlock.CommonWaveform.ChartIdentifiers != null && InfoBlock.CommonWaveform.ChartIdentifiers[0] != "") // Add one chart!
                {
                    _seriesNames.Add(string.Format("{0}-Waveform",
                                                   DeviceName
                                                   ));
                    chart.AddSeries(InfoBlock.CommonWaveform.ChartIdentifiers[0], _seriesNames[_seriesNames.Count - 1], InfoBlock.CommonWaveform.ChartColors[0]);
                }
                _chart = chart;
            }

            if (InfoBlock.CommonFft != null) // is not null when 0 < UseFftOnMathWindow < 5
            {
                _setsPerMeasure += 2;
                _fftIndicies = new List<int>();
                DrawnOverIdentifiers.Add("F");
                if (InfoBlock.CommonFft.ChartIdentifiers != null && InfoBlock.CommonFft.ChartIdentifiers[0] != "") // Add one chart!
                {
                    _seriesNames.Add(string.Format("{0}|{1}-FFT",
                                                   InfoBlock.CommonFft.DeviceIdentifier,
                                                   (InfoBlock.CommonFft.CustomName == null ? InfoBlock.CommonFft.DeviceType : InfoBlock.CommonFft.CustomName) // Use customname when given, otherwise use devicetype as seriesname
                                                   ));
                    chart.AddSeries(InfoBlock.CommonFft.ChartIdentifiers[0], _seriesNames[_seriesNames.Count - 1], InfoBlock.CommonFft.ChartColors[0]);
                }
                _chart = chart;
            }

            _device = new RTO2034(InfoBlock.Ip.IP, InfoBlock.Channel);
            if (_device == null) throw new NullReferenceException("RTO2034 device couldn't be generated.");
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
        public List<int> WaveformIndicies { get; private set; }
        public List<int> FffIndicies { get; private set; }
        public List<List<double>> XResults { get; private set; }
        public List<List<double>> YResults { get; private set; }
        public GaugeMeasureInstantly InstantMeasurement { get { return InfoBlock.Gauge.MeasureInstantly; } }
        public List<string> DrawnOverIdentifiers { get; private set; }
        #endregion



        #region Common
        public void Init()
        {
            if (_fftIndicies != null)
                _device.FFTSetup(InfoBlock.MathWindow, InfoBlock.StartFrequency, InfoBlock.FrequencyResolution, InfoBlock.StopFrequency, InfoBlock.MagnitudeOffset, InfoBlock.MagnitudeRange, InfoBlock.WindowType);

            _device.SetYDivScale(InfoBlock.Gauge.Range);
            _device.SetXDivScale(InfoBlock.QuantPerDivX);
            _device.SetSamplingRate(InfoBlock.SampleRate);
            if(InfoBlock.TriggerOnWaveform) _device.TriggerSetup(InfoBlock.TriggerLevel, InfoBlock.TriggerSource, InfoBlock.TriggerMode, InfoBlock.TriggerType, InfoBlock.TriggerSlope);
            _device.DisplayUpdate(InfoBlock.UpdateDisplay);
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

            // Get drawnOver-values
            double[] drawnOver = GetDrawnOver(DrawnOverIdentifiers);

            // Read waveform
            var waveform = new RTO2034WaveformResult();
            List<double> xWaveVals = null;
            List<double> yWaveVals = null;
            if (InfoBlock.CommonWaveform != null) // is null when ReadWaveform = 0
            {
                waveform = _device.GetWaveform(InfoBlock.TriggerOnWaveform);
                if (waveform.ErrorState == -1)
                    return;
                var waveDiff = waveform.TotalTime.TotalSeconds / waveform.Values.Length;
                var waveStart = drawnOver[0];

                // Create waveform-results
                xWaveVals = new List<double>();
                yWaveVals = new List<double>(waveform.Values);
                for (int valueCount = yWaveVals.Count; valueCount > 0; valueCount--, waveStart += waveDiff)
                    xWaveVals.Add(waveStart);
            }


            var fft = new RTO2034FFTResult();
            List<double> xFftVals = null;
            List<double> yFftVals = null;
            // Read fft
            if (_fftIndicies != null) // is not null when 0 < UseFftOnMathWindow < 5
            {
                fft = _device.GetFFT(InfoBlock.TriggerOnFFT);
                var fftDiff = fft.FrequencySpan / fft.Values.Length;
                var fftStart = fft.StartFrequency;

                // Create fft-results
                xFftVals = new List<double>();
                yFftVals = new List<double>(fft.Values);
                for (int valueCount = yFftVals.Count; valueCount > 0; valueCount--, fftStart += fftDiff)
                    xFftVals.Add(fftStart);
            }

            lock (XResults)
            {
                lock (YResults)
                {
                    if (InfoBlock.CommonWaveform != null) // Is null when ReadWaveform = 0
                    {
                        _waveformIndicies.Add(XResults.Count);
                        XResults.Add(xWaveVals);
                        YResults.Add(yWaveVals);
                    }

                    if(InfoBlock.CommonFft != null)
                    {
                        _fftIndicies.Add(XResults.Count);
                        XResults.Add(xFftVals);
                        YResults.Add(yFftVals);
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

            if (InfoBlock.Gauge.MeasureInstantly < 0) // No measurements
                return;

            SaveWaveform(folderPath, filePrefix);
            if (InfoBlock.CommonFft != null) // Is not null when 0 < UseFftOnMathWindow < 5
                SaveFFT(folderPath, filePrefix);
        }



        private void SaveWaveform(string folderPath, string filePrefix)
        {
            var deviceName = string.Format("{0}_{1}",
                                           InfoBlock.CommonWaveform.DeviceIdentifier,
                                           (InfoBlock.CommonWaveform.CustomName == null ? InfoBlock.CommonWaveform.DeviceType : InfoBlock.CommonWaveform.CustomName)
                                           );
            var output = new StringBuilder("Device: [" + deviceName + "]\n");
            output.AppendFormat("# DivX: {0}\n", InfoBlock.QuantPerDivX);
            output.AppendFormat("# DivY: {0}\n", InfoBlock.Gauge.Range);
            output.AppendFormat("# Samplerate: {0}\n", InfoBlock.SampleRate);
            var recLenDataset = (int)(10 * InfoBlock.QuantPerDivX * InfoBlock.SampleRate);
            output.AppendFormat("# DatasetSize: {0}\n", recLenDataset);
            double x, y;
            output.AppendLine("# X, Y");

            // Write header (create new file)
            var filename = folderPath + "\\" + filePrefix + deviceName + "_Waveform.dat";
            var fileWriter = new StreamWriter(filename, false);
            fileWriter.Write(output);
            fileWriter.Close();
            fileWriter.Dispose();

            for (var dataSetIndex = 0; dataSetIndex < _waveformIndicies.Count; dataSetIndex++)
            {
                output = new StringBuilder();
                for (var valueIndex = 0; valueIndex < recLenDataset; valueIndex++)
                {
                    x = XResults[_waveformIndicies[dataSetIndex]][valueIndex];
                    y = YResults[_waveformIndicies[dataSetIndex]][valueIndex];
                    output.AppendLine(string.Format("{0}, {1}", Convert.ToString(x), Convert.ToString(y)));
                }
                fileWriter = new StreamWriter(filename, true);
                fileWriter.Write(output);
                fileWriter.Close();
                fileWriter.Dispose();
            }

            fileWriter.Dispose();
            fileWriter = null;
            output = null;
        }



        private void SaveFFT(string folderPath, string filePrefix)
        {
            var deviceName = string.Format("{0}_{1}",
                               InfoBlock.CommonFft.DeviceIdentifier,
                               (InfoBlock.CommonFft.CustomName == null ? InfoBlock.CommonFft.DeviceType : InfoBlock.CommonFft.CustomName)
                               );
            var output = new StringBuilder("Device: [" + deviceName + "]\n");
            output.AppendFormat("# Startfrequency: {0}\n", InfoBlock.StartFrequency);
            output.AppendFormat("# Stopfrequency: {0}\n", InfoBlock.StopFrequency);
            output.AppendFormat("# Frequencyresolution: {0}\n", InfoBlock.FrequencyResolution);
            output.AppendFormat("# DatasetSize: {0}\n", XResults[2].Count);
            double x, y;
            output.AppendLine("# X, Y");

            for (var dataSetIndex = 0; dataSetIndex < _fftIndicies.Count; dataSetIndex++)
            {
                for (var valueIndex = 0; valueIndex < XResults[_fftIndicies[dataSetIndex]].Count; valueIndex++)
                {
                    x = XResults[_fftIndicies[dataSetIndex]][valueIndex];
                    y = YResults[_fftIndicies[dataSetIndex]][valueIndex];
                    output.AppendLine(string.Format("{0}, {1}", Convert.ToString(x), Convert.ToString(y)));
                }
            }
            var filename = folderPath + "\\" + filePrefix + deviceName + "_FFT.dat";
            var fileWriter = new StreamWriter(filename, false);
            fileWriter.Write(output);
            fileWriter.Dispose();
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
            if (_fftIndicies != null)
                _fftIndicies.Clear();
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
            if (InfoBlock.CommonWaveform != null && InfoBlock.CommonWaveform.ChartIdentifiers.Count > 1)
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

            if (_fftIndicies != null && InfoBlock.CommonFft.ChartIdentifiers.Count > 0) // Is not null when 0 < UseFftOnMathWindow < 5
            {
                var lastDataSetIndex = _fftIndicies.Count - 1;
                if (lastDataSetIndex > 0)
                {
                    lastDataSetIndex = _fftIndicies[lastDataSetIndex];
                    lock (XResults)
                    {
                        lock (YResults)
                        {
                            var x = XResults[lastDataSetIndex];
                            var y = YResults[lastDataSetIndex];
                            _chart.AddXYSet(_seriesNames[_seriesNames.Count - 1], x, y);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
