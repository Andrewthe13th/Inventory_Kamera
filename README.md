# Inventory Kamera - A Genshin Data Scanner
An OCR Scanner that scans your Characters, Weapons, Artifacts, Materials and Character Development items in your Inventory.

This scanner exports in GOOD formatting. This format is supported in a variety of online tools and websites like [Genshin Optimizer](https://frzyc.github.io/genshin-optimizer/#/) and [SEELIE.me](https://seelie.me/).


Currently Inventory Kamera English as the in-game language, 16:9 and 16:10 game window aspect ratios, as well as the English (GenshinImpact.exe) and Chinese (YuanShen.exe) clients. Please read the instructions below for more information.



## How to use the Scanner
1. Make sure that you already have [Microsoft Visual C++ 2015-2022 Redistributable x86 and x64 packages](https://docs.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022) installed on your device.
2. [Download the program](https://github.com/Andrewthe13th/Inventory_Kamera/releases) and unzip the downloaded file.
3. Open the folder, and double click `InventoryKamera.exe` to start up the scanner.
    - This will likely prompt you for Security or/and User Access. Allow access to continue.
4. Open Genshin Impact > Log In > and click 'Start'. 
5. Press `ESC` to open Paimon menu.
6. Go to `Settings` (cog button) and set to these setup.
   - Under `Languages`, set Game Language to `English`.
   - Under `Graphics`, set Display Mode to any windowed resolution that is 16:9 or 16:10. [Here](https://andrew.hedges.name/experiments/aspect_ratio/) is a website that you can use to determine the aspect ratio of a given resoltuion. Please make sure the entire window is visible on the screen!
   - Under `Controls`, set Control Type to `Keyboard`. If you have rebound the inventory key or character screen key (B and C respectively), you may choose to rebind them in game or in Inventory Kamera.
7. Exit `Settings` and leave the game in Paimon Menu
8. Go back to the scanner application, then click SCAN.
   - (optional) Configure which sections to scan
   - (optional) Configure weapon and artifact rarity filters
   - (optional) Set the scanner delays to improve performance (default : 0ms)
   - (optional) Set the export destination in File Location
9. You can stop the scanner at any time by pressing `ENTER` key. However, anything scanned will not be exported.

**NOTE**: Please **do not** use your mouse or keyboard while scanning as Inventory Kamera simulates their inputs to interact with Genshin. 

## Updating for new game versions

Inventory Kamera uses lists of valid items and characters to assist with text recognition. These lists are kept local in the `inventorylists` folder. These lists must be updated with every new version of the game and can be updated both automatically or manually.

### **Updating Automatically:** 
Can be done easily with the `Update Lookup Tables` under `options`. Checking the `Create New Lists` option will force all lists to be created from scratch.

### **Updating Manually:**
All lists, with the exception of characters, are kept in a simple key:value JSON-readable formats. 'value' is the name of an item in [PascalCase](https://en.wikipedia.org/wiki/Naming_convention_(programming)#Examples_of_multiple-word_identifier_formats) and 'key' is whatever 'value' is only in all lowercase. `materialscomplete.json` is the combination of `devmaterials.json` and `materials.json` so updating either of those requires and update to `materialscomplete.json`  as well. The format for manually updating characters is slightly different. The key for a character is still the lowercase version of the character's name in PascalCase. The value is of the following format:

```
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

## Can this get me banned?
 According to [miHoYo's response to Script, Plug-In, and Third-Party Software](https://genshin.mihoyo.com/en/news/detail/5763), we believe not. 
- This scanner does not exploit the game. It only takes pictures of the game window and navigates menus.
- This scanner does not provide artificial in-game progress.
- This scanner does nothing to provide account selling/exchanging.
- Doesn't provide Top-up Primogems. 

We have not heard of any reports of users nor have we being directly warned or banned for using this application. However, that does not mean it will stay that way forever. We are at the mercy of miHoYo.

## Contribute or Report an Issue
If you ran through any issue with our scanner, please [report your issue here](https://github.com/Andrewthe13th/Inventory_Kamera/issues).

Please be descriptive when describing your issue. Helpful information includes:

* Error output including message and first few lines of stack trace
* Window Resolution
* One or two screenshots that might have been generated by Inventory Kamera. These can be found in their respective subfolders under `logging` in the application's folder. These screenshots are not generated for every error.




**NOTES**:
* Before sending an issue, please check that there are no other similar issues already open. If a similar issue is open, consider reacting to that issue's first comment with an emoji instead. Duplicate issues will likely be closed and redirected to another thread of the similar issue unless sufficienty different.
* Screenshots are usually only generated when Inventory Kamera cannot determine how many artifacts/weapons/materials can be found on the screen or when a weapon/artifact was scanned and found to be not valid. **Please check the `logging` folder for screenshots to upload. These can be very helpful in debugging your issue. You do not need to upload all screenshots from each folder.**


## License
* This project is under the [MIT](LICENSE.md) license.
* This project uses third-party libraries or other resources that may be
distributed under [different licenses](THIRD-PARTY-NOTICES.md).

---

All rights reserved by © miHoYo Co., Ltd. This project is not affiliated nor endorsed by miHoYo. Genshin Impact™ and other properties belong to their respective owners.
