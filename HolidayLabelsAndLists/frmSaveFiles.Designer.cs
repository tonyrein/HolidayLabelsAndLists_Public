namespace HolidayLabelsAndLists
{
    partial class frmSaveFiles
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblDocType = new System.Windows.Forms.Label();
            this.lblYear = new System.Windows.Forms.Label();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.lblDestination = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lbxDocType = new System.Windows.Forms.ListBox();
            this.lbxYears = new System.Windows.Forms.ListBox();
            this.chkIncludeBackups = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(195, 239);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "button1";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(97, 239);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "button1";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // lblDocType
            // 
            this.lblDocType.AutoSize = true;
            this.lblDocType.Location = new System.Drawing.Point(50, 24);
            this.lblDocType.Name = "lblDocType";
            this.lblDocType.Size = new System.Drawing.Size(35, 13);
            this.lblDocType.TabIndex = 3;
            this.lblDocType.Text = "label1";
            // 
            // lblYear
            // 
            this.lblYear.AutoSize = true;
            this.lblYear.Location = new System.Drawing.Point(214, 24);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(35, 13);
            this.lblYear.TabIndex = 5;
            this.lblYear.Text = "label1";
            // 
            // txtDestination
            // 
            this.txtDestination.Location = new System.Drawing.Point(56, 197);
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(384, 20);
            this.txtDestination.TabIndex = 6;
            // 
            // lblDestination
            // 
            this.lblDestination.AutoSize = true;
            this.lblDestination.Location = new System.Drawing.Point(58, 178);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(35, 13);
            this.lblDestination.TabIndex = 7;
            this.lblDestination.Text = "label1";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(457, 197);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 8;
            this.btnBrowse.Text = "button1";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lbxDocType
            // 
            this.lbxDocType.FormattingEnabled = true;
            this.lbxDocType.Items.AddRange(new object[] {
            "All Types",
            "Bag Labels",
            "Gift Labels",
            "Donor and Master List",
            "Participant List",
            "Postcard Labels"});
            this.lbxDocType.Location = new System.Drawing.Point(53, 40);
            this.lbxDocType.Name = "lbxDocType";
            this.lbxDocType.Size = new System.Drawing.Size(119, 82);
            this.lbxDocType.TabIndex = 9;
            this.lbxDocType.SelectedIndexChanged += new System.EventHandler(this.lbxDocType_SelectedIndexChanged);
            // 
            // lbxYears
            // 
            this.lbxYears.FormattingEnabled = true;
            this.lbxYears.Location = new System.Drawing.Point(217, 40);
            this.lbxYears.Name = "lbxYears";
            this.lbxYears.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbxYears.Size = new System.Drawing.Size(120, 82);
            this.lbxYears.TabIndex = 10;
            // 
            // chkIncludeBackups
            // 
            this.chkIncludeBackups.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.chkIncludeBackups.AutoSize = true;
            this.chkIncludeBackups.Location = new System.Drawing.Point(382, 40);
            this.chkIncludeBackups.Name = "chkIncludeBackups";
            this.chkIncludeBackups.Size = new System.Drawing.Size(80, 17);
            this.chkIncludeBackups.TabIndex = 11;
            this.chkIncludeBackups.Text = "checkBox1";
            this.chkIncludeBackups.UseVisualStyleBackColor = true;
            // 
            // frmSaveFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 302);
            this.Controls.Add(this.chkIncludeBackups);
            this.Controls.Add(this.lbxYears);
            this.Controls.Add(this.lbxDocType);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.lblDestination);
            this.Controls.Add(this.txtDestination);
            this.Controls.Add(this.lblYear);
            this.Controls.Add(this.lblDocType);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Name = "frmSaveFiles";
            this.Text = "frmSaveFiles";
            this.Load += new System.EventHandler(this.frmSaveFiles_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblDocType;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.TextBox txtDestination;
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.ListBox lbxDocType;
        private System.Windows.Forms.ListBox lbxYears;
        private System.Windows.Forms.CheckBox chkIncludeBackups;
    }
}