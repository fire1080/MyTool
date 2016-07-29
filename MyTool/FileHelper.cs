using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MyTool
{
    class FileHelper
    {
        public static void OpenDirectory(string directory)
        {
            Process.Start("explorer.exe", directory);
        }

        public static void OpenFileWithNotePad(string file)
        {
            var notepadPlus = @"C:\Program Files (x86)\Notepad++\notepad++.exe";
            Process newProcess = new Process();
            newProcess.StartInfo.FileName = file;
            newProcess.Start();
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
            Process.Start(file);
        }

        public static void ExecuteFileWithCmd(string file)
        {
            
        }

        public static void ExecuteFileWithSqlPlus(string file)
        {
            
        }

    }
}
