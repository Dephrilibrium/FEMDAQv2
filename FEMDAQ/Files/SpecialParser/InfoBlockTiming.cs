using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Own usings
using FEMDAQ.StaticHelper;

namespace Files.Parser
{
    public class InfoBlockTiming
    {
        public string BlockIdentifier { get; private set; }
        public int InitialTime { get; private set; }
        public int IterativeTime { get; private set; }

        public string Comment { get; private set; }



        public InfoBlockTiming(IEnumerable<string> infoBlock)
        {
            string lineInfo = null;

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "[Timing");
            BlockIdentifier = ParseBlockIdentifier(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Initial=");
            InitialTime = (int)ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Iterative=");
            IterativeTime = (int)ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Comment=");
            Comment = ParseHelper.ParseStringValueFromLineInfo(lineInfo);
        }



        private string ParseBlockIdentifier(string info)
        {
            var splitInfo = info.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitInfo == null) throw new FormatException("Line: " + info);

            return splitInfo[0];
        }
    }
}
