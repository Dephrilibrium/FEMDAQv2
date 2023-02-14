using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FEMDAQ
{
    static class Program
    {
        static FEMDAQ mainFrame = null;
        static bool firstTryFatalShutdown = true;

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new ThreadExceptionEventHandler(CatchUnhandledException);

            mainFrame = new FEMDAQ();
            Application.Run(mainFrame);
        }



        /// <summary>
        /// Catches all unhandled exception (fatal errors).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CatchUnhandledException(object sender, ThreadExceptionEventArgs e)
        {
            var fatalExceptionSavePath = Application.StartupPath + @"\fatalException\" + DateTime.Now.ToString("yyMMdd_HHmmss");

            // Try to save results by normal shutdown on first try! Will may not work!
            //  When it won't work a force shutdown method will be necessary!
            if (firstTryFatalShutdown)
            {
                firstTryFatalShutdown = false;

                if (Directory.Exists(fatalExceptionSavePath) == false)
                    Directory.CreateDirectory(fatalExceptionSavePath);
                var fileWriter = new StreamWriter(fatalExceptionSavePath + @"\fatalException.log");
                fileWriter.Write(e.Exception.ToString());
                fileWriter.Dispose();

                mainFrame.SaveFolderFullPath = fatalExceptionSavePath;
                mainFrame.ExternalStop();
            }


            MessageBox.Show(string.Format("The program ran into an instable state caused by an unhandled exception (see below). After clicking ok, the acutal data will be (if possible) saved to {0} including a logfile which contains information to the unhandled exception.\n\n Exceptioninfo: {1}", fatalExceptionSavePath, e.Exception.ToString()),
              "Fatal error: Unhandled exception occured!",
              MessageBoxButtons.OK,
              MessageBoxIcon.Error);

            //// Try to save results otherwise catch error and save as another log
            //try
            //{
            //    mainFrame.SaveResultsFromQueue(fatalExceptionSavePath);
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show(string.Format("Saving the results failed!", fatalExceptionSavePath, e.Exception.ToString()),
            //        "Fatal error: Unhandled exception occured!",
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Error);

            //    fileWriter = new StreamWriter(fatalExceptionSavePath + @"\fatalException - saving failed.log");
            //    fileWriter.Write(e.Exception.ToString());
            //    fileWriter.Dispose();
            //}

            Application.Exit();
        }
    }
}
