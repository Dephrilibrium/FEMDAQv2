using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Own usings
using FEMDAQ.StaticHelper;

namespace Files.Parser
{
    public class InfoBlockToolInfo
    {
        public string BlockIdentifier { get; private set; }
        public string Title { get; private set; }
        public string Operator { get; private set; }
        public string Description { get; private set; }

        public string Comment { get; private set; }


        public InfoBlockToolInfo(IEnumerable<string> infoBlock)
        {
            string lineInfo = null;

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "[Tool");
            BlockIdentifier = ParseBlockIdentifier(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Title=");
            Title = ParseHelper.ParseStringValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Operator=");
            Operator = ParseHelper.ParseStringValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Description=");
            Description = ParseHelper.ParseStringValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Comment=");
            Comment = ParseHelper.ParseStringValueFromLineInfo(lineInfo);
        }



        public string ParseBlockIdentifier(string info)
        {
            var splitInfo = info.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitInfo == null) throw new FormatException("Line: " + info);

            return splitInfo[0];
        }
    }
}
