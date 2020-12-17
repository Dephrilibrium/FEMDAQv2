using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Own usings
using FEMDAQ.StaticHelper;



namespace Files.Parser
{

    public class InfoBlockDmySU
    {
        public CommonParser Common { get; private set; }
        public SourceParser Source { get; private set; }


        public InfoBlockDmySU(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            Common = new CommonParser(infoBlock, null, null, null, "CustomName=", "Comment=");
            Source = new SourceParser(infoBlock, "SourceNode=", null);
        }

    }
}
