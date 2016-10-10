using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace ClipBoardWatcher
{
	/// <summary>
	/// Summary description for ClipBoardItems.
	/// </summary>
	public class ClipBoardItems : System.Collections.CollectionBase
	{
		private readonly System.Windows.Forms.Form ParentForm;
		public ClipBoardItems()
		{
			
		}
		public ClipBoardItems(System.Windows.Forms.Form mainform)
		{
			ParentForm = mainform;
		}
		public System.Windows.Forms.Button this [int Index]
		{
			get
			{
				return (System.Windows.Forms.Button)this.List[Index];
			}
		}
		public void RemoveSelectedClipBoardItem(int clipitemindex)
		{
			if (this.Count > 0)
			{
				int index = Form1.objclipboard.Controls.GetChildIndex(this[clipitemindex],false);
				if(index>= 0)
				{
					ParentForm.Controls.Remove(this[index]);
					Form1.clipboardcopytimings.Remove(this[index].Name);
					this.List.RemoveAt(index);
					if(((index != 0) ||(index == 0 && this.Count>0)) && index != this.Count)
					{
						this[index+1].Top -= 20;
						for(int i=index+2;i<=this.Count-1;i++)
						{
							try
							{
								if(this[i] != null)
								{
									this[i].Top -= 20;
								}
							}
							catch(Exception ex)
							{
								System.Windows.Forms.MessageBox.Show(ex.Message);
							}
						}
					}
					else
					{
						try
						{
							if(System.IO.Directory.Exists(@"c:\clipboard"))
							{
								System.IO.Directory.Delete(@"c:\clipboard",true);
							}
						}
						catch{}
						System.IO.Directory.CreateDirectory(@"c:\clipboard");
					}
				}
			}
		}
		public  void SaveClipItems()
		{
			try
			{
				System.IO.File.Create(@"C:\clipboarddata.txt").Close();
				string clipboardcontents = "";
				int clipitemslength = this.List.Count;
				if(clipitemslength > 0)
				{
					for(int i=0;i<clipitemslength;i++)
					{
						clipboardcontents += this[i].Text+"\r\n"; 
					}
					System.IO.StreamWriter clipwriter = new System.IO.StreamWriter(@"C:\clipboarddata.txt",true);
					clipwriter.Write(clipboardcontents);
					clipwriter.Flush();
					clipwriter.Close();
				    System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("ClipBoard Contents are saved to "+@"C:\clipboarddata.txt..."+"Do you want to Browse that file?","ClipBoard Watcher",System.Windows.Forms.MessageBoxButtons.YesNo);					
					if(result == System.Windows.Forms.DialogResult.Yes)
					{
						System.Diagnostics.Process.Start("notepad.exe",@"C:\clipboarddata.txt");
					}
				}
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);

			}
		}
		public void RemoveAllClipBoardItems()
		{
			try
			{
				int clipitemslength = this.List.Count;
				for(int i=0;i<clipitemslength;i++)
				{
					ParentForm.Controls.Remove(this[i]);
				}
				Form1.clipboardcopytimings.Clear();
				this.List.Clear();
			}
			catch(Exception ex) 
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
			}
		}
		public void ClipBoardItemClick(Object sender, System.EventArgs e)
		{
			string clipdata = sender.ToString().Substring(sender.ToString().IndexOf("Text: ")+6);
			if(clipdata.Trim().Length>0)
			{
				 int index = Form1.objclipboard.Controls.GetChildIndex((System.Windows.Forms.Control)sender);
				 System.Windows.Forms.Button b =  (System.Windows.Forms.Button)Form1.objclipboard.Controls[index];
				if(Form1.clipboardcopytimings.Count > 0)
				{
					System.Collections.IDictionaryEnumerator  iclipdata = Form1.clipboardcopytimings.GetEnumerator();
					while(iclipdata.MoveNext())
					{
						if(iclipdata.Key.ToString().Trim() == b.Name.Trim())
						{
							System.Windows.Forms.MessageBox.Show(sender.ToString().Substring(sender.ToString().IndexOf("Text: ")+6)+" copied to clipboard at "+iclipdata.Value.ToString());
							break;
						}
					}
				}
				else
				{
					System.Windows.Forms.MessageBox.Show(sender.ToString().Substring(sender.ToString().IndexOf("Text: ")+6));
				}
			}
		}
		public System.Windows.Forms.Button AddNewClipItem(string clipdata,string processpath)
		{
			    string filename = DateTime.Now.ToLongTimeString().Replace(":","").Replace("AM","").Replace("PM","")+this.Count.ToString();	
			    System.Windows.Forms.Button newClipItem = new 
				System.Windows.Forms.Button();
				if(Form1.remotemcname != "" && Form1.objclipboard.menuItem10.Text.IndexOf("DisConnect From")!=-1)
				{
					ArrayList ClipBoardData = new ArrayList();
					IDataObject iData = Clipboard.GetDataObject();
					if(iData != null)
					{
						string currentclipboardcontents = iData.GetData(DataFormats.StringFormat).ToString();
						ClipBoardData.Add(currentclipboardcontents);
						if (ClipBoardData.Count > 0)
						{
							Cursor.Current = Cursors.WaitCursor;
							Form1.m_remoteClipboard.SendClipboard(ClipBoardData);
							Cursor.Current = Cursors.Default;
						}
					}
				}
			if(!Form1.remoteClipData)
			{
				ShellIcon.GetSmallIcon(processpath).ToBitmap().Save(@"c:\clipboard\"+filename+".ico");
				newClipItem.Image = System.Drawing.Image.FromFile(@"c:\clipboard\"+filename+".ico");
				newClipItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
				newClipItem.TextAlign  = System.Drawing.ContentAlignment.MiddleRight;
			}
			else
			{
				newClipItem.TextAlign  = System.Drawing.ContentAlignment.MiddleLeft;
			}
			    Form1.remoteClipData = false;    
			    newClipItem.Text = clipdata;
			    newClipItem.Name = "temp"+DateTime.Now.ToLongTimeString().Replace(":","").Replace("AM","").Replace("PM","");
				newClipItem.Width = ParentForm.Width - 10;
				this.List.Add(newClipItem);
				ParentForm.Controls.Add(newClipItem);
				newClipItem.ContextMenu = Form1.objclipboard.cxtmnuclip;
			    newClipItem.TextAlign  = System.Drawing.ContentAlignment.MiddleRight;
				newClipItem.Click += new System.EventHandler(ClipBoardItemClick);
				newClipItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.newClipItem_MouseDown);
				newClipItem.Top = Count * 20;
				newClipItem.MouseHover +=new EventHandler(this.newClipItem_MouseHover);
				newClipItem.Tag = this.Count;
				if(Form1.formloaded) 
				{
					Form1.formloaded = false;
					return newClipItem;
				}
				Form1.clipboardcopytimings.Add(newClipItem.Name,DateTime.Now.ToLongTimeString());
				return newClipItem;
		}


		private void newClipItem_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			 Form1.selectedClipItemData = sender.ToString().Substring(sender.ToString().IndexOf("Text: ")+6);
			 Form1.selectedClipItemIndex = Form1.objclipboard.Controls.GetChildIndex((System.Windows.Forms.Control)sender);
		}
		private void newClipItem_MouseHover(object sender, EventArgs e)
		{
			string clipdata = sender.ToString().Substring(sender.ToString().IndexOf("Text: ")+6);
			if(clipdata.Trim().Length>0)
			{
				Form1.objclipboard.tlpclipboarditem.SetToolTip((System.Windows.Forms.Control)sender,clipdata.Trim());
			}
			Form1.objclipboard.Opacity = 1;
		}
	}
}
