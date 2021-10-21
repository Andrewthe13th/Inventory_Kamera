using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GenshinGuide
{
	public static class Constants
	{
		//windows message id for hotkey
		public const int WM_HOTKEY_MSG_ID = 0x0312;
	}

	public class KeyHandler
	{
		[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		private readonly int key;
		private readonly IntPtr hWnd;
		private readonly int id;

		public KeyHandler(Keys key, Form form)
		{
			this.key = (int)key;
			hWnd = form.Handle;
			id = GetHashCode();
		}

		public override int GetHashCode()
		{
			return key ^ hWnd.ToInt32();
		}

		public bool Register()
		{
			return RegisterHotKey(hWnd, id, 0, key);
		}

		public bool Unregiser()
		{
			return UnregisterHotKey(hWnd, id);
		}
	}
}
