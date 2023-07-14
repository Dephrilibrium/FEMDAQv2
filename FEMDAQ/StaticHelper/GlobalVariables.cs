using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMDAQ.StaticHelper
{
    internal static class GlobalVariables
    {
        static public FEMDAQ MainFrame { get; set; } // Keep it for the moment with set, but not a clean thing here! Needs to be changed to private set later on
    }
}
