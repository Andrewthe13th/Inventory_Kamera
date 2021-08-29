# Genshin Scanner

An OCR Scanner that scans your characters, weapons and artifacts in Genshin Impact. This produces a JSON file that can be easily formated to work with most genshin websites~
Currently it only supports ENGLISH words. 

## How to use
1. [Download the program](https://github.com/Andrewthe13th/Genshin_Scanner/releases/download/v1.0/GenshinGuide.zip), unzip and then run.
2. Open Genshin Impact and Log In. 
3. In Genshin Impact, go to Settings
4. Under Graphics, set Display Mode to 1280x720 Windowed
5. Under Controls, set Control Type to Keyboard
6. Exit Settings and leave the game in the Paimon Menu

### Can this get me banned?
 According to [miHoYo's response](https://genshin.mihoyo.com/en/news/detail/5763), I would say no. 
1. This scanner does not exploit the game. It just takes pictures of the game window.
2. This scanner does no provide artificial in-game progress.
3. This scanner does nothing to provide account selling/exchanging.
4. Doesn't provide top up in primo gems. 
Also this scanner has random length pauses which makes sure it doesn't scan too fast to prevent false positives. My account has been used to test this script and has yet to be banned or warned of the use of scripts. 

### JSON Format
```
characters: [{},{},{}...] //contains all characters with equipped weapon and artifacts
inventory: {[],[]} //All weapons and artifacts
```

# Character Object
```
name: digit // use CharacterList to get name  (ex: CharacterList[name])
element: digit // use ElementNameList
level: digit
ascension: boolean // ex: 80/80 -> false; 80/90 -> true
experience: digit (NOT functional right now)
weapon: {} // detailed in Weapon Object Section
artifacts: [ // detailed in Artifact Object Section
  0: {} // flower
  1: {} // feather
  2: {} // sands
  3: {} // goblet
  4: {} // circlet
]
constellation: digit
talents: [
  0: digit //auto attack level
  1: digit //skill attack level
  2: digit //burst attack level
]
```

# Weapon Object
```
name: digit // use example list to get name
level: digit
ascension: boolean
refinementLevel: digit
equippedCharacter: digit // use CharacterList
id: digit // order of weapon scanned
```

# Artifact Object
```
gearSlot: digit // 0-4 (flower,feather,sands,goblet,circlet) use ArtifactSlotList
rarity: digit // 5-1 stars
mainStat: digit // use ArtifactMainStatList
level: digit
subStats: [{stat: digit, value: float}{stat: digit, value: float}{stat: digit, value: float}{stat: digit, value: float}] // use ArtifactSubStatList
subStatsCount: digit
setName: digit // use ArtifactList
equippedCharacter: digit // use CharacterList
id: digit // order of artifact scanned
```

### Decoder List
// Example for decoding JSON to english. To support other languages replace words with the corresponding language
// example if name == 1: CharacterList[characters[0].name] = "Traveler"

const CharacterList=[
    null,//0
    "Traveler",
    "Amber",
    "Kaeya",
    "Lisa",
    "Barbara",//5
    "Razor",
    "Xiangling",
    "Beidou",
    "Xingqiu",
    "Ningguang",//10
    "Fischl",
    "Bennett",
    "Noelle",
    "Chongyun",
    "Sucrose",//15
    "Jean",
    "Diluc",
    "Qiqi",
    "Mona",
    "Keqing",//20
    "Venti",
    "Klee",
    "Diona",
    "Tartaglia",
    "Xinyan",//25
    "Zhongli",
    "Albedo",
    "Ganyu",
    "Xiao",
    "Hu Tao",//30
    "Rosaria",
    "Yanfei",
    "Eula",
    "Kaedehara Kazuha",
    "Kamisato Ayaka",//35
    "Yoimiya",
    "Sayu",
    "Raiden Shogun",
    "Kujousara",
    "Aloy", // 40
    "Sangonomi Yakokomi",
]

const WeaponList=[
    //1Star
    "Dull Blade",
    "Waster Greatsword",
    "Beginners Protector",
    "Apprentices Notes",
    "Hunter's Bow",//4
    //2Stars
    "Silver Sword",
    "Old Mercs Pal",
    "Iron Point",
    "Pocket Grimoire",
    "Seasoned Hunters Bow",//9
    //3Star
    "Cool Steel",//10
    "Harbinger Of Dawn",
    "Travelers Handy Sword",
    "Fillet Blade",
    "Skyrider Sword",
    "Ferrous Shadow",//15
    "Bloodtainted Greatsword",
    "White Iron Greatsword",
    "Debate Club",
    "Skyrider Greatsword",
    "White Tassel",//20
    "Halberd",
    "BlackTassel",
    "MagicGuide",
    "Thrilling Tales Of Dragon Slayers",
    "Otherworldly Story",//25
    "Emerald Orb",
    "Twin Nephrite",
    "Raven Bow",
    "Sharpshooters Oath",
    "Recurve Bow",//30
    "Slingshot",
    "Messenger",
    //4star
    "Favonius Sword",
    "The Flute",
    "Sacrificial Sword",//35
    "Royal Longsword",
    "Lions Roar",
    "Prototype Rancour",
    "Iron Sting",
    "Blackcliff Longsword",//40
    "The Black Sword",
    "Sword Of Descension",
    "Festering Desire",
    "The Alley Flash",
    "Favonius Greatsword",//45
    "The Bell",
    "Sacrificial Greatsword",
    "Royal Greatsword",
    "Rainslasher",
    "Prototype Archaic",//50
    "Whiteblind",
    "Blackcliff Slasher",
    "Serpent Spine",
    "SnowTombed Starsilver",
    "Lithic Blade",//55
    "Dragons Bane",
    "Prototype Starglitter",
    "Crescent Pike",
    "Blackcliff Pole",
    "Deathmatch",//60
    "Favonius Lance",
    "Royal Spear",
    "Dragonspine Spear",
    "Lithic Spear",
    "Favonius Codex",//65
    "The Widsith",
    "Sacrificial Fragments",
    "Royal Grimoire",
    "Solar Pearl",
    "Prototype Amber",//70
    "Mappa Mare",
    "Blackcliff Agate",
    "Eye Of Perception",
    "Frostbearer",
    "Wine And Song",//75
    "Favonius Warbow",
    "The Stringless",
    "Sacrificial Bow",
    "Royal Bow",
    "Rust",//80
    "Prototype Crescent",
    "Compound Bow",
    "Blackcliff Warbow",
    "The Viridescent",
    "Alley Hunter",//85
    "Windblume Ode",
    "Aquilia Favonia",
    "Skyward Blade",
    "Summit Shaper",
    "Primordial Jade Cutter",//90
    "Skyward Pride",
    "Wolf's Gravestone",
    "The Unforged",
    "Primordial Jade-Winged Spear",
    "Skyward Spine",//95
    "Vortext Vanquisher",
    "Staff Of Homa",
    "Skyward Atlas",
    "Lost Prayer To The SacredWinds",
    "Memory Of Dust",//100
    "Skyward Harp",
    "Amo's Bow",
    "Elegy For The End",
    "Song Of Broken Pines",
    "Mitternachts Waltz",//105
    "Freedom-Sworn",
    "Dodoco Tales",
    //InazumaPatch
    "Amenoma Kageuchi",
    "Katsuragikiri Nagamasa",
    "Kitain Cross Spear",//110
    "Hamayumi",
    "Hakushin Ring",
    "Mistsplitter Reforged",
    "Thundering Pulse",
    // 2.1
    "Predator", // 115
    "Luxurious SeaLord",
    "The Catch",
    "Engulfing Lightning",
    "Everlasting Moonglow",
]

const ArtifactList=[
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

const ArtifactSlotList=[
    "Flower",
    "Plume",
    "Sands",
    "Goblet",
    "Circlet",
]

const ArtifactMainStatList = [
    "HP", //0
    "ATK",
    "HP%",
    "ATK%", //3
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

const ArtifactSubStatList = [
    "HP", // 0
    "HP%",
    "ATK",
    "ATK%",
    "DEF",
    "DEF%", // 5
    "Energy Recharage",
    "Elemental Mastery",
    "CRIT Rate",
    "CRIT DMG", //9
]

const ElementNameList = [
    'Pyro',
    'Hydro',
    'Dendro',
    'Electro',
    'Anemo',
    'Cryo',
    'Geo',
]

# License
* This project is under the [MIT](LICENSE.md) license.
* All rights reserved by © miHoYo Co., Ltd. This project is not affiliated nor endorsed by miHoYo. Genshin Impact™ and other properties belong to their respective owners.
* This project uses third-party libraries or other resources that may be
distributed under [different licenses](THIRD-PARTY-NOTICES.md).

