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
//using GlobRes = AppWideResources.Properties.Resources;

namespace HolidayLabelsAndLists
{
    public partial class frmDeleteOldFiles : Form
    {
        private HllFileListManager FileListManager;
        // Let caller (frmMain) know if it needs to update itself:
        public bool FilesChanged
        {
            get; private set;
        }
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
            this.btnCancel.Text = Properties.Resources.CancelBtnCaption;
            this.btnDelete.Text = Properties.Resources.DelOldBackupsGoButtonCaption;
        }

        private void SetLabels()
        {
            this.Text =  Properties.Resources.DeleteOldFilesTitle;
            this.groupBox1.Text = Properties.Resources.SelectYears;
        }

        private void frmDeleteOldFiles_Activated(object sender, EventArgs e)
        {
            this.FilesChanged = false;
            // ActiveYears() returns an array. This SHOULD be sorted
            // in descending order, but just to make sure we don't
            // inadvertently delete any files from the current or most
            // recent previous year, sort it again, putting the most recent year at position 0.
            // Then skip the first two elements when assigning list box's data source:
            string[] sa = this.FileListManager.ActiveYears();
            Array.Sort(sa);
            Array.Reverse(sa);
            this.lbxYearsToDelete.DataSource = sa.Skip(2).ToArray();
            // Anything to delete? If not, display a message and close this form.
            if (this.lbxYearsToDelete.Items.Count < 1)
            {
                this.Hide();
                MessageBox.Show(Properties.Resources.MsgNoOldFiles,
                    Properties.Resources.TitleNoOldFIles,
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                this.Close();
            }
        }

        /// <summary>
        /// Enable the action button if there are any years selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbxYearsToDelete_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDelete.Enabled = (lbxYearsToDelete.SelectedItems.Count > 0);
        }

        /// <summary>
        /// Display a confirmation message. If the user
        /// confirms file deletion choice, delete the files.
        /// Otherwise, just cancel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            List<String> selectedYearsList = new List<string>();
            foreach (object item in lbxYearsToDelete.Items)
            {
                selectedYearsList.Add(item.ToString());
            }
            string years = String.Join(", ", selectedYearsList);
            string msg = String.Format(Properties.Resources.DeleteOldConfirmation, years);
            var msgBoxResp = MessageBox.Show(msg, Properties.Resources.ReallyDeleteQuestion,
                MessageBoxButtons.YesNoCancel);
            if (msgBoxResp == DialogResult.Yes)
            {
                int numFilesDeleted = this.FileListManager.DeleteOldFiles(selectedYearsList);
                if (numFilesDeleted > 0)
                    this.FilesChanged = true;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
