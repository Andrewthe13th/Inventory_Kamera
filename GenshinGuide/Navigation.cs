using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsInput;

namespace GenshinGuide
{
    public static class Navigation
    {
        private static Process genshinImpact;
        public static InputSimulator sim = new InputSimulator();
        private static RECT area = new RECT();
        private static RECT position = new RECT();

        public static void Initialize(string processName)
        {
            BringMainWindowToFront(processName);

            IntPtr handle = genshinImpact.MainWindowHandle;
            try
            {
                // Get area and position
                ClientToScreen(handle, ref position);
                GetClientRect(handle, ref area);
            }
            catch (Exception)
            {
                // Exit if wasn't able to set area and position
                Debug.Print("Can't initialize Navigation!!!!");
                Form1.UnexpectedError("Can't initialize Navigation!!!!");
                throw;
            }
        }

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

        public static int GetWidth()
        {
            return area.right;
        }

        public static int GetHeight()
        {
            return area.bottom;
        }

        public static RECT GetPosition()
        {
            return position;
        }

        public static void SelectArtifactInventory()
        {
            Double artifactButtonLocation_X = (Double)area.right * ((Double)57 / (Double)160);
            Double artifactButtonLocation_Y = (Double)area.bottom * ((Double)5 / (Double)90);
            SetCursorPos(position.left + Convert.ToInt32(artifactButtonLocation_X), position.top + Convert.ToInt32(artifactButtonLocation_Y));
            sim.Mouse.LeftButtonClick();
            SystemRandomWait(Speed.UI);
        }

        public static void SelectWeaponInventory()
        {
            Double artifactButtonLocation_X = (Double)area.right * ((Double)50 / (Double)160);
            Double artifactButtonLocation_Y = (Double)area.bottom * ((Double)5 / (Double)90);
            SetCursorPos(position.left + Convert.ToInt32(artifactButtonLocation_X), position.top + Convert.ToInt32(artifactButtonLocation_Y));
            sim.Mouse.LeftButtonClick();
            SystemRandomWait(Speed.UI);
        }

        public static void SelectCharacterDevelopmentItems()
        {
            Double artifactButtonLocation_X = (Double)area.right * ((Double)64 / (Double)160);
            Double artifactButtonLocation_Y = (Double)area.bottom * ((Double)5 / (Double)90);
            SetCursorPos(position.left + Convert.ToInt32(artifactButtonLocation_X), position.top + Convert.ToInt32(artifactButtonLocation_Y));
            sim.Mouse.LeftButtonClick();
            SystemRandomWait(Speed.UI);
        }

        public static void SelectCharacterAttributes()
        {
            int xOffset = 100;
            int yOffset = 100;
            Navigation.SetCursorPos(Navigation.GetPosition().left + xOffset, Navigation.GetPosition().top + yOffset);
            Navigation.sim.Mouse.LeftButtonClick();
            Navigation.SystemRandomWait(Speed.CharacterUI);
        }

        public static void SelectCharacterConstellation()
        {
            int xOffset = 100;
            int yOffset = 235;
            Navigation.SetCursorPos(Navigation.GetPosition().left + xOffset, Navigation.GetPosition().top + yOffset);
            Navigation.sim.Mouse.LeftButtonClick();
            Navigation.SystemRandomWait(Speed.CharacterUI);
        }

        public static void SelectCharacterTalents()
        {
            int xOffset = 100;
            int yOffset = 290;
            Navigation.SetCursorPos(Navigation.GetPosition().left + xOffset, Navigation.GetPosition().top + yOffset);
            Navigation.sim.Mouse.LeftButtonClick();
            Navigation.SystemRandomWait(Speed.CharacterUI);
        }

        public static void SelectNextCharacter()
        {
            int xOffset = 1230;
            int yOffset = 350;
            //Navigation.SetCursorPos(Navigation.GetPosition().left + xOffset, Navigation.GetPosition().top + yOffset);
            //Navigation.SystemRandomWait(); 
            Navigation.SetCursorPos(Navigation.GetPosition().left + xOffset, Navigation.GetPosition().top + yOffset);
            Navigation.sim.Mouse.LeftButtonClick();
            Navigation.SystemRandomWait(Speed.SelectNextCharacter);
        }

        public static void CharacterScreen()
        {
            Navigation.sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.ESCAPE);
            Navigation.SystemRandomWait(Speed.UI);
            Navigation.sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_C);
            Navigation.SystemRandomWait(Speed.UI);
        }

        public static void InventoryScreen()
        {
            Navigation.sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.ESCAPE);
            Navigation.SystemRandomWait(Speed.UI);
            Navigation.sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_B);
            Navigation.SystemRandomWait(Speed.UI);
        }

        public static void MainMenuScreen()
        {
            Navigation.sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.ESCAPE);
            Navigation.SystemRandomWait(Speed.UI);
            Navigation.sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.ESCAPE);
            Navigation.SystemRandomWait(Speed.UI);
        }

        #region Window size of Genshin Impact

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT Rect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetClientRect(IntPtr hWnd, ref RECT Rect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ClientToScreen(IntPtr hWnd, ref RECT Rect);
        #endregion

        #region Switch Process into main focus
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        public static void BringMainWindowToFront(string processName)
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
                Form1.UnexpectedError("Cannot find process");
            }
        }
        #endregion

        #region Mouse Libaries
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
        #endregion

        #region System Sleep
        public static void SystemRandomWait(Speed type = Speed.Normal)
        {
            Random r = new Random();

            if (type == Speed.Normal)
            {
                int value = r.Next(320, 500);
                System.Threading.Thread.Sleep(value);

            }
            else if (type == Speed.Instant)
            {
                int value = 10;
                System.Threading.Thread.Sleep(value);
            }
            else if (type == Speed.Faster)
            {
                int value = 40;
                System.Threading.Thread.Sleep(value);
            }
            else if (type == Speed.Fast)
            {
                int value = r.Next(125, 150);
                System.Threading.Thread.Sleep(value);
            }
            else if (type == Speed.CharacterUI)
            {
                int value = 200;
                System.Threading.Thread.Sleep(value);
            }
            else if (type == Speed.ArtifactIgnore)
            {
                int value = r.Next(80, 120);
                System.Threading.Thread.Sleep(value);
            }
            else if (type == Speed.Slow)
            {
                int value = r.Next(1900, 2600);
                System.Threading.Thread.Sleep(value);
            }else if (type == Speed.UI)
            {
                int value = r.Next(1000, 1200);
                System.Threading.Thread.Sleep(value);
            }
            else if (type == Speed.UI)
            {
                int value = r.Next(1200, 1500);
                System.Threading.Thread.Sleep(value);
            }
            else if (type == Speed.SelectNextCharacter)
            {
                int value = 600;
                System.Threading.Thread.Sleep(value);
            }
            else if (type == Speed.SelectNextInventoryItem)
            {
                int value = 64;
                System.Threading.Thread.Sleep(value);
            }

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
            CharacterUI,
        }
        #endregion
    }
}
