using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MyTool
{
    public partial class FrmOpenICCIncident : Form
    {
        private const int MaxAmountForRecentIncidents = 8;
        private List<int> _recentIncidents;

        public FrmOpenICCIncident()
        {
            InitializeComponent();
            LoadRecentIncidents();
        }

        private void FrmOpenICCIncident_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char) Keys.Enter:
                {
                    string incidentId = cboIncident.Text.Trim();
                    if (!String.IsNullOrWhiteSpace(incidentId) && Regex.IsMatch(incidentId, @"^\d{5}$"))
                    {
                        OpenIncidentUrl(int.Parse(incidentId));
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid Incident No.");
                        e.Handled = true;
                    }
                }
                    break;
                case (char)Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void OpenIncidentUrl(int incidentId)
        {
            //open an ibs incident on the default brower
            Process.Start("Chrome.exe",
                string.Format(
                    @"http://icconline.hsntech.com//Incidents/incidentdetail.aspx?id={0}&BrowseType=all",
                    incidentId));

            //save to xmlFile
            if (!_recentIncidents.Contains(incidentId))
            {
                _recentIncidents.Insert(0, incidentId);
                if (_recentIncidents.Count > MaxAmountForRecentIncidents)
                {
                    _recentIncidents.RemoveAt(_recentIncidents.Count - 1);
                }
            }
            XmlHelper.OpenIccIncidentInfo.RecentIncidents = _recentIncidents.ToList();
        }

        private void LoadRecentIncidents()
        {
            cboIncident.Items.Clear();

            _recentIncidents = XmlHelper.OpenIccIncidentInfo.RecentIncidents;

            cboIncident.DataSource = _recentIncidents;
            cboIncident.SelectedIndex = 0;
        }
    }
}
