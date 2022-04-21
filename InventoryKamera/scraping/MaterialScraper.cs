using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Accord.Imaging;
using Accord.Imaging.Filters;

namespace InventoryKamera
{
	[Serializable]
	public struct Material : ISerializable
	{
		public string name;
		public int count;

		public Material(string _name, int _count)
		{
			name = _name;
			count = _count;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context) => info.AddValue(name, count);

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is Material material && name == material.name;
		}
	}

	public enum InventorySection
	{
		CharacterDevelopmentItems,
		Materials,
		Food,
		Furnishings,
	}

	public static class MaterialScraper
	{
		public static void Scan_Materials(InventorySection section, ref HashSet<Material> materials)
		{
			if (!materials.Contains(new Material("Mora", 0)))
			{
				materials.Add(new Material("Mora", ScanMora()));
			}

			int scrollCount = 0;

			Material material = new Material(null, 0);
			Material previousMaterial = new Material(null, -1);

			List<Rectangle> rectangles;
			int page = 0;

			// Keep scanning while not repeating any items names
			while (true)
			{
				int rows, cols;
				// Find all items on the screen
				(rectangles, cols, rows) = GetPageOfItems(section, page);

				// Remove last row. Sometimes the bottom of a page of items is caught which results
				// in a faded quantity that can't be parsed. Removing slightly increases the number of pages that
				// need to be scrolled but it's fine.
				var r = rectangles.Take(rectangles.Count() - cols).ToList();

				foreach (var rectangle in r)
				{
					// Select Material
					Navigation.SetCursorPos(Navigation.GetPosition().Left + rectangle.Center().X, Navigation.GetPosition().Top + rectangle.Center().Y);
					Navigation.Click();
					Navigation.SystemRandomWait(Navigation.Speed.SelectNextInventoryItem);

					material.name = ScanMaterialName(section, out Bitmap nameplate);
					material.count = 0;

					// Check if new material has been found
					if (materials.Contains(material))
					{
						goto LastPage;
					}
					else
					{
						if (!string.IsNullOrEmpty(material.name))
						{
							// Scan Material Number
							material.count = ScanMaterialCount(rectangle, out Bitmap quantity);
							if (material.count == 0)
							{
								UserInterface.AddError($"Failed to parse quantity for {material.name}");
								quantity.Save($"./logging/materials/{material.name}_Quantity.png");
							}
							materials.Add(material);
							UserInterface.ResetCharacterDisplay();
							UserInterface.SetMaterial(nameplate, quantity, material.name, material.count);

							previousMaterial.name = material.name;
						}
					}
					nameplate.Dispose();
					Navigation.Wait(150);
				}

				Navigation.SetCursorPos(Navigation.GetPosition().Left + r.Last().Center().X, Navigation.GetPosition().Top + r.Last().Center().Y);
				Navigation.Click();
				Navigation.Wait(150);
				// Scroll to next page
				for (int i = 0; i < rows - 1; i++)
				{
					scrollCount++;

					// scroll down
					for (int k = 0; k < 10; k++)
					{
						Navigation.sim.Mouse.VerticalScroll(-1);
						// skip a scroll
						if (( k == 7 ) && ( ( scrollCount % 3 ) == 0 ))
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
						Navigation.SystemRandomWait(Navigation.Speed.InventoryScroll);
					}
				}
				Navigation.SystemRandomWait(Navigation.Speed.Normal);
				++page;
			}

		LastPage:
			// scroll down as much as possible
			for (int i = 0; i < 20; i++)
			{
				Navigation.sim.Mouse.VerticalScroll(-1);
				Navigation.SystemRandomWait(Navigation.Speed.InventoryScroll);
			}

			Navigation.Wait(500);

			(rectangles, _, _) = GetPageOfItems(section, page);
			bool passby = true;
			for (int i = rectangles.Count - 1; i >= 0; i--) // Click through but backwards to short-circuit after new materials
			{
				// Select Material
				Rectangle rectangle = rectangles[i];
				Navigation.SetCursorPos(Navigation.GetPosition().Left + rectangle.Center().X, Navigation.GetPosition().Top + rectangle.Center().Y);
				Navigation.Click();
				Navigation.SystemRandomWait(Navigation.Speed.SelectNextInventoryItem);

				material.name = ScanMaterialName(section, out Bitmap nameplate);
				material.count = 0;

				if (materials.Contains(material) && passby) continue;

				if (!materials.Contains(material))
				{
					if (!string.IsNullOrEmpty(material.name))
					{
						// Scan Material Number
						material.count = ScanMaterialCount(rectangle, out Bitmap quantity);
						if (material.count == 0)
						{
							UserInterface.AddError($"Failed to parse quantity for {material.name}");
							quantity.Save($"./logging/materials/{material.name}_Quantity.png");
						}
						materials.Add(material);
						UserInterface.ResetCharacterDisplay();
						UserInterface.SetMaterial(nameplate, quantity, material.name, material.count);
						passby = false; // New material found so break on next old material
						quantity.Dispose();
					}
				}
				else
				{
					nameplate.Dispose();
					break;
				}
				Navigation.Wait(150);
			}
		}

		private static int ScanMora()
		{
			var region = new Rectangle(
				x: (int)(125 / 1280.0 * Navigation.GetWidth()),
				y: (int)(665 / 720.0 * Navigation.GetHeight()),
				width: (int)(300 / 1280.0 * Navigation.GetWidth()),
				height: (int)(30 / 720.0 * Navigation.GetHeight()));

			if (Navigation.GetAspectRatio() == new Size(8, 5))
			{
				region.Y = (int)( 740 / 800.0 * Navigation.GetHeight() );
			}

			using (var screenshot = Navigation.CaptureRegion(region))
			{
				string mora = ParseMoraFromScreenshot(screenshot);

				if (int.TryParse(mora, out int count))
				{
					UserInterface.ResetCharacterDisplay();
					UserInterface.SetMora(screenshot, count);
				}
				else
				{
					UserInterface.SetNavigation_Image(screenshot);
					UserInterface.AddError("Unable to parse mora count");
					screenshot.Save("./logging/materials/mora.png");
				}
				return count;
			}
		}

		public static string ParseMoraFromScreenshot(Bitmap screenshot)
		{
			using (var gray = Scraper.ConvertToGrayscale(screenshot))
			{
				var input = Scraper.AnalyzeText(gray).Split(' ').ToList();
				input.RemoveAll(e => Regex.IsMatch(e.Trim(), @"[^0-9]") || string.IsNullOrWhiteSpace(e.Trim()));
				var mora = input.LastOrDefault();
				return mora;
			}
		}

		private static (List<Rectangle> rectangles, int cols, int rows) GetPageOfItems(InventorySection section, int page)
		{
			// Screenshot of inventory
			using (Bitmap screenshot = Navigation.CaptureWindow())
			{
				try
				{
					var (rectangles, cols, rows) = ProcessScreenshot(screenshot);
					if (cols != 8 || rows < 4)
					{
						// Generated rectangles
						screenshot.Save($"./logging/materials/{section}Inventory{page}.png");
						using (Graphics g = Graphics.FromImage(screenshot))
							rectangles.ForEach(r => g.DrawRectangle(new Pen(Color.Green, 3), r));
						screenshot.Save($"./logging/materials/{section}Inventory{page}_{cols}x{rows}.png");
					}
					return (rectangles, cols, rows);
				}
				catch (Exception)
				{
					screenshot.Save($"./logging/materials/{section}Inventory{page}.png");
					throw new Exception($"Insufficient items found in {section} inventory");
				}
			}
		}

		public static (List<Rectangle> rectangles, int cols, int rows) ProcessScreenshot(Bitmap screenshot)
		{
			var card = new RECT(
				Left: 0,
				Top: 0,
				Right: (int)(85 / 1280.0 * screenshot.Width),
				Bottom: (int)(100 / 720.0 * screenshot.Height));

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
					throw new Exception();
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

		public static string ScanMaterialName(InventorySection section, out Bitmap nameplate)
		{
			// Grab item name on right
			var refWidth = 1280.0;
			var refHeight = Navigation.GetAspectRatio() == new Size(16,9) ? 720.0 : 800.0;

			var width = Navigation.GetWidth();
			var height = Navigation.GetHeight();

			var reference = new Rectangle(872, 80, 327, 37);

			// Nameplate is in the same place in 16:9 and 16:10
			var region= new RECT(
				Left:   (int)( reference.Left   / refWidth  * width),
				Top:    (int)( reference.Top    / refHeight * height),
				Right:  (int)( reference.Right  / refWidth  * width),
				Bottom: (int)( reference.Bottom / refHeight * height));

			Bitmap bm = Navigation.CaptureRegion(region);
			nameplate = (Bitmap)bm.Clone();

			// Alter Image
			Scraper.SetGamma(0.2, 0.2, 0.2, ref bm);
			Bitmap n = Scraper.ConvertToGrayscale(bm);
			Scraper.SetInvert(ref n);

			string text = Scraper.AnalyzeText(n);
			text = Regex.Replace(text, @"[\W]", string.Empty).ToLower();

			//UI
			n.Dispose();
			bm.Dispose();

			if (section == InventorySection.CharacterDevelopmentItems)
				return Scraper.FindClosestDevelopmentName(text);

			if (section == InventorySection.Materials)
				return Scraper.FindClosestMaterialName(text);

			return null;
		}

		public static int ScanMaterialCount(Rectangle rectangle, out Bitmap quantity)
		{
			var region = new RECT(
				Left: rectangle.X,
				Top: (int)(rectangle.Y + (0.8 * rectangle.Height)), // Only get the bottom of inventory item
				Right: rectangle.Right,
				Bottom: rectangle.Bottom + 10);

			using (Bitmap bm = Navigation.CaptureRegion(region))
			{
				quantity = (Bitmap)bm.Clone();

				using (Bitmap rescaled = Scraper.ResizeImage(bm, (int)( bm.Width * 3 ), (int)( bm.Height * 3 )))
				{
					Bitmap copy = (Bitmap)rescaled.Clone();
					Scraper.SetGamma(0.7, 0.7, 0.7, ref copy);
					// Image Processing
					Bitmap n  =  Scraper.ConvertToGrayscale(copy);
					Scraper.SetContrast(65, ref n); // Setting a high contrast seems to be better than thresholding
													//Scraper.SetThreshold(165, ref n);

					string old_text = Scraper.AnalyzeText(n, Tesseract.PageSegMode.SingleWord).Trim().ToLower();

					// Might be worth it to train some more numbers
					var cleaned = old_text.Replace("mm", "111").Replace("m", "11").Replace("nn", "11").Replace("n", "1"); // Tesseract struggles with 1's so close together because of font
					cleaned = cleaned.Replace("a", "4");
					cleaned = cleaned.Replace("e", "1");
					//old_text = old_text.Replace("b", "8");
					//old_text = old_text.Replace("+", "4");

					cleaned = Regex.Replace(cleaned, @"[^0-9]", string.Empty);

					_ = int.TryParse(cleaned, out int count);

					Debug.WriteLine($"{old_text} -> {cleaned} -> {count}");

					//if (count > 3000 || count == 0)
					//{
					//	//Navigation.DisplayBitmap(n);
					//}
					copy.Dispose();
					n.Dispose();
					return count;
				}
			}
		}
	}
}