using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace InventoryKamera
{
    public static class Navigation
	{
		private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		internal static InputSimulator sim = new InputSimulator();
		private static RECT WindowSize;
		private static RECT WindowPosition;
		private static Size AspectRatio;
		public static bool IsNormal { get; private set; }

		private static double delay = 1;

		public static VirtualKeyCode escapeKey = VirtualKeyCode.ESCAPE;
		public static VirtualKeyCode characterKey = VirtualKeyCode.VK_C;
		public static VirtualKeyCode inventoryKey = VirtualKeyCode.VK_B;
		public static VirtualKeyCode oneKey = VirtualKeyCode.VK_1;

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool GetClientRect(IntPtr hWnd, ref RECT Rect);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool ClientToScreen(IntPtr hWnd, ref RECT Rect);

		public static void Initialize()
		{
			var executables = Properties.Settings.Default.Executables;
			foreach (var processName in executables)
			{
				Logger.Debug("Checking for {genshin}.exe", processName);
				if (InitializeProcess(processName, out IntPtr handle))
				{
					// Get area and position
					ClientToScreen(handle, ref WindowPosition);
					GetClientRect(handle, ref WindowSize);
					
					try
					{
						AspectRatio = GetAspectRatio();
					}
					catch (DivideByZeroException)
					{
						throw new NotImplementedException("Genshin window could not be focused. Please make sure the game is visible.");
					}
					catch (Exception)
					{
						throw;
					}

					Logger.Debug("Found {0}.exe", processName);
					Logger.Debug("Window ({0}x{1}): x={2}, y={3}", WindowSize.Width, WindowSize.Height, WindowPosition.Left, WindowPosition.Top);
					return;
				}
				Logger.Debug("Could not find {0}.exe", processName);
			}

			throw new NullReferenceException("Cannot find Genshin Impact process");
		}

		public static void Reset()
		{
			WindowSize = new RECT();
			WindowPosition = new RECT();
			AspectRatio = new Size();
			sim = new InputSimulator();
		}

		#region Window Capturing

		public static Bitmap CaptureWindow(PixelFormat format = PixelFormat.Format24bppRgb)
		{
			Bitmap bmp = new Bitmap(GetWidth(), GetHeight(), format);
			using (Graphics gfxBmp = Graphics.FromImage(bmp))
			{
				gfxBmp.CopyFromScreen(GetPosition().Left, GetPosition().Top, 0, 0, bmp.Size);
				Logger.Debug($"Copying window ({GetWidth()}x{GetHeight()}) starting at x={GetPosition().Left}, y={GetPosition().Top}");

				var uidRegion = new RECT(
					Left: (int)( 1070 / 1280.0 * bmp.Width ),
					Top: (int)( 695 / 720.0 * bmp.Height ),
					Right: bmp.Width,
					Bottom: bmp.Height);
				gfxBmp.FillRectangle(new SolidBrush(Color.Black), uidRegion);
			}
			return bmp;
		}

		public static Bitmap CaptureRegion(RECT region, PixelFormat format = PixelFormat.Format24bppRgb)
		{
			Bitmap bmp = new Bitmap(region.Width, region.Height, format);
			using (Graphics gfxBmp = Graphics.FromImage(bmp))
			{
				gfxBmp.CopyFromScreen(GetPosition().Left + region.Left, GetPosition().Top + region.Top, 0, 0, bmp.Size);
			}
			return bmp;
		}

		public static Bitmap CaptureRegion(int x, int y, int width, int height, PixelFormat format = PixelFormat.Format24bppRgb)
		{
			return CaptureRegion(new Rectangle(x, y, width, height), format);
		}

		#endregion Window Capturing

		#region Image Displaying

		public static void DisplayBitmap(Bitmap bm, string text = "Image")
		{
			Form form = new Form();
			
			int padding = 5;

			form.StartPosition = FormStartPosition.Manual;
			form.Location = Screen.PrimaryScreen.WorkingArea.Location;
			form.Size = new Size(bm.Width + 5*padding, bm.Height + 10*padding);
			form.Text = text;
			form.BackColor = Color.Black;

			PictureBox pb = new PictureBox
			{
				Dock = DockStyle.Fill,
				Image = bm,
				Padding = new Padding(5),
				//Size = new Size(bm.Width + 2*padding, bm.Height + 2*padding),
			};

			form.Controls.Add(pb);
			Application.Run(form);
			
		}

		#endregion Image Displaying

		#region Game Menu Navigation

		public static void SelectWeaponInventory()
		{
			int buttonX = (int)(385 / 1280.0 * GetWidth());
			int buttonY = (int)(35  / 720.0 * GetHeight());
			SetCursor(buttonX, buttonY);
			Click();
			SystemWait(Speed.UI);

			// Scroll item preview hotfix
			buttonX = (int)(975 / 1280.0 * GetWidth());
			buttonY = (int)(360 / 720.0 * GetHeight());

			SetCursor(buttonX, buttonY);
			Click();
			Scroll(Direction.UP, 15);
			Wait(200);
		}

		public static void SelectArtifactInventory()
		{
			int buttonX = (int)(448 / 1280.0 * GetWidth());
			int buttonY = (int)(31 / 720.0 * GetHeight());
			SetCursor(buttonX, buttonY);
			Click();
			SystemWait(Speed.UI);

			// Scroll item preview hotfix
			buttonX = (int)(975 / 1280.0 * GetWidth());
			buttonY = (int)(360 / 720.0 * GetHeight());

			SetCursor(buttonX, buttonY);
			Click();
			Scroll(Direction.UP, 15);
			Wait(200);
		}

		public static void SelectCharacterDevelopmentInventory()
		{
			int buttonX = (int)(512 / 1280.0 * GetWidth());
			int buttonY = (int)(40  / 720.0 * GetHeight());
			SetCursor(buttonX, buttonY);
			Click();
			SystemWait(Speed.UI);

			// Scroll item preview hotfix
			buttonX = (int)(975 / 1280.0 * GetWidth());
			buttonY = (int)(360 / 720.0 * GetHeight());

			SetCursor(buttonX, buttonY);
			Click();
			Scroll(Direction.UP, 15);
			Wait(200);
		}

		public static void SelectMaterialInventory()
		{
			int buttonX = (int)(636 / 1280.0 * GetWidth());
			int buttonY = (int)(30  / 720.0 * GetHeight());
			SetCursor(buttonX, buttonY);
			Click();
			SystemWait(Speed.UI);

			// Scroll item preview hotfix
			buttonX = (int)(975 / 1280.0 * GetWidth());
			buttonY = (int)(360 / 720.0 * GetHeight());

			SetCursor(buttonX, buttonY);
			Click();
			Scroll(Direction.UP, 15);
			Wait(200);
		}

		public static void SelectCharacterAttributes()
		{
			int xOffset = (int)(170 / 1280.0 * GetWidth());
			int yOffset = (int)(105 / 720.0 * GetHeight());

			if (GetAspectRatio() == new Size(8, 5))
			{
				yOffset = (int)( 105 / 800.0 * GetHeight() );
			}

			SetCursor(xOffset, yOffset);
			Click();
			SystemWait(Speed.CharacterUI);
		}

		public static void SelectCharacterConstellation()
		{
			int xOffset = (int)(170 / 1280.0 * GetWidth());
			int yOffset = (int)(245 / 720.0 * GetHeight());

			if (GetAspectRatio() == new Size(8, 5))
			{
				yOffset = (int)( 245 / 800.0 * GetHeight() );
			}

			SetCursor(xOffset, yOffset);
			Click();
			SystemWait(Speed.CharacterUI);
		}

		public static void SelectCharacterTalents()
		{
			int xOffset = (int)(135 / 1280.0 * GetWidth());
			int yOffset = (int)(290 / 720.0 * GetHeight());

			if (GetAspectRatio() == new Size(8, 5))
			{
				yOffset = (int)( 290 / 800.0 * GetHeight() );
			}

			SetCursor(xOffset, yOffset);
			Click();
			SystemWait(Speed.CharacterUI);
		}

		public static void SelectNextCharacter()
		{
			int xOffset = (int)(1230 / 1280.0 * GetWidth());
			int yOffset = (int)(350 / 720.0 * GetHeight());

			if (GetAspectRatio() == new Size(8, 5))
			{
				yOffset = (int)( 400 / 800.0 * GetHeight() );
			}

			SetCursor(xOffset, yOffset);
			Click();
			SystemWait(Speed.SelectNextCharacter);
		}

		public static void CharacterScreen()
		{
			sim.Keyboard.KeyPress(escapeKey);
			SystemWait(Speed.UI);
			sim.Keyboard.KeyPress(oneKey);
			SystemWait(Speed.UI);
			sim.Keyboard.KeyPress(characterKey);
			SystemWait(Speed.UI);
		}

		public static void InventoryScreen()
		{
			sim.Keyboard.KeyPress(escapeKey);
			SystemWait(Speed.UI);
			sim.Keyboard.KeyPress(inventoryKey);
			SystemWait(Speed.UI);
		}

		public static void MainMenuScreen()
		{
			sim.Keyboard.KeyPress(escapeKey);
			SystemWait(Speed.UI);
			sim.Keyboard.KeyPress(escapeKey);
			SystemWait(Speed.UI);
		}

		#endregion Game Menu Navigation

		#region Window Size Accessing

		public static Size GetSize()
		{
			return WindowSize.Size;
		}

		public static Size GetAspectRatio()
		{
			if (!AspectRatio.IsEmpty) return AspectRatio;

			if (WindowSize.Width == 0) throw new DivideByZeroException("Genshin's window width cannot be 0");
			if (WindowSize.Height == 0) throw new DivideByZeroException("Genshin's window height cannot be 0");
			int x = WindowSize.Width/GCD(WindowSize.Width, WindowSize.Height);
			int y = WindowSize.Height/GCD(WindowSize.Width, WindowSize.Height);
			var size = new Size(x, y);
			
			IsNormal = size == new Size(16, 9);

			return size;
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
			return WindowSize.Width;
		}

		public static int GetHeight()
		{
			return WindowSize.Height;
		}

		public static RECT GetPosition()
		{
			return WindowPosition;
		}

		#endregion Window Size Accessing

		#region Window Focusing

		[DllImport("user32.dll")]
		private static extern int SetForegroundWindow(IntPtr hwnd);

		[DllImport("user32.dll")]
		private static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);
		
		[DllImport("user32.dll")]
		private static extern bool SetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);
		
		private struct WindowPlacement
		{
			public int length;
			public int flags;
			public ShowWindowEnum showCmd;
			public Point ptMinPosition;
			public Point ptMaxPosition;
			public Rectangle rcNormalPosition;
		}
		
		private enum ShowWindowEnum
		{
			Hide = 0,
			ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
			Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
			Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
			Restore = 9, ShowDefault = 10, ForceMinimized = 11
		};

		public static bool InitializeProcess(string processName, out IntPtr handle)
		{
			handle = IntPtr.Zero;
			// Get process
			using (Process genshin = Process.GetProcessesByName(processName).FirstOrDefault())
			{
				// check if the process is running
				if (genshin != null)
				{
					handle = genshin.MainWindowHandle;
					
					var windowPlacement = new WindowPlacement{length = Marshal.SizeOf(typeof(WindowPlacement))};
					GetWindowPlacement(handle, ref windowPlacement);

					// Check if minimized
					if (windowPlacement.showCmd == ShowWindowEnum.ShowMinimized)
					{
						windowPlacement.showCmd = ShowWindowEnum.ShowNormal;
						SetWindowPlacement(handle, ref windowPlacement);
					}

					// Bring game to front
					SetForegroundWindow(handle);
					return true;
				}
			}
			return false;
		}

		#endregion Window Focusing

		#region Mouse

		[DllImport("user32.dll")]
		public static extern bool SetCursorPos(int X, int Y);

		public static bool SetCursor(int X, int Y)
        {
			return SetCursorPos(GetPosition().Left + X, GetPosition().Top + Y);
		}

		public static void Click()
		{
			if (SystemInformation.MouseButtonsSwapped)
				sim.Mouse.RightButtonClick();
			else
				sim.Mouse.LeftButtonClick();
		}

		public static void Scroll(Direction direction, int scrolls, int delay = 1)
        {
			Action Scroll;
            switch (direction)
            {
                case Direction.UP:
					Scroll = () => sim.Mouse.VerticalScroll(1);
					break;
                case Direction.DOWN:
					Scroll = () => sim.Mouse.VerticalScroll(-1);
                    break;
                case Direction.LEFT:
					Scroll = () => sim.Mouse.HorizontalScroll(-1);
                    break;
                case Direction.RIGHT:
					Scroll = () => sim.Mouse.HorizontalScroll(1);
                    break;
                default:
                    return;
            }
            for (int i = 0; i < scrolls; i++)
            {
				Scroll();
				Wait(delay);
            }
        }

		public enum Direction
        {
			UP = 0,
			DOWN = 1,
			LEFT = 2,
			RIGHT = 3,
        }

		#endregion Mouse

		#region Delays

		public static void SystemWait(Speed speed = Speed.Normal)
		{
			double value;
			switch (speed)
			{
				case Speed.Fastest:
					value = 10;
					break;

				case Speed.Faster:
					value = 75;
					break;

				case Speed.Fast:
					value = 100;
					break;

				case Speed.Normal:
					value = 500;
					break;

				case Speed.Slow:
					value = 750;
					break;

				case Speed.Slower:
					value = 1000;
					break;

				case Speed.Slowest:
					value = 2000;
					break;

				case Speed.CharacterUI:
					value = 2000;
					break;

				case Speed.ArtifactIgnore:
					value = 80;
					break;

				case Speed.UI:
					value = 2000;
					break;

				case Speed.SelectNextCharacter:
					value = 700;
					break;

				case Speed.InventoryScroll:
					value = 10;
					break;

				case Speed.SelectNextInventoryItem:
					value = 200;
					break;

				default:
					value = 1000;
					break;
			}
			value *= delay;

			Wait(((int)value));
		}

		public static void SystemWait(float ms)
        {
			Wait((int)(ms * delay));
        }

		public static void Wait(int ms = 1000)
		{
			Thread.Sleep(ms);
		}

		public static void SetDelay(double _delay)
		{
			delay = _delay;
		}

		public static double GetDelay()
		{
			return delay;
		}

		public enum Speed
		{
			Slowest,
			Slower,
			Slow,
			Normal,
			Fast,
			Faster,
			Fastest,
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