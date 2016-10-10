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
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

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

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            string errorStr = "";
            string strDateInfo = "错误时间：" + DateTime.Now + "\r\n";
            Exception error = e.Exception as Exception;
            if (error != null)
            {
                errorStr = string.Format(strDateInfo + "错误类型：{0}\r\n错误消息：{1}\r\n错误信息：{2}\r\n",
                     error.GetType().Name, error.Message, error.StackTrace);
            }
            else
            {
                errorStr = string.Format("应用程序线程错误:{0}", e);
            }

            MessageBox.Show("发生致命错误！" + Environment.NewLine + errorStr, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string errorStr = "";
            Exception error = e.ExceptionObject as Exception;
            string strDateInfo = "错误时间：" + DateTime.Now + "\r\n";
            if (error != null)
            {
                errorStr = string.Format(strDateInfo + "错误消息:{0};\n\r堆栈信息:{1}", error.Message, error.StackTrace);
            }
            else
            {
                errorStr = string.Format("错误消息:{0}", e);
            }

            MessageBox.Show("发生致命错误！" + Environment.NewLine + errorStr, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        } 
    }
}
