using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InventoryKamera
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
			"KaedeharaKazuha",
			"KamisatoAyaka",
			"KamisatoAyaka",
			"Yoimiya",
			"Sayu",
			"RaidenShogun",
			"RaidenShogun",
			"KujouSara",
			"KujouSara",
			"Aloy",
			"SangonomiyaKokomi",
			"SangonomiyaKokomi",
			"Thoma",
			// "AratakiItto",
			// "Gorou"
		};

		private readonly string[] eng_Weapons = {
             // 1 Star
            "DullBlade",
			"WasterGreatsword",
			"BeginnersProtector",
			"ApprenticesNotes",
			"HuntersBow",

            // 2 Stars
            "SilverSword",
			"OldMercsPal",
			"IronPoint",
			"PocketGrimoire",
			"SeasonedHuntersBow",

            // 3 Star
            "CoolSteel",
			"HarbingerOfDawn",
			"TravelersHandySword",
			"FilletBlade",
			"SkyriderSword",
			"FerrousShadow",
			"BloodtaintedGreatsword",
			"WhiteIronGreatsword",
			"DebateClub",
			"SkyriderGreatsword",
			"WhiteTassel",
			"Halberd",
			"BlackTassel",
			"MagicGuide",
			"ThrillingTalesOfDragonSlayers",
			"OtherworldlyStory",
			"EmeraldOrb",
			"TwinNephrite",
			"RavenBow",
			"SharpshootersOath",
			"RecurveBow",
			"Slingshot",
			"Messenger",

            // 4 star
            "FavoniusSword",
			"TheFlute",
			"SacrificialSword",
			"RoyalLongsword",
			"LionsRoar",
			"PrototypeRancour",
			"IronSting",
			"BlackcliffLongsword",
			"TheBlackSword",
			"SwordOfDescension",
			"FesteringDesire",
			"TheAlleyFlash",
			"FavoniusGreatsword",
			"TheBell",
			"SacrificialGreatsword",
			"RoyalGreatsword",
			"Rainslasher",
			"PrototypeArchaic",
			"Whiteblind",
			"BlackcliffSlasher",
			"SerpentSpine",
			"SnowTombedStarsilver",
			"LithicBlade",
			"DragonsBane",
			"PrototypeStarglitter",
			"CrescentPike",
			"BlackcliffPole",
			"Deathmatch",
			"FavoniusLance",
			"RoyalSpear",
			"DragonspineSpear",
			"LithicSpear",
			"FavoniusCodex",
			"TheWidsith",
			"SacrificialFragments",
			"RoyalGrimoire",
			"SolarPearl",
			"PrototypeAmber",
			"MappaMare",
			"BlackcliffAgate",
			"EyeOfPerception",
			"Frostbearer",
			"WineAndSong",
			"FavoniusWarbow",
			"TheStringless",
			"SacrificialBow",
			"RoyalBow",
			"Rust",
			"PrototypeCrescent",
			"CompoundBow",
			"BlackcliffWarbow",
			"TheViridescentHunt",
			"AlleyHunter",
			"WindblumeOde",
			"AquilaFavonia",
			"SkywardBlade",
			"SummitShaper",
			"PrimordialJadeCutter",
			"SkywardPride",
			"WolfsGravestone",
			"TheUnforged",
			"PrimordialJadeWingedSpear",
			"SkywardSpine",
			"VortextVanquisher",
			"StaffOfHoma",
			"SkywardAtlas",
			"LostPrayerToTheSacredWinds",
			"MemoryOfDust",
			"SkywardHarp",
			"AmosBow",
			"ElegyForTheEnd",
			"SongOfBrokenPines",
			"MitternachtsWaltz",
			"Freedom-Sworn",
			"DodocoTales",

            // 2.0 Inazuma Patch
            "AmenomaKageuchi",
			"KatsuragikiriNagamasa",
			"KitainCrossSpear",
			"Hamayumi",
			"HakushinRing",
			"MistsplitterReforged",
			"ThunderingPulse",

            // 2.1
            "Predator",
			"LuxuriousSeaLord",
			"TheCatch",
			"EngulfingLightning",
			"EverlastingMoonglow",
			"DarkIronSword",

            // 2.2
            "PolarStar",
			"Akuoumaru",
			"MouunsMoon",
			"WavebreakersFin",

			// 2.3
			"RedhornStonethresher",
			"CinnabarSpindle"
		};

		private readonly string[] eng_Artifacts = {
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
            "PrayersToSpringtime",
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
			"HuskOfOpulentDreams",
			"OceanHuedClam"
		};

		private readonly string[] eng_ArtifactSlotList = {
			"flower",
			"plume",
			"sands",
			"goblet",
			"circlet",
		};

		private readonly string[] eng_Stats =
		{
			"hp", //0
			"hp_",
            "atk",
			"atk_", 
			"def", //4
            "def_",
			"enerRech_",
            "eleMas",
			"heal_", //8
			"critRate_",
			"critDMG_",
			"physical_dmg_",
            "pyro_dmg_", //12
			"electro_dmg_",
			"cryo_dmg_", 
			"hydro_dmg_",
			"anemo_dmg_", //16
            "geo_dmg_",
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

		public GOOD(InventoryKamera genshinData)
		{
			// Get rid of VS warning since we are converting this class to JSON
			format = "GOOD".Trim();
			version = 1;
			source = "Genshin_Data_Scanner".Trim();
			characters = new List<ICharacter>();
			artifacts = new List<IArtifact>();
			weapons = new List<IWeapon>();


			// Assign Characters
			foreach (Character character in genshinData.GetCharacters())
			{
				ICharacter temp = new ICharacter
				{
					key = eng_Names[Scraper.characters.IndexOf(character.name)],
					level = character.level,
					constellation = character.constellation,
					ascension = character.AscensionLevel()
				};
				temp.talent.auto = character.GetTalents()[0];
				temp.talent.skill = character.GetTalents()[1];
				temp.talent.burst = character.GetTalents()[2];
				characters.Add(temp);
			}

			// Assign Weapons
			foreach (Weapon weapon in genshinData.GetInventory().GetWeapons())
			{

				IWeapon w = new IWeapon
				{
					key = eng_Weapons[Scraper.weapons.IndexOf(weapon.name)],
					level = weapon.level,
					ascension = weapon.AscensionCount(),
					refinement = weapon.refinementLevel,
					location = Scraper.characters.Contains(weapon.equippedCharacter) ? eng_Names[Scraper.characters.IndexOf(weapon.equippedCharacter)] : ""
				};
				weapons.Add(w);
			}

			// Assign Artifacts
			foreach (Artifact artifact in genshinData.GetInventory().GetArtifacts())
			{
				// only assign artifact level 5-4
				if (artifact.rarity >= 4)
				{
					string setName = eng_Artifacts[Scraper.setNames.IndexOf(artifact.setName)];
					string slotName = eng_ArtifactSlotList[Scraper.gearSlots.IndexOf(artifact.gearSlot)];
					int index = Scraper.stats.IndexOf(artifact.mainStat);
					string mainStat = eng_Stats[index];
					string location = Scraper.characters.Contains(artifact.equippedCharacter) ? eng_Names[Scraper.characters.IndexOf(artifact.equippedCharacter)] : "";
					IArtifact temp = new IArtifact
					{
						setKey = setName,
						slotKey = slotName,
						level = artifact.level,
						rarity = artifact.rarity,
						mainStatKey = mainStat,
						location = location,
						@lock = artifact.@lock,
						substats = new ISubstat[4]
					};

					for (int i = 0; i < 4; i++)
					{
						if (!string.IsNullOrEmpty(artifact.subStats[i].stat))
						{
							temp.substats[i].key = eng_Stats[Scraper.stats.IndexOf(artifact.subStats[i].stat)];
							temp.substats[i].value = artifact.subStats[i].value;
						}
						else
						{
							temp.substats[i].key = "";
							temp.substats[i].value = 0;
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

			// Check for output directory
			if (!Directory.Exists(outputDirectory))
			{
				Directory.CreateDirectory(outputDirectory);
			}

			// Create file with timestamp in name
			string fileName = "\\genshinData_GOOD_" + DateTime.Now.ToString("MM.dd.yyyy_HH.mm.ss") + ".json";
			fileName = fileName.Replace('/', '_');
			string outputFile = outputDirectory + fileName;
			if (File.Exists(oldDataFilePath) && false) // && false hack. TODO: add support for data merging
			{
				try
				{

					// Try to load external GOOD data to update.
					// For preserving information at when uploading data to
					// https://frzyc.github.io/genshin-optimizer
					// This may not even be necessary
					JObject database;
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
		public struct IArtifact
		{
			[JsonProperty] public string setKey;
			[JsonProperty] public string slotKey;
			[JsonProperty] public int level;
			[JsonProperty] public int rarity;
			[JsonProperty] public string mainStatKey;
			[JsonProperty] public string location;
			[JsonProperty] public bool @lock;
			[JsonProperty] public ISubstat[] substats;
		}

		public struct ISubstat
		{
			[JsonProperty] public string key;
			[JsonProperty] public decimal value;
		}

		public struct IWeapon
		{
			[JsonProperty] public string key;
			[JsonProperty] public int level;
			[JsonProperty] public int ascension;
			[JsonProperty] public int refinement;
			[JsonProperty] public string location;
			[JsonProperty] public bool @lock;
		}

		public struct ICharacter
		{
			[JsonProperty] public string key;
			[JsonProperty] public int level;
			[JsonProperty] public int constellation;
			[JsonProperty] public int ascension;
			[JsonProperty] public ITalent talent;
		}

		public struct ITalent
		{
			[JsonProperty] public int @auto;
			[JsonProperty] public int skill;
			[JsonProperty] public int burst;
		}
	}
}