using CefSharp;
using CefSharp.SchemeHandler;
using CefSharp.WinForms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CsGraphicsHook.Nutted;

namespace CsGraphicsHook
{
	class Program
	{
		static void Main(string[] args)
		{
			// spawn window to draw in
			IntPtr progman = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Progman", null);
			if (progman == IntPtr.Zero)
				return;
			SendMessageTimeout(progman, 0x052C, UIntPtr.Zero, IntPtr.Zero, 0, 1000, out UIntPtr a);

			// find created window
			IntPtr workerhwnd = IntPtr.Zero;
			IntPtr hwnd = IntPtr.Zero;
			while ((hwnd = FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", "")) != IntPtr.Zero)
				workerhwnd = hwnd;

			if (workerhwnd != IntPtr.Zero)
			{
				Console.WriteLine(workerhwnd);

				GetClientRect(GetDesktopWindow(), out RECT deskRect);

				InitSettings();

				// form will be put into the window
				Form form = new Form();
				form.Load += (sender, e) => {

					form.FormBorderStyle = FormBorderStyle.None;
					form.WindowState = FormWindowState.Normal;
					form.Bounds = Screen.PrimaryScreen.Bounds;

					var browser = new ChromiumWebBrowser("thighhigh://index.html");
					browser.Dock = DockStyle.Fill;
					form.Controls.Add(browser);

					SetParent(form.Handle, workerhwnd);

					//Thread.Sleep(5000);
					//form.Close();
				};

				Application.Run(form);
				// redrawing/invalidating the window didnt do anything. closing the handle to the worker also didnt repaint the desktop. the only fix was to either close explorer.exe or change wallpaper.
				ResetWallpaper();
				// TODO: if reset fails, try restarting explorer.exe instead

			}

		}

		public static void InitSettings()
		{
			CefSettings settings = new CefSettings();
			settings.RegisterScheme(new CefSharp.CefCustomScheme {
				SchemeName = "thighhigh",
				SchemeHandlerFactory = new FolderSchemeHandlerFactory(@"html")
			});

			Cef.Initialize(settings);
		}

		// gets and sets the same existing wallpaper
		public static void ResetWallpaper()
		{
			StringBuilder origPath = new StringBuilder(260);
			if (SystemParametersInfo(0x73, 260, origPath, 0))
				SystemParametersInfo(0x14, 0, origPath.ToString(), 0x1 | 0x2);
		}

	}
}
