namespace UI
{
    partial class frmProcessReports
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
            this.btnProcessHelp = new System.Windows.Forms.Button();
            this.chklstboxPendingReports = new System.Windows.Forms.CheckedListBox();
            this.lblPendingReports = new System.Windows.Forms.Label();
            this.lblCompletedReports = new System.Windows.Forms.Label();
            this.chklistboxProcessedReports = new System.Windows.Forms.CheckedListBox();
            this.btnPRDeleteSelectedDone = new System.Windows.Forms.Button();
            this.btnPRMarkForReprocessing = new System.Windows.Forms.Button();
            this.btnPRClose = new System.Windows.Forms.Button();
            this.grpPending = new System.Windows.Forms.GroupBox();
            this.grpPendingButtons = new System.Windows.Forms.GroupBox();
            this.btnPRImport = new System.Windows.Forms.Button();
            this.btnPRDeleteSelectedPending = new System.Windows.Forms.Button();
            this.btnPRProcessSelectedPending = new System.Windows.Forms.Button();
            this.btnPRProcessAllPending = new System.Windows.Forms.Button();
            this.grpCompleted = new System.Windows.Forms.GroupBox();
            this.grpCompletedButtons = new System.Windows.Forms.GroupBox();
            this.grpPending.SuspendLayout();
            this.grpPendingButtons.SuspendLayout();
            this.grpCompleted.SuspendLayout();
            this.grpCompletedButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnProcessHelp
            // 
            this.btnProcessHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProcessHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcessHelp.Location = new System.Drawing.Point(466, 543);
            this.btnProcessHelp.Name = "btnProcessHelp";
            this.btnProcessHelp.Size = new System.Drawing.Size(112, 39);
            this.btnProcessHelp.TabIndex = 1;
            this.btnProcessHelp.Text = "Help";
            this.btnProcessHelp.UseVisualStyleBackColor = true;
            this.btnProcessHelp.Click += new System.EventHandler(this.btnProcessHelp_Click);
            // 
            // chklstboxPendingReports
            // 
            this.chklstboxPendingReports.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chklstboxPendingReports.CheckOnClick = true;
            this.chklstboxPendingReports.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chklstboxPendingReports.FormattingEnabled = true;
            this.chklstboxPendingReports.HorizontalScrollbar = true;
            this.chklstboxPendingReports.Location = new System.Drawing.Point(21, 34);
            this.chklstboxPendingReports.Name = "chklstboxPendingReports";
            this.chklstboxPendingReports.Size = new System.Drawing.Size(350, 289);
            this.chklstboxPendingReports.Sorted = true;
            this.chklstboxPendingReports.TabIndex = 2;
            this.chklstboxPendingReports.ThreeDCheckBoxes = true;
            this.chklstboxPendingReports.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.reportListManager_ItemCheck);
            this.chklstboxPendingReports.Resize += new System.EventHandler(this.chklstboxPendingReports_Resize);
            // 
            // lblPendingReports
            // 
            this.lblPendingReports.AutoSize = true;
            this.lblPendingReports.Location = new System.Drawing.Point(18, 16);
            this.lblPendingReports.Name = "lblPendingReports";
            this.lblPendingReports.Size = new System.Drawing.Size(86, 13);
            this.lblPendingReports.TabIndex = 4;
            this.lblPendingReports.Text = "Pending Reports";
            // 
            // lblCompletedReports
            // 
            this.lblCompletedReports.AutoSize = true;
            this.lblCompletedReports.Location = new System.Drawing.Point(41, 16);
            this.lblCompletedReports.Name = "lblCompletedReports";
            this.lblCompletedReports.Size = new System.Drawing.Size(97, 13);
            this.lblCompletedReports.TabIndex = 5;
            this.lblCompletedReports.Text = "Completed Reports";
            // 
            // chklistboxProcessedReports
            // 
            this.chklistboxProcessedReports.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chklistboxProcessedReports.CheckOnClick = true;
            this.chklistboxProcessedReports.FormattingEnabled = true;
            this.chklistboxProcessedReports.Location = new System.Drawing.Point(44, 34);
            this.chklistboxProcessedReports.Name = "chklistboxProcessedReports";
            this.chklistboxProcessedReports.Size = new System.Drawing.Size(335, 289);
            this.chklistboxProcessedReports.Sorted = true;
            this.chklistboxProcessedReports.TabIndex = 3;
            this.chklistboxProcessedReports.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.reportListManager_ItemCheck);
            this.chklistboxProcessedReports.Resize += new System.EventHandler(this.chklistboxProcessedReports_Resize);
            // 
            // btnPRDeleteSelectedDone
            // 
            this.btnPRDeleteSelectedDone.Location = new System.Drawing.Point(41, 19);
            this.btnPRDeleteSelectedDone.Name = "btnPRDeleteSelectedDone";
            this.btnPRDeleteSelectedDone.Size = new System.Drawing.Size(109, 53);
            this.btnPRDeleteSelectedDone.TabIndex = 10;
            this.btnPRDeleteSelectedDone.Text = "Delete Selected Reports";
            this.btnPRDeleteSelectedDone.UseVisualStyleBackColor = true;
            this.btnPRDeleteSelectedDone.Click += new System.EventHandler(this.btnPRDeleteSelectedDone_Click);
            // 
            // btnPRMarkForReprocessing
            // 
            this.btnPRMarkForReprocessing.Location = new System.Drawing.Point(163, 19);
            this.btnPRMarkForReprocessing.Name = "btnPRMarkForReprocessing";
            this.btnPRMarkForReprocessing.Size = new System.Drawing.Size(109, 53);
            this.btnPRMarkForReprocessing.TabIndex = 11;
            this.btnPRMarkForReprocessing.Text = "Mark Selected Reports for Reprocessing";
            this.btnPRMarkForReprocessing.UseVisualStyleBackColor = true;
            this.btnPRMarkForReprocessing.Click += new System.EventHandler(this.btnPRMarkForReprocessing_Click);
            // 
            // btnPRClose
            // 
            this.btnPRClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPRClose.Location = new System.Drawing.Point(296, 543);
            this.btnPRClose.Name = "btnPRClose";
            this.btnPRClose.Size = new System.Drawing.Size(112, 39);
            this.btnPRClose.TabIndex = 13;
            this.btnPRClose.Text = "Back to Main Screen";
            this.btnPRClose.UseVisualStyleBackColor = true;
            this.btnPRClose.Click += new System.EventHandler(this.button5_Click);
            // 
            // grpPending
            // 
            this.grpPending.Controls.Add(this.grpPendingButtons);
            this.grpPending.Controls.Add(this.lblPendingReports);
            this.grpPending.Controls.Add(this.chklstboxPendingReports);
            this.grpPending.Location = new System.Drawing.Point(27, 31);
            this.grpPending.Name = "grpPending";
            this.grpPending.Size = new System.Drawing.Size(400, 471);
            this.grpPending.TabIndex = 14;
            this.grpPending.TabStop = false;
            // 
            // grpPendingButtons
            // 
            this.grpPendingButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPendingButtons.Controls.Add(this.btnPRImport);
            this.grpPendingButtons.Controls.Add(this.btnPRDeleteSelectedPending);
            this.grpPendingButtons.Controls.Add(this.btnPRProcessSelectedPending);
            this.grpPendingButtons.Controls.Add(this.btnPRProcessAllPending);
            this.grpPendingButtons.Location = new System.Drawing.Point(50, 325);
            this.grpPendingButtons.MaximumSize = new System.Drawing.Size(285, 140);
            this.grpPendingButtons.MinimumSize = new System.Drawing.Size(285, 140);
            this.grpPendingButtons.Name = "grpPendingButtons";
            this.grpPendingButtons.Size = new System.Drawing.Size(285, 140);
            this.grpPendingButtons.TabIndex = 16;
            this.grpPendingButtons.TabStop = false;
            // 
            // btnPRImport
            // 
            this.btnPRImport.Location = new System.Drawing.Point(18, 74);
            this.btnPRImport.Name = "btnPRImport";
            this.btnPRImport.Size = new System.Drawing.Size(109, 53);
            this.btnPRImport.TabIndex = 3;
            this.btnPRImport.Text = "Import";
            this.btnPRImport.UseVisualStyleBackColor = true;
            this.btnPRImport.Click += new System.EventHandler(this.btnPRImport_Click);
            // 
            // btnPRDeleteSelectedPending
            // 
            this.btnPRDeleteSelectedPending.Location = new System.Drawing.Point(151, 74);
            this.btnPRDeleteSelectedPending.Name = "btnPRDeleteSelectedPending";
            this.btnPRDeleteSelectedPending.Size = new System.Drawing.Size(109, 53);
            this.btnPRDeleteSelectedPending.TabIndex = 2;
            this.btnPRDeleteSelectedPending.Text = "Delete Selected";
            this.btnPRDeleteSelectedPending.UseVisualStyleBackColor = true;
            this.btnPRDeleteSelectedPending.Click += new System.EventHandler(this.btnPRDeleteSelectedPending_Click);
            // 
            // btnPRProcessSelectedPending
            // 
            this.btnPRProcessSelectedPending.Location = new System.Drawing.Point(151, 9);
            this.btnPRProcessSelectedPending.Name = "btnPRProcessSelectedPending";
            this.btnPRProcessSelectedPending.Size = new System.Drawing.Size(109, 53);
            this.btnPRProcessSelectedPending.TabIndex = 1;
            this.btnPRProcessSelectedPending.Text = "Process Selected";
            this.btnPRProcessSelectedPending.UseVisualStyleBackColor = true;
            this.btnPRProcessSelectedPending.Click += new System.EventHandler(this.btnPRProcessSelectedPending_Click);
            // 
            // btnPRProcessAllPending
            // 
            this.btnPRProcessAllPending.Location = new System.Drawing.Point(18, 9);
            this.btnPRProcessAllPending.Name = "btnPRProcessAllPending";
            this.btnPRProcessAllPending.Size = new System.Drawing.Size(109, 53);
            this.btnPRProcessAllPending.TabIndex = 0;
            this.btnPRProcessAllPending.Text = "Process All";
            this.btnPRProcessAllPending.UseVisualStyleBackColor = true;
            this.btnPRProcessAllPending.Click += new System.EventHandler(this.btnPRProcessAllPending_Click);
            // 
            // grpCompleted
            // 
            this.grpCompleted.Controls.Add(this.grpCompletedButtons);
            this.grpCompleted.Controls.Add(this.chklistboxProcessedReports);
            this.grpCompleted.Controls.Add(this.lblCompletedReports);
            this.grpCompleted.Location = new System.Drawing.Point(466, 31);
            this.grpCompleted.Name = "grpCompleted";
            this.grpCompleted.Size = new System.Drawing.Size(400, 471);
            this.grpCompleted.TabIndex = 15;
            this.grpCompleted.TabStop = false;
            // 
            // grpCompletedButtons
            // 
            this.grpCompletedButtons.Controls.Add(this.btnPRDeleteSelectedDone);
            this.grpCompletedButtons.Controls.Add(this.btnPRMarkForReprocessing);
            this.grpCompletedButtons.Location = new System.Drawing.Point(68, 350);
            this.grpCompletedButtons.Name = "grpCompletedButtons";
            this.grpCompletedButtons.Size = new System.Drawing.Size(278, 100);
            this.grpCompletedButtons.TabIndex = 12;
            this.grpCompletedButtons.TabStop = false;
            // 
            // frmProcessReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 594);
            this.Controls.Add(this.btnPRClose);
            this.Controls.Add(this.btnProcessHelp);
            this.Controls.Add(this.grpCompleted);
            this.Controls.Add(this.grpPending);
            this.HelpButton = true;
            this.MinimumSize = new System.Drawing.Size(950, 620);
            this.Name = "frmProcessReports";
            this.Text = "Process VESTA Reports";
            this.Load += new System.EventHandler(this.frmProcessReports_Load);
            this.Resize += new System.EventHandler(this.frmProcessReports_Resize);
            this.grpPending.ResumeLayout(false);
            this.grpPending.PerformLayout();
            this.grpPendingButtons.ResumeLayout(false);
            this.grpCompleted.ResumeLayout(false);
            this.grpCompleted.PerformLayout();
            this.grpCompletedButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnProcessHelp;
        private System.Windows.Forms.CheckedListBox chklstboxPendingReports;
        private System.Windows.Forms.Label lblPendingReports;
        private System.Windows.Forms.Label lblCompletedReports;
        private System.Windows.Forms.CheckedListBox chklistboxProcessedReports;
        private System.Windows.Forms.Button btnPRDeleteSelectedDone;
        private System.Windows.Forms.Button btnPRMarkForReprocessing;
        private System.Windows.Forms.Button btnPRClose;
        private System.Windows.Forms.GroupBox grpPending;
        private System.Windows.Forms.GroupBox grpCompleted;
        private System.Windows.Forms.GroupBox grpPendingButtons;
        private System.Windows.Forms.Button btnPRImport;
        private System.Windows.Forms.Button btnPRDeleteSelectedPending;
        private System.Windows.Forms.Button btnPRProcessSelectedPending;
        private System.Windows.Forms.Button btnPRProcessAllPending;
        private System.Windows.Forms.GroupBox grpCompletedButtons;
    }
}