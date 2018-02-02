using System;
using System.Windows.Forms;

namespace HolidayLabelsAndLists
{
    /// <summary>
    /// A class to display HTML help text in a popup window.
    /// The HTML text is loaded from a string resource.
    /// </summary>
    public partial class frmHelp : Form
    {
        public frmHelp()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fill in the contents of our web browser component.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmHelp_Load(object sender, EventArgs e)
        {
            string doc_html = Properties.Resources.Doc_HTML;
            this.webBrowser1.DocumentText = doc_html;
        }
    }
}
