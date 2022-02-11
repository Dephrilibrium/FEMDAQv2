using FEMDAQ.Measurement;
using FEMDAQ.StaticHelper;
using Files;
using Files.Parser;
using Keysight;
using System;
using System.Collections.Generic;

namespace Instrument.LogicalLayer
{
    enum VoltageMode { NoMode, AmplitudeOffset, HighLow, DCOffset };

    public class WavGen33511BLayer : InstrumentLogicalLayer
    {
        public InfoBlockWavGen33511B InfoBlock { get; private set; }
        private List<List<double>> _sweep;
        private WavGen33511B _device;

        // Devicespecific
        private VoltageMode _voltMode;
        private int _waveformSpecificSweepIndex = 0;



        public WavGen33511BLayer(DeviceInfoStructure infoStructure)
        {
            if (infoStructure == null) throw new ArgumentNullException("infoStructure");
            InfoBlock = infoStructure.InfoBlock as InfoBlockWavGen33511B;
            if (InfoBlock == null) throw new ArgumentException(string.Format("Wrong argument: {0} instead of KE6485", DeriveFromObject.DeriveNameFromStructure(infoStructure.InfoBlock)));

            DeviceIdentifier = infoStructure.DeviceIdentifier;
            DeviceType = infoStructure.DeviceType;
            var cName = InfoBlock.Common.CustomName;
            DeviceName = DeviceIdentifier + (cName == null || cName == "" ? DeviceType : cName);
            
            _device = new WavGen33511B(InfoBlock.Gpib.GpibBoardNumber, (byte)InfoBlock.Gpib.GpibPrimaryAdress, (byte)InfoBlock.Gpib.GpibSecondaryAdress);
            if (_device == null) throw new NullReferenceException("WavGen33511B device couldn't be generated.");

            _voltMode = VoltageMode.NoMode;
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
        public List<List<double>> xResults { get { return null; } }
        public List<List<double>> yResults { get { return null; } }
        public GaugeMeasureInstantly InstantMeasurement { get { return GaugeMeasureInstantly.Disabled; } }
        public List<string> DrawnOverIdentifiers { get { return null; } }
        #endregion



        #region Common
        public void Init()
        {
            _device.OutputLoad(InfoBlock.OutputLoad);
            switch (InfoBlock.Waveform)
            {
                case "SIN": _device.SetSine(); break;
                case "SQU": _device.SetSquare(50); break; // Dummy duty-cycle -> Changed on measure-start
                case "RAMP": _device.SetRamp(50); break; // Dummy symmetry -> Changed on measure-start
                case "PULS": _device.SetPulse(20e-3, InfoBlock.PulseLeadEdge, InfoBlock.PulseTrailEdge); break; // Dummy puls-width -> Changed on measure-start
                default: break;
            }
            _device.SetOffset(0.0); // Dummy offset -> Changed on measure-start
            _device.SetPhase(InfoBlock.Phase);

            if (InfoBlock.UseBurst != 0)
                _device.OutputBurst(InfoBlock.BurstCycles, InfoBlock.BurstPeriod);
        }
        #endregion



        #region Gauge
        public void Measure(double[] drawnOver)
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

            double waveGenValue;
            var dummy = 0d;
            switch (sourceValueType)
            {
                case "L": // Lowlevel
                    _device.GetAmplitude(out waveGenValue, out dummy);
                    break;
                case "H": // Highlevel
                    _device.GetAmplitude(out dummy, out waveGenValue);
                    break;
                case "F": // Frequency
                    waveGenValue = _device.GetFrequency();
                    break;
                case "O": // Offset
                    waveGenValue = _device.GetOffset();
                    break;
                case "D":
                    waveGenValue = _device.GetSquareDutyCycle();
                    break;

                default: // Amplitude is every other case
                    _device.GetAmplitude(out waveGenValue);
                    break;
            }
            return waveGenValue;
        }

        

        public void SetSourceValues(int sweepLine)
        {
            switch(_voltMode)
            {
                case VoltageMode.DCOffset:
                    var dcOffset = _sweep[0][sweepLine];
                    _device.SetOffset(dcOffset);
                    break;
                case VoltageMode.AmplitudeOffset:
                    var ampOffFrequency = _sweep[0][sweepLine];
                    var amplitude = _sweep[1][sweepLine];
                    var offset = _sweep[2][sweepLine];
                    _device.SetFrequency(ampOffFrequency);
                    _device.SetAmplitude(amplitude);
                    _device.SetOffset(amplitude);
                    break;
                case VoltageMode.HighLow:
                    var highLowFrequency = _sweep[0][sweepLine];
                    var high = _sweep[1][sweepLine];
                    var low = _sweep[2][sweepLine];
                    _device.SetFrequency(highLowFrequency);
                    _device.SetAmplitude(high, low);
                    break;
                case VoltageMode.NoMode:
                default:
                    break;
            }


            // Waveform and waveformspecific
            var waveform = InfoBlock.Waveform.ToUpper();
            switch (waveform)
            {
                case "DC":
                    _device.SetDC();
                    break;
                case "SQU":
                    var dutyCycle = _sweep[_waveformSpecificSweepIndex][sweepLine];
                    _device.SetSquare(dutyCycle);
                    break;
                case "RAMP":
                    var symmetry = _sweep[_waveformSpecificSweepIndex][sweepLine];
                    _device.SetRamp(symmetry);
                    break;
                case "PULS":
                    var pulsWidth = _sweep[_waveformSpecificSweepIndex][sweepLine];
                    _device.SetPulse(pulsWidth, InfoBlock.PulseLeadEdge, InfoBlock.PulseTrailEdge);
                    break;
                default:
                    break;
            }
            _device.Output(true);
        }



        public void PowerDownSource()
        {
            _device.PowerDownSource();
        }



        private VoltageMode GetVoltageMode(SweepContent sweep)
        {
            var sourceNodeAsString = Convert.ToString(InfoBlock.Source.SourceNode);
            if (AssignSweep.IdentifierIsAvailable(sweep, "O" + sourceNodeAsString) && !AssignSweep.IdentifierIsAvailable(sweep, "A" + sourceNodeAsString))
                return VoltageMode.DCOffset;
            else if (AssignSweep.IdentifierIsAvailable(sweep, "O" + sourceNodeAsString) && AssignSweep.IdentifierIsAvailable(sweep, "A" + sourceNodeAsString))
                return VoltageMode.AmplitudeOffset;
            else if (AssignSweep.IdentifierIsAvailable(sweep, "H" + sourceNodeAsString) && AssignSweep.IdentifierIsAvailable(sweep, "L" + sourceNodeAsString))
                return VoltageMode.HighLow;
            else
                return VoltageMode.NoMode;
        }



        public void AssignSweepColumn(SweepContent sweep)
        {
            if (InfoBlock.Source.SourceNode < 0)
                return;

            _sweep = new List<List<double>>();

            // Get voltage mode (necessary for control value order)
            _voltMode = GetVoltageMode(sweep);
            if (_voltMode == VoltageMode.NoMode) throw new InvalidOperationException("Couldn't find one of the valid voltage modes: \n1.) Offset\n2.) Offset/Amplitude\n3.) High/Low");

            List<double> listBuffer;
            var waveform = InfoBlock.Waveform.ToUpper();
            var sourceNodeAsString = Convert.ToString(InfoBlock.Source.SourceNode);
            if (_voltMode == VoltageMode.DCOffset && waveform == "DC")
            {
                // DC Offset
                listBuffer = AssignSweep.Assign(sweep, "O" + sourceNodeAsString);
                if (listBuffer == null) throw new MissingFieldException("Can't find O" + sourceNodeAsString + " in sweep-file.");
                _sweep.Add(listBuffer);
            }
            else if (_voltMode == VoltageMode.AmplitudeOffset && waveform != "DC")
            {
                // Frequency
                listBuffer = AssignSweep.Assign(sweep, "F" + sourceNodeAsString);
                if (listBuffer == null) throw new MissingFieldException("Can't find F" + sourceNodeAsString + " in sweep-file.");
                _sweep.Add(listBuffer);
                // Amplitude
                listBuffer = AssignSweep.Assign(sweep, "A" + sourceNodeAsString);
                if (listBuffer == null) throw new MissingFieldException("Can't find A" + sourceNodeAsString + " in sweep-file.");
                _sweep.Add(listBuffer);
                // Offset
                listBuffer = AssignSweep.Assign(sweep, "O" + sourceNodeAsString);
                if (listBuffer == null) throw new MissingFieldException("Can't find O" + sourceNodeAsString + " in sweep-file.");
                _sweep.Add(listBuffer);
            }
            else if (_voltMode == VoltageMode.HighLow && waveform != "DC")
            {
                // Frequency
                listBuffer = AssignSweep.Assign(sweep, "F" + sourceNodeAsString);
                if (listBuffer == null) throw new MissingFieldException("Can't find F" + sourceNodeAsString + " in sweep-file.");
                _sweep.Add(listBuffer);
                // High
                listBuffer = AssignSweep.Assign(sweep, "H" + sourceNodeAsString);
                if (listBuffer == null) throw new MissingFieldException("Can't find H" + sourceNodeAsString + " in sweep-file.");
                _sweep.Add(listBuffer);
                // Low
                listBuffer = AssignSweep.Assign(sweep, "L" + sourceNodeAsString);
                if (listBuffer == null) throw new MissingFieldException("Can't find L" + sourceNodeAsString + " in sweep-file.");
                _sweep.Add(listBuffer);
            }
            else
                throw new InvalidOperationException("Control values from sweep won't match with waveform from ini!");


            List<double> waveformBuffer;
            var waveformSourceNode = string.Empty;
            switch (waveform)
            {
                case "SQU":
                    waveformSourceNode = "D" + Convert.ToString(InfoBlock.Source.SourceNode); // D = Dutycycle
                    waveformBuffer = AssignSweep.Assign(sweep, waveformSourceNode);
                    if (waveformBuffer == null) throw new FormatException("Can't find column \"" + waveformSourceNode + "\" in sweep-file.");
                    _waveformSpecificSweepIndex = _sweep.Count;
                    _sweep.Add(waveformBuffer);
                    break;
                case "RAMP":
                    waveformSourceNode = "S" + Convert.ToString(InfoBlock.Source.SourceNode); // S = Symmetry
                    waveformBuffer = AssignSweep.Assign(sweep, waveformSourceNode);
                    if (waveformBuffer == null) throw new FormatException("Can't find column \"" + waveformSourceNode + "\" in sweep-file.");
                    _waveformSpecificSweepIndex = _sweep.Count;
                    _sweep.Add(waveformBuffer);
                    break;
                case "PULS":
                    waveformSourceNode = "PW" + Convert.ToString(InfoBlock.Source.SourceNode); // PW = Puls-Width
                    waveformBuffer = AssignSweep.Assign(sweep, waveformSourceNode);
                    if (waveformBuffer == null) throw new FormatException("Can't find column \"" + waveformSourceNode + "\" in sweep-file.");
                    _waveformSpecificSweepIndex = _sweep.Count;
                    _sweep.Add(waveformBuffer);
                    break;
                case "DC": // Has no specials
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
