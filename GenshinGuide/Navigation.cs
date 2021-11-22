using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace GenshinGuide
{
	public static class Navigation
	{
		private static Process genshinImpact;
		public static InputSimulator sim = new InputSimulator();
		private static RECT area = new RECT();
		private static RECT position = new RECT();

		private static int delay = 0;

		public const VirtualKeyCode escapeKey = VirtualKeyCode.ESCAPE;
		public static VirtualKeyCode characterKey = VirtualKeyCode.VK_C;
		public static VirtualKeyCode inventoryKey = VirtualKeyCode.VK_B;

		public static void Initialize(string processName)
		{
			try
			{
				InitializeProcess(processName);
			}
			catch (NullReferenceException)
			{
				throw;
			}
			IntPtr handle;

			// Get area and position
			handle = genshinImpact.MainWindowHandle;
			ClientToScreen(handle, ref position);
			GetClientRect(handle, ref area);
			return;
		}

		#region Window Capturing

		public static Bitmap CaptureWindow(PixelFormat format = PixelFormat.Format24bppRgb)
		{
			Bitmap bmp = new Bitmap(GetWidth(), GetHeight(), format);
			using (Graphics gfxBmp = Graphics.FromImage(bmp))
			{
				gfxBmp.CopyFromScreen(position.Left, position.Top, 0, 0, bmp.Size);
			}
			return bmp;
		}

		public static Bitmap CaptureRegion(RECT region)
		{
			Bitmap bmp = new Bitmap(region.Width, region.Height, PixelFormat.Format24bppRgb);
			using (Graphics gfxBmp = Graphics.FromImage(bmp))
			{
				gfxBmp.CopyFromScreen(position.Left + region.Left, position.Top + region.Top, 0, 0, bmp.Size);
			}
			return bmp;
		}

		public static Bitmap CaptureRegion(int x, int y, int width, int height)
		{
			return CaptureRegion(new Rectangle(x, y, width, height));
		}

		#endregion Window Capturing

		#region Image Displaying

		public static void DisplayBitmap(Bitmap bm, string text = "Image")
		{
			using (Form form = new Form())
			{
				form.StartPosition = FormStartPosition.Manual;
				form.Location = Screen.PrimaryScreen.WorkingArea.Location;
				form.Size = new Size(bm.Width + 20, bm.Height + 40);
				form.Text = text;

				PictureBox pb = new PictureBox
				{
					Dock = DockStyle.Fill,
					Image = bm,
					Location = new Point(0,0)
				};

				form.Controls.Add(pb);
				form.ShowDialog();
			}
		}

		#endregion Image Displaying

		public static void Reset()
		{
			area = new RECT();
			position = new RECT();
			sim = new InputSimulator();
			genshinImpact = null;
		}

		public static RECT GetArea()
		{
			return area;
		}

		public static Size GetSize()
		{
			return new Size(area.Width, area.Height);
		}

		public static Size GetAspectRatio()
		{
			int x = area.Width/GCD(area.Width, area.Height);
			int y = area.Height/GCD(area.Width, area.Height);
			return new Size(x, y);
		}

		private static int GCD(int a, int b)
		{
			int r;
			while (b != 0)
			{
				r = a % b;
				a = b;
				b = r;
			}
			return a;
		}

		public static int GetWidth()
		{
			return area.Width;
		}

		public static int GetHeight()
		{
			return area.Height;
		}

		public static RECT GetPosition()
		{
			return position;
		}

		#region Game Menu Navigation

		public static void SelectWeaponInventory()
		{
			int buttonX = (int)(385 / 1280.0 * GetWidth());
			int buttonY = (int)(35  / 720.0 * GetHeight());
			SetCursorPos(position.Left + buttonX, position.Top + buttonY);
			sim.Mouse.LeftButtonClick();
			SystemRandomWait(Speed.UI);
		}

		public static void SelectArtifactInventory()
		{
			int buttonX = (int)(448 / 1280.0 * GetWidth());
			int buttonY = (int)(31 / 720.0 * GetHeight());
			SetCursorPos(position.Left + buttonX, position.Top + buttonY);
			sim.Mouse.LeftButtonClick();
			SystemRandomWait(Speed.UI);
		}

		public static void SelectCharacterDevelopmentItems()
		{
			int artifactButtonLocation_X =(int)(area.Right *(64 /(Double) 160));
			int artifactButtonLocation_Y =(int)(area.Bottom *(5 /(Double) 90));
			SetCursorPos(position.Left + artifactButtonLocation_X, position.Top + artifactButtonLocation_Y);
			sim.Mouse.LeftButtonClick();
			SystemRandomWait(Speed.UI);
		}

		public static void SelectCharacterAttributes()
		{
			
			int xOffset = (int)(170 / 1280.0 * GetWidth());
			int yOffset = (int)(105 / 720.0 * GetHeight());

			if (GetAspectRatio() == new Size(8, 5))
			{
				yOffset = (int)( 105 / 800.0 * GetHeight() );
			}

			SetCursorPos(GetPosition().Left + xOffset, GetPosition().Top + yOffset);
			sim.Mouse.LeftButtonClick();
			SystemRandomWait(Speed.CharacterUI);
		}

		public static void SelectCharacterConstellation()
		{
			int xOffset = (int)(170 / 1280.0 * GetWidth());
			int yOffset = (int)(245 / 720.0 * GetHeight());

			if (GetAspectRatio() == new Size(8,5))
			{
				yOffset = (int)( 245 / 800.0 * GetHeight() );
			}

			SetCursorPos(GetPosition().Left + xOffset, GetPosition().Top + yOffset);
			sim.Mouse.LeftButtonClick();
			SystemRandomWait(Speed.CharacterUI);
		}

		public static void SelectCharacterTalents()
		{
			int xOffset = (int)(135 / 1280.0 * GetWidth());
			int yOffset = (int)(290 / 720.0 * GetHeight());

			if (GetAspectRatio() == new Size(8, 5))
			{
				yOffset = (int)( 290 / 800.0 * GetHeight() );
			}

			SetCursorPos(GetPosition().Left + xOffset, GetPosition().Top + yOffset);
			sim.Mouse.LeftButtonClick();
			SystemRandomWait(Speed.CharacterUI);
		}

		public static void SelectNextCharacter()
		{
			int xOffset = (int)(1230 / 1280.0 * GetWidth());
			int yOffset = (int)(350 / 720.0 * GetHeight());

			if (GetAspectRatio() == new Size(8,5))
			{
				yOffset = (int)( 400 / 800.0 * GetHeight() );
			}

			SetCursorPos(GetPosition().Left + xOffset, GetPosition().Top + yOffset);
			sim.Mouse.LeftButtonClick();
			SystemRandomWait(Speed.SelectNextCharacter);
		}

		public static void CharacterScreen()
		{
			sim.Keyboard.KeyPress(escapeKey);
			SystemRandomWait(Speed.UI);
			sim.Keyboard.KeyPress(characterKey);
			SystemRandomWait(Speed.UI);
		}

		public static void InventoryScreen()
		{
			sim.Keyboard.KeyPress(escapeKey);
			SystemRandomWait(Speed.UI);
			sim.Keyboard.KeyPress(inventoryKey);
			SystemRandomWait(Speed.UI);
		}

		public static void MainMenuScreen()
		{
			sim.Keyboard.KeyPress(escapeKey);
			SystemRandomWait(Speed.UI);
			sim.Keyboard.KeyPress(escapeKey);
			SystemRandomWait(Speed.UI);
		}

		#endregion Game Menu Navigation

		#region Window Size

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool GetWindowRect(IntPtr hWnd, ref RECT Rect);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool GetClientRect(IntPtr hWnd, ref RECT Rect);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool ClientToScreen(IntPtr hWnd, ref RECT Rect);

		#endregion Window Size

		#region Window Focusing

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

		[DllImport("user32.dll")]
		private static extern int SetForegroundWindow(IntPtr hwnd);

		private enum ShowWindowEnum
		{
			Hide = 0,
			ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
			Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
			Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
			Restore = 9, ShowDefault = 10, ForceMinimized = 11
		};

		public static void InitializeProcess(string processName)
		{
			// get the process
			genshinImpact = Process.GetProcessesByName(processName).FirstOrDefault();

			// check if the process is running
			if (genshinImpact != null)
			{
				// check if the window is hidden / minimized
				if (genshinImpact.MainWindowHandle == IntPtr.Zero)
				{
					// the window is hidden so try to restore it before setting focus.
					ShowWindow(genshinImpact.Handle, ShowWindowEnum.Restore);
				}

				// set user the focus to the window
				SetForegroundWindow(genshinImpact.MainWindowHandle);
				SystemRandomWait(Speed.Slow);
			}
			else
			{
				// the process is not running, so start it
				UserInterface.AddError("Cannot find Genshin Impact process");
				throw new NullReferenceException();
			}
		}

		#endregion Window Focusing

		#region Mouse

		[DllImport("user32.dll")]
		public static extern bool SetCursorPos(int X, int Y);

		#endregion Mouse

		#region Delays

		public static void SystemRandomWait(Speed type = Speed.Normal)
		{
			Random r = new Random();
			int value = 0;

			switch (type)
			{
				case Speed.Normal:
					value = r.Next(320, 500);
					value += delay;
					break;

				case Speed.Instant:
					value = 10;
					value += delay / 10;
					break;

				case Speed.Faster:
					value = 40;
					value += delay / 5;
					break;

				case Speed.Fast:
					value = r.Next(125, 150);
					value += delay / 2;
					break;

				case Speed.CharacterUI:
					value = 400;
					value += delay;
					break;

				case Speed.ArtifactIgnore:
					value = r.Next(50, 70);
					value += delay / 5;
					break;

				case Speed.Slow:
					value = r.Next(1900, 2600);
					value += 3 * delay;
					break;

				case Speed.UI:
					value = r.Next(1000, 1200);
					value += delay;
					break;

				case Speed.SelectNextCharacter:
					value = 600;
					value += 2 * delay;
					break;

				case Speed.InventoryScroll:
					value = 20;
					value += delay / 5;
					break;

				case Speed.SelectNextInventoryItem:
					value = 64;
					value += delay / 4;
					break;
			}

			System.Threading.Thread.Sleep(value);
		}

		public static void AddDelay(int _delay)
		{
			delay = _delay;
		}

		public enum Speed
		{
			Slow,
			Normal,
			Instant,
			Fast,
			Faster,
			UI,
			ArtifactIgnore,
			SelectNextCharacter,
			SelectNextInventoryItem,
			InventoryScroll,
			CharacterUI,
		}

		#endregion Delays
	}
}