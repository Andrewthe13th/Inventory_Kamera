using System.Collections.Generic;
using Newtonsoft.Json;

namespace GenshinGuide
{
    public class GOOD
    {
        [JsonProperty] private string format;
        [JsonProperty] private int version;
        [JsonProperty] private string source;
        [JsonProperty] private List<ICharacter> characters;
        [JsonProperty] private List<IArtifact> artifacts;
        [JsonProperty] private List<IWeapon> weapons;
        private string[] eng_Names = {
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
            "SangonomiyaKokomi",
            "Thoma",
        };
        private string[] eng_Weapons = {
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
            List<Artifact> _artifacts = genshinData.GetInventory().GetArtifactList();
            foreach (Artifact x in _artifacts)
            {
                // only assign artifact level 5-4
                if(x.rarity >= 4)
                {
                    IArtifact temp = new IArtifact();
                    temp.setKey = eng_Artifacts[x.setName];
                    temp.slotKey = eng_ArtifactSlotList[x.gearSlot];
                    temp.level = x.level;
                    temp.rarity = x.rarity;
                    temp.mainStatKey = eng_ArtifactMainStatList[x.mainStat];
                    temp.location = eng_Names[x.equippedCharacter];
                    temp._lock = x._lock;
                    // SubStats
                    temp.substats = new ISubstat[4];
                    for(int i = 0; i < 4; i++)
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

        private struct IWeapon
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

        private struct ICharacter
        {
            [JsonProperty] public string key;
            [JsonProperty] public int level;
            [JsonProperty] public int constellation;
            [JsonProperty] public int ascension;
            [JsonProperty] public ITalent talent;
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
