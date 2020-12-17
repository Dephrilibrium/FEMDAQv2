using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows.Forms;

namespace FEMDAQ.JobQueue
{
    public partial class JobQueueAddFrame : Form
    {
        private FEMDAQ _mainFrame = null;
        private JobQueueFrame _jobQueueFrame = null;
        private OpenFileDialog _fileDialog = null;
        //private FolderBrowserDialog _folderDialog = null;
        private CommonOpenFileDialog _folderDialog = null;

        public JobQueueAddFrame(JobQueueFrame jobQueueFrame, FEMDAQ mainFrame)
        {
            InitializeComponent();

            if (mainFrame == null) throw new NullReferenceException("mainFrame");
            if (jobQueueFrame == null) throw new ArgumentNullException("jobQueueFrame");

            _mainFrame = mainFrame;
            _jobQueueFrame = jobQueueFrame;
            _fileDialog = new OpenFileDialog();
            //_folderDialog = new FolderBrowserDialog();
            _folderDialog = new CommonOpenFileDialog("Select folder...");
            _folderDialog.IsFolderPicker = true;
            //_folderDialog.InitialDirectory = Application.StartupPath;


            tbIniFile.Text = Path.Combine(Application.StartupPath, @"default.ini");
            tbSweepFile.Text = Path.Combine(Application.StartupPath, @"sweep\default.swp");
            tbSaveFolder.Text = Path.Combine(Application.StartupPath, @"NonSpecifiedSaveFolder\");
        }



        #region UI-Events
        private void JobQueueAddFrame_Shown(object sender, EventArgs e)
        {
            bAdd.Focus();
        }



        private void tbIniFile_DoubleClick(object sender, EventArgs e)
        {
            if (_fileDialog == null) throw new NullReferenceException("OpenFileDialog");

            _fileDialog.Filter = "Ini-File|*.ini";
            if (_mainFrame.LastOpenIniPath != null && _mainFrame.LastOpenIniPath != "")
                _fileDialog.InitialDirectory = _mainFrame.LastOpenIniPath;
            _fileDialog.ShowDialog();
            if (_fileDialog.FileName == "")
                return;

            _mainFrame.LastOpenIniPath = Path.GetDirectoryName(_fileDialog.FileName); // Save a valid directory
            tbIniFile.Text = _fileDialog.FileName;
        }



        private void tbSweepFile_DoubleClick(object sender, EventArgs e)
        {
            if (_fileDialog == null) throw new NullReferenceException("OpenFileDialog");

            _fileDialog.Filter = "Sweep-File|*.swp;*.sweep";
            if (_mainFrame.LastOpenSweepPath != null && _mainFrame.LastOpenSweepPath != "")
                _fileDialog.InitialDirectory = _mainFrame.LastOpenSweepPath;
            _fileDialog.ShowDialog();
            if (_fileDialog.FileName == "")
                return;

            _mainFrame.LastOpenSweepPath = Path.GetDirectoryName(_fileDialog.FileName); // Save a valid directory
            tbSweepFile.Text = _fileDialog.FileName;
        }



        private void tbSaveFolder_DoubleClick(object sender, EventArgs e)
        {
            if (_folderDialog == null) throw new NullReferenceException("FolderDialog");

            var folderDialogResult = _folderDialog.ShowDialog();
            if (folderDialogResult != CommonFileDialogResult.Ok)
                tbSaveFolder.Text = "";
            else
            {
                _mainFrame.LastSavePath = _folderDialog.FileName; // Save a valid directory
                tbSaveFolder.Text = _folderDialog.FileName;
            }
            //_folderDialog.SelectedPath = Application.StartupPath;
            //_folderDialog.ShowDialog();
            //tbSaveFolder.Text = _folderDialog.SelectedPath;
        }



        private void bAdd_Click(object sender, EventArgs e)
        {
            if (tbIniFile == null) throw new NullReferenceException("tbIniFile");
            if (tbSweepFile == null) throw new NullReferenceException("tbSweepFile");
            if (tbSaveFolder== null) throw new NullReferenceException("tbSaveFolder");

            if (!File.Exists(tbIniFile.Text))
            {
                MessageBox.Show("There must be a valid path to an ini-file.", "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!File.Exists(tbSweepFile.Text))
            {
                MessageBox.Show("There must be a valid path to an sweep-file.", "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!Directory.Exists(tbSaveFolder.Text))
            {
                MessageBox.Show("There must be a valid path to the save-folder.", "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _jobQueueFrame.AddJob(tbIniFile.Text, tbSweepFile.Text, tbSaveFolder.Text);
            Hide();
            _mainFrame.Show();
            _jobQueueFrame.Show();
        }



        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion
    }
}
