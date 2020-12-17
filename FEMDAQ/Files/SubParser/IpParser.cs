using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;

namespace Files.Parser
{
    public class IpParser
    {
        public string IP { get; set; }



        public IpParser(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            ParseIp(StringHelper.FindStringWhichStartsWith(infoBlock, "Ip="));
        }



        private void ParseIp(string info)
        {
            IP = ParseHelper.ParseStringValueFromLineInfo(info);
            if (IP == "") throw new FormatException("Line: " + info);
        }
    }
}
