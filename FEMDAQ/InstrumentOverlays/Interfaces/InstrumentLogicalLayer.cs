using Files;
using System;
using System.Collections.Generic;
using Files.Parser;

namespace Instrument.LogicalLayer
{
    public enum GaugeMeasureInstantly
    {
        Disabled = -1,
        CycleStart = 1,
        CycleEnd = 0,
    };

    public interface InstrumentLogicalLayer : IDisposable
    {
        // Getter/Setter
        List<List<double>> xResults { get; }
        List<List<double>> yResults { get; }
        string DeviceIdentifier { get; }
        string DeviceType { get; }
        string DeviceName { get; }
        //GaugeMeasureInstantly InstantMeasurement(string identifier);
        GaugeMeasureInstantly InstantMeasurement { get; }
        List<string> DrawnOverIdentifiers { get; }



        // Common
        void Init();



        // Gauge
        //void Measure(double[] drawnOver);
        void Measure(Func<List<string>, double[]> GetDrawnOver, GaugeMeasureInstantly MeasureCycle);
        //void SaveResultsToFolder(string folderPath);
        void SaveResultsToFolder(string folderPath, string filePrefix);
        void ClearResults();



        // Source
        double GetSourceValue(string identifier);
        void SetSourceValues(int sweepLine);
        void PowerDownSource();
        void AssignSweepColumn(SweepContent sweep);



        // UI
        void UpdateGraph();
    }
}
