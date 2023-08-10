using NHotkey.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryKamera.ui.main
{
    public partial class MainUI : Form
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public MainUI()
        {
            InitializeComponent();

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

            Logger.Info("Inventory Kamera Version {0}", version);
            versionLabel.Text = version;

            Logger.Info("MainForm initialization complete");
        }

        internal void ExitButton_Click(object sender, EventArgs e)
        {
            Logger.Info("Closing applicaiton");
            Application.Exit();
        }

        internal void SaveSettings()
        {
            Properties.Settings.Default.Save();
        }

        private void RemoveHotkey()
        {
            HotkeyManager.Current.Remove("Stop");
        }

        private void MainUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            RemoveHotkey();
            NLog.LogManager.Shutdown();
        }

        internal void Activate_Form()
        {
            BeginInvoke((MethodInvoker)delegate { Activate(); });
        }

        private void Log_Button_Click(object sender, EventArgs e)
        {
            Process.Start($@"logging");
        }

        private void GithubLabelLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Logger.Info("Opening Github...");
            Process.Start("https://github.com/Andrewthe13th/Inventory_Kamera/");
        }
    }
}
