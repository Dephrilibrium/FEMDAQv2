using Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMDAQ.Measurement
{
    public class OperationStatus
    {
        public MeasurementStatus Status { get; set; }
        public bool InitInProgress { get; private set; }
        public int SweepLinesInFile { get; private set; }
        public int SweepLineIndexInProgress { get; private set; }
        public bool SweepLineIndexOverflow { get; private set; }
        public int RepeatsOfCurrentSweepLine { get; private set; }
        public int RepeatInProgress { get; private set; }
        public int IteratesDone { get; private set; }
        public int NumberOfIterates { get; private set; }

        public TimeSpan RemainingDuration { get; private set; }
        public DateTime EstimatedStopTime { get; private set; }

      
        private readonly IniContent _ini;
        private readonly SweepContent _sweep;




        public OperationStatus(IniContent Ini, SweepContent Sweep)
        {
            if (Ini == null) throw new ArgumentNullException("OperationRemaining - Ini is NULL");
            if (Sweep == null) throw new ArgumentNullException("OperationRemaining - Sweep is NULL");

            _ini = Ini;
            _sweep = Sweep;
            Status = MeasurementStatus.Stopped;

            // Predefine first run
            InitInProgress = true;
            SweepLineIndexInProgress = 0;
            SweepLineIndexOverflow = false;
            SweepLinesInFile = _sweep.Values.Count;
            RepeatInProgress = 0;
            RepeatsOfCurrentSweepLine = (int)_sweep.Values[SweepLineIndexInProgress][0];
            IteratesDone = 0;
            NumberOfIterates = GetAmountOfIterates();

            RemainingDuration = new TimeSpan(0, 0, 0, 0, _ini.TimingInfo.InitialTime + _ini.TimingInfo.IterativeTime * (NumberOfIterates - 1));
            EstimatedStopTime = DateTime.Now.Add(RemainingDuration);
        }

        private int GetAmountOfIterates()
        {
            if (_sweep == null)
                return 0;

            var retVal = 0d;
            foreach(var line in _sweep.Values)
            {
                retVal += line[0];
            }
            return (int)retVal;
        }



        public void ClearInitFlag()
        {
            InitInProgress = false;
        }



        public bool UpdateToNextIterate()
        {
            ClearInitFlag();
            if(IteratesDone == 0)
                RemainingDuration = RemainingDuration.Subtract(new TimeSpan(0, 0, 0, 0, _ini.TimingInfo.InitialTime));
            else
                RemainingDuration = RemainingDuration.Subtract(new TimeSpan(0, 0, 0, 0, _ini.TimingInfo.IterativeTime));
            IteratesDone++;
            return UpdateRepeatInProgress();
        }



        private bool UpdateRepeatInProgress()
        {
            RepeatInProgress++;
            if (RepeatInProgress < RepeatsOfCurrentSweepLine)
                return false;

            UpdateSweepLineInProgress();
            return true;
        }



        public void UpdateSweepLineInProgress()
        {
            SweepLineIndexInProgress++;
            if (SweepLineIndexInProgress < SweepLinesInFile)
            {
                RepeatsOfCurrentSweepLine = RepeatsOfCurrentSweepLine = (int)_sweep.Values[SweepLineIndexInProgress][0];
                RepeatInProgress = 0;
                return;
            }

            SweepLineIndexOverflow = true;
            return;
        }
    }
}
