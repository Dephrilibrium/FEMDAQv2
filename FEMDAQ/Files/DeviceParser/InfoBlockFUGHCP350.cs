using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Own usings
using FEMDAQ.StaticHelper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace Files.Parser
{

    public class InfoBlockFUGHCP350 : InfoBlock
    {
        public CommonParser Common { get; private set; }
        public GpibParser Gpib { get; private set; }
        public SourceParser Source { get; private set; }



        public InfoBlockFUGHCP350(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            Common = new CommonParser(infoBlock);
            Gpib = new GpibParser(infoBlock);
            Source = new SourceParser(infoBlock);
        }


        public void Dispose()
        {
            Common.Dispose();
            Gpib.Dispose();
            Source.Dispose();
        }

    }
}
