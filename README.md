# Inventory Kamera - A Genshin Data Scanner
Fan-made Genshin Impact tool that scans your Characters, Weapons, Artifacts, Materials, and Character Development items in your Inventory using the OCR technique.

This scanner exports in `.GOOD`, a JSON-based exporting format, which allows you to use it with compatible online Genshin Impact tools. These tools include artifact optimizing tools including [Genshin Optimizer](https://frzyc.github.io/genshin-optimizer/#/), [SEELIE.me](https://seelie.me/), and [Aspirine's Genshin Impact Calculator](https://genshin.aspirine.su/).

Inventory Kamera supports both International (GenshinImpact.exe) and Mainland China (YuanShen.exe) launchers. Please read the following instruction on how you need to set up the scanner.

## Table of Contents
- Using Inventory Kamera
   - [Setting up Inventory Kamera](#set-up-the-inventory-kamera)
   - [Setting up Genshin Impact](#set-up-with-your-genshin-impact)
   - [Configuring Inventory Kamera settings](#how-to-configure-inventory-kamera)
- Updating Inventory Kamera database
   - [How to update database](#updating-for-new-game-versions)
- Repository
   - [Report an Issue / Bug](#report-an-issue--requesting-new-feature)
- Frequently Asked Questions (FAQ)
   - [Can Inventory Kamera get me banned?](#can-inventory-kamera-get-me-banned)
- Legal
   - [Licenses](#license)


## How to use Inventory Kamera
To use Inventory Kamera, please set up and configures Inventory Kamera as follows:

### Set up the Inventory Kamera
1. [Download the latest version](https://github.com/Andrewthe13th/Inventory_Kamera/releases) of the program (Not the source code zip!) and unzip the downloaded file.
2. Open the folder, and double click `InventoryKamera.exe` to start up the scanner. This will likely prompt you for Security or/and User Access. Allow access to continue.
3. Make sure that you already have [Microsoft Visual C++ 2015-2022 Redistributable x86 and x64 packages](https://docs.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022) installed on your PC.

### Set up with your Genshin Impact
1. Log in to Genshin Impact and click start.
2. Open [Paimon menu](https://genshin-impact.fandom.com/wiki/Paimon_Menu). (Default shortcut: `ESC`)
3. Go to `Settings` (Cog Icon ⚙) and set it to these settings:
   - Under `Languages`, set *Game Language* to English.
   - Under `Graphics`, set *Display Mode* to any windowed resolution that is 16:9 or 16:10 ratio. Make sure that the entire game window is visible on the screen.
     - Example of 16:9 includes 1920x1080 (Full HD), 3840x2160 (4K), 2560x1440 (2K).
     - Example of 16:10 includes 1920x1200.
     - Is this 16:9 or 16:10? [Find out here](https://andrew.hedges.name/experiments/aspect_ratio/).
   - Under `Controls`, set *Control Type* to Keyboard.
      - If you have rebound the inventory key (default: B) or character screen key (default: C), either revert your binding to default or set up new key binding in Inventory Kamera.

### How to Configure Inventory Kamera
Before starting the scanner, you can (optionally) choose to edit these following configurations:

- Configure which category (Weapons, Artifacts, Characters, Items) wanted to be scanned.
- Configure minimum weapon and artifact rarity to be scanned.
- Set the scanner delays to reduce lag-induced scan error (default: 0ms)
- Set the file export destination in the File Directory

### How to Run Inventory Kamera
Starts the Inventory Kamera scans by **leaving the game screen with the Paimon Menu stays open** and click 'Scan' to start scanning.

> **Warning**<br>
> While scanning, **do not use your mouse or keyboard**. We use your keyboard and mouse to automate artifact scanning.
> 
> If you like to terminate the scan, press the `ENTER` button at any time. However, the application will not output any scanned results.

## Updating for new game versions
Inventory Kamera uses lists of valid items and characters to assist with text recognition. These lists are kept local in the `inventorylists` folder. These lists must be updated with every new version of the game and can be updated both automatically or manually.

### Updating Automatically
Can be done easily with the `Update Lookup Tables` under `options`. Checking the `Create New Lists` option will force all lists to be created from scratch.

### Updating Manually
All lists, with the exception of characters, are kept in a simple key:value JSON-readable formats. 'value' is the name of an item in [PascalCase](https://en.wikipedia.org/wiki/Naming_convention_(programming)#Examples_of_multiple-word_identifier_formats) and 'key' is whatever 'value' is only in all lowercase. `materialscomplete.json` is the combination of `devmaterials.json` and `materials.json` so updating either of those requires and update to `materialscomplete.json`  as well. The format for manually updating characters is slightly different. The key for a character is still the lowercase version of the character's name in PascalCase. The value is of the following format:

``` json
{
    "GOOD": "SangonomiyaKokomi",
    "ConstellationOrder": [
        "burst",
        "skill"
    ],
    "WeaponType": 4
}
```

The character's name is as it appears in the party menu on the right side of the in-world UI or the character's menu screen in the top left corner. The constellation order depends on which talent the third constellation upgrades for each character. The weapon type values are as follows:

0 = Sword, 1 = Claymore, 2 = Polearm, 3 = Bow, 4 = Catalyst

Consider using [a JSON text validator](https://jsonlint.com/) after following this manual method.


## Report an Issue / Requesting new Feature
If you ran into a problem with our scanner (eg. bug, app crash, invalid export format) or want to request new feature, please [create your issue here](https://github.com/Andrewthe13th/Inventory_Kamera/issues/new/choose). Choose from one of the four templates that best suits your request **and try to fill it out as much as possible.** Itself, along with the evidence, will greatly speed up the bug fix process(es).

- If you have questions related to InventoryKamera, feel free to ask! There are no stupid questions here.
- Requesting a Feature Enhancement template is also available. We are also open to Pull Requests and discussions.
- If you are still unsure that [this is a bug or a feature](https://www.quora.com/What-is-the-difference-between-a-bug-and-a-feature), there is a high chance that this is a bug. Report it.

> **Note**<br>
> Before sending an issue, **please [check that there is no other similar issue(s)](https://github.com/Andrewthe13th/Inventory_Kamera/issues?q=is%3Aissue), especially ones that are still opened.**<br>
> Start by leaving a reaction emoji to that issue, and start discussing. For introverts, you can choose to *subscribe* to that issue by clicking 'Subscribe' in the Notifications section.

> **Note**<br>
> Requires GitHub Account to create an issue

### About writing a new Issue
We would **love to have Screenshots (especially video recordings) and Error Log as evidence**. These can be very helpful in debugging your issue. Add it to the issue by drag-and-drop or attatch the file to the issue template. 

When Inventory Kamera cannot determine how many artifacts/weapons/materials on the screen or scanned content are found to be invalid, screenshots are automatically generated. These images are located in `logging` folder, and we need that too (if you have any).


## Frequently Asked Questions (FAQ)
#### Can Inventory Kamera get me banned?
According to HoYoverse's [response to Script, Plug-In, and Third-Party Software](https://genshin.hoyoverse.com/en/news/detail/5763), we believe not. 

Since the scanner does not provide any exploits or game progress. It takes a portion of the game, locally process the image, and lets you control your data. And the account is still yours to keep. We also not provide any account exchanges or topping up Primogem.

In addition, we have not received warnings about the application's development. 
However, that does not mean it will stay that way forever; We are at the mercy of HoYoverse.

## License
- This project is under the [MIT license](LICENSE).
- This project uses third-party libraries or other resources that may be
distributed under different licenses.

---

All rights reserved by © Cognosphere Pte. Ltd. This project is not affiliated with, nor endorsed by HoYoverse. Genshin Impact™ and other properties belong to their respective owners.
