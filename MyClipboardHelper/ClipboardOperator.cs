using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clipboard_helper
{
    class ClipboardOperator : IDisposable
    {

        #region External APIs
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        [DllImport("user32", EntryPoint = "VkKeyScan")]
        public static extern short VkKeyScan(byte cChar_Renamed);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern int SetFocus(IntPtr hWnd);

        //This Function is used to get Active Window Title...
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hwnd, string lpString, int cch);
        //This Function is used to get Handle for Active Window...
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetForegroundWindow();
        //This Function is used to get Active process ID...
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out Int32 lpdwProcessId);

        public const int WM_PASTE = 0x0302;

        public const byte VK_CONTROL = 0x11;
        public const int KEYEVENTF_KEYUP = 0x0002;

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;

        #endregion

        private const int MaxHistoryAmount = 5;
        private KeyboardHook _keyboardHook;
        private FrmMyClipboard _frmHistoryList;
        private IntPtr _operatorWindowHandle = IntPtr.Zero;
        private readonly List<string> _clipboardHistory = new List<string>();

        public ClipboardOperator()
        {
            _keyboardHook = new KeyboardHook();
            _keyboardHook.CopyToClipboard += OnCopyToClipboard;
            _keyboardHook.MyPaste += OnMyPasteRequest;

        }

        private bool OnCopyToClipboard()
        {
            GetNewCopyInfoAndUpdateHistoryList();
            return true;
        }

        private void GetNewCopyInfoAndUpdateHistoryList()
        {
            string newCopyInfo = null;
            
            //直接使用Clipboard.GetText()会取得上一次copy的string，估计是异步copy到clipboard未完成（主线程中的异步copy，所以用thread.Sleep(500)是无效的！）
            //使用以下子线程读取Clipboard.GetText()会永远是null，因为clipboard是分线程的！
            //var newTask =Task.Run(() =>
            //{
            //    Thread.Sleep(500);
            //    if (Clipboard.ContainsText())
            //    {
            //        newCopyInfo = Clipboard.GetText();
            //    }
            //});
            //newTask.Wait();

            //使用System.Threading.Timer 或  System.Timers.Timer 同样不行!
            //System.Threading.Timer t = new System.Threading.Timer(new TimerCallback(o =>
            //{
            //    if (Clipboard.ContainsText())
            //    {
            //        newCopyInfo = Clipboard.GetText();
            //    }

            //    Debug.Print(newCopyInfo);
            //    if (!string.IsNullOrEmpty(newCopyInfo) && !_clipboardHistory.Contains(newCopyInfo))
            //    {
            //        _clipboardHistory.Insert(0, newCopyInfo);
            //    }
            //    if (_clipboardHistory.Count > MaxHistoryAmount)
            //    {
            //        _clipboardHistory.RemoveAt(MaxHistoryAmount - 1);
            //    }

            //}),null, 500, Timeout.Infinite);
            //这样的时间设置--> run only once  ：  timer = new Timer(CreatorLoop, null, 1000, Timeout.Infinite);

            //解决方案：
            //让子线运行在父线程中，即STA
            //1. 使用Task的TaskScheduler
            var task = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                var data = Clipboard.GetDataObject();
                if(data.GetDataPresent(DataFormats.Text) | data.GetDataPresent(DataFormats.OemText))
                {
                    newCopyInfo = (string)data.GetData(DataFormats.Text);
                }
                //效果同下：
                //if (Clipboard.ContainsText())
                //{
                //    newCopyInfo = Clipboard.GetText();
                //}

                Debug.Print(newCopyInfo);
                if (!string.IsNullOrEmpty(newCopyInfo))
                {
                    if (_clipboardHistory.Contains(newCopyInfo))
                        _clipboardHistory.Remove(newCopyInfo);
                    _clipboardHistory.Insert(0, newCopyInfo);
                }
                if (_clipboardHistory.Count > MaxHistoryAmount)
                {
                    _clipboardHistory.RemoveAt(MaxHistoryAmount - 1);
                }
            }
            , CancellationToken.None
            , TaskCreationOptions.None
            , TaskScheduler.FromCurrentSynchronizationContext());

            ////2. 使用thread
            //var newThread = new Thread(new ThreadStart(() =>
            //{
            //    Thread.Sleep(300);
            //    if (Clipboard.ContainsText())
            //    {
            //        newCopyInfo = Clipboard.GetText();
            //    }

            //    Debug.Print(newCopyInfo);
            //    if (!string.IsNullOrEmpty(newCopyInfo) && !_clipboardHistory.Contains(newCopyInfo))
            //    {
            //        _clipboardHistory.Insert(0, newCopyInfo);
            //    }
            //    if (_clipboardHistory.Count > MaxHistoryAmount)
            //    {
            //        _clipboardHistory.RemoveAt(MaxHistoryAmount - 1);
            //    }

            //}));
            //newThread.SetApartmentState(ApartmentState.STA);
            //newThread.Start();

            //3. 使用现有windows form 的 Invoke() （如下Paste（）方法）
        }

        //即使true也没法吞掉ctrl + V, 而是在因为_frmHistoryList成为当前window而使之无效
        //在ctrl + V起效前，使用SetFocus(IntPtr.Zero);防止粘帖发生
        private bool OnMyPasteRequest()
        {
            SetFocus(IntPtr.Zero);

            if (_frmHistoryList != null && _frmHistoryList.Visible)
            {
                SetFocus(_frmHistoryList.Handle);
                _frmHistoryList.MoveToNextHistory();
            }
            else
            {
                _operatorWindowHandle = GetForegroundWindow();
                ShowClipboardHistoryWindow();
            }
            return true;
        }

        private void ShowClipboardHistoryWindow()
        {
            if (_frmHistoryList == null || !_frmHistoryList.Created)
            {
                _frmHistoryList = new FrmMyClipboard();
                _frmHistoryList.PasteInfoSelected += Paste;
                _frmHistoryList.TopMost = true;
            }

            int x = Math.Min(Control.MousePosition.X, Screen.PrimaryScreen.Bounds.Width - _frmHistoryList.Width);
            int y = Math.Min(Control.MousePosition.Y, Screen.PrimaryScreen.Bounds.Height - _frmHistoryList.Height);
            _frmHistoryList.Location = new Point(x, y);
            _frmHistoryList.populate(_clipboardHistory);
        }



        private void Paste(string text)
        {
            if (string.IsNullOrEmpty(text) && _operatorWindowHandle == IntPtr.Zero)
            {
                return;
            }

            //可以使用winform.Invoke()实现UI线程中Clipboard.GetText();
            //var newTask = Task.Run(() => _frmHistoryList.Invoke(new Action(() =>
            //  {...
            //      Clipboard.GetText(); ...
            //  })));

            string pasteText = text;
            string onClipboardText = _clipboardHistory.Any() ?  _clipboardHistory[0] : text;

            var newTask = Task.Factory.StartNew(() =>
            {
                //无法不区分类型地先备份再复原当前copy的DataObject。。。
                //var currentInfo = Clipboard.GetData(DataFormats.Text) ?? string.Empty;
                //Clipboard.SetData(DataFormats.Text, currentInfo);

                //Clipboard.SetText(pasteText);
                Clipboard.SetDataObject(pasteText, true);
                Thread.Sleep(200);

                SetForegroundWindow(_operatorWindowHandle);
                SetFocus(_operatorWindowHandle);
                //对于word/skipy等输入窗口，不sleep()会导致ctrl + V被丢失
                Thread.Sleep(20);
                keybd_event(ClipboardOperator.VK_CONTROL, 0, 0, 0);
                keybd_event((byte)Keys.V, 0, 0, 0); //Send the V key
                Thread.Sleep(20);
                keybd_event((byte)Keys.V, 0, KEYEVENTF_KEYUP, 0);
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0); // 'Left Control Up
                Thread.Sleep(20);

                //Clipboard.SetText(onClipboardText);
                Clipboard.SetDataObject(onClipboardText, true);
                //不加此步骤会在下次copy后GetText()时报错。。。原因不明
                Clipboard.GetText();

            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Dispose()
        {
            if (_keyboardHook != null)
            {
                _keyboardHook.Unhook();
            }
            _keyboardHook = null;

            if (_frmHistoryList != null)
            {
                _frmHistoryList.Close();
            }
        }
    }
}
