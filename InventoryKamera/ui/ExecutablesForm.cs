using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace InventoryKamera.ui
{
    public partial class ExecutablesForm : Form
    {
        public ExecutablesForm()
        {
            InitializeComponent();
        }

        private void ExecutablesForm_Load(object sender, EventArgs e)
        {
            PopulateExecutables();
            PopulateProcesses();

        }

        private void PopulateProcesses()
        {
            var processes = new HashSet<string>();

            Process.GetProcesses().ToList().ForEach(p => processes.Add(p.ProcessName));

            ProcessListBox.Items.Clear();
            foreach (var process in processes)
            {
                if (Properties.Settings.Default.Executables.Contains(process)) continue;

                ProcessListBox.Items.Add(process);
            }
        }

        private void PopulateExecutables()
        {
            ExecutableListBox.Items.Clear();
            foreach (var executable in Properties.Settings.Default.Executables)
            {
                ExecutableListBox.Items.Add(executable);
            }
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string selected = (string) ProcessListBox.SelectedItem;
            if (selected != null) 
            { 
                if (!Properties.Settings.Default.Executables.Contains(selected))
                {
                    Properties.Settings.Default.Executables.Add(selected);
                    PopulateExecutables();
                    PopulateProcesses();
                }
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            string selected = (string) ExecutableListBox.SelectedItem;
            if (selected != null) 
            {
                Properties.Settings.Default.Executables.Remove(selected);
                PopulateExecutables();
                PopulateProcesses();
            }
        }
    }
}
