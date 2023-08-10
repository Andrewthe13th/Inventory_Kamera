namespace InventoryKamera.ui
{
    partial class ExecutablesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ProcessListBox = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AddButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.ExecutableListBox = new System.Windows.Forms.ListBox();
            this.DoneButton = new System.Windows.Forms.Button();
            this.ProcessLabel = new System.Windows.Forms.Label();
            this.ExecutableLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProcessListBox
            // 
            this.ProcessListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.ProcessListBox.FormattingEnabled = true;
            this.ProcessListBox.HorizontalScrollbar = true;
            this.ProcessListBox.Location = new System.Drawing.Point(0, 0);
            this.ProcessListBox.Name = "ProcessListBox";
            this.ProcessListBox.Size = new System.Drawing.Size(219, 238);
            this.ProcessListBox.Sorted = true;
            this.ProcessListBox.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.AddButton);
            this.panel1.Controls.Add(this.ProcessListBox);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(219, 262);
            this.panel1.TabIndex = 3;
            // 
            // AddButton
            // 
            this.AddButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddButton.Location = new System.Drawing.Point(0, 238);
            this.AddButton.Margin = new System.Windows.Forms.Padding(10);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(219, 24);
            this.AddButton.TabIndex = 1;
            this.AddButton.Text = "Add Selected";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.RemoveButton);
            this.panel2.Controls.Add(this.ExecutableListBox);
            this.panel2.Location = new System.Drawing.Point(254, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(205, 262);
            this.panel2.TabIndex = 4;
            // 
            // RemoveButton
            // 
            this.RemoveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RemoveButton.Location = new System.Drawing.Point(0, 238);
            this.RemoveButton.Margin = new System.Windows.Forms.Padding(10);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(205, 24);
            this.RemoveButton.TabIndex = 1;
            this.RemoveButton.Text = "Remove Selected";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // ExecutableListBox
            // 
            this.ExecutableListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.ExecutableListBox.FormattingEnabled = true;
            this.ExecutableListBox.HorizontalScrollbar = true;
            this.ExecutableListBox.Location = new System.Drawing.Point(0, 0);
            this.ExecutableListBox.Name = "ExecutableListBox";
            this.ExecutableListBox.Size = new System.Drawing.Size(205, 238);
            this.ExecutableListBox.TabIndex = 0;
            // 
            // DoneButton
            // 
            this.DoneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DoneButton.Location = new System.Drawing.Point(384, 309);
            this.DoneButton.Name = "DoneButton";
            this.DoneButton.Size = new System.Drawing.Size(75, 23);
            this.DoneButton.TabIndex = 5;
            this.DoneButton.Text = "Done";
            this.DoneButton.UseVisualStyleBackColor = true;
            this.DoneButton.Click += new System.EventHandler(this.DoneButton_Click);
            // 
            // ProcessLabel
            // 
            this.ProcessLabel.AutoSize = true;
            this.ProcessLabel.Location = new System.Drawing.Point(67, 284);
            this.ProcessLabel.Name = "ProcessLabel";
            this.ProcessLabel.Size = new System.Drawing.Size(102, 13);
            this.ProcessLabel.TabIndex = 6;
            this.ProcessLabel.Text = "Available Processes";
            // 
            // ExecutableLabel
            // 
            this.ExecutableLabel.AutoSize = true;
            this.ExecutableLabel.Location = new System.Drawing.Point(301, 284);
            this.ExecutableLabel.Name = "ExecutableLabel";
            this.ExecutableLabel.Size = new System.Drawing.Size(107, 13);
            this.ExecutableLabel.TabIndex = 7;
            this.ExecutableLabel.Text = "Genshin Executables";
            // 
            // ExecutablesForm
            // 
            this.AcceptButton = this.DoneButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(475, 344);
            this.Controls.Add(this.ExecutableLabel);
            this.Controls.Add(this.ProcessLabel);
            this.Controls.Add(this.DoneButton);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExecutablesForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Executable Chooser";
            this.Load += new System.EventHandler(this.ExecutablesForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ProcessListBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.ListBox ExecutableListBox;
        private System.Windows.Forms.Button DoneButton;
        private System.Windows.Forms.Label ProcessLabel;
        private System.Windows.Forms.Label ExecutableLabel;
    }
}