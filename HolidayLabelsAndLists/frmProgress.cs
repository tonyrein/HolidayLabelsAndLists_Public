using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace HolidayLabelsAndLists
{
    /// <summary>
    /// A popup window to be used by the background worker
    /// to display summary of progress.
    /// 
    /// </summary>
    public partial class frmProgress : Form
    {
        public BackgroundWorker Worker { get; set; }
        private bool _done;
        public bool Done
        {
            get
            {
                return _done;
            }
            set
            {
                _done = value;
                SetControls(_done);
            }
        }
        public frmProgress()
        {
            InitializeComponent();
            Done = false;
        }

        /// <summary>
        /// Reset progress display
        /// </summary>
        public void Clear()
        {
            txtDisplay.Clear();
        }

        /// <summary>
        /// Cancel buton enabled until we're done;
        /// then Close button enabled.
        /// </summary>
        /// <param name="isDone"></param>
        private void SetControls(bool isDone)
        {
            btnCancel.Enabled = !isDone;
            btnClose.Enabled = isDone;
        }
        /// <summary>
        /// Append a string to the end of the display
        /// </summary>
        /// <param name="msg"></param>
        public void AddMessage(string msg)
        {
            txtDisplay.AppendText(msg + Environment.NewLine);
        }

        /// <summary>
        /// If user clicks Cancel button, send word to BackgroundWorker
        /// to stop. Make appropriate changes to our display.
        /// </summary>
        public void Cancel()
        {
            if (Worker != null) 
                Worker.CancelAsync();
            Done = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((!Done) && (!Worker.CancellationPending))
                e.Cancel = true;
            else
                e.Cancel = false;
        }
    }
}
