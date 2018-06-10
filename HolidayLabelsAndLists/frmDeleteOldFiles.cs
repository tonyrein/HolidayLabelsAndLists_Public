using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using HolidayLabelsAndListsHelper;
using GlobRes = AppWideResources.Properties.Resources;

namespace HolidayLabelsAndLists
{
    public partial class frmDeleteOldFiles : Form
    {
        private HllFileListManager FileListManager;
        public frmDeleteOldFiles(HllFileListManager flm)
        {
            InitializeComponent();
            this.FileListManager = flm;
            SetControls();
        }

        private void SetControls()
        {
            SetLabels();
            SetButtons();
        }

        private void SetButtons()
        {

        }
        private void SetLabels()
        {
            this.Text = GlobRes.DeleteOldFilesTitle;
            this.groupBox1.Text = GlobRes.SelectYears;

        }
        private void frmDeleteOldFiles_Activated(object sender, EventArgs e)
        {
            // ActiveYears() returns an array. This SHOULD be sorted
            // in descending order, but just to make sure we don't
            // inadvertently delete any of the current year's files, sort
            // it again, putting the most recent year at position 0.
            // Then skip that element when assigning list box's data source:
            string[] sa = this.FileListManager.ActiveYears();
            Array.Sort(sa);
            Array.Reverse(sa);
            this.lbxYearsToDelete.DataSource = sa.Skip(1).ToArray();
        }

        private void lbxYearsToDelete_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDelete.Enabled = (lbxYearsToDelete.SelectedItems.Count > 0);
        }
    }
}
