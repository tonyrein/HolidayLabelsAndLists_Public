using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Utils
{
    public partial class frmEitherOr : Form
    {
        public frmEitherOr()
        {
            InitializeComponent();
        }

        public frmEitherOr(string title, string prompt_text, string button_a_text, string button_b_text, string button_cancel_text, bool show_cancel_button)
        {
            InitializeComponent();
            this.Text = title;
            this.lblPrompt.Text = prompt_text;
            this.btnA.Text = button_a_text;
            this.btnB.Text = button_b_text;
            this.btnCancel.Text = button_cancel_text;
            this.btnCancel.Enabled = show_cancel_button;
        }
    }
}
