using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Runtime.InteropServices;

namespace MyTool
{
    public partial class FrmMyClipboard : Form//,IDisposable
    {
        public event Action<string> PasteInfoSelected = null;

        public FrmMyClipboard()
        {
            InitializeComponent();
        }

        public void populate(List<string> history)
        {
            //This step is necessary for a listbox to refresh its binding datasource
            //It'll only refresh when objects have changed (ex. a different query), not the data.
            lstHistory.DataSource = null;
            lstHistory.DataSource = history;
            if(history != null && history.Count > 0)
            {
                lstHistory.SelectedIndex = 0;
            }
            using (Graphics g = this.CreateGraphics())
            {
                var charMaxAmount = 0;
                history.ForEach(h => charMaxAmount = Math.Max(charMaxAmount, h.Length));
                var stringSize = g.MeasureString(new string('w', charMaxAmount), lstHistory.Font);
                this.Width = (int)Math.Min(Math.Ceiling(stringSize.Width), 300);
                //��lstHistory��fulfill��this�����еģ���������lstHistory�Ŀ��û�����塣ͬʱ��this.Height <= 16ʱ��lstHistory.Height����4...
                //this.Height = lstHistory.Height = (int)Math.Ceiling(stringSize.Height) * Math.Min(history.Count, 10) + 2;
                this.Height = Math.Max(17, (int)Math.Ceiling(stringSize.Height) * Math.Min(history.Count, 10) + 2);
            }
            this.Show();
        }

        public void MoveToNextHistory()
        {
            var currentIndex = lstHistory.SelectedIndex;
            var nextIndex = currentIndex + 1;
            nextIndex = nextIndex >= lstHistory.Items.Count ? 0 : nextIndex;
            lstHistory.SelectedIndex = nextIndex;
        }

        private void lstHistory_MouseClick(object sender, MouseEventArgs e)
        {
            int index;
            index = lstHistory.IndexFromPoint(e.X, e.Y);
            if (PasteInfoSelected != null && index >= 0 && index < lstHistory.Items.Count)
            {
                PasteInfoSelected(lstHistory.Items[index].ToString());
            }
            this.Hide();
        }

        //���MouseClick()������Listbox.DataSource�ı�ʱ������������֮6
        //private void lstHistory_SelectedValueChanged(object sender, EventArgs e)
        //{
        //    int index = lstHistory.SelectedIndex;
        //    if (PasteInfoSelected != null && index >= 0)
        //    {
        //        PasteInfoSelected((string)lstHistory.SelectedItem);
        //    }
        //    this.Hide();
        //}

        //private void lstHistory_MouseHover(object sender, EventArgs e)
        //{
        //    string text = "";
        //    Point p = lstHistory.PointToClient(FrmMyClipboard.MousePosition);
        //    int index = lstHistory.IndexFromPoint(p);
        //    if (index >= 0 && index < lstHistory.Items.Count)
        //        text = lstHistory.Items[index].ToString();
        //    else
        //        text = "Clipboard Little Helper";
        //    toolTip1.SetToolTip(lstHistory, text);
        //}

        private void FrmMyClipboard_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:
                case (char)Keys.Space:
                    if (lstHistory.SelectedIndex >= 0 && PasteInfoSelected != null)
                    {
                        PasteInfoSelected((string)lstHistory.SelectedItem);
                    }
                    e.Handled = true;
                    this.Hide();
                    break;
                case (char)Keys.Escape:
                    this.Hide();
                    break;
                //ListBox �޷����� �������Ҽ�
                //case (char)Keys.Up:
                //    if (lstHistory.SelectedIndex == 0 && lstHistory.Items.Count > 1)
                //        lstHistory.SelectedIndex = lstHistory.Items.Count - 1;
                //    break;
            }
        }

    }
}

//todo ��wndproc�������¼���ѭ���� ���ʹ��paste����ʱ���ܲ�����һ��copy������(������pasteʱ�ٴβ���)�� �������hotkey���ܣ�win + V ==> 1/2/3��, �򿪴��ڵ�λ�ÿ��������ֱ�ӵ��