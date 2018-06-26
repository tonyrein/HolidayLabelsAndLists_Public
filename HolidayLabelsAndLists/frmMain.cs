using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

using DAO;
using HolidayLabelsAndListsHelper;
using VestaProcessor;
using GlobRes = AppWideResources.Properties.Resources;

namespace HolidayLabelsAndLists
{
    public partial class frmMain : Form
    {
        private DBWrapper context = new DBWrapper();
        private HllFileListManager FileListManager;

        private enum AppStates { Processing, Viewing, ShowingWork };

        private AppStates CurrentState;
        private BackgroundWorker _bgworker;
        private frmProgress ProgressForm;


        public frmMain()
        {
            InitializeComponent();
            FileListManager = new HllFileListManager(context);
            cmbTypeToView.DataSource = Properties.Resources.DocumentTypes.Split('#');
            SetCaptions();
            SetAppState(AppStates.Viewing);
        }

        private void SetCaptions()
        {
            this.btnMaintenance.Text = Properties.Resources.MaintBtnCaption;
        }
        private void FillFileListView()
        {
            lvAvailableFiles.Items.Clear();
            foreach (string s in FileListManager.FileNameList())
            {
                ListViewItem item = new ListViewItem(s);
                item.ToolTipText = s;
                lvAvailableFiles.Items.Add(item);
            }
            // No matches?
            if (lvAvailableFiles.Items.Count == 0)
            {
                //ListViewItem item = new ListViewItem("No files match the selected criteria.");
                //item.ToolTipText = "Select different criteria for Document Type to View, Year, and/or Donor.";
                ListViewItem item = new ListViewItem(GlobRes.NoMatchingFilesMsg);
                item.ToolTipText = GlobRes.NoMatchingFilesTooltip;
                lvAvailableFiles.Items.Add(item);
            }

        }

        /// <summary>
        /// Set cmbYear's list to all of the years contained
        /// in our data's records.
        /// 
        /// Save the old value, and if reset==false, reselect
        /// that value when done. If reset==true, select first
        /// element in list.
        /// </summary>
        /// <param name="set_to_zero"></param>
        private void PopulateYearCombo(bool set_to_zero = true)
        {
            // turn off combo box's IndexChanged event handler
            cmbYear.SelectedIndexChanged -= cmbYear_SelectedIndexChanged;
            // save "before" value:
            string old_value = cmbYear.Text;
            cmbYear.DataSource = FileListManager.ActiveYears();
            if (set_to_zero)
            {
                if (cmbYear.Items.Count > 0)
                    cmbYear.SelectedIndex = 0;
            }
            else
            {
                // Restore to old value, if that value still exists in the list.
                // If not, set to first value
                int idx = cmbYear.FindStringExact(old_value);
                if (idx == -1) // not found
                {
                    if (cmbYear.Items.Count > 0)
                        cmbYear.SelectedIndex = 0;
                }
                else
                {
                    cmbYear.SelectedIndex = idx;
                }
            }
            // turn IndexChanged event handler back on
            cmbYear.SelectedIndexChanged += cmbYear_SelectedIndexChanged;
        }

        /// <summary>
        /// Get list of donors for whom there are files
        /// for selected year, and use that list as the
        /// data source for the Donor combo. If there are
        /// any donors in the list, select the first one.
        /// 
        /// </summary>
        /// <param name="year"></param>
        private void PopulateDonorCombo(string year, bool set_to_zero = true)
        {
            // turn IndexChanged event handler off
            cmbDonor.SelectedIndexChanged -= cmbDonor_SelectedIndexChanged;
            string old_value = cmbDonor.Text;
            if (!String.IsNullOrEmpty(year))
            {
                cmbDonor.DataSource = FileListManager.ActiveDonorsForYear(year);
                cmbDonor.DisplayMember = "name";
                cmbDonor.ValueMember = "code";
                if (set_to_zero)
                {
                    if (cmbDonor.Items.Count > 0)
                    {
                        cmbDonor.SelectedIndex = 0;
                        SetDonorFilter();
                    }
                }
                else
                {
                    int idx = cmbDonor.FindStringExact(old_value);
                    if (idx == -1) // not found
                    {
                        if (cmbDonor.Items.Count > 0)
                            cmbDonor.SelectedIndex = 0;
                    }
                    else
                    {
                        cmbDonor.SelectedIndex = idx;
                    }
                }
            }
            // turn IndexChanged event handler back on
            cmbDonor.SelectedIndexChanged += cmbDonor_SelectedIndexChanged;
        }

        private void PopulateTypeToViewCombo(bool set_to_zero = true)
        {
            //cmbTypeToView.DataSource = Properties.Resources.DocumentTypes.Split('#');
            // turn IndexChanged event handler off
            cmbTypeToView.SelectedIndexChanged -= cmbTypeToView_SelectedIndexChanged;
            if (set_to_zero)    
                cmbTypeToView.SelectedIndex = 0;
            // turn IndexChanged event handler back on
            cmbTypeToView.SelectedIndexChanged += cmbTypeToView_SelectedIndexChanged;
        }
        /// <summary>
        /// Set column width of file name column to magic number which
        /// means "full width of parent"
        /// </summary>
        private void FormatFileListView()
        {
            lvAvailableFiles.Columns[0].Width = -2;
        }

        private void SetButtonAndCheckboxState()
        {
            //btnMaintenance.Enabled = FileListManager.HasBackupFiles;
            chbxIncludeBackups.Enabled = FileListManager.HasBackupFiles;
        }
        /// <summary>
        /// Set data sources and initial selections of combo boxes.
        /// 
        /// Turn off event handlers beforehand, so we don't fire
        /// them when they won't work.
        /// 
        /// </summary>
        private void InitializeComboBoxes(bool set_to_zero = true)
        {
            PopulateTypeToViewCombo(set_to_zero: set_to_zero);
            PopulateYearCombo(set_to_zero: set_to_zero);
            string y = cmbYear.Text;
            if (!String.IsNullOrEmpty(y))
            {
                PopulateDonorCombo(y, set_to_zero: set_to_zero);
            }
            SetFilters();
            FileListManager.ApplyFilters();
        }
        /// <summary>
        /// Set donor filter to string value of combo box selection.
        /// (This will be the donor code.)
        /// If there is no selected item, set to empty string.
        /// </summary>
        private void SetDonorFilter()
        {
            var obj = cmbDonor.SelectedItem;
            FileListManager.DonorFilter = obj != null ? ((Donor)obj).code : "";
        }

        private void SetTypeFilter()
        {
            // Is it safe to assume this won't be null?
            //FileListManager.TypeFilter = cmbTypeToView.Text;
            FileListManager.TypeFilter = new FilterSetTypeFilters(cmbTypeToView.Text);
        }

        /// <summary>
        /// Set year filter to string value of combo box selection.
        /// 
        /// If there is no selected item, set to empty string.
        /// </summary>
        private void SetYearFilter()
        {
            var obj = cmbYear.SelectedItem;
            FileListManager.YearFilter = obj != null ? (obj).ToString() : "";
        }

        /// <summary>
        /// Determine whether or not to show backups in
        /// the file list, based on the state of the
        /// "Include Backup Files" checkbox.
        /// 
        /// </summary>
        private void SetShowBackupsFilter()
        {
            FileListManager.IncludeBackupsFilter = chbxIncludeBackups.Checked;
        }

        private void SetFilters()
        {
            SetDonorFilter();
            SetTypeFilter();
            SetYearFilter();
            SetShowBackupsFilter();
        }

        /// <summary>
        /// Display a message box telling the user that there are no
        /// output (label and list) documents to display and
        /// directing the user to import some VESTA reports in order
        /// to create some output documents.
        /// </summary>
        private void ShowNoFilesMessage()
        {
            MessageBox.Show(
                GlobRes.NoOutputFilesMsg, GlobRes.NoOutputFilesTitle,
                MessageBoxButtons.OK, MessageBoxIcon.Information
                );

        }


        /// <summary>
        /// After the list of matching files has changed,
        /// repopulate the form and set the controls.
        /// </summary>
        private void UpdateView()
        {
            PopulateForm(first_run: false);
            FileListManager.ApplyFilters();
            if (FileListManager.IsEmpty)
            {
                ShowNoFilesMessage();
            }
        }

        /// <summary>
        /// Tell the FileListManager to refresh its list of
        /// files.
        /// 
        /// If the list isn't empty, set up the visible
        /// controls and fill up the listview.
        /// </summary>
        private void PopulateForm(bool first_run = false)
        {
            FileListManager.LoadSourceFileList();
            if (!FileListManager.IsEmpty)
            {
                InitializeComboBoxes(set_to_zero: first_run);
                FileListManager.ApplyFilters();
                FillFileListView();
                FormatFileListView();
                SetButtonAndCheckboxState();
            }
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            PopulateForm(first_run: true);
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            if (FileListManager.IsEmpty)
                ShowNoFilesMessage();
        }

        /// <summary>
        /// Show matching files and enable relevant controls when
        /// the TypeToView selection is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbTypeToView_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTypeFilter();
            // Disable donor combo when file type is one where donors aren't relevant.
            //if (HllUtils.TypeHasDonor(FileListManager.TypeFilter))
            //    cmbDonor.Enabled = true;
            cmbDonor.Enabled = FileListManager.TypeFilter.HasDonor();
            //    cmbDonor.Enabled = true;
            //else
            //    cmbDonor.Enabled = false;

            this.FileListManager.ApplyFilters();
            this.PopulateForm(first_run: false);
        }

        private void cmbYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetYearFilter();
            string y = FileListManager.YearFilter;
            if (!String.IsNullOrEmpty(y))
            {
                PopulateDonorCombo(y, set_to_zero: false);
                SetDonorFilter();
            }
            this.FileListManager.ApplyFilters();
            this.PopulateForm(first_run: false);
        }

        private void cmbDonor_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDonorFilter();
            this.FileListManager.ApplyFilters();
            this.PopulateForm(first_run: false);
        }

        /// <summary>
        /// When user double-clicks on an item in the
        /// list of available files, tell the operating system to open
        /// it with the default application (Word or Excel).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvAvailableFiles_DoubleClick(object sender, EventArgs e)
        {
            string fn = lvAvailableFiles.SelectedItems[0].Text;
            string filespec = FileListManager.FullPathForFile(fn);
            if (File.Exists(filespec))
                HllUtils.OpenFile(filespec);
            else
                MessageBox.Show(
                    string.Format(GlobRes.FileNotFoundMsg, filespec),
                    GlobRes.FileNotFoundTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
        }

        private void chbxIncludeBackups_CheckedChanged(object sender, EventArgs e)
        {
            SetShowBackupsFilter();
            FileListManager.ApplyFilters();
            this.PopulateForm(first_run: false);
        }


        /// <summary>
        /// Open Output File Maintenance dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMaintenance_Click(object sender, EventArgs e)
        {
            using (frmFileMaintenance MaintForm = new frmFileMaintenance(this.FileListManager))
            {
                MaintForm.ShowDialog();
                if (MaintForm.FilesChanged)
                {
                    PopulateForm(first_run: false);
                    UpdateView();
                }
            }
        }
       

        /// <summary>
        /// Display general program help.
        /// The Form property called "Text" actually
        /// sets a form's title.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHelp_Click(object sender, EventArgs e)
        {
            frmHelp helpForm = new frmHelp();
            string doc_html = Properties.Resources.Doc_HTML;
            helpForm.HelpText = doc_html;
            helpForm.Text = GlobRes.DocGeneralTitle;
            helpForm.ShowDialog();
        }

        /// <summary>
        /// Import contents of each file selected
        /// by the user.
        /// </summary>
        /// <param name="wk"></param>
        /// <param name="context"></param>
        /// <param name="report_names"></param>
        /// <returns></returns>
        /// TODO: Move this method out of this class. It doesn't
        /// refer to anything in frmMain and does no user interaction. It should probably
        /// be in a helper or utility class -- perhaps VestaImporterUtils?
        /// 
        private int ImportFromVesta(BackgroundWorker wk,
            DBWrapper context, string[] report_names)
        {
            int retInt = 0;
            foreach (string fn in report_names)
            {
                if (Path.GetExtension(fn) == ".xlsx")
                {
                    VestaImporter p = new VestaImporter(wk, fn, GlobRes.ResultsSheetDefaultName);
                    retInt += p.execute(context);
                }
            }
            return retInt;
        }

        /// <summary>
        /// Read info from VESTA reports. Store the info in
        /// a DBWrapper object. Then use the data in the DBWrapper
        /// to generate output files.
        /// 
        /// During processing, display progress messages.
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="report_names"></param>
        /// <returns></returns>
        private int DoProcessing(BackgroundWorker worker,
            string[] report_names)
        {
            int retInt = 0;
            this.context.Clean();
            worker.ReportProgress(0,
                string.Format(GlobRes.VestaReportCountMsg, report_names.Length)
                );
            ImportFromVesta(worker, this.context, report_names);
            if (!worker.CancellationPending)
            {

                worker.ReportProgress(0, GlobRes.GeneratingOutputFilesMsg);
                retInt = HllUtils.MakeOutputFiles(worker, this.context);
            }
            return retInt;
        }

        private void btnAddVestaReports_Click(object sender, EventArgs e)
        {
            string[] report_names = HllUtils.GetVestaReportNames();
            if (report_names != null)
            {
                try
                {
                    // create an object to handle communication between
                    // background stuff and progress dialog:
                    _bgworker = HllUtils.MakeWorker(
                        new DoWorkEventHandler(bgworker_DoWork),
                        new RunWorkerCompletedEventHandler(bgworker_RunWorkerCompleted),
                        new ProgressChangedEventHandler(bgworker_ProgressChanged)
                        );
                    SetAppState(AppStates.Processing);
                    // create and configure a progress dialog:
                    ProgressForm = new frmProgress();
                    ProgressForm.Done = false;
                    ProgressForm.Worker = _bgworker;
                    // Hook up "FormClosed" event handler:
                    ProgressForm.FormClosed += ProgressForm_FormClosed;
                    ProgressForm.Show();
                    ProgressForm.AddMessage(GlobRes.VestaReportProcessingStartMsg);
                    // start the background work:
                    _bgworker.RunWorkerAsync(report_names);
                }
                catch
                {
                    SetAppState(AppStates.Viewing);
                    if (ProgressForm != null)
                        ProgressForm.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Get notified when the progress form closes,
        /// so that we can show ourselves again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (CurrentState != AppStates.Viewing)
            {
                SetAppState(AppStates.Viewing);
            }
        }
        /////////////////////////////////////
        // Background thread stuff: /////////
        ////////////////////////////////////



        /// <summary>
        /// The possible states of the app are:
        /// 
        /// Processing -- in the process of importing data from VESTA reports,
        ///     or creating label or list documents using the imported data.
        ///     
        /// Viewing -- The startup state. Present a list of label and list files
        ///     to the user, along with buttons to perform various operations.
        ///     
        /// Showing Work -- Exhibiting the results of the Processing stage.
        /// 
        /// </summary>
        /// <param name="newState"></param>

        private void SetAppState(AppStates newState)
        {
            CurrentState = newState;
            switch (CurrentState)
            {
                case AppStates.Processing:
                    this.Hide();
                    break;
                case AppStates.Viewing:
                    this.Show();
                    break;
                case AppStates.ShowingWork:
                    ProgressForm.Done = true;
                    break;
            }
        }

        /// <summary>
        /// Launch the processing code in a separate thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker wk = sender as BackgroundWorker;
            string[] args = (string[])e.Argument;
            e.Result = DoProcessing(wk, args);
            if (wk.CancellationPending)
                e.Cancel = true;
        }

        private void bgworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
                ProgressForm.Done = true;
            ProgressForm.AddMessage((string)e.UserState);
        }

        /// <summary>
        /// Called when background worker RunWorkerAsynch task finishes,
        /// that is, when exiting Processing state and entering
        /// ShowingWork state.
        /// 
        /// Update progress form and, if appropriate, update
        /// file list view, then set state to "ShowingWork,"
        /// leaving the progress form displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    ProgressForm.AddMessage(
                        string.Format(GlobRes.VestaReportExceptionMsg,
                            e.Error.Message)
                            );
                }
                else if (e.Cancelled)
                {
                    ProgressForm.AddMessage(
                        GlobRes.ProcessingCancelledMsg + GlobRes.OKToCloseMsg
                        );
                }
                else
                {
                    int res = (int)e.Result;
                    ProgressForm.AddMessage(
                        string.Format(GlobRes.FileAddingSuccessMsg, res)
                        );
                    ProgressForm.AddMessage(GlobRes.OKToCloseMsg);
                    if (res > 0)
                        UpdateView();
                }
            }
            finally
            {
                SetAppState(AppStates.ShowingWork);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
