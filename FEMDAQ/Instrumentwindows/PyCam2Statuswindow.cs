using FEMDAQ.StaticHelper;
using Instrument.LogicalLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FEMDAQ.Instrumentwindows.PyCam2Statuswindow
{
    public partial class PyCam2Statuswindow : Form
    {
        private PyCam2Layer _picam2 = null;
        public int LPad { get; set; } // Default = 22 -> Constructor

        public PyCam2Statuswindow(PyCam2Layer PiCam2Overlay)
        {
            InitializeComponent();

            _picam2 = PiCam2Overlay;
            LPad = 22;
            UpdateActiveDownloadsToolstrip();
            this.Text = string.Format("PyCam2 Status of {0}@{1}:{2}", _picam2.InfoBlock.Ip.Username, _picam2.InfoBlock.Ip.IP, _picam2.InfoBlock.Ip.Port);
        }


        public void UpdateActiveDownloadsToolstrip()
        {
            tsPiCam2ActiveDownloads.Text = "<Active>/<Started>".Replace("<Active>", _picam2._activeDownloads.ToString())
                                                               .Replace("<Started>", _picam2._startedDownloads.ToString()
                                                               );
        }

        
        public void OverrideLog(string PlainLogText, bool TimeStamp = true)
        {

            rtbPiCam2Status.Text = (TimeStamp ? DateTime.Now.ToString("yyMMdd - HH:mm:ss".PadRight(LPad)) : "") + PlainLogText;
            rtbPiCam2Status.ScrollToCaret();
        }


        public void Append2Log(string LogText, bool TimeStamp = true, bool AddNewLine = true)
        {

            string _appText = (TimeStamp ? DateTime.Now.ToString("yyMMdd - HH:mm:ss ".PadRight(LPad)) : "")   // Timestamp if configured
                            + LogText                                                                        // Actual log
                            + (AddNewLine ? "\r\n" : "");                                                      // New line if configured
            
            rtbPiCam2Status.AppendText(_appText);
            rtbPiCam2Status.ScrollToCaret();
        }
    }
}
