# Inventory Kamera - A Genshin Data Scanner

Fan-made Genshin Impact tool that scans characters, weapons, artifacts, materials and character development items in your inventory using the OCR technique.

This scanner exports in `.GOOD`, a JSON-based exporting format, which allows you to use it with compatible online Genshin Impact tools. These tools include artifact optimizing tools including [Genshin Optimizer](https://frzyc.github.io/genshin-optimizer/#/), [SEELIE.me](https://seelie.me/) and [Aspirine's Genshin Impact Calculator](https://genshin.aspirine.su/).

## Download
[LATEST DOWNLOAD HERE](https://github.com/JaDCHEH/Inventory_Kamera/releases/tag/release) <- CLICK ME NOT THE "CODE" BUTTON NEAR THE TOP OF THIS PAGE

## DISCORD
https://discord.gg/zh56aVWe3U



## Table of Contents
> **Note**
>
> Please **read the following instructions carefully** and setup before using the scanner.
- Getting Started
  - [Installing Inventory Kamera](#installing-inventory-kamera)
  - [Setting up Genshin Impact](#setting-up-genshin-impact)
  - [Settings and configurations](#how-to-configure-inventory-kamera)
  - [Running Inventory Kamera](#how-to-run-inventory-kamera)
- Scanner
  - [Updating the database](#updating-for-new-game-versions)
- Repository
  - [Reporting a bug or a scanning issue](#reporting-an-issue)
  - [Requesting a new feature](#requesting-a-new-feature)
  - [Asking a question](#asking-a-question)
  - [Frequently Asked Questions (FAQ)](#frequently-asked-questions-faq)
  - [License](#license)

## Installing Inventory Kamera

Before installing Inventory Kamera, please have **the following things installed on your device**:

- [GenshinImpact.exe](https://genshin.hoyoverse.com/) or [YuanShen.exe](https://ys.mihoyo.com/) launcher
- [Microsoft Visual C++ Redistributable for Visual Studio 2015-2022 (x86 or x64)](https://docs.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022), _**A computer restart may be required after installing this package for the first time**_

[Download the latest version of Inventory Kamera here](https://github.com/Andrewthe13th/Inventory_Kamera/releases/latest) and unzip its files into a folder of your choosing, then launch the extracted .exe file. It will likely prompt you for Security and/or User Access. This access is required to interact with the game.

To upgrade from a previous version, download a newer release and unzip its files into the current Inventory Kamera location (allow it to replace old files when prompted) or a new folder of your choosing.

## Setting up Genshin Impact

1. Log in to Genshin Impact and click start.
2. Open [Paimon menu](https://genshin-impact.fandom.com/wiki/Paimon_Menu) (Default shortcut: `ESC`).
3. Go to `Settings` (Cog Icon ⚙) and set these settings:
   - Under `Languages`, set _Game Language_ to English.
   - The game resolution should be _16:9_ or _16:10_. If your screen has a matching native resolution, you may set _Display Mode_ under `Graphics` to fullscreen. Otherwise, you'll have to switch it to any _windowed_ resolution that matches one of these aspect ratios.
     - Some examples of good resolutions are: 1920x1080 (Full HD), 1920x1200, 2560x1440 (QHD), 3840x2160 (4K), etc.
     - Don't know if a resolution is 16:9 or 16:10? [Find out here](https://andrew.hedges.name/experiments/aspect_ratio/).
        > **Warning**
        >
        > If you have an ultrawide screen, please see [this thread](https://github.com/Andrewthe13th/Inventory_Kamera/issues/40)
        > Additionally, HDR is not currently supported. If you experience scanning issues, please make sure this is disabled in Windows Settings
   - Under `Controls`, set _Control Type_ to Keyboard.
     - If you rebound keys (inventory (default: B), character screen (default: C), or selecting character 1 (default: 1)) revert your binding to default or set up the new key binding in Inventory Kamera.

## How to configure Inventory Kamera

Before starting the scanner, you can (optionally) edit the following options:

- Select which categories (weapons, artifacts, characters, items) you want to scan.
- Configure minimum weapon and artifact rarity to be scanned.
- Set the scanner delays to slow Inventory Kamera's scanning speed if you experience problems with scanning.
- Set the file export destination in the File Directory.
- Inventory Kamera uses the default keybinds to open the inventory (B) and character screens (C). These settings may be customized within Inventory Kamera's options.

## How to run Inventory Kamera

1. Start the Inventory Kamera scan by **leaving the game with the Paimon Menu open** and clicking 'Scan'.
> **Warning**
>
> While scanning, **do not use your mouse or keyboard**. The scanner uses keyboard and mouse input to automate scanning.
>
> If you want to terminate the scan early, you can press `ENTER` button at any time. This will cancel the scan, and the application will not automatically output any scanned results. You may press the 'Export Scanned Data' button to force the export of the most recently collected data (whether it's complete or incomplete).

2. Wait for Inventory Kamera to complete its scan for the `Scan Items` selections.

3. Press the `Open Export Folder` button to open a File Explorer window. This is where Inventory Kamera exports its scanned data.




## Updating for new game versions

Inventory Kamera uses lists of valid items and characters to assist with text recognition. These lists are kept locally in the `inventorylists` folder. These lists should be updated every time a new version of the game is released, and it can be done either automatically or manually.

### Updating automatically

A notification window will appear when starting the application and a new version of Genshin game data is found. Inventory Kamera syncs with [Dimbreath's Anime Game Data Repo](https://gitlab.com/Dimbreath/AnimeGameData) for new game versions. Big thanks for all the hard work done there.

If the update window does not show up when the applications starts, you can select `Update Lookup Tables` under `Options` to run the automatic updater. You may optionally force the updater to run if it does not detect a new Genshin Impact version. 

### Updating manually

> **Warning**
> 
> This option should only be necessary if Inventory Kamera's `Update Lookup Tables` reports an error or the generated ouputs are invalid.

The materials and weapons lists are kept in a simple key:value JSON-readable format. 'value' is the name of an item in [PascalCase](<https://en.wikipedia.org/wiki/Naming_convention_(programming)#Examples_of_multiple-word_identifier_formats>) and 'key' is whatever 'value' is but in lowercase. 

The format for manually updating characters and artifacts is slightly different. The key for both json files is still the lowercase version of the character/artifact set name in PascalCase, and the value is in the following example formats:

characters.json
```json
"albedo": {
    "GOOD": "Albedo",
    "ConstellationName": [
      "Princeps Cretaceus"
    ],
    "ConstellationOrder": [
      "skill",
      "burst"
    ],
    "Element": [
      "geo"
    ],
    "WeaponType": 0
  },


"traveler": {
    "GOOD": "Traveler",
    "Element": [
      "electro",
      "pyro",
      "dendro",
      "geo",
      "hydro",
      "anemo"
    ],
    "ConstellationOrder": {
      "electro": [
        "burst",
        "skill"
      ],
      "dendro": [
        "skill",
        "burst"
      ],
      "geo": [
        "burst",
        "skill"
      ],
      "hydro": [
        "skill",
        "burst"
      ],
      "anemo": [
        "burst",
        "skill"
      ]
    },
    "WeaponType": 0
  },
```
Note: The ConstellationName value is formatted as an array. This primarily applies to only the traveler as the male and female versions of the characters have different constellation names.
Note: The Traveler is a unique case in which constellation skill orders are be provided for each currently available element.

The character's name is as it appears in the party menu on the right side of the in-world UI or the character's menu screen in the top left corner. The constellation order depends on which talent the third constellation upgrades for each character.

Weapon type values are as follows:

0 = Sword, 1 = Claymore, 2 = Polearm, 3 = Bow, 4 = Catalyst

artifacts.json
```json
"adventurer": {
    "setName": "Adventurer",
    "GOOD": "Adventurer",
    "normalizedName": "adventurer",
    "artifacts": {
      "goblet": {
        "artifactName": "Adventurer's Golden Goblet",
        "GOOD": "AdventurersGoldenGoblet",
        "normalizedName": "adventurersgoldengoblet"
      },
      "plume": {
        "artifactName": "Adventurer's Tail Feather",
        "GOOD": "AdventurersTailFeather",
        "normalizedName": "adventurerstailfeather"
      },
      "circlet": {
        "artifactName": "Adventurer's Bandana",
        "GOOD": "AdventurersBandana",
        "normalizedName": "adventurersbandana"
      },
      "flower": {
        "artifactName": "Adventurer's Flower",
        "GOOD": "AdventurersFlower",
        "normalizedName": "adventurersflower"
      },
      "sands": {
        "artifactName": "Adventurer's Pocket Watch",
        "GOOD": "AdventurersPocketWatch",
        "normalizedName": "adventurerspocketwatch"
      }
    }
  },
```

Consider using [a JSON text validator](https://jsonlint.com/) after following this manual method. Support may or may not be provided if you choose this route.

## Reporting an issue

If you run into a problem with our scanner (e.g. a bug, app crash, invalid export format), please [create an issue here](https://github.com/Andrewthe13th/Inventory_Kamera/issues/new/choose) and try to fill it out as much as possible. It, along with the evidence, will greatly speed up the bug-fixing process. 

Alternatively, the [Inventory Kamera Discord](https://discord.gg/zh56aVWe3U) is open for more real-time discussion and support help.

> **Note**
>
> Before submitting an issue, **please [check for similar issues](https://github.com/Andrewthe13th/Inventory_Kamera/issues?q=is%3Aissue), especially the ones that are still open.**
>
> Start by leaving a reaction emoji to that issue (more reactions means more dev attention!). Please try to limit comments to new or helpful information (i.e. not "Same issue here" comments). You can choose to _subscribe_ to that issue by clicking 'Subscribe' in the Notifications section to get notifications on thread developments.

### Creating a new issue

We would **love to have Screenshots (especially video recordings!) and Error Logs as evidence**. These can be very helpful for debugging your problem. Add it to the issue via drag-and-drop or by attaching the file to the template. Inventory Kamera may place screenshots in the `logging` folder (divided into categories) when it thinks it encountered an issue. Attaching a zipped copy of your `logging` folder is the best way to submit logs. You may check the 'Log All Screenshots' box to force this behavior in most areas that may concern the devs.

## Requesting a new feature

If you would like to request a new feature, please visit the [discussion forum](https://github.com/Andrewthe13th/Inventory_Kamera/discussions) before opening a new feature request using [Inventory Kamera feature request form](https://github.com/Andrewthe13th/Inventory_Kamera/discussions/new?category=ideas-or-feature-requests).

Alternatively, the [Inventory Kamera Discord](https://discord.gg/zh56aVWe3U) is has a feature request channel.

## Asking a question

General questions? Start by looking for similar questions in the [Inventory Kamera discussion forum](https://github.com/Andrewthe13th/Inventory_Kamera/discussions) OR the [Inventory Kamera Discord](https://discord.gg/zh56aVWe3U).

If you have a question that doesn't have a thread, create a new [general](https://github.com/Andrewthe13th/Inventory_Kamera/discussions/new?category=general) or [Q&A](https://github.com/Andrewthe13th/Inventory_Kamera/discussions/new?category=q-a) thread.

## Frequently Asked Questions (FAQ)

#### Can Inventory Kamera get me banned?

According to [HoYoverse's response to Script, Plug-In, and Third-Party Software](https://genshin.hoyoverse.com/en/news/detail/5763), we believe, no.

The scanner does not provide any exploits or game progression. It only takes screenshots of a portion of the game window, processes the image and exports the data. The account will still be yours to keep. We do not provide any account exchanges or primogem top-ups. Game interaction is limited to only menu navigation.

In addition, we have not received any warnings about the application development.
However, that does not mean it will stay that way forever! We are at the mercy of HoYoverse.

#### What Languages does Inventory Kamera support?

Inventory Kamera only supports _**English**_ currently. This limitation is due to only having a trained dataset available for the English language. The in-game "Game Langugae" option must be set to Engish. Scanning efficiency is not guaranteed for other languages. More languages may be implemented in the future but there are no current plans.

#### I need help! Where can I go?

Help may be found in the [Inventory Kamera discussion forum](https://github.com/Andrewthe13th/Inventory_Kamera/discussions) or in the [Inventory Kamera Discord](https://discord.gg/zh56aVWe3U).

## License

- This project is distributed under the [MIT license](LICENSE).
- This project uses third-party libraries and other resources that may be distributed under other licenses.

---

All rights reserved by © Cognosphere Pte. Ltd. This project is not affiliated with nor endorsed by HoYoverse. Genshin Impact™ and other properties belong to their respective owners.
