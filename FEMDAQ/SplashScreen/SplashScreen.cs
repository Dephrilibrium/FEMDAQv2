using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FEMDAQ.SplashScreen
{
    public partial class SplashScreenFrame : Form
    {
        private Timer _splashScreenTimer;

        public SplashScreenFrame()
        {
            InitializeComponent();

            lToolname.Text = Application.ProductName;
            lToolbuild.Text = "Build: V" + Application.ProductVersion;
            TopMost = true;
            BringToFront();

            _splashScreenTimer = new Timer();
            _splashScreenTimer.Interval = 3000;
            _splashScreenTimer.Tick += SplashScreenShow_TickEvent;
            _splashScreenTimer.Start();
        }


        private void SplashScreenShow_TickEvent(object sender, EventArgs e)
        {
            _splashScreenTimer.Stop();
            _splashScreenTimer.Dispose();
            Close();
        }
    }
}
