using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;

namespace Files.Parser  
{
    public class IpParser
    {
        public string IP { get; set; }
        public uint Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public IpParser(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            ParseIp(StringHelper.FindStringWhichStartsWith(infoBlock, "Ip="));
            Port = uint.Parse(ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "Port=")));
            Username = ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "User="));
            Password = ParseHelper.ParseStringValueFromLineInfo(StringHelper.FindStringWhichStartsWith(infoBlock, "Passwd="));
        }



        private void ParseIp(string info)
        {
            IP = ParseHelper.ParseStringValueFromLineInfo(info);
            if (IP == "") throw new FormatException("Line: " + info);
        }
    }
}
