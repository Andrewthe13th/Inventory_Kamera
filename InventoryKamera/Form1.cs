using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using NHotkey;
using NHotkey.WindowsForms;
using WindowsInput.Native;

namespace InventoryKamera
{
	public partial class Form1 : Form
	{
		private static Thread mainThread;
		private static InventoryKamera data = new InventoryKamera();
		private static DatabaseManager databaseManager = new DatabaseManager();
		private static string filePath = "";

		private bool GOODChecked;
		private bool SeelieChecked;

		private bool WeaponsChecked;
		private bool ArtifactsChecked;
		private bool CharactersChecked;
		private bool MaterialsChecked;
		private bool CharDevItemsChecked;

		private int Delay;

		private bool running = false;

		public Form1()
		{
			InitializeComponent();

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

		private void Hotkey_Pressed(object sender, HotkeyEventArgs e)
		{
			e.Handled = true;
			// Check if scanner is running
			if (mainThread.IsAlive)
			{
				// Stop navigating weapons/artifacts
				mainThread.Abort();

				UserInterface.SetProgramStatus("Scan Stopped");

				Navigation.Reset();
			}
		}

		private void ResetUI()
		{
			// Reset data
			data = new InventoryKamera();

			Navigation.Reset();

			// Need to invoke method from the UI's handle, not the worker thread
			BeginInvoke((MethodInvoker)delegate { RemoveHotkey(); });
		}

		private void RemoveHotkey()
		{
			HotkeyManager.Current.Remove("Stop");
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
			UpdateKeyTextBoxes();

			GOODChecked = GOOD_CheckBox.Checked;
			SeelieChecked = Seelie_CheckBox.Checked;

			WeaponsChecked = Weapons_CheckBox.Checked;
			ArtifactsChecked = Artifacts_Checkbox.Checked;
			CharactersChecked = Characters_CheckBox.Checked;
			CharDevItemsChecked = CharDevItems_CheckBox.Checked;
			MaterialsChecked = Materials_CheckBox.Checked;

			Delay = ScannerDelay_TrackBar.Value;
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
			SaveSettings();

			UserInterface.ResetAll();

			UserInterface.SetProgramStatus("Scanning");

			if (Directory.Exists(OutputPath_TextBox.Text))
			{
				if (running)
				{
					Debug.WriteLine("Already running");
					return;
				}
				running = true;

				// Create boolean array
				bool[] items = new bool[5];
				items[0] = WeaponsChecked;
				items[1] = ArtifactsChecked;
				items[2] = CharactersChecked;
				items[3] = CharDevItemsChecked;
				items[4] = MaterialsChecked;

				bool[] formats = new bool[2];
				formats[0] = GOODChecked;
				formats[1] = SeelieChecked;

				HotkeyManager.Current.AddOrReplace("Stop", Keys.Enter, Hotkey_Pressed);

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
							throw new NotImplementedException($"{Navigation.GetSize().Width}x{Navigation.GetSize().Height} is an unsupported resolution.");
						}

						// Add navigation delay
						Navigation.SetDelay(ScannerDelayValue(Delay));

						// The Data object of json object
						data.GatherData(formats, items);

						// Covert to GOOD
						GOOD good = new GOOD(data);

						// Make Json File
						good.WriteToJSON(OutputPath_TextBox.Text);
						
						UserInterface.SetProgramStatus("Finished");
					}
					catch (ThreadAbortException)
					{
						// Workers can get stuck if the thread is aborted or an exception is raised
						data.StopImageProcessorWorkers();
						UserInterface.SetProgramStatus("Scan stopped");
					}
					catch (NotImplementedException ex)
					{
						UserInterface.AddError(ex.Message);
					}
					catch (Exception ex)
					{
						// Workers can get stuck if the thread is aborted or an exception is raised
						data.StopImageProcessorWorkers();
						Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}\n");
						UserInterface.AddError($"{ex.Message}" + Environment.NewLine + $"{ex.StackTrace}");
						UserInterface.SetProgramStatus("Scan aborted", ok: false);
					}
					finally
					{
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
			RemoveHotkey();
		}

		private void SaveSettings()
		{
			Properties.Settings.Default.Save();
		}

		private void Format_CheckboxClick(object sender, EventArgs e)
		{
			CheckBox box = (CheckBox) sender;
			if (box.Name.Contains("GOOD") && Seelie_CheckBox.Checked)
			{
				box.Checked = !box.Checked;
				//Seelie_CheckBox.Enabled = !Seelie_CheckBox.Enabled;
				//Weapons_CheckBox.Enabled = Weapons_CheckBox.Enabled ? false : true ;
				//Artifacts_Checkbox.Enabled = !Artifacts_Checkbox.Enabled;
				//Characters_CheckBox.Enabled = !Characters_CheckBox.Enabled;
			}
			else if (box.Name.Contains("Seelie") && GOOD_CheckBox.Checked)
			{
				box.Checked = !box.Checked;
				//GOOD_CheckBox.Enabled = !GOOD_CheckBox.Enabled;
				//CharDevItems_CheckBox.Enabled = !CharDevItems_CheckBox.Enabled;
				//Materials_CheckBox.Enabled = !Materials_CheckBox.Enabled;
			}
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

		private void CharDevItems_CheckBox_CheckedChanged(object sender, EventArgs e)
		{
			CharDevItemsChecked = ( (CheckBox)sender ).Checked;
		}

		private void Materials_CheckBox_CheckedChanged(object sender, EventArgs e)
		{
			MaterialsChecked = ( (CheckBox)sender ).Checked;
		}

		private void ScannerDelay_TrackBar_ValueChanged(object sender, EventArgs e)
		{
			Delay = ( (TrackBar)sender ).Value;
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
				Debug.WriteLine($"Invalid {e.KeyCode} key pressed");
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
	}
}