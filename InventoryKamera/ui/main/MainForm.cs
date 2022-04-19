using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using NHotkey;
using NHotkey.WindowsForms;
using WindowsInput.Native;

namespace InventoryKamera
{
	public partial class MainForm : Form
	{
		private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
		private static Thread scannerThread;
		private static InventoryKamera data;

		private int Delay;

		private bool running = false;

		public MainForm()
		{
			InitializeComponent();

			Language_ComboBox.SelectedItem = "ENG";

			var version = Regex.Replace(Assembly.GetExecutingAssembly().GetName().Version.ToString(), @"[.0]*$", string.Empty);
			Logger.Info("Inventory Kamera version {0}", version);

			Text = $"Inventory Kamera V{version}";

			UserInterface.Init(
				GearPictureBox,
				ArtifactOutput_TextBox,
				CharacterName_PictureBox,
				CharacterLevel_PictureBox,
				new[] { CharacterTalent1_PictureBox, CharacterTalent2_PictureBox, CharacterTalent3_PictureBox },
				CharacterOutput_TextBox,
				WeaponsScannedCount_Label,
				WeaponsMax_Labell,
				ArtifactsScanned_Label,
				ArtifactsMax_Label,
				CharactersScanned_Label,
				ProgramStatus_Label,
				ErrorLog_TextBox,
				Navigation_Image);
			MaximizeBox = false;
			MinimizeBox = false;
			Logger.Info("MainForm initialization complete");
		}

		private int ScannerDelayValue(int value)
		{
			switch (value)
			{
				case 0:
					return 0;

				case 1:
					return 50;

				case 2:
					return 100;

				default:
					return 100;
			}
		}

		private void Hotkey_Pressed(object sender, HotkeyEventArgs e)
		{
			Logger.Info("Hotkey pressed");
			e.Handled = true;
			// Check if scanner is running
			if (scannerThread.IsAlive)
			{
				// Stop navigating weapons/artifacts
				scannerThread.Abort();

				UserInterface.SetProgramStatus("Scan Stopped");

				Navigation.Reset();
			}
		}

		private void ResetUI()
		{
			Navigation.Reset();

			// Need to invoke method from the UI's handle, not the worker thread
			BeginInvoke((MethodInvoker)delegate { RemoveHotkey(); });
			Logger.Info("Hotkey removed");
		}

		private void RemoveHotkey()
		{
			HotkeyManager.Current.Remove("Stop");
		}

		public static void UnexpectedError(string error)
		{
			if (scannerThread.IsAlive)
			{
				UserInterface.AddError(error);
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			if (Properties.Settings.Default.UpgradeNeeded)
			{
				try
				{
					Properties.Settings.Default.Upgrade();
					Logger.Info("Application settings loaded from previous version");
				}
				catch (Exception) { }
				Properties.Settings.Default.UpgradeNeeded = false;
			}

			UpdateKeyTextBoxes();

			Delay = ScannerDelay_TrackBar.Value;

			ProgramStatus_Label.Text = "";
			if (string.IsNullOrWhiteSpace(OutputPath_TextBox.Text))
			{
				OutputPath_TextBox.Text = Directory.GetCurrentDirectory() + @"\GenshinData";
			}

		}

        private bool CheckForUpdates()
        {
            var databaseManager = new DatabaseManager();
            return databaseManager.CheckForUpdates();
        }

        private void UpdateKeyTextBoxes()
		{
			Navigation.inventoryKey = (VirtualKeyCode)Properties.Settings.Default.InventoryKey;
			Navigation.characterKey = (VirtualKeyCode)Properties.Settings.Default.CharacterKey;

			inventoryToolStripTextBox.Text = new KeysConverter().ConvertToString((Keys)Navigation.inventoryKey);
			characterToolStripTextBox.Text = new KeysConverter().ConvertToString((Keys)Navigation.characterKey);

			// Make sure text boxes show key glyph and not "OEM..."
			if (inventoryToolStripTextBox.Text.ToUpper().Contains("OEM"))
			{
				inventoryToolStripTextBox.Text = KeyCodeToUnicode((Keys)Navigation.inventoryKey);
			}
			if (characterToolStripTextBox.Text.ToUpper().Contains("OEM"))
			{
				characterToolStripTextBox.Text = KeyCodeToUnicode((Keys)Navigation.characterKey);
			}

		}

		private void StartButton_Clicked(object sender, EventArgs e)
		{
			GC.Collect();

			UserInterface.ResetAll();

			UserInterface.SetProgramStatus("Scanning");
			Logger.Info("Starting scan");

			if (Directory.Exists(OutputPath_TextBox.Text) || Directory.CreateDirectory(OutputPath_TextBox.Text).Exists)
			{
				if (running)
				{
					Logger.Debug("Already running");
					return;
				}
				running = true;

				HotkeyManager.Current.AddOrReplace("Stop", Keys.Enter, Hotkey_Pressed);
				Logger.Info("Hotkey registered");
				var settings = Properties.Settings.Default;
				var options = $"\n\tWeapons:\t\t\t {settings.ScanWeapons}\n" +
                    $"\tArtifacts:\t\t\t {settings.ScanArtifacts}\n" +
                    $"\tCharacters:\t\t\t {settings.ScanCharacters}\n" +
                    $"\tDev Items:\t\t\t {settings.ScanCharDevItems}\n" +
                    $"\tMaterials:\t\t\t {settings.ScanMaterials}\n" +
                    $"\tMin Weapon Rarity:\t {settings.MinimumWeaponRarity}\n" +
                    $"\tMin Artifact Rarity: {settings.MinimumArtifactRarity}\n" +
                    $"\tDelay:\t\t\t\t {settings.ScannerDelay}";

				scannerThread = new Thread(() =>
				{
					try
					{
						// Get Screen Location and Size
						Navigation.Initialize();

						List<Size> sizes = new List<Size>
						{
							new Size(16,9),
							new Size(8,5),
						};

						if (!sizes.Contains(Navigation.GetAspectRatio()))
						{
							throw new NotImplementedException($"{Navigation.GetSize().Width}x{Navigation.GetSize().Height} is an unsupported resolution.");
						}

						if (Navigation.GetSize() != Navigation.CaptureWindow().Size) throw new FormatException("Window size and screenshot size mismatch. Please make sure the game is not in a fullscreen mode.");

						data = new InventoryKamera();

						// Add navigation delay
						Navigation.SetDelay(ScannerDelayValue(Delay));

						Logger.Info("Scan settings: {0}", options);
						
						// The Data object of json object
						data.GatherData();

						// Covert to GOOD
						GOOD good = new GOOD(data);
						Logger.Info("Data converted to GOOD");

						// Make Json File
						good.WriteToJSON(OutputPath_TextBox.Text);
						Logger.Info("Exported data");

						UserInterface.SetProgramStatus("Finished");
					}
					catch (ThreadAbortException)
					{
						// Workers can get stuck if the thread is aborted or an exception is raised
						if (!( data is null )) data.StopImageProcessorWorkers();
						UserInterface.SetProgramStatus("Scan stopped");
					}
					catch (NotImplementedException ex)
					{
						UserInterface.AddError(ex.Message);
					}
					catch (Exception ex)
					{
						// Workers can get stuck if the thread is aborted or an exception is raised
						if (!( data is null )) data.StopImageProcessorWorkers();
						UserInterface.AddError(ex.ToString());
						UserInterface.SetProgramStatus("Scan aborted", ok: false);
					}
					finally
					{
						ResetUI();
						running = false;
						Logger.Info("Scanner stopped");
					}
				})
				{
					IsBackground = true
				};
				scannerThread.Start();
			}
			else
			{
				if (string.IsNullOrWhiteSpace(OutputPath_TextBox.Text))
					UserInterface.AddError("Please set an output directory");
				else
					UserInterface.AddError($"{OutputPath_TextBox.Text} is not a valid directory");
			}
		}

		private void Github_Label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://github.com/Andrewthe13th/Inventory_Kamera/");
		}

		private void Releases_Label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://github.com/Andrewthe13th/Inventory_Kamera/releases");
		}

		private void IssuesPage_Label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://github.com/Andrewthe13th/Inventory_Kamera/issues");
		}

		private void FileSelectButton_Click(object sender, EventArgs e)
		{
			// A nicer file browser
			CommonOpenFileDialog d = new CommonOpenFileDialog
			{
				InitialDirectory = !Directory.Exists(OutputPath_TextBox.Text) ? Directory.GetCurrentDirectory() : OutputPath_TextBox.Text,
				IsFolderPicker = true
			};

			if (d.ShowDialog() == CommonFileDialogResult.Ok)
			{
				OutputPath_TextBox.Text = d.FileName;
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveSettings();
			RemoveHotkey();
			NLog.LogManager.Shutdown();
		}

		private void SaveSettings()
		{
			Properties.Settings.Default.Save();
		}

		private void ScannerDelay_TrackBar_ValueChanged(object sender, EventArgs e)
		{
			Delay = ( (TrackBar)sender ).Value;
			SaveSettings();
		}

		private void Exit_MenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void OptionsMenuItem_KeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			e.SuppressKeyPress = true;

			// Virtual keys for 0-9, A-Z
			bool vk = e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.Z;
			// Numpad keys and function keys (internally accepts up to F24)
			bool np = e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.F24;
			// OEM keys (Keys that vary depending on keyboard layout)
			bool oem = e.KeyCode >= Keys.Oem1 && e.KeyCode <= Keys.Oem7;
			// Arrow keys, spacebar, INS, DEL, HOME, END, PAGEUP, PAGEDOWN
			bool misc = e.KeyCode == Keys.Space || (e.KeyCode >= Keys.Left && e.KeyCode <= Keys.Down) || (e.KeyCode >= Keys.Prior && e.KeyCode <= Keys.Home) || e.KeyCode == Keys.Insert || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back;

			// Validate that key is an acceptable Genshin keybind.
			if (!vk && !np && !oem && !misc)
			{
				Logger.Debug("Invalid {key} key pressed", e.KeyCode);
				return;
			}
			ToolStripTextBox s = (ToolStripTextBox)sender;

			// Needed to differentiate between NUMPAD numbers and numbers at top of keyboard
			s.Text = np || e.KeyCode == Keys.Back ? new KeysConverter().ConvertToString(e.KeyCode) : KeyCodeToUnicode(e.KeyData);

			// Spacebar or upper navigation keys (INSERT-PAGEDOWN keys) make textbox empty
			if (string.IsNullOrWhiteSpace(s.Text) || string.IsNullOrEmpty(s.Text))
			{
				s.Text = new KeysConverter().ConvertToString(e.KeyCode);
			}


			switch (s.Tag)
			{
				case "InventoryKey":
					Navigation.inventoryKey = (VirtualKeyCode)e.KeyCode;
					Logger.Debug("Inv key set to: {key}", Navigation.inventoryKey);
					break;

				case "CharacterKey":
					Navigation.characterKey = (VirtualKeyCode)e.KeyCode;
					Logger.Debug("Char key set to: {key}", Navigation.characterKey);
					break;

				default:
					break;
			}
		}

		private void DatabaseUpdateMenuItem_Click(object sender, EventArgs e)
		{
			var updateForm = new DatabaseUpdateForm();
			updateForm.ShowDialog();
		}

		#region Unicode Helper Functions

		// Needed to display OEM keys as glyphs from keyboard. Should work for other languages
		// and keyboard layouts but only tested with QWERTY layout.
		private string KeyCodeToUnicode(Keys key)
		{
			byte[] keyboardState = new byte[255];
			bool keyboardStateStatus = GetKeyboardState(keyboardState);

			if (!keyboardStateStatus)
			{
				return "";
			}
			uint virtualKeyCode = (uint)key;
			uint scanCode = MapVirtualKey(virtualKeyCode, 0);
			IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

			StringBuilder result = new StringBuilder();
			ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, result, 5, 0, inputLocaleIdentifier);

			return result.ToString();
		}

		[DllImport("user32.dll")]
		private static extern bool GetKeyboardState(byte[] lpKeyState);

		[DllImport("user32.dll")]
		private static extern uint MapVirtualKey(uint uCode, uint uMapType);

		[DllImport("user32.dll")]
		private static extern IntPtr GetKeyboardLayout(uint idThread);

		[DllImport("user32.dll")]
		private static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

		#endregion Unicode Helper Functions

		private void SaveSettings(object sender, EventArgs e)
		{
			SaveSettings();
		}

		private void ExportFolderMenuItem_Click(object sender, EventArgs e)
		{
			if (Directory.Exists(OutputPath_TextBox.Text) || Directory.CreateDirectory(OutputPath_TextBox.Text).Exists)
			{
				Process.Start($@"{OutputPath_TextBox.Text}");
			}
			else
			{
				Process.Start("explorer.exe");
			}
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			var updatesAvailable = CheckForUpdates();
			if (updatesAvailable)
			{ 
				var message = "A new version for Genshin Impact has been found. Would you like to update this Kamera's lookup tables? (Recommended)";
				var result = MessageBox.Show(message, "Game Version Update", MessageBoxButtons.YesNo);
				if (result == DialogResult.Yes)
				{
					new DatabaseManager().UpdateAllLists();
					MessageBox.Show("Update complete");
				}
				else if (result == DialogResult.No)
				{
					MessageBox.Show("Update skipped. Please know that skipping this update will likely result in incorrect scans.\n" +
                        "\nYou may check for updates again on restarting this application or by using the update manager found" +
                        " under 'options'", "Update declined", MessageBoxButtons.OK);
				}
			}
		}
    }
}