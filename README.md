# Inventory Kamera - A Genshin Data Scanner
Fan-made Genshin Impact tool that scans your Characters, Weapons, Artifacts, Materials, and Character Development items in your Inventory using the OCR technique.

This scanner exports in `.GOOD`, a JSON-based exporting format, which allows you to use it with compatible online Genshin Impact tools. These tools include artifact optimizing tools including [Genshin Optimizer](https://frzyc.github.io/genshin-optimizer/#/), [SEELIE.me](https://seelie.me/), and [Aspirine's Genshin Impact Calculator](https://genshin.aspirine.su/).

Inventory Kamera supports both International (GenshinImpact.exe) and Mainland China (YuanShen.exe) launchers. Please read the following instruction on how you need to set up the scanner.

## How to use Inventory Kamera
To use Invetory Kamera, please set up and configures Inventory Kamera as follows:

### Set up the Inventory Kamera
1. [Download the latest version](https://github.com/Andrewthe13th/Inventory_Kamera/releases) of the program and unzip the downloaded file.
2. Open the folder, and double click `InventoryKamera.exe` to start up the scanner. This will likely prompt you for Security or/and User Access. Allow access to continue.
3. Make sure that you already have [Microsoft Visual C++ 2015-2022 Redistributable x86 and x64 packages](https://docs.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022) installed on your PC.

### Set up with your Genshin Impact
1. Open Genshin Impact 🡢 Log In 🡢 and click 'Start'.
2. Open the Paimon menu. (Default shortcut: `ESC`)
3. Go to `Settings` (Cog Icon ⚙) and set it to these settings:
   - Under `Languages`, set *Game Language* to English.
   - Under `Graphics`, set *Display Mode* to any windowed resolution that is 16:9 or 16:10 ratio. Make sure that the entire game window is visible on the screen.
     - Example of 16:9 includes 1920x1080 (Full HD), 3840x2160 (4K), 2560x1440 (2K).
     - Example of 16:10 includes 1920x1200.
     - Is this screen resolution a 16:9 or 16:10? [Click Here](https://andrew.hedges.name/experiments/aspect_ratio/).
   - Under `Controls`, set *Control Type* to Keyboard.
      - If you have rebound the inventory key (default: B) or character screen key (default: C), either revert your binding back to default or set up new key binding in Inventory Kamera.

### How to Configure Inventory Kamera
Before starting the scanner, you can (optionally) choose to edit these following configurations:

- Configure which category (Weapons, Artifacts, Characters, Items) wanted to be scanned.
- Configure minimum weapon and artifact rarity to be scanned.
- Set the scanner delays to reduce lag-induced scan error (default: 0ms)
- Set the file export destination in the File Directory

### How to Run Inventory Kamera
Starts the Inventory Kamera scans by **leaving the game screen with the Paimon Menu stays open** and click 'Scan' to start scanning.

**While scanning, do not use your mouse or keyboard**. We uses your keyboard and mouse to automate artifact scanning.

You can terminate the scanning process at any time by pressing `ENTER` button. However, the application will not output any scanned results.

## Updating for new game versions

Inventory Kamera uses lists of valid items and characters to assist with text recognition. These lists are kept local in the `inventorylists` folder. These lists must be updated with every new version of the game and can be updated both automatically or manually.

### **Updating Automatically:**
Can be done easily with the `Update Lookup Tables` under `options`. Checking the `Create New Lists` option will force all lists to be created from scratch.

### **Updating Manually:**
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

## Can Inventory Kamera get me banned?
According to miHoYo's [response to Script, Plug-In, and Third-Party Software](https://genshin.mihoyo.com/en/news/detail/5763), we believe not.
- This scanner does not exploit the game. It only takes pictures of the game window and navigates menus.
- This scanner does not provide artificial in-game progress.
- This scanner does nothing to provide account selling/exchanging.
- Doesn't provide Top-up Primogems.

We have neither heard of any reports nor received warnings of the application's development. However, that does not mean it will stay that way forever. We are at the mercy of miHoYo.

## Report an Issue
If you ran through any issue with our scanner, please [report your issue here](https://github.com/Andrewthe13th/Inventory_Kamera/issues). Please be descriptive and precise as much as much as possible while describing.

Helpful information includes:
- Window Resolution(s) that have been affected (eg. 1920x1080)
- affected InventoryKamera.exe version (eg. v1.2.3)
- affected Genshin Impact launcher and game version (eg. v2.6)
- One or two screenshots that might have been generated by Inventory Kamera.
  - These can be found in their respective subfolders under `logging` in the application's folder. These screenshots are not generated for every error.
- Error output including message and first few lines of stack trace.
- Everything you done to reproduce the following issue.

### Important Remarks
- Before sending an issue, **please check that there are no other similar issues already open.** If a similar issue is open, consider clicking 'Subscribe' or leave a reaction emoji to that issue. We are likely to ignore or/and close duplicate issues.
  - If you are not so sure about originality, please leave a link referencing possible duplicates. This speeds up our process and allow us to allocate valuable time resource on fixing bugs and add new features.
  - (You are required to have a GitHub account for creating and receiving notifications on the issue.)
- Screenshots are usually only generated when Inventory Kamera cannot determine how many artifacts/weapons/materials can be found on the screen or when a weapon/artifact was scanned and found to be not valid. **Please check the `logging` folder for screenshots to upload. These can be very helpful in debugging your issue. You do not need to upload all screenshots from each folder.**

## License
- This project is under the [MIT](LICENSE.md) license.
- This project uses third-party libraries or other resources that may be
distributed under [different licenses](THIRD-PARTY-NOTICES.md).

---

All rights reserved by © miHoYo Co., Ltd. This project is not affiliated with, nor endorsed by miHoYo. Genshin Impact™ and other properties belong to their respective owners.
