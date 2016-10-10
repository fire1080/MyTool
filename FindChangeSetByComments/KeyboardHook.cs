using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MouseKeyboardLibrary
{

    /// <summary>
    /// Captures global keyboard events
    /// </summary>
    public class KeyboardHook : GlobalHook, IDisposable
    {

        #region Events

        //public event KeyEventHandler KeyDown;
        //public event KeyEventHandler KeyUp;
        //public event KeyPressEventHandler KeyPress;

        #endregion

        #region Constructor

        public KeyboardHook()
        {
            _hookType = WH_KEYBOARD_LL;
        }

        #endregion

#region HotKey

        public class HotKey
        {
            public bool Control;
            public bool Alt;
            public bool Shift;
            //public bool Capslock;
            public Keys KeyCode;

            public KeyEventHandler KeyDown;
            public KeyEventHandler KeyUp;
            public KeyPressEventHandler KeyPress;

            public int Id
            {
                get
                {
                    return (int) KeyCode |
                           (Control ? (int) Keys.Control : 0) |
                           (Shift ? (int) Keys.Shift : 0) |
                           (Alt ? (int)Keys.Alt : 0);
                }
            }

            public bool Valid
            {
                get { return KeyCode != 0 && Active; }
            }

            private bool Active
            {
                get { return (KeyDown != null || KeyUp != null || KeyPress != null); }
            }

        }

        private List<Keys> _hotKeyCodes = new List<Keys>();

        private readonly Dictionary<int, HotKey> _hotKeyDic = new Dictionary<int, HotKey>();

        public void RegisterHotKey(HotKey hotKey)
        {
            if (!hotKey.Valid)
                throw new ArgumentException("Invalid HotKey");
            var hotKeyId = hotKey.Id;
            if (!_hotKeyDic.ContainsKey(hotKeyId))
                _hotKeyDic.Add(hotKeyId, hotKey);
            _hotKeyCodes = _hotKeyDic.Values.Select(k => k.KeyCode).Distinct().ToList();
        }

        public void UnRegisterHotKey(HotKey hotKey)
        {
            if (!hotKey.Valid)
                throw new ArgumentException("Invalid HotKey");
            var hotKeyId = hotKey.Id;
            if (!_hotKeyDic.ContainsKey(hotKeyId))
                _hotKeyDic.Remove(hotKeyId);
            _hotKeyCodes = _hotKeyDic.Values.Select(k => k.KeyCode).Distinct().ToList();
        }

#endregion

        #region Methods

        protected override int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
        {
            bool handled = false;
            if (nCode > -1 && (wParam == WM_KEYDOWN || wParam == WM_KEYUP) && _hotKeyDic.Any())
            {
                KeyboardHookStruct keyboardHookStruct =
                    (KeyboardHookStruct) Marshal.PtrToStructure(lParam, typeof (KeyboardHookStruct));

                if (_hotKeyCodes.Contains((Keys) keyboardHookStruct.vkCode))
                {
#if DEBUG
                    Debug.Print(string.Format("nCode:{0},wParam:{1},lParam:{2}, vkCode:{3}", nCode.ToString("x2"),
                        wParam.ToString("x2"),
                        lParam.ToString("x2"), keyboardHookStruct.vkCode.ToString()));
#endif

                    // Is Control being held down?
                    bool control = ((GetKeyState(VK_LCONTROL) & 0x80) != 0) ||
                                   ((GetKeyState(VK_RCONTROL) & 0x80) != 0);

                    //Debug.Print(string.Format("GetKeyState(VK_LCONTROL):{0},GetKeyState(VK_RCONTROL):{1},GetKeyState(VK_CONTROL)", GetKeyState(VK_LCONTROL).ToString(), GetKeyState(VK_RCONTROL).ToString()));

                    // Is Shift being held down?
                    bool shift = ((GetKeyState(VK_LSHIFT) & 0x80) != 0) ||
                                 ((GetKeyState(VK_RSHIFT) & 0x80) != 0);

                    // Is Alt being held down?
                    bool alt = ((GetKeyState(VK_LALT) & 0x80) != 0) ||
                               ((GetKeyState(VK_RALT) & 0x80) != 0);

                    // Is CapsLock on?
                    bool capslock = (GetKeyState(VK_CAPITAL) != 0);

                    // Create event using keycode and control/shift/alt values found above
                    KeyEventArgs e = new KeyEventArgs(
                        (Keys) (
                            keyboardHookStruct.vkCode |
                            (control ? (int) Keys.Control : 0) |
                            (shift ? (int) Keys.Shift : 0) |
                            (alt ? (int) Keys.Alt : 0)
                            ));

                    //Find Matched HotKeys
                    var matchedHotKeys =
                        _hotKeyDic.Values.Where(
                            h =>
                                h.Control == control && h.Alt == alt && h.Shift == shift &&
                                h.KeyCode == (Keys) keyboardHookStruct.vkCode).ToList();

                    foreach (var hotKey in matchedHotKeys)
                    {
                        // Handle KeyDown and KeyUp events
                        switch (wParam)
                        {

                            case WM_KEYDOWN:
                            case WM_SYSKEYDOWN:
                                if (hotKey.KeyDown != null)
                                {
                                    hotKey.KeyDown(this, e);
                                    handled = handled || e.Handled;
                                }
                                break;
                            case WM_KEYUP:
                            case WM_SYSKEYUP:
                                if (hotKey.KeyUp != null)
                                {
                                    hotKey.KeyUp(this, e);
                                    handled = handled || e.Handled;
                                }
                                break;

                        }

                        // Handle KeyPress event
                        if (wParam == WM_KEYDOWN &&
                            !handled &&
                            !e.SuppressKeyPress &&
                            hotKey.KeyPress != null)
                        {

                            byte[] keyState = new byte[256];
                            byte[] inBuffer = new byte[2];
                            GetKeyboardState(keyState);

                            if (ToAscii(keyboardHookStruct.vkCode,
                                keyboardHookStruct.scanCode,
                                keyState,
                                inBuffer,
                                keyboardHookStruct.flags) == 1)
                            {

                                char key = (char) inBuffer[0];
                                if ((capslock ^ shift) && Char.IsLetter(key))
                                    key = Char.ToUpper(key);
                                KeyPressEventArgs e2 = new KeyPressEventArgs(key);
                                hotKey.KeyPress(this, e2);
                                handled = handled || e.Handled;
                            }
                        }
                    }
                }
            }

            if (handled)
            {
                return 1;
            }
            else
            {
                return CallNextHookEx(_handleToHook, nCode, wParam, lParam);
            }
        }
        #endregion

        public void Dispose()
        {
            //UnRegisterHotKey
            _hotKeyDic.Clear();
            _hotKeyCodes.Clear();

            //Stop Hook
            this.Stop();
        }
    }

}
