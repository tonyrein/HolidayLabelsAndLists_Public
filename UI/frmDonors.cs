using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;

using DAO;

namespace UI
{
    public partial class frmDonors : Form
    {
       
        private BindingSource bs;
        private ValleyLabelsAndListsEntities ctx;
        private bool contents_changed;

        // event handling:
        public delegate void DonorListEventHandler(object source,
            EventArgs e);

        public event DonorListEventHandler OnDonorListEventHandler;

        public void UpdateDonorList()
        {
            if (OnDonorListEventHandler != null)
            {
                OnDonorListEventHandler(this, EventArgs.Empty);
            }
        }

        
        public frmDonors()
        {
            InitializeComponent();
        }

        private void frmDonors_Load(object sender, EventArgs e)
        {
            this.ctx = new ValleyLabelsAndListsEntities();
            this.ctx.donors.Load();
            BindingList<donor> donors_binding_list =
                this.ctx.donors.Local.ToBindingList<donor>();
            donors_binding_list.AllowEdit = true;
            donors_binding_list.AllowNew = true;

            bs = new BindingSource();
            bs.DataSource = donors_binding_list;
            bs.AllowNew = true;
            dgvDonors.DataSource = bs;
            dgvDonors.Columns[0].FillWeight = 25;
            dgvDonors.Columns[0].HeaderText = "Code";
            dgvDonors.Columns[1].FillWeight = 75;
            dgvDonors.Columns[1].HeaderText = "Name";
            dgvDonors.Refresh();
            this._mark_as_changed(false);
//            contents_changed = false;
//            this._set_buttons_to_enabled(false);
        }

        private void _do_save()
        {
            this.ctx.SaveChanges(); // persist to DB
            this.UpdateDonorList(); // fire change event
            this._mark_as_changed(false);
//            this.contents_changed = false;
//            this._set_buttons_to_enabled(false);
        }

        private void _mark_as_changed(bool dirty)
        {
            this.contents_changed = dirty;
            this._set_buttons_to_enabled(dirty);
        }
        private void _set_buttons_to_enabled(bool enable)
        {
            this.btnDonorsSave.Enabled = enable;
        }
        private void btnDonorsSave_Click(object sender, EventArgs e)
        {
            this._do_save();
        }

        private void frmDonors_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.contents_changed)
            {
                if (MessageBox.Show("Quit Without Saving?", "Donor Info Modified!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
            this.ctx.Dispose();
        }

        private void dgvDonors_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                this._mark_as_changed(true);
            }
        }

        private void btnDonorsClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvDonors_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            this._mark_as_changed(true);
        }
    }
}
