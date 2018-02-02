using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using DAO;
using ReportProcessors;

namespace UI
{

    public partial class frmProcessReports : Form
    {
        public class CountChangedEventArgs : EventArgs
        {
            private readonly int _count;
            public CountChangedEventArgs(int i)
            { this._count = i; }
            public int Count
            {
                get { return this._count; }
            }
        }
        private class ReportListManager
        {
            //private Form containerForm;
            private string _folder;
            private CheckedListBox _managedListBox;
            private List<String> _pathList = new List<String>();
            internal ReportListManager(string folder, CheckedListBox clb)
            {
                //this.containerForm = f;
                this._folder = folder;
                this._managedListBox = clb;
                this._fill();
            }

            private void _fill()
            {
                this._managedListBox.Items.Clear();
                this._pathList.Clear();
                // Populate list of files in this._folder,
                // including folder name -- ie, full paths:
                string[] patts = { "*.xls?" };
                foreach (string patt in patts)
                {
                    this._pathList.AddRange(Directory.GetFiles(this._folder, patt).ToList());
                }

                this._pathList.Sort();
                // Now get list of filenames only, to be displayed
                // in the visible control.
                IEnumerable<string> query = from s in this._pathList
                                            orderby s
                                            select Path.GetFileName(s);
                this._managedListBox.Items.AddRange(query.ToArray<object>());
            } // end of _fill()

            public bool hasItemsChecked()
            {
                return this.selectedIndices().Count() > 0;
            }

            public int Count
            {
                get { return this._managedListBox.Items.Count; }
            }

            public string Folder
            {
                get { return this._folder; }
            }

            /// <summary>
            /// Returns path (folder + filename) for
            /// report at givenindex
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public string filepathForIndex(int index)
            {
                return this._pathList[index];
            }
            public List<int> selectedIndices()
            {
                // Sort result numerically but in descending order. This
                // is neccessary due to the structure of the checked listbox
                // items collection.
                //
                // Only include items that are actually checked. (For some reason,
                // CheckedIndices includes indices of items that are in an indeterminate state,
                // not just checked.
                IEnumerable<int> query = from int i in this._managedListBox.CheckedIndices
                                         where this._managedListBox.GetItemChecked(i)
                                         orderby i descending
                                         select i;
                return query.ToList<int>();
            }

            public void deleteSelectedFiles()
            {
                foreach (int i in this.selectedIndices())
                {
                    this.doDelete(i);
                }
            }

            // CRUD: What to do when files are added or deleted
            public void doDelete(int selectedIndex)
            {
                // Find name of file at selected index.
                string pathname = this.filepathForIndex(selectedIndex);
                // Attempt to delete the file from disk. If that succeeds,
                // (ie no Exception raised), then delete that item
                // from checkedlistbox's items collection and pathname list.
                File.Delete(pathname);
                this._pathList.RemoveAt(selectedIndex);
                this._managedListBox.Items.RemoveAt(selectedIndex);
                CountChangedEventArgs e = new CountChangedEventArgs(this._managedListBox.Items.Count);
                this.raiseCountChangedEvent(this, e);
            }
            public void doAdd(string filename)
            {
                this._managedListBox.Items.Add(filename);
                this._pathList.Add(Path.Combine(this._folder, filename));
                this._pathList.Sort();
                CountChangedEventArgs e = new CountChangedEventArgs(this._managedListBox.Items.Count);
                this.raiseCountChangedEvent(this, e);
            }

            // event handling stuff

            public event EventHandler<CountChangedEventArgs> countChange;
            public void raiseCountChangedEvent(object sender, CountChangedEventArgs e)
            {
                if (countChange != null)
                {
                    countChange(this, e);
                }
            }


        } // end of class ReportListManager

        private ReportListManager pendingManager;
        private ReportListManager processedManager;
        private int _marginWidth = 30;


        private void _setPendingButtonsStatePerSelected(bool willBeCheckedItems)
        {
            this.btnPRDeleteSelectedPending.Enabled = willBeCheckedItems;
            this.btnPRProcessSelectedPending.Enabled = willBeCheckedItems;
        }

        private void _setCompletedButtonsStatePerSelected(bool willBeCheckedItems)
        {
            this.btnPRMarkForReprocessing.Enabled = willBeCheckedItems;
            this.btnPRDeleteSelectedDone.Enabled = willBeCheckedItems;
        }
        private void _setPendingButtonsStatePerItemCount(int itemCount)
        {
            this.btnPRProcessAllPending.Enabled = (itemCount > 0);
        }
        private void _setCompletedButtonsStatePerItemCount(int itemCount)
        {
            if (itemCount == 0) // no items means none selected
            {
                this._setCompletedButtonsStatePerSelected(false);
            }

        }
        private void _initializeControls()
        {
            this._setPendingButtonsStatePerSelected(false);
            this._setCompletedButtonsStatePerSelected(false);
            this._setPendingButtonsStatePerItemCount(this.pendingManager.Count);
            this._setCompletedButtonsStatePerItemCount(this.processedManager.Count);
        }

        private void reportListManager_handlecountChange(object sender, CountChangedEventArgs e)
        {
            if (sender == this.pendingManager)
            {
                //MessageBox.Show(e.Count.ToString(), "Event handler called by pendingManager!");
                this._setPendingButtonsStatePerItemCount(e.Count);
                if (e.Count == 0) // no items means none selected
                {
                    this._setPendingButtonsStatePerSelected(false);
                }
            }
            if (sender == this.processedManager)
            {
                this._setCompletedButtonsStatePerItemCount(e.Count);
                if (e.Count == 0)
                {
                    this._setCompletedButtonsStatePerSelected(false);
                }
            }
        }
        public frmProcessReports()
        {
            InitializeComponent();
            //string listFolder = Path.Combine(Properties.Settings.Default.TopDirectory,
            //     Properties.Settings.Default.PendingReportsFolder);
            string listFolder = FolderManager.PendingReportsFolder;
            this.pendingManager = new ReportListManager(listFolder,
                this.chklstboxPendingReports);
            this.pendingManager.countChange += reportListManager_handlecountChange;
            //listFolder = Path.Combine(Properties.Settings.Default.TopDirectory,
            //    Properties.Settings.Default.ProcessedReportsFolder);
            listFolder = FolderManager.ProcessedReportsFolder;
            this.processedManager = new ReportListManager(listFolder,
                this.chklistboxProcessedReports);
            this.processedManager.countChange += reportListManager_handlecountChange;
        }

        private void btnProcessHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This message box will be replaced by a dialog that will show help and instructions.", "Processing VESTA Reports");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmProcessReports_Load(object sender, EventArgs e)
        {
            this._initializeControls();
        }

        // TODO: Refactor this so it's not so ugly. Will probably require adding
        // a button list to the manager, along with rules for each control -- ie
        // whether it should be enabled or disabled with item count == 0 and
        // with selected count == 0.
        private void reportListManager_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (sender == this.chklstboxPendingReports)
            {
                // If new value is "Checked," then we know we'll have at least one
                // item selected. If it's not "Checked," then find out if the
                // item being unchecked is the last one.
                if (CheckState.Checked == e.NewValue) // we know we'll now have at least one item checked
                {
                    this._setPendingButtonsStatePerSelected(true);
                }
                else // something's getting unchecked. Is it the last item?
                {
                    if (1 == this.pendingManager.selectedIndices().Count)
                    {
                        this._setPendingButtonsStatePerSelected(false);
                    }
                }
            }
            else
            {
                if (CheckState.Checked == e.NewValue)
                {
                    this._setCompletedButtonsStatePerSelected(true);
                }
                else
                {
                    if (1 == this.processedManager.selectedIndices().Count)
                    {
                        this._setCompletedButtonsStatePerSelected(false);
                    }
                }
            }
        }

       

        private void _set_listbox_positions()
        {
            int groupboxWidth = (this.ClientSize.Width - (3 * this._marginWidth)) / 2;
            this.grpCompleted.Width = groupboxWidth;
            this.grpPending.Width = groupboxWidth;
            this.grpPending.Left = this._marginWidth;
            this.grpCompleted.Left = (2 * this._marginWidth) + groupboxWidth;
        }

        private void frmProcessReports_Resize(object sender, EventArgs e)
        {
            this._set_listbox_positions();
        }

        private void btnPRDeleteSelectedDone_Click(object sender, EventArgs e)
        {
            if (this.processedManager.hasItemsChecked())
            {
                if (MessageBox.Show("Are you sure you want to delete these reports?", "Confirm Report Deletion",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.processedManager.deleteSelectedFiles();
                }
            }
        }


        private void markForReprocessing(int index)
        {
            string sourcepath = null;
            try
            {
                sourcepath = this.processedManager.filepathForIndex(index);
            }
            catch (IndexOutOfRangeException e)
            {
                MessageBox.Show("No report found -- " + index + " is not a valid index.");
            }
            if (sourcepath != null)
            {
                this.moveReportsToOtherList(index, sourcepath,
                    this.processedManager, this.pendingManager,
                    confirmOverwrite: true);
            }
        }

        private void btnPRMarkForReprocessing_Click(object sender, EventArgs e)
        {
            foreach(int i in processedManager.selectedIndices())
            {
                this.markForReprocessing(i);
            }
        }


        private void _alignToMiddle(Control masterControl, Control controlToAlign)
        {
            int middleX = (masterControl.Right - masterControl.Left) / 2;
            controlToAlign.Left = middleX - (controlToAlign.Width / 2);
        }
      
        private void chklstboxPendingReports_Resize(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            this._alignToMiddle(c, grpPendingButtons);
        }
        private void chklistboxProcessedReports_Resize(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            this._alignToMiddle(c, grpCompletedButtons);
        }

        
        private void btnPRDeleteSelectedPending_Click(object sender, EventArgs e)
        {
            if (this.pendingManager.hasItemsChecked())
            {
                if (MessageBox.Show("Are you sure you want to delete these reports?", "Confirm Report Deletion",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.pendingManager.deleteSelectedFiles();
                }
            }
        }

        private void btnPRImport_Click(object sender, EventArgs e)
        {
            // show dialog to choose files.
            // for each file:
            //    copy to _pendingManager.Folder. Confirm if already there.
            //    call _pendingManager.doAdd()
            OpenFileDialog d = new OpenFileDialog();
            d.Multiselect = true;
            d.Filter = "Excel Files (*.XLS;*XLS?)|*.XLS;*.XLS?|All Files (*.*)|*.*";
            d.FilterIndex = 1;
            d.Title = "Select VESTA Reports";
            DialogResult dr = d.ShowDialog();
            if (dr == DialogResult.OK)
            {
                foreach (string sourcefile in d.FileNames)
                {
                    string filename = Path.GetFileName(sourcefile);
                    string targetfile = Path.Combine(this.pendingManager.Folder, filename);
                    if (File.Exists(targetfile))
                    {
                        if (MessageBox.Show("File " + filename + " already in reports list. Replace?", "Replace Report File?",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                            continue; // don't process this one -- go to next loop iteration
                    }
                    File.Copy(sourcefile, targetfile, true); // true means OK to overwrite
                    this.pendingManager.doAdd(filename);
                }

            }
        }

        private void moveReportsToOtherList(int index, string sourcepath,
            ReportListManager fromList, ReportListManager toList, bool confirmOverwrite = false)
        {
            string filename = Path.GetFileName(sourcepath);
            string targetpath = Path.Combine(toList.Folder, filename);
            bool proceed = true;
            if (confirmOverwrite && File.Exists(targetpath))
            {
                proceed = (MessageBox.Show("File " + filename + " already in target folder. Overwrite?",
                    "Confirm Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
                    == DialogResult.Yes);
            }

            if (proceed)
            {
                FilesystemUtils.MoveWithReplace(sourcepath, targetpath);
                fromList.doDelete(index);
                toList.doAdd(filename);
            }

        }
        private void _processOneReport(int index)
        {
            string sourcepath = null;
            try
            {
                sourcepath = this.pendingManager.filepathForIndex(index);
            }
            catch (IndexOutOfRangeException e)
            {
                MessageBox.Show("No report found -- " + index + " is not a valid index.");
            }
            if (sourcepath != null)
            {
                ReportProcessor p = new ReportProcessor(sourcepath);
                bool executionResult = p.execute();
                if (executionResult == true) // success
                {
                    this.moveReportsToOtherList(index, sourcepath,
                        this.pendingManager, this.processedManager,
                        confirmOverwrite:false);
                }
            }
        }
        private void btnPRProcessAllPending_Click(object sender, EventArgs e)
        {
            // Loop from last to first so that indices of remaining
            // files don't change as each file is processed.
            for(int i = this.pendingManager.Count - 1; i >= 0; i--)
            {
                this._processOneReport(i);
            }
        }

        private void btnPRProcessSelectedPending_Click(object sender, EventArgs e)
        {
            foreach (int i in this.pendingManager.selectedIndices())
            {
                this._processOneReport(i);
            }
        }
    } // end of class frmProcessReports
} // end of namespace

