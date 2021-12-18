using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace InventoryKamera
{
	public static class UserInterface
	{
		// Artifacts and Weapons
		private static PictureBox gear_PictureBox;

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

		// Status
		private static Label programStatus_Label;

		// Error box
		private static TextBox error_TextBox;

		// Current Images
		private static PictureBox navigation_PictureBox;

		public static void Init(PictureBox _gear_PictureBox, TextBox _a_textbox, PictureBox _c_name, PictureBox _c_level, PictureBox[] _c_talent, TextBox _c_textbox, Label _weaponCount, Label _weaponMax, Label _artifactCount, Label _artifactMax, Label _characterCount, Label _programStatus, TextBox _error_textBox, PictureBox _navigation_Image)
		{
			// Artifacts and Weapons
			gear_PictureBox = _gear_PictureBox;
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

		private static void UpdateElements(Bitmap bm, string text, PictureBox pictureBox, TextBox textBox)
		{
			UpdatePictureBox(bm, pictureBox);
			UpdateTextBox(text, textBox);
		}

		private static void UpdatePictureBox(Bitmap bm, PictureBox pictureBox)
		{
			try
			{
				Bitmap clone = new Bitmap(bm.Width, bm.Height);
				using (var copy = Graphics.FromImage(clone))
				{
					copy.DrawImage(bm, 0, 0);
				}
				MethodInvoker pictureBoxAction = delegate
			{
				pictureBox.Image = clone;
				pictureBox.Refresh();
			};
				pictureBox.Invoke(pictureBoxAction);
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Problem updating picturebox {pictureBox.Name}\n{e.StackTrace}");
			}
		}

		private static void UpdateTextBox(string text, TextBox textBox)
		{
			try
			{
				MethodInvoker textBoxAction = delegate
				{
					textBox.AppendText(text);
					textBox.AppendText(Environment.NewLine);
					textBox.Refresh();
				};
				textBox.Invoke(textBoxAction);
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Problem updating textbox {textBox.Name}\n{e.StackTrace}");
			}
		}

		private static void UpdateLabel(string text, Label label)
		{
			try
			{
				MethodInvoker labelAction =  delegate
			{
				label.Text = text;
				label.Refresh();
			};
				label.Invoke(labelAction);
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Problem updating label {label.Name}\n{e.StackTrace}");
			}
		}

		public static void SetGear(Bitmap bm, Weapon weapon)
		{
			ResetGearDisplay();
			UpdatePictureBox(bm, gear_PictureBox);
			int maxLevel = (int)( weapon.Ascended ? Math.Floor(weapon.Level / 10.0) + 1 * 10 : Math.Floor(weapon.Level / 10.0) * 10 );
			string text =
				$"Name: {weapon.Name}" + Environment.NewLine +
				$"Level: {weapon.Level} / {maxLevel}" + Environment.NewLine +
				$"Refinement: {weapon.RefinementLevel}" + Environment.NewLine +
				$"Equipped: {weapon.EquippedCharacter}";
			UpdateTextBox(text, gear_TextBox);
		}

		public static void SetGear(Bitmap bm, Artifact artifact)
		{
			ResetGearDisplay();
			UpdatePictureBox(bm, gear_PictureBox);
			string text =
				$"Rarity: {artifact.Rarity}" + Environment.NewLine +
				$"Level: {artifact.Level}" + Environment.NewLine +
				$"Set: {artifact.SetName}" + Environment.NewLine +
				$"Slot: {artifact.GearSlot}" + Environment.NewLine +
				$"Stat: {artifact.MainStat}" + Environment.NewLine +
				$"SubStats:" + Environment.NewLine;

			for (int i = 0; i < artifact.SubStats.Length; i++)
			{
				Artifact.SubStat substat = artifact.SubStats[i];
				if (string.IsNullOrEmpty(substat.stat)) break;
				text += $"{i + 1}: {substat}" + Environment.NewLine;
			}

			text += $"Equipped: {artifact.EquippedCharacter}" + Environment.NewLine +
				$"Locked: {artifact.Lock}";

			UpdateTextBox(text, gear_TextBox);
		}

		internal static void SetMainCharacterName(string text)
		{
			UpdateTextBox($"Traveler name: {text}", character_TextBox);
		}

		public static void SetCharacter_NameAndElement(Bitmap bm, string name, string element)
		{
			UpdateElements(bm, $"Name: {name}\nElement: {element}", cName_PictureBox, character_TextBox);
		}

		public static void SetCharacter_Level(Bitmap bm, int level, int maxLevel)
		{
			UpdateElements(bm, $"Level: {level} / {maxLevel}", cLevel_PictureBox, character_TextBox);
		}

		public static void SetCharacter_Constellation(int level)
		{
			UpdateTextBox($"Constellation: {level}", character_TextBox);
		}

		internal static void SetMaterial(Bitmap nameplate, Bitmap quantity, string name, int count)
		{
			UpdateElements(nameplate, $"Name: {name}", cName_PictureBox, character_TextBox);
			UpdateElements(quantity, $"Count: {count}", cLevel_PictureBox, character_TextBox);
		}

		public static void SetCharacter_Talent(Bitmap bm, string text, int i)
		{
			if (i > -1 && i < 3)
			{
				UpdatePictureBox(bm, cTalent_PictureBoxes[i]);
				UpdateTextBox($"Talent {i + 1}: {text}", character_TextBox);
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
			UpdateLabel($"{Int32.Parse(artifactCount_Label.Text) + 1}", artifactCount_Label);
		}

		public static void IncrementWeaponCount()
		{
			UpdateLabel($"{Int32.Parse(weaponCount_Label.Text) + 1}", weaponCount_Label);
		}

		public static void IncrementCharacterCount()
		{
			UpdateLabel($"{Int32.Parse(characterCount_Label.Text) + 1}", characterCount_Label);
		}

		public static void SetProgramStatus(string status, bool ok = true)
		{
			MethodInvoker statusAction = delegate
			{
				programStatus_Label.Text = status;
				programStatus_Label.ForeColor = ok ? Color.Green : Color.Red;
				programStatus_Label.Font = new Font(programStatus_Label.Font.FontFamily, 15);
				programStatus_Label.Refresh();
			};

			programStatus_Label.Invoke(statusAction);
		}

		public static void AddError(string error)
		{
			UpdateTextBox($"{error.Replace("\n", Environment.NewLine)}" + Environment.NewLine, error_TextBox);
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
			MethodInvoker gearAction = delegate { gear_PictureBox.Image = null; };

			MethodInvoker textAction = delegate { gear_TextBox.Clear(); };

			gear_PictureBox.Invoke(gearAction);
			gear_TextBox.Invoke(textAction);
		}

		public static void ResetCounters()
		{
			MethodInvoker characterCountAction = delegate { characterCount_Label.Text = "0"; weaponMax_Label.Refresh(); };
			MethodInvoker weaponCountAction = delegate { weaponCount_Label.Text = "0"; weaponMax_Label.Refresh(); };
			MethodInvoker weaponMaxAction = delegate { weaponMax_Label.Text = "?"; weaponMax_Label.Refresh(); };
			MethodInvoker artifactCountAction = delegate { artifactCount_Label.Text = "0"; artifactMax_Label.Refresh(); };
			MethodInvoker artifactMaxAction = delegate { artifactMax_Label.Text = "?"; artifactMax_Label.Refresh(); };

			characterCount_Label.Invoke(characterCountAction);
			weaponCount_Label.Invoke(weaponCountAction);
			weaponMax_Label.Invoke(weaponMaxAction);
			artifactCount_Label.Invoke(artifactCountAction);
			artifactMax_Label.Invoke(artifactMaxAction);
		}

		public static void ResetErrors()
		{
			MethodInvoker textAction = delegate { error_TextBox.Clear(); };

			error_TextBox.Invoke(textAction);
		}

		public static void ResetAll()
		{
			ResetGearDisplay();

			ResetCharacterDisplay();

			ResetCounters();

			ResetErrors();
		}
	}
}