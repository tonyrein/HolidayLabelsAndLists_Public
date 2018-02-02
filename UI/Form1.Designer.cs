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
            this.grpboxView = new System.Windows.Forms.GroupBox();
            this.btnPrintLists = new System.Windows.Forms.Button();
            this.btnPrintLabels = new System.Windows.Forms.Button();
            this.btnViewLists = new System.Windows.Forms.Button();
            this.btnViewLabels = new System.Windows.Forms.Button();
            this.lblVPDonor = new System.Windows.Forms.Label();
            this.cmbViewPrintDonor = new System.Windows.Forms.ComboBox();
            this.grpboxGenerate = new System.Windows.Forms.GroupBox();
            this.grpboxImport = new System.Windows.Forms.GroupBox();
            this.btnImportHelp = new System.Windows.Forms.Button();
            this.grpboxSettings = new System.Windows.Forms.GroupBox();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.pnlMain.SuspendLayout();
            this.grpboxView.SuspendLayout();
            this.grpboxImport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.grpboxSettings);
            this.pnlMain.Controls.Add(this.grpboxView);
            this.pnlMain.Controls.Add(this.grpboxGenerate);
            this.pnlMain.Controls.Add(this.grpboxImport);
            this.pnlMain.Location = new System.Drawing.Point(52, 63);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(749, 383);
            this.pnlMain.TabIndex = 0;
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
            // 
            // btnViewLabels
            // 
            this.btnViewLabels.Location = new System.Drawing.Point(25, 73);
            this.btnViewLabels.Name = "btnViewLabels";
            this.btnViewLabels.Size = new System.Drawing.Size(95, 27);
            this.btnViewLabels.TabIndex = 2;
            this.btnViewLabels.Text = "View Labels";
            this.btnViewLabels.UseVisualStyleBackColor = true;
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
            this.cmbViewPrintDonor.TabIndex = 0;
            // 
            // grpboxGenerate
            // 
            this.grpboxGenerate.Location = new System.Drawing.Point(292, 37);
            this.grpboxGenerate.Name = "grpboxGenerate";
            this.grpboxGenerate.Size = new System.Drawing.Size(200, 100);
            this.grpboxGenerate.TabIndex = 1;
            this.grpboxGenerate.TabStop = false;
            this.grpboxGenerate.Text = "Generate Labels and Lists";
            // 
            // grpboxImport
            // 
            this.grpboxImport.Controls.Add(this.btnImportHelp);
            this.grpboxImport.Location = new System.Drawing.Point(49, 37);
            this.grpboxImport.Name = "grpboxImport";
            this.grpboxImport.Size = new System.Drawing.Size(200, 100);
            this.grpboxImport.TabIndex = 0;
            this.grpboxImport.TabStop = false;
            this.grpboxImport.Text = "Import VESTA Reports";
            // 
            // btnImportHelp
            // 
            this.btnImportHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportHelp.Location = new System.Drawing.Point(165, 57);
            this.btnImportHelp.Name = "btnImportHelp";
            this.btnImportHelp.Size = new System.Drawing.Size(29, 37);
            this.btnImportHelp.TabIndex = 0;
            this.btnImportHelp.Text = "?";
            this.btnImportHelp.UseVisualStyleBackColor = true;
            // 
            // grpboxSettings
            // 
            this.grpboxSettings.Location = new System.Drawing.Point(443, 227);
            this.grpboxSettings.Name = "grpboxSettings";
            this.grpboxSettings.Size = new System.Drawing.Size(200, 100);
            this.grpboxSettings.TabIndex = 3;
            this.grpboxSettings.TabStop = false;
            this.grpboxSettings.Text = "Settings";
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 497);
            this.Controls.Add(this.pnlMain);
            this.Name = "frmMain";
            this.Text = "Valley Labels and Lists";
            this.pnlMain.ResumeLayout(false);
            this.grpboxView.ResumeLayout(false);
            this.grpboxView.PerformLayout();
            this.grpboxImport.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.GroupBox grpboxView;
        private System.Windows.Forms.GroupBox grpboxGenerate;
        private System.Windows.Forms.GroupBox grpboxImport;
        private System.Windows.Forms.Button btnPrintLists;
        private System.Windows.Forms.Button btnPrintLabels;
        private System.Windows.Forms.Button btnViewLists;
        private System.Windows.Forms.Button btnViewLabels;
        private System.Windows.Forms.Label lblVPDonor;
        private System.Windows.Forms.ComboBox cmbViewPrintDonor;
        private System.Windows.Forms.Button btnImportHelp;
        private System.Windows.Forms.GroupBox grpboxSettings;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
    }
}

