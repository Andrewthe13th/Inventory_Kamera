namespace GenshinGuide
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnClickHere = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.a_GearSlot_Image = new System.Windows.Forms.PictureBox();
            this.a_MainStat_Image = new System.Windows.Forms.PictureBox();
            this.a_Level_Image = new System.Windows.Forms.PictureBox();
            this.a_SubStat1_Image = new System.Windows.Forms.PictureBox();
            this.a_SubStat2_Image = new System.Windows.Forms.PictureBox();
            this.a_SubStat3_Image = new System.Windows.Forms.PictureBox();
            this.a_SubStat4_Image = new System.Windows.Forms.PictureBox();
            this.a_SetName_Image = new System.Windows.Forms.PictureBox();
            this.a_Equipped_Image = new System.Windows.Forms.PictureBox();
            this.a_TextBox = new System.Windows.Forms.TextBox();
            this.c_TextBox = new System.Windows.Forms.TextBox();
            this.c_Talent_3 = new System.Windows.Forms.PictureBox();
            this.c_Talent_2 = new System.Windows.Forms.PictureBox();
            this.c_Talent_1 = new System.Windows.Forms.PictureBox();
            this.c_Level_Image = new System.Windows.Forms.PictureBox();
            this.c_Name_Image = new System.Windows.Forms.PictureBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.characterCount = new System.Windows.Forms.Label();
            this.artifactCount = new System.Windows.Forms.Label();
            this.weaponCount = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.maxWeapons = new System.Windows.Forms.Label();
            this.maxArtifacts = new System.Windows.Forms.Label();
            this.ProgramStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.a_GearSlot_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_MainStat_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_Level_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_SubStat1_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_SubStat2_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_SubStat3_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_SubStat4_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_SetName_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_Equipped_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_Talent_3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_Talent_2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_Talent_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_Level_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_Name_Image)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClickHere
            // 
            this.btnClickHere.Location = new System.Drawing.Point(276, 233);
            this.btnClickHere.Name = "btnClickHere";
            this.btnClickHere.Size = new System.Drawing.Size(155, 43);
            this.btnClickHere.TabIndex = 0;
            this.btnClickHere.Text = "Scan Genshin";
            this.btnClickHere.UseVisualStyleBackColor = true;
            this.btnClickHere.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(36, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(501, 59);
            this.label1.TabIndex = 6;
            this.label1.Text = "This tool will take control of your mouse and keyboard.  You will not be able to " +
    "use your computer while it runs. Cancel anytime by pressing \'ENTER\' on keyboard." +
    "";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 314);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Weapon/Artifact:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(128, 314);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "Output";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(12, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "NOTICE:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(227, 279);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(268, 17);
            this.label5.TabIndex = 11;
            this.label5.Text = "Cancel by pressing \'ENTER\' on keyboard";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.DarkGreen;
            this.label6.Location = new System.Drawing.Point(12, 113);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 17);
            this.label6.TabIndex = 12;
            this.label6.Text = "INSTRUCTIONS:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 134);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(223, 17);
            this.label7.TabIndex = 13;
            this.label7.Text = "1. Run Genshin Impact and Log In";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(39, 151);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(114, 17);
            this.label8.TabIndex = 14;
            this.label8.Text = "2. Go to Settings";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(65, 168);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(399, 17);
            this.label9.TabIndex = 15;
            this.label9.Text = "2a. Under Graphics, set Display Mode to 1280x720 Windowed";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(65, 185);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(320, 17);
            this.label10.TabIndex = 16;
            this.label10.Text = "2b. Under Controls, set Control Type to Keyboard";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(39, 202);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(309, 17);
            this.label11.TabIndex = 17;
            this.label11.Text = "3. Exit settings and leave game in Paimon Menu";
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.White;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "ENG"});
            this.comboBox1.Location = new System.Drawing.Point(12, 10);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(70, 24);
            this.comboBox1.TabIndex = 18;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // a_GearSlot_Image
            // 
            this.a_GearSlot_Image.Location = new System.Drawing.Point(15, 335);
            this.a_GearSlot_Image.Name = "a_GearSlot_Image";
            this.a_GearSlot_Image.Size = new System.Drawing.Size(90, 18);
            this.a_GearSlot_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.a_GearSlot_Image.TabIndex = 19;
            this.a_GearSlot_Image.TabStop = false;
            // 
            // a_MainStat_Image
            // 
            this.a_MainStat_Image.Location = new System.Drawing.Point(15, 359);
            this.a_MainStat_Image.Name = "a_MainStat_Image";
            this.a_MainStat_Image.Size = new System.Drawing.Size(90, 21);
            this.a_MainStat_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.a_MainStat_Image.TabIndex = 20;
            this.a_MainStat_Image.TabStop = false;
            // 
            // a_Level_Image
            // 
            this.a_Level_Image.Location = new System.Drawing.Point(15, 386);
            this.a_Level_Image.Name = "a_Level_Image";
            this.a_Level_Image.Size = new System.Drawing.Size(16, 16);
            this.a_Level_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.a_Level_Image.TabIndex = 21;
            this.a_Level_Image.TabStop = false;
            // 
            // a_SubStat1_Image
            // 
            this.a_SubStat1_Image.Location = new System.Drawing.Point(15, 408);
            this.a_SubStat1_Image.Name = "a_SubStat1_Image";
            this.a_SubStat1_Image.Size = new System.Drawing.Size(90, 18);
            this.a_SubStat1_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.a_SubStat1_Image.TabIndex = 22;
            this.a_SubStat1_Image.TabStop = false;
            // 
            // a_SubStat2_Image
            // 
            this.a_SubStat2_Image.Location = new System.Drawing.Point(15, 432);
            this.a_SubStat2_Image.Name = "a_SubStat2_Image";
            this.a_SubStat2_Image.Size = new System.Drawing.Size(90, 18);
            this.a_SubStat2_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.a_SubStat2_Image.TabIndex = 23;
            this.a_SubStat2_Image.TabStop = false;
            // 
            // a_SubStat3_Image
            // 
            this.a_SubStat3_Image.Location = new System.Drawing.Point(15, 456);
            this.a_SubStat3_Image.Name = "a_SubStat3_Image";
            this.a_SubStat3_Image.Size = new System.Drawing.Size(90, 18);
            this.a_SubStat3_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.a_SubStat3_Image.TabIndex = 24;
            this.a_SubStat3_Image.TabStop = false;
            // 
            // a_SubStat4_Image
            // 
            this.a_SubStat4_Image.Location = new System.Drawing.Point(15, 480);
            this.a_SubStat4_Image.Name = "a_SubStat4_Image";
            this.a_SubStat4_Image.Size = new System.Drawing.Size(90, 18);
            this.a_SubStat4_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.a_SubStat4_Image.TabIndex = 25;
            this.a_SubStat4_Image.TabStop = false;
            // 
            // a_SetName_Image
            // 
            this.a_SetName_Image.Location = new System.Drawing.Point(15, 504);
            this.a_SetName_Image.Name = "a_SetName_Image";
            this.a_SetName_Image.Size = new System.Drawing.Size(90, 18);
            this.a_SetName_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.a_SetName_Image.TabIndex = 26;
            this.a_SetName_Image.TabStop = false;
            // 
            // a_Equipped_Image
            // 
            this.a_Equipped_Image.Location = new System.Drawing.Point(15, 528);
            this.a_Equipped_Image.Name = "a_Equipped_Image";
            this.a_Equipped_Image.Size = new System.Drawing.Size(90, 18);
            this.a_Equipped_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.a_Equipped_Image.TabIndex = 27;
            this.a_Equipped_Image.TabStop = false;
            // 
            // a_TextBox
            // 
            this.a_TextBox.Location = new System.Drawing.Point(131, 335);
            this.a_TextBox.Multiline = true;
            this.a_TextBox.Name = "a_TextBox";
            this.a_TextBox.Size = new System.Drawing.Size(131, 211);
            this.a_TextBox.TabIndex = 28;
            // 
            // c_TextBox
            // 
            this.c_TextBox.Location = new System.Drawing.Point(406, 335);
            this.c_TextBox.Multiline = true;
            this.c_TextBox.Name = "c_TextBox";
            this.c_TextBox.Size = new System.Drawing.Size(131, 211);
            this.c_TextBox.TabIndex = 40;
            // 
            // c_Talent_3
            // 
            this.c_Talent_3.Location = new System.Drawing.Point(290, 441);
            this.c_Talent_3.Name = "c_Talent_3";
            this.c_Talent_3.Size = new System.Drawing.Size(90, 18);
            this.c_Talent_3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.c_Talent_3.TabIndex = 36;
            this.c_Talent_3.TabStop = false;
            // 
            // c_Talent_2
            // 
            this.c_Talent_2.Location = new System.Drawing.Point(290, 417);
            this.c_Talent_2.Name = "c_Talent_2";
            this.c_Talent_2.Size = new System.Drawing.Size(90, 18);
            this.c_Talent_2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.c_Talent_2.TabIndex = 35;
            this.c_Talent_2.TabStop = false;
            this.c_Talent_2.Click += new System.EventHandler(this.pictureBox5_Click);
            // 
            // c_Talent_1
            // 
            this.c_Talent_1.Location = new System.Drawing.Point(290, 393);
            this.c_Talent_1.Name = "c_Talent_1";
            this.c_Talent_1.Size = new System.Drawing.Size(90, 18);
            this.c_Talent_1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.c_Talent_1.TabIndex = 34;
            this.c_Talent_1.TabStop = false;
            // 
            // c_Level_Image
            // 
            this.c_Level_Image.Location = new System.Drawing.Point(290, 366);
            this.c_Level_Image.Name = "c_Level_Image";
            this.c_Level_Image.Size = new System.Drawing.Size(90, 21);
            this.c_Level_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.c_Level_Image.TabIndex = 32;
            this.c_Level_Image.TabStop = false;
            // 
            // c_Name_Image
            // 
            this.c_Name_Image.Location = new System.Drawing.Point(290, 335);
            this.c_Name_Image.Name = "c_Name_Image";
            this.c_Name_Image.Size = new System.Drawing.Size(110, 25);
            this.c_Name_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.c_Name_Image.TabIndex = 31;
            this.c_Name_Image.TabStop = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(403, 314);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(51, 17);
            this.label12.TabIndex = 30;
            this.label12.Text = "Output";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(284, 314);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(74, 17);
            this.label13.TabIndex = 29;
            this.label13.Text = "Character:";
            this.label13.Click += new System.EventHandler(this.label13_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(15, 242);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(76, 17);
            this.label14.TabIndex = 41;
            this.label14.Text = "Weapons: ";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(15, 259);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(67, 17);
            this.label15.TabIndex = 42;
            this.label15.Text = "Artifacts: ";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(15, 276);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(85, 17);
            this.label16.TabIndex = 43;
            this.label16.Text = "Characters: ";
            // 
            // characterCount
            // 
            this.characterCount.AutoSize = true;
            this.characterCount.Location = new System.Drawing.Point(106, 276);
            this.characterCount.Name = "characterCount";
            this.characterCount.Size = new System.Drawing.Size(16, 17);
            this.characterCount.TabIndex = 44;
            this.characterCount.Text = "0";
            this.characterCount.Click += new System.EventHandler(this.label17_Click);
            // 
            // artifactCount
            // 
            this.artifactCount.AutoSize = true;
            this.artifactCount.Location = new System.Drawing.Point(106, 259);
            this.artifactCount.Name = "artifactCount";
            this.artifactCount.Size = new System.Drawing.Size(16, 17);
            this.artifactCount.TabIndex = 45;
            this.artifactCount.Text = "0";
            // 
            // weaponCount
            // 
            this.weaponCount.AutoSize = true;
            this.weaponCount.Location = new System.Drawing.Point(106, 242);
            this.weaponCount.Name = "weaponCount";
            this.weaponCount.Size = new System.Drawing.Size(16, 17);
            this.weaponCount.TabIndex = 46;
            this.weaponCount.Text = "0";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(145, 242);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(12, 17);
            this.label20.TabIndex = 47;
            this.label20.Text = "/";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(145, 259);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(12, 17);
            this.label21.TabIndex = 48;
            this.label21.Text = "/";
            // 
            // maxWeapons
            // 
            this.maxWeapons.AutoSize = true;
            this.maxWeapons.Location = new System.Drawing.Point(163, 242);
            this.maxWeapons.Name = "maxWeapons";
            this.maxWeapons.Size = new System.Drawing.Size(16, 17);
            this.maxWeapons.TabIndex = 49;
            this.maxWeapons.Text = "0";
            // 
            // maxArtifacts
            // 
            this.maxArtifacts.AutoSize = true;
            this.maxArtifacts.Location = new System.Drawing.Point(163, 259);
            this.maxArtifacts.Name = "maxArtifacts";
            this.maxArtifacts.Size = new System.Drawing.Size(16, 17);
            this.maxArtifacts.TabIndex = 50;
            this.maxArtifacts.Text = "0";
            // 
            // ProgramStatus
            // 
            this.ProgramStatus.AutoSize = true;
            this.ProgramStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProgramStatus.ForeColor = System.Drawing.Color.Green;
            this.ProgramStatus.Location = new System.Drawing.Point(201, 10);
            this.ProgramStatus.Name = "ProgramStatus";
            this.ProgramStatus.Size = new System.Drawing.Size(0, 29);
            this.ProgramStatus.TabIndex = 51;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(549, 552);
            this.Controls.Add(this.ProgramStatus);
            this.Controls.Add(this.maxArtifacts);
            this.Controls.Add(this.maxWeapons);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.weaponCount);
            this.Controls.Add(this.artifactCount);
            this.Controls.Add(this.characterCount);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.c_TextBox);
            this.Controls.Add(this.c_Talent_3);
            this.Controls.Add(this.c_Talent_2);
            this.Controls.Add(this.c_Talent_1);
            this.Controls.Add(this.c_Level_Image);
            this.Controls.Add(this.c_Name_Image);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.a_TextBox);
            this.Controls.Add(this.a_Equipped_Image);
            this.Controls.Add(this.a_SetName_Image);
            this.Controls.Add(this.a_SubStat4_Image);
            this.Controls.Add(this.a_SubStat3_Image);
            this.Controls.Add(this.a_SubStat2_Image);
            this.Controls.Add(this.a_SubStat1_Image);
            this.Controls.Add(this.a_Level_Image);
            this.Controls.Add(this.a_MainStat_Image);
            this.Controls.Add(this.a_GearSlot_Image);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClickHere);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Genshin Impact Scanner";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.a_GearSlot_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_MainStat_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_Level_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_SubStat1_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_SubStat2_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_SubStat3_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_SubStat4_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_SetName_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a_Equipped_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_Talent_3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_Talent_2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_Talent_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_Level_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c_Name_Image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClickHere;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.PictureBox a_GearSlot_Image;
        private System.Windows.Forms.PictureBox a_MainStat_Image;
        private System.Windows.Forms.PictureBox a_Level_Image;
        private System.Windows.Forms.PictureBox a_SubStat1_Image;
        private System.Windows.Forms.PictureBox a_SubStat2_Image;
        private System.Windows.Forms.PictureBox a_SubStat3_Image;
        private System.Windows.Forms.PictureBox a_SubStat4_Image;
        private System.Windows.Forms.PictureBox a_SetName_Image;
        private System.Windows.Forms.PictureBox a_Equipped_Image;
        private System.Windows.Forms.TextBox a_TextBox;
        private System.Windows.Forms.TextBox c_TextBox;
        private System.Windows.Forms.PictureBox c_Talent_3;
        private System.Windows.Forms.PictureBox c_Talent_2;
        private System.Windows.Forms.PictureBox c_Talent_1;
        private System.Windows.Forms.PictureBox c_Level_Image;
        private System.Windows.Forms.PictureBox c_Name_Image;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label characterCount;
        private System.Windows.Forms.Label artifactCount;
        private System.Windows.Forms.Label weaponCount;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label maxWeapons;
        private System.Windows.Forms.Label maxArtifacts;
        private System.Windows.Forms.Label ProgramStatus;
    }
}

