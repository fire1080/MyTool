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

namespace clipboard_helper
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
            lstHistory.DataSource = history;
        }

        private void lstHistory_MouseClick(object sender, MouseEventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            //Stopwatch sw = new Stopwatch();sw.Start();
            //int index;
            //index = lstHistory.IndexFromPoint(e.X,e.Y);
            //if (PasteInfoSelected != null && index >= 0 && index < lstHistory.Items.Count)
            //{
            //    PasteInfoSelected(lstHistory.Items[index].ToString());
            //}
            //this.DialogResult = DialogResult.OK;
            //Debug.Print(sw.ElapsedMilliseconds.ToString());sw.Stop();
        }

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
                    if (lstHistory.SelectedIndex >= 0 && PasteInfoSelected != null)
                    {
                        PasteInfoSelected(lstHistory.Text);
                    }
                    e.Handled = true;
                    this.DialogResult = DialogResult.OK;
                    break;
                case (char)Keys.Escape:
                    this.DialogResult = DialogResult.Cancel;
                    break;
            }
        }

        private void lstHistory_SelectedValueChanged(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

    }
}