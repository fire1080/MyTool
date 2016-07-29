using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MyTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // make sure that one instance is running at the time
            bool isFirstInstance = false;
            var mutex = new Mutex(true, "Mytool", out isFirstInstance);
            if (isFirstInstance)
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var frmMain = new FrmMain();
                Application.Run(); //no need Form1 : Application.Run(new Form1());
            }
            else
            {
                mutex.Close();
            }
        }
    }
}
