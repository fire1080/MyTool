using System.Windows.Forms;

namespace clipboard_helper
{
    partial class FrmMyClipboard
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
            this.lstHistory = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lstHistory
            // 
            this.lstHistory.BackColor = System.Drawing.Color.White;
            this.lstHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstHistory.HorizontalScrollbar = true;
            this.lstHistory.Location = new System.Drawing.Point(0, 0);
            this.lstHistory.Name = "lstHistory";
            this.lstHistory.Size = new System.Drawing.Size(252, 161);
            this.lstHistory.TabIndex = 0;
            this.lstHistory.SelectedValueChanged += new System.EventHandler(this.lstHistory_SelectedValueChanged);
            // 
            // FrmMyClipboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(252, 161);
            this.Controls.Add(this.lstHistory);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "FrmMyClipboard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Clipboard Little Helper";
            this.TopMost = true;
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FrmMyClipboard_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstHistory;
        //private System.Windows.Forms.ToolTip toolTip1;

    }
}

