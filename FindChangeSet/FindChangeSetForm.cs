using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using MouseKeyboardLibrary;

namespace FindChangeSet
{
    public partial class FindChangeSetForm : Form
    {
        KeyboardHook keyboardHook = new KeyboardHook();
        private string diskFlag;
        public FindChangeSetForm()
        {
            InitializeComponent();
            richTextBox1.Text = string.Empty;
            diskFlag = Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory);
            keyboardHook.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            keyboardHook.KeyUp += (o, args) =>
            {
                if (args.Alt && args.KeyCode == Keys.C)
                {
                    richTextBox1.AppendText("Up\t" + args.KeyCode.ToString() + Environment.NewLine);
                    args.Handled = true;
                }
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var vscmd = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsDevCmd.bat";
            var str = @"ipconfig";
            var p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();

            p.StandardInput.WriteLine(vscmd + "&" + str + "&exit");
            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

            
            //获取cmd窗口的输出信息
            string output = null;
            //output = p.StandardOutput.ReadToEnd();

            StreamReader reader = p.StandardOutput;
            reader.ReadLine(); 
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith(@"C:\") || line.StartsWith(diskFlag)) continue;
                output += line + Environment.NewLine;
            }

            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
            DisplayResult(output);
        }

        private void DisplayResult(string outPut)
        {

            richTextBox1.Text = outPut + Environment.NewLine;

            Debug.Print(outPut);
        }

    }
}
