using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Files.Parser
{
    public class SourceParser
    {
        public int SourceNode { get; set; }
        public double Compliance { get; set; }


        public SourceParser(IEnumerable<string> infoBlock, string sourceNodeToken = "SourceNode=", string complianceToken = "Compliance=")
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            ParseSourceNode(StringHelper.FindStringWhichStartsWith(infoBlock, sourceNodeToken));
            ParseCompliance(StringHelper.FindStringWhichStartsWith(infoBlock, complianceToken));
        }

        public void Dispose()
        {

        }



        private void ParseSourceNode(string info)
        {
            if (info == null)
                return; // Not found
            SourceNode = (int)ParseHelper.ParseDoubleValueFromLineInfo(info);
        }



        private void ParseCompliance(string info)
        {
            if (info == null)
                return; // Not found
            Compliance = ParseHelper.ParseDoubleValueFromLineInfo(info);
        }
    }
}
