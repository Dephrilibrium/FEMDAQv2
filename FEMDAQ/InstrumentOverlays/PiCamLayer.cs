using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using HaumOTH;
using Keithley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace Instrument.LogicalLayer
{
    public class PiCamLayer : InstrumentLogicalLayer
    {
        public InfoBlockPiCam InfoBlock { get; private set; }
        private PiCam _device;
        //private HaumChart.HaumChart _chart;
        //private List<string> _seriesNames;

        //private int _filenameCounter = 0;
        private string _shutterSpeeds = null;
        private int _measureCalls = -1;
        //private string TempDownloadDir = null;


        public PiCamLayer(DeviceInfoStructure infoStructure, HaumChart.HaumChart chart)
        {
            //_device.
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockPiCam;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Wrong argument: {0} instead of DMM7510", DeriveFromObject.DeriveNameFromStructure(infoStructure.InfoBlock)));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + "|" + (cName == null || cName == "" ? DeviceType : cName);

            //XResults = new List<double>();
            //YResults = new List<double>();

            try { _device = new PiCam(InfoBlock.Ip.IP, InfoBlock.Ip.Port, InfoBlock.Ip.Username, InfoBlock.Ip.Password, InfoBlock.PyCamScriptPath); }
            catch (Exception e) { throw new Exception("Can't create PiCam:\n\nAdditional Info:\n" + e.Message); }
            CommunicationPhy = InstrumentCommunicationPHY.Ethernet;

            if (InfoBlock.ShutterSpeeds.Length <= 0)
                throw new Exception("No shutterspeeds given!");

            _device.ConfShutterSpeed(InfoBlock.ShutterSpeeds[0]); // Shutterspeeds are sorted! Configurate shortest SS before starting
            _shutterSpeeds = string.Empty;
            foreach (var ss in InfoBlock.ShutterSpeeds)
                _shutterSpeeds += ss.ToString() + ":";
            _shutterSpeeds.Remove(_shutterSpeeds.Length - 1); // Remove tailing ':'

            // TempDownload gets a default value from InfoBlockPicam.cs
                if (!Directory.Exists(InfoBlock.TempDownloadDir))
                Directory.CreateDirectory(InfoBlock.TempDownloadDir);

            var fileList = Directory.GetFiles(InfoBlock.TempDownloadDir);
            foreach (var filepath in fileList)
                File.Delete(filepath);
        }



        public void Dispose()
        {
            if (_device != null)
                _device.Dispose();
        }



        #region Getter/Setter
        //public List<double> XResults { get; private set; }
        //public List<double> YResults { get; private set; }
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        public InstrumentCommunicationPHY CommunicationPhy { get; private set; }
        public GaugeMeasureInstantly InstantMeasurement { get { return InfoBlock.Gauge.MeasureInstantly; } }
        public List<string> DrawnOverIdentifiers { get { return InfoBlock.Common.ChartDrawnOvers; } }
        #endregion



        #region Common
        public void Init()
        {
            //var couplingAC = DMM7510.ConvertCoupling(InfoBlock.Coupling);
            //if (InfoBlock.MeasurementType == "VOLT")
            //{
            //    DMM7510_VoltageRange vRange = DMM7510.ConvertVoltageRange(InfoBlock.Gauge.Range);
            //    //_device.SetVoltageMeasurement(couplingAC, vRange, InfoBlock.Gauge.Nplc, InfoBlock.AutoZero);
            //}
            //else if (InfoBlock.MeasurementType == "CURR")
            //{
            //    DMM7510_CurrentRange cRange = DMM7510.ConvertCurrentRange(InfoBlock.Gauge.Range);
            //    //_device.SetCurrentMeasurement(couplingAC, cRange, InfoBlock.Gauge.Nplc, InfoBlock.AutoZero);
            //}
            //else throw new ArgumentOutOfRangeException("Unsupported measurementtype.");
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
            string tarGzName = String.Empty;

            if (_measureCalls < 0)
            {
                tarGzName = string.Format("{0}-BlackSubtraction",
                                          DeviceName.Replace('|', '_')
                                          //InfoBlock.Common.DeviceIdentifier,
                                          //(InfoBlock.Common.CustomName == null ? InfoBlock.Common.DeviceType : InfoBlock.Common.CustomName)
                                          );
            }
            else
            {
                if (MeasureCycle != InfoBlock.Gauge.MeasureInstantly)
                    return;

                tarGzName = string.Format("{0}-{1}",
                                          DeviceName.Replace('|', '_'),
                                          //InfoBlock.Common.DeviceIdentifier,
                                          //(InfoBlock.Common.CustomName == null ? InfoBlock.Common.DeviceType : InfoBlock.Common.CustomName),
                                          _measureCalls.ToString().PadLeft(4, '0')
                                          );
            }

            _device.TakePicSequence2Ramdisk(tarGzName, InfoBlock.ShutterSpeeds.Length, InfoBlock.ShutterSpeeds, 0);
            _measureCalls++;
            _device.ConfShutterSpeed(InfoBlock.ShutterSpeeds[0]); // Shutterspeeds are sorted

            _device.ReceiveAllRawAsTar(InfoBlock.TempDownloadDir, tarGzName); // Puth them temporary to a dummy-folder
        }



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

            var tarGzSrcPaths = Directory.GetFiles(InfoBlock.TempDownloadDir);
            foreach (string srcPath in tarGzSrcPaths)
            {
                //                                     deviceName is inserted during download -> Already has the correct filename
                var dstPath = folderPath + "\\" + filePrefix +/* deviceName + */Path.GetFileName(srcPath);
                File.Move(srcPath, dstPath);
            }

            _measureCalls = 0; // Reset for next run
        }



        public void ClearResults()
        {
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
        // Invoke this when using threads!
        public void UpdateGraph()
        {
        }
        #endregion
    }
}
