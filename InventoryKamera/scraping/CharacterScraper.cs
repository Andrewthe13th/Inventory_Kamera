using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using Accord.Imaging;

namespace InventoryKamera
{
	public static class CharacterScraper
	{
		private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		public static void ScanCharacters(ref List<Character> Characters)
		{
			int viewed = 0;
			string first = null;
			HashSet<string> scanned = new HashSet<string>();

			UserInterface.ResetCharacterDisplay();

			while (true)
			{
				var character = ScanCharacter(first);
				if (Characters.Count > 0 && character.Name == Characters.ElementAt(0).Name) break;
				if (character.IsValid())
				{
					if (!scanned.Contains(character.Name))
					{
						Characters.Add(character);
						UserInterface.IncrementCharacterCount();
						Logger.Info("Scanned {0} successfully", character.Name);
						if (Characters.Count == 1) first = character.Name;
					}
					else
                    {
						Logger.Info("Prevented {0} duplicate scan", character.Name);
                    }
				}
                else 
				{
					string error = "";
					if (!character.HasValidName()) error += "Invalid character name\n";
					if (!character.HasValidLevel()) error += "Invalid level\n";
					if (!character.HasValidElement()) error += "Invalid element\n";
					if (!character.HasValidConstellation()) error += "Invalid constellation\n";
					if (!character.HasValidTalents()) error += "Invalid talents\n";
					Logger.Error("Failed to scan character\n" + error + character);
				}

				Navigation.SelectNextCharacter();
				UserInterface.ResetCharacterDisplay();

				if (++viewed > 3 && Characters.Count < 1) break;
			}

            // Childe passive buff fix
            foreach (var character in Characters.Take(4))
            {
				if (character.Name.ToLower() == "tartaglia")
                {
					for (int i = 0; i < Characters.Count; i++)
					{
						Characters[i].Talents["auto"] -= 1;
					}
					Logger.Info("Tartaglia in on-field party, applied auto attack fix.");
					break;
				}
            }
		}

		private static Character ScanCharacter(string firstCharacter)
		{
			var character = new Character();
			Navigation.SelectCharacterAttributes();
			string name = null;
			string element = null;

			// Scan the Name and element of Character. Attempt 75 times max.
			ScanNameAndElement(ref name, ref element);

			if (string.IsNullOrWhiteSpace(name))
			{
				if (string.IsNullOrWhiteSpace(name)) UserInterface.AddError("Could not determine character's name");
				if (string.IsNullOrWhiteSpace(element)) UserInterface.AddError("Could not determine character's element");
				return character;
			}

			character.Name = name;
			character.Element = element;

			// Check if character was first scanned
			if (name != firstCharacter)
			{
				bool ascended = false;
				// Scan Level and ascension
				int level = ScanLevel(ref ascended);
				if (level == -1)
				{
					UserInterface.AddError($"Could not determine {name}'s level");
					return character;
				}
				character.Level = level;
				character.Ascended = ascended;

				Logger.Info("{name:l} Level: {level:l}", character.Name, character.Level);
				Logger.Info("{name:l} Ascended: {ascended:l}", character.Name, character.Ascended);

				// Scan Experience
				//experience = ScanExperience();
				//Navigation.SystemRandomWait(Navigation.Speed.Normal);

				// Scan Constellation
				Navigation.SelectCharacterConstellation();
				character.Constellation = ScanConstellations();
				Logger.Info("{name:l} Constellation: {constellation:l}", character.Name, character.Constellation);
				Navigation.SystemWait(Navigation.Speed.Normal);

				// Scan Talents
				Navigation.SelectCharacterTalents();
				character.Talents = ScanTalents(name);
				Logger.Info("{name:l} Talents: {talents:l}", character.Name, character.Talents);
				Navigation.SystemWait(Navigation.Speed.Normal);

				// Scale down talents due to constellations
				if (character.Constellation >= 3)
				{
					if (Scraper.Characters.ContainsKey(name.ToLower()))
					{
						// get talent if character
						if (character.Constellation >= 5)
						{
							character.Talents["skill"] -= 3;
							character.Talents["burst"] -= 3;
						}
						else if ((string)Scraper.Characters[name.ToLower()]["ConstellationOrder"][0] == "skill")
						{
							character.Talents["skill"] -= 3;
						}
						else
						{
							character.Talents["burst"] -= 3;
						}
					}
					else
						return character;
				}

				character.WeaponType = Scraper.Characters[name.ToLower()]["WeaponType"].ToObject<WeaponType>();

				return character;
			}
			Logger.Info("Repeat character {0} detected. Finishing character scan...", name);
			return character;
		}

		public static string ScanMainCharacterName()
		{
			var xReference = 1280.0;
			var yReference = 720.0;
			if (Navigation.GetAspectRatio() == new Size(8, 5))
			{
				yReference = 800.0;
			}

			RECT region = new RECT(
				Left:   (int)(185 / xReference * Navigation.GetWidth()),
				Top:    (int)(26  / yReference * Navigation.GetHeight()),
				Right:  (int)(460 / xReference * Navigation.GetWidth()),
				Bottom: (int)(60  / yReference * Navigation.GetHeight()));

			Bitmap nameBitmap = Navigation.CaptureRegion(region);

			//Image Operations
			Scraper.SetGamma(0.2, 0.2, 0.2, ref nameBitmap);
			Scraper.SetInvert(ref nameBitmap);
			Bitmap n = Scraper.ConvertToGrayscale(nameBitmap);

			UserInterface.SetNavigation_Image(nameBitmap);

			string text = Scraper.AnalyzeText(n).Trim();
			if (text != "")
			{
				// Only keep a-Z and 0-9
				text = Regex.Replace(text, @"[\W_]", string.Empty).ToLower();

				// Only keep text up until first space
				text = Regex.Replace(text, @"\s+\w*", string.Empty);

				UserInterface.SetMainCharacterName(text);
			}
			else
			{
				UserInterface.AddError(text);
			}
			n.Dispose();
			nameBitmap.Dispose();
			return text;
		}

		private static void ScanNameAndElement(ref string name, ref string element)
		{
			int attempts = 0;
			int maxAttempts = 75;
			Rectangle region = new RECT(
				Left:   (int)( 85  / 1280.0 * Navigation.GetWidth() ),
				Top:    (int)( 10  / 720.0 * Navigation.GetHeight() ),
				Right:  (int)( 305 / 1280.0 * Navigation.GetWidth() ),
				Bottom: (int)( 55  / 720.0 * Navigation.GetHeight() ));

			do
			{
				Navigation.SystemWait(Navigation.Speed.Fast);
				using (Bitmap bm = Navigation.CaptureRegion(region))
				{
					Bitmap n = Scraper.ConvertToGrayscale(bm);
					Scraper.SetThreshold(110, ref n);
					Scraper.SetInvert(ref n);

					n = Scraper.ResizeImage(n, n.Width * 2, n.Height * 2);
					string block = Scraper.AnalyzeText(n, Tesseract.PageSegMode.Auto).ToLower().Trim();
					string line = Scraper.AnalyzeText(n, Tesseract.PageSegMode.SingleLine).ToLower().Trim();

					// Characters with wrapped names will not have a slash
					string nameAndElement = line.Contains("/") ? line : block;

					if (nameAndElement.Contains("/"))
					{
						var split = nameAndElement.Split('/');

						// Search for element and character name in block

						// Long name characters might look like
						// <Element>   <First Name>
						// /           <Last Name>
						element = !split[0].Contains(" ") ? Scraper.FindElementByName(split[0].Trim()) : Scraper.FindElementByName(split[0].Split(' ')[0].Trim());

						// Find character based on string after /
						// Long name characters might search by their last name only but it'll still work.
						name = Scraper.FindClosestCharacterName(Regex.Replace(split[1], @"[\W]", string.Empty));
						if (name == "Traveler")
						{
							foreach (var item in from item in Scraper.Characters
												 where item.Value["GOOD"].ToString() == "Traveler"
												 select item)
							{
								name = item.Key;
							}
						}
					}
					n.Dispose();

					if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(element))
					{
						UserInterface.SetCharacter_NameAndElement(bm, name, element);
						return;
					}
				}
				attempts++;
				Navigation.SystemWait(Navigation.Speed.Normal);
			} while (( string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(element) ) && ( attempts < maxAttempts ));
			name = null;
			element = null;
		}

		private static int ScanLevel(ref bool ascended)
		{
			int level = -1;

			var xRef = 1280.0;
			var yRef = 720.0;
			if (Navigation.GetAspectRatio() == new Size(8, 5))
			{
				yRef = 800.0;
			}

			Rectangle region =  new RECT(
				Left:   (int)( 960  / xRef * Navigation.GetWidth() ),
				Top:    (int)( 135  / yRef * Navigation.GetHeight() ),
				Right:  (int)( 1125 / xRef * Navigation.GetWidth() ),
				Bottom: (int)( 163  / yRef * Navigation.GetHeight() ));

			do
			{
				Bitmap bm = Navigation.CaptureRegion(region);

				bm = Scraper.ResizeImage(bm, bm.Width * 2, bm.Height * 2);
				Bitmap n = Scraper.ConvertToGrayscale(bm);
				Scraper.SetInvert(ref n);
				Scraper.SetContrast(30.0, ref bm);

				string text = Scraper.AnalyzeText(n).Trim();

				text = Regex.Replace(text, @"(?![0-9/]).", string.Empty);
				if (text.Contains("/"))
				{
					var values = text.Split('/');
					if (int.TryParse(values[0], out level) && int.TryParse(values[1], out int maxLevel))
					{
						maxLevel = (int)Math.Round(maxLevel / 10.0, MidpointRounding.AwayFromZero) * 10;
						ascended = 20 <= level && level < maxLevel;
						UserInterface.SetCharacter_Level(bm, level, maxLevel);
						n.Dispose();
						bm.Dispose();
						return level;
					}
					n.Dispose();
					bm.Dispose();
				}
				Navigation.SystemWait(Navigation.Speed.Normal);
			} while (level == -1);

			return -1;
		}

		private static int ScanExperience()
		{
			int experience = 0;

			int xOffset = 1117;
			int yOffset = 151;
			Bitmap bm = new Bitmap(90, 10);
			Graphics g = Graphics.FromImage(bm);
			int screenLocation_X = Navigation.GetPosition().Left + xOffset;
			int screenLocation_Y = Navigation.GetPosition().Top + yOffset;
			g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

			//Image Operations
			bm = Scraper.ResizeImage(bm, bm.Width * 6, bm.Height * 6);
			//Scraper.ConvertToGrayscale(ref bm);
			//Scraper.SetInvert(ref bm);
			Scraper.SetContrast(30.0, ref bm);

			string text = Scraper.AnalyzeText(bm);
			text = text.Trim();
			text = Regex.Replace(text, @"(?![0-9\s/]).", string.Empty);

			if (Regex.IsMatch(text, "/"))
			{
				string[] temp = text.Split('/');
				experience = Convert.ToInt32(temp[0]);
			}
			else
			{
				Debug.Print("Error: Found " + experience + " instead of experience");
				UserInterface.AddError("Found " + experience + " instead of experience");
			}

			return experience;
		}

		private static int ScanConstellations()
		{
			double yReference = 720.0;
			int constellation;

			if (Navigation.GetAspectRatio() == new Size(8, 5))
			{
				yReference = 800.0;
			}

			Rectangle constActivate =  new RECT(
				Left:   (int)( 70 / 1280.0 * Navigation.GetWidth() ),
				Top:    (int)( 665 / 720.0 * Navigation.GetHeight() ),
				Right:  (int)( 100 / 1280.0 * Navigation.GetWidth() ),
				Bottom: (int)( 695 / 720.0 * Navigation.GetHeight() ));

			for (constellation = 0; constellation < 6; constellation++)
			{
				// Select Constellation
				int yOffset = (int)( ( 180 + ( constellation * 75 ) ) / yReference * Navigation.GetHeight() );

				if (Navigation.GetAspectRatio() == new Size(8, 5))
				{
					yOffset = (int)( ( 225 + ( constellation * 75 ) ) / yReference * Navigation.GetHeight() );
				}

				Navigation.SetCursor((int)( 1130 / 1280.0 * Navigation.GetWidth() ), yOffset);
				Navigation.Click();

				var pause = constellation == 0 ? 700 : 550;
				Navigation.SystemWait(pause);

				// Grab Color
				using (Bitmap region = Navigation.CaptureRegion(constActivate))
				{
					// Check a small region next to the text "Activate"
					// for a mostly white backround
					ImageStatistics statistics = new ImageStatistics(region);
					if (statistics.Red.Mean >= 190 && statistics.Green.Mean >= 190 && statistics.Blue.Mean >= 190)
					{
						break;
					}
				}
			}

			Navigation.sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.ESCAPE);
			UserInterface.SetCharacter_Constellation(constellation);
			return constellation;
		}

		private static Dictionary<string, int> ScanTalents(string name)
		{
			var talents = new Dictionary<string, int>
			{
				{ "auto" , -1 },
				{ "skill", -1 },
				{ "burst", -1 }
			};

			int specialOffset = 0;

			// Check if character has a movement talent like
			// Mona or Ayaka
			if (name.Contains("Mona") || name.Contains("Ayaka")) specialOffset = 1;

			var xRef = 1280.0;
			var yRef = 720.0;

			if (Navigation.GetAspectRatio() == new Size(8, 5)) yRef = 800.0;

			Rectangle region =  new RECT(
				Left:   (int)( 160 / xRef * Navigation.GetWidth() ),
				Top:    (int)( 116 / yRef * Navigation.GetHeight() ),
				Right:  (int)( 225 / xRef * Navigation.GetWidth() ),
				Bottom: (int)( 141 / yRef * Navigation.GetHeight() ));

			for (int i = 0; i < 3; i++)
			{
				string talent;
				// Change y-offset for talent clicking
				int yOffset = (int)( 110 / yRef * Navigation.GetHeight() ) + ( i + ( ( i == 2 ) ? specialOffset : 0 ) ) * (int)(60 / yRef * Navigation.GetHeight() );

				Navigation.SetCursor((int)(1130 / xRef * Navigation.GetWidth()), yOffset);
				Navigation.Click();
				int pause = i == 0 ? 700 : 550;
				Navigation.SystemWait(pause);
                switch (i)
                {
					default:
						talent = "auto";
						break;
					case 1:
						talent = "skill";
						break;
					case 2:
						talent = "burst";
						break;
                }

                while (talents[talent] < 1 || talents[talent] > 15)
				{
					Bitmap talentLevel = Navigation.CaptureRegion(region);

					talentLevel = Scraper.ResizeImage(talentLevel, talentLevel.Width * 2, talentLevel.Height * 2);

					Bitmap n = Scraper.ConvertToGrayscale(talentLevel);
					Scraper.SetContrast(60, ref n);
					Scraper.SetInvert(ref n);

					string text = Scraper.AnalyzeText(n).Trim();
					text = Regex.Replace(text, @"\D", string.Empty);

					if (int.TryParse(text, out int level))
					{
						if (level >= 1 && level <= 15)
						{
							talents[talent] = level;
							UserInterface.SetCharacter_Talent(talentLevel, text, i);
						}
					}

					n.Dispose();
					talentLevel.Dispose();
				}
			}

			Navigation.sim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.ESCAPE);
			return talents;
		}
	}
}