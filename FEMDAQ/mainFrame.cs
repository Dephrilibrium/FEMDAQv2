//#define NO_DEVICES_ALLOWED  // Used for debugging JobQueue!

using FEMDAQ.Initialization;
using FEMDAQ.Measurement;
using FEMDAQ.SplashScreen;
using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using Instrument.LogicalLayer;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FEMDAQ
{
    public partial class FEMDAQ : Form
    {
        // Mainframe basic header
        public string ProgramBasicTitletext { get; private set; }

        /* 
         * Thread-Locks and -stuff
         */
        // Manager / Global
        //private ManualResetEvent _measureTaskWakeSignal;
        private Task _measureTask;
        private ManualResetEvent _measureManagerTaskWakeSignal;
        private CancellationTokenSource _measureManagerTaskCancellation;
        private CancellationTokenSource _measureTaskCancellation;
        private GaugeMeasureInstantly _measureAtActual; // Used for sources-setup by setting an out-of-range value (non-valid member of GaugeMeasureInstantly-Enumeration)
        private bool _setSources = false;

        // Device threads
        private List<Task> _devMeasureTasks;
        private List<ManualResetEvent> _measureTaskWakeSignals;
        private List<ManualResetEvent> _measureTaskReadySignals;
        //private List<CancellationTokenSource> _measureTaskCancellations;

        // Files
        private IniContent _ini = null;
        private SweepContent _sweep = null;

        // Paths
        public string LastOpenIniPath { get; set; }
        public string LastOpenSweepPath { get; set; }
        public string LastSavePath { get; set; }
        public SavingPopup SavingPopup { get; set; }

        // Devices
        private List<InstrumentLogicalLayer> _devices;
        public OperationStatus OperationStatus;

        // Time
        private DateTime _startTime;
        private List<double> _diffTimeStamps;
        private string _timestampFormat = "yyMMdd_HHmmss";

        // Jobqueue
        private JobQueue.JobQueueFrame _jobQueueFrame = null;
        public event Action CurrentJobDone;
        public string SaveFolderFullPath { get; set; } // On using the jobqueue there is a valid path stored otherwise null is default. 
                                                       //  If there is null, "" or an invalid path a wild folderbrowserdialog appears ^_^.

        // Autoupdater
        private Process _autoUpdater = null;


        /// <summary>
        /// Initializing the entire program!
        /// </summary>
        public FEMDAQ()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("EN-US");
            ProgramBasicTitletext = Application.ProductName + " (Build: V" + Application.ProductVersion + ")";
            Text = ProgramBasicTitletext;

            SavingPopup = new SavingPopup(0); // Marquee at first!
            SavingPopup.Owner = this;

            GlobalVariables.MainFrame = this; // Provide mainframe via static "GlobalVariables" class

            var splashScreen = new SplashScreenFrame();
            splashScreen.Show();


            //OpenIni(@"Z:\Hausladen\Programs\FEMDAQ_V2\Debug\default.ini");
            //OpenSweep(@"Z:\Hausladen\Programs\FEMDAQ_V2\Debug\default.swp");

            //OpenIni(@"Z:\_FEMDAQ V2 for Measurement\Hausi\240918 BA Const IIon\Messungen\_INI - Activation (CathReg).ini");
            //OpenSweep(@"Z:\_FEMDAQ V2 for Measurement\Hausi\240918 BA Const IIon\Messungen\_swp VSwp - -150_-350V_-5VS, IMax=9.00.swp");
        }



        /// <summary>
        /// Event called when the form is closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FEMDAQ_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (OperationStatus != null)
            {
                if (OperationStatus.Status == MeasurementStatus.Running
                || OperationStatus.Status == MeasurementStatus.Paused)
                    StopMeasureLogic();
            }

            DisposeDevices();
            if (Chart != null)
                Chart.Dispose();

            if (_jobQueueFrame != null)
                _jobQueueFrame.Dispose();
        }


        private void DisposeDevices()
        {
            if (_devices != null)
            {
                foreach (var device in _devices)
                {
                    try { device.Dispose(); }
                    catch { }
                }
                _devices = null;
            }
        }



        #region Open file (Ini/Sweep) methods
        public void OpenIni(string filename)
        {
            if (filename != null && filename != "")
                LastOpenIniPath = Path.GetDirectoryName(filename);

            if (!File.Exists(filename)) return;

            // Clean up old ressources
            //if (_devices != null)
            //    foreach (var device in _devices)
            //        device.Dispose();
            DisposeDevices();
            Chart.WipeChart();

            // Renew configuration
            tbLog.Clear();

            try { _ini = new IniContent(filename); }
            catch (Exception e)
            {
                MessageBox.Show("Error while instanciating InfoBlocks!\r\n\r\n" + e.Message + (e.InnerException != null ? "\r\n\r\n" + e.InnerException.Message : "") /* + "\r\n\r\n" +
                                "Stack-Trace:" + e.StackTrace*/, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            gbIniInfo.Text = "Ini-Log (" + _ini.Filename + "):";
            ManageInit.InitToolInfo(this, _ini.ToolInfo);
            ManageInit.InitTimers(this, _ini.TimingInfo);
            foreach (var chartInfo in _ini.ChartInfo)
            {
                //var showLegend = (chartInfo.ShowLegend != 0 ? true : false);
                //Chart.AddArea(chartInfo.ChartIdentifier, chartInfo.ChartName, chartInfo.XAxisTitle, chartInfo.YAxisTitle, chartInfo.XAxisBoundaries, chartInfo.YAxisBoundaries, chartInfo.XAxisLogBase, chartInfo.YAxisLogBase, showLegend);
                Chart.AddArea(chartInfo.ChartInfo);
            }


            // Device creation
            _devices = new List<InstrumentLogicalLayer>();
            foreach (var deviceInfoStructure in _ini.DeviceInfoStructures)
            {
                try
                {
                    _devices.Add(ManageInit.GenerateDevice(deviceInfoStructure, _sweep, Chart));
                    AddingTextToLog(string.Format("{0}|{1}: ", deviceInfoStructure.DeviceIdentifier, deviceInfoStructure.DeviceType),
                                    string.Format("OK\n"),
                                    true);
                }
                catch (Exception e)
                {
                    AddingTextToLog(string.Format("{0}|{1}: ", deviceInfoStructure.DeviceIdentifier, deviceInfoStructure.DeviceType),
                                    string.Format("Failed:\nMessage: {0}\n\n\nStackTrace:\n", e.Message, e.StackTrace),
                                    false);
                }
            }
        }



        public void OpenSweep(string filename)
        {
            if (filename != null && filename != "")
                LastOpenSweepPath = Path.GetDirectoryName(filename);

            if (filename == null) throw new ArgumentNullException("filename");
            if (!File.Exists(filename))
            {
                if (_sweep == null)
                {
                    AddingTextToLog(string.Format("Writing sweep-data to listview: "),
                                    string.Format("File not found and no old one loaded.\n\n"), false);
                    return;
                }
                AddingTextToLog(string.Format("Writing sweep-data to listview: "),
                                    string.Format("File not found."), false);
                AddingTextToLog("", string.Format(" Using old one instead.\n\n"), true);
            }
            else
            {
                try { _sweep = new SweepContent(filename); }
                catch (Exception e)
                {
                    AddingTextToLog("Loading sweep failed: ", e.Message, false);
                    return;
                }
                try
                {
                    SweepToListview(_sweep.Header, _sweep.Values);
                    AddingTextToLog(string.Format("Writing sweep-data to listview: "),
                                    string.Format("OK\n"),
                                    true);
                }
                catch (Exception e)
                {
                    AddingTextToLog(string.Format("Writing sweep-data to listview: "),
                                    string.Format("Failed\nMessage: {0}\n\n", e.Message),
                                    false);
                    return;
                }
            }

            if (_devices != null)
            {
                foreach (var device in _devices)
                {
                    try { device.AssignSweepColumn(_sweep); }
                    catch (Exception e) { AddingTextToLog("Assigning sweep failed - " + device.DeviceIdentifier + "|" + device.DeviceType + ": \r\nMessage: ", e.Message + "\r\n", false); }
                }
            }
        }
        #endregion




        #region Log-methods
        /// <summary>
        /// Adds the given text to the log-window. Success choose the color for ok (limegreen) and error (red) of the ColoredText. 
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="DrawColor"></param>
        public void AddingTextToLog(string BlackText, string ColoredText, bool Success)
        {
            tbLog.SelectionColor = Color.Black;
            tbLog.AppendText(BlackText);
            tbLog.SelectionColor = (Success ? Color.LimeGreen : Color.Red);
            tbLog.AppendText(ColoredText); // + Environment.NewLine;
        }
        #endregion




        #region Listview
        /// <summary>
        /// Inserts the sweepfile into a listview.
        /// 
        /// Return 0 if everything gone well. A negative number indicates an error.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private void SweepToListview(List<string> header, List<List<double>> values)
        {
            lvSweepData.Clear();
            if (header == null) { gbSweepInfo.Text = "Sweepfile (None):"; throw new ArgumentNullException("header"); }
            if (values == null) { gbSweepInfo.Text = "Sweepfile (None):"; throw new ArgumentNullException("values"); }

            // Header
            var columnWidth = lvSweepData.Width / header.Count;
            foreach (var headerCell in header)
                lvSweepData.Columns.Add(headerCell);

            // Add values
            foreach (var valueRow in values)
            {
                var stringItems = new List<string>();
                foreach (var valueCell in valueRow)
                    stringItems.Add(Convert.ToString(valueCell));

                var listViewItem = new ListViewItem(stringItems.ToArray());
                lvSweepData.Items.Add(listViewItem);
            }

            ResizeListViewColumns();
            gbSweepInfo.Text = "Sweepfile (" + _sweep.Filename + "):";
        }



        /// <summary>
        /// Resizing the columnheader of the listview.
        /// 
        /// Return 0 if everything gone well. A negative number indicates an error.
        /// </summary>
        /// <returns></returns>
        public int ResizeListViewColumns()
        {
            if (lvSweepData.Columns.Count < 1)
                return -1; // Error: No columns in listview

            // Get columnwidth and resize
            int columnWidth = lvSweepData.ClientSize.Width / lvSweepData.Columns.Count;
            foreach (ColumnHeader column in lvSweepData.Columns)
                column.Width = columnWidth;
            return 0;
        }
        #endregion




        #region Timerevents
        /// <summary>
        /// Event is called when the initial timerticks are elapsed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitialTimer_TicksElapsed(object sender, EventArgs e)
        {
            InitialTimer.Stop(); // Stop initial timer
            //_measureTaskWakeSignal.Set(); // Wake measureTask
            //foreach (var wake in _measureTaskWakeSignals)
            //    wake.Set();
            _measureManagerTaskWakeSignal.Set();
            IterativeTimer.Start(); // Start iterating
        }



        /// <summary>
        /// Event is called when the iterative timerticks are elapsed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IterativeTimer_TicksElapsed(object sender, EventArgs e)
        {
            // Measurement-sequence
            //_measureTaskWakeSignal.Set(); // Wake measureTask
            _measureManagerTaskWakeSignal.Set();
            //foreach (var wake in _measureTaskWakeSignals)
            //    wake.Set();
        }
        #endregion



        #region Measurementlogic running on measure thread
        private void ThreadMeasure(Control Dispatcher, List<InstrumentLogicalLayer> Devices, ManualResetEvent MeasureWake, ManualResetEvent Ready, CancellationTokenSource Cancel)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("EN-US");

            GaugeMeasureInstantly measureAtActual = GaugeMeasureInstantly.Disabled;
            bool setSource = false;
            while (!Cancel.IsCancellationRequested)
            {
                Ready.Set();
                MeasureWake.WaitOne();
                MeasureWake.Reset();
                if (Cancel.IsCancellationRequested)
                    break;

                foreach (var device in Devices)
                {
                    setSource = _setSources;
                    if (_setSources) // On cycle-start setup the source
                        device.SetSourceValues(OperationStatus.SweepLineIndexInProgress);

                    if (Enum.IsDefined(typeof(GaugeMeasureInstantly), _measureAtActual)) // !!! ATTENTION !!! There exists measureAtActual and _measureAtActual
                    {
                        measureAtActual = _measureAtActual; // Create copy
                        device.Measure(GetDrawnOver, measureAtActual);

                        if (measureAtActual == GaugeMeasureInstantly.CycleEnd)
                            Dispatcher.BeginInvoke(new Action(device.UpdateGraph));
                    }
                }
            }

            foreach(var device in Devices)
                device.PowerDownSource();


            Ready.Set(); // Finished shutting down!
        }

        private void MeasureThreadsReady()
        {
            // Wait for each device-thread to be ready
            foreach (var ready in _measureTaskReadySignals)
            {
                ready.WaitOne(/*_ini.TimingInfo.IterativeTime*/); // Just ignore non responding threads, may get crashed by windows later on?
                ready.Reset();
            }
        }


        private void ProcessMeasureCycle(Control dispatcher, ManualResetEvent Wake, /*CancellationTokenSource*/ CancellationToken Cancel/*, Action updateListView*/)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("EN-US");

            // Init-thing
            //_measureManagerTaskWakeSignal.WaitOne(); // Wait for wake on signal
            //_measureManagerTaskWakeSignal.Reset(); // Reset wake-signal
            Wake.WaitOne(); // Wait for wake on signal
            Wake.Reset(); // Reset wake-signal


            //if (_measureManagerTaskCancellation.IsCancellationRequested) // Check cancellation!
            if (Cancel.IsCancellationRequested) // Check for "instant-cancellation"!
            {
                goto CancelMeasThreads;
                //_measureTaskCancellation.Cancel();
                //WakeMeasureThreads();
                //MeasureThreadsReady();
                //return;
            }

            SetupSources(); // Moved to device-thread
            lock (_diffTimeStamps)
            {
                var diffTimeArr = GetDrawnOver(new List<string> { "TIME" });
                _diffTimeStamps.Add(diffTimeArr[0]);
            }
            InstantMeasureCycle();

            // Regular measuring
            //while (_measureManagerTaskCancellation.IsCancellationRequested == false) // Check for cancellation before waiting for next measure-step
            while (Cancel.IsCancellationRequested == false) // Check for cancellation before waiting for next measure-step
            {
                //_measureManagerTaskWakeSignal.WaitOne(); // Wait for wake on signal
                //_measureManagerTaskWakeSignal.Reset(); // Reset wake-signal
                Wake.WaitOne(); // Wait for wake on signal
                Wake.Reset(); // Reset wake-signal

                /* main-Thread requests cancellation to measureManager
                   measureManager requests cancellation to measureTasks
                   Otherwise it can happen, that main- and measureManager threads watching and resetting the "finish"-response-events of the measuretasks
                   so that they wait forever.
                */
                // if (_measureManagerTaskCancellation.IsCancellationRequested) // Check for cancellation after waiting!
                if (Cancel.IsCancellationRequested) // Check for cancellation after waiting!
                    break;

                RegularMeasureCycle();
                // Stop measurement
                if (OperationStatus.SweepLineIndexOverflow)
                {
                    dispatcher.BeginInvoke(new Action(StopMeasureLogic)); // Sets taskKill
                    dispatcher.BeginInvoke(new Action(UpdateListView));
                    dispatcher.BeginInvoke(new Action(UpdateProgress));

                    // Tested: When invoking StopMeasureLogic it needs to much time to set the cancellation-token.
                    //  In the meantime the MeasureManger may come back and invoke again before noticing to get closed!
                    //  Therefore wait first for next wake before going back to "while(!Canceled)"
                    Wake.WaitOne(); // Wait for wake on signal
                    Wake.Reset(); // Reset wake-signal
                    continue;
                }

                if (OperationStatus.UpdateToNextIterate())
                {
                    if (!OperationStatus.SweepLineIndexOverflow) // Update sources only when no overflow is appeared
                    {
                        dispatcher.BeginInvoke(new Action(UpdateListView));
                        SetupSources();
                    }
                    lock (_diffTimeStamps)
                    {
                        var diffTimeArr = GetDrawnOver(new List<string> { "TIME" });
                        _diffTimeStamps.Add(diffTimeArr[0]);
                    }
                }
                if (!OperationStatus.SweepLineIndexOverflow) // Measure gauges only when no overflow is appeard
                {
                    dispatcher.BeginInvoke(new Action(UpdateProgress));
                    InstantMeasureCycle();
                }
            }

            lock (_diffTimeStamps)
            {
                var diffTimeArr = GetDrawnOver(new List<string> { "TIME" });
                _diffTimeStamps.Add(diffTimeArr[0]);
            }

        CancelMeasThreads:
            // When breaking out "cancellation" or "finished measurement" occured
            // Sends cancellation to measure-threads and wait for their finish before leaving
            //  Cancellint the measure-threads has to be done by the measure-manager
            //  due to otherwise (older code) started to hang up, waiting for the
            //  measure-threads finishing (the response tokens of them were checked and resetted from 2 threads!)
            _measureTaskCancellation.Cancel();
            WakeMeasureThreads();
            MeasureThreadsReady();
        }



        private void SetupSources()
        {
            _measureAtActual = (GaugeMeasureInstantly)(Enum.GetValues(typeof(GaugeMeasureInstantly)).Length); // Use non-existing value to avoid unexpected measurements!
            _setSources = true;
            WakeMeasureThreads();
            MeasureThreadsReady();
        }


        private void InstantMeasureCycle()
        {
            _measureAtActual = GaugeMeasureInstantly.CycleStart;
            _setSources = false;
            WakeMeasureThreads();
            MeasureThreadsReady();

        }


        private void RegularMeasureCycle()
        {
            _measureAtActual = GaugeMeasureInstantly.CycleEnd;
            _setSources = false;
            WakeMeasureThreads();
            MeasureThreadsReady();
        }

        private void WakeMeasureThreads()
        {
            foreach (var wake in _measureTaskWakeSignals)
                wake.Set();
        }


        private double[] GetDrawnOver(List<string> identifiers)
        {
            if (identifiers != null)
            {
                var drawnOvers = new double[identifiers.Count];
                string identifier;
                string[] splitIdentifier;

                for (var index = 0; index < identifiers.Count; index++)
                {
                    identifier = identifiers[index].ToUpper();
                    if (identifier == "TIME")
                    {
                        //var diffTime = new double[1];
                        drawnOvers[index] = DateTime.Now.Subtract(_startTime).TotalSeconds;
                        continue;
                    }

                    splitIdentifier = StringHelper.TrimArray(identifier.Split(new char[] { '|' }));
                    foreach (var device in _devices)
                    {
                        if (splitIdentifier.Length > 0 && splitIdentifier[0] == device.DeviceIdentifier.ToUpper()) // Identifier can be in the form <Dev3|F> -> StartsWith!
                        {
                            drawnOvers[index] = device.GetSourceValue(identifier);
                            break;
                        }
                    }
                }
                return drawnOvers;
            }
            var dummy = new double[1];
            return dummy;
        }

        #endregion



        #region Measurement logic related methods running on mainthread (Start, Pause/Resume, Stop)
        /// <summary>
        /// Starts the measurement.
        /// </summary>
        /// <returns></returns>
        private void StartMeasureLogic()
        {
            // Errorhandling
            if (_ini == null)
            {
                MessageBox.Show("No ini-file loaded. Measurement aborted!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_sweep == null)
            {
                MessageBox.Show("No sweep-file loaded. Measurement aborted!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
#if !NO_DEVICES_ALLOWED  // Used for debugging JobQueue!
            if (_devices == null || _devices.Count <= 0)
            {
                MessageBox.Show("There are no constructed devices. Measurement aborted!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
#endif


            // Cleaning up all old datasets
            lock (_devices)
            {
                foreach (var device in _devices)
                {
                    device.ClearResults();
                    device.Init();
                    device.DoBeforeStart();
                }
            }

            // Mark first sweep-line
            foreach (ListViewItem item in lvSweepData.Items)
                item.BackColor = Color.White;
            lvSweepData.Items[0].BackColor = Color.NavajoWhite;

            // Create sequence-object
            OperationStatus = new OperationStatus(_ini, _sweep);
            OperationStatus.Status = MeasurementStatus.Running;
            UpdateUI();
            sbProgress.Value = 0;
            sbProgress.Maximum = OperationStatus.NumberOfIterates;
            UpdateProgress(); // Initial update of the statusbar-info!


            /*
             * Due to GPIB can't talk with more than 1 device simultaneously, it's necessary to create lists of devices for each thread.
             * Each of that lists contains the devices foreseen for a separate thread.
             * Therefore all devices are iterated through and collected in the lists (e.g. for GPIB)
             */
            List<List<InstrumentLogicalLayer>> ThreadDeviceLists = new List<List<InstrumentLogicalLayer>>(); // Each of the containing device-lists getting a separate thread
            List<InstrumentCommunicationPHY> sequencedCommunicationTypes = new List<InstrumentCommunicationPHY>();// List which contains the CommunicationTypes which have to be collected for sequence-measurement
            sequencedCommunicationTypes.Add(InstrumentCommunicationPHY.GPIB); // GPIB can't communicate with > 1 device simultaneously
            //sequencedCommunicationTypes.Add(/* Add other CommunicationPhyTypes if there should be measured in sequence! */);

            List<InstrumentLogicalLayer> devicesCopy = new List<InstrumentLogicalLayer>(_devices); // This _devices copy is used to remove collected devices based on their CommunicationType

            // Find the devices of the communication-types (list from above) which are not able to communicate simultaneously
            foreach (var type in sequencedCommunicationTypes)
            {
                var sequenceList = _devices.FindAll(layer => layer.CommunicationPhy == type); // Find all devices of a sequence-communication-type
                devicesCopy.RemoveAll(dev => sequenceList.Contains(dev));                     // Remove that from the device-copy
                ThreadDeviceLists.Add(sequenceList);                                          // Add the List
            }

            //foreach (var device in _devices)
            foreach (var device in devicesCopy)
            {
                ThreadDeviceLists.Add(new List<InstrumentLogicalLayer>());
                ThreadDeviceLists[ThreadDeviceLists.Count - 1].Add(device);
            }



            /*
             * Measurement Threading (Manager)
             */
            _measureManagerTaskCancellation = new CancellationTokenSource();
            _measureManagerTaskWakeSignal = new ManualResetEvent(false);
            _measureTask = new Task(() => ProcessMeasureCycle(this, _measureManagerTaskWakeSignal, _measureManagerTaskCancellation.Token /*, UpdateListView*/));
            _measureTask.Start(); // By starting this one first, it should be ready to start until the device-threads are started

            /*
             * Measurement Threading (Devices)
             */
            _measureTaskCancellation = new CancellationTokenSource();
            _devMeasureTasks = new List<Task>();
            _measureTaskWakeSignals = new List<ManualResetEvent>();
            _measureTaskReadySignals = new List<ManualResetEvent>();
            for (int iDevPhyList = 0; iDevPhyList < ThreadDeviceLists.Count; iDevPhyList++)
            {
                // Create signals
                _measureTaskWakeSignals.Add(new ManualResetEvent(false));
                _measureTaskReadySignals.Add(new ManualResetEvent(false));

                // Fetch separate copies (otherwise async calls can lead to expections due to iDev is not existing anymore when the thread runs its lambda-method)
                var devList = ThreadDeviceLists[iDevPhyList];
                var wakeSignal = _measureTaskWakeSignals[iDevPhyList];
                var readySignal = _measureTaskReadySignals[iDevPhyList];

                // Instanciating and run task
                wakeSignal.Reset();
                _devMeasureTasks.Add(new Task(() => ThreadMeasure(this, devList, wakeSignal, readySignal, _measureTaskCancellation)));
                //var devTask = _devMeasureTasks[iDev];
                _devMeasureTasks[iDevPhyList].Start();

                //ThreadPool.QueueUserWorkItem((state) => { ThreadMeasure(this, device, wakeSignal, readySignal, _measureTaskCancellation); });
            }

           
            MeasureThreadsReady(); // Wait for each device-thread to be ready



            /*
             * Starting the measurement
             */
            // Starting the initialtimer -> Starts measure-chain
            _diffTimeStamps = new List<double>();
            _startTime = DateTime.Now;
            InitialTimer.Start();
            _measureManagerTaskWakeSignal.Set();
        }



        /// <summary>
        /// Pauses the measurement.
        /// </summary>
        private void PauseMeasureLogic()
        {
            OperationStatus.Status = MeasurementStatus.Paused;
            if (OperationStatus.InitInProgress)
                InitialTimer.Stop();
            else
                IterativeTimer.Stop();

            UpdateUI();
        }



        /// <summary>
        /// Resumes the measurement.
        /// </summary>
        private void ResumeMeasureLogic()
        {
            OperationStatus.Status = MeasurementStatus.Running;
            if (OperationStatus.InitInProgress)
                InitialTimer.Start();
            else
                IterativeTimer.Start();

            UpdateUI();
        }



        /// <summary>
        /// Stops the measurement.
        /// </summary>
        private void StopMeasureLogic()
        {
            _measureManagerTaskCancellation.Cancel();
            InitialTimer.Stop();
            IterativeTimer.Stop();

            // This can cause big problems due to the Response-Events can be resetted from 2 threads!
            //  Therefore when not pressing in the correct moment, the programs stops to work due to
            //  one of the threads waits forever for the measure-threads to finish
            // -> Solution: Stop the measuremanager from here, and use the measuremanager to cancel the measurethreads!
            //WakeMeasureThreads(); 
            //ThreadsReady();

            _measureManagerTaskWakeSignal.Set();
            while (!_measureTask.IsCompleted)
            {
                Thread.Sleep(25);
                //_measureManagerTaskWakeSignal.Set();
            } // Wait for the measurethread to be finished!


            lock (_devices) // This causes any problems when added after saving-results! (directly below)
            {
                foreach (var device in _devices)
                {
                    device.DoAfterFinished();
                }
            }


            OperationStatus.Status = MeasurementStatus.Stopped;
            UpdateUI();
            if (SaveFolderFullPath != null) // In case of using jobqueue
            {
                SaveResultsFromQueue(SaveFolderFullPath, CurrentJobDone);
            }
            else // No jobqueue!
                SaveResultsFromUser();
        }



        /// <summary>
        /// Updates the UI depending on the actual set program-state.
        /// </summary>
        private void UpdateUI()
        {
            switch (OperationStatus.Status)
            {
                case MeasurementStatus.Running:
                    miFileOpen.Enabled = false;
                    miFileSaveLastResults.Enabled = false;
                    miFileOpenIni.Enabled = false;
                    miFileOpenSweep.Enabled = false;
                    miStart.Enabled = false;
                    miPause.Enabled = true;
                    miPause.Text = "Pause (F6)";
                    miStop.Enabled = true;
                    miUpdate.Enabled = false;
                    sbStatus.Text = OperationStatus.Status.ToString();
                    sbStatus.BackColor = Color.LimeGreen;
                    break;

                case MeasurementStatus.Paused:
                    miPause.Text = "Resume (F6)";
                    sbStatus.Text = OperationStatus.Status.ToString();
                    sbStatus.BackColor = Color.NavajoWhite;
                    break;

                case MeasurementStatus.Stopped:
                    miFileOpen.Enabled = true;
                    miFileSaveLastResults.Enabled = true;
                    miFileOpenIni.Enabled = true;
                    miFileOpenSweep.Enabled = true;
                    miStart.Enabled = true;
                    miPause.Enabled = false;
                    miPause.Text = "Pause (F6)";
                    miStop.Enabled = false;
                    miUpdate.Enabled = true;
                    sbStatus.Text = OperationStatus.Status.ToString();
                    sbStatus.BackColor = Color.Red;
                    break;
            }
        }



        private void UpdateListView()
        {
            if (OperationStatus.SweepLineIndexInProgress >= 1)
            {
                lvSweepData.Items[OperationStatus.SweepLineIndexInProgress - 1].BackColor = Color.White;
                if (!OperationStatus.SweepLineIndexOverflow)
                {
                    lvSweepData.Items[OperationStatus.SweepLineIndexInProgress].BackColor = Color.NavajoWhite;
                    lvSweepData.EnsureVisible(OperationStatus.SweepLineIndexInProgress);
                }
                //sbProgress.Value = _operationStatus.IteratesDone;
            }
        }


        private void UpdateProgress()
        {
            sbProgress.Value = OperationStatus.IteratesDone;
            sbIterates.Text = string.Format("Iterates: {0}/{1}", OperationStatus.IteratesDone, OperationStatus.NumberOfIterates);
            sbRemaining.Text = string.Format("Remaining: {0} ({1})", OperationStatus.RemainingDuration.ToString(@"hh\:mm\:ss"), OperationStatus.EstimatedStopTime.ToString("HH:mm"));
        }



        /// <summary>
        /// Saves the results after the measurement is stopped.
        /// </summary>
        private void SaveResultsFromUser()
        {
            var folderDialog = new CommonOpenFileDialog("Select folder...");
            folderDialog.IsFolderPicker = true;
            if (LastSavePath != null && LastSavePath != "")
                folderDialog.InitialDirectory = LastSavePath;
            var folderDialogResult = folderDialog.ShowDialog();
            if (folderDialogResult != CommonFileDialogResult.Ok)
                return;

            // Check grouped results
            var scanFolder = folderDialog.FileName;
            var timestamp = DateTime.Now;
            if(_ini.ToolSettings.SaveResultsGrouped)
                scanFolder = Path.Combine(scanFolder, timestamp.ToString(_timestampFormat));

            if (Directory.Exists(scanFolder))
            {
                var folderFiles = Directory.GetFiles(scanFolder);
                var dialogResult = DialogResult.None;
                if (folderFiles.Length > 0)
                    dialogResult = MessageBox.Show("The folder already contains some files. It's possible that some files will be overwritten.\n\nHit \"Yes\" if you want to choose this folder anyway. Otherwise hit \"No\".",
                                                   "Folder already exists...",
                                                   MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Exclamation);
                if (dialogResult == DialogResult.No)
                    return;
            }

            LastSavePath = folderDialog.FileName; // Last savepath should be always the parent directory!
            if (_ini.ToolSettings.SaveResultsGrouped) 
                SaveResultsToFolder(scanFolder, ""); // Save in subfolder 
            else
                SaveResultsToFolder(folderDialog.FileName, timestamp.ToString(_timestampFormat)+"_"); // Or with timestamp as prefix
        }


        public void SaveResultsFromQueue(string folderPath, Action jobDoneCallback = null)
        {
            var timestamp = DateTime.Now;

            if (folderPath == null)
                folderPath = Path.Combine(Application.StartupPath, @"\FolderPathWasNull\", DateTime.Now.ToString("yyMMdd_HHmm"));

            var filePrefix = timestamp.ToString(_timestampFormat) + "_"; // Predefine filePrefix (no grouping is standard!)
            if (_ini.ToolSettings.SaveResultsGrouped)
            {
                folderPath = Path.Combine(folderPath, timestamp.ToString(_timestampFormat));
                filePrefix = ""; // Clear filePrefix!
            }

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath); // Create directory if not exists

            SaveResultsToFolder(folderPath, filePrefix, jobDoneCallback); // Execute save: folderPath and filePrefix manipulated above
        }

        bool _alreadySaving = false;
        private void SaveResultsToFolder(string folderPath, string filePrefix, Action jobDoneCallback = null)
        {
            // Checking if saving is already ongoing
            if (_alreadySaving == true)
                return;
            _alreadySaving = true; // Set until leaving save-method


            // Guard clauses
            if (folderPath == null)
                throw new NullReferenceException("No savepath given");
            if (filePrefix == null)
                filePrefix = "";

#if !NO_DEVICES_ALLOWED // Used for debugging JobQueue!
            if (_devices == null)
            {
                MessageBox.Show("There are no devices created.\n\nSaving aborted.",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
#endif

            // Create directory if not exists
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Saving files
            Enabled = false; // Disable gui while saving files
            SavingPopup.CenterToOwner();
            SavingPopup.SetBarMax(_devices.Count);
            SavingPopup.ResetBar();
            SavingPopup.Show(this);
            // save routine itself to background-thread
            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                lock (_devices)
                {
                    foreach (var devLayer in _devices)
                    {
                        devLayer.SaveResultsToFolder(folderPath, filePrefix);
                        Invoke(new Action(
                                            () =>
                                            {
                                                SavingPopup.IncrementBarValueBy(1);
                                            }
                                            ));
                    }
                }
                // Invoke from backgroundworker to mainframe to reenable gui
                Invoke(new Action(
                    () =>
                    {
                        Enabled = true;
                        SavingPopup.Hide();
                        if (jobDoneCallback != null)
                            jobDoneCallback();
                    }
                    ));
            }); // Threadpool close

            // Fast stuff can be done in GUI-Thread
            SaveChartCapturePng(folderPath, filePrefix);
            SaveTimeStamps(folderPath, filePrefix);
            SaveLog(folderPath, filePrefix);
            try
            {
                CopyIniAndSweepFiles(folderPath, filePrefix); // This as last, because if renamed or something, it crashes after the other parts already saved!
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\nIgnoring Ini and Sweep!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _alreadySaving = false; // Remove flag
        }



        //private void SaveTimeStamps(string folderPath)
        //{
        //    SaveTimeStamps(folderPath, DateTime.Now.ToString("yyMMdd_HHmmss");
        //}

        private void SaveTimeStamps(string folderPath, string filePrefix)
        {
            // Guard clauses
            if (folderPath == null)
                throw new NullReferenceException("No savepath given");
            if (filePrefix == null)
                filePrefix = "";

            var deviceName = string.Format("TimestampsAfterSourceSetup");
            var output = new StringBuilder("# [Timestamps after sourcesetup] in [s]\n");
            double diffTimeStamp;
            output.AppendLine("# Inittime: " + (_ini.TimingInfo.InitialTime / 1000d).ToString("F3") + "[s]");
            output.AppendLine("# Iterativetime: " + (_ini.TimingInfo.IterativeTime / 1000d).ToString("F3") + "[s]");
            lock (_diffTimeStamps)
            {
                for (var valueIndex = 0; valueIndex < _diffTimeStamps.Count; valueIndex++)
                {
                    diffTimeStamp = _diffTimeStamps[valueIndex];
                    output.AppendLine(string.Format("{0}", Convert.ToString(diffTimeStamp)));
                }
            }
            var filename = Path.Combine(folderPath, filePrefix + deviceName + ".dat");
            var fileWriter = new StreamWriter(filename, false);
            fileWriter.Write(output);
            fileWriter.Dispose();
        }



        //private void CopyIniAndSweepFiles(string folderPath)
        //{
        //    CopyIniAndSweepFiles(folderPath, DateTime.Now);
        //}

        private void CopyIniAndSweepFiles(string folderPath, string filePrefix)
        {
            // Guard clauses
            if (folderPath == null)
                throw new NullReferenceException("No savepath given");
            if (filePrefix == null)
                filePrefix = "";

            var targetIniPath = Path.Combine(folderPath, filePrefix + _ini.Filename);
            var targetSwpPath = Path.Combine(folderPath, filePrefix + _sweep.Filename);

            try { File.Copy(_ini.Fullfilename, targetIniPath, true); }
            catch (Exception) { throw new Exception("Ini-file not found."); }

            try { File.Copy(_sweep.FullFilename, targetSwpPath, true); }
            catch (Exception) { throw new Exception("Sweep-file not found."); }
        }


        //private void SaveChartCapturePng(string folderPath)
        //{
        //    SaveChartCapturePng(folderPath, DateTime.Now);
        //}

        private void SaveChartCapturePng(string folderPath, string filePrefix)
        {
            // Guard clauses
            if (folderPath == null)
                throw new NullReferenceException("No savepath given");
            if (filePrefix == null)
                filePrefix = "";

            Chart.SaveChartCapture(Path.Combine(folderPath, filePrefix + "ChartScreen.png"), ImageFormat.Png);
        }



        //private void SaveLog(string folderPath)
        //{
        //    SaveLog(folderPath, DateTime.Now);
        //}

        private void SaveLog(string folderPath, string filePrefix)
        {
            // Guard clauses
            if (folderPath == null)
                throw new NullReferenceException("No savepath given");
            if (filePrefix == null)
                filePrefix = "";


            var logOutput = "Original ini-File location: " + _ini.Fullfilename + "\n"
                          + "Original sweep-File location: " + _sweep.FullFilename + "\n";
            var targetLogPath = Path.Combine(folderPath, filePrefix + "log.log");

            var logStream = new StreamWriter(targetLogPath);
            logStream.Write(logOutput);
            logStream.Dispose();
            logStream = null;
        }
        #endregion



        #region "UI-Events"
        /// <summary>
        /// Event called by clicking "File->Open->Ini-File". It opens a file dialogue to choose an ini-file and runs the configuration!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miFileOpenIni_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Ini|*.ini";
            if (LastOpenIniPath != null && LastOpenIniPath != "")
                fileDialog.InitialDirectory = LastOpenIniPath;
            fileDialog.ShowDialog(this);
            if (fileDialog.CheckFileExists)
                OpenIni(fileDialog.FileName);

            if (_ini != null && _ini.SweepInfo != null && _ini.SweepInfo.FullFilename != null && _ini.SweepInfo.FullFilename != "") // Ini has sweep configured
                OpenSweep(_ini.SweepInfo.FullFilename);
            else if (_sweep != null && _devices != null) // if not use old one instead if there is already any sweep loaded
            {
                foreach (var device in _devices)
                {
                    try { device.AssignSweepColumn(_sweep); }
                    catch (Exception exeption) { AddingTextToLog("Assigning sweep failed - " + device.DeviceIdentifier + "|" + device.DeviceType + ": \r\nMessage: ", exeption.Message + "\r\n", false); }
                }
            }
        }



        /// <summary>
        /// Event called by clicking "File->Open->Sweep-File". I opens a file dialogue to choose a sweep-file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miFileOpenSweep_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Sweep|*.swp;*.sweep";
            if (LastOpenSweepPath != null && LastOpenSweepPath != "")
                fileDialog.InitialDirectory = LastOpenSweepPath;
            //fileDialog.InitialDirectory = Application.StartupPath;
            var result = fileDialog.ShowDialog(this);
            if (result == DialogResult.Cancel)
                return;

            OpenSweep(fileDialog.FileName);
        }



        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveResultsFromUser();
        }



        /// <summary>
        /// Event called by clicking "File->Close" or using "Alt+F4" macro. It closes the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miFileClose_Click(object sender, EventArgs e)
        {
            Close();
        }




        /// <summary>
        /// Event called by clicking "Measurement->Start" or using "F5" macro. It runs the measurement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void msStart_Click(object sender, EventArgs e)
        {
            StartMeasureLogic();
        }

        public void ExternalStart()
        {
            msStart_Click(null, new EventArgs());
        }



        /// <summary>
        /// Event called by clicking "Measurement->Pause" or using "F6" macro. It pauses or resume the measurement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void msPause_Click(object sender, EventArgs e)
        {
            if (OperationStatus.Status == MeasurementStatus.Running)
                PauseMeasureLogic();
            else
                ResumeMeasureLogic();
        }

        public void ExternalPause()
        {
            msPause_Click(null, new EventArgs());
        }



        /// <summary>
        /// Event called by clicking "Measurement->Stop" or using "F7" macro. It stops the measurement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void msStop_Click(object sender, EventArgs e)
        {
            StopMeasureLogic();
        }

        public void ExternalStop()
        {
            msStop_Click(null, new EventArgs());
        }



        /// <summary>
        /// Shows an info about FEMDAQ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("FEMDAQ V" + Application.ProductVersion + "\nProgrammer: Matthias Hausladen (©)\nInstitute: OTH-Regensburg",
                            "Info/About",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
        }



        /// <summary>
        /// Event called when the mainframe changes its size.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FEMDAQ_SizeChanged(object sender, EventArgs e)
        {
            ResizeListViewColumns();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miJobQueueCreateOrShow_Click(object sender, EventArgs e)
        {
            if (_jobQueueFrame == null)
            {
                miJobQueueCreateOrShow.Text = "Show queue";
                _jobQueueFrame = new JobQueue.JobQueueFrame(this);
            }
            if (!_jobQueueFrame.Visible)
                _jobQueueFrame.Show();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miJobQueueDelete_Click(object sender, EventArgs e)
        {
            if (_jobQueueFrame == null)
                return;

            var diagResult = MessageBox.Show("Are you sure you want delete your jobqueue irreversible? \n\n(Ongoing measurement will be stopped).",
                                             "Warning: Queue will be deleted inreversible!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (diagResult != DialogResult.Yes)
                return;

            miJobQueueCreateOrShow.Text = "Create queue";
            _jobQueueFrame.Dispose();
            _jobQueueFrame = null;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangelogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filename = @".\Properties\Changelog.txt";
            if (File.Exists(filename))
                System.Diagnostics.Process.Start(filename);
            else
                MessageBox.Show(this, "Changelogfile not found at " + filename, "File not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoUpdaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _autoUpdater = new Process();
            _autoUpdater.StartInfo.FileName = Path.Combine(Application.StartupPath, @".\AutoUpdater\AutoUpdater.exe");
            _autoUpdater.StartInfo.Arguments = @"-pid=" + Process.GetCurrentProcess().Id.ToString() + " -as";
            _autoUpdater.Start();
        }

        #endregion
    }
}
