using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMDAQ.JobQueue
{
    public enum QueueState { Error = -1, Stopped, Paused, Running }


    public class JobQueueStatus
    {
        public QueueState State { get; private set; }
        public int CurrentJobIndex { get; private set; }
        public int AmountAllJobs { get; private set; }
        public bool JobIndexOverflow { get; private set; }


        public JobQueueStatus(int jobs)
        {
            State = QueueState.Stopped;
            AmountAllJobs = jobs;
            CurrentJobIndex = 0;
            if (jobs == 0)
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
            CurrentJobIndex++;
            if (CurrentJobIndex >= AmountAllJobs)
                JobIndexOverflow = true;

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
