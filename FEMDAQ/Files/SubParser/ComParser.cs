using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;

namespace Files.Parser
{
    public class ComParser
    {
        public string ComPort { get; set; }
        public int Baudrate { get; set; }


        public ComParser(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            ParseComPort(StringHelper.FindStringWhichStartsWith(infoBlock, "ComPort"));
            ParseBaudRate(StringHelper.FindStringWhichStartsWith(infoBlock, "Baudrate="));
        }



        private void ParseComPort(string info)
        {
            ComPort = ParseHelper.ParseStringValueFromLineInfo(info);
            if (ComPort == "") throw new FormatException("Line: " + info);
        }

        private void ParseBaudRate(string info)
        {
            var baudString= ParseHelper.ParseStringValueFromLineInfo(info);
            try { Baudrate = int.Parse(baudString); }
            catch (Exception) { throw new ArgumentOutOfRangeException("Line: " + info); }
        }
    }
}
