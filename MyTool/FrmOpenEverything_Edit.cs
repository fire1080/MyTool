using System;
using System.Windows.Forms;

namespace MyTool
{
    public partial class FrmOpenEverything_Edit : Form
    {
        public event Func<string, string, bool> SaveEverything;
        private string _key;
        private string _value;
        public FrmOpenEverything_Edit()
        {
            InitializeComponent();
        }

        public FrmOpenEverything_Edit(string key, string value): this()
        {
            this._key = key;
            this._value = value;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _key = txtKey.Text.Trim();
            _value = txtValue.Text.Trim();
            if (!string.IsNullOrWhiteSpace(_key) && !string.IsNullOrWhiteSpace(_value)
                && SaveEverything != null && SaveEverything(_key, _value))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid Inputs");
            }
        }

        private void SetEverything()
        {
            this.txtKey.Text = _key;
            this.txtValue.Text = _value;
        }

        private void FrmOpenEverything_Edit_Load(object sender, EventArgs e)
        {
            SetEverything();
        }
    }
}
