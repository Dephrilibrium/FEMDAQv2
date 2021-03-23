using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FEMDAQ.JobQueue
{
    public enum QueueState { Error = -1, Stopped, Paused, Running }


    public class JobQueueStatus
    {
        public QueueState State { get; private set; }
        public int CurrentJobIndex { get; private set; }
        //public int AmountAllJobs { get; private set; }
        private DataGridView _jobQueue;
        public bool JobIndexOverflow { get; private set; }


        public JobQueueStatus(DataGridView jobQueue)
        {
            State = QueueState.Stopped;
            //AmountAllJobs = jobs;
            _jobQueue = jobQueue;
            CurrentJobIndex = 0;
            if (_jobQueue.RowCount <= 1)
                JobIndexOverflow = true;
            else
                JobIndexOverflow = false;
        }



        /// <summary>
        /// Updating to the next jobindex. If an overflow is appeared true is given back, otherwise false.
        /// </summary>
        /// <returns></returns>
        public bool UpdateToNextJobIndex()
        {
            var finishedRuns = (int)_jobQueue.Rows[CurrentJobIndex].Cells[(int)JobQueueIndicies.FinishedRuns].Value;
            finishedRuns++;
            _jobQueue.Rows[CurrentJobIndex].Cells[(int)JobQueueIndicies.FinishedRuns].Value = finishedRuns; // Writeback of the current run (not if the current run overflows the amount of runs!)
            var jobRuns = (int)_jobQueue.Rows[CurrentJobIndex].Cells[(int)JobQueueIndicies.JobRuns].Value;


            // Check job-amount overflow
            if (finishedRuns >= jobRuns)
            {
                var jobAmount = (int)_jobQueue.Rows.Count - 1;
                CurrentJobIndex++;
                //if (CurrentJobIndex >= AmountAllJobs)
                if (CurrentJobIndex >= jobAmount)
                    JobIndexOverflow = true;
            }
            return JobIndexOverflow;
        }


        public void JobQueueStarted()
        {
            State = QueueState.Running;
        }


        public void JobQueueStopped()
        {
            State = QueueState.Stopped;
        }
    }
}
