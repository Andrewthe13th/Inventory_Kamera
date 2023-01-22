namespace InventoryKamera.ui.main
{
    partial class MainUI
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
            this.StartButton = new System.Windows.Forms.Button();
            this.SidePanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.WandererTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TravelerTextBox = new System.Windows.Forms.TextBox();
            this.ArtifactRarityFilter = new System.Windows.Forms.FlowLayoutPanel();
            this.ArtifactRarity1Radio = new System.Windows.Forms.RadioButton();
            this.ArtifactRarity2Radio = new System.Windows.Forms.RadioButton();
            this.ArtifactRarity3Radio = new System.Windows.Forms.RadioButton();
            this.ArtifactRarity4Radio = new System.Windows.Forms.RadioButton();
            this.ArtifactRarity5Radio = new System.Windows.Forms.RadioButton();
            this.WeaponRarityLabel = new System.Windows.Forms.Label();
            this.WeaponRarityFilter = new System.Windows.Forms.FlowLayoutPanel();
            this.WeaponRarity1Radio = new System.Windows.Forms.RadioButton();
            this.WeaponRarity2Radio = new System.Windows.Forms.RadioButton();
            this.WeaponRarity3Radio = new System.Windows.Forms.RadioButton();
            this.WeaponRarity4Radio = new System.Windows.Forms.RadioButton();
            this.WeaponRarity5Radio = new System.Windows.Forms.RadioButton();
            this.ArtifactRarityLabel = new System.Windows.Forms.Label();
            this.ScanItemsPanel = new System.Windows.Forms.Panel();
            this.MaterialsCheckBox = new System.Windows.Forms.CheckBox();
            this.DevMaterialsCheckBox = new System.Windows.Forms.CheckBox();
            this.CharactersCheckBox = new System.Windows.Forms.CheckBox();
            this.ArtifactsCheckBox = new System.Windows.Forms.CheckBox();
            this.WeaponsCheckBox = new System.Windows.Forms.CheckBox();
            this.InformationPanel = new System.Windows.Forms.Panel();
            this.GithubLabelLink = new System.Windows.Forms.LinkLabel();
            this.versionLabel = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.ExitButton = new System.Windows.Forms.Button();
            this.OutputTextBox = new System.Windows.Forms.RichTextBox();
            this.ErrorTextBox = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SidePanel.SuspendLayout();
            this.ArtifactRarityFilter.SuspendLayout();
            this.WeaponRarityFilter.SuspendLayout();
            this.ScanItemsPanel.SuspendLayout();
            this.InformationPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(794, 623);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 23);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            // 
            // SidePanel
            // 
            this.SidePanel.Controls.Add(this.label2);
            this.SidePanel.Controls.Add(this.WandererTextBox);
            this.SidePanel.Controls.Add(this.label1);
            this.SidePanel.Controls.Add(this.TravelerTextBox);
            this.SidePanel.Controls.Add(this.ArtifactRarityFilter);
            this.SidePanel.Controls.Add(this.WeaponRarityLabel);
            this.SidePanel.Controls.Add(this.WeaponRarityFilter);
            this.SidePanel.Controls.Add(this.ArtifactRarityLabel);
            this.SidePanel.Controls.Add(this.ScanItemsPanel);
            this.SidePanel.Controls.Add(this.InformationPanel);
            this.SidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.SidePanel.Location = new System.Drawing.Point(0, 0);
            this.SidePanel.Name = "SidePanel";
            this.SidePanel.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.SidePanel.Size = new System.Drawing.Size(292, 658);
            this.SidePanel.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(13, 383);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 18);
            this.label2.TabIndex = 10;
            this.label2.Text = "Wanderer Username";
            // 
            // WandererTextBox
            // 
            this.WandererTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::InventoryKamera.Properties.Settings.Default, "WandererName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.WandererTextBox.Location = new System.Drawing.Point(13, 404);
            this.WandererTextBox.Name = "WandererTextBox";
            this.WandererTextBox.Size = new System.Drawing.Size(190, 20);
            this.WandererTextBox.TabIndex = 9;
            this.WandererTextBox.Text = global::InventoryKamera.Properties.Settings.Default.WandererName;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(14, 332);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 18);
            this.label1.TabIndex = 8;
            this.label1.Text = "Traveler Username";
            // 
            // TravelerTextBox
            // 
            this.TravelerTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::InventoryKamera.Properties.Settings.Default, "TravelerName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TravelerTextBox.Location = new System.Drawing.Point(13, 351);
            this.TravelerTextBox.Name = "TravelerTextBox";
            this.TravelerTextBox.Size = new System.Drawing.Size(190, 20);
            this.TravelerTextBox.TabIndex = 7;
            this.TravelerTextBox.Text = global::InventoryKamera.Properties.Settings.Default.TravelerName;
            // 
            // ArtifactRarityFilter
            // 
            this.ArtifactRarityFilter.AutoSize = true;
            this.ArtifactRarityFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ArtifactRarityFilter.Controls.Add(this.ArtifactRarity1Radio);
            this.ArtifactRarityFilter.Controls.Add(this.ArtifactRarity2Radio);
            this.ArtifactRarityFilter.Controls.Add(this.ArtifactRarity3Radio);
            this.ArtifactRarityFilter.Controls.Add(this.ArtifactRarity4Radio);
            this.ArtifactRarityFilter.Controls.Add(this.ArtifactRarity5Radio);
            this.ArtifactRarityFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.ArtifactRarityFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ArtifactRarityFilter.ForeColor = System.Drawing.Color.White;
            this.ArtifactRarityFilter.Location = new System.Drawing.Point(10, 295);
            this.ArtifactRarityFilter.Name = "ArtifactRarityFilter";
            this.ArtifactRarityFilter.Size = new System.Drawing.Size(282, 26);
            this.ArtifactRarityFilter.TabIndex = 4;
            // 
            // ArtifactRarity1Radio
            // 
            this.ArtifactRarity1Radio.AutoSize = true;
            this.ArtifactRarity1Radio.Checked = true;
            this.ArtifactRarity1Radio.Location = new System.Drawing.Point(3, 3);
            this.ArtifactRarity1Radio.Name = "ArtifactRarity1Radio";
            this.ArtifactRarity1Radio.Size = new System.Drawing.Size(32, 20);
            this.ArtifactRarity1Radio.TabIndex = 0;
            this.ArtifactRarity1Radio.TabStop = true;
            this.ArtifactRarity1Radio.Text = "1";
            this.ArtifactRarity1Radio.UseVisualStyleBackColor = true;
            // 
            // ArtifactRarity2Radio
            // 
            this.ArtifactRarity2Radio.AutoSize = true;
            this.ArtifactRarity2Radio.Location = new System.Drawing.Point(41, 3);
            this.ArtifactRarity2Radio.Name = "ArtifactRarity2Radio";
            this.ArtifactRarity2Radio.Size = new System.Drawing.Size(32, 20);
            this.ArtifactRarity2Radio.TabIndex = 1;
            this.ArtifactRarity2Radio.Text = "2";
            this.ArtifactRarity2Radio.UseVisualStyleBackColor = true;
            // 
            // ArtifactRarity3Radio
            // 
            this.ArtifactRarity3Radio.AutoSize = true;
            this.ArtifactRarity3Radio.Location = new System.Drawing.Point(79, 3);
            this.ArtifactRarity3Radio.Name = "ArtifactRarity3Radio";
            this.ArtifactRarity3Radio.Size = new System.Drawing.Size(32, 20);
            this.ArtifactRarity3Radio.TabIndex = 2;
            this.ArtifactRarity3Radio.Text = "3";
            this.ArtifactRarity3Radio.UseVisualStyleBackColor = true;
            // 
            // ArtifactRarity4Radio
            // 
            this.ArtifactRarity4Radio.AutoSize = true;
            this.ArtifactRarity4Radio.Location = new System.Drawing.Point(117, 3);
            this.ArtifactRarity4Radio.Name = "ArtifactRarity4Radio";
            this.ArtifactRarity4Radio.Size = new System.Drawing.Size(32, 20);
            this.ArtifactRarity4Radio.TabIndex = 3;
            this.ArtifactRarity4Radio.Text = "4";
            this.ArtifactRarity4Radio.UseVisualStyleBackColor = true;
            // 
            // ArtifactRarity5Radio
            // 
            this.ArtifactRarity5Radio.AutoSize = true;
            this.ArtifactRarity5Radio.Location = new System.Drawing.Point(155, 3);
            this.ArtifactRarity5Radio.Name = "ArtifactRarity5Radio";
            this.ArtifactRarity5Radio.Size = new System.Drawing.Size(32, 20);
            this.ArtifactRarity5Radio.TabIndex = 4;
            this.ArtifactRarity5Radio.Text = "5";
            this.ArtifactRarity5Radio.UseVisualStyleBackColor = true;
            // 
            // WeaponRarityLabel
            // 
            this.WeaponRarityLabel.AutoSize = true;
            this.WeaponRarityLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.WeaponRarityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WeaponRarityLabel.ForeColor = System.Drawing.Color.White;
            this.WeaponRarityLabel.Location = new System.Drawing.Point(10, 277);
            this.WeaponRarityLabel.Name = "WeaponRarityLabel";
            this.WeaponRarityLabel.Size = new System.Drawing.Size(171, 18);
            this.WeaponRarityLabel.TabIndex = 6;
            this.WeaponRarityLabel.Text = "Minimum Weapon Rarity";
            // 
            // WeaponRarityFilter
            // 
            this.WeaponRarityFilter.AutoSize = true;
            this.WeaponRarityFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.WeaponRarityFilter.Controls.Add(this.WeaponRarity1Radio);
            this.WeaponRarityFilter.Controls.Add(this.WeaponRarity2Radio);
            this.WeaponRarityFilter.Controls.Add(this.WeaponRarity3Radio);
            this.WeaponRarityFilter.Controls.Add(this.WeaponRarity4Radio);
            this.WeaponRarityFilter.Controls.Add(this.WeaponRarity5Radio);
            this.WeaponRarityFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.WeaponRarityFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WeaponRarityFilter.ForeColor = System.Drawing.Color.White;
            this.WeaponRarityFilter.Location = new System.Drawing.Point(10, 251);
            this.WeaponRarityFilter.Name = "WeaponRarityFilter";
            this.WeaponRarityFilter.Size = new System.Drawing.Size(282, 26);
            this.WeaponRarityFilter.TabIndex = 3;
            // 
            // WeaponRarity1Radio
            // 
            this.WeaponRarity1Radio.AutoSize = true;
            this.WeaponRarity1Radio.Checked = true;
            this.WeaponRarity1Radio.Location = new System.Drawing.Point(3, 3);
            this.WeaponRarity1Radio.Name = "WeaponRarity1Radio";
            this.WeaponRarity1Radio.Size = new System.Drawing.Size(32, 20);
            this.WeaponRarity1Radio.TabIndex = 0;
            this.WeaponRarity1Radio.TabStop = true;
            this.WeaponRarity1Radio.Text = "1";
            this.WeaponRarity1Radio.UseVisualStyleBackColor = true;
            // 
            // WeaponRarity2Radio
            // 
            this.WeaponRarity2Radio.AutoSize = true;
            this.WeaponRarity2Radio.Location = new System.Drawing.Point(41, 3);
            this.WeaponRarity2Radio.Name = "WeaponRarity2Radio";
            this.WeaponRarity2Radio.Size = new System.Drawing.Size(32, 20);
            this.WeaponRarity2Radio.TabIndex = 1;
            this.WeaponRarity2Radio.Text = "2";
            this.WeaponRarity2Radio.UseVisualStyleBackColor = true;
            // 
            // WeaponRarity3Radio
            // 
            this.WeaponRarity3Radio.AutoSize = true;
            this.WeaponRarity3Radio.Location = new System.Drawing.Point(79, 3);
            this.WeaponRarity3Radio.Name = "WeaponRarity3Radio";
            this.WeaponRarity3Radio.Size = new System.Drawing.Size(32, 20);
            this.WeaponRarity3Radio.TabIndex = 2;
            this.WeaponRarity3Radio.Text = "3";
            this.WeaponRarity3Radio.UseVisualStyleBackColor = true;
            // 
            // WeaponRarity4Radio
            // 
            this.WeaponRarity4Radio.AutoSize = true;
            this.WeaponRarity4Radio.Location = new System.Drawing.Point(117, 3);
            this.WeaponRarity4Radio.Name = "WeaponRarity4Radio";
            this.WeaponRarity4Radio.Size = new System.Drawing.Size(32, 20);
            this.WeaponRarity4Radio.TabIndex = 3;
            this.WeaponRarity4Radio.Text = "4";
            this.WeaponRarity4Radio.UseVisualStyleBackColor = true;
            // 
            // WeaponRarity5Radio
            // 
            this.WeaponRarity5Radio.AutoSize = true;
            this.WeaponRarity5Radio.Location = new System.Drawing.Point(155, 3);
            this.WeaponRarity5Radio.Name = "WeaponRarity5Radio";
            this.WeaponRarity5Radio.Size = new System.Drawing.Size(32, 20);
            this.WeaponRarity5Radio.TabIndex = 4;
            this.WeaponRarity5Radio.Text = "5";
            this.WeaponRarity5Radio.UseVisualStyleBackColor = true;
            // 
            // ArtifactRarityLabel
            // 
            this.ArtifactRarityLabel.AutoSize = true;
            this.ArtifactRarityLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ArtifactRarityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ArtifactRarityLabel.ForeColor = System.Drawing.Color.White;
            this.ArtifactRarityLabel.Location = new System.Drawing.Point(10, 233);
            this.ArtifactRarityLabel.Name = "ArtifactRarityLabel";
            this.ArtifactRarityLabel.Size = new System.Drawing.Size(160, 18);
            this.ArtifactRarityLabel.TabIndex = 6;
            this.ArtifactRarityLabel.Text = "Minimum Artifact Rarity";
            // 
            // ScanItemsPanel
            // 
            this.ScanItemsPanel.AutoSize = true;
            this.ScanItemsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ScanItemsPanel.Controls.Add(this.MaterialsCheckBox);
            this.ScanItemsPanel.Controls.Add(this.DevMaterialsCheckBox);
            this.ScanItemsPanel.Controls.Add(this.CharactersCheckBox);
            this.ScanItemsPanel.Controls.Add(this.ArtifactsCheckBox);
            this.ScanItemsPanel.Controls.Add(this.WeaponsCheckBox);
            this.ScanItemsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ScanItemsPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScanItemsPanel.ForeColor = System.Drawing.Color.White;
            this.ScanItemsPanel.Location = new System.Drawing.Point(10, 88);
            this.ScanItemsPanel.Name = "ScanItemsPanel";
            this.ScanItemsPanel.Size = new System.Drawing.Size(282, 145);
            this.ScanItemsPanel.TabIndex = 1;
            // 
            // MaterialsCheckBox
            // 
            this.MaterialsCheckBox.AutoSize = true;
            this.MaterialsCheckBox.Checked = global::InventoryKamera.Properties.Settings.Default.ScanMaterials;
            this.MaterialsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MaterialsCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::InventoryKamera.Properties.Settings.Default, "ScanMaterials", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.MaterialsCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.MaterialsCheckBox.Location = new System.Drawing.Point(0, 116);
            this.MaterialsCheckBox.Name = "MaterialsCheckBox";
            this.MaterialsCheckBox.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.MaterialsCheckBox.Size = new System.Drawing.Size(282, 29);
            this.MaterialsCheckBox.TabIndex = 4;
            this.MaterialsCheckBox.Text = "Materials";
            this.MaterialsCheckBox.UseVisualStyleBackColor = true;
            // 
            // DevMaterialsCheckBox
            // 
            this.DevMaterialsCheckBox.AutoSize = true;
            this.DevMaterialsCheckBox.Checked = global::InventoryKamera.Properties.Settings.Default.ScanCharDevItems;
            this.DevMaterialsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DevMaterialsCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::InventoryKamera.Properties.Settings.Default, "ScanCharDevItems", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DevMaterialsCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.DevMaterialsCheckBox.Location = new System.Drawing.Point(0, 87);
            this.DevMaterialsCheckBox.Name = "DevMaterialsCheckBox";
            this.DevMaterialsCheckBox.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.DevMaterialsCheckBox.Size = new System.Drawing.Size(282, 29);
            this.DevMaterialsCheckBox.TabIndex = 3;
            this.DevMaterialsCheckBox.Text = "Character Development Materials";
            this.DevMaterialsCheckBox.UseVisualStyleBackColor = true;
            // 
            // CharactersCheckBox
            // 
            this.CharactersCheckBox.AutoSize = true;
            this.CharactersCheckBox.Checked = global::InventoryKamera.Properties.Settings.Default.ScanCharacters;
            this.CharactersCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CharactersCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::InventoryKamera.Properties.Settings.Default, "ScanCharacters", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CharactersCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.CharactersCheckBox.Location = new System.Drawing.Point(0, 58);
            this.CharactersCheckBox.Name = "CharactersCheckBox";
            this.CharactersCheckBox.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.CharactersCheckBox.Size = new System.Drawing.Size(282, 29);
            this.CharactersCheckBox.TabIndex = 2;
            this.CharactersCheckBox.Text = "Characters";
            this.CharactersCheckBox.UseVisualStyleBackColor = true;
            // 
            // ArtifactsCheckBox
            // 
            this.ArtifactsCheckBox.AutoSize = true;
            this.ArtifactsCheckBox.Checked = global::InventoryKamera.Properties.Settings.Default.ScanArtifacts;
            this.ArtifactsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ArtifactsCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::InventoryKamera.Properties.Settings.Default, "ScanArtifacts", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ArtifactsCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.ArtifactsCheckBox.Location = new System.Drawing.Point(0, 29);
            this.ArtifactsCheckBox.Name = "ArtifactsCheckBox";
            this.ArtifactsCheckBox.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.ArtifactsCheckBox.Size = new System.Drawing.Size(282, 29);
            this.ArtifactsCheckBox.TabIndex = 1;
            this.ArtifactsCheckBox.Text = "Artifacts";
            this.ArtifactsCheckBox.UseVisualStyleBackColor = true;
            // 
            // WeaponsCheckBox
            // 
            this.WeaponsCheckBox.AutoSize = true;
            this.WeaponsCheckBox.Checked = global::InventoryKamera.Properties.Settings.Default.ScanWeapons;
            this.WeaponsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WeaponsCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::InventoryKamera.Properties.Settings.Default, "ScanWeapons", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.WeaponsCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.WeaponsCheckBox.Location = new System.Drawing.Point(0, 0);
            this.WeaponsCheckBox.Name = "WeaponsCheckBox";
            this.WeaponsCheckBox.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.WeaponsCheckBox.Size = new System.Drawing.Size(282, 29);
            this.WeaponsCheckBox.TabIndex = 0;
            this.WeaponsCheckBox.Text = "Weapons";
            this.WeaponsCheckBox.UseVisualStyleBackColor = true;
            // 
            // InformationPanel
            // 
            this.InformationPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.InformationPanel.Controls.Add(this.GithubLabelLink);
            this.InformationPanel.Controls.Add(this.versionLabel);
            this.InformationPanel.Controls.Add(this.TitleLabel);
            this.InformationPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.InformationPanel.Location = new System.Drawing.Point(10, 0);
            this.InformationPanel.Name = "InformationPanel";
            this.InformationPanel.Size = new System.Drawing.Size(282, 88);
            this.InformationPanel.TabIndex = 0;
            // 
            // GithubLabelLink
            // 
            this.GithubLabelLink.AutoSize = true;
            this.GithubLabelLink.Location = new System.Drawing.Point(4, 72);
            this.GithubLabelLink.Name = "GithubLabelLink";
            this.GithubLabelLink.Size = new System.Drawing.Size(40, 13);
            this.GithubLabelLink.TabIndex = 2;
            this.GithubLabelLink.TabStop = true;
            this.GithubLabelLink.Text = "GitHub";
            this.GithubLabelLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GithubLabelLink_LinkClicked);
            // 
            // versionLabel
            // 
            this.versionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.versionLabel.AutoSize = true;
            this.versionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionLabel.ForeColor = System.Drawing.Color.Black;
            this.versionLabel.Location = new System.Drawing.Point(97, 51);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(94, 24);
            this.versionLabel.TabIndex = 1;
            this.versionLabel.Text = "VERSION";
            // 
            // TitleLabel
            // 
            this.TitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.ForeColor = System.Drawing.Color.Black;
            this.TitleLabel.Location = new System.Drawing.Point(37, 8);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(184, 26);
            this.TitleLabel.TabIndex = 0;
            this.TitleLabel.Text = "Inventory Kamera";
            // 
            // ExitButton
            // 
            this.ExitButton.AutoSize = true;
            this.ExitButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ExitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(190)))), ((int)(((byte)(197)))));
            this.ExitButton.FlatAppearance.BorderSize = 0;
            this.ExitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExitButton.Location = new System.Drawing.Point(845, 12);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(24, 23);
            this.ExitButton.TabIndex = 2;
            this.ExitButton.Text = "X";
            this.ExitButton.UseVisualStyleBackColor = false;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // OutputTextBox
            // 
            this.OutputTextBox.Location = new System.Drawing.Point(682, 52);
            this.OutputTextBox.Name = "OutputTextBox";
            this.OutputTextBox.ReadOnly = true;
            this.OutputTextBox.Size = new System.Drawing.Size(187, 360);
            this.OutputTextBox.TabIndex = 4;
            this.OutputTextBox.Text = "";
            this.OutputTextBox.WordWrap = false;
            // 
            // ErrorTextBox
            // 
            this.ErrorTextBox.Location = new System.Drawing.Point(298, 478);
            this.ErrorTextBox.Name = "ErrorTextBox";
            this.ErrorTextBox.ReadOnly = true;
            this.ErrorTextBox.Size = new System.Drawing.Size(571, 139);
            this.ErrorTextBox.TabIndex = 4;
            this.ErrorTextBox.Text = "";
            this.ErrorTextBox.WordWrap = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(299, 623);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Open Log";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Log_Button_Click);
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.ClientSize = new System.Drawing.Size(881, 658);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ErrorTextBox);
            this.Controls.Add(this.OutputTextBox);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.SidePanel);
            this.Controls.Add(this.StartButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MainUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NewUIForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainUI_FormClosing);
            this.SidePanel.ResumeLayout(false);
            this.SidePanel.PerformLayout();
            this.ArtifactRarityFilter.ResumeLayout(false);
            this.ArtifactRarityFilter.PerformLayout();
            this.WeaponRarityFilter.ResumeLayout(false);
            this.WeaponRarityFilter.PerformLayout();
            this.ScanItemsPanel.ResumeLayout(false);
            this.ScanItemsPanel.PerformLayout();
            this.InformationPanel.ResumeLayout(false);
            this.InformationPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Panel SidePanel;
        private System.Windows.Forms.Panel InformationPanel;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.CheckBox ArtifactsCheckBox;
        private System.Windows.Forms.CheckBox WeaponsCheckBox;
        private System.Windows.Forms.CheckBox MaterialsCheckBox;
        private System.Windows.Forms.CheckBox DevMaterialsCheckBox;
        private System.Windows.Forms.CheckBox CharactersCheckBox;
        private System.Windows.Forms.Panel ScanItemsPanel;
        private System.Windows.Forms.FlowLayoutPanel WeaponRarityFilter;
        private System.Windows.Forms.RadioButton WeaponRarity1Radio;
        private System.Windows.Forms.RadioButton WeaponRarity2Radio;
        private System.Windows.Forms.RadioButton WeaponRarity3Radio;
        private System.Windows.Forms.RadioButton WeaponRarity4Radio;
        private System.Windows.Forms.RadioButton WeaponRarity5Radio;
        private System.Windows.Forms.FlowLayoutPanel ArtifactRarityFilter;
        private System.Windows.Forms.RadioButton ArtifactRarity1Radio;
        private System.Windows.Forms.RadioButton ArtifactRarity2Radio;
        private System.Windows.Forms.RadioButton ArtifactRarity3Radio;
        private System.Windows.Forms.RadioButton ArtifactRarity4Radio;
        private System.Windows.Forms.RadioButton ArtifactRarity5Radio;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label WeaponRarityLabel;
        private System.Windows.Forms.Label ArtifactRarityLabel;
        private System.Windows.Forms.RichTextBox OutputTextBox;
        private System.Windows.Forms.RichTextBox ErrorTextBox;
        private System.Windows.Forms.TextBox TravelerTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox WandererTextBox;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.LinkLabel GithubLabelLink;
    }
}