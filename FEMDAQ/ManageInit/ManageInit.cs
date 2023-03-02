using Files;
using Files.Parser;
using Instrument.LogicalLayer;
using System;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;

namespace FEMDAQ.Initialization
{
    public class ManageInit
    {

        /// <summary>
        /// Sets up the UI-Parameters.
        /// </summary>
        /// <param name="mainFrame"></param>
        /// <param name="ToolInfoStructure"></param>
        /// <returns></returns>
        static public void InitToolInfo(FEMDAQ MainFrame, InfoBlockToolInfo ToolInfoStructure)
        {
            MainFrame.Text = MainFrame.ProgramBasicTitletext + " - Title: " + ToolInfoStructure.Title;
            MainFrame.sbOperator.Text = "Operator: " + ToolInfoStructure.Operator;
            MainFrame.sbDescription.Text = "Description: " + ToolInfoStructure.Description;

            MainFrame.AddingTextToLog(string.Format("[Toolinfo]: "),
                                      string.Format("OK\n\n"),
                                      true); // Log-message ok
        }



        public static InstrumentLogicalLayer GenerateDevice(DeviceInfoStructure devInfoStructure, SweepContent sweep, HaumChart.HaumChart chart)
        {
            InstrumentLogicalLayer device = null;

            switch (devInfoStructure.DeviceType)
            {
                // Instanciation of dummydevices for test purposes
                case "DmyMU": // Measurement Unit only
                    device = new DmyMULayer(devInfoStructure, chart);
                    break;

                case "DmySU": // Source Unit only
                    device = new DmySULayer(devInfoStructure);
                    break;

                case "DmySMU": // Source Measurement Unit
                    device = new DmySMULayer(devInfoStructure, chart);
                    break;


                // Instanciation of real devices
                // Measurement device only
                case "PiCam":
                    device = new PiCamLayer(devInfoStructure, chart);
                    break;

                case "PiCam2":
                    device = new PiCam2Layer(devInfoStructure, chart);
                    break;

                case "DMM7510":
                    device = new DMM7510Layer(devInfoStructure, chart);
                    break;

                case "KE6485":
                    device = new KE6485Layer(devInfoStructure, chart);
                    break;

                case "RTO2034":
                    device = new RTO2034Layer(devInfoStructure, chart);
                    break;

                case "SR785":
                    device = new SR785Layer(devInfoStructure, chart);
                    break;

                case "VSH8x":
                    device = new VSH8xLayer(devInfoStructure, chart);
                    break;

                case "VD9x":
                    device = new VD9xLayer(devInfoStructure, chart);
                    break;

                case "DSOX3034T":
                    device = new DSOX3034TLayer(devInfoStructure, chart);
                    break;

                // Sweepassignment will be done at the "OpenSweepFile-Method"!
                // SMU device only
                case "KE6487":
                    device = new KE6487Layer(devInfoStructure, chart);
                    break;

                case "HP4145B":
                    device = new HP4145BLayer(devInfoStructure, chart);
                    break;

                case "KE6517B":
                    device = new KE6517BLayer(devInfoStructure, chart);
                    break;

                case "FEAR16v2":
                    device = new FEAR16v2Layer(devInfoStructure, chart);
                    break;

                // Source device only
                case "FUGMCP140":
                    device = new FUGMCP140Layer(devInfoStructure);
                    break;

                case "FUGHCP350":
                    device = new FUGHCP350Layer(devInfoStructure);
                    break;

                case "WavGen33511B":
                    device = new WavGen33511BLayer(devInfoStructure);
                    break;

                case "DSOX3000WavGen":
                    device = new DSOX3000WavGenLayer(devInfoStructure);
                    break;

                case "MOVE1250":
                    device = new MOVE1250Layer(devInfoStructure);
                    break;


                // Entry not found
                default:
                    break;
            }

            return device;
        }



        /// <summary>
        /// Uses the given parameters to initialize the timers.
        /// 
        /// Return 0 if everything gone well. A negative number indicates an error.
        /// </summary>
        /// <param name="MainFrame"></param>
        /// <param name="TimingStructure"></param>
        /// <param name="AdditionalInitialElapsedEvents"></param>
        /// <param name="AdditionalIterativeElapsedEvents"></param>
        /// <returns></returns>
        static public int InitTimers(FEMDAQ MainFrame, InfoBlockTiming TimingStructure, EventHandler[] AdditionalInitialElapsedEvents = null, EventHandler[] AdditionalIterativeElapsedEvents = null)
        {
            if (MainFrame.InitialTimer == null) throw new NullReferenceException("Missing Initialtimer");
            if (MainFrame.IterativeTimer == null) throw new NullReferenceException("Missing Iterativetimer");

            // Inital setup
            MainFrame.InitialTimer.Interval = TimingStructure.InitialTime;
            if (AdditionalInitialElapsedEvents != null)
            {
                foreach (EventHandler eventHandle in AdditionalInitialElapsedEvents)
                {
                    if (eventHandle != null)
                        MainFrame.InitialTimer.Tick += eventHandle;
                }
            }

            // Iterative setup
            MainFrame.IterativeTimer.Interval = TimingStructure.IterativeTime;
            if (AdditionalIterativeElapsedEvents != null)
            {
                foreach (EventHandler eventHandle in AdditionalIterativeElapsedEvents)
                {
                    if (eventHandle != null)
                        MainFrame.InitialTimer.Tick += eventHandle;
                }
            }


            return 0;
        }

    }
}
