using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace clipboard_helper
{
    class ClipboardOperator
    {

        #region External APIs
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        [DllImport("user32", EntryPoint = "VkKeyScan")]
        public static extern short VkKeyScan(byte cChar_Renamed);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(int hWnd);
        [DllImport("user32.dll")]
        private static extern int SetFocus(int hWnd);

        public const int WM_PASTE = 0x0302;

        public const byte VK_CONTROL = 0x11;
        public const int KEYEVENTF_KEYUP = 0x0002;

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;

        #endregion

        private const int MaxHistoryAmount = 5;
        private KeyboardHook _keyboardHook;

        private readonly List<string> _clipboardHistory = new List<string>();

        public ClipboardOperator()
        {
            _keyboardHook = new KeyboardHook();
            _keyboardHook.CopyToClipboard += GetNewInfoFromClipboard;
            _keyboardHook.MyPaste += OpenClipboardHistoryWindow;
        }

        private bool GetNewInfoFromClipboard()
        {
            var newInfo = GetTextFromClipboard();
            Debug.Print(newInfo);
            if (!string.IsNullOrEmpty(newInfo) && !_clipboardHistory.Contains(newInfo))
            {
                _clipboardHistory.Insert(0, newInfo);
            }
            if (_clipboardHistory.Count > MaxHistoryAmount)
            {
                _clipboardHistory.RemoveAt(MaxHistoryAmount - 1);
            }

            return false;
        }

        private bool OpenClipboardHistoryWindow()
        {
            string selectedText = string.Empty;

            var frmHistoryList = new FrmMyClipboard();
            int x = Math.Min(Control.MousePosition.X, Screen.PrimaryScreen.Bounds.Width - frmHistoryList.Width);
            int y = Math.Min(Control.MousePosition.Y, Screen.PrimaryScreen.Bounds.Height - frmHistoryList.Height);
            frmHistoryList.Location = new Point(x, y);
            frmHistoryList.populate(_clipboardHistory);
            frmHistoryList.PasteInfoSelected += s => selectedText = s;
            frmHistoryList.TopMost = true;
            if (DialogResult.OK == frmHistoryList.ShowDialog())
            {
                while (frmHistoryList.Created || frmHistoryList.IsHandleCreated)
                {
                    Application.DoEvents();
                }
                Paste(selectedText);
            }
            return false;
        }

        private string GetTextFromClipboard()
        {
            if (Clipboard.ContainsText())
            {
                return Clipboard.GetText();
            }
            else
            {
                return null;
            }
        }

        private void Paste(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var currentInfo = Clipboard.GetDataObject() ?? new DataObject();
            Clipboard.SetText(text);
            _keyboardHook.IsBusy = true;

            keybd_event(ClipboardOperator.VK_CONTROL, 0, 0, 0);
            keybd_event(0x43, 0, 0, 0); //Send the C key (43 is "C")
            keybd_event(0x43, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);// 'Left Control Up

            _keyboardHook.IsBusy = false;
            Clipboard.SetDataObject(currentInfo);
        }
    }
}
