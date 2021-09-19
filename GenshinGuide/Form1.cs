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
        KeyboardHook hook = new KeyboardHook();

        public Form1()
        {
            InitializeComponent();
            //ghk = new KeyHandler(Keys.Enter, this);
            //ghk.Register();
            // register the event that is fired after the key press.
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            // register the control + alt + F12 combination as hot key.
            hook.RegisterHotKey(Keys.Enter);
            comboBox1.SelectedItem = "ENG";
            UserInterface.Init(a_GearSlot_Image,a_MainStat_Image,a_Level_Image,(new []{a_SubStat1_Image,a_SubStat2_Image,a_SubStat3_Image,a_SubStat4_Image }),a_SetName_Image,a_Equipped_Image,a_TextBox,c_Name_Image,c_Level_Image,(new[] { c_Talent_1,c_Talent_2,c_Talent_3}), c_TextBox,weaponCount,maxWeapons,artifactCount,maxArtifacts,characterCount,ProgramStatus);
        }

        private void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            // show the keys pressed in a label.
            //txtOutput.Text = e.Modifier.ToString() + " + " + e.Key.ToString();
            if (mainThread.IsAlive)
            {
                data.StopImageProcessorWorker();
                mainThread.Abort();
                UserInterface.SetProgramStatus("Scan Stopped",true);
            }
        }

        public static void UnexpectedError(string error)
        {
            if (mainThread.IsAlive)
            {
                data.StopImageProcessorWorker();
                mainThread.Abort();
                UserInterface.SetProgramStatus(error, true);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            UserInterface.SetProgramStatus("Scanning");

            mainThread = new Thread(() =>
            {

                // Get Screen Location and Size
                Navigation.Initialize("GenshinImpact");

                // The Data object of json object
                data.GatherData();

                // Covert to GOOD format
                GOOD good = new GOOD(data);

                // Make Json File
                Scraper.CreateJsonFile(good);


                // Open GenshinDataFolder
                Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GenshinData");

                UserInterface.SetProgramStatus("Success");
            });
            mainThread.IsBackground = true;
            mainThread.Start();

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
    }



}
