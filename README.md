# Genshin Scanner

An OCR Scanner that scans your characters, weapons and artifacts in Genshin Impact. This produces a JSON file that can be easily formated to work with most genshin websites.
Currently it only supports ENGLISH. 

## How to use
1. [Download the program](https://github.com/Andrewthe13th/Genshin_Scanner/releases/download/v0.9/Genshin_Scanner_V0.9.zip), unzip and then run.
2. Open Genshin Impact and Log In. 
3. In Genshin Impact, go to Settings
4. Under Controls, set Control Type to Keyboard
5. Exit Settings and leave the game in the Paimon Menu
6. Go back to the scanner application, then click SCAN. (You can stop the scan anytime by pressing the 'ENTER' key)
Note: You will not be able to use your computer while the scan is taking place due to require the use of mouse and keyboard inputs.

### Can this get me banned?
 According to [miHoYo's response](https://genshin.mihoyo.com/en/news/detail/5763), I would say no. 
1. This scanner does not exploit the game. It just takes pictures of the game window.
2. This scanner does no provide artificial in-game progress.
3. This scanner does nothing to provide account selling/exchanging.
4. Doesn't provide top up in primo gems. 
Also this scanner has random length pauses which makes sure it doesn't scan too fast to prevent false positives.
My account has been used to test this script and has yet to be banned or warned of the use of scripts. 

# JSON Format
```
characters: [{},{},{}...] // contains all characters with equipped weapon and any artifacts
inventory: {[],[]} // All weapons and artifacts
```

## Character Object
```
name: string (See 'characters' in Scraper.cs)
element: string (See 'elements' in Scraper.cs)
level: number
ascension: boolean // ex: 80/80 -> false; 80/90 -> true
experience: number (NOT implemented)
weapon: Weapon // detailed in Weapon Object Section
artifacts: [ // detailed in Artifact Object Section
  'flower'  : Artifact
  'plume'   : Artifact
  'sands'   : Artifact
  'goblet'  : Artifact
  'circlet' : Artifact
]
constellation: number
talents: [
  0: number //auto attack level
  1: number //skill attack level
  2: number //burst attack level
]
```

## Weapon Object
```
name: string // See 'weapons' in Scraper.cs
level: number
ascension: boolean // ex: 80/80 -> false; 80/90 -> true
refinementLevel: number
equippedCharacter: string ("" or name from 'characters' in Scraper.cs)
id: number // order of weapon scanned
```

## Artifact Object
```
gearSlot: string  (See 'gearSlots' in Scraper.cs)
rarity: number 
mainStat: string  (See 'stats' in Scraper.cs)
level: number
subStats: [
	{stat: string, value: float}
	{stat: string, value: float}
	{stat: string, value: float}
	{stat: string, value: float}]
subStatsCount: number
setName: string (See 'setNames' in Scraper.cs)
equippedCharacter: string
id: number // order of artifact scanned
```

## Decoder List
This lists bellow are used for decoding internal objects to english. To support other languages replace words with the corresponding language.
ex: (if name == "amber") => CharacterList[characters.IndexOf(name)] is "Amber". These lists can also be found in Scraper.cs and GOOD.cs.
```
const characters=[
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
	"Hu Tao",
	"Rosaria",
	"Yanfei",
	"Eula",
	"Kaedehara Kazuha",
	"Kamisato Ayaka",
	"Yoimiya",
	"Sayu",
	"Raiden Shogun",
	"Kujousara",
	"Aloy",
	"Sangonomi Yakokomi",
	"Thoma",
]

const weapons=[
	// Release Weapons
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
]

const setNames=[
	"Adventurer",
	"LuckyDog",
	"Traveling Doctor",
	"Resolution Of Sojourner",
	"Tiny Miracle",
	"Berserker",//5
	"Instructor",
	"The Exile",
	"Defenders Will",
	"Brave Heart",
	"Martial Artist",//10
	"Gambler",
	"Scholar",
	"Prayers For Illumination",
	"Prayers For Destiny",
	"Prayers For Wisdom",//15
	"Prayers For Springtime",
	"Thundersoother",
	"Lavawalker",
	"Maiden Beloved",
	"Gladiator's Finale",//20
	"Viridescent Venerer",
	"Wanderers Troupe",
	"Thundering Fury",
	"Crimson Witch Of Flames",
	"Noblesse Oblige",//25
	"Bloodstained Chivalry",
	"Archaic Petra",
	"Retracing Bolide",
	"Heart Of Depth",
	"Blizzard Strayer",//30
	"Pale Flame",
	"Tenacity Of The Millelith",
	"Shimenawas Reminiscence",
	"Emblem Of SeveredFate",
]

const gearSlots =[
	"flower",
	"plume",
	"sands",
	"goblet",
	"circlet",
]

const stats = [
	"HP", //0
	"HP%",
	"ATK",
	"ATK%", //3
	"DEF"
	"DEF%",
	"Energy Recharge", //5
	"Elemental Mastery",
	"Healing Bonus",
	"CRIT Rate",
	"CRIT DMG",
	"Physical DMG Bonus", //10
	"Pyro DMG Bonus",
	"Electro DMG Bonus",
	"Cryo DMG Bonus",
	"Hydro DMG Bonus",
	"Anemo DMG Bonus", //15
	"Geo DMG Bonus",
]

const elements = [
	"Pyro",
	"Hydro",
	"Dendro",
	"Electro",
	"Anemo",
	"Cryo",
	"Geo",
]
```
# License
* This project is under the [MIT](LICENSE) license.
* All rights reserved by © miHoYo Co., Ltd. This project is not affiliated nor endorsed by miHoYo. Genshin Impact™ and other properties belong to their respective owners.
* This project uses third-party libraries or other resources that may be
distributed under [different licenses](THIRD-PARTY-NOTICES.md).

