using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using System.Numerics;

namespace GenshinGuide
{
    public static class CharacterScraper
    {
        private static int firstCharacterName = -1;
        private static int characterMaxLevel = 90;

        public static string ScanMainCharacterName()
        {
            int xOffset = 185;
            int yOffset = 26;
            Bitmap bm = new Bitmap(275, 30);
            Graphics g = Graphics.FromImage(bm);
            int screenLocation_X = Navigation.GetPosition().left + xOffset;
            int screenLocation_Y = Navigation.GetPosition().top + yOffset;
            g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

            //Image Operations
            Scraper.SetGamma(0.2, 0.2, 0.2, ref bm);
            Scraper.SetInvert(ref bm);
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(40.0, ref bm);
            //bm = Scraper.ResizeImage(bm, bm.Width * 2, bm.Height * 2);
            //Scraper.SetGrayscale(ref bm);
            //Scraper.SetInvert(ref bm);
            //Scraper.SetContrast(80.0, ref bm);

            UserInterface.Reset();
            UserInterface.SetImage(bm);
            Application.DoEvents();

#if DEBUG
            if (Scraper.s_bDoDebugOnlyCode)
            {
                UserInterface.Reset();
                UserInterface.SetImage(bm);
            }
#endif

            string text = Scraper.AnalyzeText(bm);
            text = text.Trim();

            if (text != "")
            {
                text = Regex.Replace(text, @"[\W_]", "");
                // Get rid of space and the text right after it
                text = Regex.Replace(text, @"\s+\w*", "");
            }
            else
            {
                System.Environment.Exit(1);
            }

            return text;
        }

        public static List<Character> ScanCharacters()
        {
            List<Character> characters = new List<Character>();

            // first character name is used to stop scanning characters
            Character character;
            while(ScanCharacter(out character))
            {
                characters.Add(character);
                Navigation.SelectNextCharacter();
                Navigation.SystemRandomWait(Navigation.Speed.SelectNextCharacter);
            }


            return characters;
        }

        private static bool ScanCharacter(out Character character)
        {
            int name = -1;
            int element = -1;
            int level = -1;
            bool ascension = false;
            int experience = 0;
            int constellation = 0;
            int[] talents = new int[3];

            // Scan the Name and element of Character
            int maxRuntimes = 100;
            int currentRuntimes = 0;
            do
            {
                ScanNameAndElement(ref name, ref element);
                Navigation.SystemRandomWait(Navigation.Speed.Faster);
                currentRuntimes++;
            } while ( (name <= 0 || element == -1) && (currentRuntimes < maxRuntimes) );
            

            // Check if character has been scanned before
            if (name != firstCharacterName)
            {
                // assign the first name to serve as first index
                if (firstCharacterName == -1)
                    firstCharacterName = name;

                // Scan Level and ascension
                currentRuntimes = 0;
                Navigation.SelectCharacterAttributes();
                // Used to make remove numbers altered by the stars in background
                List<int> LevelComparison = new List<int>();
                do
                {
                    level = ScanLevel(ref ascension);
                    Navigation.SystemRandomWait(Navigation.Speed.Faster);
                    currentRuntimes++;
                    // check if level exists in Level comparison
                    if (!LevelComparison.Contains(level))
                    {
                        LevelComparison.Add(level);
                        level = -1;
                    }

                } while (level == -1 && currentRuntimes < maxRuntimes);

                // Scan Experience
                //experience = ScanExperience();

                // Scan Constellation
                Navigation.SelectCharacterConstellation();
                constellation = ScanConstellations(name);

                // Scan Talents
                Navigation.SelectCharacterTalents();
                talents = ScanTalents();

                // Scale down talents due to constellations
                if(constellation >= 3)
                {
                    // get talent if character 
                    string talent = Scraper.characterTalentConstellationOrder[name][0];
                    if (constellation >= 5)
                    {
                        talents[1] = talents[1] - 3;
                        talents[2] = talents[2] - 3;
                    }
                    else if (talent == "skill")
                    {
                        talents[1] = talents[1] - 3;
                    }
                    else
                    {
                        talents[2] = talents[2] - 3;
                    }

                }

                character = new Character(name, element, level, ascension, experience, constellation, talents);
                return true;
            }
            else
            {
                character = new Character(name, element, level, ascension, experience, constellation, talents);
                return false;
            }
        }

        private static void ScanNameAndElement(ref int name, ref int element)
        {
            int xOffset = 83;
            int yOffset = 22;

            //Bitmap bm = new Bitmap(220,23);
            //Bitmap bm = new Bitmap(220, 20);
            Bitmap bm = new Bitmap(220, 20);
            Graphics g = Graphics.FromImage(bm);
            int screenLocation_X = Navigation.GetPosition().left + xOffset;
            int screenLocation_Y = Navigation.GetPosition().top + yOffset;
            g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

            //Image Operations
            //bm = bm.Clone(new Rectangle(0, 0, 220, 20), bm.PixelFormat);
            Scraper.SetGrayscale(ref bm);
            Scraper.SetInvert(ref bm);
            Scraper.SetContrast(100.0, ref bm);
            bm = Scraper.ResizeImage(bm, bm.Width * 3, bm.Height * 3);

            //UserInterface.Reset();
            //UserInterface.SetImage(bm);
            //Application.DoEvents();

#if DEBUG
            if (Scraper.s_bDoDebugOnlyCode)
            {
                UserInterface.Reset();
                UserInterface.SetImage(bm);
            }
#endif

            //string text = Scraper.AnalyzeElementAndCharName(bm);
            string text = Scraper.AnalyzeText(bm);
            text = text.Trim();

            if(text != "")
            {
                text = Regex.Replace(text, @"[!@#$%^&*()\[\]\-_`~\\+={};:',.<>?‘]", "");
                text = Regex.Replace(text, @"\s{1,}\/", "/");
                text = Regex.Replace(text, @"\/\s{1,}", "/");
                // Get rid of / in front of Anemo bug
                if(text[0] == '/')
                {
                    text = text.Substring(1);
                }

                //text = Regex.Replace(text, @"(?![\0-9\s]).", "");
                //text = Regex.Replace(text, @"[\s]{2,}", " ");

                string[] x = text.Split(new[] {'/'}, 2);
                // Get rid of spacing 
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = x[i].Trim();
                }

                if (x.Length <= 1)
                {
                    Debug.Print("Error: " + x + " is not a valid element and character name.");
                    //System.Environment.Exit(1);
                }
                else
                {
#if DEBUG
                    if (Scraper.s_bDoDebugOnlyCode)
                    {
                        UserInterface.AddText("Element: " + x[0]);
                        UserInterface.AddText(System.Environment.NewLine);
                        UserInterface.AddText("Name: " + x[1]);
                    }
#endif

                    element = Scraper.GetElementalCode(x[0],true);

                    int temp = -1;
                    // strip each char from name until found in dictionary
                    while(x[1].Length > 1)
                    {
                        temp = Scraper.GetCharacterCode(x[1], true);
                        if(temp == -1)
                        {
                            x[1] = x[1].Substring(0, x[1].Length - 1);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if(x[1].Length > 1)
                    {
                        name = Scraper.GetCharacterCode(x[1], false);
                    }
                    
                }
            }
        }

        private static int ScanLevel(ref bool ascension)
        {
            int level = -1;

            //int xOffset = 1035;
            int xOffset = 960;
            //int yOffset = 138;
            int yOffset = 135;
            //Bitmap bm = new Bitmap(80, 24);
            Bitmap bm = new Bitmap(165, 28);
            Graphics g = Graphics.FromImage(bm);
            int screenLocation_X = Navigation.GetPosition().left + xOffset;
            int screenLocation_Y = Navigation.GetPosition().top + yOffset;
            g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

            //Image Operations
            bm = Scraper.ResizeImage(bm, bm.Width * 2, bm.Height * 2);
            Scraper.SetGrayscale(ref bm);
            Scraper.SetInvert(ref bm);
            Scraper.SetContrast(30.0, ref bm);

            

            //string text = Scraper.AnalyzeFewText(bm);
            string text = Scraper.AnalyzeText(bm);
            text = text.Trim();
            text = Regex.Replace(text, @"(?![0-9/]).", "");
            text = Regex.Replace(text, @"/", " ");

#if DEBUG
            if (Scraper.s_bDoDebugOnlyCode)
            {
                UserInterface.Reset();
                UserInterface.SetImage(bm);
                UserInterface.AddText(text);
            }
#endif

            string[] temp = { text };

            if (Regex.IsMatch(text, " "))
            {
                temp = text.Split(' ');
            }
            else if (temp.Length == 1)
            {
                //int numR = (int)((temp[0].Length/2) + 1);
                //string[] temp1 = new string[2];
                //temp1[0] = temp[0].Substring(0, numR - 1);
                //temp1[1] = temp[0].Substring(temp1[0].Length + 1, numR - 1);
                //temp = temp1;
                return level;
            }

            if (temp.Length == 3)
            {
                string[] temp1 = new string[2];
                temp1[0] = temp[0];
                temp1[1] = temp[2];
                temp = temp1;
            }
            int x = -1;
            int y = -1;
            if (int.TryParse(temp[0], out x) && int.TryParse(temp[1], out y))
            {
                // level must be within 1-100
                if (x > 0 && x < 101)
                {
                    level = Convert.ToInt32(temp[0]);
                    if (level != y && level > 0 && level <= characterMaxLevel)
                    {
                        ascension = true;
                    }
                }
            }
            else
            {
                if (temp.Length > 1)
                {
                    Debug.Print("Error: Found " + temp[0] + " and " + temp[1] + " instead of level");
                    //System.Environment.Exit(1);
                }
                else
                {
                    Debug.Print("Error: Found " + temp[0] + " instead of level");
                    //System.Environment.Exit(1);
                }

            }

            return level;
        }

        //private static int ScanLevel(ref bool ascension)
        //{
        //    int level = -1;

        //    int xOffset = 1035;
        //    int yOffset = 138;
        //    Bitmap bm = new Bitmap(80, 24);
        //    Graphics g = Graphics.FromImage(bm);
        //    int screenLocation_X = Navigation.GetPosition().left + xOffset;
        //    int screenLocation_Y = Navigation.GetPosition().top + yOffset;
        //    g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

        //    //Image Operations
        //    bm = Scraper.ResizeImage(bm, bm.Width * 2, bm.Height * 2);
        //    Scraper.SetGrayscale(ref bm);
        //    Scraper.SetInvert(ref bm);
        //    Scraper.SetContrast(30.0, ref bm);

        //    UserInterface.Reset();
        //    UserInterface.SetImage(bm);

        //    string text = Scraper.AnalyzeFewText(bm);
        //    text = text.Trim();
        //    text = Regex.Replace(text, @"(?![0-9\s/]).", "");
        //    text = Regex.Replace(text, @"/", " ");

        //    UserInterface.AddText(text);
        //    string[] temp = { text };

        //    if (Regex.IsMatch(text, " "))
        //    {
        //        temp = text.Split(' ');
        //    }
        //    else if(temp.Length == 1)
        //    {
        //        //int numR = (int)((temp[0].Length/2) + 1);
        //        //string[] temp1 = new string[2];
        //        //temp1[0] = temp[0].Substring(0, numR - 1);
        //        //temp1[1] = temp[0].Substring(temp1[0].Length + 1, numR - 1);
        //        //temp = temp1;
        //        return level;
        //    }

        //    if(temp.Length == 3)
        //    {
        //        string[] temp1 = new string[2];
        //        temp1[0] = temp[0];
        //        temp1[1] = temp[2];
        //        temp = temp1;
        //    }
        //    int x = -1;
        //    int y = -1;
        //    if(int.TryParse(temp[0],out x) && int.TryParse(temp[1], out y))
        //    {
        //        // level must be within 1-100
        //        if (x > 0 && x < 101)
        //        {
        //            level = Convert.ToInt32(temp[0]);
        //            if (level != y && level > 0 && level <= characterMaxLevel)
        //            {
        //                ascension = true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if(temp.Length > 1)
        //        {
        //            Debug.Print("Error: Found " + temp[0] + " and " + temp[1] + " instead of level");
        //            //System.Environment.Exit(1);
        //        }
        //        else
        //        {
        //            Debug.Print("Error: Found " + temp[0] + " instead of level");
        //            //System.Environment.Exit(1);
        //        }

        //    }

        //    return level;
        //}

        private static int ScanExperience()
        {
            int experience = 0;

            int xOffset = 1117;
            int yOffset = 151;
            Bitmap bm = new Bitmap(90, 10);
            Graphics g = Graphics.FromImage(bm);
            int screenLocation_X = Navigation.GetPosition().left + xOffset;
            int screenLocation_Y = Navigation.GetPosition().top + yOffset;
            g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

            //Image Operations
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);
            //Scraper.SetGrayscale(ref bm);
            //Scraper.SetInvert(ref bm);
            Scraper.SetContrast(30.0, ref bm);

#if DEBUG
            if (Scraper.s_bDoDebugOnlyCode)
            {
                UserInterface.Reset();
                UserInterface.SetImage(bm);
            }
#endif

            string text = Scraper.AnalyzeText(bm);
            text = text.Trim();
            text = Regex.Replace(text, @"(?![0-9\s/]).", "");

            if (Regex.IsMatch(text, "/"))
            {
                string[] temp = text.Split('/');
                experience = Convert.ToInt32(temp[0]);
            }
            else
            {
                Debug.Print("Error: Found " + experience + " instead of experience");
                System.Environment.Exit(1);
            }

            return experience;
        }

        private static int ScanConstellations(int name)
        {
            int constellation = 0;
            Color lockColor = Color.FromArgb(255,255,255,255);
            Color constellationColor = new Color();

            int xOffset = 155;
            int yOffset = 70;
            Bitmap bm = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(bm);
            int screenLocation_X = Navigation.GetPosition().left + xOffset;
            int screenLocation_Y = Navigation.GetPosition().top + yOffset;
            g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);
            //UserInterface.SetImage(bm);

            for(int i = 0; i < 6; i++)
            {
                // Select Constellation
                Navigation.SetCursorPos(Navigation.GetPosition().left + 1130, Navigation.GetPosition().top + 180 + (i * 75));
                Navigation.sim.Mouse.LeftButtonClick();
                // Selecting the first constellation takes a while to show
                if(i == 0)
                    Navigation.SystemRandomWait(Navigation.Speed.Normal);
                else
                    Navigation.SystemRandomWait(Navigation.Speed.Fast);

                // Grab Color
                g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);
#if DEBUG
                if (Scraper.s_bDoDebugOnlyCode)
                {
                    UserInterface.Reset();
                    UserInterface.SetImage(bm);
                }
#endif
                constellationColor = bm.GetPixel(0, 0);

                // Compare
                if (constellationColor == lockColor)
                {
                    // Check for character like Noelle with pure white in her constellation

                    if (name == 13 && i == 1)
                    {
                        // Check if says activate at bottom
                        Bitmap bm1 = new Bitmap(140, 24);
                        Graphics g1 = Graphics.FromImage(bm1);
                        int screenLocation_X1 = Navigation.GetPosition().left + 100;
                        int screenLocation_Y1 = Navigation.GetPosition().top + 667;
                        g1.CopyFromScreen(screenLocation_X1, screenLocation_Y1, 0, 0, bm1.Size);

#if DEBUG
                        if (Scraper.s_bDoDebugOnlyCode)
                        {
                            UserInterface.SetImage(bm1);
                        }
#endif

                        string text = Scraper.AnalyzeText(bm1);

                        if (text == "Activate")
                        {
                            break;
                        }
                    }
                    else
                        break;
                }
                constellation = i+1;

            }

#if DEBUG
            if (Scraper.s_bDoDebugOnlyCode)
            {
                UserInterface.AddText(constellation.ToString());
            }
#endif
            Navigation.sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.ESCAPE);
            Navigation.SystemRandomWait();
            return constellation;
        }

        private static int[] ScanTalents()
        {
            int[] talents = {-1,-1,-1};

            int xOffset = 165;
            int yOffset = 116;
            string text = "";
            int screenLocation_X = Navigation.GetPosition().left + xOffset;
            int screenLocation_Y = Navigation.GetPosition().top + yOffset;

            for(int i = 0; i < 3; i++)
            {
                Bitmap bm = new Bitmap(60, 25);
                Graphics g = Graphics.FromImage(bm);

                Navigation.SetCursorPos(Navigation.GetPosition().left + 1130, Navigation.GetPosition().top + 110 + (i * 60));
                Navigation.sim.Mouse.LeftButtonClick();
                if(i == 0)
                {
                    Navigation.SystemRandomWait(Navigation.Speed.Normal);
                }
                else
                {
                    Navigation.SystemRandomWait(Navigation.Speed.Fast);
                }

                g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

                bm = Scraper.ResizeImage(bm, bm.Width * 2, bm.Height * 2);
                Scraper.SetGrayscale(ref bm);
                Scraper.SetInvert(ref bm);
                Scraper.SetContrast(60.0, ref bm);

                text = Scraper.AnalyzeText(bm);
                text = text.Trim();
                text = Regex.Replace(text, @"(?![0-9]).", "");

#if DEBUG
                if (Scraper.s_bDoDebugOnlyCode)
                {
                    UserInterface.Reset();
                    UserInterface.SetImage(bm);
                    UserInterface.AddText(text);
                }
#endif

                int x = -1;
                if(int.TryParse(text, out x))
                {
                    if(x >= 1 && x <= 15)
                        talents[i] = x;
                }
                else
                {
                    text = Scraper.AnalyzeFewText(bm);
                    text = text.Trim();
                    text = Regex.Replace(text, @"(?![0-9]).", "");

                    int y = -1;
                    if (int.TryParse(text, out y))
                    {
                        if (y >= 1 && y <= 15)
                            talents[i] = y;
                    }
                    else
                    {
                        Debug.Print("Error: " + x + " is not a valid Talent Number");
                        // Try Again
                        i--;
                        System.Environment.Exit(1);
                    }
                    Debug.Print("Error: " + x + " is not a valid Talent Number");
                    System.Environment.Exit(1);
                }
            }

            Navigation.sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.ESCAPE);
            Navigation.SystemRandomWait();

            return talents;
        }
    }
}
