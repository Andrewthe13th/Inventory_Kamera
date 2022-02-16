using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accord.Imaging;
using Accord.Imaging.Filters;
using static InventoryKamera.Artifact;

namespace InventoryKamera
{
	public static class ArtifactScraper
	{
		public static bool StopScanning { get; set; }

		public static void ScanArtifacts(int count = 0)
		{
			// Get Max artifacts from screen
			int artifactCount = count == 0 ? ScanArtifactCount() : count;
			var (rectangles, cols, rows) = GetPageOfItems();
			int fullPage = cols * rows;
			int totalRows = (int)Math.Ceiling(artifactCount / (decimal)cols);
			int cardsQueued = 0;
			int rowsQueued = 0;
			int offset = 0;
			UserInterface.SetArtifact_Max(artifactCount);

			StopScanning = false;

			// Go through artifact list
			while (cardsQueued < artifactCount)
			{
				int cardsRemaining = artifactCount - cardsQueued;
				// Go through each "page" of items and queue. In the event that not a full page of
				// items are scrolled to, offset the index of rectangle to start clicking from
				for (int i = cardsRemaining < fullPage ? ( rows - ( totalRows - rowsQueued ) ) * cols : 0; i < rectangles.Count; i++)
				{
					Rectangle item = rectangles[i];
					Navigation.SetCursorPos(Navigation.GetPosition().Left + item.Center().X, Navigation.GetPosition().Top + item.Center().Y + offset);
					Navigation.Click();
					Navigation.SystemRandomWait(Navigation.Speed.SelectNextInventoryItem);

					// Queue card for scanning
					QueueScan(cardsQueued);
					cardsQueued++;
					if (cardsQueued >= artifactCount || StopScanning)
					{
						return;
					}
				}

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
					}
					Navigation.SystemRandomWait(Navigation.Speed.Fast);
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
					}
					Navigation.SystemRandomWait(Navigation.Speed.Fast);
				}
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

				if (string.IsNullOrWhiteSpace(text))
				{
					countBitmap.Save($"./logging/artifacts/ArtifactCount.png");
					Navigation.CaptureWindow().Save($"./logging/artifacts/ArtifactWindow_{Navigation.GetWidth()}x{Navigation.GetHeight()}.png");
					throw new FormatException("Unable to locate artifact count.");
				}

				int count;

				// Check for slash
				if (Regex.IsMatch(text, "/"))
				{
					count = int.Parse(text.Split('/')[0]);
					Debug.WriteLine($"Parsed {count} for artifact count");
				}
				else if (Regex.Matches(text, "1500").Count == 1) // Remove the inventory limit from number
				{
					text = text.Replace("1500", string.Empty);
					count = int.Parse(text);
					Debug.WriteLine($"Parsed {count} for artifact count");
				}
				else // Extreme worst case
				{
					count = 1500;
					Debug.WriteLine("Defaulted to 1500 for artifact count");
				}

				return count;
			}
		}

		private static (List<Rectangle> rectangles, int cols, int rows) GetPageOfItems()
		{
			// Screenshot of inventory
			using (Bitmap screenshot = Navigation.CaptureWindow())
			{
				
				try
				{
					var (rectangles, cols, rows) = ProcessScreenshot(screenshot);
					if (cols != 7 || rows < 5 || true)
					{
						// Generated rectangles
						screenshot.Save($"./logging/artifacts/ArtifactInventoryIncomplete.png");
						using (Graphics g = Graphics.FromImage(screenshot))
							rectangles.ForEach(r => g.DrawRectangle(new Pen(Color.Green, 2), r));
						screenshot.Save($"./logging/artifacts/ArtifactInventory_{cols}x{rows}.png");
					}
					return (rectangles, cols, rows);
				}
				catch (Exception e)
				{
					screenshot.Save($"./logging/artifacts/ArtifactInventory.png");
					throw e;
				}
				
			}
		}

		public static (List<Rectangle> rectangles, int cols, int rows) ProcessScreenshot(Bitmap screenshot)
		{
			// Size of an item card is the same in 16:10 and 16:9. Also accounts for character icon and resolution size.
			var card = new RECT(
				Left: 0,
				Top: 0,
				Right: (int)(85 / 1280.0 * screenshot.Width),
				Bottom: (int)(105 / 720.0 * screenshot.Height));

			// Filter for relative size of items in inventory, give or take a few pixels
			using (BlobCounter blobCounter = new BlobCounter
			{
				FilterBlobs = true,
				MinHeight = card.Height - 15,
				MaxHeight = card.Height + 15,
				MinWidth = card.Width - 15,
				MaxWidth = card.Width + 15,
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
						if (x - 50 / 1280.0 * screenshot.Width <= xC && xC <= x + 50 / 1280.0 * screenshot.Width)
						{
							addX = false;
							break;
						}
					}
					foreach (var y in rowCoords)
					{
						var yC = item.Center().Y;
						if (y - 50 / 720.0 * screenshot.Height <= yC && yC <= y + 50 / 720.0 * screenshot.Height)
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

				Debug.WriteLine($"{colCoords.Count} columns: ");
				colCoords.ForEach(c => Debug.Write(c + ", ")); Debug.WriteLine("");
				Debug.WriteLine($"{rowCoords.Count} rows: ");
				rowCoords.ForEach(c => Debug.Write(c + ", ")); Debug.WriteLine("");
				Debug.WriteLine($"{rectangles.Count} rectangles");

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
			Bitmap gearSlot, mainStat, subStats, level, equipped, locked;

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

			//Navigation.DisplayBitmap(equipped);

			// GearSlot
			gearSlot = card.Clone(new RECT(
				Left: (int)( 3.0 / reference.Width * card.Width ),
				Top: (int)( 46.0 / reference.Height * card.Height ),
				Right: (int)( ( ( reference.Width / 2.0 ) + 20 ) / reference.Width * card.Width ),
				Bottom: (int)( 66.0 / reference.Height * card.Height )), card.PixelFormat);
			//Navigation.DisplayBitmap(gearSlot);

			// MainStat
			mainStat = card.Clone(new RECT(
				Left: 0,
				Top: (int)( 100.0 / reference.Height * card.Height ),
				Right: (int)( ( ( reference.Width / 2.0 ) + 20 ) / reference.Width * card.Width ),
				Bottom: (int)( 120.0 / reference.Height * card.Height )), card.PixelFormat);
			//Navigation.DisplayBitmap(mainStat);

			// Level
			level = card.Clone(new RECT(
				Left: (int)( 18.0 / reference.Width * card.Width ),
				Top: (int)( 203.0 / reference.Height * card.Height ),
				Right: (int)( 61.0 / reference.Width * card.Width ),
				Bottom: (int)( 228.0 / reference.Height * card.Height )), card.PixelFormat);
			//Navigation.DisplayBitmap(level);

			// SubStats
			subStats = card.Clone(new RECT(
				Left: 0,
				Top: (int)( 235.0 / reference.Height * card.Height ),
				Right: card.Width,
				Bottom: (int)( 370.0 / reference.Height * card.Height )), card.PixelFormat);
			//Navigation.DisplayBitmap(subStats);

			// Locked Status
			locked = card.Clone(new RECT(
				Left: (int)( 284.0 / reference.Width * card.Width ),
				Top: (int)( 201.0 / reference.Height * card.Height ),
				Right: (int)( 312.0 / reference.Width * card.Width ),
				Bottom: (int)( 228.0 / reference.Height * card.Height )), card.PixelFormat);
			//Navigation.DisplayBitmap(locked);

			// Add all to artifact Images
			artifactImages.Add(gearSlot); // 0
			artifactImages.Add(mainStat);
			artifactImages.Add(level);
			artifactImages.Add(subStats);
			artifactImages.Add(equipped);
			artifactImages.Add(locked); //5
			artifactImages.Add(card);

			try
			{
				int rarity = GetRarity(card);
				if (0 < rarity && rarity < Properties.Settings.Default.MinimumArtifactRarity)
				{
					artifactImages.ForEach(i => i.Dispose());
					StopScanning = true;
				}
				else // Send images to Worker Queue
					InventoryKamera.workerQueue.Enqueue(new OCRImage(artifactImages, "artifact", id));
			}
			catch (Exception ex)
			{
				UserInterface.AddError($"Unexpected error {ex.Message} for artifact ID#{id}");
				UserInterface.AddError($"{ex.StackTrace}");
				card.Save($"./logging/artifacts/artifact{id}.png");
			}
		}

		public static async Task<Artifact> CatalogueFromBitmapsAsync(List<Bitmap> bm, int id)
		{
			// Init Variables
			string gearSlot = null;
			string mainStat = null;

			int rarity = 0;

			int level = 0;

			List<SubStat> subStats = new List<SubStat>();

			string setName = null;

			string equippedCharacter = null;

			bool _lock = false;

			if (bm.Count >= 6)
			{
				int a_gearSlot = 0; int a_mainStat = 1; int a_level = 2; int a_subStats = 3; int a_equippedCharacter = 4; int a_lock = 5; int a_card = 6;

				// Get Rarity
				rarity = GetRarity(bm[a_card]);

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
				var taskSubs  = Task.Run(() => subStats = ScanArtifactSubStats(bm[a_subStats], ref setName));
				var taskEquip = Task.Run(() => equippedCharacter = ScanArtifactEquippedCharacter(bm[a_equippedCharacter]));

				tasks.Add(taskGear);
				tasks.Add(taskMain);
				tasks.Add(taskLevel);
				tasks.Add(taskSubs);
				if (b_equipped)
				{
					tasks.Add(taskEquip);
				}

				await Task.WhenAll(tasks.ToArray());
			}

			return new Artifact(setName, rarity, level, gearSlot, mainStat, subStats.ToArray(), subStats.Count, equippedCharacter, id, _lock);
		}

		private static int GetRarity(Bitmap bitmap)
		{
			int x = (int)(10/1280.0 * Navigation.GetWidth());
			int y = (int)(10/720.0 * Navigation.GetHeight());

			Color rarityColor = bitmap.GetPixel(x,y);

			Color fiveStar    = Color.FromArgb(255, 188, 105,  50);
			Color fourStar    = Color.FromArgb(255, 161,  86, 224);
			Color threeStar   = Color.FromArgb(255,  81, 127, 203);
			Color twoStar     = Color.FromArgb(255,  42, 143, 114);
			Color oneStar     = Color.FromArgb(255, 114, 119, 138);

			if (Scraper.CompareColors(fiveStar, rarityColor)) return 5;
			else if (Scraper.CompareColors(fourStar, rarityColor)) return 4;
			else if (Scraper.CompareColors(threeStar, rarityColor)) return 3;
			else if (Scraper.CompareColors(twoStar, rarityColor)) return 2;
			else if (Scraper.CompareColors(oneStar, rarityColor)) return 1;
			else return 0; // throw new ArgumentException("Unable to determine artifact rarity");
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
					n.Dispose();

					// Remove anything not a-z as well as removes spaces/underscores
					mainStat = Regex.Replace(mainStat, @"[\W_0-9]", string.Empty);
					// Replace double characters (ex. aanemodmgbonus). Seemed to be a somewhat common problem.
					mainStat = Regex.Replace(mainStat, "(.)\\1+", "$1");

					if (mainStat == "def" || mainStat == "atk" || mainStat == "hp")
					{
						mainStat += "%";
					}

					return Scraper.FindClosestStat(mainStat);
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

		private static List<SubStat> ScanArtifactSubStats(Bitmap artifactImage, ref string setName)
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
					setName = string.IsNullOrWhiteSpace(setName) ? task.Result : setName;
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

		#endregion Task Methods
	}
}