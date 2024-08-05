using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;

namespace Files.Parser
{
    public class GpibParser
    {
        public int GpibBoardNumber { get; private set; }
        public int GpibPrimaryAdress { get; private set; }
        public int GpibSecondaryAdress { get; private set; }



        public GpibParser(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            ParseGpibAddress(StringHelper.FindStringWhichStartsWith(infoBlock, "GpibAddr="));
        }

        public void Dispose()
        {

        }




        private void ParseGpibAddress(string info)
        {
            var gpibAddress = ParseHelper.ParseStringValueFromLineInfo(info);
            if (gpibAddress == "")
                throw new FormatException("Line: " + info);

            var splitAddress = gpibAddress.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                GpibBoardNumber = int.Parse(splitAddress[0]);
                GpibPrimaryAdress = int.Parse(splitAddress[1]);
                GpibSecondaryAdress = int.Parse(splitAddress[2]);
            }
            catch(Exception)
            {
                throw new FormatException("File: " + info);
            }
        }

    }
}
