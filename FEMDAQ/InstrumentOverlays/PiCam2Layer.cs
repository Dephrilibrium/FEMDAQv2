using FEMDAQ.StaticHelper;
using FEMDAQ.Instrumentwindows;
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
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FEMDAQ.Instrumentwindows.PiCam2Statuswindow;
using FEMDAQ;

namespace Instrument.LogicalLayer
{
    public class PiCam2Layer : InstrumentLogicalLayer
    {
        public InfoBlockPiCam2 InfoBlock { get; private set; }
        private PiCam2 _device;
        //private HaumChart.HaumChart _chart;
        //private List<string> _seriesNames;

        //private int _filenameCounter = 0;
        private string _shutterSpeeds = null;
        private int _measureCalls = -1;

        private PyCam2Statuswindow _statWin = null;
        public int _startedDownloads { get; private set; }
        public int _activeDownloads { get; private set; }
        //private string TempDownloadDir = null;


        //// Log-Vars
        //private int _padding = 45;

        public PiCam2Layer(DeviceInfoStructure infoStructure, HaumChart.HaumChart chart)
        {
            //_device.
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockPiCam2;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Wrong argument: {0} instead of DMM7510", DeriveFromObject.DeriveNameFromStructure(infoStructure.InfoBlock)));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + "|" + (cName == null || cName == "" ? DeviceType : cName);

            //XResults = new List<double>();
            //YResults = new List<double>();

            // Open the status window
            try
            {
                _statWin = new PyCam2Statuswindow(this);
                _statWin.LPad = 22;
                _statWin.Owner = GlobalVariables.MainFrame;
                _statWin.Show();
                _statWin.OverrideLog("!!! ATTENTION !!!\nThis log is only for your information and therefore NOT saved.\nIf you want to have a full and detailed log, use the PiCam2's logger functionality!\n\n\n"
                                     , false);
                _statWin.Append2Log("Creating PyCam2 instance.");


                var exptimes = "";
                if (InfoBlock.ShutterSpeedsColumn > 0) // Prepare presetting first ET
                {
                    exptimes = string.Format("{0}", 1000); // Static 1000µs, since _sweep containing the SWP-Columns is not known yet!
                }
                else // Build static parameter list
                {
                    for (int _i = 0; _i < InfoBlock.ShutterSpeeds.Length; _i++)
                        exptimes += string.Format("{0},", InfoBlock.ShutterSpeeds[_i]);
                    exptimes = exptimes.Remove(exptimes.Length - 1);
                }

                _statWin.Append2Log(string.Format("Trying to connect to: {0}:{1}", InfoBlock.Ip.IP, InfoBlock.Ip.Port));
                _statWin.Append2Log(string.Format("using:"));
                _statWin.Append2Log(string.Format("- User: {0}", InfoBlock.Ip.Username));
                _statWin.Append2Log(string.Format("- Password: {0}", InfoBlock.Ip.Password));
                _statWin.Append2Log(string.Format("- FPS: {0}", InfoBlock.FrameRate));
                if (InfoBlock.BayerClipWin.Length == 2)
                    _statWin.Append2Log(string.Format("- Window (w,h): {0},{1} around center", InfoBlock.BayerClipWin[0], InfoBlock.BayerClipWin[1]));
                else
                    _statWin.Append2Log(string.Format("- Window (x,y,w,h): {0},{1},{2},{3}", InfoBlock.BayerClipWin[0], InfoBlock.BayerClipWin[1], InfoBlock.BayerClipWin[2], InfoBlock.BayerClipWin[3]));
                if (InfoBlock.ShutterSpeedsColumn > 0)
                    _statWin.Append2Log(string.Format("- Variable ExposureTimes of SWP-Column: ET{0} (presetting ET=1ms)", InfoBlock.ShutterSpeedsColumn));
                else
                    _statWin.Append2Log(string.Format("- Static ExposureTimes: {0}", exptimes));
                _statWin.Append2Log(string.Format("- PicsPerET: {0}", InfoBlock.PicsPerShutterSpeed));

                try
                { _device = new PiCam2(InfoBlock.Ip.IP, InfoBlock.Ip.Port, InfoBlock.Ip.Username, InfoBlock.Ip.Password, InfoBlock.PyCamScriptPath); }
                catch (Exception e) { throw new Exception("Can't create PiCam:\n\nAdditional Info:\n" + e.Message); }

                CommunicationPhy = InstrumentCommunicationPHY.Ethernet;


                if (InfoBlock.ShutterSpeedsColumn < 0 && InfoBlock.ShutterSpeeds != null && InfoBlock.ShutterSpeeds.Length <= 0)
                    throw new Exception("No shutterspeeds given!");

                if(InfoBlock.ShutterSpeedsColumn >= 0)
                    InfoBlock.OverrideShutterSpeeds(new uint[] { 1000 }); // Give Shutterspeeds a value


                // Setup of cam
                //_statWin.Append2Log("Configuring PyCam2");
                //_device.ConfExposureMode(InfoBlock.ExposureMode);
                //_device.ConfFrameRate(InfoBlock.FrameRate);
                _device.ConfShutterSpeed(InfoBlock.ShutterSpeeds[0]); // SS sorted ascending
                _shutterSpeeds = string.Empty;
                foreach (var ss in InfoBlock.ShutterSpeeds)
                    _shutterSpeeds += ss.ToString() + ":";
                _shutterSpeeds.Remove(_shutterSpeeds.Length - 1); // Remove tailing ':'

                _statWin.Append2Log(string.Format("Configuring analogue gain to: {0:.##}", InfoBlock.AnalogGain));
                _device.ConfAnalogGain(InfoBlock.AnalogGain);                               // Adjust static analogue gain
                if (InfoBlock.AwbGainRBalance > 0 && InfoBlock.AwbGainBBalance > 0)
                {
                    _statWin.Append2Log(string.Format("Configuring AutoWhiteBalance to: {0:.##}, {1:.##}", InfoBlock.AwbGainRBalance, InfoBlock.AwbGainBBalance));
                    _device.ConfAwbGains(InfoBlock.AwbGainRBalance, InfoBlock.AwbGainBBalance); // Adjust static auto-white-balance if valid inputs were given
                }
                _statWin.Append2Log(string.Format("Using the server's default AutoWhiteBalance (depending on config-file!)."));


                //_device.ConfScalerCrop(InfoBlock.ScalerCrop[0], InfoBlock.ScalerCrop[1], InfoBlock.ScalerCrop[2], InfoBlock.ScalerCrop[3]);
                if (InfoBlock.BayerClipWin.Length == 4)
                    _device.ServerBayerClipSize(InfoBlock.BayerClipWin[0], InfoBlock.BayerClipWin[1], InfoBlock.BayerClipWin[2], InfoBlock.BayerClipWin[3]);
                else
                    _device.ServerBayerClipSize(InfoBlock.BayerClipWin[0], InfoBlock.BayerClipWin[1]);
                _device.ServerDeBayerClippedBayer(InfoBlock.DeBayerClipWindow);
                _device.ServerShrinkHalfDebayeredImageIterations(InfoBlock.ShrinkDebayeredByBinPow);

                // TempDownload gets a default value from InfoBlockPicam.cs
                if (!Directory.Exists(InfoBlock.TempDownloadDir))
                    Directory.CreateDirectory(InfoBlock.TempDownloadDir);

                _statWin.Append2Log("Temp. download folder is " + InfoBlock.TempDownloadDir);

                // Is done before measurement-start!
                //ResetMeasureCalls();
                // ClearTempFolder();

                //_statWin.Append2Log(string.Format("Waiting an extra second because of RPC_E_DISCONNECTED exception..."));
                //Thread.Sleep(5000);
                //_statWin.Append2Log(string.Format("Ready for start"));
            }
            catch (Exception e)
            {
                Dispose();
                throw e;
            }
        }



        public void Dispose()
        {
            if (_device != null)
                _device.Dispose();

            if (_statWin != null)
            {
                _statWin.Close();
                _statWin.Dispose();
            }

            InfoBlock.Dispose();
        }

        public void ClearTempFolder()
        {
            var fileList = Directory.GetFiles(InfoBlock.TempDownloadDir);

            if (fileList.Length > 0)
                _statWin.Append2Log("Deleting contents of temp. download folder.");

            foreach (var filepath in fileList)
                File.Delete(filepath);
        }

        public void ResetMeasureCalls()
        {
            _measureCalls = -1;
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
            // Is done before measurement-start!
            //ResetMeasureCalls();
            //ClearTempFolder();

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

        public void DoBeforeStart()
        {
            _statWin.Append2Log("Resetting internal measurement status variables.");
            ResetMeasureCalls();
            ClearTempFolder();
            _startedDownloads = 0;
            _activeDownloads = 0;

            if (InfoBlock.ShutterSpeedsColumn >= 0 && InfoBlock.ShutterSpeeds != null)                      // If Shutterspeeds are variables, preset the first SS
                _device.ConfShutterSpeed(InfoBlock.ShutterSpeeds[0]);
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
            string tarGzName = String.Empty;

            if (_measureCalls < 0)
            {
                tarGzName = string.Format("{0}-BlackSubtraction",
                                          DeviceName.Replace('|', '_')
                                          //InfoBlock.Common.DeviceIdentifier,
                                          //(InfoBlock.Common.CustomName == null ? InfoBlock.Common.DeviceType : InfoBlock.Common.CustomName)
                                          );
                _statWin.Invoke(new Action(() => _statWin.Append2Log(string.Format("Requesting black image set."))));
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

                _statWin.Invoke(new Action(() => _statWin.Append2Log(string.Format("Requesting image set {0}.", _measureCalls.ToString()))));
            }


            //_device.TakePicSequence2Ramdisk(tarGzName, InfoBlock.PicsPerShutterSpeed, InfoBlock.ShutterSpeeds, InfoBlock.PictureInterval, InfoBlock.Bayer);

            if (InfoBlock.ShutterSpeedsColumn < 0 || _measureCalls < 0)
            {
                List<uint> _blackETs = new List<uint>();
                foreach (uint _bET in InfoBlock.ShutterSpeeds) // Avoid making multiple black-image sets of the same SS
                    if(!_blackETs.Contains(_bET))
                        _blackETs.Add(_bET);

                _device.CaptureShutterSpeedSequence(tarGzName, InfoBlock.PicsPerShutterSpeed, _blackETs.ToArray(), 0.2, InfoBlock.SaveSSLog);
            }
            else
            {
                int _iSSVar = ((_measureCalls) % InfoBlock.ShutterSpeeds.Length);
                _device.CaptureShutterSpeedSequence(tarGzName, InfoBlock.PicsPerShutterSpeed, new uint[] { InfoBlock.ShutterSpeeds[_iSSVar] }, 0.2, InfoBlock.SaveSSLog);
            }

            _measureCalls++;

            if (InfoBlock.ShutterSpeedsColumn >= 0) // If Shutterspeeds is not a parameter -> Preset the next SS
            {
                int _ssIndex = (_measureCalls) % InfoBlock.ShutterSpeeds.Length;
                _device.ConfShutterSpeed(InfoBlock.ShutterSpeeds[_ssIndex]);
            }
            //else // This is already done by the server!
            //{
            //    _device.ConfShutterSpeed(InfoBlock.ShutterSpeeds[0]); 
            //}


            var srcPath = _device.Raw2Archive(tarGzName,
                                              InfoBlock.Compress2TarGz,
                                              InfoBlock.CompressMulticore,
                                              InfoBlock.CompressSuppressParents);
            var dstPath = string.Format("{0}", Path.Combine(InfoBlock.TempDownloadDir, Path.GetFileName(srcPath))); // Append .gz if compression is enabled



            ThreadPool.QueueUserWorkItem((state) =>
            {

                string imgSetName = (_startedDownloads - 1 < 0) ? "black" : (_startedDownloads - 1).ToString();
                _statWin.Invoke(new Action(() => _statWin.Append2Log(string.Format("Downloading image set " + imgSetName))));

                _startedDownloads++;
                _activeDownloads++; // Add during download

                _device.DownloadFile(srcPath, dstPath);
                _device.ClearFile(srcPath);              // Remove archive

                //_device.ReceiveAllRawAsArchive(InfoBlock.TempDownloadDir, 
                //                                tarGzName, 
                //                                InfoBlock.Compress2TarGz, 
                //                                InfoBlock.CompressMulticore, 
                //                                InfoBlock.CompressSuppressParents); // Puth them temporary to a dummy-folder
                _activeDownloads--; // Remove after download

                _statWin.Invoke(new Action(() => _statWin.Append2Log(string.Format("Finished image set " + imgSetName))));
                if (_activeDownloads == 0)
                    _statWin.Invoke(new Action(() => _statWin.UpdateActiveDownloadsToolstrip()));
            });
            //_device.ReceiveAllRawAsArchive(InfoBlock.TempDownloadDir, tarGzName, InfoBlock.Compress2TarGz, InfoBlock.CompressMulticore, InfoBlock.CompressSuppressParents); // Puth them temporary to a dummy-folder

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

            while (_activeDownloads > 0)
            {
                Thread.Sleep(100); // Wait 100ms and check again!
            }

            var deviceName = string.Format("{0}_{1}",
                                           InfoBlock.Common.DeviceIdentifier,
                                           (InfoBlock.Common.CustomName == null ? InfoBlock.Common.DeviceType : InfoBlock.Common.CustomName)
                                           );

            // Download the log-file
            var srcFullFilename = _device.PyLogPath;
            var srcFilename = Path.GetFileName(srcFullFilename);
            var dstFullFilename = Path.Combine(InfoBlock.TempDownloadDir, deviceName + ".log");
            //var dstFullFilename = Path.Combine(InfoBlock.TempDownloadDir, srcFilename); // Old Filename - Kept that one from picam-server
            try
            {
                string logContent = _device.CatFile(srcFullFilename);
                var logFileWriter = new StreamWriter(dstFullFilename);
                logFileWriter.Write(logContent);
                logFileWriter.Close();
                //_device.DownloadFile(srcFullFilename, dstFullFilename); // Didn't worked. Maybe rightproblems on logfile, so that download fails
            }
            catch (Exception) { }

            // Grab all downloaded filenames and copy to save-folder
            var tarGzSrcPaths = Directory.GetFiles(InfoBlock.TempDownloadDir);

            string failedSrcDir = null;
            foreach (string srcPath in tarGzSrcPaths)
            {
                try
                {
                    var dstPath = folderPath + "\\" + filePrefix +/* deviceName + */Path.GetFileName(srcPath);
                    File.Move(srcPath, dstPath);
                }
                catch (Exception)
                {
                    if (failedSrcDir == null)
                        failedSrcDir = folderPath;
                }
                //                                     deviceName is inserted during download -> Already has the correct filename
            }

            // Is now done before measurement-start!
            //ResetMeasureCalls(); // Reset for next run

            if (failedSrcDir != null)
            {
                var msgText = "Wasn't able to move files in: " + failedSrcDir + "\n\nCheck directory for files and copy them manually!";
                MessageBox.Show(msgText, "Warning", MessageBoxButtons.OK);
            }

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
            if (InfoBlock.ShutterSpeedsColumn < 0)
                return;

            var etSwpColumn = "ET" + Convert.ToString(InfoBlock.ShutterSpeedsColumn);
            List<double> swp;
            swp = AssignSweep.Assign(sweep, etSwpColumn);
            if (swp == null) throw new MissingFieldException("Can't find " + etSwpColumn + " in sweep-file.");

            uint[] _sweep = new uint[swp.Count];
            for (int _i = 0; _i < swp.Count; _i++)
                _sweep[_i] = (uint)swp[_i];

            InfoBlock.OverrideShutterSpeeds(_sweep);
        }
        #endregion



        #region UI
        // Invoke this when using threads!
        public void UpdateGraph()
        {
            _statWin.UpdateActiveDownloadsToolstrip();
        }
        #endregion
    }
}
