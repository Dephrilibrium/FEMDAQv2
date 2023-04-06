using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using Instrument.HP4145B;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Instrument.LogicalLayer
{
    class HP4145BLayer : InstrumentLogicalLayer
    {
        public InfoBlockHP4145B InfoBlock { get; private set; }
        private List<double> _sweep;
        private HP4145B.HP4145B _device;
        private HaumChart.HaumChart _chart;
        private List<string> _seriesNames;

        public HP4145BLayer(DeviceInfoStructure infoStructure, HaumChart.HaumChart chart)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockHP4145B;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Wrong argument: {0} instead of KE6485", DeriveFromObject.DeriveNameFromStructure(infoStructure.InfoBlock)));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + "|" + (cName == null || cName == "" ? DeviceType : cName);

            XResults =new List<List<double>>();
            YResults =new List<double>();

            _device = new HP4145B.HP4145B(InfoBlock.Gpib.GpibBoardNumber, (byte)InfoBlock.Gpib.GpibPrimaryAdress, (byte)InfoBlock.Gpib.GpibSecondaryAdress);
            if (_device == null) throw new NullReferenceException("HP4145B device couldn't be generated.");
            CommunicationPhy = InstrumentCommunicationPHY.GPIB;

            if (DrawnOverIdentifiers != null)
            {
                foreach (var drawnOver in DrawnOverIdentifiers)
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

            //YResults.Add(new List<double>());
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
        public InstrumentCommunicationPHY CommunicationPhy { get; private set; }
        public List<List<double>> XResults { get; private set; }
        public List<double> YResults { get; private set; }
        public GaugeMeasureInstantly InstantMeasurement { get { return InfoBlock.Gauge.MeasureInstantly; } }
        public List<string> DrawnOverIdentifiers { get { return InfoBlock.Common.ChartDrawnOvers; } }
        #endregion



        #region Common
        public void Init()
        {
            _device.SetParameter(InfoBlock.IntegrationTime, AutoCalibration.On);
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
                    var value = _device.GetChannel(InfoBlock.MeasureMode, InfoBlock.SMUChannel);
                    YResults.Add(value);
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

            if (InfoBlock.Gauge.MeasureInstantly < 0) // Hadn't done measurements
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

            for (var line = 0; line < YResults.Count; line++)
            {
                for (var xRow = 0; xRow < XResults.Count; xRow++)
                    output.Append(Convert.ToString(XResults[xRow][line]) + ", ");
                output.AppendLine(Convert.ToString(YResults[line]));
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
            return _device.GetChannel(InfoBlock.SourceMode, InfoBlock.SMUChannel);
        }



        public void SetSourceValues(int sweepLine)
        {
            if (InfoBlock.Source.SourceNode < 0)
                return;
            var value = _sweep[sweepLine];
            if (InfoBlock.SourceMode == Mode.Voltage)
                _device.SetChannel(InfoBlock.SourceMode, InfoBlock.SMUChannel, value, InfoBlock.Compliance);
            else
                _device.SetChannel(InfoBlock.SourceMode, InfoBlock.SMUChannel, InfoBlock.Compliance, value);
        }



        public void PowerDownSource()
        {
            _device.SafeMode();
        }



        public void AssignSweepColumn(SweepContent sweep)
        {
            if (InfoBlock.Source.SourceNode < 0)
                return;

            var valueSourceNode = InfoBlock.SourceMode == Mode.Voltage ? "U" : "I";
            var nameOfSourceNode = valueSourceNode + Convert.ToString(InfoBlock.Source.SourceNode);
            _sweep = AssignSweep.Assign(sweep, nameOfSourceNode);
            if (_sweep == null) throw new MissingFieldException("Can't find " + nameOfSourceNode + " in sweep-file.");
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
