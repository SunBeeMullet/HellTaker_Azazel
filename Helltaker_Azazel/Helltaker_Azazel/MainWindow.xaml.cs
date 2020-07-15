using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Helltaker_Azazel
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Bitmap original;
		Bitmap[] frames = new Bitmap[12];
		ImageSource[] imgFrames = new ImageSource[12];
		string bitmapPath = "Resources/Azazel.png";
		int frame = -1;

		/* for release bitmap */
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject([In] IntPtr hObject);

		public MainWindow()
		{
			InitializeComponent();

			original = System.Drawing.Image.FromFile(bitmapPath) as Bitmap;
			for (int i = 0; i < 12; i++)
			{
				frames[i] = new Bitmap(100, 100);
				using (Graphics g = Graphics.FromImage(frames[i]))
				{
					g.DrawImage(original, 
						new System.Drawing.Rectangle(0, 0, 100, 100), 
						new System.Drawing.Rectangle(i * 100, 0, 100, 100), 
						GraphicsUnit.Pixel);
				}
				var handle = frames[i].GetHbitmap();
				try
				{
					imgFrames[i] = Imaging.CreateBitmapSourceFromHBitmap(handle,
						IntPtr.Zero,
						Int32Rect.Empty,
						BitmapSizeOptions.FromEmptyOptions());
				}
				finally 
				{
					DeleteObject(handle);
				}
				Topmost = true;
			}
			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(0.0167*3); // 60FPS
			timer.Tick += NextFrame;
			timer.Start();

			MouseDown += MainWindow_MouseDown;

			/* for notify icon */
			var menu = new System.Windows.Forms.ContextMenu();
			var noti = new System.Windows.Forms.NotifyIcon
			{
				Icon = System.Drawing.Icon.FromHandle(frames[0].GetHicon()),
				Visible = true,
				Text = "Azazel",
				ContextMenu = menu,
			};
			var item = new System.Windows.Forms.MenuItem
			{
				Index = 0,
				Text = "Good bye Azazel",
			};
			item.Click += (object o, EventArgs e) =>
			{
				Application.Current.Shutdown();
			};

			menu.MenuItems.Add(item);
			noti.ContextMenu = menu;
		}

		private void NextFrame(object sender, EventArgs e)
		{
			frame = (frame + 1) % 12;
			iAzazel.Source = imgFrames[frame];
		}
		private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left) this.DragMove();
		}

	}
}
