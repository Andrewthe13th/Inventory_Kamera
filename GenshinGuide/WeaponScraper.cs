using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GenshinGuide
{
	public static class WeaponScraper
	{
		public static List<Weapon> ScanWeapons(ref List<Weapon> equippedWeapon)
		{
			List<Weapon> weapons = new List<Weapon>();

			// Get Max weapons from screen
			int weaponCount = ScanWeaponCount();
			//int weaponCount = 29;
			int currentweaponCount = 0;
			int scrollCount = 0;

			// Where in screen space weapons are
			Double weaponLocation_X = Navigation.GetArea().right * (21 / (Double)160);
			Double weaponLocation_Y = Navigation.GetArea().bottom * (14 / (Double)90);
			Bitmap bm = new Bitmap(130, 130);
			Graphics g = Graphics.FromImage(bm);
			int maxColumns = 7;
			int maxRows = 4;
			int totalRows = (int)Math.Ceiling( weaponCount / (decimal)(maxColumns) );
			int currentColumn = 0;
			int currentRow = 0;

			// offset used to move mouse to other weapons
			int xOffset = Convert.ToInt32(Navigation.GetArea().right * ((Double)12.25 / 160));
			int yOffset = Convert.ToInt32(Navigation.GetArea().bottom * ((Double)14.5 / 90));

			// Go through weapon list
			while (currentweaponCount < weaponCount)
			{
				if (currentweaponCount % maxColumns == 0)
				{
					currentRow++;
					if (totalRows - currentRow <= maxRows - 1)
					{
						break;
					}
				}


				// Select weapon
				Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X) + (xOffset * (currentweaponCount % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y));
				Navigation.sim.Mouse.LeftButtonClick();
				Navigation.SystemRandomWait(Navigation.Speed.Faster);

				// Scan weapon
				Weapon w = ScanWeapon(currentweaponCount);
				currentweaponCount++;
				currentColumn++;

				// Add weapon to equipped list
				if (w.GetEquippedCharacter() != 0)
				{
					equippedWeapon.Add(w);
				}

				// Add to weapon List Object
				weapons.Add(w);

				// reach end of row
				if (currentColumn == maxColumns)
				{
					// reset mouse pointer and scroll down weapon list
					currentColumn = 0;
					scrollCount++;

					// scroll down
					for (int k = 0; k < 10; k++)
					{
						Navigation.sim.Mouse.VerticalScroll(-1);
						// skip a scroll
						if ((k == 7) && ((scrollCount % 3) == 0))
						{
							k++;
							if (scrollCount % 9 == 0)
							{
								if (scrollCount == 18)
								{
									scrollCount = 0;
								}
								else
								{
									Navigation.sim.Mouse.VerticalScroll(-1);
								}
							}
						}
					}
					Navigation.SystemRandomWait(Navigation.Speed.Fast);
				}
			};

			// scroll down as much as possible
			for (int i = 0; i < 20; i++)
			{
				Navigation.sim.Mouse.VerticalScroll(-1);
			}
			Navigation.SystemRandomWait(Navigation.Speed.Normal);

			// Get weapons on bottom of page
			int rowsLeft = (int)Math.Ceiling((weaponCount - currentweaponCount) / (double)maxColumns);
			bool b_EnchancementOre = false;
			int startPostion = 1;
			for (int i = startPostion; i < (rowsLeft + startPostion); i++)
			{
				for (int k = 0; k < maxColumns; k++)
				{
					if (weaponCount - currentweaponCount <= 0)
					{
						break;
					}
					if (!b_EnchancementOre)
					{
						Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X) + (xOffset * (k % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y) + (yOffset * (i % (maxRows + 1))));
						Navigation.sim.Mouse.LeftButtonClick();
						Navigation.SystemRandomWait(Navigation.Speed.Faster);
					}

					// check if enchnacement Ore
					if (weaponCount - currentweaponCount == 7)
					{
						b_EnchancementOre = CheckForEnchancementOre();
					}

					if (b_EnchancementOre)
					{
						// Scan top row instead
						Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X) + (xOffset * (k % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y) + (yOffset * (0 % (maxRows + 1))));
						Navigation.sim.Mouse.LeftButtonClick();
						Navigation.SystemRandomWait(Navigation.Speed.Faster);
					}

					Weapon w = ScanWeapon(currentweaponCount);
					currentweaponCount++;

					// Add to weapon List Object
					weapons.Add(w);
				}
			}//*/


			return weapons;
		}

		public static void ScanWeapons(int count = 0)
		{
			// Get Max weapons from screen
			int weaponCount = count != 0 ? count : ScanWeaponCount();
			UserInterface.SetWeapon_Max(weaponCount);
			int currentweaponCount = 0;
			int scrollCount = 0;

			// Where in screen space weapons are
			Double weaponLocation_X = Navigation.GetArea().right * (21 / (Double)160);
			Double weaponLocation_Y = Navigation.GetArea().bottom * (14 / (Double)90);
			int maxColumns = 7;
			int maxRows = 4;
			int totalRows = (int)Math.Ceiling(weaponCount / (decimal)(maxColumns));
			int currentColumn = 0;
			int currentRow = 0;

			// offset used to move mouse to other weapons
			int xOffset = Convert.ToInt32(Navigation.GetArea().right * ((Double)12.25 / 160));
			int yOffset = Convert.ToInt32(Navigation.GetArea().bottom * ((Double)14.5 / 90));

			// Go through weapon list
			while (currentweaponCount < weaponCount)
			{
				if (currentweaponCount % maxColumns == 0)
				{
					currentRow++;
					if (totalRows - currentRow <= maxRows - 1)
					{
						break;
					}
				}


				// Select weapon
				Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X) + (xOffset * (currentweaponCount % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y));
				Navigation.sim.Mouse.LeftButtonClick();
				Navigation.SystemRandomWait(Navigation.Speed.SelectNextInventoryItem);

				// Scan weapon
				ScanWeaponImage(currentweaponCount);

				currentweaponCount++;
				currentColumn++;

				// reach end of row
				if (currentColumn == maxColumns)
				{
					// reset mouse pointer and scroll down weapon list
					currentColumn = 0;
					scrollCount++;

					// scroll down
					for (int k = 0; k < 10; k++)
					{
						Navigation.sim.Mouse.VerticalScroll(-1);
						// skip a scroll
						if ((k == 7) && ((scrollCount % 3) == 0))
						{
							k++;
							if (scrollCount % 9 == 0)
							{
								if (scrollCount == 18)
								{
									scrollCount = 0;
								}
								else
								{
									Navigation.sim.Mouse.VerticalScroll(-1);
								}
							}
						}
					}
					Navigation.SystemRandomWait(Navigation.Speed.InventoryScroll);
				}
			};

			// scroll down as much as possible
			for (int i = 0; i < 20; i++)
			{
				Navigation.sim.Mouse.VerticalScroll(-1);
			}
			Navigation.SystemRandomWait(Navigation.Speed.InventoryScroll);

			// Get weapons on bottom of page
			int rowsLeft = (int)Math.Ceiling((weaponCount - currentweaponCount) / (double)maxColumns);
			bool b_EnchancementOre = false;
			int startPostion = 1;
			for (int i = startPostion; i < (rowsLeft + startPostion); i++)
			{
				for (int k = 0; k < maxColumns; k++)
				{
					if (weaponCount - currentweaponCount <= 0)
					{
						break;
					}
					if (!b_EnchancementOre)
					{
						Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X) + (xOffset * (k % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y) + (yOffset * (i % (maxRows + 1))));
						Navigation.sim.Mouse.LeftButtonClick();
						Navigation.SystemRandomWait(Navigation.Speed.SelectNextInventoryItem);
					}

					// check if enchnacement Ore
					if (weaponCount - currentweaponCount == 7)
					{
						b_EnchancementOre = CheckForEnchancementOre();
					}

					if (b_EnchancementOre)
					{
						// Scan top row instead
						Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X) + (xOffset * (k % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y) + (yOffset * (0 % (maxRows + 1))));
						Navigation.sim.Mouse.LeftButtonClick();
						Navigation.SystemRandomWait(Navigation.Speed.SelectNextInventoryItem);
					}

					ScanWeaponImage(currentweaponCount);
					currentweaponCount++;
				}
			}//*/
		}

		public static void ScanWeaponImage(int id)
		{

			// Grab Image of Entire weapon on Right
			Double weaponLocation_X = Navigation.GetArea().right * (108 / (Double)160);
			Double weaponLocation_Y = Navigation.GetArea().bottom * (10 / (Double)90);
			int width = 325; int height = 560;
			Bitmap bm = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics g = Graphics.FromImage(bm);
			int screenLocation_X = Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X);
			int screenLocation_Y = Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y);
			g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

			// Separate to all pieces of weapon and add to pics
			List<Bitmap> weaponImages = new List<Bitmap>();

			// Name
			int xOffset = 10;
			int yOffset = 7;
			Bitmap weaponName = bm.Clone(new Rectangle(xOffset, yOffset, width - 2 * xOffset, 25), bm.PixelFormat);

			// Level
			xOffset = 14;
			yOffset = 204;
			Bitmap weaponLevel = bm.Clone(new Rectangle(xOffset, yOffset, 110, 22), bm.PixelFormat);

			// Refinement
			xOffset = 13;
			yOffset = 230;
			Bitmap weaponRefinement = bm.Clone(new Rectangle(xOffset, yOffset, 30, 30), bm.PixelFormat);
			g = Graphics.FromImage(weaponRefinement);
			g.DrawRectangle(new Pen(weaponRefinement.GetPixel(7, 10), 14), new Rectangle(0, 0, 30, 30));

			// Equipped Character
			xOffset = 30;
			yOffset = 532;
			Bitmap weaponEquippedCharacter = bm.Clone(new Rectangle(xOffset, yOffset, width - xOffset, 20), bm.PixelFormat);
			g = Graphics.FromImage(weaponEquippedCharacter);
			// Gets rid of character head on Left
			g.DrawRectangle(new Pen(weaponEquippedCharacter.GetPixel(width - xOffset - 1, 10), 14), new Rectangle(0, 0, 13, bm.Height));

			// Assign to List
			weaponImages.Add(weaponName);
			weaponImages.Add(weaponLevel);
			weaponImages.Add(weaponRefinement);
			weaponImages.Add(weaponEquippedCharacter);

			// Send Image to Worker Queue
			GenshinData.workerQueue.Enqueue(new OCRImage(weaponImages, "weapon", id));
			g.Dispose();
		}

		public static Weapon ScanWeapon(List<Bitmap> bm, int id)
		{

			// Init Variables
			int name = 0;
			int level = 1;
			bool ascension = false;
			int refinementLevel = 1;
			int equippedCharacter = 0;
			int width = 325; int height = 560;

			if (bm.Count == 4)
			{
				int w_name = 0; int w_level = 1; int w_refinement = 2; int w_equippedCharacter = 3;
				// Check for Rarity
				Color rarityColor = bm[0].GetPixel(5, 5);
				Color fiveStar = Color.FromArgb(255, 188, 105, 50);
				Color fourthStar = Color.FromArgb(255, 161, 86, 224);
				Color threeStar = Color.FromArgb(255, 81, 127, 203);
				// Check for equipped color
				Color equipped = Color.FromArgb(255, 255, 231, 187);
				Color equippedColor = bm[w_equippedCharacter].GetPixel(5, 5);

				// Scan different parts of the weapon
				bool b_equipped = Scraper.CompareColors(equipped, equippedColor);
				bool b_RarityAboveTwo = Scraper.CompareColors(fiveStar, rarityColor) || Scraper.CompareColors(fourthStar, rarityColor) || Scraper.CompareColors(threeStar, rarityColor);

				if (b_RarityAboveTwo || b_equipped)
				{

					Thread thr1 = new Thread(() => name = ScanName(bm[w_name], width, height));
					Thread thr2 = new Thread(() => level = ScanLevel(bm[w_level], width, height, ref ascension));
					Thread thr3 = new Thread(() => refinementLevel = ScanRefinement(bm[w_refinement], width, height));
					Thread thr4 = new Thread(() => equippedCharacter = ScanEquippedCharacter(bm[w_equippedCharacter], width, height));	

					// Start Threads

					thr1.Start(); thr2.Start();
					if (b_RarityAboveTwo)
					{
						thr3.Start();
					}
					if (b_equipped)
					{
						thr4.Start();
					}

					// End Threads

					thr1.Join(); thr2.Join();
					if (b_equipped)
					{
						thr4.Join();
					}
					if (b_RarityAboveTwo)
					{
						thr3.Join();
					}

					// dispose the list
					foreach (Bitmap x in bm)
					{
						x.Dispose();
					}
				}
				else
				{
					name = -1; refinementLevel = -1; equippedCharacter = -1;
				}
			}

			Weapon weapon = new Weapon(name, level, ascension, refinementLevel, equippedCharacter, id);

			// Check for Errors

			return weapon;

		}

		public static Weapon ScanWeapon(int id)
		{
			// Init Variables
			int name = 0;
			int level = 1;
			bool ascension = false;
			int refinementLevel = 1;
			int equippedCharacter = 0;

			// Grab Image of Entire weapon on Right
			Double weaponLocation_X = Navigation.GetArea().right * (108 / (Double)160);
			Double weaponLocation_Y = Navigation.GetArea().bottom * (10 / (Double)90);
			int width = 325; int height = 560;
			Bitmap bm = new Bitmap(width, height);
			Graphics g = Graphics.FromImage(bm);
			int screenLocation_X = Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X);
			int screenLocation_Y = Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y);
			g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);

			// Check for Rarity
			Color rarityColor = bm.GetPixel(12, 10);
			Color fiveStar = Color.FromArgb(255, 188, 105, 50);
			Color fourthStar = Color.FromArgb(255, 161, 86, 224);
			Color threeStar = Color.FromArgb(255, 81, 127, 203);
			// Check for equipped color
			Color equipped = Color.FromArgb(255, 255, 231, 187);
			Color equippedColor = bm.GetPixel(5, height - 10);

			// Scan different parts of the weapon

			bool b_equipped = Scraper.CompareColors(equipped, equippedColor);
			bool b_RarityAboveTwo = Scraper.CompareColors(fiveStar, rarityColor) || Scraper.CompareColors(fourthStar, rarityColor) || Scraper.CompareColors(threeStar, rarityColor);
			// TODO:: ADD multi threading support 
			Bitmap bm_1 = bm.Clone(new Rectangle(0, 0, width, height), bm.PixelFormat);
			Bitmap bm_2 = bm.Clone(new Rectangle(0, 0, width, height), bm.PixelFormat);
			Bitmap bm_3 = bm.Clone(new Rectangle(0, 0, width, height), bm.PixelFormat);
			Bitmap bm_4 = bm.Clone(new Rectangle(0, 0, width, height), bm.PixelFormat);

			Thread thr1 = new Thread(() => name = ScanName(bm_1, width, height));
			Thread thr2 = new Thread(() => level = ScanLevel(bm_2, width, height, ref ascension));
			Thread thr3 = new Thread(() => refinementLevel = ScanRefinement(bm_3, width, height));
			Thread thr4 = new Thread(() => equippedCharacter = ScanEquippedCharacter(bm_4, width, height));

			// Start Threads

			thr1.Start(); thr2.Start();
			if (b_RarityAboveTwo)
			{
				thr3.Start();
			}
			if (b_equipped)
			{
				thr4.Start();
			}

			// End Threads

			thr1.Join(); thr2.Join();
			if (b_equipped)
			{
				thr4.Join();
			}
			if (b_RarityAboveTwo)
			{
				thr3.Join();
			}




			Weapon weapon = new Weapon(name, level, ascension, refinementLevel, equippedCharacter, id);

			return weapon;

		}

		public static bool CheckForEnchancementOre()
		{
			// Init Variables
			int name = 0;

			// Grab Image of Entire weapon on Right
			Double weaponLocation_X = Navigation.GetArea().right * (108 / (Double)160);
			Double weaponLocation_Y = Navigation.GetArea().bottom * (10 / (Double)90);
			int width = 325; int height = 560;
			Bitmap bm = new Bitmap(width, height);
			Graphics g = Graphics.FromImage(bm);
			int screenLocation_X = Navigation.GetPosition().left + Convert.ToInt32(weaponLocation_X);
			int screenLocation_Y = Navigation.GetPosition().top + Convert.ToInt32(weaponLocation_Y);
			g.CopyFromScreen(screenLocation_X, screenLocation_Y, 0, 0, bm.Size);
			//bm = Scraper.ResizeImage(bm, bm.Width * 2, bm.Height * 2);

			name = ScanEnchancementOreName(bm, width, height);
			bm.Dispose();

			return (name > 0) ? true : false;
		}

		public static int ScanWeaponCount()
		{
			//Find weapon count
			Double weaponCountLocation_X = Navigation.GetArea().right * (130 / (Double)160);
			Double weaponCountLocation_Y = Navigation.GetArea().bottom * (2 / (Double)90);
			Bitmap bm = new Bitmap(175, 34);
			Graphics g = Graphics.FromImage(bm);
			g.CopyFromScreen(Navigation.GetPosition().left + Convert.ToInt32(weaponCountLocation_X), Navigation.GetPosition().top + Convert.ToInt32(weaponCountLocation_Y), 0, 0, bm.Size);

			Scraper.SetGrayscale(ref bm);
			Scraper.SetContrast(60.0, ref bm);
			Scraper.SetInvert(ref bm);
			UserInterface.SetNavigation_Image(bm);

			string text = Scraper.AnalyzeText(bm);
			text = Regex.Replace(text, @"[^\d/]", "");

			int count;
			// Check for dash
			if (Regex.IsMatch(text, "/"))
			{
				count = Int32.Parse(text.Split('/')[0]);
			}
			else
			{
				// divide by the number on the right if both numbers fused
				count = Int32.Parse(text) / 2000;
			}

			// Check if larger than 1000
			while (count > 2000)
			{
				count /= 20;
			}


			return count;
		}

		public static int ScanName(Bitmap bm, int max_X, int max_Y)
		{
			int name = 0;

			Scraper.SetGamma(0.2, 0.2, 0.2, ref bm);
			Scraper.SetGrayscale(ref bm);
			Scraper.SetInvert(ref bm);
			//bm = Scraper.ResizeImage(bm, max_X * 2, max_Y * 2);

			// Analyze
			string text = Scraper.AnalyzeText_1(bm);
			text = text.Trim();
			text = Regex.Replace(text, @"[\W]", "");
			text = text.ToLower();

			UserInterface.SetArtifact_GearSlot(bm, text, true);

			// Check in Dictionary
			name = Scraper.GetWeaponCode(text);

			return name;
		}

		public static int ScanEnchancementOreName(Bitmap weaponImage, int max_X, int max_Y)
		{
			int name = 0;

			//Init
			int xOffset = 10;
			int yOffset = 7;
			//Bitmap bm = new Bitmap(max_X-2*xOffset, 25);
			Bitmap bm = weaponImage.Clone(new Rectangle(xOffset, yOffset, max_X - 2 * xOffset, 25), weaponImage.PixelFormat);

			// Setup Img
			Graphics g = Graphics.FromImage(bm);

			Scraper.SetGamma(0.2, 0.2, 0.2, ref bm);
			Scraper.SetGrayscale(ref bm);
			Scraper.SetInvert(ref bm);

			UserInterface.SetNavigation_Image(bm);

			// Analyze
			string text = Scraper.AnalyzeText(bm);
			text = text.Trim();
			bm.Dispose();


			text = text.Trim();
			//text = Regex.Replace(text, @"(?![A-Za-z\s]).", "");
			text = Regex.Replace(text, @"[\W_]", "");
			text = text.ToLower();
			//Debug.Print("Weapon Name: " + text);

			// Check in Dictionary
			name = Scraper.GetEnhancementMaterialCode(text);

			return name;
		}

		public static int ScanLevel(Bitmap bm, int max_X, int max_Y, ref bool ascension)
		{
			Scraper.SetGrayscale(ref bm);
			Scraper.SetInvert(ref bm);
			Scraper.SetContrast(100.0, ref bm);

			string text = Scraper.AnalyzeText_2(bm);
			text = Regex.Replace(text, @"(?![\d/]).", "");
			text = text.Trim();

			UserInterface.SetArtifact_MainStat(bm, text, true);

			if (text.Contains('/'))
			{
				string[] temp = text.Split(new[] { '/' }, 2);

				if (temp.Length == 2)
				{
					if (temp[0] == temp[1])
						ascension = true;

					int level = -1;
					if (int.TryParse(temp[0], out level))
					{


						return level;
					}
					else
					{
						string text1 = "Found " + temp[0] + "instead of Weapon Level.";
						Debug.Print("Found " + temp[0] + "instead of Weapon Level.");
						UserInterface.AddError(text1);
						//Form1.UnexpectedError(text);
						return -1;
					}
				}
				else
				{
					string text1 = "Found " + temp[0] + "instead of Weapon Level.";
					Debug.Print("Found " + temp[0] + "instead of Weapon Level.");
					UserInterface.AddError(text1);
					//Form1.UnexpectedError(text);
					return -1;
				}

			}
			else
			{
				string text1 = "Found " + text + "instead of Weapon Level.";
				Debug.Print("Found " + text + "instead of Weapon Level.");
				UserInterface.AddError(text1);
				//Form1.UnexpectedError(text);
				return -1;
			}
		}

		public static int ScanRefinement(Bitmap bm, int max_X, int max_Y)
		{
			Bitmap bm_copy = bm.Clone(new Rectangle(0, 0, 30, 30), bm.PixelFormat);

			Scraper.SetInvert(ref bm);
			Scraper.SetGrayscale(ref bm);

			string text = Scraper.AnalyzeText_3(bm);
			text = text.Trim();
			text = Regex.Replace(text, @"[^\d]", "");

			// Parse Int
			int refinementLevel = -1;
			if (int.TryParse(text, out refinementLevel))
			{
				UserInterface.SetArtifact_Level(bm, text, true);
				bm.Dispose();
				return refinementLevel;
			}
			else
			{
				// try again to try to get 5
				bm = bm_copy;
				//bm = Scraper.ResizeImage(bm, 30 * 2, 30 * 2);
				text = Scraper.AnalyzeText_3(bm);
				text = text.Trim();

				refinementLevel = -1;
				if (int.TryParse(text, out refinementLevel))
				{
					UserInterface.SetArtifact_Level(bm, text);
					bm.Dispose();
					return refinementLevel;
				}
				else
				{
					return refinementLevel;
				}
			}
		}

		public static int ScanEquippedCharacter(Bitmap bm, int max_X, int max_Y)
		{
			Scraper.SetGrayscale(ref bm);
			Scraper.SetContrast(60.0, ref bm);

			string equippedCharacter = Scraper.AnalyzeText_4(bm);
			equippedCharacter.Trim();

			if (equippedCharacter != "")
			{
				var regexItem = new Regex("Equipped:");
				if (regexItem.IsMatch(equippedCharacter))
				{
					string[] tempString = equippedCharacter.Split(':');
					equippedCharacter = tempString[1].Replace("\n", String.Empty);
					UserInterface.SetArtifact_Equipped(bm, equippedCharacter);
					equippedCharacter = Regex.Replace(equippedCharacter, @"[^\w_]", "");
					equippedCharacter = equippedCharacter.ToLower();

					// Assign Traveler Name if not found
					int character = Scraper.GetCharacterCode(equippedCharacter);
					if (Scraper.b_AssignedTravelerName == false && character == 1)
					{
						Scraper.AssignTravelerName(equippedCharacter);
						Scraper.b_AssignedTravelerName = true;
					}

					// Used to match with Traveler Name
					while (equippedCharacter.Length > 1)
					{
						int temp = Scraper.GetCharacterCode(equippedCharacter, true);
						if (temp == -1)
						{
							equippedCharacter = equippedCharacter.Substring(0, equippedCharacter.Length - 1);
						}
						else
						{
							break;
						}
					}

					if (equippedCharacter.Length > 0)
					{
						return Scraper.GetCharacterCode(equippedCharacter);
					}
					return 0;
				}
			}
			// artifact has no equipped character
			return 0;
		}
	}
}
