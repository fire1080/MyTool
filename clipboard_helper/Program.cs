using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace clipboard_helper
{
    static class Program
    {
        static ClipboardOperator clipboardOperator = new ClipboardOperator();
        static NotifyIcon ni;
        private static Mutex mut = new Mutex(); // make sure that one instance is running at the time

        static void onNotifyIconClick(object sender, EventArgs e)
        {

            if (((MouseEventArgs)e).Button == MouseButtons.Right)
            {
                if (MessageBox.Show("Finish?", "Clipboard Little Helper", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ni.Visible = false;
                    Application.Exit();
                }
            }
        }
        [STAThread]
        static void Main()
        {
            mut.WaitOne();
            MessageBox.Show("Clipboard helper has started", "Clipboard Little Helper", MessageBoxButtons.OK);
            ni = new NotifyIcon();            
            ni.Icon = new System.Drawing.Icon(typeof(Program), "Icon1.ico");
            ni.Text = "Clipboard Little Helper";
            ni.Click += new EventHandler(onNotifyIconClick);
            ni.Visible = true;
            Application.Run();
            mut.ReleaseMutex();
        }        
    }
}