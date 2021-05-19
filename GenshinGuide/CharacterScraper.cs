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
            int xOffset = 175;
            int yOffset = 26;
            Bitmap bm = new Bitmap(240, 30);
            Graphics g = Graphics.FromImage(bm);
            int screenLocation_X = Navigation.GetPosition().left + xOffset;
            int screenLocation_Y = Navigation.GetPosition().top + yOffset;
            g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

            //Image Operations
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);
            Scraper.SetGrayscale(ref bm);
            Scraper.SetInvert(ref bm);
            Scraper.SetContrast(20.0, ref bm);

            UserInterface.Reset();
            UserInterface.SetImage(bm);

            string text = Scraper.AnalyzeText(bm);
            text = text.Trim();

            if (text != "")
            {
                text = Regex.Replace(text, @"[\/!@#$%^&*()\[\]\-_`~\\+={};:',.<>?‘]", "");
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
                Navigation.SystemRandomWait();
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
                Navigation.SystemRandomWait(Navigation.Speed.Normal);
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
                do
                {
                    level = ScanLevel(ref ascension);
                    Navigation.SystemRandomWait(Navigation.Speed.Normal);
                    currentRuntimes++;
                } while (level == -1 && currentRuntimes < maxRuntimes);

                // Scan Experience
                //experience = ScanExperience();

                // Scan Constellation
                Navigation.SelectCharacterConstellation();
                constellation = ScanConstellations(name);

                // Scan Talents
                Navigation.SelectCharacterTalents();
                talents = ScanTalents();

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
            Bitmap bm = new Bitmap(158,23);
            Graphics g = Graphics.FromImage(bm);
            int screenLocation_X = Navigation.GetPosition().left + xOffset;
            int screenLocation_Y = Navigation.GetPosition().top + yOffset;
            g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

            //Image Operations
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);
            Scraper.SetGrayscale(ref bm);
            Scraper.SetInvert(ref bm);
            Scraper.SetContrast(20.0, ref bm);

            UserInterface.Reset();
            UserInterface.SetImage(bm);

            string text = Scraper.AnalyzeText(bm);
            text = text.Trim();

            if(text != "")
            {
                text = Regex.Replace(text, @"[\/!@#$%^&*()\[\]\-_`~\\+={};:',.<>?‘]", "");
                //text = Regex.Replace(text, @"(?![\0-9\s]).", "");
                text = Regex.Replace(text, @"[\s]{2,}", " ");
                string[] x = text.Split(new[] {' '}, 2);

                if (x.Length != 2)
                {
                    Debug.Print("Error: " + x + " is not a valid element and character name.");
                    //System.Environment.Exit(1);
                }
                else
                {
                    UserInterface.AddText("Element: " + x[0]);
                    UserInterface.AddText(System.Environment.NewLine);
                    UserInterface.AddText("Name: " + x[1]);

                    element = Scraper.GetElementalCode(x[0],true);
                    name = Scraper.GetCharacterCode(x[1],true);
                }
            }
        }

        private static int ScanLevel(ref bool ascension)
        {
            int level = -1;

            int xOffset = 1032;
            int yOffset = 138;
            Bitmap bm = new Bitmap(90, 24);
            Graphics g = Graphics.FromImage(bm);
            int screenLocation_X = Navigation.GetPosition().left + xOffset;
            int screenLocation_Y = Navigation.GetPosition().top + yOffset;
            g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

            //Image Operations
            bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);
            Scraper.SetGrayscale(ref bm);
            Scraper.SetInvert(ref bm);
            Scraper.SetContrast(20.0, ref bm);

            UserInterface.Reset();
            UserInterface.SetImage(bm);

            string text = Scraper.AnalyzeFewText(bm);
            text = text.Trim();
            text = Regex.Replace(text, @"(?![0-9\s/]).", "");
            text = Regex.Replace(text, @"/", " ");

            UserInterface.AddText(text);

            if (Regex.IsMatch(text, " "))
            {
                string[] temp = text.Split(' ');
                int x = -1;
                int y = -1;
                if(int.TryParse(temp[0],out x) && int.TryParse(temp[1], out y))
                {
                    level = Convert.ToInt32(temp[0]);
                    if(level != y && level > 0 && level <= characterMaxLevel)
                    {
                        ascension = true;
                    }
                }
                else
                {
                    Debug.Print("Error: Found " + temp[0] + " and " + temp[1] + " instead of level");
                    //System.Environment.Exit(1);
                }
            }
            else
            {
                Debug.Print("Error: Found " + level + " instead of level");
                //System.Environment.Exit(1);
            }

            return level;
        }

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

            UserInterface.Reset();
            UserInterface.SetImage(bm);

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
                Navigation.SystemRandomWait();

                // Grab Color
                g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);
                UserInterface.Reset();
                UserInterface.SetImage(bm);
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

                        UserInterface.SetImage(bm1);

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

            UserInterface.AddText(constellation.ToString());
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
                Navigation.SystemRandomWait(Navigation.Speed.UI);
                g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

                bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);
                Scraper.SetGrayscale(ref bm);
                Scraper.SetInvert(ref bm);
                Scraper.SetContrast(60.0, ref bm);

                UserInterface.Reset();
                UserInterface.SetImage(bm);

                text = Scraper.AnalyzeText(bm);
                text = text.Trim();
                text = Regex.Replace(text, @"(?![0-9]).", "");
                UserInterface.AddText(text);

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
                        //System.Environment.Exit(1);
                    }
                    //Debug.Print("Error: " + x + " is not a valid Talent Number");
                    //System.Environment.Exit(1);
                }
            }

            Navigation.sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.ESCAPE);
            Navigation.SystemRandomWait();

            return talents;
        }
    }
}
