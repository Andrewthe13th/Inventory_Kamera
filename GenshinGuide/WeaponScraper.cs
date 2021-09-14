using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GenshinGuide
{
    public static class WeaponScraper
    {
        public static List<Weapon> ScanWeapons(ref List<Weapon> equippedWeapon)
        {
            List<Weapon> weapons = new List<Weapon>();

            // Get Max weapons from screen
            int weaponCount = ScanWeaponCount();
            //Debug.Print("Max Weapons: " + weaponCount.ToString());
            //int weaponCount = 29;
            int currentweaponCount = 0;
            int scrollCount = 0;

            // Where in screen space weapons are
            Double weaponLocation_X = (Double)Navigation.GetArea().right * ((Double)21 / (Double)160);
            Double weaponLocation_Y = (Double)Navigation.GetArea().bottom * ((Double)14 / (Double)90);
            Bitmap bm = new Bitmap(130, 130);
            Graphics g = Graphics.FromImage(bm);
            int maxColumns = 7;
            int maxRows = 4;
            int totalRows = (int)Math.Ceiling((decimal)( (decimal)(weaponCount) / (decimal)(maxColumns) ));
            int currentColumn = 0;
            int currentRow = 0;

            // offset used to move mouse to other weapons
            int xOffset = Convert.ToInt32((Double)Navigation.GetArea().right * ((Double)12.25 / (Double)160));
            int yOffset = Convert.ToInt32((Double)Navigation.GetArea().bottom * ((Double)14.5 / (Double)90));

            // Testing for single weapons. REMOVE LATER!!!
            //Weapon a = ScanWeapon(0);

            
            // Go through weapon list
            while (currentweaponCount < weaponCount)
            {
                //if ((weaponCount - currentweaponCount <= (maxRows * maxColumns-1)) && (currentColumn == 0))
                //{

                //    break;
                //}
                if ( currentweaponCount % maxColumns == 0)
                {
                    currentRow++;
                    if(totalRows - currentRow <= maxRows-1)
                    {
                        break;
                    }
                }


                // Select weapon
                Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X) + (xOffset * (currentweaponCount % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y));
                Navigation.sim.Mouse.LeftButtonClick();
                Navigation.SystemRandomWait(Navigation.Speed.Faster);

                // Scan weapon
                Weapon w = ScanWeapon(currentweaponCount);
                currentweaponCount++;
                //Debug.Print("Weapon Count: " + currentweaponCount.ToString());
                currentColumn++;

                // Add weapon to equipped list
                if (w.GetEquippedCharacter() != 0)
                {
                    equippedWeapon.Add(w);
                }

                // Add to weapon List Object
                weapons.Add(w);
                //GC.Collect();

                // reach end of row
                if (currentColumn == maxColumns)
                {
                    // reset mouse pointer and scroll down weapon list
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
                                if (scrollCount == 18)
                                {
                                    scrollCount = 0;
                                }
                                else
                                {
                                    Navigation.sim.Mouse.VerticalScroll(-1);
                                }
                            }
                        }
                    }
                    Navigation.SystemRandomWait(Navigation.Speed.Fast);
                }
            };

            // scroll down as much as possible
            for (int i = 0; i < 20; i++)
            {
                Navigation.sim.Mouse.VerticalScroll(-1);
            }
            Navigation.SystemRandomWait(Navigation.Speed.Normal);

            // Get weapons on bottom of page
            int rowsLeft = (int)Math.Ceiling((double)(weaponCount - currentweaponCount) / (double)maxColumns);
            int startPostion = 1;
            if( (weaponCount - currentweaponCount) > 21)
            {
                startPostion = 0;
            }
            for (int i = startPostion; i < (rowsLeft + startPostion); i++)
            {
                for (int k = 0; k < maxColumns; k++)
                {
                    if (weaponCount - currentweaponCount <= 0)
                    {
                        break;
                    }
                    Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X) + (xOffset * (k % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y) + (yOffset * (i % (maxRows + 1))));
                    Navigation.sim.Mouse.LeftButtonClick();
                    Navigation.SystemRandomWait(Navigation.Speed.Instant);

                    // Scan weapon
                    Weapon w = ScanWeapon(currentweaponCount);
                    currentweaponCount++;
                    //Debug.Print("Weapon Count: " + currentweaponCount.ToString());
                    //Debug.Print("Weapon Name: " + w.GetName());

                    // Add to weapon List Object
                    weapons.Add(w);
                }
            }//*/


            return weapons;
        }

        public static Weapon ScanWeapon(int id)
        {
            // Init Variables
            int name = 0;
            int level = 1;
            bool ascension = false;
            int refinementLevel = 1;
            int equippedCharacter = 0;

            // Grab Image of Entire weapon on Right
            Double weaponLocation_X = (Double)Navigation.GetArea().right * ((Double)108 / (Double)160);
            Double weaponLocation_Y = (Double)Navigation.GetArea().bottom * ((Double)10 / (Double)90);
            int width = 325; int height = 560;
            Bitmap bm = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bm);
            int screenLocation_X = Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X);
            int screenLocation_Y = Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y);
            g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

            // Display Image
            UserInterface.SetImage(bm);

            // Check for Rarity
            Color rarityColor = bm.GetPixel(12, 10);
            Color fiveStar = Color.FromArgb(255, 188, 105, 50);
            Color fourthStar = Color.FromArgb(255, 161, 86, 224);
            Color threeStar = Color.FromArgb(255, 81, 127, 203);
            // Check for equipped color
            Color equipped = Color.FromArgb(255, 255, 231, 187);
            Color equippedColor = bm.GetPixel(5, height - 10);

            // Scan different parts of the weapon

            bool b_equipped = Scraper.CompareColors(equipped, equippedColor);
            bool b_RarityAboveTwo = Scraper.CompareColors(fiveStar, rarityColor) || Scraper.CompareColors(fourthStar, rarityColor) || Scraper.CompareColors(threeStar, rarityColor);
            // TODO:: ADD multi threading support 
            Bitmap bm_1 = bm.Clone(new Rectangle(0, 0, width, height), bm.PixelFormat);
            Bitmap bm_2 = bm.Clone(new Rectangle(0, 0, width, height), bm.PixelFormat);
            Bitmap bm_3 = bm.Clone(new Rectangle(0, 0, width, height), bm.PixelFormat);
            Bitmap bm_4 = bm.Clone(new Rectangle(0, 0, width, height), bm.PixelFormat);

            Thread thr1 = new Thread(() => name = ScanWeaponName(bm_1, width, height));
            Thread thr2 = new Thread(() => level = ScanWeaponLevel(bm_2, width, height, ref ascension));
            Thread thr3 = new Thread(() => refinementLevel = ScanWeaponRefinement(bm_3, width, height));
            Thread thr4 = new Thread(() => equippedCharacter = ScanWeaponEquippedCharacter(bm_4, width, height));

            // Start Threads

            thr1.Start();thr2.Start();
            if (b_RarityAboveTwo)
            {
                thr3.Start();
            }
            if(b_equipped)
            {
                thr4.Start();
            }

            // End Threads

            thr1.Join(); thr2.Join();
            if (b_equipped)
            {
                thr4.Join();
            }
            if (b_RarityAboveTwo)
            {
                thr3.Join();
            }

            


            Weapon weapon = new Weapon(name,level,ascension,refinementLevel,equippedCharacter,id);

            return weapon;
        }

        public static int ScanWeaponCount()
        {
            //Find weapon count
            Double weaponCountLocation_X = (Double)Navigation.GetArea().right * ((Double)130 / (Double)160);
            Double weaponCountLocation_Y = (Double)Navigation.GetArea().bottom * ((Double)3 / (Double)90);
            Bitmap bm = new Bitmap(160, 16);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(Navigation.GetPosition().left + Convert.ToInt32(weaponCountLocation_X), Navigation.GetPosition().top + Convert.ToInt32(weaponCountLocation_Y), 0, 0, bm.Size);

            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(60.0, ref bm);
            Scraper.SetInvert(ref bm);
            bm = Scraper.ResizeImage(bm, bm.Width * 2, bm.Height * 2);

            string text = Scraper.AnalyzeText(bm);

            //UserInterface.Reset();
            //UserInterface.SetImage(bm);
            //UserInterface.AddText(text);

            return Int32.Parse(text.Split()[1].Split('/')[0]);
        }

        public static int ScanWeaponName(Bitmap weaponImage, int max_X, int max_Y)
        {
            int name = 0;

            //Init
            int xOffset = 10;
            int yOffset = 7;
            //Bitmap bm = new Bitmap(max_X-2*xOffset, 25);
            Bitmap bm = weaponImage.Clone(new Rectangle(xOffset, yOffset, max_X-2*xOffset, 25), weaponImage.PixelFormat);

            // Setup Img
            Graphics g = Graphics.FromImage(bm);
            //g.CopyFromScreen(weaponLocation_X + xOffset, weaponLocation_Y + yOffset, 0, 0, bm.Size);
            //g.Dispose();
            // View Picture
            //UserInterface.Reset();
            //UserInterface.SetImage(bm);

            Scraper.SetGamma(0.2, 0.2, 0.2, ref bm);
            Scraper.SetGrayscale(ref bm);
            Scraper.SetInvert(ref bm);

            //Scraper.SetGrayscale(ref bm);
            //Scraper.SetInvert(ref bm);
            //Scraper.SetContrast(80.0, ref bm);

            // View Picture
            UserInterface.Reset();
            UserInterface.SetImage(bm);

            //Scraper.SetGrayscale(ref bm);
            //Scraper.SetInvert(ref bm);
            //Scraper.SetGamma(0.2, 0.2, 0.2, ref bm);

            //Scraper.SetGrayscale(ref bm);
            //Scraper.SetInvert(ref bm);
            //Scraper.SetContrast(20.0, ref bm);


            //bm = Scraper.ResizeImage(bm, max_X * 2, max_Y * 2);

            // View Picture
            //UserInterface.Reset();
            //UserInterface.SetImage(bm);

            // Analyze
            //string text = Scraper.AnalyzeText(bm);
            string text = Scraper.AnalyzeText_1(bm);
            text = text.Trim();

            // View Picture
            UserInterface.Reset();
            UserInterface.SetImage(bm);
            UserInterface.AddText(text);
            text = text.Trim();
            text = Regex.Replace(text, @"(?![A-Za-z\s]).", "");
            //Debug.Print("Weapon Name: " + text);

            // Check in Dictionary
            name = Scraper.GetWeaponCode(text);

            return name;
        }

        public static int ScanWeaponLevel(Bitmap weaponImage, int max_X, int max_Y, ref bool ascension)
        {
            // Get Level
            int xOffset = 14;
            int yOffset = 204;
            //Bitmap bm = new Bitmap(110, 22); // old was 100
            Bitmap bm = weaponImage.Clone(new Rectangle(xOffset, yOffset, 110, 22), weaponImage.PixelFormat);
            //Graphics g = Graphics.FromImage(bm);
            //g.CopyFromScreen(weaponLocation_X + xOffset, weaponLocation_Y + yOffset, 0, 0, bm.Size);
            //g.DrawRectangle(new Pen(bm.GetPixel(1, 20), 22), new Rectangle(0, 0, max_X, max_Y));
            //bm = Scraper.ResizeImage(bm, max_X * 2, max_Y * 2);

            Scraper.SetGrayscale(ref bm);
            Scraper.SetInvert(ref bm);
            Scraper.SetContrast(100.0, ref bm);

            // View Picture
            UserInterface.Reset();
            UserInterface.SetImage(bm);


            string text = Scraper.AnalyzeText_2(bm);
            //string text = Scraper.AnalyzeOneText(bm);
            text = Regex.Replace(text, @"(?![0-9\s/]).", "");
            text = text.Trim();
            text = Regex.Replace(text, @"(\s{1}.*)", "");
            text = text.Trim();

            // View Picture
            UserInterface.Reset();
            UserInterface.SetImage(bm);
            UserInterface.AddText(text);

            if (text.Contains('/'))
            {
                string[] temp = text.Split(new[]{ '/' }, 2);

                if(temp.Length == 2)
                {
                    if (temp[0] == temp[1])
                        ascension = true;

                    int level = -1;
                    if (int.TryParse(temp[0],out level))
                    {
                        return level;
                    }
                    else
                    {
                        Debug.Print("Error: Found " + temp[0] + "instead of Weapon Level.");
                        System.Environment.Exit(1);
                        return level;
                    }
                }
                else
                {
                    Debug.Print("Error: Found " + temp[0] + "instead of Weapon Level.");
                    System.Environment.Exit(1);
                    return -1;
                }

            }
            else
            {
                Debug.Print("Error: Found " + text + "instead of Weapon Level." );
                System.Environment.Exit(1);
                return -1;
            }
        }

        public static int ScanWeaponRefinement(Bitmap weaponImage, int max_X, int max_Y)
        {
            int xOffset = 13;
            int yOffset = 230;
            // Get Level
            //Bitmap bm = new Bitmap(30, 30);
            Bitmap bm = weaponImage.Clone(new Rectangle(xOffset, yOffset, 30, 30), weaponImage.PixelFormat);
            Graphics g = Graphics.FromImage(bm);
            //g.CopyFromScreen(weaponLocation_X + xOffset, weaponLocation_Y + yOffset, 0, 0, bm.Size);
            g.DrawRectangle(new Pen(bm.GetPixel(7, 10), 14), new Rectangle(0, 0, 30, 30));
            //Scraper.SetContrast(100.0, ref bm);
            
            //Scraper.SetContrast(60.0, ref bm);
            Scraper.SetInvert(ref bm);
            Scraper.SetGrayscale(ref bm);
            //bm = Scraper.ResizeImage(bm, max_X * 2, max_Y * 2);

            string text = Scraper.AnalyzeText_3(bm);
            text = text.Trim();
            text = Regex.Replace(text, @"[!@#$%^&*()\[\]\-_`~\\+={};:',.<>?‘]", "");

            // View Picture
            UserInterface.Reset();
            UserInterface.SetImage(bm);
            UserInterface.AddText(text);

            // Parse Int
            int refinementLevel = -1;
            if(int.TryParse(text, out refinementLevel))
            {
                return refinementLevel;
            }
            else
            {
                // try again to try to get 5
                bm = new Bitmap(30, 30);
                bm = weaponImage.Clone(new Rectangle(xOffset, yOffset, max_X - xOffset, max_Y - yOffset), weaponImage.PixelFormat);
                g = Graphics.FromImage(bm);
                //g.CopyFromScreen(weaponLocation_X + xOffset, weaponLocation_Y + yOffset, 0, 0, bm.Size);
                g.DrawRectangle(new Pen(bm.GetPixel(7, 10), 14), new Rectangle(0, 0, max_X, max_Y));
                //Scraper.SetContrast(100.0, ref bm);
                Scraper.SetGrayscale(ref bm);
                //Scraper.SetContrast(60.0, ref bm);
                //Scraper.SetInvert(ref bm);
                bm = Scraper.ResizeImage(bm, max_X * 2, max_Y * 2);

                text = Scraper.AnalyzeText(bm);
                text = text.Trim();

                // View Picture
                UserInterface.Reset();
                UserInterface.SetImage(bm);
                UserInterface.AddText(text);

                refinementLevel = -1;
                if (int.TryParse(text, out refinementLevel))
                {
                    return refinementLevel;
                }
                else
                    return refinementLevel;
            }
        }

        public static int ScanWeaponEquippedCharacter(Bitmap weaponImage, int max_X, int max_Y)
        {
            int xOffset = 30;
            int yOffset = 532;

            Bitmap bm = weaponImage.Clone(new Rectangle(xOffset, yOffset, max_X - xOffset, 20), weaponImage.PixelFormat);
            Graphics g = Graphics.FromImage(bm);
            // Gets rid of character head on Left
            g.DrawRectangle(new Pen(bm.GetPixel(max_X - xOffset - 1, 10), 14), new Rectangle(0, 0, 13, bm.Height));
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(60.0, ref bm);

            string equippedCharacter = Scraper.AnalyzeText_4(bm);
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
                    equippedCharacter = Regex.Replace(equippedCharacter, @"[\/!@#$%^&*()\[\]\-_`~\\+={};:',.<>?‘|]", "");

                    return Scraper.GetCharacterCode(equippedCharacter);
                }
            }
            // artifact has no equipped character
            return 0;
        }
    }
}
