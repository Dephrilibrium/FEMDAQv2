using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMDAQ.StaticHelper
{
    class ParseHelper
    {
        public static IEnumerable<string> ParseBlockIdentifier(string info)
        {
            if (info == null) throw new ArgumentNullException("info");

            var splitInfo = info.Split(new char[] { '[', '|', ']' }, StringSplitOptions.RemoveEmptyEntries);
            return splitInfo;
        }



        public static string ParseStringValueFromLineInfo(string info)
        {
            if (info == null) throw new ArgumentNullException("info");

            var parseHelper = info.Split(new char[] { '=' }, StringSplitOptions.None);
            if (parseHelper == null || parseHelper.Length < 2)
                throw new FormatException("Line: " + info);

            return parseHelper[1];
        }


        public static double ParseDoubleValueFromLineInfo(string info)
        {
            var stringValue = ParseStringValueFromLineInfo(info);
            if (stringValue == "") throw new FormatException("Line: " + info);

            return double.Parse(stringValue);
        }


        public static bool ParseBoolValueFromLineInfo(string info, bool valueOnNullInfo)
        {
            if (info == null)
                return valueOnNullInfo;

            var stringVal = ParseStringValueFromLineInfo(info).ToUpper();
            if (stringVal == "FALSE" || stringVal == "0")
                return false;
            else if (stringVal != "FALSE" || stringVal != "TRUE")
                return true;

            throw new FormatException("Line: " + info); // No valid 
        }
    }
}
