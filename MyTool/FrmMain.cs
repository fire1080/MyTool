using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows.Forms;
using FindChangeSet;
using MouseKeyboardLibrary;
using System.Threading.Tasks;

namespace MyTool
{
    public class FrmMain : NativeWindow, IDisposable
    {
        private KeyboardHook _keyboardHook = null;
        private KeyboardHook.HotKey _findChangeSetHotKey = null;

        private FrmFindChangeSet _frmFindChangeSet = null;
        private FrmOpenICCIncident _frmOpenICCIncident = null;
        private FrmOpenEverything _frmOpenEverything = null;
        private ClipboardOperator _clipboardOperator = null;

        public FrmMain()
        {
            _keyboardHook = new KeyboardHook();
            _keyboardHook.RegisterHotKey(new KeyboardHook.HotKey() { Control = true, Shift = true, KeyCode = Keys.D1, KeyUp = new KeyEventHandler(FindChangeSetProc) });
            _keyboardHook.RegisterHotKey(new KeyboardHook.HotKey() { Control = true, Shift = true, KeyCode = Keys.D2, KeyUp = new KeyEventHandler(OpenIccInstant) });
            _keyboardHook.RegisterHotKey(new KeyboardHook.HotKey() { Control = true, Shift = true, KeyCode = Keys.Oem3, KeyUp = new KeyEventHandler(OpenEverything) });
            _clipboardOperator = new ClipboardOperator();
            _keyboardHook.RegisterHotKey(new KeyboardHook.HotKey() { Control = true, KeyCode = Keys.C, KeyUp = new KeyEventHandler(_clipboardOperator.OnCopyToClipboard), Interval = 100 });
            _keyboardHook.RegisterHotKey(new KeyboardHook.HotKey() { Win = true, KeyCode = Keys.V, KeyUp = new KeyEventHandler(_clipboardOperator.OnMyPasteRequest), Interval = 100 });
            const string pasteSelectSql = @"Select * from T1 t order by t.Id;";
            _keyboardHook.RegisterHotKey(new KeyboardHook.HotKey() { Win = true, KeyCode = Keys.Q, KeyUp = (sender, args) => _clipboardOperator.OnMyPasteText(pasteSelectSql, "T1"), Interval = 100 });
            _keyboardHook.Start();
            Debug.Print(_keyboardHook.IsStarted.ToString());
        }

        private void FindChangeSetProc(object sender, KeyEventArgs args)
        {
            if (_frmFindChangeSet == null || !_frmFindChangeSet.Created)
            {
                _frmFindChangeSet = new FrmFindChangeSet();
            }
            _frmFindChangeSet.WindowState = FormWindowState.Normal;
            _frmFindChangeSet.TopMost = true;
            _frmFindChangeSet.Show();
            _frmFindChangeSet.Focus();

            args.Handled = true;
        }

        private void OpenIccInstant(object sender, KeyEventArgs args)
        {
            if (_frmOpenICCIncident == null || !_frmOpenICCIncident.Created)
            {
                _frmOpenICCIncident = new FrmOpenICCIncident();
            }
            _frmOpenICCIncident.WindowState = FormWindowState.Normal;
            _frmOpenICCIncident.TopMost = true;
            _frmOpenICCIncident.Show();
            _frmOpenICCIncident.Focus();
            args.Handled = true;
        }

        private void OpenEverything(object sender, KeyEventArgs args)
        {
            if (_frmOpenEverything == null || !_frmOpenEverything.Created)
            {
                _frmOpenEverything = new FrmOpenEverything();
            }
            _frmOpenEverything.WindowState = FormWindowState.Normal;
            _frmOpenEverything.TopMost = true;
            _frmOpenEverything.Show();
            _frmOpenEverything.Focus();
            args.Handled = true;
        }

        public void Dispose()
        {
            if (_frmFindChangeSet != null)
            {
                _frmFindChangeSet.Dispose();
            }
            if (_frmOpenICCIncident != null)
            {
                _frmOpenICCIncident.Dispose();
            }
            if (_keyboardHook != null)
            {
                if (_keyboardHook.IsStarted) 
                    _keyboardHook.Stop();
                _keyboardHook.Dispose();
            }
            _keyboardHook = null;

            XmlHelper.SaveMyToolInfo();
            this.DestroyHandle();
        }
    }
}
