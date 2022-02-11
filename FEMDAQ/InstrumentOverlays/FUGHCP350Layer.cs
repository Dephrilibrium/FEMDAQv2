using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Instrument.LogicalLayer
{
    public class FUGHCP350Layer : InstrumentLogicalLayer
    {
        public InfoBlockFUGHCP350 InfoBlock { get; private set; }
        private List<double> _sweep;
        private FUGHCP350.FUGHCP350 _device;



        public FUGHCP350Layer(DeviceInfoStructure infoStructure)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockFUGHCP350;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Cast failed: infoBlock -> FUGHCP350InfoBlock"));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + (cName == null || cName == "" ? DeviceType : cName);

            _device = new FUGHCP350.FUGHCP350(InfoBlock.Gpib.GpibBoardNumber, (byte)InfoBlock.Gpib.GpibPrimaryAdress, (byte)InfoBlock.Gpib.GpibSecondaryAdress);
            if (_device == null) throw new NullReferenceException("FUGHCP350 device couldn't be generated.");
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
        public List<List<double>> xResults { get { return null; } }
        public List<List<double>> yResults { get { return null; } }
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
            _device.SetCurrent(InfoBlock.Source.Compliance);
            _device.SetOutput(true);
        }
        #endregion



        #region Gauge | collects the voltages to each iterate!
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
            return _device.GetVoltage();
        }



        public void SetSourceValues(int sweepLine)
        {
            var voltage = _sweep[sweepLine];
            _device.SetVoltage(voltage);
            //_device.SetOutput(true); // Output now enabled in Init() instead of each time a value is sent
        }



        public void PowerDownSource()
        {
            _device.SetOutput(false);
            _device.SetVoltage(0);
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
