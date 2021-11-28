using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using WindowsInput.Native;

namespace GenshinGuide
{
	public partial class Form1 : Form
	{
		//private KeyHandler ghk;
		private static Thread mainThread;

		private static GenshinData data = new GenshinData();
		public static KeyboardHook hook = new KeyboardHook();
		private static string filePath = "";

		private int Delay;
		private bool WeaponsChecked;
		private bool ArtifactsChecked;
		private bool CharactersChecked;

		private bool running = false;

		public Form1()
		{
			InitializeComponent();

			// register the event that is fired after the key press.
			hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(Hook_KeyPressed);

			Language_ComboBox.SelectedItem = "ENG";
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

		private void Hook_KeyPressed(object sender, KeyPressedEventArgs e)
		{
			// show the keys pressed in a label.
			if (mainThread.IsAlive)
			{
				// stop navigating throw weapons/artifacts
				mainThread.Abort();
				// stop weapon/artifact processor thread
				data.StopImageProcessorWorkers();
				UserInterface.SetProgramStatus("Scan Stopped");
				// Reset data
				data = new GenshinData();
				Navigation.Reset();
			}
			hook.UnRegisterHotKeys();
		}

		private void ResetUI()
		{
			// Reset data
			data = new GenshinData();

			Navigation.Reset();

			// Un-register ENTER key. Otherwise you can't hit ENTER in another application
			hook.UnRegisterHotKeys();
		}

		public static void UnexpectedError(string error)
		{
			if (mainThread.IsAlive)
			{
				UserInterface.AddError(error);
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			LoadSettings();

			Delay = ScannerDelay_TrackBar.Value;

			WeaponsChecked = Weapons_CheckBox.Checked;
			ArtifactsChecked = Artifacts_Checkbox.Checked;
			CharactersChecked = Characters_CheckBox.Checked;
		}

		private void LoadSettings()
		{
			GOOD_CheckBox.Checked = Properties.Settings.Default.FormatGood;
			Seelie_CheckBox.Checked = Properties.Settings.Default.FormatSeelie;

			Weapons_CheckBox.Checked = Properties.Settings.Default.ScanWeapons;
			Artifacts_Checkbox.Checked = Properties.Settings.Default.ScanArtifacts;
			Characters_CheckBox.Checked = Properties.Settings.Default.ScanCharacters;
			Materials_CheckBox.Checked = Properties.Settings.Default.ScanMaterials;

			ScannerDelay_TrackBar.Value = Properties.Settings.Default.ScannerDelay;

			OutputPath_TextBox.Text = Properties.Settings.Default.OutputPath;
			if (!Directory.Exists(OutputPath_TextBox.Text))
			{
				OutputPath_TextBox.Text = Directory.GetCurrentDirectory() + "\\GenshinData";
			}

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

			Database_MenuItem.Text = Properties.Settings.Default.OldDatabase;
		}

		private void SaveSettings()
		{
			Properties.Settings.Default.FormatGood = GOOD_CheckBox.Checked;
			Properties.Settings.Default.FormatSeelie = Seelie_CheckBox.Checked;

			Properties.Settings.Default.ScanWeapons = Weapons_CheckBox.Checked;
			Properties.Settings.Default.ScanArtifacts = Artifacts_Checkbox.Checked;
			Properties.Settings.Default.ScanCharacters = Characters_CheckBox.Checked;
			Properties.Settings.Default.ScanMaterials = Materials_CheckBox.Checked;

			Properties.Settings.Default.ScannerDelay = ScannerDelay_TrackBar.Value;

			if (Directory.Exists(OutputPath_TextBox.Text))
			{
				Properties.Settings.Default.OutputPath = OutputPath_TextBox.Text;
			}

			Properties.Settings.Default.InventoryKey = (int)Navigation.inventoryKey;
			Properties.Settings.Default.CharacterKey = (int)Navigation.characterKey;

			Properties.Settings.Default.OldDatabase = Database_MenuItem.Text.Trim();

			Properties.Settings.Default.Save();
		}

		private void StartButton_Clicked(object sender, EventArgs e)
		{
			SaveSettings();
			
			UserInterface.SetProgramStatus("Scanning");

			if (Directory.Exists(OutputPath_TextBox.Text))
			{
				UserInterface.ResetAll();
				hook.RegisterHotKey(Keys.Enter);
				if (running)
				{
					Debug.WriteLine("Already running");
					ResetUI();
					return;
				}

				running = true;
				mainThread = new Thread(() =>
				{
					try
					{
						// Get Screen Location and Size
						Navigation.Initialize("GenshinImpact");

						List<Size> sizes = new List<Size>
						{ 
							new Size(16,9),
							new Size(8,5),
						};

						if (!sizes.Contains(Navigation.GetAspectRatio()))
						{
							throw new Exception($"{Navigation.GetSize()} is an invalid resolution.");
						}

						// Add navigation delay
						Navigation.SetDelay(ScannerDelayValue(Delay));

						// Create boolean array
						bool[] checkbox = new bool[3];
						checkbox[0] = WeaponsChecked;
						checkbox[1] = ArtifactsChecked;
						checkbox[2] = CharactersChecked;

						// The Data object of json object
						data.GatherData(checkbox);

						// Covert to GOOD format
						GOOD good = new GOOD(data);

						// Make Json File
						good.WriteToJSON(OutputPath_TextBox.Text, Database_MenuItem.Text);

						UserInterface.SetProgramStatus("Finished");
					}
					catch (ThreadAbortException)
					{ }
					catch (Exception ex)
					{
						Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}\n");
						UserInterface.AddError($"{ex.Message}"); 
						UserInterface.AddError($"{ex.StackTrace}");
					}
					finally
					{
						// Clear saved data
						ResetUI();
						running = false;
						Debug.WriteLine("No longer running");
					}
				})
				{
					IsBackground = true
				};
				mainThread.Start();
			}
			else
			{
				UserInterface.AddError("Set Folder Location");
			}
		}

		private void Github_Label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://github.com/Andrewthe13th/Genshin_Scanner/");
		}

		private void Releases_Label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://github.com/Andrewthe13th/Genshin_Scanner/releases");
		}

		private void IssuesPage_Label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://github.com/Andrewthe13th/Genshin_Scanner/issues");
		}

		private void FileSelectButton_Click(object sender, EventArgs e)
		{
			// A nicer file browser
			CommonOpenFileDialog d = new CommonOpenFileDialog
			{
				InitialDirectory = filePath,
				IsFolderPicker = true
			};

			if (d.ShowDialog() == CommonFileDialogResult.Ok)
			{
				OutputPath_TextBox.Text = d.FileName;
				filePath = d.FileName;
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveSettings();
			hook.Dispose();
		}

		private void SaveSettings(object sender, EventArgs e)
		{
			SaveSettings();
		}

		private void Weapons_CheckBox_CheckedChanged(object sender, EventArgs e)
		{
			WeaponsChecked = ( (CheckBox)sender ).Checked;
		}

		private void Artifacts_Checkbox_CheckedChanged(object sender, EventArgs e)
		{
			ArtifactsChecked = ( (CheckBox)sender ).Checked;
		}

		private void Characters_CheckBox_CheckedChanged(object sender, EventArgs e)
		{
			CharactersChecked = ( (CheckBox)sender ).Checked;
		}

		private void ScannerDelay_TrackBar_ValueChanged(object sender, EventArgs e)
		{
			Delay = ( (TrackBar)sender ).Value;
		}

		private void Exit_MenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void DatabaseMenuItem_Click(object sender, EventArgs e)
		{
			CommonOpenFileDialog d = new CommonOpenFileDialog
			{
				InitialDirectory = Database_MenuItem.Text,
			};

			d.Filters.Add(new CommonFileDialogFilter("JSON Files", ".json"));

			if (d.ShowDialog() == CommonFileDialogResult.Ok)
			{
				Database_MenuItem.Text = d.FileName;
			}
		}

		private void OptionsMenuItem_KeyDown(object sender, KeyEventArgs e)
		{
			// Stops windows from making pinging sound
			e.SuppressKeyPress = true;

			// Menu, Esc, Alt, PrintScreen, ScrollLock, and NumLock are unable to be used
			VirtualKeyCode[] invalidKeys = new VirtualKeyCode[]{VirtualKeyCode.MENU, VirtualKeyCode.ESCAPE, VirtualKeyCode.SNAPSHOT, VirtualKeyCode.SCROLL, VirtualKeyCode.NUMLOCK };

			if (invalidKeys.Contains((VirtualKeyCode)e.KeyCode) || e.Alt)
			{
				return;
			}

			// Virtual keys for 0-9, A-Z
			bool vk = e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.Z;
			// Numpad keys and function keys (internally accepts up to F24)
			bool np = e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.F24;
			// OEM keys (Keys that vary depending on keyboard layout)
			bool oem = e.KeyCode >= Keys.Oem1 && e.KeyCode <= Keys.Oem7;
			// Arrow keys, spacebar, INS, DEL, HOME, END, PAGEUP, PAGEDOWN
			bool misc = e.KeyCode == Keys.Space || (e.KeyCode >= Keys.Left && e.KeyCode <= Keys.Down) || (e.KeyCode >= Keys.Prior && e.KeyCode <= Keys.Home) || e.KeyCode == Keys.Insert || e.KeyCode == Keys.Delete;

			// Validate that key is an acceptable Genshin keybind.
			if (!vk && !np && !oem && !misc)
			{
				Debug.WriteLine($"Invalid {e.KeyCode} key pressed");
				return;
			}
			ToolStripTextBox s = (ToolStripTextBox)sender;

			// Needed to differentiate between NUMPAD numbers and numbers at top of keyboard
			s.Text = np ? new KeysConverter().ConvertToString(e.KeyCode) : KeyCodeToUnicode(e.KeyData);

			// Spacebar or upper navigation keys (INSERT-PAGEDOWN keys) make textbox empty
			if (s.Text.Equals("") || s.Text.Equals(" "))
			{
				s.Text = new KeysConverter().ConvertToString(e.KeyCode);
			}

			switch (s.Tag)
			{
				case "InventoryKey":
					Navigation.inventoryKey = (VirtualKeyCode)e.KeyCode;
					Debug.WriteLine($"Inv key set to: {Navigation.inventoryKey}");
					break;

				case "CharacterKey":
					Navigation.characterKey = (VirtualKeyCode)e.KeyCode;
					Debug.WriteLine($"Char key set to: {Navigation.characterKey}");
					break;

				default:
					break;
			}
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

	}
}