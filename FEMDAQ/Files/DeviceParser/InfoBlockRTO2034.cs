using FEMDAQ.StaticHelper;
using RohdeSchwarz.RsScope;
using RohdeUndSchwarz;
using System;
using System.Collections.Generic;
using System.Drawing;


namespace Files.Parser
{
    public class InfoBlockRTO2034 : InfoBlock
    {
        public CommonParser CommonWaveform { get; private set; }
        public CommonParser CommonFft { get; private set; }
        public IpParser Ip { get; private set; }
        public GaugeParser Gauge { get; private set; }

        public RTO2034Channel Channel { get; private set; }
        public RTO2034Waveform Waveform { get; private set; }
        public int MeasureInstantly { get; private set; }
        public double QuantPerDivX { get; private set; }
        public double SampleRate { get; private set; }
        public bool TriggerOnWaveform { get; private set; }
        public double TriggerLevel { get; private set; }
        public TriggerSource TriggerSource { get; private set; }
        public TriggerModifier TriggerMode { get; private set; }
        public TriggerType TriggerType { get; private set; }
        public Slope TriggerSlope { get; private set; }
        public int ReadWaveform { get; private set; }
        public bool UpdateDisplay { get; private set; }

        // FFT-Settings
        public RTO2034MathWindow MathWindow { get; private set; }
        public bool TriggerOnFFT { get; private set; }
        public double FrequencyResolution { get; private set; }
        public double StartFrequency { get; private set; }
        public double StopFrequency { get; private set; }
        public double MagnitudeOffset { get; private set; }
        public double MagnitudeRange { get; private set; }
        public FFTWindowType WindowType { get; private set; }



        public InfoBlockRTO2034(IEnumerable<string> infoBlock)
        {
            if (infoBlock == null) throw new ArgumentNullException("infoBlock");
            string lineInfo = null;

            ParseChannel(StringHelper.FindStringWhichStartsWith(infoBlock, "[Dev"));
            ParseWaveform(StringHelper.FindStringWhichStartsWith(infoBlock, "[Dev"));

            Ip = new IpParser(infoBlock);

            Gauge = new GaugeParser(infoBlock, "MeasureInstantly=", "QuantPerDivY=", null);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "QuantPerDivX=");
            QuantPerDivX = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "SampleRate=");
            SampleRate = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "TriggerOnWaveform=");
            if (lineInfo == null)
                TriggerOnWaveform = true;
            else
                TriggerOnWaveform = bool.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "TriggerLevel=");
            if (lineInfo == null)
                TriggerLevel = 0;
            else
                TriggerLevel = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            ParseTriggerSource(StringHelper.FindStringWhichStartsWith(infoBlock, "TriggerSource="));

            ParseTriggerMode(StringHelper.FindStringWhichStartsWith(infoBlock, "TriggerMode="));

            ParseTriggerType(StringHelper.FindStringWhichStartsWith(infoBlock, "TriggerType="));

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "ReadWaveform=");
            ReadWaveform = (int)ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "UpdateDisplay=");
            if (lineInfo == null)
                UpdateDisplay = true;
            else
                UpdateDisplay = bool.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));

            if (ReadWaveform != 0) // Create only when waveform is used!
                CommonWaveform = new CommonParser(infoBlock, "WaveformChartIdentifier=", null, "WaveformCustomName=", "WaveformChartColor=");

            lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "UseFFTOnMathWindow=");
            if (lineInfo == null)
                MathWindow = 0;
            else
                MathWindow = (RTO2034MathWindow)ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

            if ((int)MathWindow >= 1 && (int)MathWindow <= 4) // Creation only when FFT is used
            {
                lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "TriggerOnFFT=");
                if (lineInfo == null) // Use default
                {
                    if (TriggerOnWaveform == false) // No waveform-trigger
                        TriggerOnFFT = true; // Extra-trigger to avoid exception due to no aquisition is given!
                    else
                        TriggerOnFFT = false; // No extra trigger!
                }
                else
                    TriggerOnFFT = bool.Parse(ParseHelper.ParseStringValueFromLineInfo(lineInfo));

                CommonFft = new CommonParser(infoBlock, "FFTChartIdentifier=", null, "FFTCustomName=", "FFTChartColor="); // DrawnOver is not possible for FFT -> Always over f!

                lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "FrequencyResolution=");
                FrequencyResolution = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

                lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "StartFrequency=");
                StartFrequency = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

                lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "StopFrequency=");
                StopFrequency = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

                lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "MagnitudeOffset=");
                MagnitudeOffset = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

                lineInfo = StringHelper.FindStringWhichStartsWith(infoBlock, "MagnitudeRange=");
                MagnitudeRange = ParseHelper.ParseDoubleValueFromLineInfo(lineInfo);

                ParseWindowType(StringHelper.FindStringWhichStartsWith(infoBlock, "WindowType="));
            }
        }


        public void Dispose()
        {
            CommonWaveform.Dispose();
            CommonFft.Dispose();
            Ip.Dispose();
            Gauge.Dispose();
        }



        private void ParseChannel(string info)
        {
            var splitString = info.Split(new char[] { '[', '|', ']' }, StringSplitOptions.RemoveEmptyEntries);

            switch (splitString[2].ToUpper())
            {
                case "CHANNEL1":
                    Channel = RTO2034Channel.CH1;
                    break;

                case "CHANNEL2":
                    Channel = RTO2034Channel.CH2;
                    break;

                case "CHANNEL3":
                    Channel = RTO2034Channel.CH3;
                    break;

                case "CHANNEL4":
                    Channel = RTO2034Channel.CH4;
                    break;

                default:
                    throw new FormatException("Line: " + info);
            }
        }



        private void ParseWaveform(string info)
        {
            var splitString = info.Split(new char[] { '[', '|', ']' }, StringSplitOptions.RemoveEmptyEntries);

            switch (splitString[3].ToUpper())
            {
                case "WAVEFORM1":
                    Waveform = RTO2034Waveform.W1;
                    break;

                case "WAVEFORM2":
                    Waveform = RTO2034Waveform.W2;
                    break;

                case "WAVEFORM3":
                    Waveform = RTO2034Waveform.W3;
                    break;

                default:
                    throw new FormatException("Line: " + info);
            }
        }



        private void ParseTriggerSource(string info)
        {
            var stringValue = ParseHelper.ParseStringValueFromLineInfo(info)
                                         .ToUpper();

            switch (stringValue)
            {
                case "CHANNEL1":
                    TriggerSource = TriggerSource.Channel1;
                    break;

                case "CHANNEL2":
                    TriggerSource = TriggerSource.Channel2;
                    break;

                case "CHANNEL3":
                    TriggerSource = TriggerSource.Channel3;
                    break;

                case "CHANNEL4":
                    TriggerSource = TriggerSource.Channel4;
                    break;

                default:
                    throw new FormatException("Unknown triggersource - Line: " + info);
            }
        }



        private void ParseTriggerMode(string info)
        {
            var stringValue = ParseHelper.ParseStringValueFromLineInfo(info)
                                         .ToUpper();

            switch (stringValue)
            {
                case "NORMAL":
                    TriggerMode = TriggerModifier.Normal;
                    break;

                case "AUTO":
                    TriggerMode = TriggerModifier.Auto;
                    break;

                case "FREERUN":
                    TriggerMode = TriggerModifier.FreeRun;
                    break;

                default:
                    throw new FormatException("Line: " + info);
            }

        }



        private void ParseTriggerType(string info)
        {
            var stringValue = ParseHelper.ParseStringValueFromLineInfo(info)
                                         .ToUpper();
            var splitString = stringValue.Split(new char[] { '|' }, StringSplitOptions.None);
            if (splitString == null || splitString.Length < 2) throw new FormatException("Line: " + info);

            // Type
            switch (splitString[0])
            {
                case "EDGE":
                    TriggerType = TriggerType.Edge;
                    break;

                default:
                    throw new FormatException("Line: " + info);
            }

            // Slope
            switch (splitString[1])
            {
                case "FALLING":
                    TriggerSlope = Slope.Negative;
                    break;

                case "RISING":
                    TriggerSlope = Slope.Positive;
                    break;

                case "EITHER":
                    TriggerSlope = Slope.Either;
                    break;

                default:
                    throw new FormatException("Line: " + info);
            }
        }



        private void ParseWindowType(string info)
        {
            var stringValue = ParseHelper.ParseStringValueFromLineInfo(info.ToUpper());
            switch (stringValue)
            {
                case "RECT":
                case "RECTANGLE":
                case "NONE":
                    WindowType = FFTWindowType.Rectangular;
                    break;

                case "HAMMING":
                    WindowType = FFTWindowType.Hamming;
                    break;

                case "HANNING":
                    WindowType = FFTWindowType.Hann;
                    break;

                case "BLACKMANHARRIS":
                    WindowType = FFTWindowType.Blackman;
                    break;

                case "GAUSS":
                    WindowType = FFTWindowType.Gaussian;
                    break;

                case "FLATTOP":
                    WindowType = FFTWindowType.Flattop;
                    break;

                case "KAISERBESSEL":
                default:
                    WindowType = FFTWindowType.KaiserBessel;
                    break;
            }

        }


    }
}
