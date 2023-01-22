using Accord.Imaging;
using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static InventoryKamera.Artifact;

namespace InventoryKamera
{
    public static class ArtifactScraper
	{
		private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		private static bool SortByLevel;
		public static bool StopScanning { get; set; }

		public static void ScanArtifacts(int count = 0)
		{
			// Get Max artifacts from screen
			int artifactCount = count == 0 ? ScanArtifactCount() : count;
			int page = 0;
			var (rectangles, cols, rows) = GetPageOfItems(page);
			int fullPage = cols * rows;
			int totalRows = (int)Math.Ceiling(artifactCount / (decimal)cols);
			int cardsQueued = 0;
			int rowsQueued = 0;
			int offset = 0;
			UserInterface.SetArtifact_Max(artifactCount);

			StopScanning = false;

			Logger.Info("Found {0} for artifact count.", artifactCount);

			SortByLevel = Properties.Settings.Default.MinimumArtifactLevel > 0;

			if (SortByLevel)
			{
				Logger.Debug("Sorting by level to optimize total scan time");
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
						Y: (int)(615 / 720.0 * Navigation.GetHeight()));
					Navigation.Click();
					Navigation.Wait();
				}
				Logger.Debug("Inventory is sorted by level.");
			}
			else
            {
				Logger.Debug("Sorting by quality to scan all artifacts matching quality filter.");
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
						Y: (int)(645 / 720.0 * Navigation.GetHeight()));
					Navigation.Click();
					Navigation.Wait();
				}
				Logger.Debug("Inventory is sorted by quality");
			}

			// Go through artifact list
			while (cardsQueued < artifactCount)
			{
				Logger.Debug("Scanning artifact page {0}", page);
				Logger.Debug("Located {0} possible item locations on page.", rectangles.Count);

				int cardsRemaining = artifactCount - cardsQueued;
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
					if (cardsQueued >= artifactCount || StopScanning)
					{
						if (StopScanning) Logger.Info("Stopping artifact scan based on filtering");
						else Logger.Info("Stopping artifact scan based on scans queued ({0} of {1})", cardsQueued, artifactCount);
						return;
					}
				}

				Logger.Debug("Finished queuing page of artifacts. Scrolling...");

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
				x: (int)(140.0 / 1280.0 * Navigation.GetWidth()),
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

        private static int ScanArtifactCount()
		{
			//Find artifact count
			var region = new Rectangle(
				x: (int)(1030 / 1280.0 * Navigation.GetWidth()),
				y: (int)(20 / 720.0 * Navigation.GetHeight()),
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
					countBitmap.Save($"./logging/artifacts/ArtifactCount.png");
					Navigation.CaptureWindow().Save($"./logging/artifacts/ArtifactWindow_{Navigation.GetWidth()}x{Navigation.GetHeight()}.png");
					if (string.IsNullOrWhiteSpace(text)) throw new FormatException("Unable to locate artifact count.");
				}

				int count;

				// Check for slash
				if (Regex.IsMatch(text, "/"))
				{
					count = int.Parse(text.Split('/')[0]);
				}
				else if (Regex.Matches(text, "1500").Count == 1) // Remove the inventory limit from number
				{
					text = text.Replace("1500", string.Empty);
					count = int.Parse(text);
				}
				else // Extreme worst case
				{
					count = 1500;
					Logger.Debug("Defaulted to 1500 for artifact count");
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
					if (cols != 8 || rows < 5 || Properties.Settings.Default.LogScreenshots)
					{
						// Generated rectangles
						screenshot.Save($"./logging/artifacts/ArtifactInventory.png");
						using (Graphics g = Graphics.FromImage(screenshot))
							rectangles.ForEach(r => g.DrawRectangle(new Pen(Color.Green, 2), r));
						screenshot.Save($"./logging/artifacts/ArtifactInventory{page}_{cols}x{rows}.png");
					}
					return (rectangles, cols, rows);
				}
				catch (Exception)
				{ 
					screenshot.Save($"./logging/artifacts/ArtifactInventory.png");
					throw;
				}
				
			}
		}

		public static (List<Rectangle> rectangles, int cols, int rows) ProcessScreenshot(Bitmap screenshot)
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
				MinHeight = card.Height - ((int)(card.Height * 0.2)),
				MaxHeight = card.Height + ((int)(card.Height * 0.2)),
				MinWidth = card.Width - ((int)(card.Width * 0.2)),
				MaxWidth = card.Width + ((int)(card.Width * 0.2)),
			})
			{
				// Image pre-processing
				screenshot = new KirschEdgeDetector().Apply(screenshot); // Algorithm to find edges. Really good but can take ~1s
				screenshot = new Grayscale(0.2125, 0.7154, 0.0721).Apply(screenshot);
				screenshot = new Threshold(100).Apply(screenshot); // Convert to black and white only based on pixel intensity			


				blobCounter.ProcessImage(screenshot);
				// Note: Processing won't always detect all item rectangles on screen. Since the
				// background isn't a solid color it's a bit trickier to filter out.

				if (blobCounter.ObjectsCount < 7)
				{
					throw new Exception("Insufficient items found in artifact inventory");
				}

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
						int x = (int)( col - (minWidth * .5) );
						int y = (int)( row - (minHeight * .5) );

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

			// Separate to all pieces of artifact and add to pics
			List<Bitmap> artifactImages = new List<Bitmap>();

			Bitmap card;
			RECT reference;
			Bitmap name, gearSlot, mainStat, subStats, level, equipped, locked;

			if (Navigation.GetAspectRatio() == new Size(16, 9))
			{
				reference = new RECT(new Rectangle(872, 80, 327, 560));

				int left   = (int)Math.Round(reference.Left   / 1280.0 * width, MidpointRounding.AwayFromZero);
				int top    = (int)Math.Round(reference.Top    / 720.0 * height, MidpointRounding.AwayFromZero);
				int right  = (int)Math.Round(reference.Right  / 1280.0 * width, MidpointRounding.AwayFromZero);
				int bottom = (int)Math.Round(reference.Bottom / 720.0 * height, MidpointRounding.AwayFromZero);

				card = Navigation.CaptureRegion(new RECT(left, top, right, bottom));

				equipped = card.Clone(new RECT(
					Left: (int)( 50.0 / reference.Width * card.Width ),
					Top: (int)( 522.0 / reference.Height * card.Height ),
					Right: card.Width,
					Bottom: card.Height), card.PixelFormat);
			}
			else // if (Navigation.GetAspectRatio() == new Size(8, 5))
			{
				reference = new RECT(new Rectangle(872, 80, 327, 640));

				int left   = (int)Math.Round(reference.Left   / 1280.0 * width, MidpointRounding.AwayFromZero);
				int top    = (int)Math.Round(reference.Top    / 800.0 * height, MidpointRounding.AwayFromZero);
				int right  = (int)Math.Round(reference.Right  / 1280.0 * width, MidpointRounding.AwayFromZero);
				int bottom = (int)Math.Round(reference.Bottom / 800.0 * height, MidpointRounding.AwayFromZero);

				card = Navigation.CaptureRegion(new RECT(left, top, right, bottom));

				equipped = card.Clone(new RECT(
					Left: (int)( 50.0 / reference.Width * card.Width ),
					Top: (int)( 602.0 / reference.Height * card.Height ),
					Right: card.Width,
					Bottom: card.Height), card.PixelFormat);
			}

			gearSlot = card.Clone(new RECT(
				Left: (int)( 3.0 / reference.Width * card.Width ),
				Top: (int)( 46.0 / reference.Height * card.Height ),
				Right: (int)( ( ( reference.Width / 2.0 ) + 20 ) / reference.Width * card.Width ),
				Bottom: (int)( 66.0 / reference.Height * card.Height )), card.PixelFormat);

			mainStat = card.Clone(new RECT(
				Left: 0,
				Top: (int)( 100.0 / reference.Height * card.Height ),
				Right: (int)( ( ( reference.Width / 2.0 ) + 20 ) / reference.Width * card.Width ),
				Bottom: (int)( 120.0 / reference.Height * card.Height )), card.PixelFormat);

			level = card.Clone(new RECT(
				Left: (int)( 18.0 / reference.Width * card.Width ),
				Top: (int)( 203.0 / reference.Height * card.Height ),
				Right: (int)( 61.0 / reference.Width * card.Width ),
				Bottom: (int)( 228.0 / reference.Height * card.Height )), card.PixelFormat);

			subStats = card.Clone(new RECT(
				Left: 0,
				Top: (int)( 235.0 / reference.Height * card.Height ),
				Right: card.Width,
				Bottom: (int)( 370.0 / reference.Height * card.Height )), card.PixelFormat);

			locked = card.Clone(new RECT(
				Left: (int)( 284.0 / reference.Width * card.Width ),
				Top: (int)( 201.0 / reference.Height * card.Height ),
				Right: (int)( 312.0 / reference.Width * card.Width ),
				Bottom: (int)( 228.0 / reference.Height * card.Height )), card.PixelFormat);

			name = card.Clone(new RECT(
				Left: 0,
				Top: 0,
				Right: card.Width,
				Bottom: (int)( 38.0 / reference.Height * card.Height )), card.PixelFormat);


			// Add all to artifact Images
			artifactImages.Add(name); // 0
			artifactImages.Add(gearSlot);
			artifactImages.Add(mainStat);
			artifactImages.Add(level);
			artifactImages.Add(subStats);
			artifactImages.Add(equipped); // 5
			artifactImages.Add(locked);
			artifactImages.Add(card);


            bool belowRarity = GetRarity(name) < Properties.Settings.Default.MinimumArtifactRarity;
            bool belowLevel = ScanArtifactLevel(level) < Properties.Settings.Default.MinimumArtifactLevel;
            StopScanning = (SortByLevel && belowLevel) || (!SortByLevel && belowRarity);

			if (StopScanning || belowRarity || belowLevel)
            {
				artifactImages.ForEach(i => i.Dispose());
				return;
            }
            // Send images to Worker Queue
            InventoryKamera.workerQueue.Enqueue(new OCRImageCollection(artifactImages, "artifact", id));
        }

        public static async Task<Artifact> CatalogueFromBitmapsAsync(List<Bitmap> bm, int id)
		{
			// Init Variables
			string gearSlot = null;
			string mainStat = null;
			string setName = null;
			string equippedCharacter = null;
			List<SubStat> subStats = new List<SubStat>();
			int rarity = 0;
			int level = 0;
			bool _lock = false;

			if (bm.Count >= 6)
			{
				int a_name = 0; int a_gearSlot = 1; int a_mainStat = 2; int a_level = 3; int a_subStats = 4; int a_equippedCharacter = 5; int a_lock = 6; 
				// Get Rarity
				rarity = GetRarity(bm[a_name]);

				// Check for equipped color
				Color equippedColor = Color.FromArgb(255, 255, 231, 187);
				Color equippedStatus = bm[a_equippedCharacter].GetPixel(5, 5);
				bool b_equipped = Scraper.CompareColors(equippedColor, equippedStatus);

				// Check for lock color
				Color lockedColor = Color.FromArgb(255, 70, 80, 100); // Dark area around red lock
				Color lockStatus = bm[a_lock].GetPixel(5, 5);
				_lock = Scraper.CompareColors(lockedColor, lockStatus);

				// Improved Scanning using multi threading
				List<Task> tasks = new List<Task>();

				var taskGear  = Task.Run(() => gearSlot = ScanArtifactGearSlot(bm[a_gearSlot]));
				var taskMain  = taskGear.ContinueWith( (antecedent) => mainStat = ScanArtifactMainStat(bm[a_mainStat], antecedent.Result));
				var taskLevel = Task.Run(() => level = ScanArtifactLevel(bm[a_level]));
				var taskSubs  = Task.Run(() => subStats = ScanArtifactSubStats(bm[a_subStats]));
				var taskEquip = Task.Run(() => equippedCharacter = ScanArtifactEquippedCharacter(bm[a_equippedCharacter]));
				var taskName = Task.Run(() => setName = ScanArtifactSet(bm[a_name]));

				tasks.Add(taskGear);
				tasks.Add(taskMain);
				tasks.Add(taskLevel);
				tasks.Add(taskSubs);
				tasks.Add(taskName);
				if (b_equipped)
				{
					tasks.Add(taskEquip);
				}

				await Task.WhenAll(tasks.ToArray());
			}
			if (!Properties.Settings.Default.EquipArtifacts) equippedCharacter = "";

			return new Artifact(setName, rarity, level, gearSlot, mainStat, subStats.ToArray(), subStats.Count, equippedCharacter, id, _lock);
		}

		private static int GetRarity(Bitmap bm)
		{
			var averageColor = new ImageStatistics(bm);

			Color fiveStar = Color.FromArgb(255, 188, 105, 50);
			Color fourStar = Color.FromArgb(255, 161, 86, 224);
			Color threeStar = Color.FromArgb(255, 81, 127, 203);
			Color twoStar = Color.FromArgb(255, 42, 143, 114);
			Color oneStar = Color.FromArgb(255, 114, 119, 138);

			var colors = new List<Color> { Color.Black, oneStar, twoStar, threeStar, fourStar, fiveStar };

			var c = Scraper.ClosestColor(colors, averageColor);

			return colors.IndexOf(c);
		}

		public static bool IsEnhancementMaterial(Bitmap card)
		{
			RECT reference = Navigation.GetAspectRatio() == new Size(16, 9) ?
				new RECT(new Rectangle(862, 80, 327, 560)) : (RECT)new Rectangle(862, 80, 328, 640);
			Bitmap nameBitmap = card.Clone(new RECT(
				Left: 0,
				Top: 0,
				Right: card.Width,
				Bottom: (int)( 38.0 / reference.Height * card.Height )), card.PixelFormat);
			string material = ScanEnhancementMaterialName(nameBitmap);
			return !string.IsNullOrWhiteSpace(material) && Scraper.enhancementMaterials.Contains(material.ToLower());
		}

		private static string ScanEnhancementMaterialName(Bitmap bm)
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

		private static string ScanArtifactGearSlot(Bitmap bm)
		{
			// Process Img
			Bitmap n = Scraper.ConvertToGrayscale(bm);
			Scraper.SetContrast(80.0, ref n);
			Scraper.SetInvert(ref n);

			string gearSlot = Scraper.AnalyzeText(n).Trim().ToLower();
			gearSlot = Regex.Replace(gearSlot, @"[\W_]", string.Empty);
			gearSlot = Scraper.FindClosestGearSlot(gearSlot);
			n.Dispose();
			return gearSlot;
		}

		private static string ScanArtifactMainStat(Bitmap bm, string gearSlot)
		{
			switch (gearSlot)
			{
				// Flower of Life. Flat HP
				case "flower":
					return Scraper.Stats["hp"];

				// Plume of Death. Flat ATK
				case "plume":
					return Scraper.Stats["atk"];

				// Otherwise it's either sands, goblet or circlet.
				default:

					Scraper.SetContrast(100.0, ref bm);
					Bitmap n = Scraper.ConvertToGrayscale(bm);
					Scraper.SetThreshold(135, ref n);
					Scraper.SetInvert(ref n);

					// Get Main Stat
					string mainStat = Scraper.AnalyzeText(n).ToLower().Trim();
					

					// Remove anything not a-z as well as removes spaces/underscores
					mainStat = Regex.Replace(mainStat, @"[\W_0-9]", string.Empty);
					// Replace double characters (ex. aanemodmgbonus). Seemed to be a somewhat common problem.
					mainStat = Regex.Replace(mainStat, "(.)\\1+", "$1");
					mainStat = Scraper.FindClosestStat(mainStat);

					if (mainStat == "def" || mainStat == "atk" || mainStat == "hp")
					{
						mainStat += "_";
					}
					n.Dispose();
					return mainStat;
			}
		}

		private static int ScanArtifactLevel(Bitmap bm)
		{
			// Process Img
			Bitmap n = Scraper.ConvertToGrayscale(bm);
			Scraper.SetContrast(80.0, ref n);
			Scraper.SetInvert(ref n);

			// numbersOnly = true => seems to interpret the '+' as a '4'
			string text = Scraper.AnalyzeText(n, Tesseract.PageSegMode.SingleWord).Trim().ToLower();
			n.Dispose();

			// Get rid of all non digits
			text = Regex.Replace(text, @"[\D]", string.Empty);

			return int.TryParse(text, out int level) ? level : -1;
		}

		private static List<SubStat> ScanArtifactSubStats(Bitmap artifactImage)
		{
			Bitmap bm = (Bitmap)artifactImage.Clone();
			Scraper.SetBrightness(-30, ref bm);
			Scraper.SetContrast(85, ref bm);
			var n = Scraper.ConvertToGrayscale(bm);
			var text = Scraper.AnalyzeText(n, Tesseract.PageSegMode.Auto).ToLower();

			List<string> lines = new List<string>(text.Split('\n'));
			lines.RemoveAll(line => string.IsNullOrWhiteSpace(line));

			var index = lines.FindIndex(line => line.Contains("piece") || line.Contains("set") || line.Contains("2-"));
			if (index >= 0)
			{
				lines.RemoveRange(index, lines.Count - index);
			}

			n.Dispose();
			bm.Dispose();
			SubStat[] substats = new SubStat[4];
			List<Task<string>> tasks = new List<Task<string>>();
			for (int i = 0; i < lines.Count; i++)
			{
				int j = i;
				var task = Task.Factory.StartNew(() =>
				{
					var line = Regex.Replace(lines[j], @"(?:^[^a-zA-Z]*)", string.Empty).Replace(" ", string.Empty);

					if (line.Any(char.IsDigit))
					{
						SubStat substat = new SubStat();
						Regex re = new Regex(@"([\w]+\W*)(\d+.*\d+)");
						var result = re.Match(line);
						var stat = Regex.Replace(result.Groups[1].Value, @"[^\w]", string.Empty);
						var value = result.Groups[2].Value;

						string name = line.Contains("%") ? stat + "%" : stat;

						substat.stat = Scraper.FindClosestStat(name) ?? "";

						// Remove any non digits.
					    value = Regex.Replace(value, @"[^0-9]", string.Empty);

						var cultureInfo = new CultureInfo("en-US");
						if (!decimal.TryParse(value, NumberStyles.Number, cultureInfo, out substat.value))
						{
							substat.value = -1;
						}

						// Need to retain the decimal place for percent boosts
						if (substat.stat.Contains("_")) substat.value /= 10;

						substats[j] = substat;
						return null;
					}
					else // if (line.Contains(":")) // Sometimes Tesseract wouldn't detect a ':' making this check troublesome
					{
						var name = line.Trim().ToLower();

						name = Regex.Replace(name, @"[^\w]", string.Empty);

						name = Scraper.FindClosestSetName(name);

						return !string.IsNullOrWhiteSpace(name) ? name : null;
					}
				});
				tasks.Add(task);
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while (tasks.Count > 0 && stopwatch.Elapsed.TotalSeconds < 10)
			{
				for (int i = 0; i < tasks.Count; i++)
				{
					Task<string> task = tasks[i];
					if (!task.IsCompleted)
					{
						continue;
					}
					tasks.Remove(task);
					break;
				}
			}
			return substats.ToList();
		}

		private static string ScanArtifactEquippedCharacter(Bitmap bm)
		{
			Bitmap n = Scraper.ConvertToGrayscale(bm);
			Scraper.SetContrast(60.0, ref n);

			string equippedCharacter = Scraper.AnalyzeText(n).ToLower();
			n.Dispose();

			if (equippedCharacter != "")
			{
				if (equippedCharacter.Contains(":"))
				{
					equippedCharacter = Regex.Replace(equippedCharacter.Split(':')[1], @"[\W]", string.Empty);
					equippedCharacter = Scraper.FindClosestCharacterName(equippedCharacter);

					return equippedCharacter;
				}
			}
			// artifact has no equipped character
			return null;
		}

		private static string ScanArtifactSet(Bitmap itemName)
        {
            Scraper.SetGamma(0.2, 0.2, 0.2, ref itemName);
            Bitmap grayscale = Scraper.ConvertToGrayscale(itemName);
            Scraper.SetInvert(ref grayscale);

            // Analyze
            using (Bitmap padded = new Bitmap((int)(grayscale.Width + grayscale.Width * .1), grayscale.Height + (int)(grayscale.Height * .5)))
            {
                using (Graphics g = Graphics.FromImage(padded))
                {
                    g.Clear(Color.White);
                    g.DrawImage(grayscale, (padded.Width - grayscale.Width) / 2, (padded.Height - grayscale.Height) / 2);

                    var scannedText = Scraper.AnalyzeText(grayscale, Tesseract.PageSegMode.Auto).ToLower().Replace("\n", " ");
                    string text = Regex.Replace(scannedText, @"[\W]", string.Empty);
                    text = Scraper.FindClosestArtifactSetFromArtifactName(text);

					grayscale.Dispose();

					return text;
                }
            }
        }

        #endregion Task Methods
    }
}