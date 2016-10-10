//#undef DEBUG
#define DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MouseKeyboardLibrary;

namespace FindChangeSet
{
    class FindChangeSet : IDisposable
    {
        private KeyboardHook _keyboardHook = new KeyboardHook();
        private FindChangeSetForm _findChangeSetForm = null;

        public bool IsStarted
        {
            get {
                return _keyboardHook != null && _keyboardHook.IsStarted;
            }
        }

        public FindChangeSet()
        {
            _keyboardHook.KeyUp += new KeyEventHandler(KeyUpProc);
        }

        public void StartHook()
        {
            _keyboardHook.Start();
        }

        public void StopHook()
        {
            _keyboardHook.Stop();
        }

        private void KeyUpProc(object sender, KeyEventArgs args)
        {
#if DEBUG
            LoggerHelper.LogToLogFile(string.Format("isControl:{0}, isAlt:{1}, isShift:{2}, KeyCode:{3}, ",args.Control, args.Alt, args.Shift,args.KeyCode.ToString()));
#endif
            if (!args.Alt || args.KeyCode != Keys.C) return;
            if (_findChangeSetForm == null || !_findChangeSetForm.IsHandleCreated)
            {
                _findChangeSetForm = new FindChangeSetForm();
                _findChangeSetForm.ShowDialog();
            }
            else
            {
                _findChangeSetForm.BringToFront();
                _findChangeSetForm.Focus();
            }
        }

        ~FindChangeSet()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_findChangeSetForm != null && _findChangeSetForm.IsHandleCreated)
            {
                _findChangeSetForm.Dispose();
                _findChangeSetForm = null;
            }
            if (_keyboardHook != null && _keyboardHook.IsStarted)
            {
                _keyboardHook.Stop();
                _keyboardHook = null;
            }
        }
    }
}
