using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using Accord.Imaging;

namespace GenshinGuide
{
	public static class CharacterScraper
	{
		private static string firstCharacterName = null;

		public static List<Character> ScanCharacters()
		{
			List<Character> characters = new List<Character>();

			// first character name is used to stop scanning characters
			int characterCount = 0;
			while (ScanCharacter(out Character character) || characterCount < 4)
			{
				if (character.IsValid())
				{
					characters.Add(character);
					UserInterface.IncrementCharacterCount();
					characterCount++;
				}
				Navigation.SelectNextCharacter();
				UserInterface.ResetCharacterDisplay();
			}

			return characters;
		}
		
		private static bool ScanCharacter(out Character character)
		{
			string name = null; string element = null;
			bool ascension = false;
			int experience = 0;

			// Scan the Name and element of Character. Attempt 20 times max.
			ScanNameAndElement(ref name, ref element);

			if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(element))
			{
				UserInterface.AddError("Could not determine character's name or element");
				goto Fail;
			}


			// Check if character was first scanned
			if (name != firstCharacterName)
			{
				if (string.IsNullOrEmpty(firstCharacterName))
					firstCharacterName = name;
				
				// Scan Level and ascension
				Navigation.SelectCharacterAttributes();
				int level = ScanLevel(ref ascension);
				if (level == -1)
				{
					UserInterface.AddError($"Could not determine {name}'s level");
					goto Fail;
				}

				// Scan Experience
				//experience = ScanExperience();
				Navigation.SystemRandomWait(Navigation.Speed.Normal);

				// Scan Constellation
				Navigation.SelectCharacterConstellation();
				int constellation = ScanConstellations();
				Navigation.SystemRandomWait(Navigation.Speed.Normal);

				// Scan Talents
				Navigation.SelectCharacterTalents();
				int[] talents = ScanTalents(name);
				Navigation.SystemRandomWait(Navigation.Speed.Normal);

				// Scale down talents due to constellations
				if (constellation >= 3)
				{
					if (Scraper.characterTalentConstellationOrder.ContainsKey(name))
					{
						// get talent if character
						string talent = Scraper.characterTalentConstellationOrder[name][0];
						if (constellation >= 5)
						{
							talents[1] -= 3;
							talents[2] -= 3;
						}
						else if (talent == "skill")
						{
							talents[1] -= 3;
						}
						else
						{
							talents[2] -= 3;
						}
					}
					else
						goto Fail;
				}

				character = new Character(name, element, level, ascension, experience, constellation, talents);
				return true;
			}
		Fail: 
			character = new Character();
			return false;
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
			Scraper.SetContrast(40.0, ref n);

			UserInterface.SetNavigation_Image(n);

			string text = Scraper.AnalyzeText(n).Trim();
			if (text != "")
			{
				// Only keep a-Z and 0-9
				text = Regex.Replace(text, @"[\W_]", "").ToLower();

				// Only keep text up until first space
				text = Regex.Replace(text, @"\s+\w*", "");
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
			int maxAttempts = 50;
			Rectangle region = new RECT(
				Left:   (int)( 85  / 1280.0 * Navigation.GetWidth() ),
				Top:    (int)( 10  / 720.0 * Navigation.GetHeight() ),
				Right:  (int)( 305 / 1280.0 * Navigation.GetWidth() ),
				Bottom: (int)( 55  / 720.0 * Navigation.GetHeight() ));

			do
			{
				Navigation.SystemRandomWait(Navigation.Speed.Fast);
				using (Bitmap bm = Navigation.CaptureRegion(region))
				{
					Bitmap n = Scraper.ConvertToGrayscale(bm);
					Scraper.SetThreshold(110, ref n);
					Scraper.SetInvert(ref n);

					n = Scraper.ResizeImage(n, n.Width * 2, n.Height * 2);

					string text = Scraper.AnalyzeText(n).ToLower().Trim();

					if (text.Contains("/"))
					{
						var split = text.Split('/');


						// search for element in block
						element = Scraper.FindElementByName(split[0].Trim());


						// strip each char from name until found in dictionary
						name = Scraper.FindClosestCharacterName(split[1].Trim().Replace(" ", ""));
						if (!string.IsNullOrEmpty(name) && ! string.IsNullOrEmpty(element))
						{
							UserInterface.SetCharacter_NameAndElement(bm, name, element);
							return;
						}
					}
					n.Dispose();
					
				}
				attempts++;
				Navigation.SystemRandomWait(Navigation.Speed.Faster);
			} while ((string.IsNullOrEmpty(name) || string.IsNullOrEmpty(element)) && (attempts < maxAttempts));
			name = null;
			element = null;
		}

		private static int ScanLevel(ref bool ascension)
		{
			int level = -1;
			int attempt = 0;
			int maxAttempts = 50;

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
				Navigation.SystemRandomWait(Navigation.Speed.Normal);
				Bitmap bm = Navigation.CaptureRegion(region);

				bm = Scraper.ResizeImage(bm, bm.Width * 2, bm.Height * 2);
				Bitmap n = Scraper.ConvertToGrayscale(bm);
				Scraper.SetInvert(ref n);
				Scraper.SetContrast(30.0, ref bm);


				string text = Scraper.AnalyzeText(n).Trim();

				text = Regex.Replace(text, @"(?![0-9/]).", "");
				if (text.Contains("/"))
				{

					var values = text.Split('/');
					if (int.TryParse(values[0], out level) && int.TryParse(values[1], out int maxLevel))
					{
						ascension = level < maxLevel;
						UserInterface.SetCharacter_Level(bm, level, maxLevel);
						n.Dispose();
						bm.Dispose();
						return level;
					}
					n.Dispose();
					bm.Dispose();
				}
			} while (level == -1 && attempt < maxAttempts);

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
			text = Regex.Replace(text, @"(?![0-9\s/]).", "");

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

				
				Navigation.SetCursorPos(Navigation.GetPosition().Left + (int)(1130 / 1280.0 * Navigation.GetWidth()),
										Navigation.GetPosition().Top + yOffset);
				Navigation.sim.Mouse.LeftButtonClick();


				Navigation.Speed speed = constellation == 0 ? Navigation.Speed.Normal : Navigation.Speed.Fast;
				Navigation.SystemRandomWait(speed);



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

		private static int[] ScanTalents(string name)
		{
			int[] talents = {-1,-1,-1};

			
			int specialOffset = 0;

			// Check if character has a movement talent like
			// Mona or Ayaka
			if (name == "mona" || name == "ayaka")
			{
				specialOffset = 1;
			}

			var xRef = 1280.0;
			var yRef = 720.0;

			if (Navigation.GetAspectRatio() == new Size(8, 5))
			{
				yRef = 800.0;
			}

			Rectangle region =  new RECT(
				Left:   (int)( 160 / xRef * Navigation.GetWidth() ),
				Top:    (int)( 115 / yRef * Navigation.GetHeight() ),
				Right:  (int)( 225 / xRef * Navigation.GetWidth() ),
				Bottom: (int)( 140 / yRef * Navigation.GetHeight() ));

			for (int i = 0; i < 3; i++)
			{
				// Change y-offset for talent clicking
				int yOffset = (int)( 110 / yRef * Navigation.GetHeight() ) + ( i + ( ( i == 2 ) ? specialOffset : 0 ) ) * (int)(60 / yRef * Navigation.GetHeight() );

				Navigation.SetCursorPos(Navigation.GetPosition().Left + (int)(1130 / xRef * Navigation.GetWidth()), Navigation.GetPosition().Top + yOffset);
				Navigation.sim.Mouse.LeftButtonClick();
				Navigation.Speed speed = i == 0 ? Navigation.Speed.Normal : Navigation.Speed.Fast;
				Navigation.SystemRandomWait(speed);

				while (talents[i] < 1 || talents[i] > 15)
				{
					Bitmap talentLevel = Navigation.CaptureRegion(region);

					talentLevel = Scraper.ResizeImage(talentLevel, talentLevel.Width * 2, talentLevel.Height * 2);

					Bitmap n = Scraper.ConvertToGrayscale(talentLevel);
					Scraper.SetContrast(60, ref n);
					Scraper.SetInvert(ref n);


					string text = Scraper.AnalyzeText(n).Trim();
					text = Regex.Replace(text, @"\D", "");


					if (int.TryParse(text, out int level))
					{
						if (level >= 1 && level <= 15)
						{
							talents[i] = level;
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