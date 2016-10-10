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

namespace MyTool
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
        //private event Func<bool> CopyToClipboard;
        //private event Func<bool> MyPasteRequest;
        private FrmMyClipboard _frmHistoryList;
        private IntPtr _operatorWindowHandle = IntPtr.Zero;
        private readonly List<string> _clipboardHistory = new List<string>();

        public ClipboardOperator()
        {
        }

        public void OnCopyToClipboard(object sender, KeyEventArgs args)
        {
            GetNewCopyInfoAndUpdateHistoryList();
        }

        private void GetNewCopyInfoAndUpdateHistoryList()
        {
            string newCopyInfo = null;

            //使用以下子线程读取Clipboard.GetText()会永远是null，因为clipboard是区分线程的！
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
            //这样的时间设置--> run only once  ：  timer = new Timer(TimerCallback, null, 1000, Timeout.Infinite);


            //直接使用Clipboard.GetText()会取得上一次copy的string，估计是异步copy到clipboard未完成，直接加上thread.Sleep(500)会卡住当前操作文件，同时卡住了copy的动作，所以用thread.Sleep(500)是无效的！
            //2016-30-20  再次试验直接 Clipboard.GetText(); 居然可以取到了，而且不需要任何sleep()等待。。。？？？ 难道之前是由于多次启动 SetWindowsHookEx() 引发？？？
            //so : 可以简单如下了？：
            //if (Clipboard.ContainsText())
            //{
            //    newCopyInfo = Clipboard.GetText();
            //}

            //解决方案：
            //让子线运行在父线程中，即STA
            //1. 使用Task的TaskScheduler
            var task = Task.Factory.StartNew(() =>
            {
                //Thread.Sleep(500); //2016-30-20 不需要了，见上

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
                    while (_clipboardHistory.Count >= MaxHistoryAmount)
                    {
                        _clipboardHistory.RemoveAt(_clipboardHistory.Count - 2);
                    }

                    if (_clipboardHistory.Count > 1)
                    {
                        var lastCopyInfo = _clipboardHistory[_clipboardHistory.Count - 1];
                        _clipboardHistory.RemoveAt(_clipboardHistory.Count - 1);
                        _clipboardHistory.Insert(0, lastCopyInfo);
                    }
                    if (_clipboardHistory.Contains(newCopyInfo))
                        _clipboardHistory.Remove(newCopyInfo);
                    _clipboardHistory.Add(newCopyInfo);
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

        //对于ctrl + V失效的情况： 即使true也没法吞掉ctrl + V, 而是因为_frmHistoryList成为了当前window而使粘帖没有发生在目标窗体上
        //在粘帖起效前，使用SetFocus(IntPtr.Zero);防止快捷键字符输入到目标窗体上
        public void OnMyPasteRequest(object sender, KeyEventArgs args)
        {
            SetFocus(IntPtr.Zero);

            if (_frmHistoryList != null && _frmHistoryList.Visible)
            {
                _frmHistoryList.MoveToNextHistory();
                SetFocus(_frmHistoryList.Handle);
            }
            else
            {
                _operatorWindowHandle = GetForegroundWindow();
                ShowClipboardHistoryWindow();
            }
        }

        private void ShowClipboardHistoryWindow()
        {
            if (_frmHistoryList == null || !_frmHistoryList.Created)
            {
                _frmHistoryList = new FrmMyClipboard();
                _frmHistoryList.PasteInfoSelected += Paste;
                _frmHistoryList.TopMost = true;
                SetForegroundWindow(_frmHistoryList.Handle);
            }

            int x = Math.Min(Control.MousePosition.X, Screen.PrimaryScreen.Bounds.Width - _frmHistoryList.Width);
            int y = Math.Min(Control.MousePosition.Y, Screen.PrimaryScreen.Bounds.Height - _frmHistoryList.Height);
            _frmHistoryList.Location = new Point(x, y);
            var historyList = new List<string>();
            historyList.AddRange(_clipboardHistory);
            historyList.AddRange(XmlHelper.ClipboardHelperInfo.PredifinedCopies);
            _frmHistoryList.populate(historyList);
        }

        public void OnMyPasteText(string pasteText, string removeText = null, int removeHotkeyLength = 1)
        {
            //remove the hotkey input
            for (int i = 0; i < removeHotkeyLength; i++)
            {
                keybd_event((byte)Keys.Back, 0, 0, 0);
                keybd_event((byte)Keys.Back, 0, KEYEVENTF_KEYUP, 0);
            }

            //paste
            _operatorWindowHandle = GetForegroundWindow();
            Paste(pasteText, removeText);
        }

        private void Paste(string text, string removeText = null)
        {
            if (string.IsNullOrEmpty(text) && _operatorWindowHandle == IntPtr.Zero)
            {
                return;
            }

            //简单实现取得clipboard中文本信息：可以使用winform.Invoke()实现UI线程中Clipboard.GetText();
            //var newTask = Task.Run(() => _frmHistoryList.Invoke(new Action(() =>
            //  {...
            //      Clipboard.GetText(); ...
            //  })));

            string pasteText = text;
            string onClipboardText = _clipboardHistory.Any() ?  _clipboardHistory[0] : null;

            var newTask = Task.Factory.StartNew(() =>
            {
                //无法不区分类型地先备份再复原当前copy的DataObject。。。
                //var currentInfo = Clipboard.GetData(DataFormats.Text) ?? string.Empty;
                //Clipboard.SetData(DataFormats.Text, currentInfo);

                //Clipboard.SetText(pasteText);
                Clipboard.SetDataObject(pasteText, true);
                Thread.Sleep(100);

                SetForegroundWindow(_operatorWindowHandle);
                SetFocus(_operatorWindowHandle);
                //对于word/skipy等输入窗口，不sleep()会导致ctrl + V被丢失
                Thread.Sleep(20);
                keybd_event(ClipboardOperator.VK_CONTROL, 0, 0, 0);
                keybd_event((byte)Keys.V, 0, 0, 0); //Send the V key
                Thread.Sleep(10);
                keybd_event((byte)Keys.V, 0, KEYEVENTF_KEYUP, 0);
                keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0); // 'Left Control Up
                Thread.Sleep(20);

                //Clipboard.SetText(onClipboardText);
                if (!string.IsNullOrEmpty(onClipboardText))
                    Clipboard.SetDataObject(onClipboardText, true);
                //不加此步骤会在下次copy后GetText()时报错。。。原因不明
                Clipboard.GetText();

            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext())
            //to select the specified text
            .ContinueWith(task =>
            {
                if (!string.IsNullOrEmpty(removeText) && pasteText.Contains(removeText))
                {
                    int startIndex = pasteText.LastIndexOf(removeText);
                    int endIndex = startIndex + removeText.Length;
                    for (int i = pasteText.Length; i > endIndex; i--)
                    {
                        keybd_event((byte)Keys.Left, 0, 0, 0);
                        keybd_event((byte)Keys.Left, 0, KEYEVENTF_KEYUP, 0);
                    }
                    for (int i = 0; i < removeText.Length; i++)
                    {
                        keybd_event((byte)Keys.Back, 0, 0, 0);
                        keybd_event((byte)Keys.Back, 0, KEYEVENTF_KEYUP, 0);
                    }

                    //keybd_event((byte)Keys.ShiftKey, 0, 0, 0);
                    //for (int i = 0; i < removeText.Length; i++)
                    //{
                    //    keybd_event((byte)Keys.Left, 0, 0, 0);
                    //    keybd_event((byte)Keys.Left, 0, KEYEVENTF_KEYUP, 0);
                    //}
                    //keybd_event((byte)Keys.ShiftKey, 0, KEYEVENTF_KEYUP, 0);
                }
            });
        }

        public void Dispose()
        {
            if (_frmHistoryList != null)
            {
                _frmHistoryList.Close();
            }
        }
    }
}
