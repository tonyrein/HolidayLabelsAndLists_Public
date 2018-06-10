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
    public partial class frmFileMaintenance : Form
    {
        private HllFileListManager flm;
        // Let caller (frmMain) know if it needs to update itself:
        public bool FilesChanged
        {
            get; private set;
        }
        public frmFileMaintenance(HllFileListManager m)
        {
            InitializeComponent();
            this.flm = m;
            this.FilesChanged = false;
            SetControls();
        }

        private void SetControls()
        {
            SetCaptions();
            SetButtons();
        }

        private void SetCaptions()
        {
            btnDeleteBackups.Text = Properties.Resources.DelBackupsBtnCaption;
            btnDeleteOld.Text = Properties.Resources.DelOldFilesBtnCaption;
            btnDone.Text = Properties.Resources.CloseBtnCaption;
            btnSave.Text = Properties.Resources.SaveFilesBtnCaption;
            btnHelp.Text = Properties.Resources.HelpBtnCaption;
        }

        /// <summary>
        /// Gray out buttons which aren't relevant.
        /// </summary>
        private void SetButtons()
        {
            // Show Delete BU btn if there are backup files to delete:
            btnDeleteBackups.Enabled = flm.HasBackupFiles;
            // We can't delete latest year's files -- don't show this
            // button unless there are some years we can delete from:
            btnDeleteOld.Enabled = flm.ActiveYears().Count() > 0;
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDeleteBackups_Click(object sender, EventArgs e)
        {
            var mbRes = MessageBox.Show(
                GlobRes.DelBackupConfirmPrompt,
                GlobRes.DelBackupConfirmTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
                );
            if (mbRes != DialogResult.Yes)
                return;
            try
            {
                if (this.flm.DeleteBackupFiles() > 0)
                {
                    SetButtons();
                    this.FilesChanged = true;
                }
            }
            catch (Exception fe)
            {
                MessageBox.Show(
                    string.Format(GlobRes.DelBackupErrorMsg, fe.Message),
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                    );
            }
        }

        /// <summary>
        /// TODO: Add code to remind about saving before opening
        /// dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteOld_Click(object sender, EventArgs e)
        {
            using (frmDeleteOldFiles OldFilesForm = new frmDeleteOldFiles(flm))
            {
                OldFilesForm.ShowDialog();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (frmSaveFiles SaveForm = new frmSaveFiles(flm))
            {
                SaveForm.ShowDialog();
            }
        }
    }
}
