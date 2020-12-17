using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMDAQ.Measurement
{
    public class OperationRemaining
    {
        private TimeSpan _duration;
        private DateTime _stopTime;

        public OperationRemaining(int DurationInMilliseconds)
        {
            if (DurationInMilliseconds < 0)
                throw new ArgumentOutOfRangeException("Duration must be a value > 0");

            _duration = new TimeSpan(0, 0, 0, 0, DurationInMilliseconds);
            _stopTime = DateTime.Now.Add(_duration);
        }

        public string StopTime
        {
            get
            {
                return _stopTime.ToString("HH:mm");
            }
        }

        public string RemainingDuration
        {
            get
            {
                return _duration.ToString("hh:mm:ss");
            }
        }

        public void DecrementTimeByGivenMilliseconds(int Milliseconds)
        {
            _duration = _duration.Subtract(new TimeSpan(0, 0, 0, 0, Milliseconds));
        }
    }
}
