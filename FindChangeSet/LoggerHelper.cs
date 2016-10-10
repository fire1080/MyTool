using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FindChangeSet
{
    class LoggerHelper
    {
        private static string _logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyServiceLog.log");
        public static void LogToLogFile(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                msg = string.Format(@"{0} {1}: 
{2}
", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), msg);
                File.AppendAllText(_logFile, msg);
            }
        }
    }
}
