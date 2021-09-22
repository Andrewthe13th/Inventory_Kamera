using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace GenshinGuide
{
    public static class UserInterface
    {
        // Artifact
        private static PictureBox a_gearSlot;
        private static PictureBox a_mainStat;
        private static PictureBox a_level;
        private static PictureBox[] a_subStats = new PictureBox[4];
        private static PictureBox a_setName;
        private static PictureBox a_equipped;
        private static TextBox a_textBox;

        // Character
        private static PictureBox c_name;
        private static PictureBox c_level;
        private static PictureBox[] c_talent = new PictureBox[3];
        private static TextBox c_textBox;

        // Counters
        private static Label weaponCount;
        private static Label weaponMax;
        private static Label artifactCount;
        private static Label artifactMax;
        private static Label characterCount;
        private static Label programStatus;

        // Error Log
        private static TextBox error_textBox;

        public static void Init(PictureBox _a_gearSlot, PictureBox _a_mainStat, PictureBox _a_level, PictureBox[] _a_subStats, PictureBox _a_setName, PictureBox _a_equipped, TextBox _a_textbox, PictureBox _c_name, PictureBox _c_level, PictureBox[] _c_talent, TextBox _c_textbox, Label _weaponCount, Label _weaponMax, Label _artifactCount, Label _artifactMax, Label _characterCount, Label _programStatus, TextBox _error_textBox)
        {
            // Artifact
            a_gearSlot = _a_gearSlot;
            a_mainStat = _a_mainStat;
            a_level = _a_level;
            a_subStats = _a_subStats;
            a_setName = _a_setName;
            a_equipped = _a_equipped;
            a_textBox = _a_textbox;
            // Character
            c_name = _c_name;
            c_level = _c_level;
            c_talent = _c_talent;
            c_textBox = _c_textbox;
            // Counters
            weaponCount = _weaponCount;
            weaponMax = _weaponMax;
            artifactCount = _artifactCount;
            artifactMax = _artifactMax;
            characterCount = _characterCount;
            // Status
            programStatus = _programStatus;
            // Error
            error_textBox = _error_textBox;
        }

        public static void SetArtifact_GearSlot(Bitmap bm, string text, bool bWeapon = false)
        {
            MethodInvoker imageAction = delegate
            {
                a_gearSlot.Image = bm;
                a_gearSlot.Refresh();
            };
            MethodInvoker textAction;
            if (bWeapon)
            {
                textAction = delegate
                {
                    a_textBox.AppendText("Weapon: " + text);
                    a_textBox.AppendText(Environment.NewLine);
                    a_textBox.Refresh();
                };
            }
            else
            {
                textAction = delegate
                {
                    a_textBox.AppendText("GearSlot: " + text);
                    a_textBox.AppendText(Environment.NewLine);
                    a_textBox.Refresh();
                };
            }

            a_gearSlot.Invoke(imageAction);
            a_textBox.Invoke(textAction);
        }

        public static void SetArtifact_MainStat(Bitmap bm, string text, bool bWeapon = false)
        {
            MethodInvoker imageAction = delegate
            {
                a_mainStat.Image = bm;
                a_mainStat.Refresh();
            };
            MethodInvoker textAction;
            if (bWeapon)
            {
                textAction = delegate
                {
                    a_textBox.AppendText("Level: " + text);
                    a_textBox.AppendText(Environment.NewLine);
                    a_textBox.Refresh();
                };
            }
            else
            {
                textAction = delegate
                {
                    a_textBox.AppendText("MainStat: " + text);
                    a_textBox.AppendText(Environment.NewLine);
                    a_textBox.Refresh();
                };
            }

            a_mainStat.Invoke(imageAction);
            a_textBox.Invoke(textAction);
        }

        public static void SetArtifact_Level(Bitmap bm, string text, bool bWeapon = false)
        {
            MethodInvoker imageAction = delegate
            {
                a_level.Image = bm;
                a_level.Refresh();
            };
            MethodInvoker textAction;
            if (bWeapon)
            {
                textAction = delegate
                {
                    a_textBox.AppendText("Refinement: " + text);
                    a_textBox.AppendText(Environment.NewLine);
                    a_textBox.Refresh();
                };
            }
            else
            {
                textAction = delegate
                {
                    a_textBox.AppendText("Level: " + text);
                    a_textBox.AppendText(Environment.NewLine);
                    a_textBox.Refresh();
                };
            }

            a_level.Invoke(imageAction);
            a_textBox.Invoke(textAction);
        }

        public static void SetArtifact_SubStat(Bitmap bm, string text, int i)
        {
            if (i > -1 && i < 4)
            {
                MethodInvoker imageAction = delegate
                {
                    a_subStats[i].Image = bm;
                    a_subStats[i].Refresh();
                };
                MethodInvoker textAction = delegate
                {
                    a_textBox.AppendText("SubStat " + i.ToString() + ": " + text);
                    a_textBox.AppendText(Environment.NewLine);
                    a_textBox.Refresh();
                };

                a_subStats[i].Invoke(imageAction);
                a_textBox.Invoke(textAction);
            }
        }

        public static void SetArtifact_SetName(Bitmap bm, string text)
        {
            MethodInvoker imageAction = delegate
            {
                a_setName.Image = bm;
                a_setName.Refresh();
            };
            MethodInvoker textAction = delegate
            {
                a_textBox.AppendText("SetName: " + text);
                a_textBox.AppendText(Environment.NewLine);
                a_textBox.Refresh();
            };

            a_setName.Invoke(imageAction);
            a_textBox.Invoke(textAction);
        }

        public static void SetArtifact_Equipped(Bitmap bm, string text)
        {
            MethodInvoker imageAction = delegate
            {
                a_equipped.Image = bm;
                a_equipped.Refresh();
            };
            MethodInvoker textAction = delegate
            {
                a_textBox.AppendText("Equipped: " + text);
                a_textBox.AppendText(Environment.NewLine);
                a_textBox.Refresh();
            };

            a_equipped.Invoke(imageAction);
            a_textBox.Invoke(textAction);
        }

        public static void Reset_Artifact()
        {
            MethodInvoker gearSlotAction = delegate { a_gearSlot.Image = null; };
            MethodInvoker mainStatAction = delegate { a_mainStat.Image = null; };
            MethodInvoker levelAction = delegate { a_level.Image = null; };
            MethodInvoker subStatsAction_1 = delegate { a_subStats[0].Image = null; };
            MethodInvoker subStatsAction_2 = delegate { a_subStats[1].Image = null; };
            MethodInvoker subStatsAction_3 = delegate { a_subStats[2].Image = null; };
            MethodInvoker subStatsAction_4 = delegate { a_subStats[3].Image = null; };
            MethodInvoker setNameAction = delegate { a_setName.Image = null; };
            MethodInvoker equippedAction = delegate { a_equipped.Image = null; };
            MethodInvoker textAction = delegate { a_textBox.Text = ""; };

            a_gearSlot.Invoke(gearSlotAction);
            a_mainStat.Invoke(mainStatAction);
            a_level.Invoke(levelAction);
            a_subStats[0].Invoke(subStatsAction_1);
            a_subStats[1].Invoke(subStatsAction_2);
            a_subStats[2].Invoke(subStatsAction_3);
            a_subStats[3].Invoke(subStatsAction_4);
            a_setName.Invoke(setNameAction);
            a_equipped.Invoke(equippedAction);
            a_textBox.Invoke(textAction);
        }

        public static void SetCharacter_NameAndElement(Bitmap bm, string name,string element)
        {
            MethodInvoker imageAction = delegate
            {
                c_name.Image = bm;
                c_name.Refresh();
            };
            MethodInvoker textAction = delegate
            {
                c_textBox.AppendText("Name: " + name);
                c_textBox.AppendText(Environment.NewLine);
                c_textBox.AppendText("Element: " + element);
                c_textBox.AppendText(Environment.NewLine);
                c_textBox.Refresh();
            };

            c_name.Invoke(imageAction);
            c_textBox.Invoke(textAction);
        }

        public static void SetCharacter_Level(Bitmap bm, int level)
        {
            MethodInvoker imageAction = delegate
            {
                c_level.Image = bm;
                c_level.Refresh();
            };
            MethodInvoker textAction = delegate
            {
                c_textBox.AppendText("Level: " + level.ToString());
                c_textBox.AppendText(Environment.NewLine);
                c_textBox.Refresh();
            };

            c_level.Invoke(imageAction);
            c_textBox.Invoke(textAction);
        }

        public static void SetCharacter_Talent(Bitmap bm, string text, int i)
        {
            if (i > -1 && i < 3)
            {
                MethodInvoker imageAction = delegate
                {
                    c_talent[i].Image = bm;
                    c_talent[i].Refresh();
                };
                MethodInvoker textAction = delegate
                {
                    c_textBox.AppendText("Talent " + i.ToString() + ": " + text);
                    c_textBox.AppendText(Environment.NewLine);
                    c_textBox.Refresh();
                };

                c_talent[i].Invoke(imageAction);
                c_textBox.Invoke(textAction);
            }
        }

        public static void Reset_Character()
        {
            MethodInvoker nameAction = delegate { c_name.Image = null; };
            MethodInvoker levelAction = delegate { c_level.Image = null; };
            MethodInvoker talentAction_1 = delegate { c_talent[0].Image = null; };
            MethodInvoker talentAction_2 = delegate { c_talent[1].Image = null; };
            MethodInvoker talentAction_3 = delegate { c_talent[2].Image = null; };
            MethodInvoker textAction = delegate { c_textBox.Text = ""; };

            c_name.Invoke(nameAction);
            c_level.Invoke(levelAction);
            c_talent[0].Invoke(talentAction_1);
            c_talent[1].Invoke(talentAction_2);
            c_talent[2].Invoke(talentAction_3);
            c_textBox.Invoke(textAction);
        }

        public static void IncrementWeaponCount()
        {
            MethodInvoker countAction = delegate
            {
                weaponCount.Text = (Int32.Parse(weaponCount.Text) + 1).ToString();
                weaponCount.Refresh();
            };

            weaponCount.Invoke(countAction);
        }

        public static void SetWeapon_Max(int max)
        {
            MethodInvoker countAction = delegate
            {
                weaponMax.Text = max.ToString();
                weaponMax.Refresh();
            };

            weaponMax.Invoke(countAction);
        }

        public static void IncrementArtifactCount()
        {
            MethodInvoker countAction = delegate
            {
                artifactCount.Text = (Int32.Parse(artifactCount.Text) + 1).ToString();
                artifactCount.Refresh();
            };

            artifactCount.Invoke(countAction);
        }

        public static void SetArtifact_Max(int max)
        {
            MethodInvoker countAction = delegate { artifactMax.Text = max.ToString(); artifactMax.Refresh();};

            artifactMax.Invoke(countAction);
        }

        public static void IncrementCharacterCount()
        {
            MethodInvoker countAction = delegate
            {
                characterCount.Text = (Int32.Parse(characterCount.Text) + 1).ToString();
                characterCount.Refresh();
            };

            characterCount.Invoke(countAction);
        }

        public static void SetProgramStatus(string status, bool b_crash = false)
        {
            MethodInvoker statusAction;
            if (!b_crash) {
                statusAction = delegate
                {
                    programStatus.Text = status;
                    programStatus.ForeColor = Color.Green;
                    programStatus.Font = new Font(programStatus.Font.FontFamily, 15);
                    programStatus.Location = new Point(40, 164);
                    programStatus.Refresh();
                };
            }
            else
            {
                statusAction = delegate
                {
                    programStatus.Text = "Error: " + status;
                    programStatus.ForeColor = Color.Red;
                    programStatus.Font = new Font(programStatus.Font.FontFamily, 8);
                    programStatus.Location = new Point(31, 166);
                    programStatus.Refresh();
                };
            }

            programStatus.Invoke(statusAction);
        }

        public static void AddError(string error)
        {
            MethodInvoker textAction = delegate
            {
                error_textBox.AppendText("Error: " + error);
                error_textBox.AppendText(Environment.NewLine);
                error_textBox.Refresh();
            };

            error_textBox.Invoke(textAction);
        }

        public static void Reset_All()
        {
            // Counters
            MethodInvoker characterCountAction = delegate { characterCount.Text = "0"; weaponMax.Refresh(); };
            MethodInvoker weaponCountAction = delegate { weaponCount.Text = "0"; weaponMax.Refresh(); };
            MethodInvoker weaponMaxAction = delegate { weaponMax.Text = "?"; weaponMax.Refresh(); };
            MethodInvoker artifactCountAction = delegate { artifactCount.Text = "0"; artifactMax.Refresh(); };
            MethodInvoker artifactMaxAction = delegate { artifactMax.Text = "?"; artifactMax.Refresh(); };
            // Images
            MethodInvoker gearSlotAction = delegate { a_gearSlot.Image = null; };
            MethodInvoker mainStatAction = delegate { a_mainStat.Image = null; };
            MethodInvoker artifactlevelAction = delegate { a_level.Image = null; };
            MethodInvoker subStatsAction_1 = delegate { a_subStats[0].Image = null; };
            MethodInvoker subStatsAction_2 = delegate { a_subStats[1].Image = null; };
            MethodInvoker subStatsAction_3 = delegate { a_subStats[2].Image = null; };
            MethodInvoker subStatsAction_4 = delegate { a_subStats[3].Image = null; };
            MethodInvoker setNameAction = delegate { a_setName.Image = null; };
            MethodInvoker equippedAction = delegate { a_equipped.Image = null; };
            MethodInvoker artifactAction = delegate { a_textBox.Text = ""; };
            MethodInvoker nameAction = delegate { c_name.Image = null; };
            MethodInvoker levelAction = delegate { c_level.Image = null; };
            MethodInvoker talentAction_1 = delegate { c_talent[0].Image = null; };
            MethodInvoker talentAction_2 = delegate { c_talent[1].Image = null; };
            MethodInvoker talentAction_3 = delegate { c_talent[2].Image = null; };
            MethodInvoker characterAction = delegate { c_textBox.Text = ""; };
            //Error
            MethodInvoker errorAction = delegate { error_textBox.Text = ""; };

            characterCount.Invoke(characterCountAction);
            weaponCount.Invoke(weaponCountAction);
            weaponMax.Invoke(weaponMaxAction);
            artifactCount.Invoke(artifactCountAction);
            artifactMax.Invoke(artifactMaxAction);
            a_gearSlot.Invoke(gearSlotAction);
            a_mainStat.Invoke(mainStatAction);
            a_level.Invoke(artifactlevelAction);
            a_subStats[0].Invoke(subStatsAction_1);
            a_subStats[1].Invoke(subStatsAction_2);
            a_subStats[2].Invoke(subStatsAction_3);
            a_subStats[3].Invoke(subStatsAction_4);
            a_setName.Invoke(setNameAction);
            a_equipped.Invoke(equippedAction);
            a_textBox.Invoke(artifactAction);
            c_name.Invoke(nameAction);
            c_level.Invoke(levelAction);
            c_talent[0].Invoke(talentAction_1);
            c_talent[1].Invoke(talentAction_2);
            c_talent[2].Invoke(talentAction_3);
            c_textBox.Invoke(characterAction);
            error_textBox.Invoke(errorAction);
        }

        public static void Reset_Error()
        {
            MethodInvoker textAction = delegate { error_textBox.Text = ""; };

            error_textBox.Invoke(textAction);
        }

    }
}
