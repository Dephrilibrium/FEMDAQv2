using FEMDAQ.StaticHelper;
using StanfordResearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Files.Parser
{
    class InfoBlockSR785 : InfoBlock
    {
        public CommonParser Common { get; private set; }
        public GpibParser Gpib { get; private set; }
        public GaugeParser Gauge { get; private set; }


        // Specific
        // X-Axis
        public sr785XAxisView XAxisView { get; private set; }
        public double StartFrequency { get; private set; }
        public double FrequencySpan { get; private set; }
        public sr785FftLines FftLines { get; private set; }

        // Y-Axis
        public sr785YAxisView YAxisView { get; private set; }
        public sr785YAxisdBUnit YdBUnit { get; private set; }
        public sr785YAxisPeakUnit YPeakUnit { get; private set; }
        public double YMin { get; private set; }
        public double YMax { get; private set; }

        // Average
        public sr785AveragingDisplayed DisplayedAverage { get; private set; }
        public int Averages { get; private set; }
        public double timeRecordIncrement { get; private set; }

        // Window
        public sr785WindowType WindowType { get; private set; }
        public double ExponentialTimeConstant { get; private set; }




        public InfoBlockSR785(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");
            string lineInfo = null;

            Common = new CommonParser(infoBlock, "ChartIdentifier=", null, "ChartColor=", "CustomName=", "Comment=");
            Gpib = new GpibParser(infoBlock);
            Gauge = new GaugeParser(infoBlock, "MeasureInstantly=", null, null);

            // X-Axis
            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "XAxisView=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
            if (lineInfo.StartsWith("LIN"))
                XAxisView = sr785XAxisView.Linear;
            else
                XAxisView = sr785XAxisView.Logarithmic;

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "StartFrequency=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo);
            StartFrequency = double.Parse(lineInfo);
            
            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "FrequencySpan=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo);
            FrequencySpan = double.Parse(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "FFTLines=");
            if (lineInfo == null)
                FftLines = sr785FftLines.Lines800;
            else
            {
                lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo);
                FftLines = (lineInfo.StartsWith("100") ? sr785FftLines.Lines100 :
                            lineInfo.StartsWith("200") ? sr785FftLines.Lines200 :
                            lineInfo.StartsWith("400") ? sr785FftLines.Lines400 : sr785FftLines.Lines800);
            }

            // Y-Axis
            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "YAxisView=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
            YAxisView = (lineInfo.StartsWith("LINMAG") ? sr785YAxisView.LinMag :
                         lineInfo.StartsWith("MAGSQUARED") ? sr785YAxisView.MagSquared :
                         lineInfo.StartsWith("REAL") ? sr785YAxisView.Real :
                         lineInfo.StartsWith("IMAG") ? sr785YAxisView.Imaginary : sr785YAxisView.LogMag);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "YdBUnit=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
            YdBUnit = (lineInfo.StartsWith("OFF") ? sr785YAxisdBUnit.Off :
                       lineInfo.StartsWith("DBM") ? sr785YAxisdBUnit.dBm : sr785YAxisdBUnit.dB);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "YPeakUnit=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
            YPeakUnit = (lineInfo.StartsWith("PEAKRMS") ? sr785YAxisPeakUnit.PeakRMS :
                         lineInfo.StartsWith("PEAKPEAK") ? sr785YAxisPeakUnit.PeakPeak : sr785YAxisPeakUnit.Peak);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "YMin=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo);
            YMin = double.Parse(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "YMax=");
            lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo);
            YMax = double.Parse(lineInfo);

            // Averaging
            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "DisplayedAverage=");
            if (lineInfo == null)
                DisplayedAverage = sr785AveragingDisplayed.None;
            else
            {
                lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
                DisplayedAverage = (lineInfo.StartsWith("VECTOR") ? sr785AveragingDisplayed.Vector :
                                    lineInfo.StartsWith("RMS") ? sr785AveragingDisplayed.RMS :
                                    lineInfo.StartsWith("PEAKHOLD") ? sr785AveragingDisplayed.PeakHold : sr785AveragingDisplayed.None);
            }

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "Averages=");
            if (lineInfo == null)
                Averages = 0;
            else
            {
                lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo);
                Averages = int.Parse(lineInfo);
                if (Averages < 2 || Averages > 32767) // Min and max borders
                    Averages = 0; // Disable
            }

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "TimeRecordIncrement=");
            if (lineInfo == null)
                timeRecordIncrement = 100;
            else
            {
                lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo);
                timeRecordIncrement = double.Parse(lineInfo);
            }

            // Window
            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "WindowType=");
            if (lineInfo == null)
                WindowType = sr785WindowType.Uniform_Rect;
            else
            {
                lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo).ToUpper();
                WindowType = (lineInfo.StartsWith("FLATTOP") ? sr785WindowType.Flattop :
                              lineInfo.StartsWith("HANNING") ? sr785WindowType.Hanning :
                              lineInfo.StartsWith("BLACKMANHARRIS") ? sr785WindowType.BlackmanHarris :
                              lineInfo.StartsWith("KAISER") ? sr785WindowType.Kaiser :
                              lineInfo.StartsWith("EXPONENTIAL") ? sr785WindowType.Exponential : sr785WindowType.Uniform_Rect);
            }

            if(WindowType== sr785WindowType.Exponential)
            {
                lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "ExponentialTimeConstant=");
                if (lineInfo == null)
                    ExponentialTimeConstant = 100;
                else
                {
                    lineInfo = ParseHelper.ParseStringValueFromLineInfo(lineInfo);
                    ExponentialTimeConstant = double.Parse(lineInfo);
                }
            }

        }

        public void Dispose()
        {
            Common.Dispose();
            Gpib.Dispose();
            Gauge.Dispose();
        }
    }
}