using Files;
using System;
using System.Collections.Generic;

namespace Instrument.LogicalLayer
{
    public interface InstrumentLogicalLayer : IDisposable
    {
        // Getter/Setter
        List<List<double>> xResults { get; }
        List<List<double>> yResults { get; }
        string DeviceIdentifier { get; }
        string DeviceType { get; }
        string DeviceName { get; }
        int InstantMeasurement { get; }
        List<string> DrawnOverIdentifiers { get; }



        // Common
        void Init();
        


        // Gauge
        void Measure(double[] drawnOver);
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
