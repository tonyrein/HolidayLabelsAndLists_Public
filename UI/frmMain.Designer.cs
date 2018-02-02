namespace UI
{
    partial class frmMain
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
            this.pnlMain = new System.Windows.Forms.Panel();
            this.btnManageDonors = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnProcess = new System.Windows.Forms.Button();
            this.grpboxSettings = new System.Windows.Forms.GroupBox();
            this.grpboxView = new System.Windows.Forms.GroupBox();
            this.btnPrintLists = new System.Windows.Forms.Button();
            this.btnPrintLabels = new System.Windows.Forms.Button();
            this.btnViewLists = new System.Windows.Forms.Button();
            this.btnViewLabels = new System.Windows.Forms.Button();
            this.lblVPDonor = new System.Windows.Forms.Label();
            this.cmbViewPrintDonor = new System.Windows.Forms.ComboBox();
            this.pnlMain.SuspendLayout();
            this.grpboxView.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.btnManageDonors);
            this.pnlMain.Controls.Add(this.btnExit);
            this.pnlMain.Controls.Add(this.btnProcess);
            this.pnlMain.Controls.Add(this.grpboxSettings);
            this.pnlMain.Controls.Add(this.grpboxView);
            this.pnlMain.Location = new System.Drawing.Point(52, 63);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(749, 383);
            this.pnlMain.TabIndex = 0;
            // 
            // btnManageDonors
            // 
            this.btnManageDonors.Location = new System.Drawing.Point(367, 277);
            this.btnManageDonors.Name = "btnManageDonors";
            this.btnManageDonors.Size = new System.Drawing.Size(104, 50);
            this.btnManageDonors.TabIndex = 6;
            this.btnManageDonors.Text = "Manage Donor List";
            this.btnManageDonors.UseVisualStyleBackColor = true;
            this.btnManageDonors.Click += new System.EventHandler(this.btnManageDonors_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(570, 310);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(102, 30);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(116, 60);
            this.btnProcess.TabIndex = 4;
            this.btnProcess.Text = "Add and Process VESTA Reports";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // grpboxSettings
            // 
            this.grpboxSettings.Location = new System.Drawing.Point(494, 179);
            this.grpboxSettings.Name = "grpboxSettings";
            this.grpboxSettings.Size = new System.Drawing.Size(200, 100);
            this.grpboxSettings.TabIndex = 3;
            this.grpboxSettings.TabStop = false;
            this.grpboxSettings.Text = "Settings";
            // 
            // grpboxView
            // 
            this.grpboxView.Controls.Add(this.btnPrintLists);
            this.grpboxView.Controls.Add(this.btnPrintLabels);
            this.grpboxView.Controls.Add(this.btnViewLists);
            this.grpboxView.Controls.Add(this.btnViewLabels);
            this.grpboxView.Controls.Add(this.lblVPDonor);
            this.grpboxView.Controls.Add(this.cmbViewPrintDonor);
            this.grpboxView.Location = new System.Drawing.Point(49, 179);
            this.grpboxView.Name = "grpboxView";
            this.grpboxView.Size = new System.Drawing.Size(250, 154);
            this.grpboxView.TabIndex = 2;
            this.grpboxView.TabStop = false;
            this.grpboxView.Text = "View or Print";
            // 
            // btnPrintLists
            // 
            this.btnPrintLists.Location = new System.Drawing.Point(149, 121);
            this.btnPrintLists.Name = "btnPrintLists";
            this.btnPrintLists.Size = new System.Drawing.Size(95, 27);
            this.btnPrintLists.TabIndex = 5;
            this.btnPrintLists.Text = "Print Lists";
            this.btnPrintLists.UseVisualStyleBackColor = true;
            // 
            // btnPrintLabels
            // 
            this.btnPrintLabels.Location = new System.Drawing.Point(25, 121);
            this.btnPrintLabels.Name = "btnPrintLabels";
            this.btnPrintLabels.Size = new System.Drawing.Size(95, 27);
            this.btnPrintLabels.TabIndex = 4;
            this.btnPrintLabels.Text = "Print Labels";
            this.btnPrintLabels.UseVisualStyleBackColor = true;
            // 
            // btnViewLists
            // 
            this.btnViewLists.Location = new System.Drawing.Point(149, 73);
            this.btnViewLists.Name = "btnViewLists";
            this.btnViewLists.Size = new System.Drawing.Size(95, 27);
            this.btnViewLists.TabIndex = 3;
            this.btnViewLists.Text = "View Lists";
            this.btnViewLists.UseVisualStyleBackColor = true;
            this.btnViewLists.Click += new System.EventHandler(this.btnViewLists_Click);
            // 
            // btnViewLabels
            // 
            this.btnViewLabels.Location = new System.Drawing.Point(25, 73);
            this.btnViewLabels.Name = "btnViewLabels";
            this.btnViewLabels.Size = new System.Drawing.Size(95, 27);
            this.btnViewLabels.TabIndex = 2;
            this.btnViewLabels.Text = "View Labels";
            this.btnViewLabels.UseVisualStyleBackColor = true;
            this.btnViewLabels.Click += new System.EventHandler(this.btnViewLabels_Click);
            // 
            // lblVPDonor
            // 
            this.lblVPDonor.AutoSize = true;
            this.lblVPDonor.Location = new System.Drawing.Point(35, 35);
            this.lblVPDonor.Name = "lblVPDonor";
            this.lblVPDonor.Size = new System.Drawing.Size(36, 13);
            this.lblVPDonor.TabIndex = 1;
            this.lblVPDonor.Text = "Donor";
            // 
            // cmbViewPrintDonor
            // 
            this.cmbViewPrintDonor.FormattingEnabled = true;
            this.cmbViewPrintDonor.Location = new System.Drawing.Point(101, 28);
            this.cmbViewPrintDonor.Name = "cmbViewPrintDonor";
            this.cmbViewPrintDonor.Size = new System.Drawing.Size(134, 21);
            this.cmbViewPrintDonor.Sorted = true;
            this.cmbViewPrintDonor.TabIndex = 0;
            //this.cmbViewPrintDonor.SelectedIndexChanged += new System.EventHandler(this.cmbViewPrintDonor_SelectedIndexChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 497);
            this.Controls.Add(this.pnlMain);
            this.Name = "frmMain";
            this.Text = "Valley Labels and Lists";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.pnlMain.ResumeLayout(false);
            this.grpboxView.ResumeLayout(false);
            this.grpboxView.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.GroupBox grpboxView;
        private System.Windows.Forms.Button btnPrintLists;
        private System.Windows.Forms.Button btnPrintLabels;
        private System.Windows.Forms.Button btnViewLists;
        private System.Windows.Forms.Button btnViewLabels;
        private System.Windows.Forms.Label lblVPDonor;
        private System.Windows.Forms.ComboBox cmbViewPrintDonor;
        private System.Windows.Forms.GroupBox grpboxSettings;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnManageDonors;
    }
}

