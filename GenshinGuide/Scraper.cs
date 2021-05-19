using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using Tesseract;
using Newtonsoft.Json;

namespace GenshinGuide
{
    public static class Scraper
    {
        // GLOBALS
        private static int setCount = 0;
        private static int mainStatCount = 0;
        private static int characterCount = 1;
        private static int weaponCount = 0;
        private static Dictionary<string, int> setNameCode = new Dictionary<string, int>
        {
            ["Adventurer"] = setCount,
            ["Lucky Dog"] = ++setCount,
            ["Traveling Doctor"] = ++setCount,
            ["Resolution of Sojourner"] = ++setCount,
            ["Tiny Miracle"] = ++setCount,
            ["Berserker"] = ++setCount, // 5
            ["Instructor"] = ++setCount,
            ["The Exile"] = ++setCount,
            ["Defenders Will"] = ++setCount,
            ["Brave Heart"] = ++setCount,
            ["Martial Artist"] = ++setCount, // 10
            ["Gambler"] = ++setCount,
            ["Scholar"] = ++setCount,
            ["Prayers for Illumination"] = ++setCount,
            ["Prayers for Destiny"] = ++setCount,
            ["Prayers for Wisdom"] = ++setCount, // 15
            ["Prayers for Springtime"] = ++setCount,
            ["Thundersoother"] = ++setCount,
            ["Lavawalker"] = ++setCount,
            ["Maiden Beloved"] = ++setCount,
            ["Gladiators Finale"] = ++setCount, // 20
            ["Viridescent Venerer"] = ++setCount,
            ["Wanderers Troupe"] = ++setCount,
            ["Thundering Fury"] = ++setCount,
            ["Crimson Witch of Flames"] = ++setCount,
            ["Noblesse Oblige"] = ++setCount, // 25
            ["Bloodstained Chivalry"] = ++setCount,
            ["Archaic Petra"] = ++setCount,
            ["Retracing Bolide"] = ++setCount,
            ["Heart of Depth"] = ++setCount,
            ["Blizzard Strayer"] = ++setCount, // 30
        };
        private static Dictionary<string, int> mainStatCode = new Dictionary<string, int>
        {
            ["HP"] = mainStatCount,
            ["ATK"] = ++mainStatCount,
            ["HP%"] = ++mainStatCount,
            ["ATK%"] = ++mainStatCount,
            ["DEF%"] = ++mainStatCount,
            ["Energy Recharge"] = ++mainStatCount, //5
            ["Elemental Mastery"] = ++mainStatCount,
            ["Healing Bonus"] = ++mainStatCount,
            ["CRIT Rate"] = ++mainStatCount,
            ["CRIT DMG"] = ++mainStatCount,
            ["Physical DMG Bonus"] = ++mainStatCount, //10
            ["Pyro DMG Bonus"] = ++mainStatCount,
            ["Electro DMG Bonus"] = ++mainStatCount,
            ["Cryo DMG Bonus"] = ++mainStatCount,
            ["Hydro DMG Bonus"] = ++mainStatCount,
            ["Anemo DMG Bonus"] = ++mainStatCount, //15
            ["Geo DMG Bonus"] = ++mainStatCount,
        };
        private static Dictionary<string, int> gearSlotCode = new Dictionary<string, int>
        {
            ["Flower of Life"] = 0,
            ["Plume of Death"] = 1,
            ["Sands of Eon"] = 2,
            ["Goblet of Eonothem"] = 3,
            ["Circlet of Logos"] = 4,
        };
        private static Dictionary<string, int> subStatCode = new Dictionary<string, int>
        {
            ["HP"] = 0,
            ["DEF"] = 1,
            ["ATK"] = 2,
            ["HP%"] = 3,
            ["DEF%"] = 4,
            ["ATK%"] = 5,
            ["Elemental Mastery"] = 6,
            ["Energy Recharge"] = 7,
            ["CRIT Rate"] = 8,
            ["CRIT DMG"] = 9,
        };
        private static Dictionary<string, int> characterCode = new Dictionary<string, int>
        {
            // Artifact has no one assigned
            [""] = 0,
            /////////// Traveler is Assigned at runtime /////////////
            //["Andross"] = 1,
            ///////////////////////////////////////////////
            ["Amber"] = ++characterCount,
            ["Kaeya"] = ++characterCount,
            ["Lisa"] = ++characterCount,
            ["Barbara"] = ++characterCount, //5
            ["Razor"] = ++characterCount,
            ["Xiangling"] = ++characterCount,
            ["Beidou"] = ++characterCount,
            ["Xingqiu"] = ++characterCount,
            ["Xinggiu"] = characterCount, // dup
            ["Ningguang"] = ++characterCount, // 10
            ["Fischl"] = ++characterCount,
            ["Bennett"] = ++characterCount,
            ["Noelle"] = ++characterCount,
            ["Chongyun"] = ++characterCount,
            ["Sucrose"] = ++characterCount, // 15
            ["Jean"] = ++characterCount,
            ["Diluc"] = ++characterCount,
            ["Qiqi"] = ++characterCount,
            ["Qigi"] = characterCount, // dup
            ["Mona"] = ++characterCount,
            ["Keqing"] = ++characterCount, // 20
            ["Keging"] = characterCount, // dup
            ["Keqging"] = characterCount, // dup
            ["Venti"] = ++characterCount,
            ["Klee"] = ++characterCount,
            ["Diona"] = ++characterCount,
            ["Tartaglia"] = ++characterCount,
            ["Xinyan"] = ++characterCount, // 25
            ["Zhongli"] = ++characterCount,
            ["Albedo"] = ++characterCount,
            ["Ganyu"] = ++characterCount,
            ["Xiao"] = ++characterCount,
            ["Hu Tao"] = ++characterCount, // 30
            ["Rosaria"] = ++characterCount,
        };
        private static Dictionary<string, int> elementalCode = new Dictionary<string, int>
        {
            ["Pyro"] = 0,
            ["Hydro"] = 1,
            ["Dendro"] = 2,
            ["Electro"] = 3,
            ["Anemo"] = 4,
            ["Cryo"] = 5,
            ["Geo"] = 6,
        };
        private static Dictionary<string, int> weaponCode = new Dictionary<string, int>
        {
            // 1 Star
            ["Dull Blade"] = weaponCount,
            ["Waster Greatsword"] = ++weaponCount,
            ["Beginners Protector"] = ++weaponCount,
            ["Apprentices Notes"] = ++weaponCount,
            ["Hunters Bow"] = ++weaponCount,
            // 2 Stars
            ["Silver Sword"] = ++weaponCount,
            ["Old Mercs Pal"] = ++weaponCount,
            ["Iron Point"] = ++weaponCount,
            ["Pocket Grimoire"] = ++weaponCount,
            ["Seasoned Hunters Bow"] = ++weaponCount,
            // 3 Star
            ["Cool Steel"] = ++weaponCount,
            ["Harbinger of Dawn"] = ++weaponCount,
            ["Travelers Handy Sword"] = ++weaponCount,
            ["Fillet Blade"] = ++weaponCount,
            ["Skyrider Sword"] = ++weaponCount,
            ["Ferrous Shadow"] = ++weaponCount,
            ["Bloodtainted Greatsword"] = ++weaponCount,
            ["White Iron Greatsword"] = ++weaponCount,
            ["Debate Club"] = ++weaponCount,
            ["Skyrider Greatsword"] = ++weaponCount,
            ["White Tassel"] = ++weaponCount,
            ["Halberd"] = ++weaponCount,
            ["Black Tassel"] = ++weaponCount,
            ["Magic Guide"] = ++weaponCount,
            ["Thrilling Tales of Dragon Slayers"] = ++weaponCount,
            ["Otherworldly Story"] = ++weaponCount,
            ["Emerald Orb"] = ++weaponCount,
            ["Twin Nephrite"] = ++weaponCount,
            ["Raven Bow"] = ++weaponCount,
            ["Sharpshooters Oath"] = ++weaponCount,
            ["Recurve Bow"] = ++weaponCount,
            ["Slingshot"] = ++weaponCount,
            ["Messenger"] = ++weaponCount,
            // 4 star
            ["Favonius Sword"] = ++weaponCount,
            ["The Flute"] = ++weaponCount,
            ["Sacrificial Sword"] = ++weaponCount,
            ["Royal Longsword"] = ++weaponCount,
            ["Lions Roar"] = ++weaponCount,
            ["Prototype Rancour"] = ++weaponCount,
            ["Iron Sting"] = ++weaponCount,
            ["Blackcliff Longsword"] = ++weaponCount,
            ["The Black Sword"] = ++weaponCount,
            ["Sword of Descension"] = ++weaponCount,
            ["Festering Desire"] = ++weaponCount,
            ["The Alley Flash"] = ++weaponCount,
            ["Favonius Greatsword"] = ++weaponCount,
            ["The Bell"] = ++weaponCount,
            ["Sacrificial Greatsword"] = ++weaponCount,
            ["Royal Greatsword"] = ++weaponCount,
            ["Rainslasher"] = ++weaponCount,
            ["Prototype Archaic"] = ++weaponCount,
            ["Whiteblind"] = ++weaponCount,
            ["Blackcliff Slasher"] = ++weaponCount,
            ["Serpent Spine"] = ++weaponCount,
            ["SnowTombed Starsilver"] = ++weaponCount,
            ["Lithic Blade"] = ++weaponCount,
            ["Dragons Bane"] = ++weaponCount,
            ["Prototype Starglitter"] = ++weaponCount,
            ["Crescent Pike"] = ++weaponCount,
            ["Blackcliff Pole"] = ++weaponCount,
            ["Deathmatch"] = ++weaponCount,
            ["Favonius Lance"] = ++weaponCount,
            ["Royal Spear"] = ++weaponCount,
            ["Dragonspine Spear"] = ++weaponCount,
            ["Lithic Spear"] = ++weaponCount,
            ["Favonius Codex"] = ++weaponCount,
            ["The Widsith"] = ++weaponCount,
            ["Sacrificial Fragments"] = ++weaponCount,
            ["Royal Grimoire"] = ++weaponCount,
            ["Solar Pearl"] = ++weaponCount,
            ["Prototype Amber"] = ++weaponCount,
            ["Mappa Mare"] = ++weaponCount,
            ["Blackcliff Agate"] = ++weaponCount,
            ["Eye of Perception"] = ++weaponCount,
            ["Frostbearer"] = ++weaponCount,
            ["Wine and Song"] = ++weaponCount,
            ["Favonius Warbow"] = ++weaponCount,
            ["The Stringless"] = ++weaponCount,
            ["Sacrificial Bow"] = ++weaponCount,
            ["Royal Bow"] = ++weaponCount,
            ["Rust"] = ++weaponCount,
            ["Prototype Crescent"] = ++weaponCount,
            ["Compound Bow"] = ++weaponCount,
            ["Blackcliff Warbow"] = ++weaponCount,
            ["The Viridescent"] = ++weaponCount,
            ["Alley Hunter"] = ++weaponCount,
            ["Windblume Ode"] = ++weaponCount,
            ["Aquilia Favonia"] = ++weaponCount,
            ["Skyward Blade"] = ++weaponCount,
            ["Summit Shaper"] = ++weaponCount,
            ["Primordial Jade Cutter"] = ++weaponCount,
            ["Skyward Pride"] = ++weaponCount,
            ["Wolfs Gravestone"] = ++weaponCount,
            ["The Unforged"] = ++weaponCount,
            ["Primordial Jade WingedSpear"] = ++weaponCount,
            ["Skyward Spine"] = ++weaponCount,
            ["Vortext Vanquisher"] = ++weaponCount,
            ["Staff of Homa"] = ++weaponCount,
            ["Skyward Atlas"] = ++weaponCount,
            ["Lost Prayer to the Sacred Winds"] = ++weaponCount,
            ["Memory of Dust"] = ++weaponCount,
            ["Skyward Harp"] = ++weaponCount,
            ["Amos Bow"] = ++weaponCount,
            ["Elegy for the End"] = ++weaponCount,

            //["Dull Blade"] = ++weaponCount,
            //["Dull Blade"] = ++weaponCount,
            //["Dull Blade"] = ++weaponCount,
            //["Dull Blade"] = ++weaponCount,
            //["Dull Blade"] = ++weaponCount,

        };

        public static void AddTravelerToCharacterList(string traveler)
        {
            characterCode.Add(traveler, 1);
        }

        /// <summary> Use Tesseract OCR to find words on picture to string </summary>
        public static string AnalyzeText(Bitmap img)
        {
            string text = "";
            using (var ocr = new TesseractEngine("B:/Projects/VisualStudio/GenshinGuide/GenshinGuide/GenshinGuide/bin/Debug/tessdata", "eng", EngineMode.TesseractAndLstm))
            {
                var page = ocr.Process(img, PageSegMode.SingleLine);
                text = page.GetText();
            }
            return text;
        }

        public static string AnalyzeFewText(Bitmap img)
        {
            string text = "";
            using (var ocr = new TesseractEngine("B:/Projects/VisualStudio/GenshinGuide/GenshinGuide/GenshinGuide/bin/Debug/tessdata", "eng", EngineMode.TesseractOnly))
            {
                var page = ocr.Process(img, PageSegMode.SparseText);
                text = page.GetText();
            }
            return text;
        }

        public static int GetSetNameCode(string setName)
        {
            int code = -1;
            if (setNameCode.TryGetValue(setName, out code))
            {
                return code;
            }
            else
            {
                Debug.Print("Error: " + setName + " is not a valid Set Name");
                System.Environment.Exit(1);
                return code;
            };
        }

        public static int GetMainStatCode(string mainStat)
        {
            int code = -1;
            if (mainStatCode.TryGetValue(mainStat, out code))
            {
                return code;
            }
            else
            {
                Debug.Print("Error: " + mainStat + " is not a valid Main Stat Name");
                System.Environment.Exit(1);
                return code;
            };
        }

        public static int GetGearSlotCode(string gearSlot)
        {
            int code = -1;
            if (gearSlotCode.TryGetValue(gearSlot, out code))
            {
                return code;
            }
            else
            {
                Debug.Print("Error: " + gearSlot + " is not a valid Gear Slot");
                System.Environment.Exit(1);
                return code;
            };
        }

        public static int GetSubStatCode(string subStat)
        {
            int code = -1;
            if (subStatCode.TryGetValue(subStat, out code))
            {
                return code;
            }
            else
            {
                Debug.Print("Error: " + subStat + " is not a valid Sub Stat");
                System.Environment.Exit(1);
                return code;
            };
        }

        public static int GetCharacterCode(string character, bool bRedo = false)
        {
            int code = -1;
            if (characterCode.TryGetValue(character, out code))
            {
                return code;
            }
            else
            {
                Debug.Print("Error: " + character + " is not a valid Character Name");
                if(!bRedo)
                    System.Environment.Exit(1);
                return code;
            };
        }

        public static int GetElementalCode(string element, bool bRedo = false)
        {
            int code = -1;
            if (elementalCode.TryGetValue(element, out code))
            {
                return code;
            }
            else
            {
                Debug.Print("Error: " + element + " is not a valid Elemental Type");
                if(!bRedo)
                    System.Environment.Exit(1);
                return code;
            };
        }

        public static int GetWeaponCode(string weapon)
        {
            int code = -1;
            if (weaponCode.TryGetValue(weapon, out code))
            {
                return code;
            }
            else
            {
                Debug.Print("Error: " + weapon + " is not a valid Weapon Name");
                System.Environment.Exit(1);
                return code;
            };
        }

        public static void CreateJsonFile(GenshinData data)
        {
            // write to JSON file
            string JSONresult = JsonConvert.SerializeObject(data);
            string path = @"C:\Users\delaf\Downloads\genshinImpact_Data.json";

            if (File.Exists(path))
            {
                File.Delete(path);
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(JSONresult.ToString());
                    tw.Close();
                }
            }
            else if (!File.Exists(path))
            {
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(JSONresult.ToString());
                    tw.Close();
                }
            }
        }

        /// <summary> Take a screenshot to be used later with tesseract </summary>
        //public static Bitmap CaptureText()
        //{
        //    IntPtr handle = genshinImpact.MainWindowHandle;
        //    RECT position = new RECT();
        //    RECT area = new RECT();
        //    Bitmap bm = null;
        //    if (ClientToScreen(handle, ref position) && GetClientRect(handle, ref area))
        //    {
        //        bm = new Bitmap(area.right, area.bottom);
        //        Graphics g = Graphics.FromImage(bm);
        //        g.CopyFromScreen(position.left, position.top, 0, 0, bm.Size);
        //    }
        //    return bm;
        //}

        #region Image Operations
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static void SetGrayscale(ref Bitmap bitmap)
        {
            Bitmap temp = (Bitmap)bitmap;
            Bitmap bmap = (Bitmap)temp.Clone();
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);

                    bmap.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            bitmap = (Bitmap)bmap.Clone();
        }

        public static void SetContrast(double contrast, ref Bitmap bitmap)
        {
            Bitmap temp = (Bitmap)bitmap;
            Bitmap bmap = (Bitmap)temp.Clone();
            if (contrast < -100) contrast = -100;
            if (contrast > 100) contrast = 100;
            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    double pR = c.R / 255.0;
                    pR -= 0.5;
                    pR *= contrast;
                    pR += 0.5;
                    pR *= 255;
                    if (pR < 0) pR = 0;
                    if (pR > 255) pR = 255;

                    double pG = c.G / 255.0;
                    pG -= 0.5;
                    pG *= contrast;
                    pG += 0.5;
                    pG *= 255;
                    if (pG < 0) pG = 0;
                    if (pG > 255) pG = 255;

                    double pB = c.B / 255.0;
                    pB -= 0.5;
                    pB *= contrast;
                    pB += 0.5;
                    pB *= 255;
                    if (pB < 0) pB = 0;
                    if (pB > 255) pB = 255;

                    bmap.SetPixel(i, j,
        Color.FromArgb((byte)pR, (byte)pG, (byte)pB));
                }
            }
            bitmap = (Bitmap)bmap.Clone();
        }

        public static void SetInvert(ref Bitmap bitmap)
        {
            Bitmap temp = (Bitmap)bitmap;
            Bitmap bmap = (Bitmap)temp.Clone();
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    bmap.SetPixel(i, j,
      Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }
            bitmap = (Bitmap)bmap.Clone();
        }
        #endregion

 
    }
}
