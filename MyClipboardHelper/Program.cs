using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using MyClipboardHelper.Properties;

namespace clipboard_helper
{
    static class Program
    {
        static ClipboardOperator clipboardOperator = new ClipboardOperator();
        static NotifyIcon ni;
        //private static Mutex mut = new Mutex(); // make sure that one instance is running at the time

        static void onNotifyIconClick(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Right)
            {
                if (MessageBox.Show("Finish?", "Clipboard Little Helper", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ni.Visible = false;
                    clipboardOperator.Dispose();
                    Application.Exit();
                }
            }
        }
        [STAThread]
        static void Main()
        {
            //mut.WaitOne();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            ni = new NotifyIcon();
            ni.Icon = Resources.Icon1;
            ni.Text = "Clipboard Little Helper";
            ni.Click += new EventHandler(onNotifyIconClick);
            ni.Visible = true;
            Application.Run();
            //mut.ReleaseMutex();
        }        
    }
}