using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace GenshinGuide
{
    public static class ArtifactScraper
    {

        public static List<Artifact> ScanArtifacts(ref List<Artifact> equipped)
        {
            List<Artifact> artifacts = new List<Artifact>();

            // Get Max artifacts from screen
            int artifactCount = ScanArtifactCount();
            //int artifactCount = 29;
            int currentArtifactCount = 0;
            int scrollCount = 0;

            // Where in screen space artifacts are
            Double artifactLocation_X = (Double)Navigation.GetArea().right * ((Double)21 / (Double)160);
            Double artifactLocation_Y = (Double)Navigation.GetArea().bottom * ((Double)14 / (Double)90);
            Bitmap bm = new Bitmap(130, 130);
            Graphics g = Graphics.FromImage(bm);
            int maxColumns = 7;
            int maxRows = 4;
            int currentColumn = 0;

            // offset used to move mouse to other artifacts
            int xOffset = Convert.ToInt32((Double)Navigation.GetArea().right * ((Double)12.25 / (Double)160));
            int yOffset = Convert.ToInt32((Double)Navigation.GetArea().bottom * ((Double)14.5 / (Double)90));

            // Testing for single artifacts. REMOVE LATER!!!
            //Artifact a = ScanArtifact(Navigation.GetArea(), Navigation.GetPosition(), imageDisplay, textBox);
            //a.DebugPrintArtifact();
            //a.TextBoxPrintArtifact(textBox);

            ///*
            // Go through artifact list
            while ( currentArtifactCount < artifactCount )
            {
                if ( (artifactCount - currentArtifactCount <= (maxRows * maxColumns)) && (currentColumn == 0))
                {
                    break;
                }

                // Select Artifact
                Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(artifactLocation_X) + (xOffset * (currentArtifactCount % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(artifactLocation_Y));
                Navigation.sim.Mouse.LeftButtonClick();
                Navigation.SystemRandomWait();

                // Scan Artifact
                Artifact a = ScanArtifact(currentArtifactCount);
                currentArtifactCount++;
                currentColumn++;

                // Add artifact to equipped list
                if(a.GetEquippedCharacter() != 0)
                {
                    equipped.Add(a);
                }

                // Add to Artifact List Object
                artifacts.Add(a);
                GC.Collect();

                // reach end of row
                if (currentColumn == maxColumns)
                {
                    // reset mouse pointer and scroll down artifact list
                    currentColumn = 0;
                    scrollCount++;

                    // scroll down
                    for (int k = 0; k < 10; k++)
                    {
                        Navigation.sim.Mouse.VerticalScroll(-1);
                        // skip a scroll
                        if ((k == 7) && ((scrollCount % 3) == 0))
                        {
                            k++;
                            if (scrollCount % 9 == 0)
                            {
                                if ( scrollCount == 18)
                                {
                                    scrollCount = 0;
                                } else
                                {
                                    Navigation.sim.Mouse.VerticalScroll(-1);
                                }
                            }
                        }
                    }
                }
            };

                // scroll down as much as possible
                for (int i = 0; i < 20; i++)
                    {
                        Navigation.sim.Mouse.VerticalScroll(-1);
                    }

                // Get artifacts on bottom of page
                int rowsLeft = (int)Math.Ceiling( (double)(artifactCount - currentArtifactCount) / (double)maxRows);
                for (int i = 1; i < rowsLeft; i++)
                {
                    for (int k = 0; k < maxColumns; k++)
                    {
                        if(artifactCount - currentArtifactCount <= 0)
                        {
                            break;
                        }
                        Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(artifactLocation_X) + (xOffset * (k % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(artifactLocation_Y) + (yOffset * (i % (maxRows+1))));
                        Navigation.sim.Mouse.LeftButtonClick();
                        Navigation.SystemRandomWait();

                        // Scan Artifact
                        Artifact a = ScanArtifact(currentArtifactCount);
                        currentArtifactCount++;

                        // Add to Artifact List Object
                        artifacts.Add(a);
                    }
                }//*/

            return artifacts;

        }

        private static int ScanArtifactCount()
        {
            //Find artifact count
            Double artifactCountLocation_X = (Double)Navigation.GetArea().right * ((Double)130 / (Double)160);
            Double artifactCountLocation_Y = (Double)Navigation.GetArea().bottom * ((Double)3 / (Double)90);
            Bitmap bm = new Bitmap(160, 16);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(Navigation.GetPosition().left + Convert.ToInt32(artifactCountLocation_X), Navigation.GetPosition().top + Convert.ToInt32(artifactCountLocation_Y), 0, 0, bm.Size);
            string text = Scraper.AnalyzeText(bm);

            //picTarget.Image = bm;
            //txtOutput.Text = Scraper.AnalyzeText(bm);

            return Int32.Parse(text.Split()[1].Split('/')[0]);
        }

        private static Artifact ScanArtifact(int id)
        {
            // Init Variables
            int gearSlot = 0;
            int mainStat = 0;
            decimal mainStatValue = 0;
            int level = 0;
            Artifact.SubStats[] subStats = new Artifact.SubStats[4];
            int subStatsCount = 0;
            int setName = 0;
            int equippedCharacter = 0;


            // Grab Image of Entire Artifact on Right
            Double artifactLocation_X = (Double)Navigation.GetArea().right * ((Double)108 / (Double)160);
            Double artifactLocation_Y = (Double)Navigation.GetArea().bottom * ((Double)10 / (Double)90);
            int width = 325; int height = 560;
            Bitmap bm = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bm);
            int screenLocation_X = Navigation.GetPosition().left + Convert.ToInt32(artifactLocation_X);
            int screenLocation_Y = Navigation.GetPosition().top + Convert.ToInt32(artifactLocation_Y);
            g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

            // Display Image
            UserInterface.SetImage(bm);

            // Get Rarity (Check color)
            int rarity = 0;
            Color rarityColor = bm.GetPixel(12, 10);
            //textBox.Text = rarityColor.A + " " + rarityColor.R + " " + rarityColor.G + " " + rarityColor.B;
            Color fiveStar = Color.FromArgb(255, 188, 105, 50);
            Color fourthStar = Color.FromArgb(255, 161, 86, 224);
            Color thirdStar = Color.FromArgb(255, 81, 127, 203);
            Color twoStar = Color.FromArgb(255, 42, 143, 114);
            Color firstStar = Color.FromArgb(255, 114, 119, 138);

            if (fiveStar == rarityColor || fourthStar == rarityColor)
            {
                rarity = (fiveStar == rarityColor) ? 5 : 4;

                // Improved Scanning using multi threading
                Thread thr1 = new Thread(() => gearSlot = ScanArtifactGearSlot(screenLocation_X, screenLocation_Y, width, height));
                Thread thr2 = new Thread(() => mainStat = ScanArtifactMainStat(screenLocation_X, screenLocation_Y, width, height, gearSlot));
                Thread thr3 = new Thread(() => mainStatValue = ScanArtifactMainStatValue(screenLocation_X, screenLocation_Y, width, height));
                Thread thr4 = new Thread(() => level = ScanArtifactLevel(screenLocation_X, screenLocation_Y, width, height));
                Thread thr5 = new Thread(() => subStats = ScanArtifactSubStats(screenLocation_X, screenLocation_Y, width, height, ref subStatsCount, ref setName));
                Thread thr6 = new Thread(() => equippedCharacter = ScanArtifactEquippedCharacter(screenLocation_X, screenLocation_Y, width, height));

                thr1.Start();
                thr3.Start();
                thr4.Start();
                thr5.Start();
                thr6.Start();

                thr1.Join();
                thr2.Start();

                thr3.Join();
                thr4.Join();
                thr5.Join();
                thr6.Join();
                thr2.Join();

                //// Test individual Functions
                //gearSlot = ScanArtifactGearSlot(screenLocation_X, screenLocation_Y, width, height, imageBox, textBox);
                //mainStat = ScanArtifactMainStat(screenLocation_X, screenLocation_Y, width, height, gearSlot, imageBox, textBox);
                //mainStatValue = ScanArtifactMainStatValue(screenLocation_X, screenLocation_Y, width, height, imageBox, textBox);
                //level = ScanArtifactLevel(screenLocation_X, screenLocation_Y, width, height, imageBox, textBox);
                //subStats = ScanArtifactSubStats(screenLocation_X, screenLocation_Y, width, height, imageBox, textBox, ref subStatsCount, ref setName);
                //equippedCharacter = ScanArtifactEquippedCharacter(screenLocation_X, screenLocation_Y, width, height, imageBox, textBox);


            }
            // Don't fully scan 3 star artifacts and lower
            else if (thirdStar == rarityColor)
            {
                rarity = 3;
            }
            else if (twoStar == rarityColor)
            {
                rarity = 2;
            }
            else if (firstStar == rarityColor)
            {
                rarity = 1;
            }
            else
            { // Not found
                rarity = 0;
            }


            Artifact a =  new Artifact(rarity, gearSlot, mainStat, mainStatValue, level, subStats, subStatsCount, setName, equippedCharacter, id);
            //Artifact a = new Artifact("",0,"",0,0,null,0,"",null);

            return a;
        }

        private static int ScanArtifactGearSlot(int artifactLocation_X, int artifactLocation_Y, int max_X, int max_Y , PictureBox pictureBox, TextBox textBox)
        {
            //Init
            string gearSlot = null;
            int offset = 46;
            Bitmap bm = new Bitmap(max_X/2, 20);
            Graphics g = Graphics.FromImage(bm);

            // Setup Img
            g = Graphics.FromImage(bm);
            g.CopyFromScreen(artifactLocation_X + max_X / 28, artifactLocation_Y + offset, 0, 0, bm.Size);
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(60.0, ref bm);
            Scraper.SetInvert(ref bm);
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);

            // Analyze
            gearSlot = Scraper.AnalyzeText(bm);
            gearSlot = gearSlot.Replace("\n", String.Empty);

            // Display Image && Text
            pictureBox.Image = bm;
            pictureBox.Refresh();
            textBox.Text = gearSlot;
            textBox.Refresh();

            return Scraper.GetGearSlotCode(gearSlot);
        }

        private static int ScanArtifactMainStat(int artifactLocation_X, int artifactLocation_Y, int max_X, int max_Y, int gearSlot, PictureBox pictureBox, TextBox textBox)
        {
            // Get Main Stat
            string mainStat = null;
            int yOffset = 100;
            Bitmap bm = new Bitmap(max_X / 2 + max_X / 28, 20);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(artifactLocation_X, artifactLocation_Y + yOffset, 0, 0, bm.Size);
            //Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(50.0,ref bm);
            //Scraper.SetGrayscale(ref bm);
            Scraper.SetInvert(ref bm);
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);

            mainStat = Scraper.AnalyzeText(bm);
            mainStat = mainStat.Replace("\n", String.Empty);
            mainStat.Trim();

            // Display Image && Text
            pictureBox.Image = bm;
            pictureBox.Refresh();
            textBox.Text = mainStat;
            textBox.Refresh();

            // Check if Defense
            if (mainStat == "DEF")
                mainStat = "DEF%";

            return Scraper.GetMainStatCode(mainStat);
        }

        private static decimal ScanArtifactMainStatValue(int artifactLocation_X, int artifactLocation_Y, int max_X, int max_Y, PictureBox pictureBox, TextBox textBox)
        {
            // Get Main Stat Value
            string mainStatValueText = "";
            int yOffset = 119;
            Bitmap bm = new Bitmap(max_X / 3 + (max_X / 16), 34);
            //Bitmap bm = new Bitmap(max_X / 4, 35);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(artifactLocation_X, artifactLocation_Y + yOffset, 0, 0, bm.Size);
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(90.0, ref bm);
            Scraper.SetInvert(ref bm);
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);
            mainStatValueText = Scraper.AnalyzeText(bm);
            mainStatValueText = mainStatValueText.Replace("\n", String.Empty);

            // Display Image && Text
            pictureBox.Image = bm;
            pictureBox.Refresh();
            textBox.Text = mainStatValueText;
            textBox.Refresh();

            // Check if percentage based or flat stat
            if (mainStatValueText.Contains("%"))
            {
                mainStatValueText = mainStatValueText.Split('%')[0];
            }

            Decimal value;
            if(decimal.TryParse(mainStatValueText, out value))
            {
                return value;
            }
            else
            {
                // Second Try
                mainStatValueText = Scraper.AnalyzeFewText(bm);

                if (mainStatValueText.Contains("%"))
                {
                    mainStatValueText = mainStatValueText.Split('%')[0];
                }

                Decimal value2;
                if (decimal.TryParse(mainStatValueText, out value2))
                {
                    return value2;
                }

                Debug.Print("MainStatValue: " + value2.ToString() + " is NOT VALID");
                System.Environment.Exit(1);
                return -1.0m;
                
            }
        }

        private static int ScanArtifactLevel(int artifactLocation_X, int artifactLocation_Y, int max_X, int max_Y, PictureBox pictureBox, TextBox textBox)
        {
            // Get Level
            Bitmap bm = new Bitmap(50, 33);
            //Bitmap bm = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(artifactLocation_X + 11, artifactLocation_Y + 198, 0, 0, bm.Size);
            g.DrawRectangle(new Pen(bm.GetPixel(7, 10), 16), new Rectangle(0, 0, bm.Width, bm.Height));
            g.DrawRectangle(new Pen(bm.GetPixel(7, 10), 12), new Rectangle(0, 0, bm.Width, bm.Height));
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(60.0, ref bm);
            Scraper.SetInvert(ref bm);
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);

            string text = Scraper.AnalyzeText(bm);
            text.Trim();
            text = text.Replace("\n", String.Empty);

            // Display Image && Text
            pictureBox.Image = bm;
            pictureBox.Refresh();
            textBox.Text = text;
            textBox.Refresh();

            int level;
            if (text != "" && int.TryParse(text, out level))
            {
                // Check if level is valid
                if(level <= 20 && level >= 0)
                {
                    return level;
                }
                else
                {
                    Debug.Print("Error: Found " + level + " for level for artifact");
                    System.Environment.Exit(1);
                    return -1;
                }
                
            }
            else
            {
                Debug.Print("Error: Found '" + text + "' for level for artifact");
                System.Environment.Exit(1);
                return -1;
            }
        }

        private static Artifact.SubStats[] ScanArtifactSubStats(int artifactLocation_X, int artifactLocation_Y, int max_X, int max_Y, PictureBox pictureBox, TextBox textBox, ref int subStatsCount, ref int setName)
        {
            // Get SubStats
            Artifact.SubStats[] subStats = new Artifact.SubStats[4];
            int offset = 26;
            int yOffset = 235;
            int subStatSpacing = 27;
            string text = "";

            for (int i = 0; i < 5; i++)
            {
                Bitmap bm = new Bitmap(max_X, 29);
                Graphics g = Graphics.FromImage(bm);
                g.CopyFromScreen(artifactLocation_X, artifactLocation_Y + yOffset + (i * subStatSpacing), 0, 0, bm.Size);
                g.DrawRectangle(new Pen(bm.GetPixel( max_X - 1, 10), 18), new Rectangle(0, 0, 18, bm.Height));
                //g.DrawRectangle(new Pen(Brushes.Red, 17), new Rectangle(0, 0, 17, bm.Height));
                //Scraper.SetGrayscale(ref bm);
                //Scraper.SetContrast(60.0, ref bm);
                bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);

                text = Scraper.AnalyzeText(bm).Trim();
                text = text.Replace("\n", String.Empty);

                // Display Image && Text
                pictureBox.Image = bm;
                pictureBox.Refresh();
                textBox.Text = text;
                textBox.Refresh();

                // Check if Scanned Set Name
                if (text.Contains(":") || i >= 4)
                {
                    bm = new Bitmap(max_X - offset, 29);
                    g = Graphics.FromImage(bm);
                    g.CopyFromScreen(artifactLocation_X, artifactLocation_Y + yOffset + (i * subStatSpacing), 0, 0, bm.Size);
                    //Scraper.SetGrayscale(ref bm);
                    //Scraper.SetContrast(80.0, ref bm);
                    bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);
                    text = Scraper.AnalyzeText(bm).Trim();

                    // Display Image && Text
                    pictureBox.Image = bm;
                    pictureBox.Refresh();
                    textBox.Text = text;
                    textBox.Refresh();

                    if (text.Contains(':'))
                    {
                        text = text.Split(':')[0];
                        text = Regex.Replace(text, @"(?![A-Za-z\s]).", "");
                        setName = Scraper.GetSetNameCode(text);
                    }
                    else
                    {
                        Debug.Print("Error: " + text + " is not a valid SET NAME");
                        System.Environment.Exit(1);
                        setName = -1;
                    }
                    break;
                }
                else // Get SubStat
                {
                    if (text.Contains("+"))
                    {
                        string[] subStat = text.Split('+');
                        subStat[1].Replace("\n", String.Empty);

                        // Percentage Based
                        if (subStat[1].Contains('%'))
                        {
                            string optional = "";

                            if(subStat[0] == "ATK" || subStat[0] == "DEF" || subStat[0] == "HP")
                            {
                                optional = "%";
                            }

                            subStats[i].stat = Scraper.GetSubStatCode(subStat[0] + optional);
                            subStats[i].value = Convert.ToDecimal(subStat[1].Split('%')[0]);
                        }
                        else // flat stats
                        {
                            subStats[i].stat = Scraper.GetSubStatCode(subStat[0]);
                            subStats[i].value = Convert.ToDecimal(subStat[1].Replace("\n", String.Empty));
                        }
                    }
                    else
                    {
                        subStats[i].value = Convert.ToDecimal(-1);
                    }
                    subStatsCount++;
                }
            }

            return subStats;
        }

        private static int ScanArtifactEquippedCharacter(int artifactLocation_X, int artifactLocation_Y, int max_X, int max_Y, PictureBox pictureBox, TextBox textBox)
        {
            int xOffset = 30;
            int yOffset = 532;
            Bitmap bm = new Bitmap(max_X - xOffset, 20);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(artifactLocation_X + xOffset, artifactLocation_Y + yOffset, 0, 0, bm.Size);
            g.DrawRectangle(new Pen(bm.GetPixel(max_X - xOffset - 1, 10), 14), new Rectangle(0, 0, 13, bm.Height));
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(60.0, ref bm);
            //g.DrawRectangle(new Pen(Brushes.Red, 14), new Rectangle(0, 0, 13, bm.Height));
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);

            string equippedCharacter = Scraper.AnalyzeText(bm);
            equippedCharacter.Trim();
            equippedCharacter = equippedCharacter.Replace("\n", String.Empty);

            // Display Image && Text
            pictureBox.Image = bm;
            pictureBox.Refresh();
            textBox.Text = equippedCharacter;
            textBox.Refresh();

            if (equippedCharacter != "")
            {
                var regexItem = new Regex("Equipped:");
                if (regexItem.IsMatch(equippedCharacter))
                {
                    string[] tempString = equippedCharacter.Split(':');
                    equippedCharacter = tempString[1].Replace("\n", String.Empty);
                    equippedCharacter = equippedCharacter.Trim();

                    return Scraper.GetCharacterCode(equippedCharacter);
                }
            }
            // artifact has no equipped character
            return 0;
        }

        #region Threaded Scan Functions
        private static int ScanArtifactGearSlot(int artifactLocation_X, int artifactLocation_Y, int max_X, int max_Y)
        {
            //Init
            string gearSlot = null;
            int offset = 46;
            Bitmap bm = new Bitmap(max_X / 2, 20);
            Graphics g = Graphics.FromImage(bm);

            // Setup Img
            g = Graphics.FromImage(bm);
            g.CopyFromScreen(artifactLocation_X + max_X / 28, artifactLocation_Y + offset, 0, 0, bm.Size);
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(60.0, ref bm);
            Scraper.SetInvert(ref bm);
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);

            // Analyze
            gearSlot = Scraper.AnalyzeText(bm);
            gearSlot = gearSlot.Replace("\n", String.Empty);

            // View Picture
            UserInterface.Reset();
            UserInterface.SetImage(bm);
            UserInterface.AddText(gearSlot);

            return Scraper.GetGearSlotCode(gearSlot);
        }

        private static int ScanArtifactMainStat(int artifactLocation_X, int artifactLocation_Y, int max_X, int max_Y, int gearSlot)
        {
            // Get Main Stat
            string mainStat = null;
            int yOffset = 100;
            Bitmap bm = new Bitmap(max_X / 2 + max_X / 28, 20);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(artifactLocation_X, artifactLocation_Y + yOffset, 0, 0, bm.Size);
            //Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(50.0, ref bm);
            //Scraper.SetGrayscale(ref bm);
            Scraper.SetInvert(ref bm);
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);

            mainStat = Scraper.AnalyzeText(bm);
            mainStat = mainStat.Replace("\n", String.Empty);
            mainStat.Trim();

            // Check if Defense
            if (mainStat == "DEF")
                mainStat = "DEF%";

            // View Picture
            UserInterface.Reset();
            UserInterface.SetImage(bm);
            UserInterface.AddText(mainStat);

            return Scraper.GetMainStatCode(mainStat);
        }

        private static decimal ScanArtifactMainStatValue(int artifactLocation_X, int artifactLocation_Y, int max_X, int max_Y)
        {
            // Get Main Stat Value
            string mainStatValueText = "";
            int yOffset = 119;
            Bitmap bm = new Bitmap(max_X / 3 + (max_X / 16), 34);
            //Bitmap bm = new Bitmap(max_X / 4, 35);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(artifactLocation_X, artifactLocation_Y + yOffset, 0, 0, bm.Size);
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(90.0, ref bm);
            Scraper.SetInvert(ref bm);
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);
            mainStatValueText = Scraper.AnalyzeText(bm);
            mainStatValueText = mainStatValueText.Replace("\n", String.Empty);

            // Check if percentage based or flat stat
            if (mainStatValueText.Contains("%"))
            {
                mainStatValueText = mainStatValueText.Split('%')[0];
            }

            Decimal value;
            if (decimal.TryParse(mainStatValueText, out value))
            {
                // View Picture
                UserInterface.Reset();
                UserInterface.SetImage(bm);
                UserInterface.AddText(mainStatValueText);

                return value;
            }
            else
            {
                // Second Try
                mainStatValueText = Scraper.AnalyzeFewText(bm);

                if (mainStatValueText.Contains("%"))
                {
                    mainStatValueText = mainStatValueText.Split('%')[0];
                }

                // View Picture
                UserInterface.Reset();
                UserInterface.SetImage(bm);
                UserInterface.AddText(mainStatValueText);

                Decimal value2;
                if (decimal.TryParse(mainStatValueText, out value2))
                {
                    return value2;
                }

                Debug.Print("MainStatValue: " + value2.ToString() + " is NOT VALID");
                System.Environment.Exit(1);
                return -1.0m;

            }
        }

        private static int ScanArtifactLevel(int artifactLocation_X, int artifactLocation_Y, int max_X, int max_Y)
        {
            // Get Level
            Bitmap bm = new Bitmap(50, 33);
            //Bitmap bm = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(artifactLocation_X + 11, artifactLocation_Y + 198, 0, 0, bm.Size);
            g.DrawRectangle(new Pen(bm.GetPixel(7, 10), 16), new Rectangle(0, 0, bm.Width, bm.Height));
            g.DrawRectangle(new Pen(bm.GetPixel(7, 10), 12), new Rectangle(0, 0, bm.Width, bm.Height));
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(60.0, ref bm);
            Scraper.SetInvert(ref bm);
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);

            string text = Scraper.AnalyzeText(bm);
            text.Trim();
            text = text.Replace("\n", String.Empty);

            // View Picture
            UserInterface.Reset();
            UserInterface.SetImage(bm);
            UserInterface.AddText(text);

            int level;
            if (text != "" && int.TryParse(text, out level))
            {
                // Check if level is valid
                if (level <= 20 && level >= 0)
                {
                    return level;
                }
                else
                {
                    Debug.Print("Error: Found " + level + " for level for artifact");
                    System.Environment.Exit(1);
                    return -1;
                }

            }
            else
            {
                Debug.Print("Error: Found '" + text + "' for level for artifact");
                System.Environment.Exit(1);
                return -1;
            }
        }

        private static Artifact.SubStats[] ScanArtifactSubStats(int artifactLocation_X, int artifactLocation_Y, int max_X, int max_Y, ref int subStatsCount, ref int setName)
        {
            // Get SubStats
            Artifact.SubStats[] subStats = new Artifact.SubStats[4];
            int offset = 26;
            int yOffset = 235;
            int subStatSpacing = 27;
            string text = "";

            for (int i = 0; i < 5; i++)
            {
                Bitmap bm = new Bitmap(max_X, 29);
                Graphics g = Graphics.FromImage(bm);
                g.CopyFromScreen(artifactLocation_X, artifactLocation_Y + yOffset + (i * subStatSpacing), 0, 0, bm.Size);
                g.DrawRectangle(new Pen(bm.GetPixel(max_X - 1, 10), 18), new Rectangle(0, 0, 18, bm.Height));
                //g.DrawRectangle(new Pen(Brushes.Red, 17), new Rectangle(0, 0, 17, bm.Height));
                //Scraper.SetGrayscale(ref bm);
                //Scraper.SetContrast(60.0, ref bm);
                bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);

                text = Scraper.AnalyzeText(bm).Trim();
                text = text.Replace("\n", String.Empty);

                // View Picture
                UserInterface.Reset();
                UserInterface.SetImage(bm);
                UserInterface.AddText(text);

                // Check if Scanned Set Name
                if (text.Contains(":") || i >= 4)
                {
                    bm = new Bitmap(max_X - offset, 29);
                    g = Graphics.FromImage(bm);
                    g.CopyFromScreen(artifactLocation_X, artifactLocation_Y + yOffset + (i * subStatSpacing), 0, 0, bm.Size);
                    //Scraper.SetGrayscale(ref bm);
                    //Scraper.SetContrast(80.0, ref bm);
                    bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);
                    text = Scraper.AnalyzeText(bm).Trim();

                    if (text.Contains(':'))
                    {
                        text = text.Split(':')[0];
                        text = Regex.Replace(text, @"(?![A-Za-z\s]).", "");
                        setName = Scraper.GetSetNameCode(text);
                    }
                    else
                    {
                        Debug.Print("Error: " + text + " is not a valid SET NAME");
                        System.Environment.Exit(1);
                        setName = -1;
                    }
                    break;
                }
                else // Get SubStat
                {
                    if (text.Contains("+"))
                    {
                        string[] subStat = text.Split('+');
                        subStat[1].Replace("\n", String.Empty);

                        // Percentage Based
                        if (subStat[1].Contains('%'))
                        {
                            string optional = "";

                            if (subStat[0] == "ATK" || subStat[0] == "DEF" || subStat[0] == "HP")
                            {
                                optional = "%";
                            }

                            // View Picture
                            UserInterface.Reset();
                            UserInterface.SetImage(bm);
                            UserInterface.AddText(subStat[0] + " " + subStat[1]);

                            subStats[i].stat = Scraper.GetSubStatCode(subStat[0] + optional);
                            subStats[i].value = Convert.ToDecimal(subStat[1].Split('%')[0]);
                        }
                        else // flat stats
                        {
                            // View Picture
                            UserInterface.Reset();
                            UserInterface.SetImage(bm);
                            UserInterface.AddText(subStat[0] + " " + subStat[1]);

                            subStats[i].stat = Scraper.GetSubStatCode(subStat[0]);
                            subStats[i].value = Convert.ToDecimal(subStat[1].Replace("\n", String.Empty));
                        }
                    }
                    else
                    {
                        subStats[i].value = Convert.ToDecimal(-1);
                    }
                    subStatsCount++;
                }
            }

            return subStats;
        }

        private static int ScanArtifactEquippedCharacter(int artifactLocation_X, int artifactLocation_Y, int max_X, int max_Y)
        {
            int xOffset = 30;
            int yOffset = 532;
            Bitmap bm = new Bitmap(max_X - xOffset, 20);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(artifactLocation_X + xOffset, artifactLocation_Y + yOffset, 0, 0, bm.Size);
            g.DrawRectangle(new Pen(bm.GetPixel(max_X - xOffset - 1, 10), 14), new Rectangle(0, 0, 13, bm.Height));
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(60.0, ref bm);
            //g.DrawRectangle(new Pen(Brushes.Red, 14), new Rectangle(0, 0, 13, bm.Height));
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);

            string equippedCharacter = Scraper.AnalyzeText(bm);
            equippedCharacter.Trim();
            equippedCharacter = equippedCharacter.Replace("\n", String.Empty);

            // View Picture
            UserInterface.Reset();
            UserInterface.SetImage(bm);
            UserInterface.AddText(equippedCharacter);

            if (equippedCharacter != "")
            {
                var regexItem = new Regex("Equipped:");
                if (regexItem.IsMatch(equippedCharacter))
                {
                    string[] tempString = equippedCharacter.Split(':');
                    equippedCharacter = tempString[1].Replace("\n", String.Empty);
                    equippedCharacter = equippedCharacter.Trim();

                    return Scraper.GetCharacterCode(equippedCharacter);
                }
            }
            // artifact has no equipped character
            return 0;
        }
        #endregion

        
    }


}
