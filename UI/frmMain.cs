using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using DAO;
using DTO;

namespace UI
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            frmProcessReports processForm = new frmProcessReports();
            this.Hide();
            processForm.ShowDialog();
            this.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnViewLabels_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Properties.Settings.Default.TopDirectory);
        }

        private void btnViewLists_Click(object sender, EventArgs e)
        {
            
        }

        private void btnManageDonors_Click(object sender, EventArgs e)
        {
            frmDonors DonorsForm = new frmDonors();
            this.Hide();
            DonorsForm.OnDonorListEventHandler += this.OnDonorListChanged;
            DonorsForm.ShowDialog();

            this.Show();
            DonorsForm.OnDonorListEventHandler -= this.OnDonorListChanged;

        }

        private void OnDonorListChanged(object source, EventArgs e)
        {
            this._fillDonorDropdown();
        }

        private void _fillDonorDropdown()
        {
            List<Donor> dlist = new List<Donor>();
            using (ValleyLabelsAndListsEntities context = new ValleyLabelsAndListsEntities())
            {
                foreach(donor d in context.donors.ToList())
                {
                    dlist.Add(new Donor(d.id, d.name, d.code));
                }
            }
            this.cmbViewPrintDonor.DataSource = dlist;
            this.cmbViewPrintDonor.ValueMember = "Id";
            this.cmbViewPrintDonor.DisplayMember = "Name";
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this._fillDonorDropdown();
        }
    }
}
