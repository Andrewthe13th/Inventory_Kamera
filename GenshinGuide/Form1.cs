using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace GenshinGuide
{
	public partial class Form1 : Form
	{
		//private KeyHandler ghk;
		static Thread mainThread;
		static GenshinData data = new GenshinData();
		static public KeyboardHook hook = new KeyboardHook();
		static string filePath = "";

		private int Delay;
		private bool WeaponsChecked;
		private bool ArtifactsChecked;
		private bool CharactersChecked;

		public Form1()
		{
			InitializeComponent();
			//ghk = new KeyHandler(Keys.Enter, this);
			//ghk.Register();
			// register the event that is fired after the key press.
			hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(Hook_KeyPressed);
			// register the control + alt + F12 combination as hot key.
			//hook.RegisterHotKey(Keys.Enter);
			Language_ComboBox.SelectedItem = "ENG";
			UserInterface.Init(GearSlot_PictureBox,
					  ArtifactMainStat_PictureBox,
					  Level_PictureBox,
					  new[] { ArtifactSubStat1_PictureBox, ArtifactSubStat2_PictureBox, ArtifactSubStat3_PictureBox, ArtifactSubStat4_PictureBox },
					  ArtifactSetName_PictureBox,
					  ArtifactEquipped_PictureBox,
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
			if (value == 0)
			{
				return 0;
			}
			else if (value == 1)
			{
				return 50;
			}
			else if (value == 2)
			{
				return 100;
			}
			else
			{
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
				data.StopImageProcessorWorker();
				UserInterface.SetProgramStatus("Scan Stopped");
				// Reset data
				data = new GenshinData();
				Navigation.Reset();
				// Un register ENTER key
				//hook.Dispose();
			}
			else
			{
				hook.Dispose();
			}
		}

		private void ResetUI()
		{
			// Reset data
			data = new GenshinData();
			Navigation.Reset();
			// Un-register ENTER key. Otherwise you can't hit ENTER in another application
			hook.Dispose();
		}

		public static void UnexpectedError(string error)
		{
			if (mainThread.IsAlive)
			{
				//data.StopImageProcessorWorker();
				//mainThread.Abort();
				UserInterface.AddError(error);
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			GetSettings();

			Delay = ScannerDelay_TrackBar.Value;

			WeaponsChecked = Weapons_CheckBox.Checked;
			ArtifactsChecked = Artifacts_Checkbox.Checked;
			CharactersChecked = Characters_CheckBox.Checked;
		}

		private void GetSettings()
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

				mainThread = new Thread(() =>
				{

					try
					{
						// Get Screen Location and Size
						Navigation.Initialize("GenshinImpact");

						// Add navigation delay

						Navigation.AddDelay(ScannerDelayValue(Delay));

						// Create boolean array
						bool[] checkbox = new bool[3];
						checkbox[0] = WeaponsChecked;
						checkbox[1] = ArtifactsChecked;
						checkbox[2] = CharactersChecked;

						// check if screen size is 1280 x 720
						if (Navigation.GetWidth() == 1280 && Navigation.GetHeight() == 720)
						{
							// The Data object of json object
							data.GatherData(checkbox);

							// Covert to GOOD format
							GOOD good = new GOOD(data);

							// Make Json File
							Scraper.WriteToJSON(good, OutputPath_TextBox.Text);

							// Open GenshinDataFolder
							Process.Start("explorer.exe", OutputPath_TextBox.Text);

							UserInterface.SetProgramStatus("Finished");
						}
						else
						{
							data = new GenshinData();
							UserInterface.AddError("Game Window not set to 1280 x 720 Windowed");
							Navigation.Reset();
							// Un register ENTER key
							hook.Dispose();
							throw new Exception("Invalid game window size");
						}
					}
					catch (Exception)
					{
						Invoke((MethodInvoker)delegate { ProgramStatus_Label.Text = ""; });
					}
					// If the scanner crashed
					finally
					{
						// Clear saved data
						ResetUI();
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

		private void HandleHotkey()
		{
			// Do stuff...
			Debug.Print("Key down event captured: Enter Key!!");
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
				HandleHotkey();
			base.WndProc(ref m);
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
		}

		private void SaveSettings(object sender, EventArgs e)
		{
			SaveSettings();
		}

		private void Weapons_CheckBox_CheckedChanged(object sender, EventArgs e)
		{
			WeaponsChecked = ((CheckBox)sender).Checked;
		}

		private void Artifacts_Checkbox_CheckedChanged(object sender, EventArgs e)
		{
			ArtifactsChecked = ((CheckBox)sender).Checked;
		}

		private void Characters_CheckBox_CheckedChanged(object sender, EventArgs e)
		{
			CharactersChecked = ((CheckBox)sender).Checked;
		}
	}
}
