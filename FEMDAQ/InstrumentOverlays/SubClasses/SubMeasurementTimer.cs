using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Forms.DataVisualization.Charting;

namespace Instrument.LogicalLayer.SubClasses
{
    public class SubMeasurementTimer
    {
        public bool IsElapsed { get; private set; }
        private Timer _timer = null;

        public SubMeasurementTimer(int interval, bool oneShot = true)
        {
            if (interval < 0) throw new ArgumentOutOfRangeException(string.Format("SubMeasurementTimer does not accept negative numbers - interval: {0} < 0", interval));

            if (interval > 0)
            {
                _timer = new Timer(interval); // Contains guard-clause for negative numbers
                _timer.AutoReset = !oneShot;
                _timer.Elapsed += (sender, args) => IsElapsed = true;
            }

            IsElapsed = false;
        }

        public void Dispose()
        {
            if (_timer != null) { _timer.Dispose(); }
        }

        public void Start()
        {
            if (_timer == null)
            {
                IsElapsed = true;
                return;
            }

            _timer.Start();
        }
        public void Stop()
        {
            if(_timer != null)
                return;

            _timer.Stop();
        }

        public void ResetElapsed() => IsElapsed = false;
    }
}
