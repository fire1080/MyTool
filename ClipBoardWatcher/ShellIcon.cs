using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;


namespace ClipBoardWatcher
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class ShellIcon
	{
		[StructLayout(LayoutKind.Sequential)]
			public struct SHFILEINFO
		{
			public IntPtr hIcon;
			public IntPtr iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};

		class Win32
		{
			public const uint SHGFI_ICON = 0x100;
			public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
			public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

			[DllImport("shell32.dll")]
			public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi,uint cbSizeFileInfo, uint uFlags);
		}

		public ShellIcon()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static Icon GetSmallIcon(string fileName)
		{
			IntPtr hImgSmall; //the handle to the system image list
			SHFILEINFO shinfo = new SHFILEINFO();

			//Use this to get the small Icon
			hImgSmall = Win32.SHGetFileInfo(fileName, 0, ref shinfo,(uint)Marshal.SizeOf(shinfo),Win32.SHGFI_ICON| Win32.SHGFI_SMALLICON);

			//The icon is returned in the hIcon member of the shinfo struct
			return System.Drawing.Icon.FromHandle(shinfo.hIcon);        
		}

		public static Icon GetLargeIcon(string fileName)
		{
			IntPtr hImgLarge; //the handle to the system image list
			SHFILEINFO shinfo = new SHFILEINFO();

			//Use this to get the large Icon
			hImgLarge = Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo),Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);

			//The icon is returned in the hIcon member of the shinfo struct
			return System.Drawing.Icon.FromHandle(shinfo.hIcon);        
		}
	}
}
