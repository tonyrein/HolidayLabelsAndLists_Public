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

        private string getDestination()
        {
            string retStr = "";
            Uri internal_base = new Uri(FolderManager.OutputFolder);

            if (internal_base.IsBaseOf(new Uri(retStr)))
            {

            }
            return retStr;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.InitialDirectory = Properties.Settings.Default.ArchiveFilesStartFolder;
                dialog.IsFolderPicker = true;
                dialog.Title = Properties.Resources.SaveFolderDialogTitle;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    txtDestination.Text = dialog.FileName;
                }
            }
        }

        private void lbxDocType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
