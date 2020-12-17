using FEMDAQ.StaticHelper;
using System;
using System.Collections.Generic;
using System.IO;

namespace Files
{
    public class SweepContent
    {
        // Globals
        private string _filename;

        // Sweepblocks
        private List<List<string>> _sweepLines;
        public List<string> Header { get; private set; }
        public List<List<double>> Values { get; private set; }
        public string Filename
        {
            get
            {
                string[] splittedFilename = _filename.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedFilename == null)
                    return null; // Error: Split failed!
                return splittedFilename[splittedFilename.Length - 1];
            }
            private set
            {
                if (value != null)
                    _filename = value;
            }
        }
        public string FullFilename { get { return _filename; } }


        
        public SweepContent(string filename)
        {
            if (filename == null) throw new ArgumentNullException("filename");
            _filename = filename;
            DecodeSweep();
            ParseSweep();
            CheckForValidColumns(); // Throwing expection on non-matching column-count of header and values
        }



        private void DecodeSweep()
        {
            StreamReader file = new StreamReader(FullFilename);
            if (file == null) throw new FileNotFoundException("Filename:" + Filename);
            string[] sweepLineByLine = file.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            file.Dispose();
            if (sweepLineByLine.Length < 1) throw new FormatException("Empty sweepfile.");

            _sweepLines = new List<List<string>>();
            foreach(var line in sweepLineByLine)
            {
                if (!line.StartsWith("#"))
                {
                    var lineSplit = new List<string>();
                    lineSplit.AddRange(line.Split(new char[] { ',' }, StringSplitOptions.None));
                    lineSplit = StringHelper.TrimList(lineSplit);
                    _sweepLines.Add(lineSplit);
                }
            }
        }


        
        /// <summary>
        /// Splits the sweepheader and parses/converts the sweep-values.
        /// 
        /// Returns 0 if everything gone well. Negative number indicates exception errors. A positive number indicates the amount of failed conversions!
        /// </summary>
        /// <returns></returns>
        private void ParseSweep()
        {
            Header = new List<string>();
            foreach(var header in _sweepLines[0])
                Header.Add(header.ToUpper());

            Values = new List<List<double>>();
            List<string> row;
            for(int rowIndex = 1; rowIndex < _sweepLines.Count; rowIndex++)
            {
                row = _sweepLines[rowIndex];
                var cellValues = new List<double>();
                foreach (var cell in row)
                    cellValues.Add(double.Parse(cell));
                Values.Add(cellValues);
            }
        }



        private void CheckForValidColumns()
        {
            if (Header.Count != Values[0].Count) // Headercolumn-count don't match with valuecolumn-count
                throw new InvalidDataException("SweepContent - Count of header-columns doesn't match with count of the value-columns");
        }



        public List<double> GetSweepColumn(int columnIndex)
        {
            var column = new List<double>();

            for(var row = 0; row < Values.Count; row++)
                column.Add(Values[row][columnIndex]);
            return column;
        }


        public List<double> GetSweepRow(int rowIndex)
        {
            var row = new List<double>();

            foreach (var rowCell in Values[rowIndex])
                row.Add(rowCell);
            return row;
        }

    }
}
