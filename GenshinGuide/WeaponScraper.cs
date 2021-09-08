using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
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
            int maxRows = 3;
            int currentColumn = 0;

            // offset used to move mouse to other weapons
            int xOffset = Convert.ToInt32((Double)Navigation.GetArea().right * ((Double)12.25 / (Double)160));
            int yOffset = Convert.ToInt32((Double)Navigation.GetArea().bottom * ((Double)14.5 / (Double)90));

            // Testing for single weapons. REMOVE LATER!!!
            //Weapon a = ScanWeapon(0);

            
            // Go through weapon list
            while (currentweaponCount < weaponCount)
            {
                if ((weaponCount - currentweaponCount <= (maxRows * maxColumns)) && (currentColumn == 0))
                {
                    break;
                }

                // Select weapon
                Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X) + (xOffset * (currentweaponCount % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y));
                Navigation.sim.Mouse.LeftButtonClick();
                //Navigation.SystemRandomWait(Navigation.Speed.Instant);

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
                GC.Collect();

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
                    //Navigation.SystemRandomWait();
                }
            };

            // scroll down as much as possible
            for (int i = 0; i < 20; i++)
            {
                Navigation.sim.Mouse.VerticalScroll(-1);
            }

            // Get weapons on bottom of page
            int rowsLeft = (int)Math.Ceiling((double)(weaponCount - currentweaponCount) / (double)maxRows);
            for (int i = 1; i < rowsLeft; i++)
            {
                for (int k = 0; k < maxColumns; k++)
                {
                    if (weaponCount - currentweaponCount <= 0)
                    {
                        break;
                    }
                    Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X) + (xOffset * (k % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y) + (yOffset * (i % (maxRows + 1))));
                    Navigation.sim.Mouse.LeftButtonClick();
                    //Navigation.SystemRandomWait(Navigation.Speed.Instant);

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

            //string text = Scraper.AnalyzeWeaponImage(bm);

            // TODO:: ADD multi threading support 

            // All Weapons will be scanned 
            name = ScanWeaponName(screenLocation_X, screenLocation_Y, width, height);
            // Skip weapons lower than 3 stars
            if(name >= 10)
            {
                level = ScanWeaponLevel(screenLocation_X, screenLocation_Y, width, height, ref ascension);
                refinementLevel = ScanWeaponRefinement(screenLocation_X, screenLocation_Y, width, height);
                equippedCharacter = ScanWeaponEquippedCharacter(screenLocation_X, screenLocation_Y, width, height);
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

        public static int ScanWeaponName(int weaponLocation_X, int weaponLocation_Y, int max_X, int max_Y)
        {
            int name = 0;

            //Init
            int xOffset = 10;
            int yOffset = 7;
            Bitmap bm = new Bitmap(max_X-2*xOffset, 25);

            // Setup Img
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(weaponLocation_X + xOffset, weaponLocation_Y + yOffset, 0, 0, bm.Size);
            g.Dispose();
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


            //bm = Scraper.ResizeImage(bm, bm.Width * 2, bm.Height * 2);

            // View Picture
            //UserInterface.Reset();
            //UserInterface.SetImage(bm);

            // Analyze
            //string text = Scraper.AnalyzeText(bm);
            string text = Scraper.AnalyzeText(bm);
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

        public static int ScanWeaponLevel(int weaponLocation_X, int weaponLocation_Y, int max_X, int max_Y, ref bool ascension)
        {
            // Get Level
            int xOffset = 15;
            int yOffset = 206;
            Bitmap bm = new Bitmap(150, 19); // old was 100
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(weaponLocation_X + xOffset, weaponLocation_Y + yOffset, 0, 0, bm.Size);
            //g.DrawRectangle(new Pen(bm.GetPixel(1, 20), 22), new Rectangle(0, 0, bm.Width, bm.Height));
            bm = Scraper.ResizeImage(bm, bm.Width * 2, bm.Height * 2);

            Scraper.SetGrayscale(ref bm);
            Scraper.SetInvert(ref bm);
            Scraper.SetContrast(100.0, ref bm);

            // View Picture
            UserInterface.Reset();
            UserInterface.SetImage(bm);


            string text = Scraper.AnalyzeText(bm);
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

        public static int ScanWeaponRefinement(int weaponLocation_X, int weaponLocation_Y, int max_X, int max_Y)
        {
            int xOffset = 13;
            int yOffset = 230;
            // Get Level
            Bitmap bm = new Bitmap(30, 30);
            //Bitmap bm = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(weaponLocation_X + xOffset, weaponLocation_Y + yOffset, 0, 0, bm.Size);
            g.DrawRectangle(new Pen(bm.GetPixel(7, 10), 14), new Rectangle(0, 0, bm.Width, bm.Height));
            //Scraper.SetContrast(100.0, ref bm);
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(60.0, ref bm);
            Scraper.SetInvert(ref bm);
            bm = Scraper.ResizeImage(bm, bm.Width * 2, bm.Height * 2);

            string text = Scraper.AnalyzeText(bm);
            text = text.Trim();

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
                g = Graphics.FromImage(bm);
                g.CopyFromScreen(weaponLocation_X + xOffset, weaponLocation_Y + yOffset, 0, 0, bm.Size);
                g.DrawRectangle(new Pen(bm.GetPixel(7, 10), 14), new Rectangle(0, 0, bm.Width, bm.Height));
                //Scraper.SetContrast(100.0, ref bm);
                Scraper.SetGrayscale(ref bm);
                //Scraper.SetContrast(60.0, ref bm);
                //Scraper.SetInvert(ref bm);
                bm = Scraper.ResizeImage(bm, bm.Width * 2, bm.Height * 2);

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

        public static int ScanWeaponEquippedCharacter(int weaponLocation_X, int weaponLocation_Y, int max_X, int max_Y)
        {
            int xOffset = 30;
            int yOffset = 532;
            Bitmap bm = new Bitmap(max_X - xOffset, 20);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(weaponLocation_X + xOffset, weaponLocation_Y + yOffset, 0, 0, bm.Size);
            g.DrawRectangle(new Pen(bm.GetPixel(max_X - xOffset - 1, 10), 14), new Rectangle(0, 0, 13, bm.Height));
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(30.0, ref bm);
            //g.DrawRectangle(new Pen(Brushes.Red, 14), new Rectangle(0, 0, 13, bm.Height));
            bm = Scraper.ResizeImage(bm, bm.Width * 2, bm.Height * 2);

            string equippedCharacter = Scraper.AnalyzeText(bm);
            equippedCharacter = equippedCharacter.Trim();

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
            // Weapon has no equipped character
            return 0;
        }
    }
}
