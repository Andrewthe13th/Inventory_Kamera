# Inventory Kamera - A Genshin Data Scanner

An OCR Scanner that scans your Characters, Weapons, Artifacts, Materials and Character Development items in your Inventory.</br>
The scanner supports multiple exporting file formats, including these following:

- `GOOD` .good (JSON) format for [Genshin Optimizer](https://frzyc.github.io/genshin-optimizer/#/) that include Characters, Weapons and Artifacts; and are widely used in many Genshin Impact fan-made projects and services, such as Discord bots.
- `Seelie` JSON format to be used with [SEELIE.me](https://seelie.inmagi.com/) (Might change later in the future updates)

Currently supports certain game language, screen size, scanning settings, and export options. Please follow the instructions on what you have to do.

## How to use the Scanner
1. Make sure that you already have [Microsoft Visual C++ Redistributable packages](https://docs.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022) installed on your device.
2. [Download the program (Inventory_Kamera_Vx.x.zip)](https://github.com/Andrewthe13th/Inventory_Kamera/releases/latest) and unzip the downloaded file.
3. Open the folder, and double click `InventoryKamera` to start up the scanner.
    - This might prompt you with Security or/and User Access prompt. Allow the access to continue.
5. Open Genshin Impact > Log In > and click 'Start'. 
6. Press `ESC` (or eqivalent keystroke) to open Paimon menu.
7. Go to `Settings` (cog button) and set to these setup.
   - Under `Languages`, set Game Language to `English`.
   - Under `Graphics`, set Display Mode to `1280x720 Windowed`.
   - Under `Controls`, set Control Type to `Keyboard`.
8. Exit `Settings` and leave the game in Paimon Menu
9. Go back to the scanner application, then click SCAN.
   - (optional) Set the scanner delays to improve performance (default : 0ms)
   - (optional) Set the export destination in File Location
10. You can stop the scanner at any time by pressing `ENTER` key
11. If there's an Error on the Error Log, copy it; understand it; explain it all in [Report an Issue](https://github.com/Andrewthe13th/Inventory_Kamera/issues)

NOTE: You will not be able to use your computer while the scan is taking place due to require the use of mouse and keyboard inputs.

### Can this get me banned?
 According to [miHoYo's response to Script, Plug-In, and Third-Party Software](https://genshin.mihoyo.com/en/news/detail/5763), I would say no. 
1. This scanner does not exploit the game. It just takes pictures of the game window.
2. This scanner does not provide artificial in-game progress.
3. This scanner does nothing to provide account selling/exchanging.
4. Doesn't provide Top-up Primogem. 

Also this scanner has random duration of pauses which makes the scanner prevent false positive by scanning too fast.
My account has been used to test this script and has yet to be banned or warned of the use of scripts. 

## Contribute or Report an Issue
If you ran through any issue with our scanner, please [Report an Issue here](https://github.com/Andrewthe13th/Inventory_Kamera/issues). 

NOTE: Before sending an issue, please check that there is no other open issue similar to yours by searching up the issue list.

## License
* This project is under the [MIT](LICENSE.md) license.
* This project uses third-party libraries or other resources that may be
distributed under [different licenses](THIRD-PARTY-NOTICES.md).

---

All rights reserved by © miHoYo Co., Ltd. This project is not affiliated nor endorsed by miHoYo. Genshin Impact™ and other properties belong to their respective owners.
