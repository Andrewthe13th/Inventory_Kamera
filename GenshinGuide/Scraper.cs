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
            ["Pale Flame"] = ++setCount,
            ["Tenacity of the Millelith"] = ++setCount,
            ["Shimenawas Reminiscence"] = ++setCount,
            ["Emblem of Severed Fate"] = ++setCount,
        };
        private static Dictionary<string, int> mainStatCode = new Dictionary<string, int>
        {
            ["HP_Flat"] = mainStatCount,
            ["ATK_Flat"] = ++mainStatCount,
            // These are %
            ["HP"] = ++mainStatCount,
            ["ATK"] = ++mainStatCount, 
            ["DEF"] = ++mainStatCount,
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
            ["HP%"] = 1,
            ["ATK"] = 2,
            ["ATK%"] = 3,
            ["DEF"] = 4,
            ["DEF%"] = 5,
            ["Energy Recharge"] = 6,
            ["Elemental Mastery"] = 7,
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
            ["Ningguang"] = ++characterCount, // 10
            ["Fischl"] = ++characterCount,
            ["Bennett"] = ++characterCount,
            ["Noelle"] = ++characterCount,
            ["Chongyun"] = ++characterCount,
            ["Sucrose"] = ++characterCount, // 15
            ["Jean"] = ++characterCount,
            ["Diluc"] = ++characterCount,
            ["Qiqi"] = ++characterCount,
            ["Mona"] = ++characterCount,
            ["Keqing"] = ++characterCount, // 20
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
            ["HuTao"] = characterCount, // 30
            ["Rosaria"] = ++characterCount,
            ["Yanfei"] = ++characterCount,
            ["Eula"] = ++characterCount,
            ["Kaedehara Kazuha"] = ++characterCount,
            ["Kaedehara"] = characterCount,
            ["Kamisato Ayaka"] = ++characterCount, //35
            ["Kamisato"] = characterCount, //35
            ["Yoimiya"] = ++characterCount,
            ["Sayu"] = ++characterCount,
            ["Raiden Shogun"] = ++characterCount,
            ["Raiden"] = characterCount,
            ["Kujou Sara"] = ++characterCount,
            ["Kujou"] = characterCount,
            ["Aloy"] = ++characterCount, // 40
            ["Sangonomiya Kokomi"] = ++characterCount,
            ["Sangonomiya "] = characterCount,
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
            ["Hunters Bow"] = ++weaponCount, // 4
            // 2 Stars
            ["Silver Sword"] = ++weaponCount,
            ["Old Mercs Pal"] = ++weaponCount,
            ["Iron Point"] = ++weaponCount,
            ["Pocket Grimoire"] = ++weaponCount,
            ["Seasoned Hunters Bow"] = ++weaponCount, // 9
            // 3 Star
            ["Cool Steel"] = ++weaponCount, // 10
            ["Harbinger of Dawn"] = ++weaponCount,
            ["Travelers Handy Sword"] = ++weaponCount,
            ["Fillet Blade"] = ++weaponCount,
            ["Skyrider Sword"] = ++weaponCount,
            ["Ferrous Shadow"] = ++weaponCount, // 15
            ["Bloodtainted Greatsword"] = ++weaponCount,
            ["White Iron Greatsword"] = ++weaponCount,
            ["Debate Club"] = ++weaponCount,
            ["Skyrider Greatsword"] = ++weaponCount,
            ["White Tassel"] = ++weaponCount, // 20
            ["Halberd"] = ++weaponCount,
            ["Black Tassel"] = ++weaponCount,
            ["Magic Guide"] = ++weaponCount,
            ["Thrilling Tales of Dragon Slayers"] = ++weaponCount,
            ["Otherworldly Story"] = ++weaponCount, // 25
            ["Emerald Orb"] = ++weaponCount,
            ["Twin Nephrite"] = ++weaponCount,
            ["Raven Bow"] = ++weaponCount,
            ["Sharpshooters Oath"] = ++weaponCount,
            ["Recurve Bow"] = ++weaponCount, // 30
            ["Slingshot"] = ++weaponCount,
            ["Messenger"] = ++weaponCount,
            // 4 star
            ["Favonius Sword"] = ++weaponCount,
            ["The Flute"] = ++weaponCount,
            ["Sacrificial Sword"] = ++weaponCount, // 35
            ["Royal Longsword"] = ++weaponCount,
            ["Lions Roar"] = ++weaponCount,
            ["Prototype Rancour"] = ++weaponCount,
            ["Iron Sting"] = ++weaponCount,
            ["Blackcliff Longsword"] = ++weaponCount, // 40
            ["The Black Sword"] = ++weaponCount,
            ["Sword of Descension"] = ++weaponCount,
            ["Festering Desire"] = ++weaponCount,
            ["The Alley Flash"] = ++weaponCount,
            ["Favonius Greatsword"] = ++weaponCount, // 45
            ["The Bell"] = ++weaponCount,
            ["Sacrificial Greatsword"] = ++weaponCount,
            ["Royal Greatsword"] = ++weaponCount,
            ["Rainslasher"] = ++weaponCount,
            ["Prototype Archaic"] = ++weaponCount, // 50
            ["Whiteblind"] = ++weaponCount,
            ["Blackcliff Slasher"] = ++weaponCount,
            ["Serpent Spine"] = ++weaponCount,
            ["SnowTombed Starsilver"] = ++weaponCount,
            ["Lithic Blade"] = ++weaponCount, // 55
            ["Dragons Bane"] = ++weaponCount,
            ["Prototype Starglitter"] = ++weaponCount,
            ["Crescent Pike"] = ++weaponCount,
            ["Blackcliff Pole"] = ++weaponCount,
            ["Deathmatch"] = ++weaponCount, // 60
            ["Favonius Lance"] = ++weaponCount,
            ["Royal Spear"] = ++weaponCount,
            ["Dragonspine Spear"] = ++weaponCount,
            ["Lithic Spear"] = ++weaponCount,
            ["Favonius Codex"] = ++weaponCount, // 65
            ["The Widsith"] = ++weaponCount,
            ["Sacrificial Fragments"] = ++weaponCount,
            ["Royal Grimoire"] = ++weaponCount,
            ["Solar Pearl"] = ++weaponCount,
            ["Prototype Amber"] = ++weaponCount, // 70
            ["Mappa Mare"] = ++weaponCount,
            ["Blackcliff Agate"] = ++weaponCount,
            ["Eye of Perception"] = ++weaponCount,
            ["Frostbearer"] = ++weaponCount,
            ["Wine and Song"] = ++weaponCount, // 75
            ["Favonius Warbow"] = ++weaponCount,
            ["The Stringless"] = ++weaponCount,
            ["Sacrificial Bow"] = ++weaponCount,
            ["Royal Bow"] = ++weaponCount,
            ["Rust"] = ++weaponCount, // 80
            ["Prototype Crescent"] = ++weaponCount,
            ["Compound Bow"] = ++weaponCount,
            ["Blackcliff Warbow"] = ++weaponCount,
            ["The Viridescent"] = ++weaponCount,
            ["Alley Hunter"] = ++weaponCount, // 85
            ["Windblume Ode"] = ++weaponCount,
            ["Aquilia Favonia"] = ++weaponCount,
            ["Skyward Blade"] = ++weaponCount,
            ["Summit Shaper"] = ++weaponCount,
            ["Primordial Jade Cutter"] = ++weaponCount, // 90
            ["Skyward Pride"] = ++weaponCount,
            ["Wolfs Gravestone"] = ++weaponCount,
            ["The Unforged"] = ++weaponCount,
            ["Primordial Jade WingedSpear"] = ++weaponCount,
            ["Skyward Spine"] = ++weaponCount, // 95
            ["Vortext Vanquisher"] = ++weaponCount,
            ["Staff of Homa"] = ++weaponCount,
            ["Skyward Atlas"] = ++weaponCount,
            ["Lost Prayer to the Sacred Winds"] = ++weaponCount,
            ["Memory of Dust"] = ++weaponCount, // 100
            ["Skyward Harp"] = ++weaponCount,
            ["Amos Bow"] = ++weaponCount,
            ["Elegy for the End"] = ++weaponCount,
            ["Song of Broken Pines"] = ++weaponCount,
            ["Mitternachts Waltz"] = ++weaponCount, // 105
            ["Freedom-Sworn"] = ++weaponCount,
            ["Dodoco Tales"] = ++weaponCount,
            // 2.0 Inazuma Patch
            ["Amenoma Kageuchi"] = ++weaponCount,
            ["Katsuragikiri Nagamasa"] = ++weaponCount,
            ["Kitain Cross Spear"] = ++weaponCount, // 110
            ["Hamayumi"] = ++weaponCount,
            ["Hakushin Ring"] = ++weaponCount,
            ["Mistsplitter Reforged"] = ++weaponCount,
            ["Thundering Pulse"] = ++weaponCount,
            // 2.1
            ["Predator"] = ++weaponCount, // 115
            ["Luxurious SeaLord"] = ++weaponCount,
            ["The Catch"] = ++weaponCount,
            ["Engulfing Lightning"] = ++weaponCount,
            ["Everlasting Moonglow"] = ++weaponCount,

        };

        private static TesseractEngine ocr_live = new TesseractEngine( (Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
        private static TesseractEngine ocr_1 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
        private static TesseractEngine ocr_2 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
        private static TesseractEngine ocr_3 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
        private static TesseractEngine ocr_4 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
        private static TesseractEngine ocr_5 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
        private static TesseractEngine ocr_6 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
        private static TesseractEngine ocr_7 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
        private static TesseractEngine ocr_8 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);

        //TODO: subStats Dictionaries


        public static void AddTravelerToCharacterList(string traveler)
        {
            if (!characterCode.ContainsKey(traveler))
            {
                characterCode.Add(traveler, 1);
            }
        }

        /// <summary> Use Tesseract OCR to find words on picture to string </summary>
        //public static string AnalyzeTextWithLiveTesseract(Bitmap img)
        //{
        //    string text = "";
        //    string dir = Directory.GetCurrentDirectory() + "\\tessdata";
        //    using (var ocr = new TesseractEngine(dir, "genshin_eng", EngineMode.LstmOnly))
        //    {
        //        var page = ocr.Process(img, PageSegMode.SingleLine);
        //        text = page.GetText();
        //    }
        //    return text;
        //}

        public static string AnalyzeText(Bitmap bitmap)
        {
            string text = "";
            
                using (var page = ocr_live.Process(bitmap, PageSegMode.SingleBlock))
                {
                    using (var iter = page.GetIterator())
                    {
                        iter.Begin();
                        do
                        {
                            text += iter.GetText(PageIteratorLevel.TextLine);
                        }
                        while (iter.Next(PageIteratorLevel.TextLine));
                    }
                }

            
            return text;
        }

        #region Multi threaded Options
        public static string AnalyzeText_1(Bitmap bitmap)
        {
            string text = "";

            using (var page = ocr_1.Process(bitmap, PageSegMode.SingleBlock))
            {
                using (var iter = page.GetIterator())
                {
                    iter.Begin();
                    do
                    {
                        text += iter.GetText(PageIteratorLevel.TextLine);
                    }
                    while (iter.Next(PageIteratorLevel.TextLine));
                }
            }


            return text;
        }
        public static string AnalyzeText_2(Bitmap bitmap)
        {
            string text = "";

            using (var page = ocr_2.Process(bitmap, PageSegMode.SingleBlock))
            {
                using (var iter = page.GetIterator())
                {
                    iter.Begin();
                    do
                    {
                        text += iter.GetText(PageIteratorLevel.TextLine);
                    }
                    while (iter.Next(PageIteratorLevel.TextLine));
                }
            }


            return text;
        }
        public static string AnalyzeText_3(Bitmap bitmap)
        {
            string text = "";

            using (var page = ocr_3.Process(bitmap, PageSegMode.SingleBlock))
            {
                using (var iter = page.GetIterator())
                {
                    iter.Begin();
                    do
                    {
                        text += iter.GetText(PageIteratorLevel.TextLine);
                    }
                    while (iter.Next(PageIteratorLevel.TextLine));
                }
            }


            return text;
        }
        public static string AnalyzeText_4(Bitmap bitmap)
        {
            string text = "";

            using (var page = ocr_4.Process(bitmap, PageSegMode.SingleBlock))
            {
                using (var iter = page.GetIterator())
                {
                    iter.Begin();
                    do
                    {
                        text += iter.GetText(PageIteratorLevel.TextLine);
                    }
                    while (iter.Next(PageIteratorLevel.TextLine));
                }
            }


            return text;
        }

        #endregion
        public static string AnalyzeText_Line(Bitmap bitmap)
        {
            string text = "";

            using (var page = ocr_live.Process(bitmap, PageSegMode.SingleLine))
            {
                using (var iter = page.GetIterator())
                {
                    iter.Begin();
                    do
                    {
                        text += iter.GetText(PageIteratorLevel.TextLine);
                    }
                    while (iter.Next(PageIteratorLevel.TextLine));
                }
            }


            return text;
        }

        #region Multi thread Substats Options
        public static string AnalyzeText_Line1(Bitmap bitmap)
        {
            string text = "";

            using (var page = ocr_5.Process(bitmap, PageSegMode.SingleLine))
            {
                using (var iter = page.GetIterator())
                {
                    iter.Begin();
                    do
                    {
                        text += iter.GetText(PageIteratorLevel.TextLine);
                    }
                    while (iter.Next(PageIteratorLevel.TextLine));
                }
            }


            return text;
        }

        public static string AnalyzeText_Line2(Bitmap bitmap)
        {
            string text = "";

            using (var page = ocr_6.Process(bitmap, PageSegMode.SingleLine))
            {
                using (var iter = page.GetIterator())
                {
                    iter.Begin();
                    do
                    {
                        text += iter.GetText(PageIteratorLevel.TextLine);
                    }
                    while (iter.Next(PageIteratorLevel.TextLine));
                }
            }


            return text;
        }

        public static string AnalyzeText_Line3(Bitmap bitmap)
        {
            string text = "";

            using (var page = ocr_7.Process(bitmap, PageSegMode.SingleLine))
            {
                using (var iter = page.GetIterator())
                {
                    iter.Begin();
                    do
                    {
                        text += iter.GetText(PageIteratorLevel.TextLine);
                    }
                    while (iter.Next(PageIteratorLevel.TextLine));
                }
            }


            return text;
        }
        public static string AnalyzeText_Line4(Bitmap bitmap)
        {
            string text = "";

            using (var page = ocr_8.Process(bitmap, PageSegMode.SingleLine))
            {
                using (var iter = page.GetIterator())
                {
                    iter.Begin();
                    do
                    {
                        text += iter.GetText(PageIteratorLevel.TextLine);
                    }
                    while (iter.Next(PageIteratorLevel.TextLine));
                }
            }


            return text;
        }
        #endregion

        public static string AnalyzeText_Sparse(Bitmap bitmap)
        {
            string text = "";

            using (var page = ocr_live.Process(bitmap, PageSegMode.SparseText))
            {
                using (var iter = page.GetIterator())
                {
                    iter.Begin();
                    do
                    {
                        text += iter.GetText(PageIteratorLevel.TextLine);
                    }
                    while (iter.Next(PageIteratorLevel.TextLine));
                }
            }


            return text;
        }

        public static string AnalyzeOneText(Bitmap bitmap)
        {
            string text = "";

            using (var page = ocr_live.Process(bitmap, PageSegMode.SingleWord))
            {
                using (var iter = page.GetIterator())
                {
                    iter.Begin();
                    do
                    {
                        text += iter.GetText(PageIteratorLevel.TextLine);
                    }
                    while (iter.Next(PageIteratorLevel.TextLine));
                }
            }


            return text;
        }

        public static string AnalyzeWeaponImage(Bitmap img)
        {
            string text = "";
            string dir = Directory.GetCurrentDirectory() + "\\tessdata";
            using (var ocr = new TesseractEngine(dir, "genshin_eng", EngineMode.LstmOnly))
            {
                var page = ocr.Process(img, PageSegMode.SingleBlock);
                text = page.GetText();
            }
            return text;
        }

        public static string AnalyzeElementAndCharName(Bitmap img)
        {
            string text = "";
            string dir = Directory.GetCurrentDirectory() + "\\tessdata";
            using (var ocr = new TesseractEngine(dir, "eng", EngineMode.Default,"ElementAndCharacterName"))
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
                ocr.SetVariable("tessedit_char_whitelist", "0123456789");
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
                return -1;
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
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path += "\\GenshinData";
            //string path = @"C:\Users\delaf\Downloads\genshinImpact_Data.json";

            if (!Directory.Exists(path))
            {
                // Make Directory
                Directory.CreateDirectory(path);
            }
            // Create file with timestamp in name
            string fileName = "\\genshinData_" + DateTime.Today.ToString("d") + ".json";
            fileName = fileName.Replace('/', '_');
            string filePath = path + fileName;

            // Override previous file if exists
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                using (var tw = new StreamWriter(filePath, true))
                {
                    tw.WriteLine(JSONresult.ToString());
                    tw.Close();
                }
            }
            else if (!File.Exists(filePath))
            {
                using (var tw = new StreamWriter(filePath, true))
                {
                    tw.WriteLine(JSONresult.ToString());
                    tw.Close();
                }
            }
        }

        public static void CreateJsonFile(GOOD data)
        {
            // write to JSON file
            string JSONresult = JsonConvert.SerializeObject(data);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path += "\\GenshinData";
            //string path = @"C:\Users\delaf\Downloads\genshinImpact_Data.json";

            if (!Directory.Exists(path))
            {
                // Make Directory
                Directory.CreateDirectory(path);
            }
            // Create file with timestamp in name
            string fileName = "\\genshinData_" + DateTime.Today.ToString("d") + ".json";
            fileName = fileName.Replace('/', '_');
            string filePath = path + fileName;

            // Override previous file if exists
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                using (var tw = new StreamWriter(filePath, true))
                {
                    tw.WriteLine(JSONresult.ToString());
                    tw.Close();
                }
            }
            else if (!File.Exists(filePath))
            {
                using (var tw = new StreamWriter(filePath, true))
                {
                    tw.WriteLine(JSONresult.ToString());
                    tw.Close();
                }
            }
        }

        public static bool CompareColors(Color a, Color b)
        {
            int[] diff = new int[3];
            diff[0] = Math.Abs(a.R - b.R);
            diff[1] = Math.Abs(a.G - b.G);
            diff[2] = Math.Abs(a.B - b.B);

            if (diff[0] < 10 && diff[1] < 10 && diff[2] < 10)
            {
                return true;
            }

            return false;
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

        public static Bitmap SetGrayscale(Bitmap bitmap)
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
            return bitmap = (Bitmap)bmap.Clone();
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

        public static void SetGamma(double red, double green, double blue, ref Bitmap bitmap)
        {
            Bitmap temp = (Bitmap)bitmap;
            Bitmap bmap = (Bitmap)temp.Clone();
            Color c;
            byte[] redGamma = CreateGammaArray(red);
            byte[] greenGamma = CreateGammaArray(green);
            byte[] blueGamma = CreateGammaArray(blue);
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    bmap.SetPixel(i, j, Color.FromArgb(redGamma[c.R],
                       greenGamma[c.G], blueGamma[c.B]));
                }
            }
            bitmap = (Bitmap)bmap.Clone();
        }

        private static byte[] CreateGammaArray(double color)
        {
            byte[] gammaArray = new byte[256];
            for (int i = 0; i < 256; ++i)
            {
                gammaArray[i] = (byte)Math.Min(255,
        (int)((255.0 * Math.Pow(i / 255.0, 1.0 / color)) + 0.5));
            }
            return gammaArray;
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

        public static void SetColorFilter(string colorFilterType, ref Bitmap bitmap)
        {
            Bitmap temp = (Bitmap)bitmap;
            Bitmap bmap = (Bitmap)temp.Clone();
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    int nPixelR = 0;
                    int nPixelG = 0;
                    int nPixelB = 0;
                    if (colorFilterType == "red")
                    {
                        nPixelR = c.R;
                        nPixelG = c.G - 255;
                        nPixelB = c.B - 255;
                    }
                    else if (colorFilterType == "green")
                    {
                        nPixelR = c.R - 255;
                        nPixelG = c.G;
                        nPixelB = c.B - 255;
                    }
                    else if (colorFilterType == "blue")
                    {
                        nPixelR = c.R - 255;
                        nPixelG = c.G - 255;
                        nPixelB = c.B;
                    }
                    nPixelR = Math.Max(nPixelR, 0);
                    nPixelR = Math.Min(255, nPixelR);

                    nPixelG = Math.Max(nPixelG, 0);
                    nPixelG = Math.Min(255, nPixelG);

                    nPixelB = Math.Max(nPixelB, 0);
                    nPixelB = Math.Min(255, nPixelB);

                    bmap.SetPixel(i, j, Color.FromArgb((byte)nPixelR,
                      (byte)nPixelG, (byte)nPixelB));
                }
            }
            bitmap = (Bitmap)bmap.Clone();
        }

        public static void SetBrightness(int brightness, ref Bitmap bitmap)
        {
            Bitmap temp = (Bitmap)bitmap;
            Bitmap bmap = (Bitmap)temp.Clone();
            if (brightness < -255) brightness = -255;
            if (brightness > 255) brightness = 255;
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    int cR = c.R + brightness;
                    int cG = c.G + brightness;
                    int cB = c.B + brightness;

                    if (cR < 0) cR = 1;
                    if (cR > 255) cR = 255;

                    if (cG < 0) cG = 1;
                    if (cG > 255) cG = 255;

                    if (cB < 0) cB = 1;
                    if (cB > 255) cB = 255;

                    bmap.SetPixel(i, j,
        Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
                }
            }
            bitmap = (Bitmap)bmap.Clone();
        }
        #endregion




    }
}
