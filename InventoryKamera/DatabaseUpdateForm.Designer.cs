
namespace InventoryKamera
{
	partial class DatabaseUpdateForm
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
			if (disposing && ( components != null ))
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseUpdateForm));
			this.InstructionsLabel = new System.Windows.Forms.Label();
			this.UpdateButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.CharactersCheckBox = new System.Windows.Forms.CheckBox();
			this.WeaponsCheckBox = new System.Windows.Forms.CheckBox();
			this.ArtifactsCheckBox = new System.Windows.Forms.CheckBox();
			this.DevMaterialsCheckBox = new System.Windows.Forms.CheckBox();
			this.MaterialsCheckBox = new System.Windows.Forms.CheckBox();
			this.AllMaterialsCheckBox = new System.Windows.Forms.CheckBox();
			this.EverythingCheckBox = new System.Windows.Forms.CheckBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.CharactersProgressBar = new System.Windows.Forms.ProgressBar();
			this.WeaponsProgresBar = new System.Windows.Forms.ProgressBar();
			this.ArtifactsProgressBar = new System.Windows.Forms.ProgressBar();
			this.DevMaterialsProgressBar = new System.Windows.Forms.ProgressBar();
			this.MaterialsProgressBar = new System.Windows.Forms.ProgressBar();
			this.AllMaterialsProgressBar = new System.Windows.Forms.ProgressBar();
			this.UpdateStatusLabel = new System.Windows.Forms.Label();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.OpenFolderButton = new System.Windows.Forms.Button();
			this.CreateNewCheckBox = new System.Windows.Forms.CheckBox();
			this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// InstructionsLabel
			// 
			this.InstructionsLabel.BackColor = System.Drawing.SystemColors.ControlLight;
			this.InstructionsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.InstructionsLabel.Font = new System.Drawing.Font("Segoe UI Variable Small Semibol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.InstructionsLabel.Location = new System.Drawing.Point(3, 3);
			this.InstructionsLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.InstructionsLabel.Name = "InstructionsLabel";
			this.InstructionsLabel.Size = new System.Drawing.Size(196, 33);
			this.InstructionsLabel.TabIndex = 3;
			this.InstructionsLabel.Text = "Select Lists to Update";
			this.InstructionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// UpdateButton
			// 
			this.UpdateButton.AutoSize = true;
			this.UpdateButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.SetColumnSpan(this.UpdateButton, 2);
			this.UpdateButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.UpdateButton.Location = new System.Drawing.Point(3, 252);
			this.UpdateButton.Name = "UpdateButton";
			this.UpdateButton.Size = new System.Drawing.Size(396, 23);
			this.UpdateButton.TabIndex = 4;
			this.UpdateButton.Text = "Update";
			this.UpdateButton.UseVisualStyleBackColor = true;
			this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
			this.tableLayoutPanel1.Controls.Add(this.UpdateButton, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.InstructionsLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 1, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(10);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(402, 275);
			this.tableLayoutPanel1.TabIndex = 5;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.tableLayoutPanel3.ColumnCount = 1;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Controls.Add(this.CharactersCheckBox, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.WeaponsCheckBox, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.ArtifactsCheckBox, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this.DevMaterialsCheckBox, 0, 3);
			this.tableLayoutPanel3.Controls.Add(this.MaterialsCheckBox, 0, 4);
			this.tableLayoutPanel3.Controls.Add(this.AllMaterialsCheckBox, 0, 5);
			this.tableLayoutPanel3.Controls.Add(this.EverythingCheckBox, 0, 6);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(10, 41);
			this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(10, 5, 5, 5);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 7;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(187, 203);
			this.tableLayoutPanel3.TabIndex = 6;
			// 
			// CharactersCheckBox
			// 
			this.CharactersCheckBox.AutoSize = true;
			this.CharactersCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CharactersCheckBox.Enabled = false;
			this.CharactersCheckBox.Font = new System.Drawing.Font("Segoe UI Variable Small Semibol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CharactersCheckBox.Location = new System.Drawing.Point(4, 4);
			this.CharactersCheckBox.Name = "CharactersCheckBox";
			this.CharactersCheckBox.Size = new System.Drawing.Size(179, 21);
			this.CharactersCheckBox.TabIndex = 0;
			this.CharactersCheckBox.Tag = "Characters";
			this.CharactersCheckBox.Text = "Characters";
			this.CharactersCheckBox.UseVisualStyleBackColor = true;
			// 
			// WeaponsCheckBox
			// 
			this.WeaponsCheckBox.AutoSize = true;
			this.WeaponsCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WeaponsCheckBox.Enabled = false;
			this.WeaponsCheckBox.Font = new System.Drawing.Font("Segoe UI Variable Small Semibol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WeaponsCheckBox.Location = new System.Drawing.Point(4, 32);
			this.WeaponsCheckBox.Name = "WeaponsCheckBox";
			this.WeaponsCheckBox.Size = new System.Drawing.Size(179, 21);
			this.WeaponsCheckBox.TabIndex = 1;
			this.WeaponsCheckBox.Tag = "Weapons";
			this.WeaponsCheckBox.Text = "Weapons";
			this.WeaponsCheckBox.UseVisualStyleBackColor = true;
			// 
			// ArtifactsCheckBox
			// 
			this.ArtifactsCheckBox.AutoSize = true;
			this.ArtifactsCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ArtifactsCheckBox.Enabled = false;
			this.ArtifactsCheckBox.Font = new System.Drawing.Font("Segoe UI Variable Small Semibol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ArtifactsCheckBox.Location = new System.Drawing.Point(4, 60);
			this.ArtifactsCheckBox.Name = "ArtifactsCheckBox";
			this.ArtifactsCheckBox.Size = new System.Drawing.Size(179, 21);
			this.ArtifactsCheckBox.TabIndex = 2;
			this.ArtifactsCheckBox.Tag = "Artifacts";
			this.ArtifactsCheckBox.Text = "Artifacts";
			this.ArtifactsCheckBox.UseVisualStyleBackColor = true;
			// 
			// DevMaterialsCheckBox
			// 
			this.DevMaterialsCheckBox.AutoSize = true;
			this.DevMaterialsCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DevMaterialsCheckBox.Enabled = false;
			this.DevMaterialsCheckBox.Font = new System.Drawing.Font("Segoe UI Variable Small Semibol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DevMaterialsCheckBox.Location = new System.Drawing.Point(4, 88);
			this.DevMaterialsCheckBox.Name = "DevMaterialsCheckBox";
			this.DevMaterialsCheckBox.Size = new System.Drawing.Size(179, 21);
			this.DevMaterialsCheckBox.TabIndex = 3;
			this.DevMaterialsCheckBox.Tag = "Dev";
			this.DevMaterialsCheckBox.Text = "Development Materials";
			this.DevMaterialsCheckBox.UseVisualStyleBackColor = true;
			// 
			// MaterialsCheckBox
			// 
			this.MaterialsCheckBox.AutoSize = true;
			this.MaterialsCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MaterialsCheckBox.Enabled = false;
			this.MaterialsCheckBox.Font = new System.Drawing.Font("Segoe UI Variable Small Semibol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaterialsCheckBox.Location = new System.Drawing.Point(4, 116);
			this.MaterialsCheckBox.Name = "MaterialsCheckBox";
			this.MaterialsCheckBox.Size = new System.Drawing.Size(179, 21);
			this.MaterialsCheckBox.TabIndex = 4;
			this.MaterialsCheckBox.Tag = "Materials";
			this.MaterialsCheckBox.Text = "Materials";
			this.MaterialsCheckBox.UseVisualStyleBackColor = true;
			// 
			// AllMaterialsCheckBox
			// 
			this.AllMaterialsCheckBox.AutoSize = true;
			this.AllMaterialsCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AllMaterialsCheckBox.Enabled = false;
			this.AllMaterialsCheckBox.Font = new System.Drawing.Font("Segoe UI Variable Small Semibol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AllMaterialsCheckBox.Location = new System.Drawing.Point(4, 144);
			this.AllMaterialsCheckBox.Name = "AllMaterialsCheckBox";
			this.AllMaterialsCheckBox.Size = new System.Drawing.Size(179, 21);
			this.AllMaterialsCheckBox.TabIndex = 5;
			this.AllMaterialsCheckBox.Tag = "AllMaterials";
			this.AllMaterialsCheckBox.Text = "All Materials";
			this.AllMaterialsCheckBox.UseVisualStyleBackColor = true;
			// 
			// EverythingCheckBox
			// 
			this.EverythingCheckBox.AutoSize = true;
			this.EverythingCheckBox.Checked = true;
			this.EverythingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.EverythingCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.EverythingCheckBox.Font = new System.Drawing.Font("Segoe UI Variable Small Semibol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.EverythingCheckBox.Location = new System.Drawing.Point(4, 172);
			this.EverythingCheckBox.Name = "EverythingCheckBox";
			this.EverythingCheckBox.Size = new System.Drawing.Size(179, 27);
			this.EverythingCheckBox.TabIndex = 6;
			this.EverythingCheckBox.Tag = "Everything";
			this.EverythingCheckBox.Text = "Everything";
			this.EverythingCheckBox.UseVisualStyleBackColor = true;
			this.EverythingCheckBox.CheckedChanged += new System.EventHandler(this.EverythingCheckBox_CheckedChanged);
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.CharactersProgressBar, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.WeaponsProgresBar, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.ArtifactsProgressBar, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.DevMaterialsProgressBar, 0, 3);
			this.tableLayoutPanel2.Controls.Add(this.MaterialsProgressBar, 0, 4);
			this.tableLayoutPanel2.Controls.Add(this.AllMaterialsProgressBar, 0, 5);
			this.tableLayoutPanel2.Controls.Add(this.UpdateStatusLabel, 0, 6);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(207, 41);
			this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(5, 5, 10, 5);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 7;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(185, 203);
			this.tableLayoutPanel2.TabIndex = 7;
			// 
			// CharactersProgressBar
			// 
			this.CharactersProgressBar.Location = new System.Drawing.Point(3, 3);
			this.CharactersProgressBar.Name = "CharactersProgressBar";
			this.CharactersProgressBar.Size = new System.Drawing.Size(100, 22);
			this.CharactersProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.CharactersProgressBar.TabIndex = 0;
			this.CharactersProgressBar.Visible = false;
			// 
			// WeaponsProgresBar
			// 
			this.WeaponsProgresBar.Location = new System.Drawing.Point(3, 31);
			this.WeaponsProgresBar.Name = "WeaponsProgresBar";
			this.WeaponsProgresBar.Size = new System.Drawing.Size(100, 22);
			this.WeaponsProgresBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.WeaponsProgresBar.TabIndex = 1;
			this.WeaponsProgresBar.Visible = false;
			// 
			// ArtifactsProgressBar
			// 
			this.ArtifactsProgressBar.Location = new System.Drawing.Point(3, 59);
			this.ArtifactsProgressBar.Name = "ArtifactsProgressBar";
			this.ArtifactsProgressBar.Size = new System.Drawing.Size(100, 22);
			this.ArtifactsProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.ArtifactsProgressBar.TabIndex = 2;
			this.ArtifactsProgressBar.Visible = false;
			// 
			// DevMaterialsProgressBar
			// 
			this.DevMaterialsProgressBar.Location = new System.Drawing.Point(3, 87);
			this.DevMaterialsProgressBar.Name = "DevMaterialsProgressBar";
			this.DevMaterialsProgressBar.Size = new System.Drawing.Size(100, 22);
			this.DevMaterialsProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.DevMaterialsProgressBar.TabIndex = 3;
			this.DevMaterialsProgressBar.Visible = false;
			// 
			// MaterialsProgressBar
			// 
			this.MaterialsProgressBar.Location = new System.Drawing.Point(3, 115);
			this.MaterialsProgressBar.Name = "MaterialsProgressBar";
			this.MaterialsProgressBar.Size = new System.Drawing.Size(100, 22);
			this.MaterialsProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.MaterialsProgressBar.TabIndex = 4;
			this.MaterialsProgressBar.Visible = false;
			// 
			// AllMaterialsProgressBar
			// 
			this.AllMaterialsProgressBar.Location = new System.Drawing.Point(3, 143);
			this.AllMaterialsProgressBar.Name = "AllMaterialsProgressBar";
			this.AllMaterialsProgressBar.Size = new System.Drawing.Size(100, 22);
			this.AllMaterialsProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.AllMaterialsProgressBar.TabIndex = 5;
			this.AllMaterialsProgressBar.Visible = false;
			// 
			// UpdateStatusLabel
			// 
			this.UpdateStatusLabel.AutoSize = true;
			this.UpdateStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.UpdateStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.UpdateStatusLabel.Location = new System.Drawing.Point(3, 168);
			this.UpdateStatusLabel.Name = "UpdateStatusLabel";
			this.UpdateStatusLabel.Size = new System.Drawing.Size(179, 35);
			this.UpdateStatusLabel.TabIndex = 6;
			this.UpdateStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(205, 3);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.CreateNewCheckBox);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.OpenFolderButton);
			this.splitContainer1.Size = new System.Drawing.Size(194, 30);
			this.splitContainer1.SplitterDistance = 93;
			this.splitContainer1.TabIndex = 8;
			// 
			// OpenFolderButton
			// 
			this.OpenFolderButton.AutoSize = true;
			this.OpenFolderButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.OpenFolderButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.OpenFolderButton.Location = new System.Drawing.Point(0, 7);
			this.OpenFolderButton.Name = "OpenFolderButton";
			this.OpenFolderButton.Size = new System.Drawing.Size(97, 23);
			this.OpenFolderButton.TabIndex = 9;
			this.OpenFolderButton.Text = "Open Folder";
			this.OpenFolderButton.UseVisualStyleBackColor = true;
			this.OpenFolderButton.Click += new System.EventHandler(this.OpenFolderButton_Click);
			// 
			// CreateNewCheckBox
			// 
			this.CreateNewCheckBox.AutoSize = true;
			this.CreateNewCheckBox.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
			this.CreateNewCheckBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.CreateNewCheckBox.Location = new System.Drawing.Point(0, -1);
			this.CreateNewCheckBox.Name = "CreateNewCheckBox";
			this.CreateNewCheckBox.Size = new System.Drawing.Size(93, 31);
			this.CreateNewCheckBox.TabIndex = 0;
			this.CreateNewCheckBox.Text = "Create New Lists";
			this.CreateNewCheckBox.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.CreateNewCheckBox.UseVisualStyleBackColor = true;
			// 
			// DatabaseUpdateForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.ClientSize = new System.Drawing.Size(402, 275);
			this.Controls.Add(this.tableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DatabaseUpdateForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Update Manager";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DatabaseUpdateForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DatabaseUpdateForm_FormClosed);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Label InstructionsLabel;
		private System.Windows.Forms.Button UpdateButton;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.CheckBox CharactersCheckBox;
		private System.Windows.Forms.CheckBox WeaponsCheckBox;
		private System.Windows.Forms.CheckBox ArtifactsCheckBox;
		private System.Windows.Forms.CheckBox DevMaterialsCheckBox;
		private System.Windows.Forms.CheckBox MaterialsCheckBox;
		private System.Windows.Forms.CheckBox AllMaterialsCheckBox;
		private System.Windows.Forms.CheckBox EverythingCheckBox;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.ProgressBar CharactersProgressBar;
		private System.Windows.Forms.ProgressBar WeaponsProgresBar;
		private System.Windows.Forms.ProgressBar ArtifactsProgressBar;
		private System.Windows.Forms.ProgressBar DevMaterialsProgressBar;
		private System.Windows.Forms.ProgressBar MaterialsProgressBar;
		private System.Windows.Forms.ProgressBar AllMaterialsProgressBar;
		private System.Windows.Forms.Label UpdateStatusLabel;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.CheckBox CreateNewCheckBox;
		private System.Windows.Forms.Button OpenFolderButton;
		private System.Windows.Forms.ToolTip ToolTip;
	}
}