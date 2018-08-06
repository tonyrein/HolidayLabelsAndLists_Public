namespace HolidayLabelsAndLists
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
            this.components = new System.ComponentModel.Container();
            this.lvAvailableFiles = new System.Windows.Forms.ListView();
            this.clmFileNames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblAvailableFiles = new System.Windows.Forms.Label();
            this.cmbTypeToView = new System.Windows.Forms.ComboBox();
            this.lblTypeToView = new System.Windows.Forms.Label();
            this.lblYear = new System.Windows.Forms.Label();
            this.cmbYear = new System.Windows.Forms.ComboBox();
            this.lblDonor = new System.Windows.Forms.Label();
            this.cmbDonor = new System.Windows.Forms.ComboBox();
            this.chbxIncludeBackups = new System.Windows.Forms.CheckBox();
            this.btnAddVestaReports = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDeleteOldFiles = new System.Windows.Forms.Button();
            this.btnCreateOutput = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvAvailableFiles
            // 
            this.lvAvailableFiles.CausesValidation = false;
            this.lvAvailableFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmFileNames});
            this.lvAvailableFiles.GridLines = true;
            this.lvAvailableFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvAvailableFiles.Location = new System.Drawing.Point(41, 66);
            this.lvAvailableFiles.Margin = new System.Windows.Forms.Padding(4);
            this.lvAvailableFiles.MultiSelect = false;
            this.lvAvailableFiles.Name = "lvAvailableFiles";
            this.lvAvailableFiles.ShowItemToolTips = true;
            this.lvAvailableFiles.Size = new System.Drawing.Size(541, 411);
            this.lvAvailableFiles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvAvailableFiles.TabIndex = 1;
            this.lvAvailableFiles.UseCompatibleStateImageBehavior = false;
            this.lvAvailableFiles.View = System.Windows.Forms.View.Details;
            this.lvAvailableFiles.DoubleClick += new System.EventHandler(this.lvAvailableFiles_DoubleClick);
            // 
            // clmFileNames
            // 
            this.clmFileNames.Text = "Available Label and List Files";
            this.clmFileNames.Width = 150;
            // 
            // lblAvailableFiles
            // 
            this.lblAvailableFiles.AutoSize = true;
            this.lblAvailableFiles.Location = new System.Drawing.Point(41, 43);
            this.lblAvailableFiles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAvailableFiles.Name = "lblAvailableFiles";
            this.lblAvailableFiles.Size = new System.Drawing.Size(191, 17);
            this.lblAvailableFiles.TabIndex = 5;
            this.lblAvailableFiles.Text = "Available Label and List Files";
            // 
            // cmbTypeToView
            // 
            this.cmbTypeToView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTypeToView.FormattingEnabled = true;
            this.cmbTypeToView.Location = new System.Drawing.Point(637, 89);
            this.cmbTypeToView.Margin = new System.Windows.Forms.Padding(4);
            this.cmbTypeToView.Name = "cmbTypeToView";
            this.cmbTypeToView.Size = new System.Drawing.Size(160, 24);
            this.cmbTypeToView.TabIndex = 8;
            this.toolTip1.SetToolTip(this.cmbTypeToView, "Reduce clutter by selecting which files to include in available files list.");
            this.cmbTypeToView.SelectedIndexChanged += new System.EventHandler(this.cmbTypeToView_SelectedIndexChanged);
            // 
            // lblTypeToView
            // 
            this.lblTypeToView.AutoSize = true;
            this.lblTypeToView.Location = new System.Drawing.Point(633, 69);
            this.lblTypeToView.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTypeToView.Name = "lblTypeToView";
            this.lblTypeToView.Size = new System.Drawing.Size(157, 17);
            this.lblTypeToView.TabIndex = 9;
            this.lblTypeToView.Text = "Document Type to View";
            // 
            // lblYear
            // 
            this.lblYear.AutoSize = true;
            this.lblYear.Location = new System.Drawing.Point(633, 148);
            this.lblYear.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(38, 17);
            this.lblYear.TabIndex = 10;
            this.lblYear.Text = "Year";
            // 
            // cmbYear
            // 
            this.cmbYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYear.FormattingEnabled = true;
            this.cmbYear.Location = new System.Drawing.Point(637, 167);
            this.cmbYear.Margin = new System.Windows.Forms.Padding(4);
            this.cmbYear.Name = "cmbYear";
            this.cmbYear.Size = new System.Drawing.Size(107, 24);
            this.cmbYear.TabIndex = 11;
            this.toolTip1.SetToolTip(this.cmbYear, "Reduce clutter by selecting which files to include in available files list.");
            this.cmbYear.SelectedIndexChanged += new System.EventHandler(this.cmbYear_SelectedIndexChanged);
            // 
            // lblDonor
            // 
            this.lblDonor.AutoSize = true;
            this.lblDonor.Location = new System.Drawing.Point(633, 228);
            this.lblDonor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDonor.Name = "lblDonor";
            this.lblDonor.Size = new System.Drawing.Size(47, 17);
            this.lblDonor.TabIndex = 12;
            this.lblDonor.Text = "Donor";
            // 
            // cmbDonor
            // 
            this.cmbDonor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDonor.FormattingEnabled = true;
            this.cmbDonor.Location = new System.Drawing.Point(637, 247);
            this.cmbDonor.Margin = new System.Windows.Forms.Padding(4);
            this.cmbDonor.Name = "cmbDonor";
            this.cmbDonor.Size = new System.Drawing.Size(288, 24);
            this.cmbDonor.TabIndex = 13;
            this.toolTip1.SetToolTip(this.cmbDonor, "Reduce clutter by selecting which files to include in available files list.");
            this.cmbDonor.SelectedIndexChanged += new System.EventHandler(this.cmbDonor_SelectedIndexChanged);
            // 
            // chbxIncludeBackups
            // 
            this.chbxIncludeBackups.AutoSize = true;
            this.chbxIncludeBackups.Location = new System.Drawing.Point(637, 310);
            this.chbxIncludeBackups.Margin = new System.Windows.Forms.Padding(4);
            this.chbxIncludeBackups.Name = "chbxIncludeBackups";
            this.chbxIncludeBackups.Size = new System.Drawing.Size(156, 21);
            this.chbxIncludeBackups.TabIndex = 14;
            this.chbxIncludeBackups.Text = "Include Backup Files";
            this.chbxIncludeBackups.UseVisualStyleBackColor = true;
            this.chbxIncludeBackups.CheckedChanged += new System.EventHandler(this.chbxIncludeBackups_CheckedChanged);
            // 
            // btnAddVestaReports
            // 
            this.btnAddVestaReports.Location = new System.Drawing.Point(1032, 68);
            this.btnAddVestaReports.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddVestaReports.Name = "btnAddVestaReports";
            this.btnAddVestaReports.Size = new System.Drawing.Size(113, 64);
            this.btnAddVestaReports.TabIndex = 15;
            this.btnAddVestaReports.Text = "Process VESTA Reports";
            this.toolTip1.SetToolTip(this.btnAddVestaReports, "Select already-downloaded VESTA reports\r\nand generate label and list files from t" +
        "hose\r\nreports.");
            this.btnAddVestaReports.UseVisualStyleBackColor = true;
            this.btnAddVestaReports.Click += new System.EventHandler(this.btnAddVestaReports_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(812, 413);
            this.btnHelp.Margin = new System.Windows.Forms.Padding(4);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(113, 64);
            this.btnHelp.TabIndex = 18;
            this.btnHelp.Text = "Help";
            this.toolTip1.SetToolTip(this.btnHelp, "Instructions for using this program");
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(1032, 413);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(113, 64);
            this.btnClose.TabIndex = 19;
            this.btnClose.Text = "Exit";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDeleteOldFiles
            // 
            this.btnDeleteOldFiles.Location = new System.Drawing.Point(1032, 272);
            this.btnDeleteOldFiles.Name = "btnDeleteOldFiles";
            this.btnDeleteOldFiles.Size = new System.Drawing.Size(113, 69);
            this.btnDeleteOldFiles.TabIndex = 20;
            this.btnDeleteOldFiles.Text = "Delete Old Files";
            this.btnDeleteOldFiles.UseVisualStyleBackColor = true;
            this.btnDeleteOldFiles.Click += new System.EventHandler(this.btnDeleteOldFiles_Click);
            // 
            // btnCreateOutput
            // 
            this.btnCreateOutput.Location = new System.Drawing.Point(1032, 167);
            this.btnCreateOutput.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateOutput.Name = "btnCreateOutput";
            this.btnCreateOutput.Size = new System.Drawing.Size(113, 64);
            this.btnCreateOutput.TabIndex = 21;
            this.btnCreateOutput.Text = "Create Labels and Lists";
            this.toolTip1.SetToolTip(this.btnCreateOutput, "Select already-downloaded VESTA reports\r\nand generate label and list files from t" +
        "hose\r\nreports.");
            this.btnCreateOutput.UseVisualStyleBackColor = true;
            this.btnCreateOutput.Click += new System.EventHandler(this.btnCreateOutput_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1166, 604);
            this.Controls.Add(this.btnCreateOutput);
            this.Controls.Add(this.btnDeleteOldFiles);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnAddVestaReports);
            this.Controls.Add(this.chbxIncludeBackups);
            this.Controls.Add(this.cmbDonor);
            this.Controls.Add(this.lblDonor);
            this.Controls.Add(this.cmbYear);
            this.Controls.Add(this.lblYear);
            this.Controls.Add(this.lblTypeToView);
            this.Controls.Add(this.cmbTypeToView);
            this.Controls.Add(this.lblAvailableFiles);
            this.Controls.Add(this.lvAvailableFiles);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmMain";
            this.Text = "Holiday Labels and Lists";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvAvailableFiles;
        private System.Windows.Forms.Label lblAvailableFiles;
        private System.Windows.Forms.ComboBox cmbTypeToView;
        private System.Windows.Forms.Label lblTypeToView;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.ComboBox cmbYear;
        private System.Windows.Forms.Label lblDonor;
        private System.Windows.Forms.ComboBox cmbDonor;
        private System.Windows.Forms.ColumnHeader clmFileNames;
        private System.Windows.Forms.CheckBox chbxIncludeBackups;
        private System.Windows.Forms.Button btnAddVestaReports;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDeleteOldFiles;
        private System.Windows.Forms.Button btnCreateOutput;
    }
}

