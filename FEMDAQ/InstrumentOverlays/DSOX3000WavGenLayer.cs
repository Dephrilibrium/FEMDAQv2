using Agilent.AgInfiniiVision.Interop;
using FEMDAQ.Measurement;
using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using Keysight;
using System;
using System.Collections.Generic;

namespace Instrument.LogicalLayer
{
    public class DSOX3000WavGenLayer : InstrumentLogicalLayer
    {
        public InfoBlockDSOX3000WavGen InfoBlock { get; private set; }
        private List<List<double>> _sweep;
        private DSOX3000WavGen _device;

        // Devicespecific
        private bool _highLowMode = false;
        private int _waveformSpecificSweepIndex = 0;



        public DSOX3000WavGenLayer(DeviceInfoStructure infoStructure)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock  = infoStructure.InfoBlock as InfoBlockDSOX3000WavGen;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Wrong argument: {0} instead of KE6485", DeriveFromObject.DeriveNameFromStructure(infoStructure.InfoBlock)));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + (cName == null || cName == "" ? DeviceType : cName);

            _device = new DSOX3000WavGen(InfoBlock.Usb.USBAddress);
            if (_device == null) throw new NullReferenceException("WavGen33511B device couldn't be generated.");

            //Init(); // Now in Startemeasurement-Routine of the mainFrame
        }



        public void Dispose()
        {
            if (_device != null)
                _device.Dispose();
        }



        #region Getter/Setter
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceName { get; private set; }
        //public List<List<List<double>>> xResults { get { return null; } }
        //public List<List<List<double>>> yResults { get { return null; } }
        public GaugeMeasureInstantly InstantMeasurement { get { return GaugeMeasureInstantly.Disabled; } }
        public List<string> DrawnOverIdentifiers { get { return null; } }
        #endregion



        #region Common
        public void Init()
        {
            _device.OutputLoad = InfoBlock.OutputLoad;
            _device.Waveform = InfoBlock.Waveform;
        }
        #endregion



        #region Gauge
        public List<double> GetXResultList(int[] indicies){ return null; }
        public List<double> GetYResultList(int[] indicies) { return null; }


        //public void Measure(double[] drawnOver)
        public void Measure(Func<List<string>, double[]> GetDrawnOver, GaugeMeasureInstantly MeasureCycle)
        {
        }



        //public void SaveResultsToFolder(string folderPath)
        //{
        //}
        public void SaveResultsToFolder(string folderPath, string filePrefix)
        {
        }



        public void ClearResults()
        {
        }
        #endregion


        #region Source
        public double GetSourceValue(string identifier)
        {
            var splitIdentifier = identifier.Split(new char[] { '|' });
            var sourceValueType = string.Empty;

            if (splitIdentifier.Length == 2) // Identifier has the form <DevX|F>
                sourceValueType = splitIdentifier[1].ToUpper(); // Get the <F> from identifier
            else
                sourceValueType = "A"; // Defaultvalue is Amplitude

            double waveGenValue = 0, dummy = 0;
            switch (sourceValueType)
            {
                case "L": _device.GetAmplitude(out waveGenValue, out dummy); break;
                case "H": _device.GetAmplitude(out dummy, out waveGenValue); break;
                case "F": waveGenValue = _device.Frequency; break;
                case "O": waveGenValue = _device.Offset; break;
                case "D": waveGenValue = _device.DutyCycle; break;
                case "S": waveGenValue = _device.RampSymmetry; break;
                case "PW": waveGenValue = _device.PulseWidth; break;

                default: // Amplitude is every other case
                    _device.GetAmplitude(out waveGenValue);
                    break;
            }
            return waveGenValue;
        }



        public void SetSourceValues(int sweepLine)
        {
            // Frequency and amplitude
            var frequency = _sweep[0][sweepLine];
            _device.Frequency = frequency;

            if (!_highLowMode)
            {
                var amplitude = _sweep[1][sweepLine]; // Amplitude
                var offset = _sweep[2][sweepLine]; // Offset
                _device.SetAmplitude(amplitude);
                _device.GetAmplitude(out amplitude);
                _device.Offset = offset;
            }
            else
            {
                var high = _sweep[1][sweepLine]; // High
                var low = _sweep[2][sweepLine]; // Low
                _device.SetAmplitude(high, low);
            }


            // Waveform and waveformspecific
            switch (InfoBlock.Waveform)
            {
                case AgInfiniiVisionWaveformGeneratorFunctionEnum.AgInfiniiVisionWaveformGeneratorFunctionSquare:
                    var dutyCycle = _sweep[_waveformSpecificSweepIndex][sweepLine];
                    _device.DutyCycle = dutyCycle;
                    break;

                case AgInfiniiVisionWaveformGeneratorFunctionEnum.AgInfiniiVisionWaveformGeneratorFunctionRamp:
                    var symmetry = _sweep[_waveformSpecificSweepIndex][sweepLine];
                    _device.RampSymmetry = symmetry;
                    break;

                case AgInfiniiVisionWaveformGeneratorFunctionEnum.AgInfiniiVisionWaveformGeneratorFunctionPulse:
                    var pulsWidth = _sweep[_waveformSpecificSweepIndex][sweepLine];
                    _device.PulseWidth = pulsWidth;
                    break;
                default:
                    break;
            }
            _device.Output = true;
        }



        public void PowerDownSource()
        {
            _device.PowerDownSource();
        }



        public void AssignSweepColumn(SweepContent sweep)
        {
            if (InfoBlock.Source.SourceNode < 0)
                return;

            _sweep = new List<List<double>>();

            List<double> listBuffer;

            var sourceNodeOfList = "F" + Convert.ToString(InfoBlock.Source.SourceNode);
            listBuffer = AssignSweep.Assign(sweep, sourceNodeOfList);
            if (listBuffer == null) throw new MissingFieldException("Can't find " + sourceNodeOfList + " in sweep-file.");
            _sweep.Add(listBuffer);

            var amplitudeSourceNodeOfList = "A" + Convert.ToString(InfoBlock.Source.SourceNode);
            listBuffer = AssignSweep.Assign(sweep, amplitudeSourceNodeOfList);
            if (listBuffer != null)
            {
                _highLowMode = false;
                _sweep.Add(listBuffer);
                sourceNodeOfList = "O" + Convert.ToString(InfoBlock.Source.SourceNode);
                listBuffer = AssignSweep.Assign(sweep, sourceNodeOfList);
                if (listBuffer == null) throw new MissingFieldException("Can't find " + sourceNodeOfList + " in sweep-file. (The problem could be also caused by missing " + amplitudeSourceNodeOfList + ")");
                _sweep.Add(listBuffer);
            }
            else
            {
                _highLowMode = true;
                sourceNodeOfList = "H" + Convert.ToString(InfoBlock.Source.SourceNode);
                listBuffer = AssignSweep.Assign(sweep, sourceNodeOfList);
                if (listBuffer == null) throw new MissingFieldException("Can't find " + sourceNodeOfList + " in sweep-file. (The problem could be also caused by missing " + amplitudeSourceNodeOfList + ")");
                _sweep.Add(listBuffer);
                sourceNodeOfList = "L" + Convert.ToString(InfoBlock.Source.SourceNode);
                listBuffer = AssignSweep.Assign(sweep, sourceNodeOfList);
                if (listBuffer == null) throw new MissingFieldException("Can't find " + sourceNodeOfList + " in sweep-file. (The problem could be also caused by missing " + amplitudeSourceNodeOfList + ")");
                _sweep.Add(listBuffer);
            }

            List<double> waveformBuffer;
            var waveformSourceNode = string.Empty;
            switch (InfoBlock.Waveform)
            {
                case AgInfiniiVisionWaveformGeneratorFunctionEnum.AgInfiniiVisionWaveformGeneratorFunctionSquare:
                    waveformSourceNode = "D" + Convert.ToString(InfoBlock.Source.SourceNode); // D = Dutycycle
                    waveformBuffer = AssignSweep.Assign(sweep, waveformSourceNode);
                    if (waveformBuffer == null) throw new FormatException("Can't find column \"" + waveformSourceNode + "\" in sweep-file.");
                    _waveformSpecificSweepIndex = _sweep.Count;
                    _sweep.Add(waveformBuffer);
                    break;
                case AgInfiniiVisionWaveformGeneratorFunctionEnum.AgInfiniiVisionWaveformGeneratorFunctionRamp:
                    waveformSourceNode = "S" + Convert.ToString(InfoBlock.Source.SourceNode); // S = Symmetry
                    waveformBuffer = AssignSweep.Assign(sweep, waveformSourceNode);
                    if (waveformBuffer == null) throw new FormatException("Can't find column \"" + waveformSourceNode + "\" in sweep-file.");
                    _waveformSpecificSweepIndex = _sweep.Count;
                    _sweep.Add(waveformBuffer);
                    break;
                case AgInfiniiVisionWaveformGeneratorFunctionEnum.AgInfiniiVisionWaveformGeneratorFunctionPulse:
                    waveformSourceNode = "PW" + Convert.ToString(InfoBlock.Source.SourceNode); // PW = Puls-Width
                    waveformBuffer = AssignSweep.Assign(sweep, waveformSourceNode);
                    if (waveformBuffer == null) throw new FormatException("Can't find column \"" + waveformSourceNode + "\" in sweep-file.");
                    _waveformSpecificSweepIndex = _sweep.Count;
                    _sweep.Add(waveformBuffer);
                    break;
                default:
                    break;
            }
        }
        #endregion



        #region UI
        public void UpdateGraph()
        {
        }
        #endregion
    }
}
