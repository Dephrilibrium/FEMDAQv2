using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace Files.Parser
{
    public class InfoBlockMOVE1250 : InfoBlockInterface
    {
        public CommonParser Common { get; private set; }
        public ComParser ComPort { get; private set; }
        public SourceParser Source { get; private set; }

        public int ResponseTime { get; private set; }


        public InfoBlockMOVE1250(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            string lineInfo = null;

            Common = new CommonParser(infoBlock, null, null, null, "CustomName=", "Comment=");
            ComPort = new ComParser(infoBlock); // Normally throws NULL exception since MOVE don't have a baudrate parameter anymore!
            ComPort.Baudrate = 300; // Baudrate is fixed for this device to 300 BAUD
            Source = new SourceParser(infoBlock, "SourceNode=", null);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "ResponseTime=");
            if (lineInfo == null)
                ResponseTime = 250;
            else
                ResponseTime = (int)ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);
        }


        public void Dispose()
        {
            Common.Dispose();
            ComPort.Dispose();
            Source.Dispose();
        }
    }
}
