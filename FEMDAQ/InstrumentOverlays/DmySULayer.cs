using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Instrument.LogicalLayer
{
    public class DmySULayer : InstrumentLogicalLayer
    {
        public InfoBlockDmySU InfoBlock { get; private set; }
        private List<double> _sweep;
        private DummyDevices.DmySU _device;



        public DmySULayer(DeviceInfoStructure infoStructure)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockDmySU;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Cast failed: infoBlock -> FUGHCP350InfoBlock"));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + (cName == null || cName == "" ? DeviceType : cName);

            _device = new DummyDevices.DmySU();
            if (_device == null) throw new NullReferenceException("DSU device couldn't be generated.");
        }



        public void Dispose()
        {
            if (_device != null)
            {
                PowerDownSource();
                _device.Dispose();
            }
        }



        #region Getter/Setter
        //public List<List<List<double>>> xResults { get { return null; } }
        //public List<List<List<double>>> yResults { get { return null; } }
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public GaugeMeasureInstantly InstantMeasurement { get { return GaugeMeasureInstantly.CycleEnd; } }
        public List<string> DrawnOverIdentifiers { get { return null; } }
        #endregion



        #region Common
        public void Init()
        {
            _device.Init();
        }
        #endregion



        #region Gauge | collects the voltages to each iterate!
        public List<double> GetXResultList(int[] indicies) { return null; }
        public List<double> GetYResultList(int[] indicies){return null;}


        //public void Measure(double[] drawnOver)
        public void Measure(Func<List<string>, double[]> GetDrawnOver, GaugeMeasureInstantly MeasureCycle)
        {
        }



        //public void SaveResultsToFolder(string folderPath)
        //{
        //}
        public void SaveResultsToFolder(string folderPath, string filePrefix)
        {
        }



        public void ClearResults()
        {
        }
        #endregion



        #region Source
        public double GetSourceValue(string identifier)
        {
            return _device.GetOutputValue();
        }



        public void SetSourceValues(int sweepLine)
        {
            var voltage = _sweep[sweepLine];
            _device.SetOutputValue(voltage);
        }



        public void PowerDownSource()
        {
            _device.SetOutputValue(0);
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
        }
        #endregion
    }
}
