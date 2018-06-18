using System;
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
        private string[] _doc_types;

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

        private string getFolder(string startFolder, string title)
        {
            string retStr;
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.InitialDirectory = startFolder;
                dialog.IsFolderPicker = true;
                dialog.Title = Properties.Resources.SaveFolderDialogTitle;
                var mbRes = dialog.ShowDialog();
                retStr = (mbRes == CommonFileDialogResult.Ok) ?
                    dialog.FileName : "";
            }
            return retStr;
        }

        private string getDestination()
        {
            string retStr = "";
            //Uri internal_base = new Uri(FolderManager.OutputFolder);
            bool retry_loop;
            do
            {
                retry_loop = false;
                retStr = getFolder(
                    Properties.Settings.Default.ArchiveFilesStartFolder,
                    Properties.Resources.SaveFolderDialogTitle
                    );
                if (retStr != "")
                {
                    if (retStr.StartsWith(FolderManager.OutputFolder))
                    {
                        string dialog_msg = string.Format(Properties.Resources.OutOfBaseMsg,
                            retStr, FolderManager.OutputFolder);
                        string dialog_title = Properties.Resources.OutOfBaseTitle;
                        var mbRes = MessageBox.Show(
                            dialog_msg, dialog_title,
                            MessageBoxButtons.RetryCancel,
                            MessageBoxIcon.Exclamation
                        );
                        if (mbRes == DialogResult.Retry)
                            retry_loop = true;
                        else
                        {
                            retry_loop = false;
                            retStr = "";
                        }
                    }
                }
            }
            while (retry_loop);
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
