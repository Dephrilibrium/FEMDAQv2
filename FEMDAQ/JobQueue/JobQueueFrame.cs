using FEMDAQ.StaticHelper;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace FEMDAQ.JobQueue
{
    enum JobQueueIndicies { Status = 0, IniFile, SwpFile, SavFold }

    public partial class JobQueueFrame : Form
    {
        private FEMDAQ _mainFrame = null;

        // Framevariables
        private bool CloseFormInsteadOfHiding { get; set; }
        private JobQueueStatus _jobQueueStatus = null;
        private bool _stopRequested { get; set; }

        // IsActive-Status
        private int _lastActiveJobIndex { get; set; }
        private int _skippedJobs { get; set; }
        public string IsActive = "✓";
        public string IsInactive = "X";

        // JobQueueList
        private string _lastFilePath { get; set; }
        private int _listErrorCount { get; set; }


        // RobertMode
        private bool _robertMode { get; set; }
        private ManualResetEvent _robertModeWaitForUserInput { get; set; }
        private bool _robertRequestedRepeat { get; set; }
        private bool _robertRequestedStop { get; set; }



        public JobQueueFrame(FEMDAQ mainFrame)
        {
            InitializeComponent();

            if (mainFrame == null) throw new NullReferenceException("mainFrame");
            _mainFrame = mainFrame;

            ColorizeRobertModeButton(false);
            UpdateUI(QueueState.Stopped);
            UpdateStatusbarButtons();

            RenumberJobs();

            //AddEntriesByDefaultForTesting();
        }


        private void AddEntriesByDefaultForTesting()
        {
            AddJob(@"Ini 1", @"Sweep 1", @".\bla1\");
            AddJob(@"Ini 2", @"Sweep 2", @".\bla2\");
            AddJob(@"Ini 3", @"Sweep 3", @".\bla3\");
            AddJob(@"Ini 4", @"Sweep 4", @".\bla4\");
            AddJob(@"Ini 5", @"Sweep 5", @".\bla5\");
        }



        public new void Dispose()
        {
            if (_jobQueueStatus != null && _jobQueueStatus.State != QueueState.Stopped)
                StopJob();
            CloseFormInsteadOfHiding = true;
            Close();
        }



        private void JobQueueFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseFormInsteadOfHiding)
                return;

            Hide();
            e.Cancel = true;
        }



        #region Listviewhelper
        private void MarkActiveJobQueueEntry(int index)
        {
            DataGridViewRow row = null;
            if (index == -1)
            {
                for (int rowIndex = 0; rowIndex < dgvJobQueue.RowCount - 1; rowIndex++)
                {
                    row = dgvJobQueue.Rows[rowIndex];
                    for (int cellIndex = 0; cellIndex < row.Cells.Count; cellIndex++)
                    {
                        row.Cells[cellIndex].Style.BackColor = Color.Empty;
                    }
                }
                return;
            }


            var lvItemCount = dgvJobQueue.RowCount - 1;
            if (index >= lvItemCount)
                return;

            if (index == _lastActiveJobIndex)
                return;

            row = dgvJobQueue.Rows[index];
            foreach (DataGridViewCell cell in row.Cells)
                cell.Style.BackColor = Color.NavajoWhite;
            dgvJobQueue.FirstDisplayedScrollingRowIndex = row.Index;

            if (_lastActiveJobIndex >= 0)
            {
                row = dgvJobQueue.Rows[_lastActiveJobIndex];
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Style.BackColor = Color.Empty;
            }
            _lastActiveJobIndex = index;
        }



        public void CountJobs()
        {
            sblJobCount.Text = "Jobs: " + (dgvJobQueue.RowCount - 1);
            saveJobQueueListToolStripMenuItem.Enabled = (dgvJobQueue.RowCount > 1 ? true : false); // 1 because the empty row!
            startQueueToolStripMenuItem.Enabled = saveJobQueueListToolStripMenuItem.Enabled; // No jobs -> No Start
        }



        public void RenumberJobs()
        {
            for (var rowIndex = 0; rowIndex < dgvJobQueue.RowCount - 1; rowIndex++)
                dgvJobQueue.Rows[rowIndex].HeaderCell.Value = string.Format("{0}", rowIndex + 1);
        }


        private string ConvertActiveState(string State)
        {
            // Check if state is given in "true/false" or "1/0" an convert them into "✓/X"
            State = StringHelper.TrimString(State);
            State = State.ToUpper();
            if (State == "TRUE" || State == "1")
                return IsActive;
            else if (State == "FALSE" || State == "0")
                return IsInactive;

            return State; // Already IsActive or IsInactive!
        }
        #endregion



        #region Listview-Methods
        public void AddJob(string iniPath, string swpPath, string savePath)
        {
            AddJob(IsActive, iniPath, swpPath, savePath);
        }



        public void AddJob(bool activeState, string iniPath, string swpPath, string savePath)
        {
            // Short if decides the first argument
            AddJob((activeState ? IsActive : IsInactive), iniPath, swpPath, savePath);
        }



        public void AddJob(string activeState, string iniPath, string swpPath, string savePath)
        {
            if (activeState == null || activeState == "") throw new ArgumentNullException("activeState");
            if (iniPath == null || iniPath == "") throw new ArgumentNullException("iniPath");
            if (swpPath == null || swpPath == "") throw new ArgumentNullException("swpPath");
            if (savePath == null || savePath == "") throw new ArgumentNullException("savePath");


            activeState = ConvertActiveState(activeState);
            dgvJobQueue.Rows.Add(activeState, iniPath, swpPath, savePath);
            var row = dgvJobQueue.Rows[dgvJobQueue.RowCount - 2];
            foreach (DataGridViewCell cell in row.Cells)
                ValidateJobQueueEntryOfCell(cell);

            CountJobs();
            RenumberJobs();
            saveJobQueueListToolStripMenuItem.Enabled = true;
        }


        public void MoveSelectedJobsOnePositionUp()
        {
            if (dgvJobQueue.RowCount - 1 == 0) // No items
                return;

            var selectedRows = dgvJobQueue.SelectedRows;
            if (selectedRows.Count <= 0)
                return;

            if (selectedRows[0].Index <= 0)
                return;

            DataGridViewRow bufferRow;
            int rowIndex = 0;
            for (var rowIterator = 0; rowIterator < selectedRows.Count; rowIterator++)
            {
                bufferRow = selectedRows[rowIterator];
                rowIndex = bufferRow.Index;
                dgvJobQueue.Rows.RemoveAt(rowIndex);
                dgvJobQueue.Rows.Insert(rowIndex - 1, bufferRow);
                selectedRows[rowIterator].Selected = true;
            }
            RenumberJobs();
        }


        private void MoveSelectedJobsOnePositionDown()
        {
            if (dgvJobQueue.RowCount - 1 == 0) // No items
                return;

            var selectedRows = dgvJobQueue.SelectedRows;
            if (selectedRows.Count <= 0)
                return;

            if (selectedRows[selectedRows.Count - 1].Index >= dgvJobQueue.RowCount - 2) // Count - 1 (Empty row) - 1 (Count to Index)
                return;

            DataGridViewRow bufferRow;
            int rowIndex = 0;
            for (var rowIterator = 0; rowIterator < selectedRows.Count; rowIterator++)
            {
                bufferRow = selectedRows[rowIterator];
                rowIndex = bufferRow.Index;
                dgvJobQueue.Rows.RemoveAt(rowIndex);
                dgvJobQueue.Rows.Insert(rowIndex + 1, bufferRow);
                selectedRows[rowIterator].Selected = true;
            }

            RenumberJobs();
        }
        #endregion



        #region Single job done
        private void OneJobDone()
        {
            // Robert-Mode handling
            if (_robertMode && !_stopRequested)
            {
                _mainFrame.Invoke(new Action(() =>
                {
                    var robertDialog = new JobQueueRobertDialog();
                    var robertResult = robertDialog.ShowCustom();
                    if (robertResult == RobertDialogResult.StopQueue)
                        _robertRequestedStop = true;
                    if (robertResult == RobertDialogResult.RepeatJob)
                        _robertRequestedRepeat = true;
                }));
            }


            // Queue-Handling
            if (_stopRequested || _robertRequestedStop)
            {
                _robertRequestedStop = false; // Reset Robermode Stop-Request
                // Saving was done before this (OneJobDone) was called! So the MessageBox have only to show up, when the stop was requested during the job-exectution (stop-queue) not after it was finished (Robert-Mode Abortion)!
                if (_stopRequested)
                    MessageBox.Show("Stop current job or queue requested by the user. Actual results were saved to the given folder.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (_robertRequestedStop)
                    MessageBox.Show("Stop for the queue requested by robert.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                UnloadJob();
                _jobQueueStatus.JobQueueStopped();
                UpdateUI(_jobQueueStatus.State);
                return;
            }


            // Check Robert Requested Repeat flag and reset flag when it's set (jump over load next job and so on!
            if (_robertRequestedRepeat)
                _robertRequestedRepeat = false;
            // When no robertRequestRepeat flag is set make standard-routine and increment to the next job
            else
            {
                // Looking for the next active job to do!
                string cellBuffer;
                while (!_jobQueueStatus.UpdateToNextJobIndex())
                {
                    // Break from while when a active job was found
                    cellBuffer = dgvJobQueue.Rows[_jobQueueStatus.CurrentJobIndex].Cells[(int)JobQueueIndicies.Status].Value as string;
                    if (cellBuffer == IsActive)
                        break;
                    _skippedJobs++;
                }

                // Last job was done -> Stop here
                //if (_jobQueueStatus.UpdateToNextJobIndex())
                if (_jobQueueStatus.JobIndexOverflow)
                {
                    //if (dgvJobQueue.Rows[_jobQueueStatus.CurrentJobIndex - 1].Cells[(int)JobQueueIndicies.Status].Text == _isInactive)
                    //    _skippedJobs++;
                    MessageBox.Show(string.Format("Work done\n{0} jobs skipped!", _skippedJobs), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UnloadJob();
                    _jobQueueStatus.JobQueueStopped();
                    UpdateUI(_jobQueueStatus.State);
                    return;
                }

                // Go on with next job
                LoadJob(_jobQueueStatus.CurrentJobIndex);
            }
            StartJob();
        }
        #endregion



        #region JobQueueStatus (Load, Unload, Start, Stop, ...)
        private void LoadJob(int index)
        {
            var iniFullFilename = dgvJobQueue.Rows[index].Cells[(int)JobQueueIndicies.IniFile].Value as string;
            var swpFullFilename = dgvJobQueue.Rows[index].Cells[(int)JobQueueIndicies.SwpFile].Value as string;
            var saveFolderFullPath = dgvJobQueue.Rows[index].Cells[(int)JobQueueIndicies.SavFold].Value as string;

            _mainFrame.OpenIni(iniFullFilename);
            _mainFrame.OpenSweep(swpFullFilename);
            _mainFrame.SaveFolderFullPath = saveFolderFullPath;

            MarkActiveJobQueueEntry(index);
            sblJobCount.Text = "Job: " + (index + 1).ToString() + "/" + (dgvJobQueue.RowCount - 1).ToString();
        }


        private void UnloadJob()
        {
            MarkActiveJobQueueEntry(-1); // Mark all as white!
            CountJobs();
            _mainFrame.CurrentJobDone -= OneJobDone; // Remove callback
            _mainFrame.SaveFolderFullPath = null; // Remove target savefolder
        }


        private void StartJob()
        {
            //MarkActiveJobQueueEntry(_jobQueueStatus.CurrentJobIndex);
            _stopRequested = false;
            _jobQueueStatus.JobQueueStarted();
            UpdateUI(_jobQueueStatus.State);
            _mainFrame.ExternalStart();
        }


        private void StopJob()
        {
            _stopRequested = true;
            _mainFrame.ExternalStop();
        }
        #endregion



        #region UI-Helper (element (de)activation)
        private void UpdateUI(QueueState queueState)
        {
            switch (queueState)
            {
                case QueueState.Stopped:
                    dgvJobQueue.ReadOnly = false;
                    sbbRemoveJob.Enabled = true;
                    sbbClearAllJobs.Enabled = true;
                    sbbMoveUp.Enabled = true;
                    sbbMoveDown.Enabled = true;
                    startQueueToolStripMenuItem.Enabled = true;
                    pauseQueueToolStripMenuItem.Enabled = false; // Isn't implemented. Maybe it never will be.
                    stopQueueToolStripMenuItem.Enabled = false;
                    loadJobQueueListToolStripMenuItem.Enabled = true;
                    break;

                case QueueState.Running:
                    dgvJobQueue.ReadOnly = true;
                    sbbRemoveJob.Enabled = false;
                    sbbClearAllJobs.Enabled = false;
                    sbbMoveUp.Enabled = false;
                    sbbMoveDown.Enabled = false;
                    startQueueToolStripMenuItem.Enabled = false;
                    pauseQueueToolStripMenuItem.Enabled = false; // Isn't implemented. Maybe it never will be.
                    stopQueueToolStripMenuItem.Enabled = true;
                    loadJobQueueListToolStripMenuItem.Enabled = false;
                    break;

                default:
                    break;
            }
        }


        private void UpdateStatusbarButtons()
        {
            var rows = dgvJobQueue.SelectedRows;
            if (rows.Count == 0) // Nothing selected
            {
                sbbCopyJob.Enabled = false;
                sbbRemoveJob.Enabled = false;
                sbbMoveUp.Enabled = false;
                sbbMoveDown.Enabled = false;
            }
            else
            {
                sbbCopyJob.Enabled = true;
                sbbRemoveJob.Enabled = true;
                sbbMoveUp.Enabled = true;
                sbbMoveDown.Enabled = true;
            }
        }



        public void CountErrorsInJobQueueEntries()
        {
            _listErrorCount = 0;
            if (dgvJobQueue.RowCount < 1) // 1 because the empty-row
                return;

            for (var rowIndex = 0; rowIndex < dgvJobQueue.RowCount - 1; rowIndex++)
            {
                foreach (DataGridViewCell cell in dgvJobQueue.Rows[rowIndex].Cells)
                    _listErrorCount += (ValidateJobQueueEntryOfCell(cell) ? 0 : 1); // Add 1 error when a value wasn't validated!
            }
        }



        /// <summary>
        /// Returns true when a value is valid. Otherwise you get false or an Exception!
        /// </summary>
        /// <param name="Cell"></param>
        /// <returns></returns>
        private bool ValidateJobQueueEntryOfCell(DataGridViewCell Cell)
        {
            string pathBuffer;
            if (Cell.Value == null || Cell.Value as string == "")
            {
                Cell.Style.BackColor = Color.DarkOrange;
                Cell.ToolTipText = "Missing value";
                return false;
            }

            switch ((JobQueueIndicies)Cell.ColumnIndex)
            {
                case JobQueueIndicies.Status:
                    //var currVal = ConvertActiveState(Cell.Value as string);
                    int currVal;
                    var valConversionOk = int.TryParse((string)Cell.Value, out currVal);
                    //if (currVal != IsActive && currVal != IsInactive)
                    if (!valConversionOk && currVal <= 0)
                    {
                        Cell.Value = 0;
                        Cell.ToolTipText = "Unknown statevalue! Using inactive by default";
                        Cell.Style.BackColor = Color.Crimson;
                        return false;
                    }
                    // else
                    Cell.Value = currVal;
                    Cell.ToolTipText = "";
                    Cell.Style.BackColor = Color.Empty;
                    break;

                case JobQueueIndicies.IniFile:
                case JobQueueIndicies.SwpFile:
                    pathBuffer = Cell.Value as string;
                    if (!Path.IsPathRooted(pathBuffer))
                        pathBuffer = Path.Combine(Application.StartupPath, pathBuffer);

                    if (!File.Exists(pathBuffer))
                    {
                        Cell.ToolTipText = "Can't find file! Please check your path.";
                        Cell.Style.BackColor = Color.Crimson;
                        return false;
                    }
                    // else
                    Cell.ToolTipText = "";
                    Cell.Style.BackColor = Color.Empty;
                    break;

                case JobQueueIndicies.SavFold:
                    pathBuffer = Cell.Value as string;
                    if (!Path.IsPathRooted(pathBuffer))
                        pathBuffer = Path.Combine(Application.StartupPath, pathBuffer);

                    if (!Directory.Exists(pathBuffer))
                    {
                        var createFolderResult = MessageBox.Show("Folder doesn't exist. Do you wan't create this folder now?", "Nonexisting Folder!",
                                                                 MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (createFolderResult == DialogResult.Yes)
                        {
                            Directory.CreateDirectory(pathBuffer);
                        }
                        else
                        {
                            Cell.ToolTipText = "Can't find folder! Please check your path.";
                            Cell.Style.BackColor = Color.Crimson;
                            return false;
                        }
                    }
                    // else
                    Cell.ToolTipText = "";
                    Cell.Style.BackColor = Color.Empty;
                    break;

                default:
                    throw new InvalidExpressionException("Non-valid column-index: " + Cell.ColumnIndex);
            }

            return true; // Cellvalue ok
        }


        private void ColorizeRobertModeButton(bool NewState)
        {
            _robertMode = NewState;
            if (_robertMode)
                sbb_RobertMode.BackColor = Color.LimeGreen;
            else
                sbb_RobertMode.BackColor = Color.Red;
        }
        #endregion



        #region UI-Events
        private void dgvJobQueue_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dgvJobQueue.CurrentCell;
            if (cell == null)
                return;

            foreach (DataGridViewCell cellToValidate in cell.OwningRow.Cells)
                ValidateJobQueueEntryOfCell(cellToValidate);

            CountJobs();
            RenumberJobs();
        }



        private void dgvJobQueue_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dgvJobQueue.ReadOnly)
                return;

            var item = dgvJobQueue.CurrentCell;
            if (item == null)
                return;

            var picker = new CommonOpenFileDialog();
            CommonFileDialogResult pickerResult = CommonFileDialogResult.None;
            switch ((JobQueueIndicies)item.ColumnIndex)
            {
                case JobQueueIndicies.Status:
                    var currVal = (string)item.Value;
                    if (currVal == null)
                        currVal = "1"; // default state is active

                    currVal = ConvertActiveState(currVal);
                    item.Value = (currVal == IsActive ? IsInactive : IsActive);
                    break;

                case JobQueueIndicies.IniFile:
                    picker.Title = "Select ini...";
                    picker.Filters.Add(new CommonFileDialogFilter("Ini-Files", "*.ini"));
                    picker.EnsureFileExists = true;
                    if (_mainFrame.LastOpenIniPath != null && _mainFrame.LastOpenIniPath != "")
                        picker.InitialDirectory = _mainFrame.LastOpenIniPath;
                    pickerResult = picker.ShowDialog();
                    if (pickerResult == CommonFileDialogResult.Cancel)
                        break;
                    _mainFrame.LastOpenIniPath = Path.GetDirectoryName(picker.FileName);
                    item.Value = picker.FileName;
                    break;

                case JobQueueIndicies.SwpFile:
                    picker.Title = "Select sweep...";
                    picker.Filters.Add(new CommonFileDialogFilter("Sweep-Files", "*.swp"));
                    picker.EnsureFileExists = true;
                    if (_mainFrame.LastOpenSweepPath != null && _mainFrame.LastOpenSweepPath != "")
                        picker.InitialDirectory = _mainFrame.LastOpenSweepPath;
                    pickerResult = picker.ShowDialog();
                    if (pickerResult == CommonFileDialogResult.Cancel)
                        break;
                    _mainFrame.LastOpenSweepPath = Path.GetDirectoryName(picker.FileName);
                    item.Value = picker.FileName;
                    break;

                case JobQueueIndicies.SavFold:
                    picker.Title = "Select folder...";
                    picker.IsFolderPicker = true;
                    picker.EnsurePathExists = true;
                    if (_mainFrame.LastSavePath != null && _mainFrame.LastSavePath != "")
                        picker.InitialDirectory = _mainFrame.LastSavePath;
                    pickerResult = picker.ShowDialog();
                    if (pickerResult == CommonFileDialogResult.Cancel)
                        break;
                    _mainFrame.LastSavePath = picker.FileName;
                    item.Value = picker.FileName;
                    break;

                default:
                    throw new InvalidExpressionException("Non-valid column-index: " + item.ColumnIndex);
            }

            ValidateJobQueueEntryOfCell(item);
            CountJobs();
            RenumberJobs();
            BringToFront();
        }



        private void sbbCopyJob_ButtonClick(object sender, EventArgs e)
        {
            var rows = dgvJobQueue.SelectedRows;
            if (rows == null)
                return;

            if (rows.Count <= 0)
                return;

            if (rows[rows.Count - 1].Index > dgvJobQueue.RowCount - 2) // Count - 1 (-> Index) - 1 (Empty row)
                return;

            foreach (DataGridViewRow row in rows) // Create a copy of each row and attach to list
                AddJob(row.Cells[(int)JobQueueIndicies.Status].Value as string,
                       row.Cells[(int)JobQueueIndicies.IniFile].Value as string,
                       row.Cells[(int)JobQueueIndicies.SwpFile].Value as string,
                       row.Cells[(int)JobQueueIndicies.SavFold].Value as string);
        }



        private void sbRemoveJob_ButtonClick(object sender, EventArgs e)
        {
            var rows = dgvJobQueue.SelectedRows;
            if (rows == null)
                return;

            if (rows.Count <= 0)
                return;

            if (rows[rows.Count - 1].Index > dgvJobQueue.RowCount - 2) // Count - 1 (-> Index) - 1 (Empty row)
                return;

            for (var rowIterator = rows.Count - 1; rowIterator >= 0; rowIterator--)
                dgvJobQueue.Rows.RemoveAt(rows[rowIterator].Index);

            CountJobs();
            RenumberJobs();
        }



        private void dgvJobQueue_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CountJobs();
            RenumberJobs();
        }



        private void sbClearAllJobs_ButtonClick(object sender, EventArgs e)
        {
            ClearAllJobs();
        }


        private void ClearAllJobs()
        {
            dgvJobQueue.Rows.Clear();
            CountJobs();
            RenumberJobs();
        }



        private void startQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_mainFrame.OperationStatus != null && _mainFrame.OperationStatus.Status != Measurement.MeasurementStatus.Stopped)
            {
                MessageBox.Show("FEMDAQ runs currently another job.", "Can't start JobQueue", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _skippedJobs = 0; // Reset counter at start
            _lastActiveJobIndex = -1; // Reset 
            _mainFrame.CurrentJobDone += OneJobDone; // Add callback

            _jobQueueStatus = new JobQueueStatus(dgvJobQueue.RowCount - 1);
            if (_jobQueueStatus.JobIndexOverflow)
            {
                MessageBox.Show("There have to be more than 0 jobs entered to start the jobqueue. \nStart aborted.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Looking for the first active job to do!
            do
            {
                // Break from while when a active job was found
                var currentJobStatus = dgvJobQueue.Rows[_jobQueueStatus.CurrentJobIndex].Cells[(int)JobQueueIndicies.Status].Value as string;
                if (currentJobStatus == IsActive)
                    break;
                _skippedJobs++;
            } while (!_jobQueueStatus.UpdateToNextJobIndex());

            if (_jobQueueStatus.JobIndexOverflow)
            {
                MessageBox.Show(string.Format("All {0} jobs skipped!", _skippedJobs), "Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _jobQueueStatus.JobQueueStopped();
                UpdateUI(_jobQueueStatus.State);
                return;
            }

            CountErrorsInJobQueueEntries();
            if (_listErrorCount > 0)
            {
                MessageBox.Show(string.Format("There are {0} invalid jobqueue-entries. You have to fix them before starting the queue!", _listErrorCount), "Invalid entries detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LoadJob(_jobQueueStatus.CurrentJobIndex);
            StartJob();
        }



        private void stopQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopJob();
        }



        private void saveJobQueueListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDiag = new SaveFileDialog();
            fileDiag.Filter = "Job-Queue-List|*.jql";
            if (_lastFilePath != null && _lastFilePath != "")
                fileDiag.InitialDirectory = _lastFilePath;
            var result = fileDiag.ShowDialog();
            if (result != DialogResult.OK)
                return;

            var jqlContent = "# Job-Queue-List v1.0" + Environment.NewLine;
            jqlContent += "# Active, Ini-Path, Swp-Path, Save-Folder" + Environment.NewLine;
            DataGridViewRow currentItem = null;
            for (var jobQueueLineIndex = 0; jobQueueLineIndex < dgvJobQueue.RowCount - 1; jobQueueLineIndex++)
            {
                currentItem = dgvJobQueue.Rows[jobQueueLineIndex];
                jqlContent += currentItem.Cells[(int)JobQueueIndicies.Status].Value as string + ", "
                             + currentItem.Cells[(int)JobQueueIndicies.IniFile].Value as string + ", "
                             + currentItem.Cells[(int)JobQueueIndicies.SwpFile].Value as string + ", "
                             + currentItem.Cells[(int)JobQueueIndicies.SavFold].Value as string + Environment.NewLine;
            }
            jqlContent += Environment.NewLine;

            var streamWriter = new StreamWriter(fileDiag.FileName);
            streamWriter.Write(jqlContent);
            streamWriter.Close();
            streamWriter.Dispose();
            _lastFilePath = Path.GetDirectoryName(fileDiag.FileName);
        }



        private void loadJobQueueListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDiag = new OpenFileDialog();
            fileDiag.Filter = "Job-Queue-List|*.jql";
            if (_lastFilePath != null && _lastFilePath != "")
                fileDiag.InitialDirectory = _lastFilePath;
            fileDiag.ShowDialog();
            if (!File.Exists(fileDiag.FileName))
            {
                MessageBox.Show("File doesn't exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _lastFilePath = Path.GetDirectoryName(fileDiag.FileName);

            ClearAllJobs();

            var streamReader = new StreamReader(fileDiag.FileName);
            var jqlContent = streamReader.ReadToEnd();
            streamReader.Close();
            streamReader.Dispose();

            var jqlLines = jqlContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string[] jqlLineEntries = null;
            for (var jqlLineIndex = 0; jqlLineIndex < jqlLines.Length; jqlLineIndex++)
            {
                if (jqlLines[jqlLineIndex].StartsWith("#")) // Ignore commentaries
                    continue;

                jqlLineEntries = StringHelper.TrimArray(jqlLines[jqlLineIndex].Split(new char[] { ',' }));
                if (jqlLineEntries.Length != 4)
                    throw new FormatException(string.Format("Wrong Job-Queue-List format. Can't parse job-queue-entry {0}!", jqlLineIndex + 1));

                AddJob(jqlLineEntries[(int)JobQueueIndicies.Status],
                       jqlLineEntries[(int)JobQueueIndicies.IniFile],
                       jqlLineEntries[(int)JobQueueIndicies.SwpFile],
                       jqlLineEntries[(int)JobQueueIndicies.SavFold]);
            }
        }
        #endregion

        #region UI: Switch jobqueue entries
        private void dgvJobQueue_SelectionChanged(object sender, EventArgs e)
        {
            if (_jobQueueStatus == null ||
                (_jobQueueStatus != null && _jobQueueStatus.State == QueueState.Stopped)) // Everything is ok to enable the buttons for list-modification (Copy, remove, move up/down)
                UpdateStatusbarButtons();
        }


        private void sbbMoveUp_ButtonClick(object sender, EventArgs e)
        {
            MoveSelectedJobsOnePositionUp();
        }

        private void sbbMoveDown_ButtonClick(object sender, EventArgs e)
        {
            MoveSelectedJobsOnePositionDown();
        }
        #endregion

        private void sbb_RobertMode_ButtonClick(object sender, EventArgs e)
        {
            ColorizeRobertModeButton(!_robertMode);
        }
    }
}
