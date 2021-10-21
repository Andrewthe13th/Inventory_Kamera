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
			UserInterface.Init(ArtifactGearSlot_Image,
					  ArtifactMainStat_Image,
					  ArtifactLevel_Image,
					  new[] { ArtifactSubStat1_Image, ArtifactSubStat2_Image, ArtifactSubStat3_Image, ArtifactSubStat4_Image },
					  ArtifactSetName_Image,
					  ArtifactEquipped_PictureBox,
					  ArtifactOutput_TextBox,
					  CharacterName_Image,
					  CharacterLevel_Image,
					  new[] { CharacterTalent1_Image, CharacterTalent2_Image, CharacterTalent3_Image },
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
				UserInterface.SetProgramStatus("Scan Stopped", true);
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
			UserInterface.SetProgramStatus("Finished");
			// Reset data
			data = new GenshinData();
			Navigation.Reset();
			// Un register ENTER key
			hook.Dispose();
		}

		public static void UnexpectedError(string error)
		{
			if (mainThread.IsAlive)
			{
				//data.StopImageProcessorWorker();
				//mainThread.Abort();
				UserInterface.SetProgramStatus(error, true);
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			GetSettings();
		}

		private void GetSettings()
		{
			Console.WriteLine("Getting Settings");
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
			Console.WriteLine("Saving Settings");
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
			Console.WriteLine("Settings Saved");
		}

		private void StartButton_Clicked(object sender, EventArgs e)
		{
			Console.WriteLine("Started");
			SaveSettings();
			UserInterface.SetProgramStatus("Scanning");

			if (Directory.Exists(OutputPath_TextBox.Text))
			{
				UserInterface.Reset_All();
				hook.RegisterHotKey(Keys.Enter);

				mainThread = new Thread(() =>
				{

					// Get Screen Location and Size
					Navigation.Initialize("GenshinImpact");

					// Add navigation delay
					int delay = ScannerDelayValue(ScannerDelay_TrackBar.Value);
					Navigation.AddDelay(delay);

					// Create boolean array
					bool[] checkbox = new bool[3];
					checkbox[0] = Weapons_CheckBox.Checked;
					checkbox[1] = Artifacts_Checkbox.Checked;
					checkbox[2] = Characters_CheckBox.Checked;

					// check if screen size is 1280 x 720
					if (Navigation.GetWidth() == 1280 && Navigation.GetHeight() == 720)
					{

						// The Data object of json object
						data.GatherData(checkbox);

						// Covert to GOOD format
						GOOD good = new GOOD(data);

						// Make Json File
						Scraper.CreateJsonFile(good, OutputPath_TextBox.Text);

						// Clear saved data
						ResetUI();

						// Open GenshinDataFolder
						Process.Start("explorer.exe", OutputPath_TextBox.Text);
					}
					else
					{
						data = new GenshinData();
						UserInterface.AddError("Game Window not set to 1280 x 720 Windowed");
						Navigation.Reset();
						// Un register ENTER key
						hook.Dispose();
					}

				})
				{
					IsBackground = true
				};
				mainThread.Start();
			}
			else
			{
				UserInterface.SetProgramStatus("Set Folder Location", true);
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
				Console.WriteLine($"Selected: {d.FileName}");
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
	}
}
