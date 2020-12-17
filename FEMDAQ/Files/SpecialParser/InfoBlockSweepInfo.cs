using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Files.Parser
{
    public class InfoBlockSweepInfo
    {
        public string BlockIdentifier { get; private set; }
        public string FullFilename { get; private set; }
        public string Filename
        {
            get { return Path.GetFileName(FullFilename); }
        }

        public InfoBlockSweepInfo(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");

            string lineInfo = null;

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "[SweepInfo");
            BlockIdentifier = ParseBlockIdentifier(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "StartupSweep=");
            FullFilename = ParseHelper.ParseStringValueFromLineInfo(lineInfo);
            if (FullFilename != "" && !Path.IsPathRooted(FullFilename))
                FullFilename = Path.GetFullPath(Path.Combine(Application.StartupPath, FullFilename)); // Relative path to exe
        }

        private string ParseBlockIdentifier(string info)
        {
            var splitInfo = info.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitInfo == null) throw new FormatException("Line: " + info);

            return splitInfo[0];
        }



        private string ParseFilename()
        {
            if (FullFilename == null)
                return null;

            var splitFilename = FullFilename.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var filename = splitFilename[splitFilename.Length - 1];
            if (filename == "")
                return null;

            return filename;
        }
    }
}
