using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Runtime.InteropServices;

namespace clipboard_helper
{
    public class KeyboardHook
    {
        public KeyboardHook()
        {            
            Hook();
        }
        ~KeyboardHook()
        {
            Unhook();
        }
        //public event 
        public event Func<bool> CopyToClipboard = null;
        public event Func<bool> MyPaste = null; 

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        IntPtr hhook = IntPtr.Zero;
        private delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);
        private keyboardHookProc m_keyboardHookProc;

        public struct keyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;

        public bool Hook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            m_keyboardHookProc = new keyboardHookProc(hookProc);
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, m_keyboardHookProc, hInstance, 0);
            return (hhook.ToInt32() != 0);
        }
        public void Unhook()
        {
            UnhookWindowsHookEx(hhook);
        }

        bool ctrl_pressed = false;
        bool shft_pressed = false;
        bool v_pressed = false;
        bool c_pressed = false;
        bool sth_else = false;

        bool busy=false;

        public void Suspend()
        {
            busy = true;
        }
        public void Resume()
        {
            busy = false;
        }

        //public bool IsBusy
        //{
        //    get { return busy; }
        //    set { this.busy = value; }
        //}

        private int hookProc(int code, int wParam, ref keyboardHookStruct lParam)
        {
            if (busy)
                return 0;

            busy = true;
            bool isHandled = false;

            if (code >= 0)
            {
                Keys key = (Keys) lParam.vkCode;
                if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)
                {
                    switch (key)
                    {
                        case Keys.LControlKey:
                        case Keys.RControlKey:
                            ctrl_pressed = true;
                            break;
                        case Keys.LShiftKey:
                        case Keys.RShiftKey:
                            shft_pressed = true;
                            break;
                        case Keys.C:
                            c_pressed = true;
                            break;
                        case Keys.V:
                            v_pressed = true;
                            break;
                        default:
                            sth_else = true;
                            break;
                    }
                }
                if (wParam == WM_KEYUP || wParam == WM_SYSKEYUP)
                {
                    switch (key)
                    {
                        case Keys.LControlKey:
                        case Keys.RControlKey:
                            ctrl_pressed = false;
                            break;
                        case Keys.LShiftKey:
                        case Keys.RShiftKey:
                            shft_pressed = false;
                            break;
                        case Keys.V:
                            v_pressed = false;
                            break;
                        case Keys.C:
                            c_pressed = false;
                            break;
                        default:
                            sth_else = true;
                            break;
                    }
                }


                if (sth_else)
                {
                    ClearKeyStatus();
                }

                if (ctrl_pressed && c_pressed && CopyToClipboard != null)
                {
                    isHandled &= CopyToClipboard();
                }

                if (ctrl_pressed && shft_pressed && v_pressed && MyPaste != null)
                {
                    isHandled &= MyPaste();
                }

                if (v_pressed || c_pressed)
                    ClearKeyStatus();
            }
            busy = false;

            return isHandled ? 0 : CallNextHookEx(hhook, code, wParam, ref lParam);
        }

        private void ClearKeyStatus()
        {
            ctrl_pressed = false;
            v_pressed = false;
            c_pressed = false;
            shft_pressed = false;
            sth_else = false;
        }
   }
}