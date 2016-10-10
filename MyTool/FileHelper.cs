using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MyTool
{
    class FileHelper
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void OpenDirectory(string directory)
        {
            var process = Process.Start("explorer.exe", directory);
            SetForegroundWindow(process.Handle);
        }

        public static void OpenFileWithNotePad(string file)
        {
            var notepadPlus = @"C:\Program Files (x86)\Notepad++\notepad++.exe";

            Process.Start(notepadPlus, file);
        }

        public static void OpenBrower(string url, string browser = "Chrome.exe")
        {
            Process.Start(browser, url);
        }

        public static void OpenMusicFile(string file)
        {
            
        }

        public static void OpenExcelFile(string file)
        {
            
        }

        public static void OpenWordFile(string file)
        {
            
        }

        public static void OpenPhotoFile(string file)
        {
            
        }

        public static void OpenMovieFile(string file)
        {
            
        }

        public static void ExecuteFile(string file)
        {
            try
            {
                Process.Start(file);
            }
            catch (Win32Exception exception)
            {
                //ignore user cancel exception
                if (exception.Message.Contains("cancel"))
                {
                }
                else
                {
                    throw;
                }
            }
        }

        public static void ExecuteFileWithCmd(string file)
        {
            
        }

        public static void ExecuteFileWithSqlPlus(string file)
        {
            
        }

    }
}
