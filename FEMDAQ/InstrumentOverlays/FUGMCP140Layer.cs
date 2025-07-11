﻿using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Instrument.LogicalLayer
{
    public class FUGMCP140Layer : InstrumentLogicalLayer
    {
        public InfoBlockFUGMCP140 InfoBlock { get; private set; }
        private List<double> _sweep;
        private FUGMCP140.MCP140 _device;



        public FUGMCP140Layer(DeviceInfoStructure infoStructure)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockFUGMCP140;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Cast failed: infoBlock -> FUGMCP140InfoBlock"));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + (cName == null || cName == "" ? DeviceType : cName);

            _device = new FUGMCP140.MCP140(InfoBlock.Gpib.GpibBoardNumber, (byte)InfoBlock.Gpib.GpibPrimaryAdress, (byte)InfoBlock.Gpib.GpibSecondaryAdress);
            if (_device == null) throw new NullReferenceException("FUGMCP140 device couldn't be generated.");
            CommunicationPhy = InstrumentCommunicationPHY.GPIB;
        }



        public void Dispose()
        {
            if (_device != null)
            {
                PowerDownSource();
                _device.Dispose();
            }

            InfoBlock.Dispose();
        }



        #region Getter/Setter
        //public List<List<List<double>>> xResults { get { return null; } }
        //public List<List<List<double>>> yResults { get { return null; } }
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public InstrumentCommunicationPHY CommunicationPhy { get; private set; }
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


        public void DoBeforeStart()
        {
        }

        public void DoAfterFinished()
        {
        }
        #endregion



        #region Gauge | collects the voltages to each iterate!
        //public List<double> GetXResultList(int[] indicies) { return null; }
        //public List<double> GetYResultList(int[] indicies) { return null; }


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

            // MCP140 can't work with negative voltages -> Check if there are some negative values within sweep-values
            int numberOfNegativeVoltages = 0;
            foreach (double voltage in _sweep)
            {
                if (voltage < 0)
                    numberOfNegativeVoltages++;
            }

            if (numberOfNegativeVoltages > 0)
                throw new ArgumentOutOfRangeException("MCP140 can't handle negative voltages (found " + numberOfNegativeVoltages.ToString() + " in " + voltageSourceNode + " sweep values)");

        }
        #endregion



        #region UI
        public void UpdateGraph()
        {
        }
        #endregion
    }
}
