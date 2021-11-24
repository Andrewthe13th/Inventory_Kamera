using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Accord;
using Accord.Imaging.Filters;
using Tesseract;

namespace GenshinGuide
{
	public static class Scraper
	{
		private const int numEngines = 8;
#if DEBUG
		public static bool s_bDoDebugOnlyCode = false;
#endif
		private static readonly string tesseractDatapath = $"{Directory.GetCurrentDirectory()}\\tessdata";
		private static readonly string tesseractLanguage = "genshin_fast_09_04_21";

		// GLOBALS
		public static bool b_AssignedTravelerName = false;

		public static readonly List<string> setNames = new List<string>
		{
			"adventurer",
			"luckydog",
			"travelingdoctor",
			"resolutionofsojourner",
			"tinymiracle",
			"berserker",
			"instructor",
			"theexile",
			"defenderswill",
			"braveheart",
			"martialartist",
			"gambler",
			"scholar",
			"prayersforillumination",
			"prayersfordestiny",
			"prayersforwisdom",
			"prayerstospringtime",
			"thundersoother",
			"lavawalker",
			"maidenbeloved",
			"gladiatorsfinale",
			"viridescentvenerer",
			"wandererstroupe",
			"thunderingfury",
			"crimsonwitchofflames",
			"noblesseoblige",
			"bloodstainedchivalry",
			"archaicpetra",
			"retracingbolide",
			"heartofdepth",
			"blizzardstrayer",
			"paleflame",
			"tenacityofthemillelith",
			"shimenawasreminiscence",
			"emblemofseveredfate",
		};

		public static readonly List<string> stats = new List<string>
		{
			"hp", //0
			"hp%",
			"atk",
			"atk%", 
			"def", //4
			"def%",
			"energyrecharge", 
			"elementalmastery",
			"healingbonus", //8
			"critrate",
			"critdmg",
			"physicaldmgbonus",
			"pyrodmgbonus", //12
			"electrodmgbonus",
			"cryodmgbonus", 
			"hydrodmgbonus",
			"anemodmgbonus",//16
			"geodmgbonus",
		};

		public static readonly List<string> gearSlots = new List<string>
		{
			"floweroflife",
			"plumeofdeath",
			"sandsofeon",
			"gobletofeonothem",
			"circletoflogos",
		};

		public static readonly List<string> characters = new List<string>
		{
			"traveler",
			"amber",
			"kaeya",
			"lisa",
			"barbara",
			"razor",
			"xiangling",
			"beidou",
			"xingqiu",
			"ningguang",
			"fischl",
			"bennett",
			"noelle",
			"chongyun",
			"sucrose",
			"jean",
			"diluc",
			"qiqi",
			"mona",
			"keqing",
			"venti",
			"klee",
			"diona",
			"tartaglia",
			"xinyan",
			"zhongli",
			"albedo",
			"ganyu",
			"xiao",
			"hutao",
			"rosaria",
			"yanfei",
			"eula",
			"kaedeharakazuha",
			"kaedehara",
			"kamisatoayaka",
			"kamisato",
			"yoimiya",
			"sayu",
			"raidenshogun",
			"raiden",
			"kujousara",
			"kujou",
			"aloy",
			"sangonomiyakokomi",
			"sangonomiya",
			"thoma",
		};

		public static Dictionary<string, string[]> characterTalentConstellationOrder = new Dictionary<string, string[]>
		{
			/////////// Traveler is Assigned at runtime /////////////
			["amber"] = new string[] { "burst", "skill" }, //Amber
			["kaeya"] = new string[] { "skill", "burst" }, //Kaeya
			["lisa"] = new string[] { "burst", "skill" }, //Lisa
			["barbara"] = new string[] { "burst", "skill" }, //Barbara
			["razor"] = new string[] { "burst", "skill" }, //Razor
			["xiangling"] = new string[] { "burst", "skill" }, //Xiangling
			["beidou"] = new string[] { "skill", "burst" }, //Beidou
			["xingqiu"] = new string[] { "burst", "skill" }, //Xingqiu
			["ningguang"] = new string[] { "burst", "skill" }, //Ningguang
			["fischl"] = new string[] { "skill", "burst" }, //Fischl
			["bennett"] = new string[] { "skill", "burst" }, //Bennett
			["noelle"] = new string[] { "skill", "burst" }, //Noelle
			["chongyun"] = new string[] { "burst", "skill" }, //Chongyun
			["sucrose"] = new string[] { "skill", "burst" }, //Sucrose
			["jean"] = new string[] { "burst", "skill" }, //Jean
			["diluc"] = new string[] { "skill", "burst" }, //Diluc
			["qiqi"] = new string[] { "burst", "skill" }, //Qiqi
			["mona"] = new string[] { "burst", "skill" }, //Mona
			["keqing"] = new string[] { "burst", "skill" }, //Keqing
			["venti"] = new string[] { "burst", "skill" }, //Venti
			["klee"] = new string[] { "skill", "burst" }, //Klee
			["diona"] = new string[] { "burst", "skill" }, //Diona
			["tartaglia"] = new string[] { "skill", "burst" }, //Tartaglia
			["xinyan"] = new string[] { "skill", "burst" }, //Xinyan
			["zhongli"] = new string[] { "skill", "burst" }, //Zhongli
			["albedo"] = new string[] { "skill", "burst" }, //Albedo
			["ganyu"] = new string[] { "burst", "skill" }, //Ganyu
			["xiao"] = new string[] { "skill", "burst" }, //Xiao
			["hutao"] = new string[] { "skill", "burst" }, //Hu Tao
			["rosaria"] = new string[] { "skill", "burst" }, //Rosaria
			["yanfei"] = new string[] { "skill", "burst" }, //Yanfei
			["eula"] = new string[] { "burst", "skill" }, //Eula
			["kaedharakazuha"] = new string[] { "skill", "burst" }, //Kaedehara Kazuha
			["kaedhara"] = new string[] { "skill", "burst" }, //Kaedehara Kazuha
			["kamisatoayaka"] = new string[] { "burst", "skill" }, //Kamisato Ayaka
			["kamisato"] = new string[] { "burst", "skill" }, //Kamisato Ayaka
			["yoimiya"] = new string[] { "skill", "burst" }, //Yoimiya
			["sayu"] = new string[] { "burst", "skill" }, //Sayu
			["raidenshogun"] = new string[] { "burst", "skill" }, //Raiden Shogun
			["raiden"] = new string[] { "burst", "skill" }, //Raiden Shogun
			["kujousara"] = new string[] { "burst", "skill" }, //Kujou Sara
			["kujou"] = new string[] { "burst", "skill" }, //Kujou Sara
			["alloy"] = new string[] { "burst", "skill" }, //Aloy  Note: has no constellations
			["sangonomiyakokomi"] = new string[] { "burst", "skill" }, //Sangonomiya Kokomi
			["sangonomiya"] = new string[] { "burst", "skill" }, //Sangonomiya Kokomi
			["thoma"] = new string[] { "skill", "burst" }, //Thoma
		};

		public static readonly List<string> elements = new List<string>
		{
			"pyro",
			"hydro",
			"dendro",
			"electro",
			"anemo",
			"cryo",
			"geo",
		};

		public static readonly List<string> weapons = new List<string>
		{
			// Release Weapons
			// 1 star
			"dullblade",
			"wastergreatsword",
			"beginnersprotector",
			"apprenticesnotes",
			"huntersbow",

			// 2 stars
			"silversword",
			"oldmercspal",
			"ironpoint",
			"pocketgrimoire",
			"seasonedhuntersbow",

			// 3 star
			"coolsteel",
			"harbingerofdawn",
			"travelershandysword",
			"filletblade",
			"skyridersword",
			"ferrousshadow",
			"bloodtaintedgreatsword",
			"whiteirongreatsword",
			"debateclub",
			"skyridergreatsword",
			"whitetassel",
			"halberd",
			"blacktassel",
			"magicguide",
			"thrillingtalesofdragonslayers",
			"otherworldlystory",
			"emeraldorb",
			"twinnephrite",
			"ravenbow",
			"sharpshootersoath",
			"recurvebow",
			"slingshot",
			"messenger",

			// 4 star
			"favoniussword",
			"theflute",
			"sacrificialsword",
			"royallongsword",
			"lionsroar",
			"prototyperancour",
			"ironsting",
			"blackclifflongsword",
			"theblacksword",
			"swordofdescension",
			"festeringdesire",
			"thealleyflash",
			"favoniusgreatsword",
			"thebell",
			"sacrificialgreatsword",
			"royalgreatsword",
			"rainslasher",
			"prototypearchaic",
			"whiteblind",
			"blackcliffslasher",
			"serpentspine",
			"snowtombedstarsilver",
			"lithicblade",
			"dragonsbane",
			"prototypestarglitter",
			"crescentpike",
			"blackcliffpole",
			"deathmatch",
			"favoniuslance",
			"royalspear",
			"dragonspinespear",
			"lithicspear",
			"favoniuscodex",
			"thewidsith",
			"sacrificialfragments",
			"royalgrimoire",
			"solarpearl",
			"prototypeamber",
			"mappamare",
			"blackcliffagate",
			"eyeofperception",
			"frostbearer",
			"wineandsong",
			"favoniuswarbow",
			"thestringless",
			"sacrificialbow",
			"royalbow",
			"rust",
			"prototypecrescent",
			"compoundbow",
			"blackcliffwarbow",
			"theviridescenthunt",
			"alleyhunter",
			"windblumeode",
			"aquilafavonia",
			"skywardblade",
			"summitshaper",
			"primordialjadecutter",
			"skywardpride",
			"wolfsgravestone",
			"theunforged",
			"primordialjadewingedspear",
			"skywardspine",
			"vortexvanquisher",
			"staffofhoma",
			"skywardatlas",
			"lostprayertothesacredwinds",
			"memoryofdust",
			"skywardharp",
			"amosbow",
			"elegyfortheend",
			"songofbrokenpines",
			"mitternachtswaltz",
			"freedomsworn",
			"dodocotales",

			// 2.0 Inazuma Patch
			"amenomakageuchi",
			"katsuragikirinagamasa",
			"kitaincrossspear",
			"hamayumi",
			"hakushinring",
			"mistsplitterreforged",
			"thunderingpulse",

			// 2.1
			"predator",
			"luxurioussealord",
			"thecatch",
			"engulfinglightning",
			"everlastingmoonglow",
			"darkironsword",

			// 2.2
			"polarstar",
			"akuoumaru",
			"mouunsmoon",
			"wavebreakersfin",
		};

		public static readonly HashSet<string> characterDevelopmentItemsCode = new HashSet<string>
		{
			// character exp materials
			"heroswit",
			"adventurersexperience",
			"wanderersadvice",

			// character level-up materials
			"slimeconcentrate",
			"secretions",
			"slimecondensate",
			"ominousmask",
			"stainedmask",
			"damagedmask",
			"forbiddencursescroll",
			"sealedscroll",
			"diviningscroll",
			"weatheredarrowhead",
			"sharparrowhead",
			"firmarrowhead",
			"blackcrystalhorn",
			"blackbronzehorn",
			"heavyhorn",
			"leylinesprout",
			"deadleylineleaves",
			"deadleylinebranch",
			"chaoscore",
			"chaoscircuit",
			"chaosdevice",
			"mistgrasswick",
			"mistgrass",
			"mistgrasspollen",
			"inspectorssacrificialknife",
			"agentssacrificialknife",
			"hunterssacrificialknife",
			"lieutenantsinsignia",
			"sergeantsinsignia",
			"recruitsinsignia",
			"goldenraveninsignia",
			"silverraveninsignia",
			"treasurehoarderinsignia",
			"energynectar",
			"shimmeringnectar",
			"whopperflowernectar",
			"fossilizedboneshard",
			"sturdyboneshard",
			"fragileboneshard",
			"famedhandguard",
			"kageuchihandguard",
			"oldhandguard",
			"chaosoculus",
			"chaosaxis",
			"chaosgear",
			"polarizingprism",
			"crystalprism",
			"dismalprism",
			"spectralnucleus",
			"spectralheart",
			"spectralhusk",

			"dvalinsplume",
			"dvalinsclaw",
			"dvalinssigh",
			"tailofboreas",
			"ringofboreas",
			"spiritlocketofboreas",
			"tuskofmonoceroscaeli",
			"shardofafoullegacy",
			"shadowofthewarrior",
			"dragonlordscrown",
			"bloodjadebranch",
			"gildedscale",
			"moltenmoment",
			"hellfirebutterfly",
			"ashenheart",

			"everflameseed",
			"cleansinghearth",
			"lightningprism",
			"hoarfrostcore",
			"hurricaneseed",
			"basaltpillar",
			"juvenilejade",
			"crystallinebloom",
			"dreamsolvent",
			"marionettecore",
			"perpetualheart",
			"smolderingpearl",
			"dewofrepudiation",
			"stormbeads",

			// gemstones
			"agnidusagategemstone",
			"agnidusagatechunk",
			"agnidusagatefragment",
			"agnidusagatesliver",

			"varunadalazuritegemstone",
			"varunadalazuritechunk",
			"varunadalazuritefragment",
			"varunadalazuritesliver",

			"vajradaamethystgemstone",
			"vajradaamethystchunk",
			"vajradaamethystfragment",
			"vajradaamethystsliver",

			"vayudaturquoisefragmentgemstone",
			"vayudaturquoisefragmentchunk",
			"vayudaturquoisefragmentfragment",
			"vayudaturquoisefragmentsliver",

			"shivadajadegemstone",
			"shivadajadechunk",
			"shivadajadefragment",
			"shivadajadesliver",

			"prithivatopazgemstone",
			"prithivatopazchunk",
			"prithivatopazfragment",
			"prithivatopazsliver",

			// talent level-up materials
			"philosophiesoffreedom",
			"guidetofreedom",
			"teachingsoffreedom",
			"philosophiesofresistance",
			"guidetoresistance",
			"teachingsofresistance",
			"philosophiesofballad",
			"guidetoballad",
			"teachingsofballad",
			"philosophiesofprosperity",
			"guidetoprosperity",
			"teachingsofprosperity",
			"philosophiesofdiligence",
			"guidetodiligence",
			"teachingsofdiligence",
			"philosophiesofgold",
			"guidetogold",
			"teachingsofgold",
			"philosophiesoftransience",
			"guidetotransience",
			"teachingsoftransience",
			"philosophiesofelegance",
			"guidetoelegance",
			"teachingsofelegance",
			"philosophiesoflight",
			"guidetolight",
			"teachingsoflight",
			"crownofinsight",

			// weapon ascension materials
			"scatteredpieceofdecarabiansdream",
			"fragmentofdecarabiansepic",
			"debrisofdecarabianscity",
			"tileofdecarabianstower",

			"borealwolfsnostalgia",
			"borealwolfsbrokenfang",
			"borealwolfscrackedtooth",
			"borealwolfsmilktooth",

			"dreamofthedandeliongladiator",
			"shacklesofthedandeliongladiator",
			"chainsofthedandeliongladiator",
			"fettersofthedandeliongladiator",

			"divinebodyfromguyun",
			"relicfromguyun",
			"lustrousstonefromguyun",
			"luminoussandsfromguyun",

			"mistveiledprimoelixir",
			"mistveiledgoldelixir",
			"mistveiledmercuryelixir",
			"mistveiledleadelixir",

			"chunkofaerosiderite",
			"bitofaerosiderite",
			"pieceofaerosiderite",
			"grainofaerosiderite",

			"goldenbranchofadistantsea",
			"jadebranchofadistantsea",
			"jeweledbranchofadistantsea",
			"coralbranchofadistantsea",

			"narukamisvalor",
			"narukamisaffection",
			"narukamisjoy",
			"narukamiswisdom",

			"maskofthekijin",
			"maskoftheonehorned",
			"maskofthetigersbite",
			"maskofthewickedlieutenant",
		};

		public static readonly HashSet<string> materialsCode = new HashSet<string>
		{
			// material
			"strangetooth",
			"vitalizeddragontooth",
			"horsetail",
			"mistflowercorolla",
			"flamingflowerstamen",
			"electrocrystal",
			"butterflywings",
			"frog",
			"luminescentspine",
			"lizardtail",
			"crystalcore",
			"loachpearl",

			// furnishings
			"bluedye",
			"yellowdye",
			"reddye",
			"fabric",

			// wood
			"birch",
			"cuihua",
			"pine",
			"cedar",
			"firwood",
			"sandbearer",
			"bamboo",
			"firwood",
			"yumemiru",
			"maple",
			"otogi",
			"aralia",

			// forging ore
			"ironchunk",
			"whiteironchunk",
			"crystalchunk",
			"magicalcrystalchunk",
			"starsilver",
			"amethystlump",

			// billets
			"northlanderswordbillet",
			"northlanderbowbillet",
			"northlanderclaymorebillet",
			"northlandercatalystbillet",
			"northlanderpolearmbillet",

			// fishbait
			"fruitpastebait",
			"redrotbait",
			"falsewormbait",
			"fakeflybait",

			// fish
			"medaka",
			"glazemedaka",
			"sweetflowermedaka",
			"aizenmedake",
			"dawncatcher",
			"crystalfish",
			"lungedstickleback",
			"betta",
			"venomspinefish",
			"akaimaou",
			"snowstrider",
			"goldenkoi",
			"rustykoi",
			"brownshirakodai",
			"purpleshirakodai",
			"teacoloredshirakodai",
			"abidingangelfish",
			"raimeiangelfish",
			"pufferfish",
			"bitterpufferfish",

			// cooking ingredient
			"mushroom",
			"sweetflower",
			"carrot",
			"radish",
			"snapdragon",
			"mint",
			"wheat",
			"cabbage",
			"pinecone",
			"berry",
			"rawmeat",
			"birdegg",
			"matsutake",
			"fowl",
			"crab",
			"crabroe",
			"salt",
			"onion",
			"pepper",
			"milk",
			"tomato",
			"potato",
			"fish",
			"tofu",
			"almond",
			"bambooshoot",
			"rice",
			"shrimpmeat",
			"chilledmeat",
			"unagimeat",
			"seagrass",
			"lavendermelon",
			"flour",
			"cream",
			"smokedfowl",
			"butter",
			"ham",
			"sugar",
			"jam",
			"cheese",
			"bacon",
			"sausage",
			"lotushead",

			// local specialty
			"callalily",
			"wolfhook",
			"valberry",
			"cecilia",
			"windwheelaster",
			"philanemomushroom",
			"jueyunchili",
			"noctilucousjade",
			"silkflower",
			"glazelily",
			"qingxin",
			"starconch",
			"violetgrass",
			"smalllampgrass",
			"dandelionseed",
			"corlapis",
			"onikabuto",
			"sakurabloom",
			"crystalmarrow",
			"dendrobium",
			"nakuweed",
			"seaganoderma",
			"sangopearl",
			"amakumofruit",
		};

		public static readonly HashSet<string> enhancementMaterials = new HashSet<string>
		{
			"enhancementore",
			"fineenhancementore",
			"mysticenhancementore",
			"sanctifyingunction",
			"sanctifyingessence",
		};

		public static ConcurrentBag<TesseractEngine> engines = new ConcurrentBag<TesseractEngine>();

		static Scraper()
		{
			for (int i = 0; i < numEngines; i++)
			{
				engines.Add(new TesseractEngine(tesseractDatapath, tesseractLanguage, EngineMode.LstmOnly));
			}
		}

		public static void AddTravelerToCharacterList(string traveler)
		{
			if (!characters.Contains(traveler))
			{
				characters[0] = traveler;
				characterTalentConstellationOrder.Add(traveler, new string[] { "burst", "skill" });
			}
		}

		public static void AssignTravelerName(string traveler)
		{
			if (!string.IsNullOrEmpty(traveler))
			{
				AddTravelerToCharacterList(traveler);
				Debug.WriteLine($"Parsed traveler name {traveler}");
			}
			else
			{
				UserInterface.AddError("Could not parse Traveler's username");
			}
		}

		#region OCR

		public static void RestartEngines()
		{
			lock (engines)
			{
				while (!engines.IsEmpty)
				{
					if (engines.TryTake(out TesseractEngine e))
						e.Dispose();
				}

				for (int i = 0; i < numEngines; i++)
				{
					engines.Add(new TesseractEngine(tesseractDatapath, tesseractLanguage, EngineMode.LstmOnly));
				}
			}
		}

		/// <summary> Use Tesseract OCR to find words on picture to string </summary>
		public static string AnalyzeText(Bitmap bitmap, PageSegMode pageMode = PageSegMode.SingleLine, bool numbersOnly = false)
		{
			string text = "";
			TesseractEngine e;
			while (!engines.TryTake(out e)) { Thread.Sleep(10); }

			if (numbersOnly) e.SetVariable("tessedit_char_whitelist", "0123456789");
			using (var page = e.Process(bitmap, pageMode))
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
			engines.Add(e);

			return text;
		}

		public static string AnalyzeFewText(Bitmap img)
		{
			string text = "";
			using (var ocr = new TesseractEngine(tesseractDatapath, "eng", EngineMode.TesseractOnly))
			{
				var page = ocr.Process(img, PageSegMode.SparseText);
				ocr.SetVariable("tessedit_char_whitelist", "0123456789");
				text = page.GetText();
			}
			return text;
		}

		#endregion OCR

		#region Check valid parameters
		public static bool IsValidSetName(string setName)
		{
			if (setNames.Contains(setName))
			{
				return true;
			}
			else
			{
				Debug.WriteLine($"Error: {setName} is not a valid set name");
				UserInterface.AddError($"{setName} is not a valid set name");
				return false;
			};
		}

		

		public static bool IsValidStat(string stat)
		{
			if (string.IsNullOrEmpty(stat)) return true;
			if (stat.Contains("critr")) stat = "critrate"; // Fix for the 't' in 'rate' being read as an 'l'
			if (stat.Contains("mastery")) stat = "elementalmastery"; // Fix for the 't' in 'elemental' being read as an 'l'
			if (stats.Contains(stat))
			{
				return true;
			}
			else
			{
				Debug.WriteLine($"Error: {stat} is not a valid stat name");
				UserInterface.AddError($"{stat} is not a valid stat name");
				return false;
			};
		}

		public static bool IsValidSlot(string gearSlot)
		{
			if (gearSlots.Contains(gearSlot))
			{
				return true;
			}
			else
			{
				Debug.WriteLine($"Error: {gearSlot} is not a valid gear slot");
				UserInterface.AddError($"{gearSlot} is not a valid gear slot");
				return false;
			};
		}

		public static bool IsValidCharacter(string character)
		{
			if (characters.Contains(character))
			{
				return true;
			}
			else
			{
				Debug.WriteLine($"{character} is not a valid character name");
				UserInterface.AddError($"{character} is not a valid character name");
				return false;
			}
		}

		public static bool IsValidElement(string element, bool bRedo = false)
		{
			if (elements.Contains(element))
			{
				return true;
			}
			else
			{
				if (!bRedo)
				{
					Debug.Print($"Error: {element} is not a valid elemental type");
					Form1.UnexpectedError($"{element} is not a valid elemental type");
				}
				return false;
			};
		}

		public static bool IsEnhancementMaterial(string material)
		{
			return enhancementMaterials.Contains(material);
		}

		public static bool IsValidWeapon(string weapon)
		{
			if (weapons.Contains(weapon))
			{
				return true;
			}
			else
			{
				Debug.Print($"Error: {weapon} is not a valid weapon name");
				UserInterface.AddError($"{weapon} is not a valid weapon name");
				return false;
			};
		}

		#endregion Check valid parameters

		public static string FindElementByName(string name)
		{
			return FindClosestInList(name, elements);
		}

		public static string FindClosestWeapon(string name)
		{
			return FindClosestInList(name, weapons);
		}

		public static string FindClosestSetName(string name)
		{
			return FindClosestInList(name, setNames);
		}

		public static string FindClosestCharacterName(string name)
		{
			return FindClosestInList(name, characters);
		}

		private static string FindClosestInList(string source, List<string> targets)
		{
			if (targets.Contains(source)) return source;
			
			int index = int.MaxValue;
			int maxEdits = 15;
			for (int i = 0; i < targets.Count; i++)
			{
				string target = targets[i];
				int edits = CalcDistance(source, target, maxEdits);

				if (edits <= 1)
				{
					return target;
				}
				else if (edits < maxEdits)
				{
					index = i;
					maxEdits = edits;
				}
			}

			return index != int.MaxValue ? targets[index] : null;
		}

		// Adapted from https://stackoverflow.com/a/9454016/13205651
		private static int CalcDistance(string text, string setName, int maxEdits)
		{
			int length1 = text.Length;
			int length2 = setName.Length;

			// Return trivial case - difference in string lengths exceeds threshhold
			if (Math.Abs(length1 - length2) > maxEdits) { return int.MaxValue; }

			// Ensure arrays [i] / length1 use shorter length 
			if (length1 > length2)
			{
				Swap(ref setName, ref text);
				Swap(ref length1, ref length2);
			}

			int maxi = length1;
			int maxj = length2;

			int[] dCurrent = new int[maxi + 1];
			int[] dMinus1 = new int[maxi + 1];
			int[] dMinus2 = new int[maxi + 1];
			int[] dSwap;

			for (int i = 0; i <= maxi; i++) { dCurrent[i] = i; }

			int jm1 = 0, im1 = 0, im2 = -1;

			for (int j = 1; j <= maxj; j++)
			{

				// Rotate
				dSwap = dMinus2;
				dMinus2 = dMinus1;
				dMinus1 = dCurrent;
				dCurrent = dSwap;

				// Initialize
				int minDistance = int.MaxValue;
				dCurrent[0] = j;
				im1 = 0;
				im2 = -1;

				for (int i = 1; i <= maxi; i++)
				{

					int cost = text[im1] == setName[jm1] ? 0 : 1;

					int del = dCurrent[im1] + 1;
					int ins = dMinus1[i] + 1;
					int sub = dMinus1[im1] + cost;

					//Fastest execution for min value of 3 integers
					int min = (del > ins) ? (ins > sub ? sub : ins) : (del > sub ? sub : del);

					if (i > 1 && j > 1 && text[im2] == setName[jm1] && text[im1] == setName[j - 2])
						min = Math.Min(min, dMinus2[im2] + cost);

					dCurrent[i] = min;
					if (min < minDistance) { minDistance = min; }
					im1++;
					im2++;
				}
				jm1++;
				if (minDistance > maxEdits) { return int.MaxValue; }
			}

			int result = dCurrent[maxi];
			return ( result > maxEdits ) ? int.MaxValue : result;
		}
		private static void Swap<T>(ref T arg1, ref T arg2)
		{
			T temp = arg1;
			arg1 = arg2;
			arg2 = temp;
		}

		public static bool CompareColors(Color a, Color b)
		{
			int[] diff = new int[3];
			diff[0] = Math.Abs(a.R - b.R);
			diff[1] = Math.Abs(a.G - b.G);
			diff[2] = Math.Abs(a.B - b.B);

			return diff[0] < 10 && diff[1] < 10 && diff[2] < 10;
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

		public static Bitmap ConvertToGrayscale(Bitmap bitmap)
		{
			return new Grayscale(0.2125, 0.7154, 0.0721).Apply(bitmap);
		}

		public static void SetContrast(double contrast, ref Bitmap bitmap)
		{
			new ContrastCorrection((int)contrast).ApplyInPlace(bitmap);
		}

		public static void SetGamma(double red, double green, double blue, ref Bitmap bitmap)
		{
			Bitmap temp = bitmap;
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
		(int)( ( 255.0 * Math.Pow(i / 255.0, 1.0 / color) ) + 0.5 ));
			}
			return gammaArray;
		}

		public static void SetInvert(ref Bitmap bitmap)
		{
			new Invert().ApplyInPlace(bitmap);
		}

		public static void SetColor
			(string colorFilterType, ref Bitmap bitmap)
		{
			Bitmap temp = bitmap;
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
			Bitmap temp = bitmap;
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

		public static void SetThreshold(int threshold, ref Bitmap bitmap)
		{
			new Threshold(threshold).ApplyInPlace(bitmap);
		}

		public static void FilterColors(ref Bitmap bm, IntRange red, IntRange green, IntRange blue)
		{
			ColorFiltering colorFilter = new ColorFiltering
			{
				Red = red,
				Green = green,
				Blue = blue
			};
			colorFilter.ApplyInPlace(bm);
		}

		#endregion Image Operations
	}
}