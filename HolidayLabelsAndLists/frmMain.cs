using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DAO;
using HolidayLabelsAndListsHelper;
using VestaImporter;
//using GlobRes = AppWideResources.Properties.Resources;

namespace HolidayLabelsAndLists
{
    public partial class frmMain : Form
    {
        internal enum ProcessingType
        {
            IMPORT,
            GENERATE,
            BOTH
        }
        internal class BGWorkerResult
        {
            internal int ReportsReadCount { get; set; }
            internal int FilesGeneratedCount { get; set; }
            internal ProcessingType Type { get; set; }
        }

        private DBWrapper Context = new DBWrapper();
        private HllFileListManager FileListManager;

        private enum AppStates { Processing, Viewing, ShowingWork };

        private AppStates CurrentState;
        private BackgroundWorker _bgworker;
        private frmProgress ProgressForm;
        private Dictionary<string, string> OutputDocTypes;


        public frmMain()
        {
            InitializeComponent();
            Context.Load();
            FileListManager = new HllFileListManager(Context);
            PopulateTypeToViewCombo(set_to_zero: true);
            SetCaptions();
            SetAppState(AppStates.Viewing);
        }

        private void SetCaptions()
        {
            this.btnDeleteOldFiles.Text = Properties.Resources.DelOldFilesDialogBtnCaption;
        }

        /// <summary>
        /// Fill the listview with the name of each file.
        /// Set each item's tooltip to the same vallue.
        /// </summary>
        private void FillFileListView()
        {
            lvAvailableFiles.Items.Clear();
            foreach (string s in FileListManager.FileNameList())
            {
                ListViewItem item = new ListViewItem(s);
                item.ToolTipText = s;
                lvAvailableFiles.Items.Add(item);
            }
            // No matches? Then create an item saying so
            // to display in the file list.
            if (lvAvailableFiles.Items.Count == 0)
            {
                ListViewItem item = new ListViewItem(Properties.Resources.NoMatchingFilesMsg);
                item.ToolTipText = Properties.Resources.NoMatchingFilesTooltip;
                lvAvailableFiles.Items.Add(item);
            }

        }

        /// <summary>
        /// Set cmbYear's list to all of the years contained
        /// in our data's records.
        /// 
        /// Save the old value, and if set_to_zero==false, reselect
        /// that value when done. If set_to_zero==true, select first
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

        /// <summary>
        /// Initialize the collection of values used by cmbTypeToView
        /// and set the selection to the first one.
        /// 
        /// NOTE: Unlike the other "Populate...()" methods, there is
        /// nothing for this one to do unless set_to_zero is true.
        /// </summary>
        /// <param name="set_to_zero"></param>
        private void PopulateTypeToViewCombo(bool set_to_zero = true)
        {
            if (set_to_zero == true)
            {
                // turn IndexChanged event handler off
                cmbTypeToView.SelectedIndexChanged -= cmbTypeToView_SelectedIndexChanged;
                //string[] DocTypeKeys = Properties.Resources.DocumentTypesKeys.Split('#');
                string[] DocTypeKeys = Enum.GetNames(typeof(output_doc_types));
                string[] DocTypeValues = Properties.Resources.DocumentTypesValues.Split('#');
                this.OutputDocTypes = DocTypeKeys.Zip(DocTypeValues, (k, v)
                    => new { k, v }).ToDictionary(x => x.k, x => x.v);
                cmbTypeToView.DataSource = new BindingSource(this.OutputDocTypes, null);
                cmbTypeToView.ValueMember = "Key";
                cmbTypeToView.DisplayMember = "Value";
                cmbTypeToView.SelectedIndex = 0;
                // turn IndexChanged event handler back on
                cmbTypeToView.SelectedIndexChanged += cmbTypeToView_SelectedIndexChanged;
            }
        }

        /// <summary>
        /// Set column width of file name column to magic number which
        /// means "full width of parent"
        /// </summary>
        private void FormatFileListView()
        {
            lvAvailableFiles.Columns[0].Width = -2;
        }

        /// <summary>
        /// Only enable the "Include Backups" button if there are any
        /// backup files in FileListManager.
        /// </summary>
        private void SetButtonAndCheckboxState()
        {
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
            //FileListManager.ApplyFilters();
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

        /// <summary>
        /// Set the type filter according to the selected
        /// item of the type combo box. If there is no
        /// selected item, set the type filter to "ALL."
        /// </summary>
        private void SetTypeFilter()
        {
            object o = cmbTypeToView.SelectedItem;
            output_doc_types ty;
            if (o != null)
            {
                KeyValuePair<string, string> kv = (KeyValuePair<string, string>)o;
                ty = new output_doc_types();
                // This TryParse() should not fail, since kv.Key should be
                // the string representation of a valid type:
                if (!Enum.TryParse(kv.Key, out ty))
                    ty = output_doc_types.INVALID;
            }
            else
            {
                ty = output_doc_types.ALL;
            }
            FileListManager.TypeFilter = new FilterSetTypeFilters(ty);
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
                Properties.Resources.NoOutputFilesMsg, Properties.Resources.NoOutputFilesTitle,
                MessageBoxButtons.OK, MessageBoxIcon.Information
                );
        }

        /// <summary>
        /// Display a message box telling the user that VESTA data
        /// needs to be imported.
        /// </summary>
        private void ShowNothingImportedMessage()
        {
            MessageBox.Show(
                Properties.Resources.NothingImportedMsg, Properties.Resources.NothingImportedTitle,
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
            cmbDonor.Enabled = FileListManager.TypeFilter.HasDonor();
            this.FileListManager.ApplyFilters();
            this.PopulateForm(first_run: false);
        }

        /// <summary>
        /// Set the year filter. Since the relevant donor values
        /// may be different for each year, get the list
        /// of relevant donors and set that filter too.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    string.Format(Properties.Resources.FileNotFoundMsg, filespec),
                    Properties.Resources.FileNotFoundTitle,
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
            helpForm.Text = Properties.Resources.DocGeneralTitle;
            helpForm.ShowDialog(this);
        }

        /// <summary>
        /// Read info from VESTA reports. Store the info in
        /// a DBWrapper object.
        /// 
        /// Return number of reports read
        /// 
        /// During processing, display progress messages.
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="report_names"></param>
        /// <returns></returns>
        private int DoImportProcessing(BackgroundWorker worker,
            string[] report_names)
        {
            int retInt = 0;
            //this.Context.Clean();
            worker.ReportProgress(0,
                string.Format(Properties.Resources.VestaReportCountMsg, report_names.Length)
                );
            retInt = VestaImporterUtils.ImportFromVesta(worker, this.Context, report_names);
            return retInt;
        }

        /// <summary>
        /// Generate label and list documents. Return number of documents
        /// created.
        /// 
        /// Display progress messages during processing.
        /// </summary>
        /// <param name="worker"></param>
        /// <returns></returns>
        private int DoOutputProcessing(BackgroundWorker worker, int[]years)
        {
            worker.ReportProgress(0, Properties.Resources.GeneratingOutputFilesMsg);
            return HllUtils.MakeOutputFiles(worker, years, this.Context);
        }

        private void btnAddVestaReports_Click(object sender, EventArgs e)
        {
            AddVestaReports(sender, e);
        }

        private void AddVestaReports(object sender, EventArgs e)
        {
            string[] report_names = HllUtils.GetVestaReportNames();
            if (report_names != null)
            {
                try
                {
                    // create an object to handle communication between
                    // background stuff and progress dialog:
                    _bgworker = HllUtils.MakeWorker(
                        new DoWorkEventHandler(bgworker_DoImportWork),
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
                    ProgressForm.AddMessage(Properties.Resources.VestaReportProcessingStartMsg);
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

        private void btnCreateOutput_Click(object sender, EventArgs e)
        {
            int answer = Utils.MessageBoxUtils.GetEitherOrChoice(
                title: Properties.Resources.GenerateAllYearsTitle,
                prompt_text: Properties.Resources.GenerateAllYearsPrompt,
                button_a_text: Properties.Resources.GenerateAllYearsYesButtonText,
                button_b_text: Properties.Resources.GenerateAllYearsNoButtonText);
            if (answer == -1) // user clicked "Cancel"
                return;
            int[] years = HllUtils.YearsInDb(this.Context);
            if (answer == 1) // user wants only most recent two years
                years = years.Take(2).ToArray();

            CreateOutput(sender, e, years);
        }

        private void CreateOutput(object sender, EventArgs e, int[] years)
        {
            try
            {
                // create an object to handle communication between
                // background stuff and progress dialog:
                _bgworker = HllUtils.MakeWorker(
                    new DoWorkEventHandler(bgworker_DoGenerateWork),
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
                ProgressForm.AddMessage(Properties.Resources.GeneratingOutputFilesMsg);
                // start the background work:
                _bgworker.RunWorkerAsync(years);
            }
            catch
            {
                SetAppState(AppStates.Viewing);
                if (ProgressForm != null)
                    ProgressForm.Close();
                throw;
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
                    if (this.Context.IsEmpty)
                        ShowNothingImportedMessage();
                    else if (this.FileListManager.IsEmpty)
                        ShowNoFilesMessage();
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
        private void bgworker_DoImportWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker wk = sender as BackgroundWorker;
            string[] args = (string[])e.Argument;
            BGWorkerResult bgRes = new BGWorkerResult();
            bgRes.Type = ProcessingType.IMPORT;
            bgRes.ReportsReadCount = DoImportProcessing(wk, args);
            if (wk.CancellationPending)
                e.Cancel = true;
            else
                e.Result = bgRes;
        }

        /// <summary>
        /// Launch the processing code in a separate thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgworker_DoGenerateWork(object sender, DoWorkEventArgs e)
        {
            int[] years = (int[])e.Argument;
            BackgroundWorker wk = sender as BackgroundWorker;
            BGWorkerResult bgRes = new BGWorkerResult();
            bgRes.Type = ProcessingType.GENERATE;
            bgRes.FilesGeneratedCount = DoOutputProcessing(wk, years);
            if (wk.CancellationPending)
                e.Cancel = true;
            else
                e.Result = bgRes;
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
                        string.Format(Properties.Resources.VestaReportExceptionMsg,
                            e.Error.Message)
                            );
                }
                else if (e.Cancelled)
                {
                    ProgressForm.AddMessage(
                        Properties.Resources.ProcessingCancelledMsg + Properties.Resources.OKToCloseMsg
                        );
                }
                else
                {
                    BGWorkerResult bgRes = (BGWorkerResult)e.Result;
                    string msg = null;
                    switch(bgRes.Type)
                    {
                        case ProcessingType.GENERATE:
                            msg = string.Format(Properties.Resources.FileAddingSuccessMsg, bgRes.FilesGeneratedCount);
                            break;
                        case ProcessingType.IMPORT:
                            msg = string.Format(Properties.Resources.VestaReportProcessingSuccessMsg, bgRes.ReportsReadCount);
                            break;
                        case ProcessingType.BOTH:
                            msg = string.Format(Properties.Resources.VestaReportProcessingSuccessMsg, bgRes.ReportsReadCount) +
                                Environment.NewLine +  string.Format(Properties.Resources.FileAddingSuccessMsg, bgRes.FilesGeneratedCount);
                            break;
                    }
                    ProgressForm.AddMessage(Environment.NewLine + msg);
                    ProgressForm.AddMessage(Properties.Resources.OKToCloseMsg);
                    ProgressForm.AddMessage(Environment.NewLine);
                    if (bgRes.FilesGeneratedCount > 0)
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

        private void btnDeleteOldFiles_Click(object sender, EventArgs e)
        {
            using (frmDeleteOldFiles OldFilesForm = new frmDeleteOldFiles(this.FileListManager))
            {
                OldFilesForm.ShowDialog(this);
                if (OldFilesForm.FilesChanged)
                {
                    PopulateForm(first_run: false);
                    UpdateView();
                }
            }
        }

    }
}
