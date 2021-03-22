using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Own usings
using FEMDAQ.StaticHelper;

namespace Files.Parser
{
    public class InfoBlockToolSettings
    {
        public string BlockIdentifier { get; private set; }
        public bool SaveResultsGrouped { get; private set; }


        // Initiate with default parameters
        public InfoBlockToolSettings()
        {
            BlockIdentifier = "ToolSettings";
            SaveResultsGrouped = false;
        }

        public InfoBlockToolSettings(IEnumerable<string> infoBlock)
        {
            string lineInfo = null;

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "[Tool");
            BlockIdentifier = ParseBlockIdentifier(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "SaveResultsGrouped=");
            SaveResultsGrouped = ParseHelper.ParseBoolValueFromLineInfo(lineInfo, false);
        }



        public string ParseBlockIdentifier(string info)
        {
            var splitInfo = info.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitInfo == null) throw new FormatException("Line: " + info);

            return splitInfo[0];
        }
    }
}
