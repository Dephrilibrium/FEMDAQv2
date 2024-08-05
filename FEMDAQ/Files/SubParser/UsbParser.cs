using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files.Parser
{
    public class UsbParser
    {
        public string USBAddress { get; set; }

        public UsbParser(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            ParseUsbAddress(StringHelper.FindStringWhichStartsWith(infoBlock, "Usb="));
        }

        public void Dispose()
        {

        }


        private void ParseUsbAddress(string lineInfo)
        {
            USBAddress = ParseHelper.ParseStringValueFromLineInfo(lineInfo);
            if (USBAddress == "") throw new FormatException("Line: " + lineInfo);
        }
    }
}
