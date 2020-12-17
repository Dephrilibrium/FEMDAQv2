using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FEMDAQ.JobQueue
{
    public enum RobertDialogResult { NextJob, RepeatJob, StopQueue };

    public partial class JobQueueRobertDialog : Form
    {
        public JobQueueRobertDialog()
        {
            InitializeComponent();

            bNextJob.DialogResult = DialogResult.Yes; // Used for NextJob
            bRepeatJob.DialogResult = DialogResult.Retry; // Used for RepeatJob
            bAbortQueue.DialogResult = DialogResult.Abort; // Used for StopQueue
        }

        public RobertDialogResult ShowCustom()
        {
            var icon = SystemIcons.Information;
            pictureBox1.Image = icon.ToBitmap();
            var stdDialogResult = ShowDialog();

            if (stdDialogResult == DialogResult.Retry)
                return RobertDialogResult.RepeatJob;
            if (stdDialogResult == DialogResult.Abort)
                return RobertDialogResult.StopQueue;

            return RobertDialogResult.NextJob;
        }
    }
}
