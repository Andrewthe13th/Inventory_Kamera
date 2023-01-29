using Accord.Imaging;
using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InventoryKamera
{
    public static class WeaponScraper
	{
		private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		private static bool SortByLevel = false;

		public static bool StopScanning { get; set; }

		public static void ScanWeapons(int count = 0)
		{
			// Determine maximum number of weapons to scan
			int weaponCount = count == 0 ? ScanWeaponCount(): count;
			int page = 0;
			var (rectangles, cols, rows) = GetPageOfItems(page);
			int fullPage = cols * rows;
			int totalRows = (int)Math.Ceiling(weaponCount / (decimal)cols);
			int cardsQueued = 0;
			int rowsQueued = 0;
			int offset = 0;
			UserInterface.SetWeapon_Max(weaponCount);

			// Determine Delay if delay has not been found before
			// Scraper.FindDelay(rectangles);

			StopScanning = false;

			Logger.Info("Found {0} for weapon count.", weaponCount);

			SortByLevel = Properties.Settings.Default.MinimumWeaponLevel > 1;

			if (SortByLevel)
			{
				Logger.Debug("Sorting by level to optimize scan time.");
				// Check if sorted by level
				// If not, sort by level
				if (CurrentSortingMethod() != "level")
				{
					Logger.Debug("Not already sorting by level...");
					Navigation.SetCursor(
						X: (int)(230 / 1280.0 * Navigation.GetWidth()),
						Y: (int)(680 / 720.0 * Navigation.GetHeight()));
					Navigation.Click();
					Navigation.Wait();
					Navigation.SetCursor(
						X: (int)(250 / 1280.0 * Navigation.GetWidth()),
						Y: (int)(575 / 720.0 * Navigation.GetHeight()));
					Navigation.Click();
					Navigation.Wait();
				}
				Logger.Debug("Inventory is sorted by level.");
			}
			else
			{
				Logger.Debug("Sorting by quality to optimize scan time.");
				// Check if sorted by quality
				if (CurrentSortingMethod() != "quality")
				{
					Logger.Debug("Not already sorting by quality...");
					// If not, sort by quality
					Navigation.SetCursor(
						X: (int)(230 / 1280.0 * Navigation.GetWidth()),
						Y: (int)(680 / 720.0 * Navigation.GetHeight()));
					Navigation.Click();
					Navigation.Wait();
					Navigation.SetCursor(
						X: (int)(250 / 1280.0 * Navigation.GetWidth()),
						Y: (int)(615 / 720.0 * Navigation.GetHeight()));
					Navigation.Click();
					Navigation.Wait();
				}
				Logger.Debug("Inventory is sorted by quality");
			}

			// Go through weapon list
			while (cardsQueued < weaponCount)
			{
				Logger.Debug("Scanning weapon page {0}", page);
				Logger.Debug("Located {0} possible item locations on page.", rectangles.Count);

				int cardsRemaining =  weaponCount - cardsQueued ;
				// Go through each "page" of items and queue. In the event that not a full page of
				// items are scrolled to, offset the index of rectangle to start clicking from
				for (int i = cardsRemaining < fullPage ? ( rows - ( totalRows - rowsQueued ) ) * cols : 0; i < rectangles.Count; i++)
				{
					Rectangle item = rectangles[i];
					Navigation.SetCursor(item.Center().X, item.Center().Y + offset);
					Navigation.Click();
					Navigation.SystemWait(Navigation.Speed.SelectNextInventoryItem);

					// Queue card for scanning
					QueueScan(cardsQueued);
					cardsQueued++;
					if (cardsQueued >= weaponCount || StopScanning)
					{
						if (StopScanning) Logger.Info("Stopping weapon scan based on filtering");
						else Logger.Info("Stopping weapon scan based on scans queued ({0} of {1})", cardsQueued, weaponCount);
						return;
					}
				}
				Logger.Debug("Finished queuing page of weapons. Scrolling...");

				rowsQueued += rows;

				// Page done, now scroll
				// If the number of remaining scans is shorter than a full page then
				// only scroll a few rows
				if (totalRows - rowsQueued <= rows)
				{
					if (Navigation.GetAspectRatio() == new Size(8, 5))
					{
						offset = 35; // Lazy fix
					}
					for (int i = 0; i < 10 * ( totalRows - rowsQueued ) - 1; i++)
					{
						Navigation.sim.Mouse.VerticalScroll(-1);
						Navigation.Wait(1);
					}
					Navigation.SystemWait(Navigation.Speed.Fast);
				}
				else
				{
					// Scroll back one to keep it from getting too crazy
					if (rowsQueued % 15 == 0)
					{
						Navigation.sim.Mouse.VerticalScroll(1);
					}
					for (int i = 0; i < 10 * rows - 1; i++)
					{
						Navigation.sim.Mouse.VerticalScroll(-1);
						Navigation.Wait(1);
					}
					Navigation.SystemWait(Navigation.Speed.Fast);
				}
				++page;
				(rectangles, cols, rows) = GetPageOfItems(page);
			}
		}

		private static string CurrentSortingMethod()
		{
			var region = new Rectangle(
				x: (int)(100.0 / 1280.0 * Navigation.GetWidth()),
				y: (int)(660.0 / 720.0 * Navigation.GetHeight()),
				width: (int)(175.0 / 1280.0 * Navigation.GetWidth()),
				height: (int)(40.0 / 720.0 * Navigation.GetHeight()));

			using (var bm = Navigation.CaptureRegion(region))
			{
				var g = Scraper.ConvertToGrayscale(bm);
				var mode = Scraper.AnalyzeText(g).Trim().ToLower();
				return mode.Contains("level") ? "level" : mode.Contains("quality") ? "quality" : null;
			}
		}

		public static int ScanWeaponCount()
		{
			//Find weapon count
			Rectangle region = new Rectangle(
				x: (int)( 1030 / 1280.0 * Navigation.GetWidth() ),
				y: (int)( 20 / 720.0 * Navigation.GetHeight() ),
				width: (int)( 175 / 1280.0 * Navigation.GetWidth() ),
				height: (int)( 25 / 720.0 * Navigation.GetHeight() ));

			using (Bitmap countBitmap = Navigation.CaptureRegion(region))
			{
				UserInterface.SetNavigation_Image(countBitmap);

				Bitmap n = Scraper.ConvertToGrayscale(countBitmap);
				Scraper.SetContrast(60.0, ref n);
				Scraper.SetInvert(ref n);

				string text = Scraper.AnalyzeText(n).Trim();
				n.Dispose();

				// Remove any non-numeric and '/' characters
				text = Regex.Replace(text, @"[^0-9/]", string.Empty);

				if (string.IsNullOrWhiteSpace(text) || Properties.Settings.Default.LogScreenshots)
				{
					countBitmap.Save($"./logging/weapons/WeaponCount.png");
					Navigation.CaptureWindow().Save($"./logging/weapons/WeaponWindow_{Navigation.GetWidth()}x{Navigation.GetHeight()}.png");
					if (string.IsNullOrWhiteSpace(text)) throw new FormatException("Unable to locate weapon count.");
				}

				int count;

				// Check for slash
				if (Regex.IsMatch(text, "/"))
				{
					count = int.Parse(text.Split('/')[0]);
				}
				else if (Regex.Matches(text, "2000").Count == 1) // Remove the inventory limit from number
				{
					text = text.Replace("2000", string.Empty);
					count = int.Parse(text);
				}
				else // Extreme worst case
				{
					count = 2000;
					Logger.Debug("Defaulted to 2000 for weapon count");
				}

				return count;
			}
		}

		private static (List<Rectangle> rectangles, int cols, int rows) GetPageOfItems(int page)
		{
			// Screenshot of inventory
			using (Bitmap screenshot = Navigation.CaptureWindow())
			{
				try
				{
					var (rectangles, cols, rows) = ProcessScreenshot(screenshot);
					int weight = 0;
					while ((cols != 8 || rows != 5) && weight < 15)
					{
						Logger.Warn("Unable to locate full page of weapons with weight {0}", weight);

						// Generated rectangles
						screenshot.Save($"./logging/weapons/WeaponInventory.png");
						using (Graphics g = Graphics.FromImage(screenshot))
							rectangles.ForEach(r => g.DrawRectangle(new Pen(Color.Green, 2), r));

						screenshot.Save($"./logging/weapons/WeaponInventory{page}_{cols}x{rows} - weight {weight}.png");

						weight += 3;
						(rectangles, cols, rows) = ProcessScreenshot(screenshot, weight);
					}
					if (Properties.Settings.Default.LogScreenshots)
					{
                        screenshot.Save($"./logging/weapons/WeaponInventory.png");
                        using (Graphics g = Graphics.FromImage(screenshot))
                            rectangles.ForEach(r => g.DrawRectangle(new Pen(Color.Green, 2), r));

                        screenshot.Save($"./logging/weapons/WeaponInventory{page}_{cols}x{rows} - weight {weight}.png");
                    }
					return (rectangles, cols, rows);
				}
				catch (Exception)
				{
					screenshot.Save($"./logging/weapons/WeaponInventory.png");
					throw;
				}
			}

		}

		public static (List<Rectangle> rectangles, int cols, int rows) ProcessScreenshot(Bitmap screenshot, int weight = 0)
		{
			// Size of an item card is the same in 16:10 and 16:9. Also accounts for character icon and resolution size.
			double base_aspect_width = 1280.0;
			double base_aspect_height = 720.0;
			var card = new RECT(
				Left: 0,
				Top: 0,
				Right: (int)(85 / base_aspect_width * screenshot.Width),
				Bottom: (int)(105 / base_aspect_height * screenshot.Height));

			if (Navigation.GetAspectRatio() == new Size(8, 5))
			{
				base_aspect_height = 800.0;
			}

			// Filter for relative size of items in inventory, give or take a few pixels
			using (BlobCounter blobCounter = new BlobCounter
			{
				FilterBlobs = true,
				MinHeight = card.Height - ((int)(card.Height * 0.2)) - weight,
				MaxHeight = card.Height + ((int)(card.Height * 0.2)) + weight,
				MinWidth = card.Width - ((int)(card.Width * 0.2)) - weight,
				MaxWidth = card.Width + ((int)(card.Width * 0.2)) + weight,
			})
			{
				// Image pre-processing
				screenshot = new KirschEdgeDetector().Apply(screenshot); // Algorithm to find edges. Really good but can take ~1s
				screenshot = new Grayscale(0.2125, 0.7154, 0.0721).Apply(screenshot);
				screenshot = new Threshold(100).Apply(screenshot); // Convert to black and white only based on pixel intensity			

				blobCounter.ProcessImage(screenshot);
				// Note: Processing won't always detect all item rectangles on screen. Since the
				// background isn't a solid color it's a bit trickier to filter out.

				// Don't save overlapping blobs
				List<Rectangle> rectangles = new List<Rectangle>();
				List<Rectangle> blobRects = blobCounter.GetObjectsRectangles().ToList();

				int minWidth = blobRects[0].Width;
				int minHeight = blobRects[0].Height;
				foreach (var rect in blobRects)
				{
					bool add = true;
					foreach (var item in rectangles)
					{
						Rectangle r1 = rect;
						Rectangle r2 = item;
						Rectangle intersect = Rectangle.Intersect(r1, r2);
						if (intersect.Width > r1.Width * .2)
						{
							add = false;
							break;
						}
					}
					if (add)
					{
						minWidth = Math.Min(minWidth, rect.Width);
						minHeight = Math.Min(minHeight, rect.Height);
						rectangles.Add(rect);
					}
				}

				// Determine X and Y coordinates for columns and rows, respectively
				var colCoords = new List<int>();
				var rowCoords = new List<int>();

				foreach (var item in rectangles)
				{
					bool addX = true;
					bool addY = true;
					foreach (var x in colCoords)
					{
						var xC = item.Center().X;
						if (x - 75 / base_aspect_width * screenshot.Width <= xC && xC <= x + 75 / base_aspect_width * screenshot.Width)
						{
							addX = false;
							break;
						}
					}
					foreach (var y in rowCoords)
					{
						var yC = item.Center().Y;
						if (y - 100 / base_aspect_height * screenshot.Height <= yC && yC <= y + 100 / base_aspect_height * screenshot.Height)
						{
							addY = false;
							break;
						}
					}
					if (addX)
					{
						colCoords.Add(item.Center().X);
					}
					if (addY)
					{
						rowCoords.Add(item.Center().Y);
					}
				}

				// Going to use X,Y coordinate pairings to build rectangles around. Items that might have been missed
				// This is quite accurate and algorithmically puts rectangles over all items on the screen that were missed.
				// The center of each of these rectangles should be a good enough spot to click.
				rectangles.Clear();
				colCoords.Sort();
				rowCoords.Sort();

				colCoords.RemoveAll(col => col > screenshot.Width * 0.65);

				foreach (var row in rowCoords)
				{
					foreach (var col in colCoords)
					{
						int x = (int)(col - (minWidth * .5));
						int y = (int)(row - (minHeight * .5));

						rectangles.Add(new Rectangle(x, y, minWidth, minHeight));
					}
				}

				// Remove some rectangles that somehow overlap each other. Don't think this happens
				// but it doesn't hurt to double check.
				for (int i = 0; i < rectangles.Count - 1; i++)
				{
					for (int j = i + 1; j < rectangles.Count; j++)
					{
						Rectangle r1 = rectangles[i];
						Rectangle r2 = rectangles[j];
						Rectangle intersect = Rectangle.Intersect(r1, r2);
						if (intersect.Width > r1.Width * .2)
						{
							rectangles.RemoveAt(j);
						}
					}
				}

				// Sort by row then by column within each row
				rectangles = rectangles.OrderBy(r => r.Top).ThenBy(r => r.Left).ToList();

				return (rectangles, colCoords.Count, rowCoords.Count);
			}
		}

		public static void QueueScan(int id)
		{
			int width = Navigation.GetWidth();
			int height = Navigation.GetHeight();

			// Separate to all pieces of card
			List<Bitmap> weaponImages = new List<Bitmap>();

			Bitmap card;
			RECT reference;
			Bitmap name, level, refinement, equipped, locked;

			if (Navigation.GetAspectRatio() == new Size(16, 9))
			{
				// Grab image of entire card on Right
				reference = new RECT(new Rectangle(872, 80, 327, 560)); // In 1280x720

				int left   = (int)Math.Round(reference.Left   / 1280.0 * width, MidpointRounding.AwayFromZero);
				int top    = (int)Math.Round(reference.Top    / 720.0 * height, MidpointRounding.AwayFromZero);
				int right  = (int)Math.Round(reference.Right  / 1280.0 * width, MidpointRounding.AwayFromZero);
				int bottom = (int)Math.Round(reference.Bottom / 720.0 * height, MidpointRounding.AwayFromZero);

				card = Navigation.CaptureRegion(new RECT(left, top, right, bottom));

				// Equipped Character
				equipped = card.Clone(new RECT(
				Left: (int)( 52.0 / reference.Width * card.Width ),
				Top: (int)( 522.0 / reference.Height * card.Height ),
				Right: card.Width,
				Bottom: card.Height), card.PixelFormat);
			}
			else // if (Navigation.GetAspectRatio() == new Size(8, 5))
			{
				// Grab image of entire card on Right
				reference = new Rectangle(872, 80, 328, 640); // In 1280x800

				int left   = (int)Math.Round(reference.Left   / 1280.0 * width, MidpointRounding.AwayFromZero);
				int top    = (int)Math.Round(reference.Top    / 800.0 * height, MidpointRounding.AwayFromZero);
				int right  = (int)Math.Round(reference.Right  / 1280.0 * width, MidpointRounding.AwayFromZero);
				int bottom = (int)Math.Round(reference.Bottom / 800.0 * height, MidpointRounding.AwayFromZero);

				RECT itemCard = new RECT(left, top, right, bottom);

				card = Navigation.CaptureRegion(itemCard);

				// Equipped Character
				equipped = card.Clone(new RECT(
					Left: (int)( 52.0 / reference.Width * card.Width ),
					Top: (int)( 602.0 / reference.Height * card.Height ),
					Right: card.Width,
					Bottom: card.Height), card.PixelFormat);
			}

			// Name
			name = card.Clone(new RECT(
				Left: 0,
				Top: 0,
				Right: card.Width,
				Bottom: (int)( 38.0 / reference.Height * card.Height )), card.PixelFormat);

			// Level
			level = card.Clone(new RECT(
				Left: (int)( 19.0 / reference.Width * card.Width ),
				Top: (int)( 206.0 / reference.Height * card.Height ),
				Right: (int)( 107.0 / reference.Width * card.Width ),
				Bottom: (int)( 225.0 / reference.Height * card.Height )), card.PixelFormat);

			// Refinement
			refinement = card.Clone(new RECT(
				Left: (int)( 20.0 / reference.Width * card.Width ),
				Top: (int)( 235.0 / reference.Height * card.Height ),
				Right: (int)( 40.0 / reference.Width * card.Width ),
				Bottom: (int)( 254.0 / reference.Height * card.Height )), card.PixelFormat);

			locked = card.Clone(new RECT(
				Left: (int)( 284.0 / reference.Width * card.Width ),
				Top: (int)( 201.0 / reference.Height * card.Height ),
				Right: (int)( 312.0 / reference.Width * card.Width ),
				Bottom: (int)( 228.0 / reference.Height * card.Height )), card.PixelFormat); ;

			// Assign to List
			weaponImages.Add(name); //0
			weaponImages.Add(level);
			weaponImages.Add(refinement);
			weaponImages.Add(locked);
			weaponImages.Add(equipped);
			weaponImages.Add(card); //5

			bool a = false;

            bool belowRarity = GetRarity(name) < Properties.Settings.Default.MinimumWeaponRarity;
            bool belowLevel = ScanLevel(level, ref a) < Properties.Settings.Default.MinimumWeaponLevel;
            StopScanning = (SortByLevel && belowLevel) || (!SortByLevel && belowRarity);

			if (StopScanning || belowRarity || belowLevel)
            {
				weaponImages.ForEach(i => i.Dispose());
				return;
            }
			// Send images to worker queue
			InventoryKamera.workerQueue.Enqueue(new OCRImageCollection(weaponImages, "weapon", id));
			
		}

		public static async Task<Weapon> CatalogueFromBitmapsAsync(List<Bitmap> bm, int id)
		{
			// Init Variables
			string name = null;
			int level = -1;
			bool ascended = false;
			int refinementLevel = -1;
			bool locked = false;
			string equippedCharacter = null;
			int rarity = 0;

			if (bm.Count >= 4)
			{
				int w_name = 0; int w_level = 1; int w_refinement = 2; int w_lock = 3; int w_equippedCharacter = 4;

				// Check for Rarity
				rarity = GetRarity(bm[w_name]);

				// Check for equipped color
				Color equippedColor = Color.FromArgb(255, 255, 231, 187);
				Color equippedStatus = bm[w_equippedCharacter].GetPixel(5, 5);
				bool b_equipped = Scraper.CompareColors(equippedColor, equippedStatus);

				// Check for lock color
				Color lockedColor = Color.FromArgb(255, 70, 80, 100); // Dark area around red lock
				Color lockStatus = bm[w_lock].GetPixel(5, 5);
				locked = Scraper.CompareColors(lockedColor, lockStatus);

				List<Task> tasks = new List<Task>();

				var taskName = Task.Run(() => name = ScanName(bm[w_name]));
				var taskLevel = Task.Run(() => level = ScanLevel(bm[w_level], ref ascended));
				var taskRefinement = Task.Run(() => refinementLevel = ScanRefinement(bm[w_refinement]));
				var taskEquipped = Task.Run(() => equippedCharacter = ScanEquippedCharacter(bm[w_equippedCharacter]));

				tasks.Add(taskName);
				tasks.Add(taskLevel);
				tasks.Add(taskRefinement);

				if (b_equipped)
				{
					tasks.Add(taskEquipped);
				}

				await Task.WhenAll(tasks.ToArray());
			}
			if (!Properties.Settings.Default.EquipWeapons) equippedCharacter = "";
			return new Weapon(name, level, ascended, refinementLevel, locked, equippedCharacter, id, rarity);
		}

		private static int GetRarity(Bitmap bm)
		{
			var averageColor = new ImageStatistics(bm);

			Color fiveStar    = Color.FromArgb(255, 188, 105,  50);
			Color fourStar    = Color.FromArgb(255, 161,  86, 224);
			Color threeStar   = Color.FromArgb(255,  81, 127, 203);
			Color twoStar     = Color.FromArgb(255,  42, 143, 114);
			Color oneStar     = Color.FromArgb(255, 114, 119, 138);

			var colors = new List<Color> { Color.Black, oneStar, twoStar, threeStar, fourStar, fiveStar };

			var c = Scraper.ClosestColor(colors, averageColor);

			return colors.IndexOf(c);
		}

		public static bool IsEnhancementMaterial(Bitmap nameBitmap)
		{
			string material = ScanEnchancementOreName(nameBitmap);
			return !string.IsNullOrWhiteSpace(material) && Scraper.enhancementMaterials.Contains(material.ToLower());
		}

		public static string ScanEnchancementOreName(Bitmap bm)
		{
			Scraper.SetGamma(0.2, 0.2, 0.2, ref bm);
			Bitmap n = Scraper.ConvertToGrayscale(bm);
			Scraper.SetInvert(ref n);

			// Analyze
			string name = Regex.Replace(Scraper.AnalyzeText(n).ToLower(), @"[\W]", string.Empty);
			name = Scraper.FindClosestMaterialName(name, 3);
			n.Dispose();

			return name;
		}

		#region Task Methods

		public static string ScanName(Bitmap bm)
		{
			Scraper.SetGamma(0.2, 0.2, 0.2, ref bm);
			Bitmap n = Scraper.ConvertToGrayscale(bm);
			Scraper.SetInvert(ref n);

			// Analyze
			string text = Regex.Replace(Scraper.AnalyzeText(n).ToLower(), @"[\W]", string.Empty);
			text = Scraper.FindClosestWeapon(text);

			n.Dispose();

			// Check in Dictionary
			return text;
		}

		public static int ScanLevel(Bitmap bm, ref bool ascended)
		{
			Bitmap n = Scraper.ConvertToGrayscale(bm);
			Scraper.SetInvert(ref n);

			string text = Scraper.AnalyzeText(n).Trim();
			n.Dispose();
			text = Regex.Replace(text, @"(?![\d/]).", string.Empty);

			if (text.Contains('/'))
			{
				string[] temp = text.Split(new[] { '/' }, 2);

				if (temp.Length == 2)
				{
					if (int.TryParse(temp[0], out int level) && int.TryParse(temp[1], out int maxLevel))
					{
						maxLevel = (int)Math.Round(maxLevel / 10.0, MidpointRounding.AwayFromZero) * 10;
						ascended = 20 <= level && level < maxLevel;
						return level;
					}
				}
			}
			return -1;
		}

		public static int ScanRefinement(Bitmap image)
		{
			for (double factor = 1; factor <= 2; factor += 0.1)
			{
				using (Bitmap up = Scraper.ScaleImage(image, factor))
				{
					Bitmap n = Scraper.ConvertToGrayscale(up);
					Scraper.SetInvert(ref n);

					string text = Scraper.AnalyzeText(n).Trim();
					n.Dispose();
					text = Regex.Replace(text, @"[^\d]", string.Empty);

					// Parse Int
					if (int.TryParse(text, out int refinementLevel) && 1 <= refinementLevel && refinementLevel <= 5)
					{
						return refinementLevel;
					}
				}
			}
			return -1;
		}

		public static string ScanEquippedCharacter(Bitmap bm)
		{
			Bitmap n = Scraper.ConvertToGrayscale(bm);
			Scraper.SetContrast(60.0, ref n);

			string extractedString = Scraper.AnalyzeText(n);
			n.Dispose();

			if (extractedString != "")
			{
				var regexItem = new Regex("Equipped:");
				if (regexItem.IsMatch(extractedString))
				{
					var name = extractedString.Split(':')[1];

					name = Regex.Replace(name, @"[\W]", string.Empty).ToLower();
					name = Scraper.FindClosestCharacterName(name);

					return name;
				}
			}
			// artifact has no equipped character
			return null;
		}

		#endregion Task Methods
	}
}