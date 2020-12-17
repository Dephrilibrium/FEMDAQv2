using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FEMDAQ.JobQueue
{
    enum ListViewIndicies { Status = 0, IniFile, SwpFile, SavFold }

    public partial class JobQueueFrame : Form
    {
        private FEMDAQ _mainFrame = null;

        // Framevariables
        private JobQueueAddFrame _jobQueueAddFrame = null;
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



        public JobQueueFrame(FEMDAQ mainFrame)
        {
            InitializeComponent();

            if (mainFrame == null) throw new NullReferenceException("mainFrame");
            _mainFrame = mainFrame;
            _jobQueueAddFrame = new JobQueueAddFrame(this, _mainFrame);

            lvJobQueue.View = View.Details;
            lvJobQueue.Columns.Clear();
            // Use first column as "isActive" state
            lvJobQueue.Columns.Add("IsActive", "", 20);
            var isJobActiveStatusIcon = new ImageList();
            isJobActiveStatusIcon.Images.Add(Properties.Resources.power_button_off);
            lvJobQueue.SmallImageList = isJobActiveStatusIcon;
            lvJobQueue.Columns[(int)ListViewIndicies.Status].ImageIndex = 0;
            lvJobQueue.Columns[(int)ListViewIndicies.Status].TextAlign = HorizontalAlignment.Center;
            // Create remaining columns
            lvJobQueue.Columns.Add("Ini", "Ini-Files", 20);
            lvJobQueue.Columns.Add("Swp", "Sweep-Files", 20);
            lvJobQueue.Columns.Add("SavFold", "Savefolder", 20);
            lvJobQueue.Items.Clear();
            AdjustColumnSize();
            UpdateUI(QueueState.Stopped);
            UpdateMoveButtons();

            //AddEntriesByDefaultForTesting();
        }


        private void AddEntriesByDefaultForTesting()
        {
            AddJob(IsActive, @".\TestingMultichart\default.ini", @".\TestingMultichart\def.swp", @".\bla1\");
            AddJob(IsActive, @".\TestingMultichart\default.ini", @".\TestingMultichart\def.swp", @".\bla2\");
            AddJob(IsActive, @".\TestingMultichart\default.ini", @".\TestingMultichart\def.swp", @".\bla3\");
            AddJob(IsActive, @".\TestingMultichart\default.ini", @".\TestingMultichart\def.swp", @".\bla4\");
            AddJob(IsActive, @".\TestingMultichart\default.ini", @".\TestingMultichart\def.swp", @".\bla5\");
        }



        public new void Dispose()
        {
            if (_jobQueueAddFrame != null)
                _jobQueueAddFrame.Dispose();

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
        private void AdjustColumnSize()
        {
            if (lvJobQueue.Columns.Count < 1)
                return; // Error: No columns in listview
            // Resize active-column
            lvJobQueue.Columns[0].Width = 32;

            if (lvJobQueue.Columns.Count < 2)
                return; // Error: Only one column found! 
            // Resize reamining columns
            int columnWidth = (lvJobQueue.ClientSize.Width - lvJobQueue.Columns[0].Width) / (lvJobQueue.Columns.Count - 1);
            for (var columnHeader = 1; columnHeader < lvJobQueue.Columns.Count; columnHeader++)
                lvJobQueue.Columns[columnHeader].Width = columnWidth;
        }


        private void MarkActiveJobQueueEntry(int index)
        {
            if (index == -1)
            {
                foreach (ListViewItem item in lvJobQueue.Items)
                    item.BackColor = Color.White;
                return;
            }


            var lvItemCount = lvJobQueue.Items.Count;
            if (index >= lvItemCount)
                return;

            if (index == _lastActiveJobIndex)
                return;

            var lvItem = lvJobQueue.Items[index];
            lvItem.BackColor = Color.NavajoWhite;
            lvItem.EnsureVisible();

            if (_lastActiveJobIndex >= 0)
            {
                lvItem = lvJobQueue.Items[_lastActiveJobIndex];
                lvItem.BackColor = Color.White;
            }
            _lastActiveJobIndex = index;
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

            // Check if state is given in "true/false" or "1/0" an convert them into "✓/X"
            activeState = activeState.ToUpper();
            if (activeState == "TRUE" || activeState == "1")
                activeState = IsActive;
            else if (activeState == "FALSE" || activeState == "0")
                activeState = IsInactive;

            var lvItem = new ListViewItem(new string[] { activeState, iniPath, swpPath, savePath });
            lvJobQueue.Items.Add(lvItem);

            sblJobCount.Text = "Jobs: " + lvJobQueue.Items.Count.ToString();

            saveJobQueueListToolStripMenuItem.Enabled = true;
        }


        public void MoveSelectedJobsOnePositionUp()
        {
            if (lvJobQueue.Items.Count == 0) // No items
                return;

            var indicies = lvJobQueue.SelectedIndices;
            if (indicies.Count == 0) // Nothing marked
                return;

            var minIndex = indicies[0];
            if (minIndex == 0) // Can't shift the most upper entry into nothing!
                return;

            var maxIndex = indicies[indicies.Count - 1];

            var itemOneAbove = lvJobQueue.Items[minIndex - 1];
            lvJobQueue.Items.RemoveAt(minIndex - 1);
            lvJobQueue.Items.Insert(maxIndex, itemOneAbove);
        }


        private void MoveSelectedJobsOnePositionDown()
        {
            if (lvJobQueue.Items.Count == 0) // No items available
                return;

            var indicies = lvJobQueue.SelectedIndices;
            if (indicies.Count == 0) // Nothing marked
                return;

            var maxIndex = indicies[indicies.Count - 1];
            if (maxIndex >= lvJobQueue.Items.Count - 1) // Can't shift last entry into nothing!
                return;

            var minIndex = indicies[0];

            var itemOneBelow = lvJobQueue.Items[maxIndex + 1];
            lvJobQueue.Items.RemoveAt(maxIndex + 1);
            lvJobQueue.Items.Insert(minIndex, itemOneBelow);
        }
        #endregion



        #region Single job done
        private void OneJobDone()
        {
            if (_stopRequested == true)
            {
                MessageBox.Show("Current job was aborted by the user. Actual results were saved to the given folder.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UnloadJob();
                _jobQueueStatus.JobQueueStopped();
                UpdateUI(_jobQueueStatus.State);
                return;
            }

            // Looking for the next active job to do!
            while (!_jobQueueStatus.UpdateToNextJobIndex())
            {
                // Break from while when a active job was found
                if (lvJobQueue.Items[_jobQueueStatus.CurrentJobIndex].SubItems[(int)ListViewIndicies.Status].Text == IsActive)
                    break;
                _skippedJobs++;
            }

            // Last job was done -> Stop here
            //if (_jobQueueStatus.UpdateToNextJobIndex())
            if (_jobQueueStatus.JobIndexOverflow)
            {
                //if (lvJobQueue.Items[_jobQueueStatus.CurrentJobIndex - 1].SubItems[(int)ListViewIndicies.Status].Text == _isInactive)
                //    _skippedJobs++;
                MessageBox.Show(string.Format("Work done\n{0} jobs skipped!", _skippedJobs), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UnloadJob();
                _jobQueueStatus.JobQueueStopped();
                UpdateUI(_jobQueueStatus.State);
                return;
            }

            // Go on with next job
            LoadJob(_jobQueueStatus.CurrentJobIndex);
            StartJob();
        }
        #endregion



        #region JobQueueStatus (Load, Unload, Start, Stop, ...)
        private void LoadJob(int index)
        {
            var iniFullFilename = lvJobQueue.Items[index].SubItems[(int)ListViewIndicies.IniFile].Text;
            var swpFullFilename = lvJobQueue.Items[index].SubItems[(int)ListViewIndicies.SwpFile].Text;
            var saveFolderFullPath = lvJobQueue.Items[index].SubItems[(int)ListViewIndicies.SavFold].Text;

            _mainFrame.OpenIni(iniFullFilename);
            _mainFrame.OpenSweep(swpFullFilename);
            _mainFrame.SaveFolderFullPath = saveFolderFullPath;

            MarkActiveJobQueueEntry(index);
            sblJobCount.Text = "Job: " + (index + 1).ToString() + "/" + lvJobQueue.Items.Count.ToString();
        }


        private void UnloadJob()
        {
            MarkActiveJobQueueEntry(-1); // Mark all as white!
            sblJobCount.Text = "Jobs: " + lvJobQueue.Items.Count.ToString();
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
                    sbbAddJob.Enabled = true;
                    sbbRemoveJob.Enabled = true;
                    sbbClearAllJobs.Enabled = true;
                    startQueueToolStripMenuItem.Enabled = true;
                    pauseQueueToolStripMenuItem.Enabled = false; // Isn't implemented. Maybe it never will be.
                    stopQueueToolStripMenuItem.Enabled = false;
                    break;

                //case QueueState.Paused: // Pausing from jobqueue is not implemented
                //    sbbAddJob.Enabled = false;
                //    sbbRemoveJob.Enabled = false;
                //    sbbClearAllJobs.Enabled = false;
                //    startQueueToolStripMenuItem.Enabled = false;
                //    pauseQueueToolStripMenuItem.Enabled = false;
                //    stopQueueToolStripMenuItem.Enabled = true;
                //    break;

                case QueueState.Running:
                    sbbAddJob.Enabled = false;
                    sbbRemoveJob.Enabled = false;
                    sbbClearAllJobs.Enabled = false;
                    startQueueToolStripMenuItem.Enabled = false;
                    pauseQueueToolStripMenuItem.Enabled = false; // Isn't implemented. Maybe it never will be.
                    stopQueueToolStripMenuItem.Enabled = true;
                    break;

                default:
                    break;
            }
        }


        private void UpdateMoveButtons()
        {
            var indicies = lvJobQueue.SelectedIndices;
            if (indicies.Count == 0) // Nothing selected
            {
                sbbMoveUp.Enabled = false;
                sbbMoveDown.Enabled = false;
            }
            else
            {
                sbbMoveUp.Enabled = true;
                sbbMoveDown.Enabled = true;
            }
        }
        #endregion



        #region UI-Events
        private void sbbAddJob_ButtonClick(object sender, EventArgs e)
        {
            _jobQueueAddFrame.ShowDialog();
        }



        private void sbRemoveJob_ButtonClick(object sender, EventArgs e)
        {
            var selItems = lvJobQueue.SelectedItems;
            foreach (ListViewItem item in selItems)
                lvJobQueue.Items.Remove(item);

            var remainingItemsCount = lvJobQueue.Items.Count;
            sblJobCount.Text = string.Format("Jobs: {0}", remainingItemsCount.ToString());
            if (remainingItemsCount < 1)
                saveJobQueueListToolStripMenuItem.Enabled = false;
        }



        private void sbClearAllJobs_ButtonClick(object sender, EventArgs e)
        {
            ClearAllJobs();
        }


        private void ClearAllJobs()
        {
            lvJobQueue.Items.Clear();
            sblJobCount.Text = string.Format("Jobs: {0}", lvJobQueue.Items.Count.ToString());
            saveJobQueueListToolStripMenuItem.Enabled = false;
        }



        private void startQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (_jobQueueStatus != null)
            //    _jobQueueStatus.Dispose(); // Has no dispose!

            _skippedJobs = 0; // Reset counter at start
            _lastActiveJobIndex = -1; // Reset 
            _mainFrame.CurrentJobDone += OneJobDone; // Add callback

            _jobQueueStatus = new JobQueueStatus(lvJobQueue.Items.Count);
            if (_jobQueueStatus.JobIndexOverflow)
            {
                MessageBox.Show("There have to be more than 0 jobs entered to start the jobqueue. \nStart aborted.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Looking for the first active job to do!
            do
            {
                // Break from while when a active job was found
                if (lvJobQueue.Items[_jobQueueStatus.CurrentJobIndex].SubItems[(int)ListViewIndicies.Status].Text == IsActive)
                    break;
                _skippedJobs++;
            } while (!_jobQueueStatus.UpdateToNextJobIndex());

            if (_jobQueueStatus.JobIndexOverflow)
            {
                MessageBox.Show(string.Format("All {0} jobs skipped!", _skippedJobs), "Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //UnloadJob();
                _jobQueueStatus.JobQueueStopped();
                UpdateUI(_jobQueueStatus.State);
                return;
            }

            LoadJob(_jobQueueStatus.CurrentJobIndex);
            StartJob();
        }



        private void stopQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopJob();
        }



        private void JobQueueFrame_SizeChanged(object sender, EventArgs e)
        {
            AdjustColumnSize();
        }



        private void lvJobQueue_DoubleClick(object sender, EventArgs e)
        {
            var selected = lvJobQueue.SelectedIndices;
            if (selected == null || selected.Count <= 0)
                return;

            var activatedItem = lvJobQueue.Items[selected[0]].SubItems[(int)ListViewIndicies.Status];
            activatedItem.Text = (activatedItem.Text == IsActive ? IsInactive : IsActive);
        }



        private void saveJobQueueListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDiag = new SaveFileDialog();
            fileDiag.Filter = "Job-Queue-List|*.jql";
            if (_lastFilePath != null && _lastFilePath != "")
                fileDiag.InitialDirectory = Path.GetDirectoryName(_lastFilePath);
            var result = fileDiag.ShowDialog();
            if (result != DialogResult.OK)
                return;

            //if (!File.Exists(fileDiag.FileName))
            //    return;

            var jqlContent = "# Job-Queue-List v1.0" + Environment.NewLine;
            jqlContent += "# Active, Ini-Path, Swp-Path, Save-Folder" + Environment.NewLine;
            ListViewItem currentItem = null;
            for (var jobQueueLineIndex = 0; jobQueueLineIndex < lvJobQueue.Items.Count; jobQueueLineIndex++)
            {
                currentItem = lvJobQueue.Items[jobQueueLineIndex];
                jqlContent += currentItem.SubItems[(int)ListViewIndicies.Status].Text + ", "
                             + currentItem.SubItems[(int)ListViewIndicies.IniFile].Text + ", "
                             + currentItem.SubItems[(int)ListViewIndicies.SwpFile].Text + ", "
                             + currentItem.SubItems[(int)ListViewIndicies.SavFold].Text + Environment.NewLine;
            }
            jqlContent += Environment.NewLine;

            var streamWriter = new StreamWriter(fileDiag.FileName);
            streamWriter.Write(jqlContent);
            streamWriter.Close();
            streamWriter.Dispose();
        }



        private void loadJobQueueListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDiag = new OpenFileDialog();
            fileDiag.Filter = "Job-Queue-List|*.jql";
            if (_lastFilePath != null && _lastFilePath != "")
                fileDiag.InitialDirectory = Path.GetDirectoryName(_lastFilePath);
            fileDiag.ShowDialog();
            if (!File.Exists(fileDiag.FileName))
            {
                MessageBox.Show("File doesn't exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _lastFilePath = fileDiag.FileName;

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

                AddJob(jqlLineEntries[(int)ListViewIndicies.Status],
                       jqlLineEntries[(int)ListViewIndicies.IniFile],
                       jqlLineEntries[(int)ListViewIndicies.SwpFile],
                       jqlLineEntries[(int)ListViewIndicies.SavFold]);
            }
        }
        #endregion

        #region UI: Switch jobqueue entries
        private void lvJobQueue_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            UpdateMoveButtons();
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
    }
}
