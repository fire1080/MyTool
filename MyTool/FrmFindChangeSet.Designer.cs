namespace FindChangeSet
{
    partial class FrmFindChangeSet
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("");
            this.btnSearch = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearchString = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Num = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Person = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Date = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Comments = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cboSearchPath = new System.Windows.Forms.ComboBox();
            this.chkKeepResults = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(259, 50);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(88, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Search String：";
            // 
            // txtSearchString
            // 
            this.txtSearchString.Location = new System.Drawing.Point(91, 52);
            this.txtSearchString.Name = "txtSearchString";
            this.txtSearchString.Size = new System.Drawing.Size(162, 20);
            this.txtSearchString.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Search Path: ";
            // 
            // listView1
            // 
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Num,
            this.Person,
            this.Date,
            this.Comments});
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.listView1.Location = new System.Drawing.Point(12, 79);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(435, 200);
            this.listView1.TabIndex = 8;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // Num
            // 
            this.Num.Text = "Num";
            this.Num.Width = 67;
            // 
            // Person
            // 
            this.Person.Text = "Person";
            this.Person.Width = 102;
            // 
            // Date
            // 
            this.Date.Text = "Date";
            this.Date.Width = 70;
            // 
            // Comments
            // 
            this.Comments.Text = "Comments";
            this.Comments.Width = 724;
            // 
            // cboSearchPath
            // 
            this.cboSearchPath.Items.AddRange(new object[] {
            "$/ibs_main/",
            "$/ibs_main/Build",
            "$/ibs_main/paymedia",
            "$/ibs_main/paymedia/ApplicationServices",
            "$/Professional_Services/",
            "$/Professional_Services/Clients/GIL",
            "$/Professional_Services/Clients/GIL/BSL/Main",
            "$/Professional_Services/Clients/DishHome/VoucherManagement/Main",
            "$/Professional_Services/Clients/MCA/DisconnectionManagement/Main"});
            this.cboSearchPath.Location = new System.Drawing.Point(91, 20);
            this.cboSearchPath.Name = "cboSearchPath";
            this.cboSearchPath.Size = new System.Drawing.Size(351, 21);
            this.cboSearchPath.TabIndex = 3;
            // 
            // chkKeepResults
            // 
            this.chkKeepResults.AutoSize = true;
            this.chkKeepResults.Checked = true;
            this.chkKeepResults.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkKeepResults.Location = new System.Drawing.Point(353, 54);
            this.chkKeepResults.Name = "chkKeepResults";
            this.chkKeepResults.Size = new System.Drawing.Size(89, 17);
            this.chkKeepResults.TabIndex = 10;
            this.chkKeepResults.Text = "Keep Results";
            this.chkKeepResults.UseVisualStyleBackColor = true;
            // 
            // FrmFindChangeSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 285);
            this.Controls.Add(this.chkKeepResults);
            this.Controls.Add(this.cboSearchPath);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSearchString);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSearch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "FrmFindChangeSet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Find Change Set by comments";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmFindChangeSet_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmFindChangeSet_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FrmFindChangeSet_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSearchString;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader Num;
        private System.Windows.Forms.ColumnHeader Person;
        private System.Windows.Forms.ColumnHeader Comments;
        private System.Windows.Forms.ColumnHeader Date;
        private System.Windows.Forms.ComboBox cboSearchPath;
        private System.Windows.Forms.CheckBox chkKeepResults;
    }
}

