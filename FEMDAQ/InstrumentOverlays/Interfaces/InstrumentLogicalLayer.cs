using Files;
using System;
using System.Collections.Generic;
using Files.Parser;

namespace Instrument.LogicalLayer
{

    public enum InstrumentCommunicationPHY
    {
        Unknown = -1,
        GPIB,
        //RS232,
        //RS485,
        COMPort, // Combines all COM-Ports (RS232 and RS485 for now)
        Ethernet,
        USB,
        Virtual, // Dummy-Devices not using any PHY
    }

    public interface InstrumentLogicalLayer : IDisposable
    {
        // Getter/Setter
        //List<List<List<double>>> xResults { get; } // Obsolote due to device 
        //List<List<List<double>>> yResults { get; }
        string DeviceIdentifier { get; }
        string DeviceType { get; }
        string DeviceName { get; }
        InstrumentCommunicationPHY CommunicationPhy { get; }
        //GaugeMeasureInstantly InstantMeasurement(string identifier);
        GaugeMeasureInstantly InstantMeasurement { get; }
        List<string> DrawnOverIdentifiers { get; }



        // Common
        void Init();



        // Gauge
        //void Measure(double[] drawnOver);
        //List<double> GetXResultList(int[] indicies);
        //List<double> GetYResultList(int[] indicies);

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
