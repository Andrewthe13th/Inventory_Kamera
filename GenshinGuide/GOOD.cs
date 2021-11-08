using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenshinGuide
{
	public class GOOD
	{
		[JsonProperty] private readonly string format;
		[JsonProperty] private readonly int version;
		[JsonProperty] private readonly string source;
		[JsonProperty] public List<IWeapon> weapons;
		[JsonProperty] public List<IArtifact> artifacts;
		[JsonProperty] public List<ICharacter> characters;
		private readonly string[] eng_Names = {
			"",
			"Traveler",
			"Amber",
			"Kaeya",
			"Lisa",
			"Barbara",
			"Razor",
			"Xiangling",
			"Beidou",
			"Xingqiu",
			"Ningguang",
			"Fischl",
			"Bennett",
			"Noelle",
			"Chongyun",
			"Sucrose",
			"Jean",
			"Diluc",
			"Qiqi",
			"Mona",
			"Keqing",
			"Venti",
			"Klee",
			"Diona",
			"Tartaglia",
			"Xinyan",
			"Zhongli",
			"Albedo",
			"Ganyu",
			"Xiao",
			"HuTao",
			"Rosaria",
			"Yanfei",
			"Eula",
			"KaedeharaKazuha",
			"KamisatoAyaka",
			"Yoimiya",
			"Sayu",
			"RaidenShogun",
			"KujouSara",
			"Aloy",
			"SangonomiyaKokomi", };
		private readonly string[] eng_Weapons = {
             // 1 Star
            "DullBlade",
            "WasterGreatsword",
            "BeginnersProtector",
            "ApprenticesNotes",
            "HuntersBow",//4
            //2Stars
            "SilverSword",
            "OldMercsPal",
            "IronPoint",
            "PocketGrimoire",
            "SeasonedHuntersBow",//9
            //3Star
            "CoolSteel",//10
            "HarbingerOfDawn",
            "TravelersHandySword",
            "FilletBlade",
            "SkyriderSword",
            "FerrousShadow",//15
            "BloodtaintedGreatsword",
            "WhiteIronGreatsword",
            "DebateClub",
            "SkyriderGreatsword",
            "WhiteTassel",//20
            "Halberd",
            "BlackTassel",
            "MagicGuide",
            "ThrillingTalesOfDragonSlayers",
            "OtherworldlyStory",//25
            "EmeraldOrb",
            "TwinNephrite",
            "RavenBow",
            "SharpshootersOath",
            "RecurveBow",//30
            "Slingshot",
            "Messenger",
            //4star
            "FavoniusSword",
            "TheFlute",
            "SacrificialSword",//35
            "RoyalLongsword",
            "LionsRoar",
            "PrototypeRancour",
            "IronSting",
            "BlackcliffLongsword",//40
            "TheBlackSword",
            "SwordOfDescension",
            "FesteringDesire",
            "TheAlleyFlash",
            "FavoniusGreatsword",//45
            "TheBell",
            "SacrificialGreatsword",
            "RoyalGreatsword",
            "Rainslasher",
            "PrototypeArchaic",//50
            "Whiteblind",
            "BlackcliffSlasher",
            "SerpentSpine",
            "SnowTombedStarsilver",
            "LithicBlade",//55
            "DragonsBane",
            "PrototypeStarglitter",
            "CrescentPike",
            "BlackcliffPole",
            "Deathmatch",//60
            "FavoniusLance",
            "RoyalSpear",
            "DragonspineSpear",
            "LithicSpear",
            "FavoniusCodex",//65
            "TheWidsith",
            "SacrificialFragments",
            "RoyalGrimoire",
            "SolarPearl",
            "PrototypeAmber",//70
            "MappaMare",
            "BlackcliffAgate",
            "EyeOfPerception",
            "Frostbearer",
            "WineAndSong",//75
            "FavoniusWarbow",
            "TheStringless",
            "SacrificialBow",
            "RoyalBow",
            "Rust",//80
            "PrototypeCrescent",
            "CompoundBow",
            "BlackcliffWarbow",
            "TheViridescentHunt",
            "AlleyHunter",//85
            "WindblumeOde",
            "AquilaFavonia",
            "SkywardBlade",
            "SummitShaper",
            "PrimordialJadeCutter",//90
            "SkywardPride",
            "WolfsGravestone",
            "TheUnforged",
            "PrimordialJadeWingedSpear",
            "SkywardSpine",//95
            "VortextVanquisher",
            "StaffOfHoma",
            "SkywardAtlas",
            "LostPrayerToTheSacredWinds",
            "MemoryOfDust",//100
            "SkywardHarp",
            "AmosBow",
            "ElegyForTheEnd",
            "SongOfBrokenPines",
            "MitternachtsWaltz",//105
            "Freedom-Sworn",
            "DodocoTales",
            //InazumaPatch
            "AmenomaKageuchi",
            "KatsuragikiriNagamasa",
            "KitainCrossSpear",//110
            "Hamayumi",
            "HakushinRing",
            "MistsplitterReforged",
            "ThunderingPulse",
            // 2.1
            "Predator", // 115
            "LuxuriousSeaLord",
            "TheCatch",
            "EngulfingLightning",
            "EverlastingMoonglow",
            "DarkIronSword",
            // 2.2
            "PolarStar", 
            "Akuoumaru",
        };
        private string[] eng_Artifacts = { 
            "Adventurer",
            "LuckyDog",
            "TravelingDoctor",
            "ResolutionOfSojourner",
            "TinyMiracle",
            "Berserker",//5
            "Instructor",
            "TheExile",
            "DefendersWill",
            "BraveHeart",
            "MartialArtist",//10
            "Gambler",
            "Scholar",
            "PrayersForIllumination",
            "PrayersForDestiny",
            "PrayersForWisdom",//15
            "PrayersTheSpringtime",
            "Thundersoother",
            "Lavawalker",
            "MaidenBeloved",
            "GladiatorsFinale",//20
            "ViridescentVenerer",
            "WanderersTroupe",
            "ThunderingFury",
            "CrimsonWitchOfFlames",
            "NoblesseOblige",//25
            "BloodstainedChivalry",
            "ArchaicPetra",
            "RetracingBolide",
            "HeartOfDepth",
            "BlizzardStrayer",//30
            "PaleFlame",
            "TenacityOfTheMillelith",
            "ShimenawasReminiscence",
            "EmblemOfSeveredFate", 
        };
        private string[] eng_ArtifactSlotList = { 
            "flower",
            "plume",
            "sands",
            "goblet",
            "circlet",
        };
        private string[] eng_ArtifactMainStatList =
        {
            "hp", //0
            "atk",
            "hp_",
            "atk_", //3
            "def_",
            "enerRech_", //5
            "eleMas",
            "heal_",
            "critRate_",
            "critDMG_",
            "physical_dmg_", //10
            "pyro_dmg_",
            "electro_dmg_",
            "cryo_dmg_",
            "hydro_dmg_",
            "anemo_dmg_", //15
            "geo_dmg_",
        };
        private string[] eng_ArtifactSubStatList = {
            "hp", // 0
            "hp_",
            "atk",
            "atk_",
            "def",
            "def_", // 5
            "enerRech_",
            "eleMas",
            "critRate_",
            "critDMG_", //9
        };

        public GOOD()
        {
            format = "EMPTY";
            version = 0;
            source = "NOT FILLED";
            characters = new List<ICharacter>();
            artifacts = new List<IArtifact>();
            weapons = new List<IWeapon>();
        }

        public GOOD(GenshinData genshinData)
        {
            format = "GOOD";
            version = 1;
            source = "Genshin_Data_Scanner";
            characters = new List<ICharacter>();
            artifacts = new List<IArtifact>();
            weapons = new List<IWeapon>();

            // Get rid of VS warning since we are converting this class to JSON
            format = format.Trim();
            version = version + 0;
            source = source.Trim();

            // Assign Characters
            List<Character> _characters = genshinData.GetCharacters();
            foreach (Character x in _characters)
            {
                ICharacter temp = new ICharacter();
                temp.key = eng_Names[x.name];
                temp.level = x.level;
                temp.constellation = x.constellation;
                temp.ascension = x.AscensionCount();
                temp.talent._auto = x.GetTalents()[0];
                temp.talent.skill = x.GetTalents()[1];
                temp.talent.burst = x.GetTalents()[2];
                characters.Add(temp);
            }

            // Assign Weapons
            List<Weapon> _weapons = genshinData.GetInventory().GetWeaponList();
            foreach (Weapon x in _weapons)
            {
                IWeapon temp = new IWeapon();
                temp.key = eng_Weapons[x.name];
                temp.level = x.level;
                temp.ascension = x.AscensionCount();
                temp.refinement = x.refinementLevel;
                temp.location = eng_Names[x.equippedCharacter];
                weapons.Add(temp);
            }

			// Assign Artifacts
			List<Artifact> _artifacts = genshinData.GetInventory().GetArtifacts();
			foreach (Artifact x in _artifacts)
			{
				// only assign artifact level 5-4
				if (x.rarity >= 4)
				{
					IArtifact temp = new IArtifact
					{
						setKey = eng_Artifacts[x.setName],
						slotKey = eng_ArtifactSlotList[x.gearSlot],
						level = x.level,
						rarity = x.rarity,
						mainStatKey = eng_ArtifactMainStatList[x.mainStat],
						location = eng_Names[x.equippedCharacter],
						_lock = x._lock,
						// SubStats
						substats = new ISubstat[4]
					};
					for (int i = 0; i < 4; i++)
					{
						if (x.subStats[i].value != 0)
						{
							temp.substats[i].key = eng_ArtifactSubStatList[x.subStats[i].stat];
							temp.substats[i].value = x.subStats[i].value;
						}
						else
						{
							temp.substats[i].key = "";
							temp.substats[i].value = x.subStats[i].value;
						}
					}
					artifacts.Add(temp);
				}
			}

		}

		public void WriteToJSON(string outputDirectory, string oldDataFilePath = "")
		{
			// Create JSON object
			string dataString = JsonConvert.SerializeObject(this);

			// Conform 'lock' and 'auto' keys to GOOD format
			dataString = dataString.Replace("_lock", "lock");
			dataString = dataString.Replace("_auto", "auto");

			// Check for output directory
			if (!Directory.Exists(outputDirectory))
			{
				Directory.CreateDirectory(outputDirectory);
			}

			// Create file with timestamp in name
			string fileName = "\\genshinData_GOOD_" + DateTime.Now.ToString("MM.dd.yyyy_HH.mm.ss") + ".json";
			fileName = fileName.Replace('/', '_');
			string outputFile = outputDirectory + fileName;

			// Try to load external GOOD data to update.
			// For preserving information at when uploading data to
			// https://frzyc.github.io/genshin-optimizer 
			JObject database = null;
			if (File.Exists(oldDataFilePath))
			{
				try
				{
					// Load source data
					using (StreamReader file = File.OpenText(oldDataFilePath))
					using (JsonTextReader reader = new JsonTextReader(file))
					{
						database = (JObject)JToken.ReadFrom(reader);
					}

					// Characters
					foreach (ICharacter character in characters)
					{
						foreach (JObject dbCharacter in database["characters"])
						{
							{
								if ((string)dbCharacter["key"] == character.key)
								{
									dbCharacter["level"] = character.level;
									dbCharacter["constellation"] = character.constellation;
									dbCharacter["ascension"] = character.ascension;
									dbCharacter["talent"] = JObject.FromObject(character.talent);
									break;
								}
							}
						}
					}
					// Weapons
					foreach (IWeapon weapon in weapons)
					{
						foreach (JToken dbWeapon in database["weapons"])
						{
							if ((string)dbWeapon["key"] == weapon.key)
							{
								break;
							}
						}
					}
					// Artifacts


					using (StreamWriter file = File.CreateText(outputFile))
					using (JsonWriter writer = new JsonTextWriter(file))
					{
						database.WriteTo(writer);
					}
					Debug.WriteLine("Successfully merged databases");
				}

				catch (Exception)
				{
					UserInterface.AddError("Unable to create merged database.");
					WriteStringToFile(dataString, outputFile);
				}
			}
			else
			{
				// Write file
				WriteStringToFile(dataString, outputFile);
			}

			if (!File.Exists(outputFile)) // did not make file
			{
				UserInterface.AddError("Failed to output at : " + outputDirectory);
			}
		}

		private static void WriteStringToFile(string dataString, string outputFile)
		{
			using (var streamWriter = new StreamWriter(outputFile, true))
			{
				streamWriter.WriteLine(dataString.ToString());
			}
		}

		public struct IWeapon
		{
			[JsonProperty] public string key;
			[JsonProperty] public int level;
			[JsonProperty] public int ascension;
			[JsonProperty] public int refinement;
			[JsonProperty] public string location;
		}

        private struct IArtifact
        {
            [JsonProperty] public string setKey;
            [JsonProperty] public string slotKey;
            [JsonProperty] public int level;
            [JsonProperty] public int rarity;
            [JsonProperty] public string mainStatKey;
            [JsonProperty] public string location;
            // change Lock to "_lock" after
            [JsonProperty] public bool _lock;
            [JsonProperty] public ISubstat[] substats;
        }

        private struct ISubstat
        {
            [JsonProperty] public string key;
            [JsonProperty] public decimal value;
        }

		public struct ICharacter
		{
			[JsonProperty] public string key;
			[JsonProperty] public int level;
			[JsonProperty] public int ascension;
			[JsonProperty] public ITalent talent;
			[JsonProperty] public int constellation;
		}

        private struct ITalent
        {
            // change auto to _auto
            [JsonProperty] public int _auto;
            [JsonProperty] public int skill;
            [JsonProperty] public int burst;
        }
    }
}
