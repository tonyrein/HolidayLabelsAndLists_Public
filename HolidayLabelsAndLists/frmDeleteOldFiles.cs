﻿using System;
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
            this.btnCancel.Text = Properties.Resources.CancelBtnCaption;
            this.btnDelete.Text = Properties.Resources.DelOldBackupsGoButtonCaption;

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
            // inadvertently delete any files from the current or most
            // recent previous year, sort it again, putting the most recent year at position 0.
            // Then skip the first two elements when assigning list box's data source:
            string[] sa = this.FileListManager.ActiveYears();
            Array.Sort(sa);
            Array.Reverse(sa);
            this.lbxYearsToDelete.DataSource = sa.Skip(2).ToArray();
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


        private void DeleteSelectedYears(List<String> yearsToDelete)
        {

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
            String msg_tmpl = Properties.Resources.DeleteOldConfirmation;
            string msg = String.Format(msg_tmpl, years);
            var msgBoxResp = MessageBox.Show(msg, Properties.Resources.ReallyDeleteQuestion,
                MessageBoxButtons.YesNoCancel);
            if (msgBoxResp == DialogResult.Yes)
            {
                this.FileListManager.DeleteOldFiles(selectedYearsList);
            }
        }
    }
}
