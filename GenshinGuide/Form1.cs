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
        private bool bStopExecution = false;
        Thread mainThread;

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
        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            // show the keys pressed in a label.
            //txtOutput.Text = e.Modifier.ToString() + " + " + e.Key.ToString();
            if (mainThread.IsAlive)
            {
                mainThread.Abort();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            bStopExecution = false;
            bool threadCompletion = false;

            mainThread = new Thread(() =>
            {

                // Get Screen Location and Size
                Navigation.Initialize("GenshinImpact");

                // Global space to refer to form elements
#if DEBUG
                UserInterface.Init(picTarget, txtOutput);
#endif

                // The Data object of json object
                GenshinData data = new GenshinData();
                data.GatherData();

                // Covert to GOOD format
                GOOD good = new GOOD(data);

                // Make Json File
                Scraper.CreateJsonFile(good);


                // Open GenshinDataFolder
                Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GenshinData");

                // End Thread and finish
                threadCompletion = true;
            });
            mainThread.IsBackground = true;
            mainThread.Start();

#if DEBUG
            if (Scraper.s_bDoDebugOnlyCode)
            {
                while (!threadCompletion)
                {

                    // End program when pressing 'Enter'
                    if (bStopExecution)
                    {
                        mainThread.Abort();
                        Navigation.Reset();
                        GenshinData data = new GenshinData();
                    }

                    //Update Image and Text Box from threads
                    Application.DoEvents();
                }
                mainThread.Join();
                System.Windows.Forms.Application.Exit();
            }

#endif

            //while (Console.ReadKey(true).Key != ConsoleKey.Enter) { };

            //th.Join();
            //System.Windows.Forms.Application.Exit();

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
            bStopExecution = true;
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
    }



}
