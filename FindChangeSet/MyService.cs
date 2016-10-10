using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace FindChangeSet
{
    public partial class MyService : ServiceBase
    {
        private FindChangeSet _findChangeSet;
        public MyService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _findChangeSet = new FindChangeSet();
            LoggerHelper.LogToLogFile("_findChangeSet instance created");
            _findChangeSet.StartHook();
            LoggerHelper.LogToLogFile("hook isStarted:" + _findChangeSet.IsStarted.ToString());
        }

        protected override void OnStop()
        {
            _findChangeSet.Dispose();
            _findChangeSet = null;
        }
    }
}
