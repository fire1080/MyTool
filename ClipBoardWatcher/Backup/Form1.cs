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
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		public System.Threading.Timer t =null;
		public static RemoteClipboard m_remoteClipboard;
		public System.Windows.Forms.ToolTip tlpclipboarditem;
		public static Form1 objclipboard = null;
		public static frmnewprcdetails objremotemc = null;
		public static string selectedClipItemData = "";
		public static bool remoteClipData = false;
		public static string remotemcname = "";
		public static int selectedClipItemIndex = 0;
		public static bool formloaded = false;
		public static ClipBoardItems MyClipItems = null;
		public System.Windows.Forms.ContextMenu cxtmnuclip;
		private System.Windows.Forms.MenuItem menuItem1;
		public static string prevclipboardcontents = "";
		public static Hashtable clipboardcopytimings = null;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.ContextMenu cxttaskbar;
		private System.Windows.Forms.MenuItem menuItem18;
		private System.Windows.Forms.MenuItem menuItem19;
		private System.Windows.Forms.MenuItem menuItem20;
		private System.Windows.Forms.MenuItem menuItem21;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem2;
		public System.Windows.Forms.MenuItem menuItem10;
		public TcpChannel channel;
		public DelegateGetAndDisplayClipData m_DelegateGetAndDisplayClipData;
		public delegate void DelegateGetAndDisplayClipData();

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
			provider.TypeFilterLevel = TypeFilterLevel.Full;

			IDictionary props = new Hashtable();
			props["port"] = 4820;
			channel = new TcpChannel(props, null, provider);
			//TcpChannel channel = new TcpChannel(4820);
			ChannelServices.RegisterChannel(channel);
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteClipboard), "ClipShare",
				WellKnownObjectMode.Singleton);
			// set the callback for when an incoming clipboard event comes in
			RemoteClipboard.SetOnClipReceive(this, new ClipEventHandler(this.AddToClip));
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
				if (components != null) 
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.tlpclipboarditem = new System.Windows.Forms.ToolTip(this.components);
			this.cxtmnuclip = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.cxttaskbar = new System.Windows.Forms.ContextMenu();
			this.menuItem18 = new System.Windows.Forms.MenuItem();
			this.menuItem19 = new System.Windows.Forms.MenuItem();
			this.menuItem20 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.menuItem21 = new System.Windows.Forms.MenuItem();
			// 
			// tlpclipboarditem
			// 
			this.tlpclipboarditem.AutoPopDelay = 5000;
			this.tlpclipboarditem.InitialDelay = 200;
			this.tlpclipboarditem.ReshowDelay = 100;
			// 
			// cxtmnuclip
			// 
			this.cxtmnuclip.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.menuItem1,
																					   this.menuItem3,
																					   this.menuItem9,
																					   this.menuItem2});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "Copy To ClipBoard";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "Remove This ClipBoard Data";
			this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 2;
			this.menuItem9.Text = "Remove All ClipBoard items";
			this.menuItem9.Click += new System.EventHandler(this.menuItem9_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 3;
			this.menuItem2.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.menuItem2.Text = "Save Items to File";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.ContextMenu = this.cxttaskbar;
			this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
			this.notifyIcon1.Text = "ClipBoard Watcher is in Visible Mode";
			this.notifyIcon1.Visible = true;
			this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
			this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
			// 
			// cxttaskbar
			// 
			this.cxttaskbar.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.menuItem18,
																					   this.menuItem19,
																					   this.menuItem20,
																					   this.menuItem4,
																					   this.menuItem8,
																					   this.menuItem10,
																					   this.menuItem21});
			// 
			// menuItem18
			// 
			this.menuItem18.Index = 0;
			this.menuItem18.Text = "&Hide";
			this.menuItem18.Click += new System.EventHandler(this.menuItem18_Click);
			// 
			// menuItem19
			// 
			this.menuItem19.Checked = true;
			this.menuItem19.Index = 1;
			this.menuItem19.Text = "Show In Taskbar";
			this.menuItem19.Click += new System.EventHandler(this.menuItem19_Click);
			// 
			// menuItem20
			// 
			this.menuItem20.Checked = true;
			this.menuItem20.Index = 2;
			this.menuItem20.Text = "Show on Top";
			this.menuItem20.Click += new System.EventHandler(this.menuItem20_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 3;
			this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem5,
																					  this.menuItem6,
																					  this.menuItem7});
			this.menuItem4.Text = "Control Form Thickness";
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 0;
			this.menuItem5.Text = "++";
			this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 1;
			this.menuItem6.Text = "--";
			this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 2;
			this.menuItem7.Text = "Automatic";
			this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
			// 
			// menuItem8
			// 
			this.menuItem8.Checked = true;
			this.menuItem8.Index = 4;
			this.menuItem8.Text = "Show ControlBox";
			this.menuItem8.Click += new System.EventHandler(this.menuItem8_Click);
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 5;
			this.menuItem10.Text = "Connect To Remote Machine";
			this.menuItem10.Click += new System.EventHandler(this.menuItem10_Click);
			// 
			// menuItem21
			// 
			this.menuItem21.Index = 6;
			this.menuItem21.Text = "E&xit";
			this.menuItem21.Click += new System.EventHandler(this.menuItem21_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(168, 704);
			this.ControlBox = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Location = new System.Drawing.Point(850, 0);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.TopMost = true;
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.MouseHover += new System.EventHandler(this.Form1_MouseHover);
			this.Closed += new System.EventHandler(this.Form1_Closed);
			this.Activated += new System.EventHandler(this.Form1_Activated);
			this.MouseEnter += new System.EventHandler(this.Form1_MouseEnter);
			this.MouseLeave += new System.EventHandler(this.Form1_MouseLeave);
			this.Deactivate += new System.EventHandler(this.Form1_Deactivate);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			objclipboard = new Form1();
			objclipboard.Show();
			Application.Run();
		}

		private void Form1_Deactivate(object sender, System.EventArgs e)
		{
		if(!menuItem8.Checked)
		objclipboard.ControlBox = false;
		objclipboard.Opacity = 0.3;
		}
		private void UpdateFormOpacity(object temp)
		{
			try
			{
				if(menuItem7.Checked)
				{
					objclipboard.Opacity = objclipboard.Opacity-0.05;
					if(objclipboard.Opacity < 0.4)
					{
						t.Dispose();
						t=null;
					}
				}
			}
			catch{}
		}
		private void LoadClipBoardData(object temp)
		{
			try
			{
				objclipboard.Invoke(m_DelegateGetAndDisplayClipData);
			}
			catch
			{
			}

		}
		private void GetAndDisplayClipData()
		{
			IDataObject iData = Clipboard.GetDataObject();
			if(iData != null)
			{
				string currentclipboardcontents = "";
				try
				{
					 currentclipboardcontents = iData.GetData(DataFormats.StringFormat).ToString();
				}
				catch{}
				if(prevclipboardcontents != currentclipboardcontents)
				{
					IntPtr hwnd =APIFuncs.getforegroundWindow();
					Int32 pid = APIFuncs.GetWindowProcessID(hwnd);
					Process p = Process.GetProcessById(pid);
					prevclipboardcontents = currentclipboardcontents;
					MyClipItems.AddNewClipItem(currentclipboardcontents,p.MainModule.FileName);
				}
			}
		}
		private void Form1_Activated(object sender, System.EventArgs e)
		{
			if(menuItem8.Checked)
			objclipboard.ControlBox = true;
			objclipboard.Opacity = 1;
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			
			if(System.IO.Directory.Exists(@"c:\clipboard"))
			{
				System.IO.Directory.Delete(@"c:\clipboard",true);
			}
			System.IO.Directory.CreateDirectory(@"c:\clipboard");
			clipboardcopytimings = new Hashtable();
			formloaded = true;
			MyClipItems = new ClipBoardItems(this);
			m_DelegateGetAndDisplayClipData = new DelegateGetAndDisplayClipData(this.GetAndDisplayClipData);
			System.Threading.TimerCallback timerDelegate = 
				new System.Threading.TimerCallback(this.LoadClipBoardData);
			t = new System.Threading.Timer(timerDelegate,null,0,1000);
		}
		public void AddToClip(ArrayList theData)
		{

			try
			{
				DataObject dataObj = new DataObject();
				Form1.remoteClipData = true;
				Clipboard.SetDataObject(theData[0], true);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject(selectedClipItemData,true);
		}

		private void menuItem3_Click(object sender, System.EventArgs e)
		{
			MyClipItems.RemoveSelectedClipBoardItem(selectedClipItemIndex);
		
		}

		private void notifyIcon1_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItem21_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

		private void menuItem18_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(Form1.objclipboard.Visible == true)
				{
					Form1.objclipboard.Visible = false;
					menuItem18.Text = "Show";
					notifyIcon1.Text = "ClipBoard Watcher is in Invisible Mode";
				}
				else
				{
					Form1.objclipboard.Visible = true;
					menuItem18.Text = "Hide";
					notifyIcon1.Text = "ClipBoard Watcher is in Visible Mode";
				}
			}
			catch{}
				
		}

		private void menuItem19_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(menuItem19.Checked)
				{
					menuItem19.Checked = false;
					Form1.objclipboard.ShowInTaskbar = false;
				}
				else
				{
					menuItem19.Checked = true;
					Form1.objclipboard.ShowInTaskbar = true;
				}
			}
			catch{}
		}

		private void menuItem20_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(menuItem20.Checked)
				{
					menuItem20.Checked = false;
					Form1.objclipboard.TopMost = false;
				}
				else
				{
					menuItem20.Checked = true;
					Form1.objclipboard.TopMost = true;
				
				}
			}
			catch{}
		}

		private void menuItem5_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(Form1.objclipboard.Opacity < 1)
				Form1.objclipboard.Opacity += 0.1;
			}
			catch{}
		}

		private void menuItem6_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(Form1.objclipboard.Opacity > 0.1)
				Form1.objclipboard.Opacity -= 0.1;
			}
			catch{}

		}

		private void menuItem7_Click(object sender, System.EventArgs e)
		{
			if(!menuItem7.Checked)
			{
				System.Threading.TimerCallback timerDelegate = 
					new System.Threading.TimerCallback(this.UpdateFormOpacity);
				t = new System.Threading.Timer(timerDelegate,null,0,5000);
				menuItem7.Checked = true;
			}
			else
			{
				menuItem7.Checked = false;
				if(t != null)
				{
					t.Dispose();
					t = null;
				}
			}
		}

		private void notifyIcon1_DoubleClick(object sender, System.EventArgs e)
		{
			if(Form1.objclipboard.Visible)
			{
				Form1.objclipboard.Visible = false;
				notifyIcon1.Text = "ClipBoard Watcher is in Invisible Mode";
			}
			else
			{
				Form1.objclipboard.Visible = true;
				notifyIcon1.Text = "ClipBoard Watcher is in Visible Mode";
			}
		}

		private void menuItem8_Click(object sender, System.EventArgs e)
		{
			if(menuItem8.Checked)
			{
				menuItem8.Checked = false;
				Form1.objclipboard.ControlBox = false;
				Form1.objclipboard.Height +=  20;
			}
			else
			{
				menuItem8.Checked = true;
				Form1.objclipboard.ControlBox = true;
				Form1.objclipboard.Height -=  20;
			}
		}

		private void menuItem9_Click(object sender, System.EventArgs e)
		{
			MyClipItems.RemoveAllClipBoardItems();
		}

		private void Form1_Closed(object sender, System.EventArgs e)
		{
			try
			{
				Application.Exit();
				if(System.IO.Directory.Exists(@"c:\clipboard"))
				{
					System.IO.Directory.Delete(@"c:\clipboard",true);
				}
			}
			catch
			{
			Application.Exit();
			}
		}

		private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.Control && e.KeyCode == Keys.U)
			{
				try
				{
					if(Form1.objclipboard.Opacity < 1)
						Form1.objclipboard.Opacity += 0.1;
				}
				catch{}
			}
			else if(e.Control && e.KeyCode == Keys.D)
			{
				try
				{
					if(Form1.objclipboard.Opacity > 0.1)
						Form1.objclipboard.Opacity -= 0.1;
				}
				catch{}
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			Form1.objclipboard.Focus();
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			MyClipItems.SaveClipItems();
		}

		private void menuItem10_Click(object sender, System.EventArgs e)
		{
			if(menuItem10.Text.IndexOf("DisConnect From")==-1)    
			{
				objremotemc = new frmnewprcdetails();
				DialogResult d =  objremotemc.ShowDialog();
				if(d != DialogResult.Cancel)
				{
					InitRemoteObject();
					menuItem10.Text = "DisConnect From "+Form1.remotemcname;
				}
			}
			else
			{
				menuItem10.Text ="Connect To Remote Machine";
			}
		}
		private void InitRemoteObject()
		{
			try
			{
				string location = "tcp://" + Form1.remotemcname+ ":4820/ClipShare";
				m_remoteClipboard = (RemoteClipboard) Activator.GetObject(typeof(RemoteClipboard),location);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		private void Form1_MouseHover(object sender, System.EventArgs e)
		{
			
		}

		private void Form1_MouseLeave(object sender, System.EventArgs e)
		{

		}

		private void Form1_MouseEnter(object sender, System.EventArgs e)
		{
			objclipboard.Opacity = 1;
		}

	}

	public delegate void ClipEventHandler(ArrayList clipData);

	public class RemoteClipboard : MarshalByRefObject
	{
		private static ClipEventHandler m_OnClipReceive;
		private static Form m_receiverForm;
		public static void SetOnClipReceive(Form receiver, ClipEventHandler theCallback)
		{
			m_receiverForm = receiver;
			m_OnClipReceive = theCallback;
        }
		public void SendClipboard(ArrayList clipData)
		{
			object[] clipObjects = {clipData};
			m_receiverForm.Invoke(m_OnClipReceive, clipObjects);
		}
	}
}
