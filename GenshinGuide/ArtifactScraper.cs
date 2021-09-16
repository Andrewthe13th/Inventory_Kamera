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
            Debug.Print("Artifact Count: " + artifactCount);
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
            int totalRows = (int)Math.Ceiling((decimal)((decimal)(artifactCount) / (decimal)(maxColumns)));
            int currentColumn = 0;
            int currentRow = 0;

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

                if (currentArtifactCount % maxColumns == 0)
                {
                    currentRow++;
                    if (totalRows - currentRow <= maxRows - 1)
                    {
                        break;
                    }
                }
                //if ( (artifactCount - currentArtifactCount <= (maxRows * maxColumns)) && (currentColumn == 0))
                //{
                //    break;
                //}

                //if(currentArtifactCount > 23)
                //{
                //    Debug.Print("Nice!");
                //}

                // Select Artifact
                Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(artifactLocation_X) + (xOffset * (currentArtifactCount % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(artifactLocation_Y));
                Navigation.sim.Mouse.LeftButtonClick();
                Navigation.SystemRandomWait(Navigation.Speed.Faster);

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
                //GC.Collect();

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
                    //Navigation.SystemRandomWait(Navigation.Speed.Fast);
                }
            };

            // scroll down as much as possible
            for (int i = 0; i < 20; i++)
            {
                Navigation.sim.Mouse.VerticalScroll(-1);
            }
            Navigation.SystemRandomWait(Navigation.Speed.Normal);

            // Get artifacts on bottom of page
            int rowsLeft = (int)Math.Ceiling( (double)(artifactCount - currentArtifactCount) / (double)maxColumns);
            bool b_EnchancementOre = false;
            int startPostion = 1;
            for (int i = startPostion; i < (rowsLeft + startPostion); i++)
            {
                for (int k = 0; k < maxColumns; k++)
                {
                    if(artifactCount - currentArtifactCount <= 0)
                    {
                        break;
                    }
                    if (!b_EnchancementOre)
                    {
                        Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(artifactLocation_X) + (xOffset * (k % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(artifactLocation_Y) + (yOffset * (i % (maxRows + 1))));
                        Navigation.sim.Mouse.LeftButtonClick();
                        Navigation.SystemRandomWait(Navigation.Speed.Faster);
                    }

                    // check if enchnacement Ore
                    if (artifactCount - currentArtifactCount == 7)
                    {
                        b_EnchancementOre = WeaponScraper.CheckForEnchancementOre();
                    }

                    if (b_EnchancementOre)
                    {
                        // Scan top row instead
                        Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(artifactLocation_X) + (xOffset * (k % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(artifactLocation_Y) + (yOffset * (0 % (maxRows + 1))));
                        Navigation.sim.Mouse.LeftButtonClick();
                        Navigation.SystemRandomWait(Navigation.Speed.Faster);
                    }

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
            text = text.Trim();

            int count = 0;
            // Check for dash
            if (Regex.IsMatch(text, "/")){
                count = Int32.Parse(text.Split()[1].Split('/')[0]);
            }
            else
            {
                // divide by the number on the right if both numbers fused
                count = Int32.Parse(text.Split()[1]) / 1000;
            }

            // Check if larger than 1000
            while(count > 1000)
            {
                count = count / 10;
            }


            return count;
        }

        private static Artifact ScanArtifact(int id)
        {
            // Init Variables
            int gearSlot = 0;
            int mainStat = 0;
            //decimal mainStatValue = 0;
            int level = 0;
            Artifact.SubStats[] subStats = new Artifact.SubStats[4];
            int subStatsCount = 0;
            int setName = 0;
            int equippedCharacter = 0;
            bool _lock = false;


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
#if DEBUG
            if (Scraper.s_bDoDebugOnlyCode)
            {
                UserInterface.Reset();
                UserInterface.SetImage(bm);
            }
#endif

            // Get Rarity (Check color)
            int rarity = 0;
            Color rarityColor = bm.GetPixel(12, 10);
            //textBox.Text = rarityColor.A + " " + rarityColor.R + " " + rarityColor.G + " " + rarityColor.B;
            Color fiveStar = Color.FromArgb(255, 188, 105, 50);
            Color fourthStar = Color.FromArgb(255, 161, 86, 224);
            Color thirdStar = Color.FromArgb(255, 81, 127, 203);
            Color twoStar = Color.FromArgb(255, 42, 143, 114);
            Color firstStar = Color.FromArgb(255, 114, 119, 138);

            // Check for equipped color
            Color equipped = Color.FromArgb(255, 255, 231, 187);
            Color equippedColor = bm.GetPixel(5, height - 10);

            // Check for lock color
            Color lockColor = Color.FromArgb(255, 255, 138, 117);
            Color lockStatus = bm.GetPixel(width - 35, 220);

            //Bitmap lock_bm = new Bitmap(15, 15);
            //Graphics g_1 = Graphics.FromImage(lock_bm);
            //int screenLocation_X_1 = Navigation.GetPosition().left + Convert.ToInt32(artifactLocation_X);
            //int screenLocation_Y_1 = Navigation.GetPosition().top + Convert.ToInt32(artifactLocation_Y);
            //g_1.CopyFromScreen(screenLocation_X_1 + width - 35, screenLocation_Y_1 + 220, 0, 0, lock_bm.Size);

            //// Display Image
            //UserInterface.Reset();
            //UserInterface.SetImage(lock_bm);

            if ( Scraper.CompareColors(fiveStar,rarityColor) || Scraper.CompareColors(fourthStar, rarityColor))
            //if (true)
            {
                rarity = ( Scraper.CompareColors(fiveStar, rarityColor) ) ? 5 : 4;
                bool b_equipped = Scraper.CompareColors(equipped, equippedColor);
                _lock = Scraper.CompareColors(lockColor, lockStatus);
 
                Bitmap bm_1 = bm.Clone(new Rectangle(0, 0, width, height), bm.PixelFormat);
                Bitmap bm_2 = bm.Clone(new Rectangle(0, 0, width, height), bm.PixelFormat);
                Bitmap bm_3 = bm.Clone(new Rectangle(0, 0, width, height), bm.PixelFormat);
                Bitmap bm_4 = bm.Clone(new Rectangle(0, 0, width, height), bm.PixelFormat);

                // Improved Scanning using multi threading
                Thread thr1 = new Thread(() => gearSlot = ScanArtifactGearSlot(bm_1, width, height));
                Thread thr2 = new Thread(() => mainStat = ScanArtifactMainStat(bm_1, width, height, gearSlot));
                Thread thr3 = new Thread(() => level = ScanArtifactLevel(bm_2, width, height));
                Thread thr4 = new Thread(() => subStats = ScanArtifactSubStats(bm_3, rarity, level, ref subStatsCount, ref setName));
                Thread thr5 = new Thread(() => equippedCharacter = ScanArtifactEquippedCharacter(bm_4, width, height));

                thr1.Start();
                thr3.Start();
                if (b_equipped)
                {
                    thr5.Start();
                }

                //  sub stats will check level and rarity
                thr3.Join();
                thr4.Start();

                // main stat will check gearslot
                thr1.Join();
                thr2.Start();

                // 2-5 join back
                thr4.Join();
                if (b_equipped)
                {
                    thr5.Join();
                }
                thr2.Join();

                //// Test individual Functions
                //gearSlot = ScanArtifactGearSlot(bm, width, height);
                //mainStat = ScanArtifactMainStat(bm, width, height, gearSlot);
                //level = ScanArtifactLevel(bm, width, height);
                //subStats = ScanArtifactSubStats(bm, width, height, ref subStatsCount, ref setName);

                //if(Scraper.CompareColors(equipped, equippedColor))
                //{
                //    equippedCharacter = ScanArtifactEquippedCharacter(bm, width, height);
                //}

            }
            // Don't fully scan 3 star artifacts and lower
            else if (Scraper.CompareColors(thirdStar, rarityColor))
            {
                rarity = 3;
                Navigation.SystemRandomWait(Navigation.Speed.ArtifactIgnore);
            }
            else if (Scraper.CompareColors(twoStar, rarityColor))
            {
                rarity = 2;
                Navigation.SystemRandomWait(Navigation.Speed.ArtifactIgnore);
            }
            else if (Scraper.CompareColors(firstStar, rarityColor))
            {
                rarity = 1;
                Navigation.SystemRandomWait(Navigation.Speed.ArtifactIgnore);
            }
            else
            { // Not found
                rarity = 0;
            }


            Artifact a =  new Artifact(rarity, gearSlot, mainStat, level, subStats, subStatsCount, setName, equippedCharacter, id, _lock);
            //Artifact a = new Artifact("",0,"",0,0,null,0,"",null);

            return a;
        }

        #region Threaded Scan Functions
        private static int ScanArtifactGearSlot(Bitmap artifactImage, int max_X, int max_Y)
        {
            //Init
            string gearSlot = null;
            int offset = 46;
            Bitmap bm = artifactImage.Clone(new Rectangle(3, offset, max_X / 2, 20), artifactImage.PixelFormat);

            // Process Img
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(80.0, ref bm);
            Scraper.SetInvert(ref bm);


            // Analyze
            gearSlot = Scraper.AnalyzeText_1(bm);
            gearSlot = gearSlot.Replace("\n", String.Empty);

            // View Picture
            //UserInterface.Reset();
            //UserInterface.SetImage(bm);
            //UserInterface.AddText(gearSlot);

            return Scraper.GetGearSlotCode(gearSlot);
        }

        private static int ScanArtifactMainStat(Bitmap artifactImage, int max_X, int max_Y, int gearSlot)
        {
            // Get Main Stat
            string mainStat = null;
            int yOffset = 100;
            if(gearSlot == 0) // Flower HP
            {
                return Scraper.GetMainStatCode("HP_Flat");
            }
            else if (gearSlot == 1) // Plume ATK
            {
                return Scraper.GetMainStatCode("ATK_Flat");
            }
            else // scan time, cup, hat artifacts only
            {
                Bitmap bm = artifactImage.Clone(new Rectangle(0, yOffset, max_X / 2 + max_X / 28, 20),artifactImage.PixelFormat);
                Scraper.SetContrast(50.0, ref bm);
                Scraper.SetInvert(ref bm);

                mainStat = Scraper.AnalyzeText_1(bm);
                mainStat = mainStat.Replace("\n", String.Empty);
                mainStat.Trim();
                mainStat = Regex.Replace(mainStat, @"(?![A-Za-z\s]).", "");

                // View Picture
                //UserInterface.Reset();
                //UserInterface.SetImage(bm);
                //UserInterface.AddText(mainStat);

                return Scraper.GetMainStatCode(mainStat);
            }
        }

        private static decimal ScanArtifactMainStatValue(Bitmap artifactImage, int max_X, int max_Y)
        {
            // Get Main Stat Value
            string mainStatValueText = "";
            //int yOffset = 119;
            Bitmap bm = new Bitmap(max_X / 3 + (max_X / 16), 34);
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
#if DEBUG
                if (Scraper.s_bDoDebugOnlyCode)
                {
                    UserInterface.Reset();
                    UserInterface.SetImage(bm);
                    UserInterface.AddText(mainStatValueText);
                }
#endif

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
#if DEBUG
                if (Scraper.s_bDoDebugOnlyCode)
                {
                    UserInterface.Reset();
                    UserInterface.SetImage(bm);
                    UserInterface.AddText(mainStatValueText);
                }
#endif

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

        private static int ScanArtifactLevel(Bitmap artifactImage, int max_X, int max_Y)
        {
            int width = 50; int height = 33;
            // Get Level
            Bitmap bm = artifactImage.Clone(new Rectangle(11, 198, width, height), artifactImage.PixelFormat);
            //Bitmap bm = artifactImage.Clone(new Rectangle(18, 206, 36, 18), artifactImage.PixelFormat);
            Graphics g = Graphics.FromImage(bm);
            // Add more padding to be able to read level better
            g.DrawRectangle(new Pen(bm.GetPixel(7, 10), 16), new Rectangle(0, 0, width, height));
            g.DrawRectangle(new Pen(bm.GetPixel(7, 10), 12), new Rectangle(0, 0, width, height));
            // Process Img
            Scraper.SetGrayscale(ref bm);
            //Scraper.SetContrast(60.0, ref bm);
            Scraper.SetInvert(ref bm);

            string text = Scraper.AnalyzeText_2(bm);
            text.Trim();
            text = text.Replace("\n", String.Empty);

            // View Picture
            //UserInterface.Reset();
            //UserInterface.SetImage(bm);
            //UserInterface.AddText(text);

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

        private static Artifact.SubStats[] ScanArtifactSubStats(Bitmap artifactImage, int rarity, int level ,ref int subStatsCount, ref int setName)
        {
            // Get SubStats
            Artifact.SubStats[] subStats = new Artifact.SubStats[4];
            int offset = 26;
            int yOffset = 235;
            int xOffset = 30;
            int subStatSpacing = 27;
            string text = "";
            int width = artifactImage.Width;
            System.Drawing.Imaging.PixelFormat pixelFormat = artifactImage.PixelFormat;

            // Get Substats Count
            int count = 2;
            if(rarity == 5)
            {
                if (level >= 4)
                {
                    count = 4;
                }
                else
                    count = 3;
            }
            if(rarity == 4)
            {
                if (level >= 8)
                {
                    count = 4;
                }
                else if (level >= 4)
                {
                    count = 3;
                }
                else
                    count = 2;
            }


            if (count == 4)
            {
                //Do first two substats using threads
                Bitmap bm_1 = artifactImage.Clone(new Rectangle(xOffset, yOffset + (0 * subStatSpacing), width - xOffset, 29), pixelFormat);
                Bitmap bm_2 = artifactImage.Clone(new Rectangle(xOffset, yOffset + (1 * subStatSpacing), width - xOffset, 29), pixelFormat);
                Bitmap bm_3 = artifactImage.Clone(new Rectangle(xOffset, yOffset + (2 * subStatSpacing), width - xOffset, 29), pixelFormat);
                Bitmap bm_4 = artifactImage.Clone(new Rectangle(xOffset, yOffset + (3 * subStatSpacing), width - xOffset, 29), pixelFormat);

                Thread thr1 = new Thread(() => subStats[0] = ScanArtifactSubStat(bm_1, 1));
                Thread thr2 = new Thread(() => subStats[1] = ScanArtifactSubStat(bm_2, 2));
                Thread thr3 = new Thread(() => subStats[2] = ScanArtifactSubStat(bm_3, 3));
                Thread thr4 = new Thread(() => subStats[3] = ScanArtifactSubStat(bm_4, 4));

                thr1.Start();
                thr2.Start();
                thr3.Start();
                thr4.Start();
                thr1.Join();
                thr2.Join();
                thr3.Join();
                thr4.Join();

                Bitmap bm = artifactImage.Clone(new Rectangle(0, yOffset + (4 * subStatSpacing), width - offset, 29), pixelFormat);
                Scraper.SetGrayscale(ref bm);
                text = Scraper.AnalyzeText(bm).Trim();

                // View Picture
                //UserInterface.Reset();
                //UserInterface.SetImage(bm);
                //UserInterface.AddText(text);

                if (text.Contains(':'))
                {
                    text = text.Split(':')[0];
                    text = Regex.Replace(text, @"(?![A-Za-z\s]).", "");
                    text = text.Trim();
                    setName = Scraper.GetSetNameCode(text);
                    return subStats;
                }
                else
                {
                    Debug.Print("Error: " + text + " is not a valid SET NAME");
                    System.Environment.Exit(1);
                    setName = -1;
                }
            }
            else if(count == 3)
            {
                //Do first two substats using threads
                Bitmap bm_1 = artifactImage.Clone(new Rectangle(xOffset, yOffset + (0 * subStatSpacing), width - xOffset, 29), pixelFormat);
                Bitmap bm_2 = artifactImage.Clone(new Rectangle(xOffset, yOffset + (1 * subStatSpacing), width - xOffset, 29), pixelFormat);
                Bitmap bm_3 = artifactImage.Clone(new Rectangle(xOffset, yOffset + (2 * subStatSpacing), width - xOffset, 29), pixelFormat);

                Thread thr1 = new Thread(() => subStats[0] = ScanArtifactSubStat(bm_1, 1));
                Thread thr2 = new Thread(() => subStats[1] = ScanArtifactSubStat(bm_2, 2));
                Thread thr3 = new Thread(() => subStats[2] = ScanArtifactSubStat(bm_3, 3));

                thr1.Start();
                thr2.Start();
                thr3.Start();
                thr1.Join();
                thr2.Join();
                thr3.Join();
            }
            else 
            {
                //Do first two substats using threads
                Bitmap bm_1 = artifactImage.Clone(new Rectangle(xOffset, yOffset + (0 * subStatSpacing), width - xOffset, 29), pixelFormat);
                Bitmap bm_2 = artifactImage.Clone(new Rectangle(xOffset, yOffset + (1 * subStatSpacing), width - xOffset, 29), pixelFormat);

                Thread thr1 = new Thread(() => subStats[0] = ScanArtifactSubStat(bm_1, 1));
                Thread thr2 = new Thread(() => subStats[1] = ScanArtifactSubStat(bm_2, 2));

                thr1.Start();
                thr2.Start();
                thr1.Join();
                thr2.Join();
            }


            for (int i = count; i < 5; i++)
            {
                Bitmap bm = artifactImage.Clone(new Rectangle(xOffset, yOffset + (i * subStatSpacing), width-xOffset, 29), pixelFormat);

                text = Scraper.AnalyzeText_Line(bm);
                text = text.Replace("\n", String.Empty);

                // view picture
                //UserInterface.Reset();
                //UserInterface.SetImage(bm);
                //UserInterface.AddText(text);

                // Check if Scanned Set Name
                if (text.Contains(":") || i >= 4)
                {
                    //bm = new Bitmap(max_X - offset, 29);
                    bm = artifactImage.Clone(new Rectangle(0, yOffset + (i * subStatSpacing), width - offset, 29), pixelFormat);
                    Scraper.SetGrayscale(ref bm);
                    text = Scraper.AnalyzeText_3(bm).Trim();

                    // View Picture
                    //UserInterface.Reset();
                    //UserInterface.SetImage(bm);
                    //UserInterface.AddText(text);

                    if (text.Contains(':'))
                    {
                        text = text.Split(':')[0];
                        text = Regex.Replace(text, @"(?![A-Za-z\s]).", "");
                        text = text.Trim();
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
                        text = Regex.Replace(text, @"(?![A-Za-z0-9+%\s/.]).", "");
                        string[] subStat = text.Split('+');
                        subStat[0] = subStat[0].Trim();

                        // Percentage Based
                        if (subStat[1].Contains('%'))
                        {
                            string optional = "";

                            if (subStat[0] == "ATK" || subStat[0] == "DEF" || subStat[0] == "HP")
                            {
                                optional = "%";
                            }

                            // View Picture
                            //UserInterface.Reset();
                            //UserInterface.SetImage(bm);
                            //UserInterface.AddText(subStat[0] + " " + subStat[1]);

                            subStats[i].stat = Scraper.GetSubStatCode(subStat[0] + optional);

                            // Check if subStat has space (when value is not detected with a deciminal)
                            if (subStat[1].Contains(' ') && !subStat[1].Contains('.'))
                            {
                                subStat[1] = subStat[1].Replace(' ', '.');
                            }

                            subStats[i].value = Convert.ToDecimal(subStat[1].Split('%')[0]);
                        }
                        else // flat stats
                        {
                            // View Picture
                            //UserInterface.Reset();
                            //UserInterface.SetImage(bm);
                            //UserInterface.AddText(subStat[0] + " " + subStat[1]);
 
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

        private static Artifact.SubStats ScanArtifactSubStat(Bitmap bm, int ocr)
        {
            Artifact.SubStats subStat = new Artifact.SubStats();
            string text = "";
            if (ocr == 4)
            {
                text = Scraper.AnalyzeText_Line4(bm);
            }
            else if(ocr == 3)
            {
                text = Scraper.AnalyzeText_Line3(bm);
            }
            else if (ocr == 2)
            {
                text = Scraper.AnalyzeText_Line2(bm);
            }
            else
            {
                text = Scraper.AnalyzeText_Line1(bm);
            }
            


            text = text.Replace("\n", String.Empty);

            // view picture
            //UserInterface.Reset();
            //UserInterface.SetImage(bm);
            //UserInterface.AddText(text);

            if (text.Contains("+"))
            {
                text = Regex.Replace(text, @"(?![A-Za-z0-9+%\s/.]).", "");
                string[] subStatText = text.Split('+');
                subStatText[0] = subStatText[0].Trim();

                // Percentage Based
                if (subStatText[1].Contains('%'))
                {
                    string optional = "";

                    if (subStatText[0] == "ATK" || subStatText[0] == "DEF" || subStatText[0] == "HP")
                    {
                        optional = "%";
                    }

                    // View Picture
                    //UserInterface.Reset();
                    //UserInterface.SetImage(bm);
                    //UserInterface.AddText(subStatText[0] + " " + subStatText[1]);

                    subStat.stat = Scraper.GetSubStatCode(subStatText[0] + optional);

                    // Check if subStat has space (when value is not detected with a deciminal)
                    if (subStatText[1].Contains(' ') && !subStatText[1].Contains('.'))
                    {
                        subStatText[1] = subStatText[1].Replace(' ', '.');
                    }

                    subStat.value = Convert.ToDecimal(subStatText[1].Split('%')[0]);
                }
                else // flat stats
                {
                    // View Picture
                    //UserInterface.Reset();
                    //UserInterface.SetImage(bm);
                    //UserInterface.AddText(subStatText[0] + " " + subStatText[1]);

                    subStat.stat = Scraper.GetSubStatCode(subStatText[0]);
                    subStat.value = Convert.ToDecimal(subStatText[1].Replace("\n", String.Empty));
                }
            }
            else
            {
                subStat.value = Convert.ToDecimal(-1);
            }

            return subStat;
        }

        private static int ScanArtifactEquippedCharacter(Bitmap artifactImage, int max_X, int max_Y)
        {
            int xOffset = 30;
            int yOffset = 532;

            Bitmap bm = artifactImage.Clone(new Rectangle(xOffset, yOffset, max_X - xOffset, 20), artifactImage.PixelFormat);
            Graphics g = Graphics.FromImage(bm);
            // Gets rid of character head on Left
            g.DrawRectangle(new Pen(bm.GetPixel(max_X - xOffset - 1, 10), 14), new Rectangle(0, 0, 13, bm.Height));
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(60.0, ref bm);

            string equippedCharacter = Scraper.AnalyzeText_4(bm);
            equippedCharacter.Trim();
            equippedCharacter = equippedCharacter.Replace("\n", String.Empty);

            // View Picture
            //UserInterface.Reset();
            //UserInterface.SetImage(bm);
            //UserInterface.AddText(equippedCharacter);

            if (equippedCharacter != "")
            {
                var regexItem = new Regex("Equipped:");
                if (regexItem.IsMatch(equippedCharacter))
                {
                    string[] tempString = equippedCharacter.Split(':');
                    equippedCharacter = tempString[1].Replace("\n", String.Empty);
                    equippedCharacter = equippedCharacter.Trim();
                    equippedCharacter = Regex.Replace(equippedCharacter, @"[\/!@#$%^&*()\[\]\-_`~\\+={};:',.<>?‘|]", "");

                    // Used to match with Traveler Name
                    while (equippedCharacter.Length > 1)
                    {
                        int temp = Scraper.GetCharacterCode(equippedCharacter, true);
                        if (temp == -1)
                        {
                            equippedCharacter = equippedCharacter.Substring(0, equippedCharacter.Length - 1);
                        }
                        else
                        {
                            break;
                        }
                    }

                    return Scraper.GetCharacterCode(equippedCharacter);
                }
            }
            // artifact has no equipped character
            return 0;
        }
        #endregion

        
    }


}
