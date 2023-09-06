using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;

namespace Files.Parser  
{
    public class IpParser
    {
        public string IP { get; set; }
        public ushort Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public IpParser(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            ParseIp(StringHelper.FindStringWhichStartsWith(infoBlock, "Ip="));
            var line = StringHelper.FindStringWhichStartsWith(infoBlock, "Port=");
            if (line != null) // When not found -> 0
                Port = ushort.Parse(ParseHelper.ParseStringValueFromLineInfo(line));

            line = StringHelper.FindStringWhichStartsWith(infoBlock, "User=");
            if (line != null) // When not found -> NULL
                Username = ParseHelper.ParseStringValueFromLineInfo(line);

            line = StringHelper.FindStringWhichStartsWith(infoBlock, "Passwd=");
            if (line != null) // When not found -> NULL
                Password = ParseHelper.ParseStringValueFromLineInfo(line);
        }



        private void ParseIp(string info)
        {
            IP = ParseHelper.ParseStringValueFromLineInfo(info);
            if (IP == "") throw new FormatException("Line: " + info);
        }
    }
}
