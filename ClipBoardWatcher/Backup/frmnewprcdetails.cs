using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ClipBoardWatcher
{
	/// <summary>
	/// Summary description for frmnewprcdetails.
	/// </summary>
	public class frmnewprcdetails : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtapplnpath;
		private System.Windows.Forms.Button btnok;
		private System.Windows.Forms.Button btncancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmnewprcdetails()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
				/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtapplnpath = new System.Windows.Forms.TextBox();
			this.btnok = new System.Windows.Forms.Button();
			this.btncancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// txtapplnpath
			// 
			this.txtapplnpath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.txtapplnpath.Location = new System.Drawing.Point(80, 16);
			this.txtapplnpath.Name = "txtapplnpath";
			this.txtapplnpath.Size = new System.Drawing.Size(248, 20);
			this.txtapplnpath.TabIndex = 0;
			this.txtapplnpath.Text = "";
			// 
			// btnok
			// 
			this.btnok.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnok.Location = new System.Drawing.Point(80, 56);
			this.btnok.Name = "btnok";
			this.btnok.TabIndex = 1;
			this.btnok.Text = "&Ok";
			this.btnok.Click += new System.EventHandler(this.btnok_Click);
			// 
			// btncancel
			// 
			this.btncancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btncancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btncancel.Location = new System.Drawing.Point(184, 56);
			this.btncancel.Name = "btncancel";
			this.btncancel.TabIndex = 2;
			this.btncancel.Text = "&Cancel";
			this.btncancel.Click += new System.EventHandler(this.btncancel_Click);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(0, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "Connect To";
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.AddExtension = false;
			this.openFileDialog1.CheckFileExists = false;
			this.openFileDialog1.CheckPathExists = false;
			this.openFileDialog1.DereferenceLinks = false;
			this.openFileDialog1.RestoreDirectory = true;
			this.openFileDialog1.ValidateNames = false;
			// 
			// frmnewprcdetails
			// 
			this.AcceptButton = this.btnok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btncancel;
			this.ClientSize = new System.Drawing.Size(336, 96);
			this.ControlBox = false;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btncancel);
			this.Controls.Add(this.btnok);
			this.Controls.Add(this.txtapplnpath);
			this.Name = "frmnewprcdetails";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion

		private void btncancel_Click(object sender, System.EventArgs e)
		{
				Form1.remotemcname="";
    			Form1.objremotemc.Close();
			    this.DialogResult = DialogResult.Cancel;
		}

		private void btnok_Click(object sender, System.EventArgs e)
		{
			Form1.remotemcname = txtapplnpath.Text.Trim();
			this.DialogResult = DialogResult.OK;
			Form1.objremotemc.Close();
		}
	}
}
