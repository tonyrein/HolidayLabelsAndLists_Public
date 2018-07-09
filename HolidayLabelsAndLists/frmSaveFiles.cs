using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Dialogs;

using DAO;
using HolidayLabelsAndListsHelper;

namespace HolidayLabelsAndLists
{
    public partial class frmSaveFiles : Form
    {
        private HllFileListManager _flm;
        //private string[] _doc_types;

        public frmSaveFiles(HllFileListManager flm)
        {
            InitializeComponent();
            this._flm = flm;
            this.Text = Properties.Resources.SaveFilesFormTitle;
            SetControls();
        }

        private void SetCaptions()
        {
            btnBrowse.Text = Properties.Resources.BrowseBtnCaption;
            btnCancel.Text = Properties.Resources.CancelBtnCaption;
            btnSave.Text = Properties.Resources.SaveBtnCaption;
            chkIncludeBackups.Text = Properties.Resources.IncludeBackupsChkCaption;
        }

        private void SetLabels()
        {
            lblDestination.Text = Properties.Resources.DestLabelText;
            lblDocType.Text = Properties.Resources.DocTypeLabelText;
            lblYear.Text = Properties.Resources.YearLabelText;

        }

        private void SetLists()
        {
            this.lbxYears.DataSource = _flm.ActiveYears();
            this.lbxDocType.DataSource = Properties.Resources.DocumentTypes.Split('#');
        }
        private void SetCombos()
        {
            

        }

        private void SetControls()
        {
            SetCaptions();
            SetLabels();
            SetCombos();
            SetLists();
        }

        private void frmSaveFiles_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Present a folder selection dialog.
        /// 
        /// If the user clicks the dialog's Cancel button,
        /// return "". Otherwise, return the selected
        /// path.
        /// 
        /// If the given start folder does not exist, create
        /// it before displayint the dialog.
        /// 
        /// </summary>
        /// <param name="startFolder"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private string getFolder(string startFolder, string title)
        {
            string retStr;
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                if (!Directory.Exists(startFolder))
                    Directory.CreateDirectory(startFolder);
                dialog.EnsurePathExists = true;
                dialog.EnsureFileExists = false;
                dialog.IsFolderPicker = true;
                dialog.Title = title;
                dialog.InitialDirectory = startFolder;
                dialog.RestoreDirectory = false;
                var mbRes = dialog.ShowDialog();
                retStr = (mbRes == CommonFileDialogResult.Ok) ?
                    dialog.FileName : "";
            }
            return retStr;
        }

        /// <summary>
        /// getDestination() will ask the user for a folder name.
        /// This method takes the folder name and verifies:
        ///     1. It exists, or can be created
        ///     2. There is write access to it.
        ///     3. It's not in the "forbidden" area -
        ///     that is, it's not a subfolder of HLL's
        ///     internal storage area.
        ///     
        /// </summary>
        /// <returns></returns>
        private DialogResult FolderIsSuitableDestination()
        {

            return DialogResult.OK;
        }
        /// <summary>
        /// Use getFolder() to get a path from the user.
        /// If getFolder() returns "" (ie, the user cancelled),
        /// return "".
        /// 
        /// Do not allow the user to select a path that is a subfolder
        /// of HLL's internal storage -- if getFolder() returns a folder
        /// that is such a subfolder, tell the user and allow the user
        /// to either retry or cancel.
        /// 
        /// If the user cancels, return "".
        /// 
        /// </summary>
        /// <returns></returns>
        private string getDestination()
        {
            string retStr;
            // Loop until we either get a good path
            // or the user cancels.
            while(true)
            {
                retStr = getFolder(
                    Properties.Settings.Default.ArchiveFilesStartFolder,
                    Properties.Resources.SaveFolderDialogTitle
                    );
                // quit if user cancelled folder selection. Return
                // empty string.
                if (retStr == "")
                    break;
                // quit if we have a folder that's not in the
                // "forbidden zone." Return selected path.
                if (!retStr.StartsWith(FolderManager.OutputFolder))
                    break;
                // Selected path is a subfolder in our internal storage
                // area. Give user chance to select another path or
                // to cancel.
                string dialog_msg = string.Format(Properties.Resources.OutOfBaseMsg,
                        retStr, FolderManager.OutputFolder);
                string dialog_title = Properties.Resources.OutOfBaseTitle;
                var mbRes = MessageBox.Show(
                    dialog_msg, dialog_title,
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Exclamation
                );
                // User wants to retry -- go
                // to top of loop.
                if (mbRes == DialogResult.Retry)
                    continue;
                else
                // User cancelled. Set return
                // string to empty string
                // and exit loop.
                {
                    retStr = "";
                    break;
                }
            }
            return retStr;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string dest = getDestination();
            if (dest != "")
                txtDestination.Text = dest;
        }

        private void lbxDocType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
