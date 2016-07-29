using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using MouseKeyboardLibrary;
using MyTool;

namespace FindChangeSet
{
    public partial class FrmFindChangeSet : Form
    {
        private string diskFlag;

        public FrmFindChangeSet()
        {
            InitializeComponent();

            diskFlag = Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSearchPaths();
            listView1.Items.Clear();
        }

        private void frmFindChangeSet_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (DialogResult.Yes == MessageBox.Show(@"Do you want minimize the tool?  Yes to 'Minimize'.  No to 'Close'", Application.ProductName,MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button1))
            //{
            //    this.Hide();
            //    e.Cancel = true;
            //}
            this.Hide();
            e.Cancel = true;
        }

        private void frmFindChangeSet_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void FrmFindChangeSet_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char) Keys.Enter:
                    SearchChangeSet();
                    e.Handled = true;
                    break;
                case (char) Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchChangeSet();
        }

        private void SearchChangeSet()
        {
            SortOutResults();

            //validate inputs
            var searchPath = cboSearchPath.Text;
            var searchPattern = txtSearchString.Text;
            if (string.IsNullOrWhiteSpace(searchPath) || !searchPath.StartsWith("$"))
            {
                MessageBox.Show("Invalid Search Path.");
                return;
            }

            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                MessageBox.Show("Invalid Search Pattern.");
                return;
            }

            //search
            var vscmd = @"""C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsDevCmd.bat""";
            var searchStr =
                string.Format(
                    @"tf history {0} /collection:tfs-w2k08/DefaultCollection /recursive | findstr /i ""{1}""",
                    searchPath, searchPattern);

            var p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();

            p.StandardInput.WriteLine(vscmd + "&&" + searchStr + "&exit");
            //p.StandardInput.AutoFlush = true;
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

            //获取cmd窗口的输出信息
            //string output = null;
            //output = p.StandardOutput.ReadToEnd();

            StreamReader reader = p.StandardOutput;
            reader.ReadLine();
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string record = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(record) || record.StartsWith(@"C:\") || record.StartsWith(diskFlag)) continue;
                DisplayResult(record);
            }
            p.StandardInput.WriteLine("exit");
            p.WaitForExit();//等待程序执行完退出进程
            p.Close();

            listView1.Refresh();
            UpdateRecentPath(searchPath);
        }

        private void DisplayResult(string record)
        {
            if (record.Length <= 40)
            {
                return;
            }
            var num = record.Substring(0, 9);
            var person = record.Substring(10, 17);
            var date = record.Substring(27, 11);
            var comments = record.Substring(39);

            listView1.Items.Add(new ListViewItem(new string[] {num, person, date, comments}));

            //Debug.Print(outPut);
        }

        private void SortOutResults()
        {
            if (!this.chkKeepResults.Checked)
            {
                listView1.Items.Clear();
            }
        }

        private void LoadSearchPaths()
        {
            var recentPaths = XmlHelper.FindChangeSetInfo.RecentlyUsedPaths;
            var templetPaths = XmlHelper.FindChangeSetInfo.TempletePaths.OrderBy(p => p);
            var searchPaths = recentPaths.Concat(templetPaths).ToList();
            cboSearchPath.DataSource = searchPaths;
            cboSearchPath.SelectedIndex = 0;
        }

        private void UpdateRecentPath(string path)
        {
            XmlHelper.FindChangeSetInfo.RecentlyUsedPaths = new List<string>() {path};

            var searchPaths = (List<string>) cboSearchPath.DataSource;
            searchPaths.RemoveAt(0);
            searchPaths.Insert(0, path);
        }
    }
}
