using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using FindChangeSet;
using MouseKeyboardLibrary;

namespace FindChangeSetByComments
{
    public partial class Form1 : Form
    {
        private KeyboardHook _keyboardHook = null;
        public frmFindChangeSet _frmFindChangeSet = null;
        private KeyboardHook.HotKey _findChangeSetHotKey = null;
        public Form1()
        {
            InitializeComponent();

            _findChangeSetHotKey = new KeyboardHook.HotKey()
            {
                Control = true,
                Shift = true,
                KeyCode = Keys.C,
                KeyUp = new KeyEventHandler(FindChangeSetProc)
            };
            _keyboardHook = new KeyboardHook();
            _keyboardHook.RegisterHotKey(_findChangeSetHotKey);
            _keyboardHook.Start();
            Debug.Print(_keyboardHook.IsStarted.ToString());
        }


        private void FindChangeSetProc(object sender, KeyEventArgs args)
        {
            if (_frmFindChangeSet == null || !_frmFindChangeSet.IsHandleCreated)
            {
                _frmFindChangeSet = new frmFindChangeSet();
            }
            _frmFindChangeSet.WindowState = FormWindowState.Normal;
            _frmFindChangeSet.Show();
            _frmFindChangeSet.BringToFront();
            _frmFindChangeSet.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_keyboardHook != null)
                _keyboardHook.Dispose();
            _keyboardHook = null;

            if (_frmFindChangeSet != null)
                _frmFindChangeSet.Close();
            _frmFindChangeSet = null;
        }
    }
}
