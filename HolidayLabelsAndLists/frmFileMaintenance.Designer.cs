namespace HolidayLabelsAndLists
{
    partial class frmFileMaintenance
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDeleteBackups = new System.Windows.Forms.Button();
            this.btnDeleteOld = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnDone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(30, 45);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 81);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "button1";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnDeleteBackups
            // 
            this.btnDeleteBackups.Location = new System.Drawing.Point(166, 45);
            this.btnDeleteBackups.Name = "btnDeleteBackups";
            this.btnDeleteBackups.Size = new System.Drawing.Size(75, 81);
            this.btnDeleteBackups.TabIndex = 1;
            this.btnDeleteBackups.Text = "button2";
            this.btnDeleteBackups.UseVisualStyleBackColor = true;
            this.btnDeleteBackups.Click += new System.EventHandler(this.btnDeleteBackups_Click);
            // 
            // btnDeleteOld
            // 
            this.btnDeleteOld.Location = new System.Drawing.Point(30, 174);
            this.btnDeleteOld.Name = "btnDeleteOld";
            this.btnDeleteOld.Size = new System.Drawing.Size(75, 81);
            this.btnDeleteOld.TabIndex = 2;
            this.btnDeleteOld.Text = "button3";
            this.btnDeleteOld.UseVisualStyleBackColor = true;
            this.btnDeleteOld.Click += new System.EventHandler(this.btnDeleteOld_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(166, 174);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 81);
            this.btnHelp.TabIndex = 3;
            this.btnHelp.Text = "button4";
            this.btnHelp.UseVisualStyleBackColor = true;
            // 
            // btnDone
            // 
            this.btnDone.Location = new System.Drawing.Point(329, 115);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 57);
            this.btnDone.TabIndex = 4;
            this.btnDone.Text = "button1";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // frmFileMaintenance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 330);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnDeleteOld);
            this.Controls.Add(this.btnDeleteBackups);
            this.Controls.Add(this.btnSave);
            this.Name = "frmFileMaintenance";
            this.Text = "frmFileMaintenance";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDeleteBackups;
        private System.Windows.Forms.Button btnDeleteOld;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnDone;
    }
}