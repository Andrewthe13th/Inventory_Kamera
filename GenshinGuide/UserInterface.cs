using System;
using System.Drawing;
using System.Windows.Forms;

namespace GenshinGuide
{
	public static class UserInterface
	{
		// Artifact
		private static PictureBox gearSlot_PictureBox;
		private static PictureBox gearMainStat_PictureBox;
		private static PictureBox gearLevel_PictureBox;
		private static PictureBox[] gearSubStat_PictureBoxes = new PictureBox[4];
		private static PictureBox gearSetName_PictureBox;
		private static PictureBox gearEquipped_PictureBox;
		private static TextBox gear_TextBox;

		// Character
		private static PictureBox cName_PictureBox;
		private static PictureBox cLevel_PictureBox;
		private static PictureBox[] cTalent_PictureBoxes = new PictureBox[3];
		private static TextBox character_TextBox;

		// Counters
		private static Label weaponCount_Label;
		private static Label weaponMax_Label;
		private static Label artifactCount_Label;
		private static Label artifactMax_Label;
		private static Label characterCount_Label;
		private static Label programStatus_Label;

		// Error Log
		private static TextBox error_TextBox;

		// Current Images
		private static PictureBox navigation_PictureBox;

		public static void Init(PictureBox _a_gearSlot, PictureBox _a_mainStat, PictureBox _a_level, PictureBox[] _a_subStats, PictureBox _a_setName, PictureBox _a_equipped, TextBox _a_textbox, PictureBox _c_name, PictureBox _c_level, PictureBox[] _c_talent, TextBox _c_textbox, Label _weaponCount, Label _weaponMax, Label _artifactCount, Label _artifactMax, Label _characterCount, Label _programStatus, TextBox _error_textBox, PictureBox _navigation_Image)
		{
			// Artifacts and Weapons
			gearSlot_PictureBox = _a_gearSlot;
			gearMainStat_PictureBox = _a_mainStat;
			gearLevel_PictureBox = _a_level;
			gearSubStat_PictureBoxes = _a_subStats;
			gearSetName_PictureBox = _a_setName;
			gearEquipped_PictureBox = _a_equipped;
			gear_TextBox = _a_textbox;

			// Characters
			cName_PictureBox = _c_name;
			cLevel_PictureBox = _c_level;
			cTalent_PictureBoxes = _c_talent;
			character_TextBox = _c_textbox;

			// Counters
			weaponCount_Label = _weaponCount;
			weaponMax_Label = _weaponMax;
			artifactCount_Label = _artifactCount;
			artifactMax_Label = _artifactMax;
			characterCount_Label = _characterCount;

			// Status
			programStatus_Label = _programStatus;

			// Error
			error_TextBox = _error_textBox;

			// Navigation Image
			navigation_PictureBox = _navigation_Image;
		}

		public static void SetArtifact_GearSlot(Bitmap bm, string text, bool bWeapon = false)
		{
			MethodInvoker imageAction = delegate
			{
				gearSlot_PictureBox.Image = bm;
				gearSlot_PictureBox.Refresh();
			};
			MethodInvoker textAction;
			if (bWeapon)
			{
				textAction = delegate
				{
					gear_TextBox.AppendText("Weapon: " + text);
					gear_TextBox.AppendText(Environment.NewLine);
					gear_TextBox.Refresh();
				};
			}
			else
			{
				textAction = delegate
				{
					gear_TextBox.AppendText("GearSlot: " + text);
					gear_TextBox.AppendText(Environment.NewLine);
					gear_TextBox.Refresh();
				};
			}
			gearSlot_PictureBox.Invoke(imageAction);
			gear_TextBox.Invoke(textAction);
		}

		private static void UpdateElements(Bitmap bm, string text, PictureBox pictureBox, TextBox textBox)
		{
			UpdatePictureBox(bm, pictureBox);
			UpdateTextBox(text, textBox);
		}

		private static void UpdatePictureBox(Bitmap bm, PictureBox pictureBox)
		{
			MethodInvoker pictureBoxAction = delegate
			{
				pictureBox.Image = bm;
				pictureBox.Refresh();
			};
			pictureBox.Invoke(pictureBoxAction);
		}

		private static void UpdateTextBox(string text, TextBox textBox)
		{
			MethodInvoker textBoxAction = delegate
			{
				textBox.AppendText(text);
				textBox.Refresh();
			};
			textBox.Invoke(textBoxAction);
		}

		private static void UpdateLabel(string text, Label label)
		{
			MethodInvoker labelAction =  delegate
			{
				label.Text = text;
				label.Refresh();
			};
			label.Invoke(labelAction);
		}

		public static void SetArtifact_MainStat(Bitmap bm, string text)
		{
			UpdateElements(bm, $"MainStat: {text}\n", gearMainStat_PictureBox, gear_TextBox);
		}

		public static void SetGear_Level(Bitmap bm, string text, bool bWeapon = false)
		{
			MethodInvoker imageAction = delegate
			{
				gearLevel_PictureBox.Image = bm;
				gearLevel_PictureBox.Refresh();
			};
			MethodInvoker textAction;
			if (bWeapon)
			{
				textAction = delegate
				{
					gear_TextBox.AppendText("Refinement: " + text);
					gear_TextBox.AppendText(Environment.NewLine);
					gear_TextBox.Refresh();
				};
			}
			else
			{
				textAction = delegate
				{
					gear_TextBox.AppendText("Level: " + text);
					gear_TextBox.AppendText(Environment.NewLine);
					gear_TextBox.Refresh();
				};
			}

			gearLevel_PictureBox.Invoke(imageAction);
			gear_TextBox.Invoke(textAction);
		}

		public static void SetArtifact_SubStat(Bitmap bm, string text, int i)
		{
			if (i > -1 && i < 4)
			{
				MethodInvoker imageAction = delegate
				{
					gearSubStat_PictureBoxes[i].Image = bm;
					gearSubStat_PictureBoxes[i].Refresh();
				};
				MethodInvoker textAction = delegate
				{
					gear_TextBox.AppendText("SubStat " + i.ToString() + ": " + text);
					gear_TextBox.AppendText(Environment.NewLine);
					gear_TextBox.Refresh();
				};

				gearSubStat_PictureBoxes[i].Invoke(imageAction);
				gear_TextBox.Invoke(textAction);
			}
		}

		public static void SetArtifact_SetName(Bitmap bm, string text)
		{
			UpdateElements(bm, $"SetName: {text}\n", gearSetName_PictureBox, gear_TextBox);
		}

		public static void SetGear_Equipped(Bitmap bm, string text)
		{
			UpdateElements(bm, $"Equipped: {text}\n", gearEquipped_PictureBox, gear_TextBox);
		}

		public static void SetCharacter_NameAndElement(Bitmap bm, string name, string element)
		{
			string text = $"Name: {name}\nElement: {element}\n";
			UpdateElements(bm, text, cName_PictureBox, character_TextBox);
		}

		public static void SetCharacter_Level(Bitmap bm, int level)
		{
			UpdateElements(bm, $"Level: {level}", cLevel_PictureBox, character_TextBox);
		}

		public static void SetCharacter_Talent(Bitmap bm, string text, int i)
		{
			if (i > -1 && i < 3)
			{
				MethodInvoker imageAction = delegate
				{
					cTalent_PictureBoxes[i].Image = bm;
					cTalent_PictureBoxes[i].Refresh();
				};
				MethodInvoker textAction = delegate
				{
					character_TextBox.AppendText("Talent " + i.ToString() + ": " + text);
					character_TextBox.AppendText(Environment.NewLine);
					character_TextBox.Refresh();
				};

				cTalent_PictureBoxes[i].Invoke(imageAction);
				character_TextBox.Invoke(textAction);
			}
		}

		public static void SetWeapon_Max(int value)
		{
			UpdateLabel(value.ToString(), weaponMax_Label);
		}

		public static void SetArtifact_Max(int value)
		{
			UpdateLabel(value.ToString(), artifactMax_Label);
		}

		public static void IncrementArtifactCount()
		{
			UpdateLabel($"{artifactCount_Label.Text + 1}", artifactCount_Label);
		}

		public static void IncrementWeaponCount()
		{
			UpdateLabel($"{weaponCount_Label.Text + 1}", weaponCount_Label);
		}

		public static void IncrementCharacterCount()
		{
			UpdateLabel($"{characterCount_Label.Text + 1}", characterCount_Label);
		}

		public static void SetProgramStatus(string status)
		{
			MethodInvoker statusAction;

			statusAction = delegate
			{
				programStatus_Label.Text = status;
				programStatus_Label.ForeColor = Color.Green;
				programStatus_Label.Font = new Font(programStatus_Label.Font.FontFamily, 15);
				programStatus_Label.Location = new Point(44, 278);
				programStatus_Label.Refresh();
			};

			programStatus_Label.Invoke(statusAction);
		}

		public static void AddError(string error)
		{
			MethodInvoker textAction = delegate
			{
				error_TextBox.AppendText(error);
				error_TextBox.AppendText(Environment.NewLine);
				error_TextBox.Refresh();
			};

			error_TextBox.Invoke(textAction);
		}

		public static void SetNavigation_Image(Bitmap bm)
		{
			UpdatePictureBox(bm, navigation_PictureBox);
		}

		public static void ResetCharacterDisplay()
		{
			MethodInvoker nameAction = delegate { cName_PictureBox.Image = null; };
			MethodInvoker levelAction = delegate { cLevel_PictureBox.Image = null; };
			MethodInvoker talentAction_1 = delegate { cTalent_PictureBoxes[0].Image = null; };
			MethodInvoker talentAction_2 = delegate { cTalent_PictureBoxes[1].Image = null; };
			MethodInvoker talentAction_3 = delegate { cTalent_PictureBoxes[2].Image = null; };
			MethodInvoker textAction = delegate { character_TextBox.Clear(); };

			cName_PictureBox.Invoke(nameAction);
			cLevel_PictureBox.Invoke(levelAction);
			cTalent_PictureBoxes[0].Invoke(talentAction_1);
			cTalent_PictureBoxes[1].Invoke(talentAction_2);
			cTalent_PictureBoxes[2].Invoke(talentAction_3);
			character_TextBox.Invoke(textAction);
		}

		public static void ResetGearDisplay()
		{
			MethodInvoker gearSlotAction = delegate { gearSlot_PictureBox.Image = null; };
			MethodInvoker mainStatAction = delegate { gearMainStat_PictureBox.Image = null; };
			MethodInvoker levelAction = delegate { gearLevel_PictureBox.Image = null; };
			MethodInvoker subStatsAction_1 = delegate { gearSubStat_PictureBoxes[0].Image = null; };
			MethodInvoker subStatsAction_2 = delegate { gearSubStat_PictureBoxes[1].Image = null; };
			MethodInvoker subStatsAction_3 = delegate { gearSubStat_PictureBoxes[2].Image = null; };
			MethodInvoker subStatsAction_4 = delegate { gearSubStat_PictureBoxes[3].Image = null; };
			MethodInvoker setNameAction = delegate { gearSetName_PictureBox.Image = null; };
			MethodInvoker equippedAction = delegate { gearEquipped_PictureBox.Image = null; };
			MethodInvoker textAction = delegate { gear_TextBox.Clear(); };

			gearSlot_PictureBox.Invoke(gearSlotAction);
			gearMainStat_PictureBox.Invoke(mainStatAction);
			gearLevel_PictureBox.Invoke(levelAction);
			gearSubStat_PictureBoxes[0].Invoke(subStatsAction_1);
			gearSubStat_PictureBoxes[1].Invoke(subStatsAction_2);
			gearSubStat_PictureBoxes[2].Invoke(subStatsAction_3);
			gearSubStat_PictureBoxes[3].Invoke(subStatsAction_4);
			gearSetName_PictureBox.Invoke(setNameAction);
			gearEquipped_PictureBox.Invoke(equippedAction);
			gear_TextBox.Invoke(textAction);
		}

		public static void ResetAll()
		{
			// Counters
			MethodInvoker characterCountAction = delegate { characterCount_Label.Text = "0"; weaponMax_Label.Refresh(); };
			MethodInvoker weaponCountAction = delegate { weaponCount_Label.Text = "0"; weaponMax_Label.Refresh(); };
			MethodInvoker weaponMaxAction = delegate { weaponMax_Label.Text = "?"; weaponMax_Label.Refresh(); };
			MethodInvoker artifactCountAction = delegate { artifactCount_Label.Text = "0"; artifactMax_Label.Refresh(); };
			MethodInvoker artifactMaxAction = delegate { artifactMax_Label.Text = "?"; artifactMax_Label.Refresh(); };
			// Images
			MethodInvoker gearSlotAction = delegate { gearSlot_PictureBox.Image = null; };
			MethodInvoker mainStatAction = delegate { gearMainStat_PictureBox.Image = null; };
			MethodInvoker artifactlevelAction = delegate { gearLevel_PictureBox.Image = null; };
			MethodInvoker subStatsAction_1 = delegate { gearSubStat_PictureBoxes[0].Image = null; };
			MethodInvoker subStatsAction_2 = delegate { gearSubStat_PictureBoxes[1].Image = null; };
			MethodInvoker subStatsAction_3 = delegate { gearSubStat_PictureBoxes[2].Image = null; };
			MethodInvoker subStatsAction_4 = delegate { gearSubStat_PictureBoxes[3].Image = null; };
			MethodInvoker setNameAction = delegate { gearSetName_PictureBox.Image = null; };
			MethodInvoker equippedAction = delegate { gearEquipped_PictureBox.Image = null; };
			MethodInvoker artifactAction = delegate { gear_TextBox.Clear(); };
			MethodInvoker nameAction = delegate { cName_PictureBox.Image = null; };
			MethodInvoker levelAction = delegate { cLevel_PictureBox.Image = null; };
			MethodInvoker talentAction_1 = delegate { cTalent_PictureBoxes[0].Image = null; };
			MethodInvoker talentAction_2 = delegate { cTalent_PictureBoxes[1].Image = null; };
			MethodInvoker talentAction_3 = delegate { cTalent_PictureBoxes[2].Image = null; };
			MethodInvoker characterAction = delegate { character_TextBox.Clear(); };
			//Error
			MethodInvoker errorAction = delegate { error_TextBox.Clear(); };

			characterCount_Label.Invoke(characterCountAction);
			weaponCount_Label.Invoke(weaponCountAction);
			weaponMax_Label.Invoke(weaponMaxAction);
			artifactCount_Label.Invoke(artifactCountAction);
			artifactMax_Label.Invoke(artifactMaxAction);
			gearSlot_PictureBox.Invoke(gearSlotAction);
			gearMainStat_PictureBox.Invoke(mainStatAction);
			gearLevel_PictureBox.Invoke(artifactlevelAction);
			gearSubStat_PictureBoxes[0].Invoke(subStatsAction_1);
			gearSubStat_PictureBoxes[1].Invoke(subStatsAction_2);
			gearSubStat_PictureBoxes[2].Invoke(subStatsAction_3);
			gearSubStat_PictureBoxes[3].Invoke(subStatsAction_4);
			gearSetName_PictureBox.Invoke(setNameAction);
			gearEquipped_PictureBox.Invoke(equippedAction);
			gear_TextBox.Invoke(artifactAction);
			cName_PictureBox.Invoke(nameAction);
			cLevel_PictureBox.Invoke(levelAction);
			cTalent_PictureBoxes[0].Invoke(talentAction_1);
			cTalent_PictureBoxes[1].Invoke(talentAction_2);
			cTalent_PictureBoxes[2].Invoke(talentAction_3);
			character_TextBox.Invoke(characterAction);
			error_TextBox.Invoke(errorAction);
		}

		public static void ClearErrors()
		{
			MethodInvoker textAction = delegate { error_TextBox.Clear(); };

			error_TextBox.Invoke(textAction);
		}

	}
}
