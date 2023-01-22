using InventoryKamera.ui.main;
using NLog;
using System;
using System.Windows.Forms;

namespace InventoryKamera
{
	internal static class Program
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			ConfigureLogging();
            try
            {
				if (Environment.OSVersion.Version.Major >= 6)
					SetProcessDPIAware();

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainForm());
				//Application.Run(new MainUI());
            }
            catch (Exception ex)
            {
				Logger.Fatal(ex, "Main application crashed");
				MessageBox.Show("Inventory Kamera has encountered an error it was not meant to handle." +
					" Please check the application's log and/or upload it to GitHub when reporting your issue. Thanks!",
					"Application Crashed",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
			Logger.Info("Application closed");
		}

        private static void ConfigureLogging()
        {
            var config = new NLog.Config.LoggingConfiguration();

			var debugFile = new NLog.Targets.FileTarget("logfile")
			{
				Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss.fff}|${level:uppercase=true}|${logger:shortName=True}|${message:withexception=true}",
				FileName = "./logging/InventoryKamera.debug.log",
				ArchiveFileName = "logging/archives/InventoryKamera.{####}.debug.log",
				ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Date,
				ArchiveDateFormat = "yyyyMMddHHmmss",
				MaxArchiveFiles = 4,
				ConcurrentWrites = true,
				KeepFileOpen = true,
				ArchiveOldFileOnStartup = true,
				ArchiveFileKind = NLog.Targets.FilePathKind.Relative
			};

			var logFile = new NLog.Targets.FileTarget("logfile")
			{
				Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss.fff}|${level:uppercase=true}|${logger:shortName=True}|${message:withexception=true}",
				FileName = "./logging/InventoryKamera.log",
				ArchiveFileName = "logging/archives/InventoryKamera.{####}.log",
				ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Date,
				ArchiveDateFormat = "yyyyMMddHHmmss",
				MaxArchiveFiles = 4,
				ConcurrentWrites = true,
				KeepFileOpen = true,
				ArchiveOldFileOnStartup = true,
				ArchiveFileKind = NLog.Targets.FilePathKind.Relative
			};

			var logConsole = new NLog.Targets.ConsoleTarget("logconsole")
			{
				Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss.fff}|${level:uppercase=true}|${logger:shortName=True}|${message:withexception=true}",
			};

			config.AddRule(LogLevel.Debug, LogLevel.Fatal, logConsole);
			config.AddRule(LogLevel.Debug, LogLevel.Fatal, debugFile);
			config.AddRule(LogLevel.Info, LogLevel.Fatal, logFile);

            LogManager.Configuration = config;
        }

        // ***also dllimport of that function***
        [System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern bool SetProcessDPIAware();
	}
}