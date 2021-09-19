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
#if DEBUG
    public static bool s_bDoDebugOnlyCode = false;
#endif
        // GLOBALS
        public static bool b_AssignedTravelerName = false;
        private static int setCount = 0;
        private static int mainStatCount = 0;
        private static int characterCount = 1;
        private static int weaponCount = 0;
        private static Dictionary<string, int> setNameCode = new Dictionary<string, int>
        {
            ["Adventurer"] = setCount,
            ["LuckyDog"] = ++setCount,
            ["TravelingDoctor"] = ++setCount,
            ["ResolutionofSojourner"] = ++setCount,
            ["TinyMiracle"] = ++setCount,
            ["Berserker"] = ++setCount, // 5
            ["Instructor"] = ++setCount,
            ["TheExile"] = ++setCount,
            ["DefendersWill"] = ++setCount,
            ["BraveHeart"] = ++setCount,
            ["MartialArtist"] = ++setCount, // 10
            ["Gambler"] = ++setCount,
            ["Scholar"] = ++setCount,
            ["PrayersforIllumination"] = ++setCount,
            ["PrayersforDestiny"] = ++setCount,
            ["PrayersforWisdom"] = ++setCount, // 15
            ["PrayersforSpringtime"] = ++setCount,
            ["Thundersoother"] = ++setCount,
            ["Lavawalker"] = ++setCount,
            ["MaidenBeloved"] = ++setCount,
            ["GladiatorsFinale"] = ++setCount, // 20
            ["ViridescentVenerer"] = ++setCount,
            ["WanderersTroupe"] = ++setCount,
            ["ThunderingFury"] = ++setCount,
            ["CrimsonWitchofFlames"] = ++setCount,
            ["NoblesseOblige"] = ++setCount, // 25
            ["BloodstainedChivalry"] = ++setCount,
            ["ArchaicPetra"] = ++setCount,
            ["RetracingBolide"] = ++setCount,
            ["HeartofDepth"] = ++setCount,
            ["BlizzardStrayer"] = ++setCount, // 30
            ["PaleFlame"] = ++setCount,
            ["TenacityoftheMillelith"] = ++setCount,
            ["ShimenawasReminiscence"] = ++setCount,
            ["EmblemofSeveredFate"] = ++setCount,
        };
        private static Dictionary<string, int> mainStatCode = new Dictionary<string, int>
        {
            ["HP_Flat"] = mainStatCount,
            ["ATK_Flat"] = ++mainStatCount,
            // These are %
            ["HP"] = ++mainStatCount,
            ["ATK"] = ++mainStatCount, 
            ["DEF"] = ++mainStatCount,
            ["EnergyRecharge"] = ++mainStatCount, //5
            ["ElementalMastery"] = ++mainStatCount,
            ["HealingBonus"] = ++mainStatCount,
            ["CRITRate"] = ++mainStatCount,
            ["CRITDMG"] = ++mainStatCount,
            ["PhysicalDMGBonus"] = ++mainStatCount, //10
            ["PyroDMGBonus"] = ++mainStatCount,
            ["ElectroDMGBonus"] = ++mainStatCount,
            ["CryoDMGBonus"] = ++mainStatCount,
            ["HydroDMGBonus"] = ++mainStatCount,
            ["AnemoDMGBonus"] = ++mainStatCount, //15
            ["GeoDMGBonus"] = ++mainStatCount,
        };
        private static Dictionary<string, int> gearSlotCode = new Dictionary<string, int>
        {
            ["FlowerofLife"] = 0,
            ["PlumeofDeath"] = 1,
            ["SandsofEon"] = 2,
            ["GobletofEonothem"] = 3,
            ["CircletofLogos"] = 4,
        };
        private static Dictionary<string, int> subStatCode = new Dictionary<string, int>
        {
            ["HP"] = 0,
            ["HP%"] = 1,
            ["ATK"] = 2,
            ["ATK%"] = 3,
            ["DEF"] = 4,
            ["DEF%"] = 5,
            ["EnergyRecharge"] = 6,
            ["ElementalMastery"] = 7,
            ["CRITRate"] = 8,
            ["CRITDMG"] = 9,
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
            ["HuTao"] = ++characterCount, // 30
            ["HuTao"] = characterCount, // 30
            ["Rosaria"] = ++characterCount,
            ["Yanfei"] = ++characterCount,
            ["Eula"] = ++characterCount,
            ["KaedeharaKazuha"] = ++characterCount,
            ["Kaedehara"] = characterCount,
            ["KamisatoAyaka"] = ++characterCount, //35
            ["Kamisato"] = characterCount, //35
            ["Yoimiya"] = ++characterCount,
            ["Sayu"] = ++characterCount,
            ["RaidenShogun"] = ++characterCount,
            ["Raiden"] = characterCount,
            ["KujouSara"] = ++characterCount,
            ["Kujou"] = characterCount,
            ["Aloy"] = ++characterCount, // 40
            ["SangonomiyaKokomi"] = ++characterCount,
            ["Sangonomiya"] = characterCount,
        };
        public static Dictionary<int, string[]> characterTalentConstellationOrder = new Dictionary<int, string[]>
        {
            /////////// Traveler is Assigned at runtime /////////////
            [1] = new string[]{ "burst", "skill" }, // Traveler
            ///////////////////////////////////////////////
            [2] = new string[]{ "burst", "skill" }, //Amber 
            [3] = new string[]{ "skill", "burst" }, //Kaeya 
            [4] = new string[]{ "burst", "skill" }, //Lisa 
            [5] = new string[]{ "burst", "skill" }, //Barbara  
            [6] = new string[]{ "burst", "skill" }, //Razor 
            [7] = new string[]{ "burst", "skill" }, //Xiangling 
            [8] = new string[]{ "skill", "burst" }, //Beidou 
            [9] = new string[]{ "burst", "skill" }, //Xingqiu 
            [10] = new string[]{ "burst", "skill" }, //Ningguang  
            [11] = new string[]{ "skill", "burst" }, //Fischl 
            [12] = new string[]{ "skill", "burst" }, //Bennett 
            [13] = new string[]{ "skill", "burst" }, //Noelle 
            [14] = new string[]{ "burst", "skill" }, //Chongyun 
            [15] = new string[]{ "skill", "burst" }, //Sucrose  
            [16] = new string[]{ "burst", "skill" }, //Jean 
            [17] = new string[]{ "skill", "burst" }, //Diluc 
            [18] = new string[]{ "burst", "skill" }, //Qiqi 
            [19] = new string[]{ "burst", "skill" }, //Mona 
            [20] = new string[]{ "burst", "skill" }, //Keqing  
            [21] = new string[]{ "burst", "skill" }, //Venti 
            [22] = new string[]{ "skill", "burst" }, //Klee 
            [23] = new string[]{ "burst", "skill" }, //Diona 
            [24] = new string[]{ "skill", "burst" }, //Tartaglia 
            [25] = new string[]{ "skill", "burst" }, //Xinyan  
            [26] = new string[]{ "skill", "burst" }, //Zhongli 
            [27] = new string[]{ "skill", "burst" }, //Albedo 
            [28] = new string[]{ "burst", "skill" }, //Ganyu 
            [29] = new string[]{ "skill", "burst" }, //Xiao 
            [30] = new string[]{ "skill", "burst" }, //Hu Tao  
            [31] = new string[]{ "skill", "burst" }, //Rosaria 
            [32] = new string[]{ "skill", "burst" }, //Yanfei 
            [33] = new string[]{ "burst", "skill" }, //Eula 
            [34] = new string[]{ "skill", "burst" }, //Kaedehara Kazuha 
            [35] = new string[]{ "burst", "skill" }, //Kamisato Ayaka  
            [36] = new string[]{ "skill", "burst" }, //Yoimiya 
            [37] = new string[]{ "burst", "skill" }, //Sayu 
            [38] = new string[]{ "burst", "skill" }, //Raiden Shogun 
            [39] = new string[]{ "burst", "skill" }, //Kujou Sara 
            [40] = new string[]{ "burst", "skill" }, //Aloy  Note: has no constellations
            [41] = new string[] { "burst", "skill" }, //Sangonomiya Kokomi 
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
            ["DullBlade"] = weaponCount,
            ["WasterGreatsword"] = ++weaponCount,
            ["BeginnersProtector"] = ++weaponCount,
            ["ApprenticesNotes"] = ++weaponCount,
            ["HuntersBow"] = ++weaponCount, // 4
            // 2 Stars
            ["SilverSword"] = ++weaponCount,
            ["OldMercsPal"] = ++weaponCount,
            ["IronPoint"] = ++weaponCount,
            ["PocketGrimoire"] = ++weaponCount,
            ["SeasonedHuntersBow"] = ++weaponCount, // 9
            // 3 Star
            ["CoolSteel"] = ++weaponCount, // 10
            ["HarbingerofDawn"] = ++weaponCount,
            ["TravelersHandySword"] = ++weaponCount,
            ["FilletBlade"] = ++weaponCount,
            ["SkyriderSword"] = ++weaponCount,
            ["FerrousShadow"] = ++weaponCount, // 15
            ["BloodtaintedGreatsword"] = ++weaponCount,
            ["WhiteIronGreatsword"] = ++weaponCount,
            ["DebateClub"] = ++weaponCount,
            ["SkyriderGreatsword"] = ++weaponCount,
            ["WhiteTassel"] = ++weaponCount, // 20
            ["Halberd"] = ++weaponCount,
            ["BlackTassel"] = ++weaponCount,
            ["MagicGuide"] = ++weaponCount,
            ["ThrillingTalesofDragonSlayers"] = ++weaponCount,
            ["OtherworldlyStory"] = ++weaponCount, // 25
            ["EmeraldOrb"] = ++weaponCount,
            ["TwinNephrite"] = ++weaponCount,
            ["RavenBow"] = ++weaponCount,
            ["SharpshootersOath"] = ++weaponCount,
            ["RecurveBow"] = ++weaponCount, // 30
            ["Slingshot"] = ++weaponCount,
            ["Messenger"] = ++weaponCount,
            // 4 star
            ["FavoniusSword"] = ++weaponCount,
            ["TheFlute"] = ++weaponCount,
            ["SacrificialSword"] = ++weaponCount, // 35
            ["RoyalLongsword"] = ++weaponCount,
            ["LionsRoar"] = ++weaponCount,
            ["PrototypeRancour"] = ++weaponCount,
            ["IronSting"] = ++weaponCount,
            ["BlackcliffLongsword"] = ++weaponCount, // 40
            ["TheBlackSword"] = ++weaponCount,
            ["SwordofDescension"] = ++weaponCount,
            ["FesteringDesire"] = ++weaponCount,
            ["TheAlleyFlash"] = ++weaponCount,
            ["FavoniusGreatsword"] = ++weaponCount, // 45
            ["TheBell"] = ++weaponCount,
            ["SacrificialGreatsword"] = ++weaponCount,
            ["RoyalGreatsword"] = ++weaponCount,
            ["Rainslasher"] = ++weaponCount,
            ["PrototypeArchaic"] = ++weaponCount, // 50
            ["Whiteblind"] = ++weaponCount,
            ["BlackcliffSlasher"] = ++weaponCount,
            ["SerpentSpine"] = ++weaponCount,
            ["SnowTombedStarsilver"] = ++weaponCount,
            ["LithicBlade"] = ++weaponCount, // 55
            ["DragonsBane"] = ++weaponCount,
            ["PrototypeStarglitter"] = ++weaponCount,
            ["CrescentPike"] = ++weaponCount,
            ["BlackcliffPole"] = ++weaponCount,
            ["Deathmatch"] = ++weaponCount, // 60
            ["FavoniusLance"] = ++weaponCount,
            ["RoyalSpear"] = ++weaponCount,
            ["DragonspineSpear"] = ++weaponCount,
            ["LithicSpear"] = ++weaponCount,
            ["FavoniusCodex"] = ++weaponCount, // 65
            ["TheWidsith"] = ++weaponCount,
            ["SacrificialFragments"] = ++weaponCount,
            ["RoyalGrimoire"] = ++weaponCount,
            ["SolarPearl"] = ++weaponCount,
            ["PrototypeAmber"] = ++weaponCount, // 70
            ["MappaMare"] = ++weaponCount,
            ["BlackcliffAgate"] = ++weaponCount,
            ["EyeofPerception"] = ++weaponCount,
            ["Frostbearer"] = ++weaponCount,
            ["WineandSong"] = ++weaponCount, // 75
            ["FavoniusWarbow"] = ++weaponCount,
            ["TheStringless"] = ++weaponCount,
            ["SacrificialBow"] = ++weaponCount,
            ["RoyalBow"] = ++weaponCount,
            ["Rust"] = ++weaponCount, // 80
            ["PrototypeCrescent"] = ++weaponCount,
            ["CompoundBow"] = ++weaponCount,
            ["BlackcliffWarbow"] = ++weaponCount,
            ["TheViridescent"] = ++weaponCount,
            ["AlleyHunter"] = ++weaponCount, // 85
            ["WindblumeOde"] = ++weaponCount,
            ["AquiliaFavonia"] = ++weaponCount,
            ["SkywardBlade"] = ++weaponCount,
            ["SummitShaper"] = ++weaponCount,
            ["PrimordialJadeCutter"] = ++weaponCount, // 90
            ["SkywardPride"] = ++weaponCount,
            ["WolfsGravestone"] = ++weaponCount,
            ["TheUnforged"] = ++weaponCount,
            ["PrimordialJadeWingedSpear"] = ++weaponCount,
            ["SkywardSpine"] = ++weaponCount, // 95
            ["VortextVanquisher"] = ++weaponCount,
            ["StaffofHoma"] = ++weaponCount,
            ["SkywardAtlas"] = ++weaponCount,
            ["LostPrayertotheSacredWinds"] = ++weaponCount,
            ["MemoryofDust"] = ++weaponCount, // 100
            ["SkywardHarp"] = ++weaponCount,
            ["AmosBow"] = ++weaponCount,
            ["ElegyfortheEnd"] = ++weaponCount,
            ["SongofBrokenPines"] = ++weaponCount,
            ["MitternachtsWaltz"] = ++weaponCount, // 105
            ["FreedomSworn"] = ++weaponCount,
            ["DodocoTales"] = ++weaponCount,
            // 2.0 Inazuma Patch
            ["AmenomaKageuchi"] = ++weaponCount,
            ["KatsuragikiriNagamasa"] = ++weaponCount,
            ["KitainCrossSpear"] = ++weaponCount, // 110
            ["Hamayumi"] = ++weaponCount,
            ["HakushinRing"] = ++weaponCount,
            ["MistsplitterReforged"] = ++weaponCount,
            ["ThunderingPulse"] = ++weaponCount,
            // 2.1
            ["Predator"] = ++weaponCount, // 115
            ["LuxuriousSeaLord"] = ++weaponCount,
            ["TheCatch"] = ++weaponCount,
            ["EngulfingLightning"] = ++weaponCount,
            ["EverlastingMoonglow"] = ++weaponCount,

        };
        private static Dictionary<string, int> enchancementMaterialCode = new Dictionary<string, int>
        {
            ["EnhancementOre"] = 1,
            ["FineEnhancementOre"] = 2,
            ["MysticEnchnacementOre"] = 3,
            ["SanctifyingUnction"] = 4,
            ["SanctifyingEssence"] = 5, // 4
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

        #region Get Dictionary Codes
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
                Form1.UnexpectedError(setName + " is not a valid Set Name");
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
                Form1.UnexpectedError(mainStat + " is not a valid Main Stat Name");
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
                Form1.UnexpectedError("gearSlot" + " is not a valid Gear Slot");
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
                Form1.UnexpectedError(subStat + " is not a valid Sub Stat");
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
            else if(Scraper.b_AssignedTravelerName == false)
            {
                return 0;
            }
            else
            {
                Debug.Print("Error: " + character + " is not a valid Character Name");
                if (!bRedo)
                    Form1.UnexpectedError(character + " is not a valid Character Name");
                return -1;
            }
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
                    Form1.UnexpectedError(element + " is not a valid Elemental Type");
                return code;
            };
        }

        public static int GetEnhancementMaterialCode(string enchancementMaterial)
        {
            int code = -1;
            if (enchancementMaterialCode.TryGetValue(enchancementMaterial, out code))
            {
                return code;
            }
            else
            {
                Debug.Print(enchancementMaterial + " is not a valid Enchancement Material");
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
                Form1.UnexpectedError(weapon + " is not a valid Weapon Name");
                return code;
            };
        }
        #endregion

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


            // Alter JSON file to have correct names for lock and auto keywords
            JSONresult = JSONresult.Replace("_lock", "lock");
            JSONresult = JSONresult.Replace("_auto", "auto");

            // Put file in Genshin Data directory
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // Create file with timestamp in name
            string fileName = "\\genshinData_GOOD_" + DateTime.Today.ToString("d") + ".json";
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

        public static void AssignTravelerName(string name)
        {
            string traveler = name;
            if (traveler != "")
            {
                if (traveler.Length > 7)
                {
                    traveler = traveler.Substring(0, 7);
                    Scraper.AddTravelerToCharacterList(traveler);
                }
                else
                {
                    Scraper.AddTravelerToCharacterList(traveler);
                }
            }
            else
            {
                Form1.UnexpectedError("Traveler name cannot be empty");
            }
        }


    }
}
