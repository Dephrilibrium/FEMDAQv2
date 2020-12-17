using Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instrument.LogicalLayer
{
    class AssignSweep
    {
        public static List<double> Assign(SweepContent sweep, string headerIdentifier)
        {
            var headerIndex = IdentifierWasFoundOnIndex(sweep, headerIdentifier);
            if (headerIndex < 0)
                return null;

            return sweep.GetSweepColumn(headerIndex);
        }


        public static int IdentifierWasFoundOnIndex(SweepContent sweep, string headerIdentifier)
        {
            for (var headerIndex = 0; headerIndex < sweep.Header.Count; headerIndex++)
            {
                if (sweep.Header[headerIndex] == headerIdentifier)
                    return headerIndex;
            }
            return -1;
        }


        public static bool IdentifierIsAvailable(SweepContent sweep, string headerIdentifier)
        {
            if (IdentifierWasFoundOnIndex(sweep, headerIdentifier) >= 0)
                return true;

            return false;
        }
    }
}
