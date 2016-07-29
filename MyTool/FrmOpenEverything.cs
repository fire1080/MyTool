using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyTool
{
    //support bat, exe, jpg, png, gif, ico, txt, sql, music, movie, directory, noExtention file
    public partial class FrmOpenEverything : Form
    {

        public FrmOpenEverything()
        {
            InitializeComponent();
        }

        private void FrmOpenEverything_Load(object sender, EventArgs e)
        {
            LoadEverything();
        }

        private void FrmOpenEverything_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:
                    if (lstOpenEverything.SelectedIndex >= 0)
                    {
                        OpenItem(lstOpenEverything.SelectedValue.ToString());
                    }
                    e.Handled = true;
                    this.Close();
                    break;
                case (char)Keys.Escape:
                    this.Close();
                    break;
            }
        }


        private void lstOpenEverything_DoubleClick(object sender, EventArgs e)
        {
            OpenItem(lstOpenEverything.SelectedValue.ToString());
            this.Close();
        }

        private void OpenItem(string item)
        {
            if (Directory.Exists(item))
            {
                FileHelper.OpenDirectory(item);
            }
            else if (File.Exists(item))
            {
                if (Path.HasExtension(item))
                {
                    FileHelper.OpenFileWithNotePad(item);
                }
                var extention = Path.GetExtension(item);
                switch (extention.ToLower())
                {
                    case "exe":
                        FileHelper.ExecuteFile(item);
                        break;
                    case "bat":
                        break;
                    case "txt":
                        FileHelper.OpenFileWithNotePad(item);
                        break;
                    case "jpg":
                    case "png":
                        FileHelper.ExecuteFile(item);
                        break;
                    default:
                        FileHelper.ExecuteFile(item);
                        break;
                }
            }
        }

        private void LoadEverything()
        {
            var everythings = XmlHelper.OpenEverythingInfo.Everythings;
            BindList(everythings);
        }

        private void BindList(Dictionary<string, string> everythings)
        {
            lstOpenEverything.DataSource = new BindingSource(everythings, null);
            lstOpenEverything.ValueMember = "Value";
            lstOpenEverything.DisplayMember = "Key";
        }

        private void lstOpenEverything_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var clickPoint = new Point(e.X, e.Y);
                int selectedIndex = lstOpenEverything.IndexFromPoint(clickPoint);
                lstOpenEverything.SelectedIndex = selectedIndex;

                contextMenuStrip1.Show(lstOpenEverything, clickPoint);
            }
            lstOpenEverything.Refresh();
        }

        private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmEdit = new FrmOpenEverything_Edit(string.Empty, string.Empty);
            frmEdit.SaveEverything += AddEverything;
            frmEdit.TopMost = true;
            frmEdit.Show();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frmEdit = new FrmOpenEverything_Edit(lstOpenEverything.Text, lstOpenEverything.SelectedValue.ToString());
            frmEdit.SaveEverything += EditEverything;
            frmEdit.TopMost = true; 
            frmEdit.Show();
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var everythings = (lstOpenEverything.DataSource as BindingSource).DataSource as Dictionary<string, string>;

            if (everythings != null && lstOpenEverything.SelectedItem != null)
            {
                var originalItem = lstOpenEverything.SelectedItem;
                var newOrderDic = MoveUpItem(everythings, lstOpenEverything.Text);

                XmlHelper.OpenEverythingInfo.Everythings = newOrderDic;
                BindList(newOrderDic);
                lstOpenEverything.SelectedItem = originalItem;
            }
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var everythings = (lstOpenEverything.DataSource as BindingSource).DataSource as Dictionary<string, string>;

            if (everythings != null && lstOpenEverything.SelectedItem != null)
            {
                var originalItem = lstOpenEverything.SelectedItem;
                var newOrderDic = MoveDownItem(everythings, lstOpenEverything.Text);

                XmlHelper.OpenEverythingInfo.Everythings = newOrderDic;
                BindList(newOrderDic);
                lstOpenEverything.SelectedItem = originalItem;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstOpenEverything.SelectedItem != null 
                && DialogResult.Yes == MessageBox.Show("Are you sure to Delete?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                DeleteEverything(lstOpenEverything.Text);
            }
        }



        private bool AddEverything(string key, string value)
        {
            if (!ValidateEverythingValue(value))
                return false;

            var everythings = (lstOpenEverything.DataSource as BindingSource).DataSource as Dictionary<string, string>;
            if (everythings == null || everythings.ContainsKey(key))
                return false;

            everythings.Add(key, value);

            XmlHelper.OpenEverythingInfo.Everythings = everythings;

            BindList(everythings);

            return true;
        }

        private bool DeleteEverything(string key)
        {
            var everythings = (lstOpenEverything.DataSource as BindingSource).DataSource as Dictionary<string, string>;
            if (everythings != null)
            {
                everythings.Remove(key);
                XmlHelper.OpenEverythingInfo.Everythings = everythings;
                BindList(everythings);
                return true;
            }
            return false;
        }

        private bool EditEverything(string key, string value)
        {
            if (!ValidateEverythingValue(value))
                return false;

            var everythings = (lstOpenEverything.DataSource as BindingSource).DataSource as Dictionary<string, string>;
            if (everythings == null || lstOpenEverything.SelectedItem == null)
                return false;

            if (key == lstOpenEverything.Text)
            {
                everythings[key] = value;
            }
            else
            {
                everythings.Remove(lstOpenEverything.Text);

                if (everythings.ContainsKey(key))
                    return false;

                everythings.Add(key, value);
            }
            XmlHelper.OpenEverythingInfo.Everythings = everythings;

            BindList(everythings);

            lstOpenEverything.Text = key;

            return true;
        }

        private bool ValidateEverythingValue(string value)
        {
            return Directory.Exists(value) || File.Exists(value);
        }

        private Dictionary<string,string> MoveUpItem(Dictionary<string,string>dic , string key)
        {
            if (dic == null || !dic.ContainsKey(key))
                throw new Exception("Invalid dictionary or key.");

            var dicPairList = dic.ToList();
            var currentPair = dic.First(p => p.Key == key);

            int currentIndex = dicPairList.IndexOf(currentPair);
            int newIndex = Math.Max(currentIndex - 1, 0);
            dicPairList.Remove(currentPair);
            dicPairList.Insert(newIndex, currentPair);
            return dicPairList.ToDictionary(p => p.Key, p => p.Value);
        }

        private Dictionary<string,string> MoveDownItem(Dictionary<string, string> dic, string key)
        {
            if (dic == null || !dic.ContainsKey(key))
                throw new Exception("Invalid dictionary or key.");

            var keys = dic.Keys.ToList();

            int currentIndex = keys.IndexOf(key);
            int newIndex = Math.Min(currentIndex + 1, dic.Count -1);
            keys.Remove(key);
            keys.Insert(newIndex, key);
            return keys.ToDictionary(k => k, k => dic[k]); 

        }
    }
}
