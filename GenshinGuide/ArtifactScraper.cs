using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using static GenshinGuide.Artifact;

namespace GenshinGuide
{
	public static class ArtifactScraper
	{
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

			// Go through artifact list
			while (cardsQueued < artifactCount)
			{
				int cardsRemaining = artifactCount - cardsQueued;
				for (int i = cardsRemaining < fullPage ? ( rows - ( totalRows - rowsQueued ) ) * cols : 0; i < rectangles.Count; i++)
				{
					Rectangle item = rectangles[i];
					Navigation.SetCursorPos(Navigation.GetPosition().Left + item.Center().X, Navigation.GetPosition().Top + item.Center().Y + offset);
					Navigation.sim.Mouse.LeftButtonClick();
					Navigation.SystemRandomWait(Navigation.Speed.SelectNextInventoryItem);

					// Queue card for scanning
					QueueScan(cardsQueued);
					cardsQueued++;
					if (cardsQueued >= artifactCount)
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
			int left = (int)(1028 / 1280.0 * Navigation.GetWidth());
			int top = (int)(20 / 720.0 * Navigation.GetHeight());
			int right = (int)(1208 / 1280.0 * Navigation.GetWidth());
			int bottom = (int)(45 / 720.0 * Navigation.GetHeight());

			using (Bitmap bm = Navigation.CaptureRegion(new RECT(left, top, right, bottom)))
			{
				UserInterface.SetNavigation_Image(bm);

				Bitmap nBM = Scraper.ConvertToGrayscale(bm);
				Scraper.SetContrast(60.0, ref nBM);
				Scraper.SetInvert(ref nBM);

				string text = Scraper.AnalyzeText(nBM).Trim();
				nBM.Dispose();
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
					count = Int32.Parse(text) / 1500;
				}

				// Check if larger than 1500
				while (count > 1500)
				{
					count /= 10;
				}

				return count;
			}
		}

		private static (List<Rectangle> rectangles, int cols, int rows) GetPageOfItems()
		{
			var card = new RECT(
				Left: 0,
				Top: 0,
				Right: (int)(80 / 1280.0 * Navigation.GetWidth()),
				Bottom: (int)(100 / 720.0 * Navigation.GetHeight()));

			// Filter for relative size of items in inventory, give or take a few pixels
			BlobCounter blobCounter = new BlobCounter
			{
				FilterBlobs = true,
				MinHeight = card.Height - 15,
				MaxHeight = card.Height + 15,
				MinWidth  = card.Width - 15,
				MaxWidth  = card.Width + 15,
			};

			// Screenshot of inventory
			Bitmap screenshot = Navigation.CaptureWindow();
			Bitmap output = new Bitmap(screenshot); // Copy used to overlay onto in testing

			// Image pre-processing
			ContrastCorrection contrast = new ContrastCorrection(80);
			Grayscale grayscale = new Grayscale(0.2125, 0.7154, 0.0721);
			Edges edges = new Edges();
			Threshold threshold = new Threshold(15);
			FillHoles holes = new FillHoles
			{
				CoupledSizeFiltering = true,
				MaxHoleWidth = card.Width + 10,
				MaxHoleHeight = card.Height + 10
			};
			SobelEdgeDetector sobel = new SobelEdgeDetector();

			screenshot = contrast.Apply(screenshot);
			screenshot = edges.Apply(screenshot); // Quick way to find ~75% of edges
			screenshot = grayscale.Apply(screenshot);
			screenshot = threshold.Apply(screenshot); // Convert to black and white only based on pixel intensity

			screenshot = sobel.Apply(screenshot); // Find some more edges
			screenshot = holes.Apply(screenshot); // Fill shapes
			screenshot = sobel.Apply(screenshot); // Find edges of those shapes. A second pass removes edges within item card

			blobCounter.ProcessImage(screenshot);
			// Note: Processing won't always detect all item rectangles on screen. Since the
			// background isn't a solid color it's a bit trickier to filter out.

			if (blobCounter.ObjectsCount < 1)
			{
				throw new Exception("No items detected in inventory");
			}

			// Don't save overlapping blobs
			List<Rectangle> rectangles = new List<Rectangle>();
			List<Rectangle> blobRects = blobCounter.GetObjectsRectangles().ToList();

			int sWidth = blobRects[0].Width;
			int sHeight = blobRects[0].Height;
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
					sWidth = Math.Min(sWidth, rect.Width);
					sHeight = Math.Min(sHeight, rect.Height);
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
					if (x - 10 <= xC && xC <= x + 10)
					{
						addX = false;
						break;
					}
				}
				foreach (var y in rowCoords)
				{
					var yC = item.Center().Y;
					if (y - 10 <= yC && yC <= y + 10)
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

			// Clear it all because we're going to use X,Y coordinate pairings to build rectangles
			// around. This won't be perfect but it should algorithmically put rectangles over all
			// images on the screen. The center of each of these rectangles should be a good enough
			// spot to click.
			rectangles.Clear();
			colCoords.Sort();
			rowCoords.Sort();
			foreach (var row in rowCoords)
			{
				foreach (var col in colCoords)
				{
					int x = (int)( col - (sWidth * .5) );
					int y = (int)( row - (sHeight * .5) );

					rectangles.Add(new Rectangle(x, y, sWidth, sHeight));
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

			Debug.WriteLine($"{colCoords.Count} columns");
			Debug.WriteLine($"{rowCoords.Count} rows");
			Debug.WriteLine($"{rectangles.Count} rectangles");

			//new RectanglesMarker(rectangles, Color.Green).ApplyInPlace(output);
			//Navigation.DisplayBitmap(output, "Rectangles");
			//screenshot.Dispose();
			//output.Dispose();

			return (rectangles, colCoords.Count, rowCoords.Count);
		}

		public static void QueueScan(int id)
		{
			int width = Navigation.GetWidth();
			int height = Navigation.GetHeight();

			// Separate to all pieces of artifact and add to pics
			List<Bitmap> artifactImages = new List<Bitmap>();

			Bitmap card;
			RECT reference;
			Bitmap name, gearSlot, mainStat, subStats, level, equipped, rarity, locked;

			int left, top, right, bottom;

			if (Navigation.GetAspectRatio() == new Size(16, 9))
			{
				reference = new RECT(new Rectangle(862, 80, 327, 565));

				left   = (int)Math.Round(reference.Left   / 1280.0 * width, MidpointRounding.AwayFromZero);
				top    = (int)Math.Round(reference.Top    / 720.0 * height, MidpointRounding.AwayFromZero);
				right  = (int)Math.Round(reference.Right  / 1280.0 * width, MidpointRounding.AwayFromZero);
				bottom = (int)Math.Round(reference.Bottom / 720.0 * height, MidpointRounding.AwayFromZero);

				card = Navigation.CaptureRegion(new RECT(left, top, right, bottom));

				equipped = card.Clone(new RECT(
					Left: (int)( 50.0 / reference.Width * card.Width ),
					Top: (int)( 522.0 / reference.Height * card.Height ),
					Right: card.Width,
					Bottom: card.Height), card.PixelFormat);
			}
			else if (Navigation.GetAspectRatio() == new Size(8, 5))
			{
				reference = new Rectangle(862, 80, 327, 640);

				left   = (int)Math.Round(reference.Left   / 1280.0 * width, MidpointRounding.AwayFromZero);
				top    = (int)Math.Round(reference.Top    / 800.0 * height, MidpointRounding.AwayFromZero);
				right  = (int)Math.Round(reference.Right  / 1280.0 * width, MidpointRounding.AwayFromZero);
				bottom = (int)Math.Round(reference.Bottom / 800.0 * height, MidpointRounding.AwayFromZero);

				card = Navigation.CaptureRegion(new RECT(left, top, right, bottom));

				equipped = card.Clone(new RECT(
					Left: (int)( 50.0 / reference.Width * card.Width ),
					Top: (int)( 602.0 / reference.Height * card.Height ),
					Right: card.Width,
					Bottom: card.Height), card.PixelFormat);
			}
			else
			{
				throw new Exception("Unknown aspect ratio: " + Navigation.GetAspectRatio());
			}


			name = card.Clone(new RECT(
				Left: 0,
				Top: 0,
				Right: card.Width,
				Bottom: (int)( 38.0 / reference.Height * card.Height )), card.PixelFormat);
			//Navigation.DisplayBitmap(name);

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
				Bottom: (int)( 373.0 / reference.Height * card.Height )), card.PixelFormat);
			//Navigation.DisplayBitmap(subStats);

			// Rarity
			rarity = card.Clone(new RECT(
				Left: 1,
				Top: 0,
				Right: card.Width,
				Bottom: 1), card.PixelFormat);
			//Navigation.DisplayBitmap(rarity);

			// Locked Status
			locked = card.Clone(new RECT(
				Left: (int)( 284.0 / reference.Width * card.Width ),
				Top: (int)( 201.0 / reference.Height * card.Height ),
				Right: (int)( 312.0 / reference.Width * card.Width ),
				Bottom: (int)( 228.0 / reference.Height * card.Height )), card.PixelFormat);
			
			// Add all to artifact Images
			artifactImages.Add(gearSlot); // 0
			artifactImages.Add(mainStat);
			artifactImages.Add(level);
			artifactImages.Add(subStats);
			artifactImages.Add(equipped);
			artifactImages.Add(rarity);
			artifactImages.Add(locked); //6

			// Send Image to Worker Queue
			GenshinData.workerQueue.Enqueue(new OCRImage(artifactImages, "artifact", id));
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

			if (bm.Count == 7)
			{
				int a_gearSlot = 0; int a_mainStat = 1; int a_level = 2; int a_subStats = 3; int a_equippedCharacter = 4; int a_rarity = 5; int a_lock = 6;

				// Get Rarity
				Color rarityColor = bm[a_rarity].GetPixel(0, 0);

				Color fiveStar = Color.FromArgb(255, 188, 105, 50);
				Color fourStar = Color.FromArgb(255, 161, 86, 224);
				Color threeStar = Color.FromArgb(255, 81, 127, 203);
				Color twoStar = Color.FromArgb(255, 42, 143, 114);
				Color oneStar = Color.FromArgb(255, 114, 119, 138);

				// Check for equipped color
				Color equippedColor = Color.FromArgb(255, 255, 231, 187);
				Color equippedStatus = bm[a_equippedCharacter].GetPixel(5, 5);

				// Check for lock color
				Color lockedColor = Color.FromArgb(255, 70, 80, 100); // Dark area around red lock
				Color lockStatus = bm[a_lock].GetPixel(5, 5);

				// Only scan 4 and 5-star artifacts
				if (Scraper.CompareColors(fiveStar, rarityColor) || Scraper.CompareColors(fourStar, rarityColor))
				{
					rarity = Scraper.CompareColors(fiveStar, rarityColor) ? 5 : 4;
					bool b_equipped = Scraper.CompareColors(equippedColor, equippedStatus);
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
				else if (Scraper.CompareColors(threeStar, rarityColor))
				{
					rarity = 3; equippedCharacter = null; setName = null; level = -1; gearSlot = null;
					//rarity = 3;
				}
				else if (Scraper.CompareColors(twoStar, rarityColor))
				{
					rarity = 2; equippedCharacter = null; setName = null; level = -1; gearSlot = null;
					//rarity = 2;
				}
				else if (Scraper.CompareColors(oneStar, rarityColor))
				{
					rarity = 1; equippedCharacter = null; setName = null; level = -1; gearSlot = null;
					//rarity = 1;
				}
				else
				{
					// Not found
					rarity = 0; equippedCharacter = null; setName = null; level = -1; gearSlot = null;
					//rarity = 0;
					UserInterface.AddError("Couldn't determine rarity for artifact");
				}
			}

			return new Artifact(rarity, gearSlot, mainStat, level, subStats.ToArray(), subStats.Count, setName, equippedCharacter, id, _lock);
		}

		#region Threaded Functions

		private static string ScanArtifactGearSlot(Bitmap bm)
		{
			// Process Img
			Bitmap n = Scraper.ConvertToGrayscale(bm);
			Scraper.SetContrast(80.0, ref n);
			Scraper.SetInvert(ref n);

			string gearSlot = Scraper.AnalyzeText(n).Trim().ToLower();
			gearSlot = Regex.Replace(gearSlot, @"[\W_]", "");
			n.Dispose();
			return gearSlot;
		}

		private static string ScanArtifactMainStat(Bitmap bm, string gearSlot)
		{
			switch (gearSlot)
			{
				// Flower of Life. Flat HP
				case "floweroflife":
					//Debug.WriteLine($"ScanArtifactMainStat runtime: {ts.Milliseconds}ms");
					return "hp_flat";

				// Plume of Death. Flat ATK
				case "plumeofdeath":
					//Debug.WriteLine($"ScanArtifactMainStat runtime: {ts.Milliseconds}ms");
					return "atk_flat";

				// Otherwise it's either sands, goblet or circlet.
				default:

					Scraper.SetContrast(100.0, ref bm);
					Bitmap n = Scraper.ConvertToGrayscale(bm);
					Scraper.SetThreshold(135, ref n);
					Scraper.SetInvert(ref n);

					// Get Main Stat
					string mainStat = Scraper.AnalyzeText(n).ToLower().Trim();

					// Remove anything not a-z as well as removes spaces/underscores
					mainStat = Regex.Replace(mainStat, @"[\W_0-9]", "");
					// Replace double characters (ex. aanemodmgbonus). Seemed to be a somewhat common problem.
					mainStat = Regex.Replace(mainStat, "(.)\\1+", "$1");

					//Debug.WriteLine($"ScanArtifactMainStat runtime: {ts.Milliseconds}ms");
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

			// Get rid of all non digits
			text = Regex.Replace(text, @"[\D]", "");

			//Debug.WriteLine($"ScanArtifactLevel runtime: {ts.Milliseconds}ms");
			n.Dispose();
			return int.TryParse(text, out int level) ? level : -1;
		}

		private static List<SubStat> ScanArtifactSubStats(Bitmap artifactImage, ref string setName)
		{

			var text = Scraper.AnalyzeText(artifactImage, Tesseract.PageSegMode.Auto).ToLower();
			List<string> lines = new List<string>(text.Split('\n'));
			lines.RemoveAll(line => string.IsNullOrEmpty(line) || line.Contains("set") || line.Contains("2-piece") || line.Contains("4-piece") || line.Contains("1-piece"));

			SubStat[] substats = new SubStat[4];
			List<Task<string>> tasks = new List<Task<string>>();
			int setTask = 0;
			for (int i = 0; i < lines.Count; i++)
			{
				int j = i;
				var task = Task.Factory.StartNew(() =>
				{
					var line = Regex.Replace(lines[j], @"(?:^[^a-zA-Z]*)", "").Replace(" ", "");

					if (line.Contains("+"))
					{
						SubStat stat = new SubStat();

						var split = line.Split('+');
						string optional = "";
						if (split[1].Contains('%'))
						{
							split[1] = split[1].Replace("%", "");
							if (split[0] == "atk" || split[0] == "def" || split[0] == "hp")
							{
								optional = "%";
							}
						}
						if (Scraper.stats.Contains(split[0]))
						{
							stat.stat = split[0] + optional;
						}
						if (!decimal.TryParse(split[1], out stat.value))
						{
							stat.value = -1;
							//UserInterface.AddError($"Failed to parse stat value for {stat.stat}");
						}
						substats[j] = stat;
						return null;
					}
					else// if (line.Contains(":")) // Sometimes Tesseract wouldn't detect a ':' making this check troublesome
					{
						var name = line.Trim().ToLower();

						name = Regex.Replace(name, @"[^\w]", "");

						name = Scraper.FindClosestSetName(name);

						return !string.IsNullOrEmpty(name) ? name : null;
					}
				});
				tasks.Add(task);
			}

			bool remove = false;
			while (tasks.Count > 0)
			{
				for (int i = 0; i < tasks.Count; i++)
				{
					Task<string> task = tasks[i];
					if (!task.IsCompleted)
					{
						continue;
					}
					if (!task.IsFaulted && task.Result != null)
					{
						setName = task.Result;
						setTask = i;
						remove = true;
					}
					if (task.IsCompleted && remove)
					{
						tasks.Remove(task);
						break;
					}
				}
			}
			
			// Reset any stats accidentally caught if they're below the set name
			for (int i = setTask; i < substats.Length; i++)
			{
				substats[i].stat = null;
				substats[i].value = 0;
			}
			
			
			//Debug.WriteLine($"ScanArtifactSubStats runtime: {ts.Milliseconds}ms");
			return substats.ToList();
		}

		private static string ScanArtifactEquippedCharacter(Bitmap bm)
		{
			Bitmap n = Scraper.ConvertToGrayscale(bm);
			Scraper.SetContrast(60.0, ref n);

			string equippedCharacter = Scraper.AnalyzeText(n).ToLower().Trim();
			equippedCharacter = Regex.Replace(equippedCharacter, @"[^\w:_]", "");

			if (equippedCharacter != "")
			{
				if (equippedCharacter.Contains(":"))
				{
					equippedCharacter = equippedCharacter.Split(':')[1].Trim();
					//UserInterface.SetGear_Equipped(bm, equippedCharacter);

					//Debug.WriteLine($"ScanArtifactEquippedCharacter runtime: {ts.Milliseconds}ms");
					return equippedCharacter;
				}
			}
			// artifact has no equipped character
			//Debug.WriteLine($"ScanArtifactEquippedCharacter runtime: {ts.Milliseconds}ms");
			n.Dispose();
			return null;
		}

		#endregion Threaded Functions
	}
}