namespace UI
{
    partial class frmDonors
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDonorsSave = new System.Windows.Forms.Button();
            this.dgvDonors = new System.Windows.Forms.DataGridView();
            this.btnDonorsClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDonors)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDonorsSave
            // 
            this.btnDonorsSave.Location = new System.Drawing.Point(47, 298);
            this.btnDonorsSave.Name = "btnDonorsSave";
            this.btnDonorsSave.Size = new System.Drawing.Size(75, 44);
            this.btnDonorsSave.TabIndex = 1;
            this.btnDonorsSave.Text = "Save Changes";
            this.btnDonorsSave.UseVisualStyleBackColor = true;
            this.btnDonorsSave.Click += new System.EventHandler(this.btnDonorsSave_Click);
            // 
            // dgvDonors
            // 
            this.dgvDonors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDonors.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDonors.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvDonors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDonors.Location = new System.Drawing.Point(0, 32);
            this.dgvDonors.Name = "dgvDonors";
            this.dgvDonors.Size = new System.Drawing.Size(568, 260);
            this.dgvDonors.TabIndex = 1;
            this.dgvDonors.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDonors_CellValueChanged);
            this.dgvDonors.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvDonors_RowsRemoved);
            // 
            // btnDonorsClose
            // 
            this.btnDonorsClose.Location = new System.Drawing.Point(197, 299);
            this.btnDonorsClose.Name = "btnDonorsClose";
            this.btnDonorsClose.Size = new System.Drawing.Size(75, 43);
            this.btnDonorsClose.TabIndex = 2;
            this.btnDonorsClose.Text = "Exit";
            this.btnDonorsClose.UseVisualStyleBackColor = true;
            this.btnDonorsClose.Click += new System.EventHandler(this.btnDonorsClose_Click);
            // 
            // frmDonors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 433);
            this.Controls.Add(this.btnDonorsClose);
            this.Controls.Add(this.dgvDonors);
            this.Controls.Add(this.btnDonorsSave);
            this.Name = "frmDonors";
            this.Text = "Donor Names and Codes";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDonors_FormClosing);
            this.Load += new System.EventHandler(this.frmDonors_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDonors)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnDonorsSave;
        private System.Windows.Forms.DataGridView dgvDonors;
        private System.Windows.Forms.Button btnDonorsClose;
    }
}