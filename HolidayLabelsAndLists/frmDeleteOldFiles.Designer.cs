namespace HolidayLabelsAndLists
{
    partial class frmDeleteOldFiles
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lbxYearsToDelete = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.lbxYearsToDelete);
            this.groupBox1.Location = new System.Drawing.Point(192, 101);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(252, 228);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(157, 180);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "button1";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(17, 180);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "button1";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lbxYearsToDelete
            // 
            this.lbxYearsToDelete.FormattingEnabled = true;
            this.lbxYearsToDelete.Location = new System.Drawing.Point(51, 38);
            this.lbxYearsToDelete.Name = "lbxYearsToDelete";
            this.lbxYearsToDelete.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbxYearsToDelete.Size = new System.Drawing.Size(162, 95);
            this.lbxYearsToDelete.TabIndex = 0;
            this.lbxYearsToDelete.SelectedIndexChanged += new System.EventHandler(this.lbxYearsToDelete_SelectedIndexChanged);
            // 
            // frmDeleteOldFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 465);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmDeleteOldFiles";
            this.Text = "frmDeleteOldFiles";
            this.Activated += new System.EventHandler(this.frmDeleteOldFiles_Activated);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lbxYearsToDelete;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCancel;
    }
}