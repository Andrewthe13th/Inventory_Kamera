using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Input;
using System.Threading;


namespace GenshinGuide
{
    public partial class Form1 : Form
    {
        //private KeyHandler ghk;
        static Thread mainThread;
        static GenshinData data = new GenshinData();
        static public KeyboardHook hook = new KeyboardHook();
        static string filePath = "";
        static int delayLevel = 0;
        // Checkbox for scanner
        static bool bWeapons = true;
        static bool bArtifacts = true;
        static bool bCharacters = true;


		public Form1()
		{
			InitializeComponent();

			// register the event that is fired after the key press.
			hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(Hook_KeyPressed);
			

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

            // Initize the file path
            FilePath.Text = "";
            filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if(filePath == "")
            {
                filePath = "";
                FilePath.Text = "Select Folder";
            }
            else
            {
                filePath = filePath + "\\GenshinData";
                FilePath.Text = filePath;
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
				data.StopImageProcessorWorker();
				UserInterface.SetProgramStatus("Scan Stopped");
				// Reset data
				data = new GenshinData();
				Navigation.Reset();
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

        private void button1_Click(object sender, EventArgs e)
        {
            UserInterface.SetProgramStatus("Scanning");

            if (filePath != "")
            {

                UserInterface.Reset_All();
                hook.RegisterHotKey(Keys.Enter);
                

                mainThread = new Thread(() =>
                {

                    // Get Screen Location and Size
                    Navigation.Initialize("GenshinImpact");

                    // Add navigation delay
                    int delay = ScannerDelayValue(delayLevel);
                    Navigation.AddDelay(delay);

                    // Create boolean array
                    bool[] checkbox = new bool[3];
                    checkbox[0] = bWeapons;
                    checkbox[1] = bArtifacts;
                    checkbox[2] = bCharacters;

                    // check if screen size is 1280 x 720
                    if (Navigation.GetWidth() == 1280 && Navigation.GetHeight() == 720)
                    {

                        // The Data object of json object
                        data.GatherData(checkbox);

                        // Covert to GOOD format
                        GOOD good = new GOOD(data);

							// Make Json File
							good.WriteToJSON(OutputPath_TextBox.Text, Database_MenuItem.Text);

                        // Clear saved data
                        ResetUI();

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
					catch (ThreadAbortException)
					{ }
					catch (Exception ex)
					{
						Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}\n");
						UserInterface.AddError($"{ex.Message}\n{ ex.StackTrace}\n");
					}
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

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Andrewthe13th/Genshin_Scanner/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Andrewthe13th/Genshin_Scanner/issues");
        }

        private void label17_Click_1(object sender, EventArgs e)
        {

        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Andrewthe13th/Genshin_Scanner/releases");
        }

        private void ProgramStatus_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //MessageBox.Show(fbd.SelectedPath);
                FilePath.Text = fbd.SelectedPath;
                filePath = fbd.SelectedPath;
            }
        }

        private void label28_Click(object sender, EventArgs e)
        {

        }
    }



		private void databaseMenuItem_Click(object sender, EventArgs e)
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
	}
}
