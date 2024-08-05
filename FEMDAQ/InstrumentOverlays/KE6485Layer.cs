using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


using Instrument.LogicalLayer.SubClasses;

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
            DeviceName = DeviceIdentifier + "|" + (cName == null || cName == "" ? DeviceType : cName);

            // nSubMeasurementsDone = 0; // Done multiple times during Measure()
            _subMeasTimer = new SubMeasurementTimer(InfoBlock.Gauge.deltatimeSubMeasurements);


            XResults = new List<List<double>>();
            YResults = new List<double>();

            _device = new KE6485.KE6485(InfoBlock.Gpib.GpibBoardNumber, (byte)InfoBlock.Gpib.GpibPrimaryAdress, (byte)InfoBlock.Gpib.GpibSecondaryAdress);
            if (_device == null) throw new NullReferenceException("KE6485 device couldn't be generated.");
            CommunicationPhy = InstrumentCommunicationPHY.GPIB;

            if (DrawnOverIdentifiers != null)
            {
                foreach (var drawnOver in DrawnOverIdentifiers)
                    XResults.Add(new List<double>());
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

            //YResults.Add(new List<double>()); // Be sure to have an y-vector when no drawnover is given
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
        public List<List<double>> XResults { get; private set; }
        public List<double> YResults { get; private set; }
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public InstrumentCommunicationPHY CommunicationPhy { get; private set; }
        public GaugeMeasureInstantly InstantMeasurement => InfoBlock.Gauge.MeasureInstantly;

        public int nSubMeasurements => InfoBlock.Gauge.nSubMeasurements;
        public int nSubMeasurementsDone { get; private set; }
        public int deltatimeSubMeasurements => InfoBlock.Gauge.deltatimeSubMeasurements; 
        private SubMeasurementTimer _subMeasTimer = null;
        private bool _subMeasTimerElapsed = false;

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
        //    return YResults;
        //}



        //public void Measure(double[] drawnOver)
        public void Measure(Func<List<string>, double[]> GetDrawnOver, GaugeMeasureInstantly MeasureCycle)
        {
            if (MeasureCycle != InfoBlock.Gauge.MeasureInstantly)
                return;

            double[] drawnOver = GetDrawnOver(DrawnOverIdentifiers);

            lock (XResults)
            {
                lock (YResults)
                {
                    var current = _device.GetCurrent();
                    YResults.Add(current);
                    for (var index = 0; index < DrawnOverIdentifiers.Count; index++)
                        XResults[index].Add(drawnOver[index]);
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

            for (var line = 0; line < YResults.Count; line++)
            {
                for (var xIndex = 0; xIndex < XResults.Count; xIndex++)
                    output.Append(XResults[xIndex][line] + ", ");
                output.AppendLine(YResults[line].ToString());
            }
            var filename = folderPath + "\\" + filePrefix + deviceName + ".dat";
            var fileWriter = new StreamWriter(filename, false);
            fileWriter.Write(output);
            fileWriter.Dispose();
        }



        public void ClearResults()
        {
            if (XResults != null)
                foreach (var xResult in XResults)
                    xResult.Clear();

            if (YResults != null)
                YResults.Clear();

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
        public void UpdateGraph()
        {
            if (_seriesNames.Count <= 0) // No drawdata
                return;

            int lastLine;
            double lastYVal;
            lock (YResults)
            {
                lastLine = YResults.Count - 1;
                if (lastLine < 0) // Actual is no value measured
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