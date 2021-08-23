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
        private KeyHandler ghk;
        private bool bStopExecution = false;

        public Form1()
        {
            InitializeComponent();
            ghk = new KeyHandler(Keys.Enter, this);
            ghk.Register();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            bStopExecution = false;
            bool threadCompletion = false;

            Thread th = new Thread(() =>
            {

                // Get Screen Location and Size
                Navigation.Initialize("GenshinImpact");

                // Global space to refer to form elements
                UserInterface.Init(picTarget, txtOutput);

                // The Data object of json object
                GenshinData data = new GenshinData();
                data.GatherData();

                // Make Json File
                Scraper.CreateJsonFile(data);

                // End Thread and finish
                threadCompletion = true;
            });

            th.Start();

            while (!threadCompletion)
            {
                //Update Image and Text Box from threads
                Application.DoEvents();
                // End program when pressing 'Enter'
                if (bStopExecution)
                {
                    th.Abort();
                    Navigation.Reset();
                }
            }

            th.Join();

            //// Get Screen Location and Size
            //Navigation.Initialize("GenshinImpact");

            //// Global space to refer to form elements
            //UserInterface.Init(picTarget, txtOutput);

            //// The Data structure of json object
            //GenshinData data = new GenshinData();
            //data.GatherData();

            //// Make File
            //Scraper.CreateJsonFile(data);

            //End Program
            System.Windows.Forms.Application.Exit();

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
    }



}
