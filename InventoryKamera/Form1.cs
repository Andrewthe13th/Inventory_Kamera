using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Input;
using System.Threading;
using System.Linq;
using System.Collections.Generic;


namespace InventoryKamera
{
    public partial class Form1 : Form
    {
        //private KeyHandler ghk;
        static Thread mainThread;
        static GenshinData data = new GenshinData();
        static public KeyboardHook hook = new KeyboardHook();
        static string filePath = "";
        static int delayLevel = 0;


        public Form1()
        {
            InitializeComponent();
            //ghk = new KeyHandler(Keys.Enter, this);
            //ghk.Register();
            // register the event that is fired after the key press.
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            // register the control + alt + F12 combination as hot key.
            //hook.RegisterHotKey(Keys.Enter);
            comboBox1.SelectedItem = "ENG";
            UserInterface.Init(a_GearSlot_Image,a_MainStat_Image,a_Level_Image,(new []{a_SubStat1_Image,a_SubStat2_Image,a_SubStat3_Image,a_SubStat4_Image }),a_SetName_Image,a_Equipped_Image,a_TextBox,c_Name_Image,c_Level_Image,(new[] { c_Talent_1,c_Talent_2,c_Talent_3}), c_TextBox,weaponCount,maxWeapons,artifactCount,maxArtifacts,characterCount,ProgramStatus,error_TextBox, navigation_Image);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

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

            // Initize the delay timer
            delayLevel = ScannerDelay.Value;
            ScannerDelay.ValueChanged += trackbar_ValueChanged;

            // Set all checkbox set
            for(int i = 0; i < good_checkedListBox.Items.Count; i++)
            {
                good_checkedListBox.SetItemCheckState(i, CheckState.Checked);
            }

            for (int i = 0; i < seelie_checkedListBox.Items.Count; i++)
            {
                seelie_checkedListBox.SetItemCheckState(i, CheckState.Checked);
            }

            // Add function to update values for checkboxes for threads
            //w_CheckBox.CheckedChanged += W_Checkbox_ValueChanged;
            //a_CheckBox.CheckedChanged += A_Checkbox_ValueChanged;
            //c_CheckBox.CheckedChanged += C_Checkbox_ValueChanged;
            //cd_CheckBox.CheckedChanged += CD_Checkbox_ValueChanged;
            //m_CheckBox.CheckedChanged += M_Checkbox_ValueChanged;
        }

        private void trackbar_ValueChanged(object sender, EventArgs e)
        {
            delayLevel = ScannerDelay.Value;
        }

        private void W_Checkbox_ValueChanged(object sender, EventArgs e)
        {
            //bWeapons = w_CheckBox.Checked;
        }

        private void A_Checkbox_ValueChanged(object sender, EventArgs e)
        {
            //bArtifacts = a_CheckBox.Checked;
        }

        private void C_Checkbox_ValueChanged(object sender, EventArgs e)
        {
            //bCharacters = c_CheckBox.Checked;
        }

        private void CD_Checkbox_ValueChanged(object sender, EventArgs e)
        {
            //bCharacterDevelopmentItems = cd_CheckBox.Checked;
        }

        private void M_Checkbox_ValueChanged(object sender, EventArgs e)
        {
            //bMaterials = m_CheckBox.Checked;
        }

        private int ScannerDelayValue(int value)
        {
            if(value == 0)
            {
                return 0;
            }
            else if(value == 1)
            {
                return 50;
            }
            else if(value == 2)
            {
                return 100;
            }
            else if (value == 3)
            {
                return 200;
            }
            else
            {
                return 100;
            }
        }

        private void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            // show the keys pressed in a label.
            if (mainThread.IsAlive)
            {
                // stop navigating throw weapons/artifacts
                mainThread.Abort();
                // stop weapon/artifact processor thread
                data.StopImageProcessorWorker();
                UserInterface.SetProgramStatus("Scan Stopped",true);
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

                    // Get data format
                    string dataFormat = "";
                    foreach (RadioButton rdo in DataFormat.Controls.OfType<RadioButton>())
                    {
                        if (rdo.Checked)
                        {
                            dataFormat = rdo.Text;
                            break;
                        }
                    }

                    // Create boolean array
                    List<bool> checkbox = new List<bool>();
                    if(dataFormat == "GOOD")
                    {
                        for(int i = 0; i < good_checkedListBox.Items.Count; i++)
                        {
                            CheckState check = good_checkedListBox.GetItemCheckState(i);
                            if (check == CheckState.Checked)
                            {
                                checkbox.Add(true);
                            }
                            else
                            {
                                checkbox.Add(false);
                            }
                        }
                    }
                    else if (dataFormat == "Seelie")
                    {
                        for (int i = 0; i < seelie_checkedListBox.Items.Count; i++)
                        {
                            CheckState check = seelie_checkedListBox.GetItemCheckState(i);
                            if (check == CheckState.Checked)
                            {
                                checkbox.Add(true);
                            }
                            else
                            {
                                checkbox.Add(false);
                            }
                        }
                    }

                    // check if screen size is 1280 x 720
                    if (Navigation.GetWidth() == 1280 && Navigation.GetHeight() == 720)
                    {

                        // The Data object of json object
                        data.GatherData(dataFormat,checkbox);

                        object format = new object();

                        if(dataFormat == "GOOD")
                        {
                            // Covert to GOOD format
                            format = new GOOD(data);
                        }
                        else if(dataFormat == "Seelie")
                        {
                            // Covert to Seelie format
                            format = new Seelie(data);
                        }

                        // Make Json File
                        Scraper.CreateJsonFile(dataFormat, format, filePath, checkbox);

                        // Clear saved data
                        ResetUI();

                        // Open GenshinDataFolder
                        Process.Start("explorer.exe", filePath);
                    }
                    else
                    {
                        data = new GenshinData();
                        UserInterface.AddError("Game Window not set to 1280 x 720 Windowed");
                        Navigation.Reset();
                    // Un register ENTER key
                    hook.Dispose();
                    }

                });
                mainThread.IsBackground = true;
                mainThread.Start();
            }
            else
            {
                UserInterface.SetProgramStatus("Set Folder Location", true);
            }

        }

        private void txtOutput_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void picTarget_Click(object sender, EventArgs e)
        {

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
            System.Diagnostics.Process.Start("https://github.com/Andrewthe13th/Inventory_Kamera");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Andrewthe13th/Inventory_Kamera/issues");
        }

        private void label17_Click_1(object sender, EventArgs e)
        {

        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Andrewthe13th/Inventory_Kamera/releases");
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

        private void seelie_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            good_checkedListBox.Visible = false;
            seelie_checkedListBox.Visible = true;
        }

        private void good_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            good_checkedListBox.Visible = true;
            seelie_checkedListBox.Visible = false;
        }
    }



}
