using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Newtonsoft.Json;
using Tesseract;

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
		private static int characterDevelopmentItemsCount = 0;
		private static int materialsCount = 0;
		private static readonly Dictionary<string, int> setNames = new Dictionary<string, int>
		{
			["adventurer"] = setCount,
			["luckydog"] = ++setCount,
			["travelingdoctor"] = ++setCount,
			["resolutionofsojourner"] = ++setCount,
			["tinymiracle"] = ++setCount,
			["berserker"] = ++setCount, // 5
            ["instructor"] = ++setCount,
			["theexile"] = ++setCount,
			["defenderswill"] = ++setCount,
			["braveheart"] = ++setCount,
			["martialartist"] = ++setCount, // 10
            ["gambler"] = ++setCount,
			["scholar"] = ++setCount,
			["prayersforillumination"] = ++setCount,
			["prayersfordestiny"] = ++setCount,
			["prayersforwisdom"] = ++setCount, // 15
            ["prayersthespringtime"] = ++setCount,
			["thundersoother"] = ++setCount,
			["lavawalker"] = ++setCount,
			["maidenbeloved"] = ++setCount,
			["gladiatorsfinale"] = ++setCount, // 20
            ["viridescentvenerer"] = ++setCount,
			["wandererstroupe"] = ++setCount,
			["thunderingfury"] = ++setCount,
			["crimsonwitchofflames"] = ++setCount,
			["noblesseoblige"] = ++setCount, // 25
            ["bloodstainedchivalry"] = ++setCount,
			["archaicpetra"] = ++setCount,
			["retracingbolide"] = ++setCount,
			["heartofdepth"] = ++setCount,
			["blizzardstrayer"] = ++setCount, // 30
            ["paleflame"] = ++setCount,
			["tenacityofthemillelith"] = ++setCount,
			["shimenawasreminiscence"] = ++setCount,
			["emblemofseveredfate"] = ++setCount,
		};
		private static readonly Dictionary<string, int> mainStats = new Dictionary<string, int>
		{
			["hp_flat"] = mainStatCount,
			["atk_flat"] = ++mainStatCount,
            // these are %
            ["hp"] = ++mainStatCount,
			["atk"] = ++mainStatCount,
			["def"] = ++mainStatCount,
			["energyrecharge"] = ++mainStatCount, //5
            ["elementalmastery"] = ++mainStatCount,
			["healingbonus"] = ++mainStatCount,
			["critrate"] = ++mainStatCount,
			["critdmg"] = ++mainStatCount,
			["physicaldmgbonus"] = ++mainStatCount, //10
            ["pyrodmgbonus"] = ++mainStatCount,
			["electrodmgbonus"] = ++mainStatCount,
			["cryodmgbonus"] = ++mainStatCount,
			["hydrodmgbonus"] = ++mainStatCount,
			["anemodmgbonus"] = ++mainStatCount, //15
            ["geodmgbonus"] = ++mainStatCount,
		};
		private static readonly Dictionary<string, int> gearSlots = new Dictionary<string, int>
		{
			["floweroflife"] = 0,
			["plumeofdeath"] = 1,
			["sandsofeon"] = 2,
			["gobletofeonothem"] = 3,
			["circletoflogos"] = 4,
		};
		private static readonly Dictionary<string, int> subStats = new Dictionary<string, int>
		{
			["hp"] = 0,
			["hp%"] = 1,
			["atk"] = 2,
			["atk%"] = 3,
			["def"] = 4,
			["def%"] = 5,
			["energyrecharge"] = 6,
			["elementalmastery"] = 7,
			["critrate"] = 8,
			["critdmg"] = 9,
		};
		private static readonly Dictionary<string, int> characters = new Dictionary<string, int>
		{
			[""] = -1,
			["amber"] = ++characterCount,
			["kaeya"] = ++characterCount,
			["lisa"] = ++characterCount,
			["barbara"] = ++characterCount, //5
            ["razor"] = ++characterCount,
			["xiangling"] = ++characterCount,
			["beidou"] = ++characterCount,
			["xingqiu"] = ++characterCount,
			["ningguang"] = ++characterCount, // 10
            ["fischl"] = ++characterCount,
			["bennett"] = ++characterCount,
			["noelle"] = ++characterCount,
			["chongyun"] = ++characterCount,
			["sucrose"] = ++characterCount, // 15
            ["jean"] = ++characterCount,
			["diluc"] = ++characterCount,
			["qiqi"] = ++characterCount,
			["mona"] = ++characterCount,
			["keqing"] = ++characterCount, // 20
            ["venti"] = ++characterCount,
			["klee"] = ++characterCount,
			["diona"] = ++characterCount,
			["tartaglia"] = ++characterCount,
			["xinyan"] = ++characterCount, // 25
            ["zhongli"] = ++characterCount,
			["albedo"] = ++characterCount,
			["ganyu"] = ++characterCount,
			["xiao"] = ++characterCount,
			["hutao"] = ++characterCount, // 30
            ["hutao"] = characterCount, // 30
            ["rosaria"] = ++characterCount,
			["yanfei"] = ++characterCount,
			["eula"] = ++characterCount,
			["kaedeharakazuha"] = ++characterCount,
			["kaedehara"] = characterCount,
			["kamisatoayaka"] = ++characterCount, //35
            ["kamisato"] = characterCount, //35
            ["yoimiya"] = ++characterCount,
			["sayu"] = ++characterCount,
			["raidenshogun"] = ++characterCount,
			["raiden"] = characterCount,
			["kujousara"] = ++characterCount,
			["kujou"] = characterCount,
			["aloy"] = ++characterCount, // 40
            ["sangonomiyakokomi"] = ++characterCount,
			["sangonomiya"] = characterCount,
			["thoma"] = ++characterCount,
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
            [41] = new string[]{ "burst", "skill" }, //Sangonomiya Kokomi 
            [42] = new string[]{ "skill", "burst" }, //Thoma 
        };
		private static readonly Dictionary<string, int> elements = new Dictionary<string, int>
		{
			["pyro"] = 0,
			["hydro"] = 1,
			["dendro"] = 2,
			["electro"] = 3,
			["anemo"] = 4,
			["cryo"] = 5,
			["geo"] = 6,
		};
		private static readonly List<string> elementList = new List<string>
		{ 
			"pyro",
			"hydro",
			"dendro",
			"electro",
			"anemo",
			"cryo",
			"geo" 
		};
		private static readonly Dictionary<string, int> weapons = new Dictionary<string, int>
		{
			// Release Weapons
            // 1 star
            ["dullblade"] = weaponCount,
			["wastergreatsword"] = ++weaponCount,
			["beginnersprotector"] = ++weaponCount,
			["apprenticesnotes"] = ++weaponCount,
			["huntersbow"] = ++weaponCount, // 4
            // 2 stars
            ["silversword"] = ++weaponCount,
			["oldmercspal"] = ++weaponCount,
			["ironpoint"] = ++weaponCount,
			["pocketgrimoire"] = ++weaponCount,
			["seasonedhuntersbow"] = ++weaponCount, // 9
            // 3 star
            ["coolsteel"] = ++weaponCount, // 10
            ["harbingerofdawn"] = ++weaponCount,
			["travelershandysword"] = ++weaponCount,
			["filletblade"] = ++weaponCount,
			["skyridersword"] = ++weaponCount,
			["ferrousshadow"] = ++weaponCount, // 15
            ["bloodtaintedgreatsword"] = ++weaponCount,
			["whiteirongreatsword"] = ++weaponCount,
			["debateclub"] = ++weaponCount,
			["skyridergreatsword"] = ++weaponCount,
			["whitetassel"] = ++weaponCount, // 20
            ["halberd"] = ++weaponCount,
			["blacktassel"] = ++weaponCount,
			["magicguide"] = ++weaponCount,
			["thrillingtalesofdragonslayers"] = ++weaponCount,
			["otherworldlystory"] = ++weaponCount, // 25
            ["emeraldorb"] = ++weaponCount,
			["twinnephrite"] = ++weaponCount,
			["ravenbow"] = ++weaponCount,
			["sharpshootersoath"] = ++weaponCount,
			["recurvebow"] = ++weaponCount, // 30
            ["slingshot"] = ++weaponCount,
			["messenger"] = ++weaponCount,
            // 4 star
            ["favoniussword"] = ++weaponCount,
			["theflute"] = ++weaponCount,
			["sacrificialsword"] = ++weaponCount, // 35
            ["royallongsword"] = ++weaponCount,
			["lionsroar"] = ++weaponCount,
			["prototyperancour"] = ++weaponCount,
			["ironsting"] = ++weaponCount,
			["blackclifflongsword"] = ++weaponCount, // 40
            ["theblacksword"] = ++weaponCount,
			["swordofdescension"] = ++weaponCount,
			["festeringdesire"] = ++weaponCount,
			["thealleyflash"] = ++weaponCount,
			["favoniusgreatsword"] = ++weaponCount, // 45
            ["thebell"] = ++weaponCount,
			["sacrificialgreatsword"] = ++weaponCount,
			["royalgreatsword"] = ++weaponCount,
			["rainslasher"] = ++weaponCount,
			["prototypearchaic"] = ++weaponCount, // 50
            ["whiteblind"] = ++weaponCount,
			["blackcliffslasher"] = ++weaponCount,
			["serpentspine"] = ++weaponCount,
			["snowtombedstarsilver"] = ++weaponCount,
			["lithicblade"] = ++weaponCount, // 55
            ["dragonsbane"] = ++weaponCount,
			["prototypestarglitter"] = ++weaponCount,
			["crescentpike"] = ++weaponCount,
			["blackcliffpole"] = ++weaponCount,
			["deathmatch"] = ++weaponCount, // 60
            ["favoniuslance"] = ++weaponCount,
			["royalspear"] = ++weaponCount,
			["dragonspinespear"] = ++weaponCount,
			["lithicspear"] = ++weaponCount,
			["favoniuscodex"] = ++weaponCount, // 65
            ["thewidsith"] = ++weaponCount,
			["sacrificialfragments"] = ++weaponCount,
			["royalgrimoire"] = ++weaponCount,
			["solarpearl"] = ++weaponCount,
			["prototypeamber"] = ++weaponCount, // 70
            ["mappamare"] = ++weaponCount,
			["blackcliffagate"] = ++weaponCount,
			["eyeofperception"] = ++weaponCount,
			["frostbearer"] = ++weaponCount,
			["wineandsong"] = ++weaponCount, // 75
            ["favoniuswarbow"] = ++weaponCount,
			["thestringless"] = ++weaponCount,
			["sacrificialbow"] = ++weaponCount,
			["royalbow"] = ++weaponCount,
			["rust"] = ++weaponCount, // 80
            ["prototypecrescent"] = ++weaponCount,
			["compoundbow"] = ++weaponCount,
			["blackcliffwarbow"] = ++weaponCount,
			["theviridescenthunt"] = ++weaponCount,
			["alleyhunter"] = ++weaponCount, // 85
            ["windblumeode"] = ++weaponCount,
			["aquilafavonia"] = ++weaponCount,
			["skywardblade"] = ++weaponCount,
			["summitshaper"] = ++weaponCount,
			["primordialjadecutter"] = ++weaponCount, // 90
            ["skywardpride"] = ++weaponCount,
			["wolfsgravestone"] = ++weaponCount,
			["theunforged"] = ++weaponCount,
			["primordialjadewingedspear"] = ++weaponCount,
			["skywardspine"] = ++weaponCount, // 95
            ["vortexvanquisher"] = ++weaponCount,
			["staffofhoma"] = ++weaponCount,
			["skywardatlas"] = ++weaponCount,
			["lostprayertothesacredwinds"] = ++weaponCount,
			["memoryofdust"] = ++weaponCount, // 100
            ["skywardharp"] = ++weaponCount,
			["amosbow"] = ++weaponCount,
			["elegyfortheend"] = ++weaponCount,
			["songofbrokenpines"] = ++weaponCount,
			["mitternachtswaltz"] = ++weaponCount, // 105
            ["freedomsworn"] = ++weaponCount,
			["dodocotales"] = ++weaponCount,
            // 2.0 inazuma patch
            ["amenomakageuchi"] = ++weaponCount,
			["katsuragikirinagamasa"] = ++weaponCount,
			["kitaincrossspear"] = ++weaponCount, // 110
            ["hamayumi"] = ++weaponCount,
			["hakushinring"] = ++weaponCount,
			["mistsplitterreforged"] = ++weaponCount,
			["thunderingpulse"] = ++weaponCount,
            // 2.1
            ["predator"] = ++weaponCount, // 115
            ["luxurioussealord"] = ++weaponCount,
			["thecatch"] = ++weaponCount,
			["engulfinglightning"] = ++weaponCount,
			["everlastingmoonglow"] = ++weaponCount,
			["darkironsword"] = ++weaponCount, // 120
            // 2.2
            ["polarstar"] = ++weaponCount,
			["akuoumaru"] = ++weaponCount,
			["mouunsmoon"] = ++weaponCount,
			["wavebreakersfin"] = ++weaponCount,
		};
		private static readonly Dictionary<string, int> characterDevelopmentItemsCode = new Dictionary<string, int>
		{
            // character exp materials
            ["heroswit"] = characterDevelopmentItemsCount++,
			["adventurersexperience"] = characterDevelopmentItemsCount++,
			["wanderersadvice"] = characterDevelopmentItemsCount++,

            // character level-up materials
            ["slimeconcentrate"] = characterDevelopmentItemsCount++,
			["secretions"] = characterDevelopmentItemsCount++,
			["slimecondensate"] = characterDevelopmentItemsCount++,
			["ominousmask"] = characterDevelopmentItemsCount++,
			["stainedmask"] = characterDevelopmentItemsCount++,
			["damagedmask"] = characterDevelopmentItemsCount++,
			["forbiddencursescroll"] = characterDevelopmentItemsCount++,
			["sealedscroll"] = characterDevelopmentItemsCount++,
			["diviningscroll"] = characterDevelopmentItemsCount++,
			["weatheredarrowhead"] = characterDevelopmentItemsCount++,
			["sharparrowhead"] = characterDevelopmentItemsCount++,
			["firmarrowhead"] = characterDevelopmentItemsCount++,
			["blackcrystalhorn"] = characterDevelopmentItemsCount++,
			["blackbronzehorn"] = characterDevelopmentItemsCount++,
			["heavyhorn"] = characterDevelopmentItemsCount++,
			["leylinesprout"] = characterDevelopmentItemsCount++,
			["deadleylineleaves"] = characterDevelopmentItemsCount++,
			["deadleylinebranch"] = characterDevelopmentItemsCount++,
			["chaoscore"] = characterDevelopmentItemsCount++,
			["chaoscircuit"] = characterDevelopmentItemsCount++,
			["chaosdevice"] = characterDevelopmentItemsCount++,
			["mistgrasswick"] = characterDevelopmentItemsCount++,
			["mistgrass"] = characterDevelopmentItemsCount++,
			["mistgrasspollen"] = characterDevelopmentItemsCount++,
			["inspectorssacrificialknife"] = characterDevelopmentItemsCount++,
			["agentssacrificialknife"] = characterDevelopmentItemsCount++,
			["hunterssacrificialknife"] = characterDevelopmentItemsCount++,
			["lieutenantsinsignia"] = characterDevelopmentItemsCount++,
			["sergeantsinsignia"] = characterDevelopmentItemsCount++,
			["recruitsinsignia"] = characterDevelopmentItemsCount++,
			["goldenraveninsignia"] = characterDevelopmentItemsCount++,
			["silverraveninsignia"] = characterDevelopmentItemsCount++,
			["treasurehoarderinsignia"] = characterDevelopmentItemsCount++,
			["energynectar"] = characterDevelopmentItemsCount++,
			["shimmeringnectar"] = characterDevelopmentItemsCount++,
			["whopperflowernectar"] = characterDevelopmentItemsCount++,
			["fossilizedboneshard"] = characterDevelopmentItemsCount++,
			["sturdyboneshard"] = characterDevelopmentItemsCount++,
			["fragileboneshard"] = characterDevelopmentItemsCount++,
			["famedhandguard"] = characterDevelopmentItemsCount++,
			["kageuchihandguard"] = characterDevelopmentItemsCount++,
			["oldhandguard"] = characterDevelopmentItemsCount++,
			["chaosoculus"] = characterDevelopmentItemsCount++,
			["chaosaxis"] = characterDevelopmentItemsCount++,
			["chaosgear"] = characterDevelopmentItemsCount++,
			["polarizingprism"] = characterDevelopmentItemsCount++,
			["crystalprism"] = characterDevelopmentItemsCount++,
			["dismalprism"] = characterDevelopmentItemsCount++,
			["spectralnucleus"] = characterDevelopmentItemsCount++,
			["spectralheart"] = characterDevelopmentItemsCount++,
			["spectralhusk"] = characterDevelopmentItemsCount++,

			["dvalinsplume"] = characterDevelopmentItemsCount++,
			["dvalinsclaw"] = characterDevelopmentItemsCount++,
			["dvalinssigh"] = characterDevelopmentItemsCount++,
			["tailofboreas"] = characterDevelopmentItemsCount++,
			["ringofboreas"] = characterDevelopmentItemsCount++,
			["spiritlocketofboreas"] = characterDevelopmentItemsCount++,
			["tuskofmonoceroscaeli"] = characterDevelopmentItemsCount++,
			["shardofafoullegacy"] = characterDevelopmentItemsCount++,
			["shadowofthewarrior"] = characterDevelopmentItemsCount++,
			["dragonlordscrown"] = characterDevelopmentItemsCount++,
			["bloodjadebranch"] = characterDevelopmentItemsCount++,
			["gildedscale"] = characterDevelopmentItemsCount++,
			["moltenmoment"] = characterDevelopmentItemsCount++,
			["hellfirebutterfly"] = characterDevelopmentItemsCount++,
			["ashenheart"] = characterDevelopmentItemsCount++,

			["everflameseed"] = characterDevelopmentItemsCount++,
			["cleansinghearth"] = characterDevelopmentItemsCount++,
			["lightningprism"] = characterDevelopmentItemsCount++,
			["hoarfrostcore"] = characterDevelopmentItemsCount++,
			["hurricaneseed"] = characterDevelopmentItemsCount++,
			["basaltpillar"] = characterDevelopmentItemsCount++,
			["juvenilejade"] = characterDevelopmentItemsCount++,
			["crystallinebloom"] = characterDevelopmentItemsCount++,
			["dreamsolvent"] = characterDevelopmentItemsCount++,
			["marionettecore"] = characterDevelopmentItemsCount++,
			["perpetualheart"] = characterDevelopmentItemsCount++,
			["smolderingpearl"] = characterDevelopmentItemsCount++,
			["dewofrepudiation"] = characterDevelopmentItemsCount++,
			["stormbeads"] = characterDevelopmentItemsCount++,
            // gemstones
            ["agnidusagategemstone"] = characterDevelopmentItemsCount++,
			["agnidusagatechunk"] = characterDevelopmentItemsCount++,
			["agnidusagatefragment"] = characterDevelopmentItemsCount++,
			["agnidusagatesliver"] = characterDevelopmentItemsCount++,

			["varunadalazuritegemstone"] = characterDevelopmentItemsCount++,
			["varunadalazuritechunk"] = characterDevelopmentItemsCount++,
			["varunadalazuritefragment"] = characterDevelopmentItemsCount++,
			["varunadalazuritesliver"] = characterDevelopmentItemsCount++,

			["vajradaamethystgemstone"] = characterDevelopmentItemsCount++,
			["vajradaamethystchunk"] = characterDevelopmentItemsCount++,
			["vajradaamethystfragment"] = characterDevelopmentItemsCount++,
			["vajradaamethystsliver"] = characterDevelopmentItemsCount++,

			["vayudaturquoisefragmentgemstone"] = characterDevelopmentItemsCount++,
			["vayudaturquoisefragmentchunk"] = characterDevelopmentItemsCount++,
			["vayudaturquoisefragmentfragment"] = characterDevelopmentItemsCount++,
			["vayudaturquoisefragmentsliver"] = characterDevelopmentItemsCount++,

			["shivadajadegemstone"] = characterDevelopmentItemsCount++,
			["shivadajadechunk"] = characterDevelopmentItemsCount++,
			["shivadajadefragment"] = characterDevelopmentItemsCount++,
			["shivadajadesliver"] = characterDevelopmentItemsCount++,

			["prithivatopazgemstone"] = characterDevelopmentItemsCount++,
			["prithivatopazchunk"] = characterDevelopmentItemsCount++,
			["prithivatopazfragment"] = characterDevelopmentItemsCount++,
			["prithivatopazsliver"] = characterDevelopmentItemsCount++,

            // talent level-up materials
            ["philosophiesoffreedom"] = characterDevelopmentItemsCount++,
			["guidetofreedom"] = characterDevelopmentItemsCount++,
			["teachingsoffreedom"] = characterDevelopmentItemsCount++,
			["philosophiesofresistance"] = characterDevelopmentItemsCount++,
			["guidetoresistance"] = characterDevelopmentItemsCount++,
			["teachingsofresistance"] = characterDevelopmentItemsCount++,
			["philosophiesofballad"] = characterDevelopmentItemsCount++,
			["guidetoballad"] = characterDevelopmentItemsCount++,
			["teachingsofballad"] = characterDevelopmentItemsCount++,
			["philosophiesofprosperity"] = characterDevelopmentItemsCount++,
			["guidetoprosperity"] = characterDevelopmentItemsCount++,
			["teachingsofprosperity"] = characterDevelopmentItemsCount++,
			["philosophiesofdiligence"] = characterDevelopmentItemsCount++,
			["guidetodiligence"] = characterDevelopmentItemsCount++,
			["teachingsofdiligence"] = characterDevelopmentItemsCount++,
			["philosophiesofgold"] = characterDevelopmentItemsCount++,
			["guidetogold"] = characterDevelopmentItemsCount++,
			["teachingsofgold"] = characterDevelopmentItemsCount++,
			["philosophiesoftransience"] = characterDevelopmentItemsCount++,
			["guidetotransience"] = characterDevelopmentItemsCount++,
			["teachingsoftransience"] = characterDevelopmentItemsCount++,
			["philosophiesofelegance"] = characterDevelopmentItemsCount++,
			["guidetoelegance"] = characterDevelopmentItemsCount++,
			["teachingsofelegance"] = characterDevelopmentItemsCount++,
			["philosophiesoflight"] = characterDevelopmentItemsCount++,
			["guidetolight"] = characterDevelopmentItemsCount++,
			["teachingsoflight"] = characterDevelopmentItemsCount++,
			["crownofinsight"] = characterDevelopmentItemsCount++,

            // weapon ascension materials
            ["scatteredpieceofdecarabiansdream"] = characterDevelopmentItemsCount++,
			["fragmentofdecarabiansepic"] = characterDevelopmentItemsCount++,
			["debrisofdecarabianscity"] = characterDevelopmentItemsCount++,
			["tileofdecarabianstower"] = characterDevelopmentItemsCount++,

			["borealwolfsnostalgia"] = characterDevelopmentItemsCount++,
			["borealwolfsbrokenfang"] = characterDevelopmentItemsCount++,
			["borealwolfscrackedtooth"] = characterDevelopmentItemsCount++,
			["borealwolfsmilktooth"] = characterDevelopmentItemsCount++,

			["dreamofthedandeliongladiator"] = characterDevelopmentItemsCount++,
			["shacklesofthedandeliongladiator"] = characterDevelopmentItemsCount++,
			["chainsofthedandeliongladiator"] = characterDevelopmentItemsCount++,
			["fettersofthedandeliongladiator"] = characterDevelopmentItemsCount++,

			["divinebodyfromguyun"] = characterDevelopmentItemsCount++,
			["relicfromguyun"] = characterDevelopmentItemsCount++,
			["lustrousstonefromguyun"] = characterDevelopmentItemsCount++,
			["luminoussandsfromguyun"] = characterDevelopmentItemsCount++,

			["mistveiledprimoelixir"] = characterDevelopmentItemsCount++,
			["mistveiledgoldelixir"] = characterDevelopmentItemsCount++,
			["mistveiledmercuryelixir"] = characterDevelopmentItemsCount++,
			["mistveiledleadelixir"] = characterDevelopmentItemsCount++,

			["chunkofaerosiderite"] = characterDevelopmentItemsCount++,
			["bitofaerosiderite"] = characterDevelopmentItemsCount++,
			["pieceofaerosiderite"] = characterDevelopmentItemsCount++,
			["grainofaerosiderite"] = characterDevelopmentItemsCount++,

			["goldenbranchofadistantsea"] = characterDevelopmentItemsCount++,
			["jadebranchofadistantsea"] = characterDevelopmentItemsCount++,
			["jeweledbranchofadistantsea"] = characterDevelopmentItemsCount++,
			["coralbranchofadistantsea"] = characterDevelopmentItemsCount++,

			["narukamisvalor"] = characterDevelopmentItemsCount++,
			["narukamisaffection"] = characterDevelopmentItemsCount++,
			["narukamisjoy"] = characterDevelopmentItemsCount++,
			["narukamiswisdom"] = characterDevelopmentItemsCount++,

			["maskofthekijin"] = characterDevelopmentItemsCount++,
			["maskoftheonehorned"] = characterDevelopmentItemsCount++,
			["maskofthetigersbite"] = characterDevelopmentItemsCount++,
			["maskofthewickedlieutenant"] = characterDevelopmentItemsCount++,
        };
		private static readonly Dictionary<string, int> materialsCode = new Dictionary<string, int>
		{
            // material
            ["strangetooth"] = materialsCount++,
			["vitalizeddragontooth"] = materialsCount++,
			["horsetail"] = materialsCount++,
			["mistflowercorolla"] = materialsCount++,
			["flamingflowerstamen"] = materialsCount++,
			["electrocrystal"] = materialsCount++,
			["butterflywings"] = materialsCount++,
			["frog"] = materialsCount++,
			["luminescentspine"] = materialsCount++,
			["lizardtail"] = materialsCount++,
			["crystalcore"] = materialsCount++,
			["loachpearl"] = materialsCount++,

            // furnishings
            ["bluedye"] = materialsCount++,
			["yellowdye"] = materialsCount++,
			["reddye"] = materialsCount++,
			["fabric"] = materialsCount++,

            // wood
            ["birch"] = materialsCount++,
			["cuihua"] = materialsCount++,
			["pine"] = materialsCount++,
			["cedar"] = materialsCount++,
			["firwood"] = materialsCount++,
			["sandbearer"] = materialsCount++,
			["bamboo"] = materialsCount++,
			["firwood"] = materialsCount++,
			["yumemiru"] = materialsCount++,
			["maple"] = materialsCount++,
			["otogi"] = materialsCount++,
			["aralia"] = materialsCount++,

            // forging ore
            ["ironchunk"] = materialsCount++,
			["whiteironchunk"] = materialsCount++,
			["crystalchunk"] = materialsCount++,
			["magicalcrystalchunk"] = materialsCount++,
			["starsilver"] = materialsCount++,
			["amethystlump"] = materialsCount++,

            // billets
            ["northlanderswordbillet"] = materialsCount++,
			["northlanderbowbillet"] = materialsCount++,
			["northlanderclaymorebillet"] = materialsCount++,
			["northlandercatalystbillet"] = materialsCount++,
			["northlanderpolearmbillet"] = materialsCount++,

            // fishbait
            ["fruitpastebait"] = materialsCount++,
			["redrotbait"] = materialsCount++,
			["falsewormbait"] = materialsCount++,
			["fakeflybait"] = materialsCount++,

            // fish
            ["medaka"] = materialsCount++,
			["glazemedaka"] = materialsCount++,
			["sweetflowermedaka"] = materialsCount++,
			["aizenmedake"] = materialsCount++,
			["dawncatcher"] = materialsCount++,
			["crystalfish"] = materialsCount++,
			["lungedstickleback"] = materialsCount++,
			["betta"] = materialsCount++,
			["venomspinefish"] = materialsCount++,
			["akaimaou"] = materialsCount++,
			["snowstrider"] = materialsCount++,
			["goldenkoi"] = materialsCount++,
			["rustykoi"] = materialsCount++,
			["brownshirakodai"] = materialsCount++,
			["purpleshirakodai"] = materialsCount++,
			["teacoloredshirakodai"] = materialsCount++,
			["abidingangelfish"] = materialsCount++,
			["raimeiangelfish"] = materialsCount++,
			["pufferfish"] = materialsCount++,
			["bitterpufferfish"] = materialsCount++,

            // cooking ingredient
            ["mushroom"] = materialsCount++,
			["sweetflower"] = materialsCount++,
			["carrot"] = materialsCount++,
			["radish"] = materialsCount++,
			["snapdragon"] = materialsCount++,
			["mint"] = materialsCount++,
			["wheat"] = materialsCount++,
			["cabbage"] = materialsCount++,
			["pinecone"] = materialsCount++,
			["berry"] = materialsCount++,
			["rawmeat"] = materialsCount++,
			["birdegg"] = materialsCount++,
			["matsutake"] = materialsCount++,
			["fowl"] = materialsCount++,
			["crab"] = materialsCount++,
			["crabroe"] = materialsCount++,
			["salt"] = materialsCount++,
			["onion"] = materialsCount++,
			["pepper"] = materialsCount++,
			["milk"] = materialsCount++,
			["tomato"] = materialsCount++,
			["potato"] = materialsCount++,
			["fish"] = materialsCount++,
			["tofu"] = materialsCount++,
			["almond"] = materialsCount++,
			["bambooshoot"] = materialsCount++,
			["rice"] = materialsCount++,
			["shrimpmeat"] = materialsCount++,
			["chilledmeat"] = materialsCount++,
			["unagimeat"] = materialsCount++,
			["seagrass"] = materialsCount++,
			["lavendermelon"] = materialsCount++,
			["flour"] = materialsCount++,
			["cream"] = materialsCount++,
			["smokedfowl"] = materialsCount++,
			["butter"] = materialsCount++,
			["ham"] = materialsCount++,
			["sugar"] = materialsCount++,
			["jam"] = materialsCount++,
			["cheese"] = materialsCount++,
			["bacon"] = materialsCount++,
			["sausage"] = materialsCount++,
			["lotushead"] = materialsCount++,

            // local specialty
            ["callalily"] = materialsCount++,
			["wolfhook"] = materialsCount++,
			["valberry"] = materialsCount++,
			["cecilia"] = materialsCount++,
			["windwheelaster"] = materialsCount++,
			["philanemomushroom"] = materialsCount++,
			["jueyunchili"] = materialsCount++,
			["noctilucousjade"] = materialsCount++,
			["silkflower"] = materialsCount++,
			["glazelily"] = materialsCount++,
			["qingxin"] = materialsCount++,
			["starconch"] = materialsCount++,
			["violetgrass"] = materialsCount++,
			["smalllampgrass"] = materialsCount++,
			["dandelionseed"] = materialsCount++,
			["corlapis"] = materialsCount++,
			["onikabuto"] = materialsCount++,
			["sakurabloom"] = materialsCount++,
			["crystalmarrow"] = materialsCount++,
			["dendrobium"] = materialsCount++,
			["nakuweed"] = materialsCount++,
			["seaganoderma"] = materialsCount++,
			["sangopearl"] = materialsCount++,
			["amakumofruit"] = materialsCount++,
        };
		private static readonly Dictionary<string, int> enhancementMaterials = new Dictionary<string, int>
		{
			["enhancementore"] = 1,
			["fineenhancementore"] = 2,
			["mysticenhancementore"] = 3,
			["sanctifyingunction"] = 4,
			["sanctifyingessence"] = 5, // 4
        };

		
		// TODO: Remove static modifier. This could probably cause problems if a scanning crash is caught while an engine is running.
		// May be ok to create new engines as needed
		private static readonly TesseractEngine ocr_live = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
		private static readonly TesseractEngine ocr_1 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
		private static readonly TesseractEngine ocr_2 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
		private static readonly TesseractEngine ocr_3 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
		private static readonly TesseractEngine ocr_4 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
		private static readonly TesseractEngine ocr_5 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
		private static readonly TesseractEngine ocr_6 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
		private static readonly TesseractEngine ocr_7 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);
		private static readonly TesseractEngine ocr_8 = new TesseractEngine((Directory.GetCurrentDirectory()) + "\\tessdata", "genshin_fast_09_04_21", EngineMode.LstmOnly);

		//TODO: subStats Dictionaries


		public static void AddTravelerToCharacterList(string traveler)
		{
			if (!characters.ContainsKey(traveler))
			{
				characters.Add(traveler, 1);
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

		public static string AnalyzeFewText(Bitmap img)
		{
			string text = "";
			using (var ocr = new TesseractEngine(Directory.GetCurrentDirectory() + "\\tessdata", "eng", EngineMode.TesseractOnly))
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
			if (setNames.TryGetValue(setName, out code))
			{
				return code;
			}
			else
			{
				Debug.Print("Error: " + setName + " is not a valid Set Name");
				UserInterface.AddError("Error: " + setName + " is not a valid Set Name");
				//Form1.UnexpectedError(setName + " is not a valid Set Name");
				return -1;
			};
		}

		public static int GetMainStatCode(string mainStat)
		{
			int code = -1;
			if (mainStats.TryGetValue(mainStat, out code))
			{
				return code;
			}
			else
			{
				Debug.Print("Error: " + mainStat + " is not a valid Main Stat Name");
				UserInterface.AddError(mainStat + " is not a valid Main Stat Name");
				return -1;
			};
		}

		public static int GetGearSlotCode(string gearSlot)
		{
			int code = -1;
			if (gearSlots.TryGetValue(gearSlot, out code))
			{
				return code;
			}
			else
			{
				Debug.Print("Error: " + gearSlot + " is not a valid Gear Slot");
				UserInterface.AddError("gearSlot" + " is not a valid Gear Slot");
				return -1;
			};
		}

		public static int GetSubStatCode(string subStat)
		{
			int code = -1;
			if (subStats.TryGetValue(subStat, out code))
			{
				return code;
			}
			else
			{
				Debug.Print("Error: " + subStat + " is not a valid Sub Stat");
				UserInterface.AddError(subStat + " is not a valid Sub Stat");
				return -1;
			};
		}

		public static int GetCharacterCode(string character, bool bRedo = false)
		{
			if (characters.TryGetValue(character, out int code))
			{
				return code;
			}
			else if (Scraper.b_AssignedTravelerName == false)
			{
				return 1;
			}
			else
			{
				if (!bRedo)
				{
					UserInterface.AddError(character + " is not a valid Character Name");
				}
				return -1;
			}
		}

		public static int GetElementalCode(string element, bool bRedo = false)
		{
			if (elements.TryGetValue(element, out int code))
			{
				return code;
			}
			else
			{
				Debug.Print("Error: " + element + " is not a valid Elemental Type");
				if (!bRedo)
				{
					Form1.UnexpectedError(element + " is not a valid Elemental Type");
				}
				return -1;
			};
		}

		public static int GetEnhancementMaterialCode(string material)
		{
			return enhancementMaterials.TryGetValue(material, out int code) ? code : -1; ;
		}

		public static int GetWeaponCode(string weapon)
		{
			int code = -1;
			if (weapons.TryGetValue(weapon, out code))
			{
				return code;
			}
			else
			{
				Debug.Print("Error: " + weapon + " is not a valid Weapon Name");
				UserInterface.AddError(weapon + " is not a valid Weapon Name");
				//Form1.UnexpectedError(weapon + " is not a valid Weapon Name");
				return -1;
			};
		}
		#endregion

		public static string FindElementByName(string name)
		{
			string element = "";

			foreach (string e in elementList)
			{
				if (name.Contains(e))
				{
					element = e;
					break;
				}
			}

			return element;
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

		public static Bitmap SetGrayscale(Bitmap bitmap)
		{
			Bitmap temp = bitmap;
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
			Bitmap temp = bitmap;
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
			Bitmap temp = bitmap;
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
		(int)((255.0 * Math.Pow(i / 255.0, 1.0 / color)) + 0.5));
			}
			return gammaArray;
		}

		public static void SetInvert(ref Bitmap bitmap)
		{
			Bitmap temp = bitmap;
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
