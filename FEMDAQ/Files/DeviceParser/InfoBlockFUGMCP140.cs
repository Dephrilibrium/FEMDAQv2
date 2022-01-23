using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Own usings
using FEMDAQ.StaticHelper;



namespace Files.Parser
{

    public class InfoBlockFUGMCP140
    {
        public CommonParser Common { get; private set; }
        public GpibParser Gpib { get; private set; }
        public SourceParser Source { get; private set; }



        public InfoBlockFUGMCP140(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            Common = new CommonParser(infoBlock);
            Gpib = new GpibParser(infoBlock);
            Source = new SourceParser(infoBlock);
        }

    }
}
