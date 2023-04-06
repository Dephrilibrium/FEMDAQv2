using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using Instrument.KE6487;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Instrument.LogicalLayer
{
    public class KE6487Layer : InstrumentLogicalLayer
    {
        public InfoBlockKE6487 InfoBlock { get; private set; }
        private List<double> _sweep;
        private KE6487.KE6487 _device;
        private HaumChart.HaumChart _chart;
        private List<string> _seriesNames;



        public KE6487Layer(DeviceInfoStructure infoStructure, HaumChart.HaumChart chart)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockKE6487;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Wrong argument: {0} instead of KE6485", DeriveFromObject.DeriveNameFromStructure(infoStructure.InfoBlock)));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + "|" + (cName == null || cName == "" ? DeviceType : cName);

            XResults = new List<List<double>>();
            YResults = new List<List<double>>();

            _device = new KE6487.KE6487(InfoBlock.Gpib.GpibBoardNumber, (byte)InfoBlock.Gpib.GpibPrimaryAdress, (byte)InfoBlock.Gpib.GpibSecondaryAdress);
            if (_device == null) throw new NullReferenceException("KE6487 device couldn't be generated.");
            CommunicationPhy = InstrumentCommunicationPHY.GPIB;

            if(InfoBlock.Common.ChartDrawnOvers != null)
            {
                foreach (var drawnOver in InfoBlock.Common.ChartDrawnOvers)
                    XResults.Add(new List<double>());
            }

            if (InfoBlock.Common.ChartIdentifiers != null)
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

            YResults.Add(new List<double>());
        }


        public void Dispose()
        {
            if (_device != null)
                _device.Dispose();
            if (_chart != null)
                foreach (var seriesName in _seriesNames)
                    _chart.DeleteArea(seriesName);
        }



        #region Getter/Setter
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public InstrumentCommunicationPHY CommunicationPhy { get; private set; }
        public List<List<double>> XResults { get; set; }
        public List<List<double>> YResults { get; set; }
        public GaugeMeasureInstantly InstantMeasurement { get { return InfoBlock.Gauge.MeasureInstantly; } }
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
        //    StandardGuardClauses.CheckGaugeResultIndicies(indicies, 1, DeviceIdentifier); // Throws exceptions

        //    return XResults[indicies[0]];
        //}

        //public List<double> GetYResultList(int[] indicies)
        //{
        //    StandardGuardClauses.CheckGaugeResultIndicies(indicies, 1, DeviceIdentifier); // Throws exceptions

        //    return YResults[indicies[0]];
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
                    YResults[0].Add(current);
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
            output.AppendLine("# NPLC: " + InfoBlock.Gauge.Nplc.ToString());

            for (var line = 0; line < YResults[0].Count; line++)
            {
                for (var xRow = 0; xRow < XResults.Count; xRow++)
                    output.Append(Convert.ToString(XResults[xRow][line]) + ", ");
                output.AppendLine(Convert.ToString(YResults[0][line]));
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
                YResults[0].Clear();


            if (_chart != null)
                foreach (var seriesName in _seriesNames)
                    _chart.ClearXY(seriesName);
        }
        #endregion



        #region Source
        public double GetSourceValue(string identifier)
        {
            return _device.GetVoltage();
        }



        public void SetSourceValues(int sweepLine)
        {
            if (InfoBlock.Source.SourceNode < 0) // Unused source!
                return;
            var voltage = _sweep[sweepLine];
            _device.SetVoltage(voltage, VoltageRange.V500, InfoBlock.Source.Compliance, true);
        }



        public void PowerDownSource()
        {
            _device.SafeMode();
        }



        public void AssignSweepColumn(SweepContent sweep)
        {
            if (InfoBlock.Source.SourceNode < 0)
                return;

            var voltageSourceNode = "U" + Convert.ToString(InfoBlock.Source.SourceNode);
            _sweep = AssignSweep.Assign(sweep, voltageSourceNode);
            if (_sweep == null) throw new MissingFieldException("Can't find " + voltageSourceNode + " in sweep-file.");
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
                lastLine = YResults[0].Count - 1;
                if (lastLine < 0) // Actual no value measured
                    return;
                lastYVal = YResults[0][lastLine];
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
