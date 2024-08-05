using System;
using System.Collections.Generic;
using System.Text;
using Files;
using Files.Parser;
using System.IO;
using Leybold;
using System.IO.Ports;

namespace Instrument.LogicalLayer
{



    class MOVE1250Layer : InstrumentLogicalLayer
    {
        public InfoBlockMOVE1250 InfoBlock { get; private set; }
        private List<double> _sweep;
        private MOVE1250 _device;



        public MOVE1250Layer(DeviceInfoStructure infoStructure)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockMOVE1250;
            if (InfoBlock == null) throw new InvalidCastException("Cast failed: infoBlock -> InfoBlockMOVE1250");

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + (cName == null || cName == "" ? DeviceType : cName);

            _device = new MOVE1250(InfoBlock.ComPort.ComPort, 300, 7, StopBits.Two, Parity.None, 2000, InfoBlock.ResponseTime);
            if (_device == null) throw new NullReferenceException("MOVE1250 device couldn't be generated.");
            CommunicationPhy = InstrumentCommunicationPHY.COMPort;
        }



        public void Dispose()
        {
            if (_device != null)
                _device.Dispose();

            InfoBlock.Dispose();
        }


        #region Getter/Setter
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public InstrumentCommunicationPHY CommunicationPhy { get; private set; }
        public List<string> DrawnOverIdentifiers { get { return null; } }
        public GaugeMeasureInstantly InstantMeasurement { get { return GaugeMeasureInstantly.CycleEnd; } }
        //public List<List<List<double>>> xResults { get { return null; } }
        //public List<List<List<double>>> yResults { get { return null; } }
        #endregion



        #region Common
        public void Init()
        {
            _device.OpenConnection();
        }

        public void DoBeforeStart()
        {
        }

        public void DoAfterFinished()
        {
        }
        #endregion



        #region Gauge
        //public List<double> GetXResultList(int[] indicies)        {            return null;        }

        //public List<double> GetYResultList(int[] indicies)        {            return null;        }

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
            return _device.GetPositionAbsOffsetCorrected();
        }



        public void SetSourceValues(int sweepLine)
        {
            var position = (int)_sweep[sweepLine];
            _device.SetPositionAbsOffsetcorrected(position);
        }



        public void PowerDownSource()
        {
            _device.CloseImmediately();
        }



        public void AssignSweepColumn(SweepContent sweep)
        {
            if (InfoBlock.Source.SourceNode < 0)
                return;

            var positionSourceNode = "P" + Convert.ToString(InfoBlock.Source.SourceNode);
            _sweep = AssignSweep.Assign(sweep, positionSourceNode);
            if (_sweep == null)
            {
                var cName = InfoBlock.Common.CustomName;
                var devName = InfoBlock.Common.DeviceIdentifier + (cName == null || cName == "" ? InfoBlock.Common.DeviceType : cName);
                throw new MissingFieldException(string.Format("Couldn't find sweep-column for " + devName));
            }
        }
        #endregion


        
        #region UI
        public void UpdateGraph()
        {
        }
        #endregion
    }
}
