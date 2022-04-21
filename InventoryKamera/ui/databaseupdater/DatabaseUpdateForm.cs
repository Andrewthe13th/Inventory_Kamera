using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryKamera
{
	public partial class DatabaseUpdateForm : Form
	{
		private List<CheckBox> checkBoxes = new List<CheckBox>();
		private List<ProgressBar> progressBars = new List<ProgressBar>();
		private DatabaseManager databaseManager = new DatabaseManager();

		private bool processing = false;

		public DatabaseUpdateForm()
		{
			InitializeComponent();
			checkBoxes.Add(CharactersCheckBox);
			checkBoxes.Add(WeaponsCheckBox);
			checkBoxes.Add(ArtifactsCheckBox);
			checkBoxes.Add(DevMaterialsCheckBox);
			checkBoxes.Add(MaterialsCheckBox);
			checkBoxes.Add(AllMaterialsCheckBox);

			progressBars.Add(CharactersProgressBar);
			progressBars.Add(WeaponsProgresBar);
			progressBars.Add(ArtifactsProgressBar);
			progressBars.Add(DevMaterialsProgressBar);
			progressBars.Add(MaterialsProgressBar);
			progressBars.Add(AllMaterialsProgressBar);

			ToolTip.SetToolTip(CreateNewCheckBox, "Create new selected files");
		}

		private async void UpdateButton_Click(object sender, EventArgs e)
		{
			processing = true;

			databaseManager = new DatabaseManager();
			Task<bool> updateTask;
			UpdateStatusLabel.Text = "";


			if (EverythingCheckBox.Checked)
			{
				updateTask = Task.Run(() => { return databaseManager.UpdateAllLists(CreateNewCheckBox.Checked); });
			}
			else
			{
				var lists = new List<ListType>();
				for (int i = 0; i < checkBoxes.Count; i++)
				{
					if (checkBoxes[i].Checked)
					{
						lists.Add((ListType)i);
					}
				}

				if (lists.Count == 0) { processing = false; return; }

				updateTask = Task.Run(() => { return databaseManager.UpdateLists(lists, CreateNewCheckBox.Checked); });
			}
			await updateTask;

			processing = false;

			switch (updateTask.Result) // Maybe change to progress bars in the future?
			{
				case true:
					UpdateStatusLabel.ForeColor = Color.Green;
					UpdateStatusLabel.Text = "Updated lists";
					Properties.Settings.Default.LastUpdateCheck = DateTime.Now.TimeOfDay;
					Properties.Settings.Default.Save();
					break;

				default:
					UpdateStatusLabel.ForeColor = Color.Red;
					UpdateStatusLabel.Text = "Could not update lists";
					break;
			}
		}

		private void EverythingCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			CharactersCheckBox.Enabled = !EverythingCheckBox.Checked;
			WeaponsCheckBox.Enabled = !EverythingCheckBox.Checked;
			ArtifactsCheckBox.Enabled = !EverythingCheckBox.Checked;
			DevMaterialsCheckBox.Enabled = !EverythingCheckBox.Checked;
			MaterialsCheckBox.Enabled = !EverythingCheckBox.Checked;
			AllMaterialsCheckBox.Enabled = !EverythingCheckBox.Checked;
		}

		private void DatabaseUpdateForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			GC.Collect();
		}

		private void DatabaseUpdateForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = processing;
		}

		private void OpenFolderButton_Click(object sender, EventArgs e)
		{
			Process.Start("explorer.exe", $"{databaseManager.ListsDir}");
		}
	}
}