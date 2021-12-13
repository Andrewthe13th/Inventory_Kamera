using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Accord.Imaging.Filters;
using Accord.Imaging;
using System.Diagnostics;

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
		Food,
		Materials,
		Furnishings,
	}

	public static class MaterialScraper
	{
		public static HashSet<Material> Scan_Materials(InventorySection section)
		{
			int scrollCount = 0;

			HashSet<Material> materials = new HashSet<Material>();
			Material material = new Material(null, 0);
			Material previousMaterial = new Material(null, -1);

			List<Rectangle> rectangles;

			// Keep scanning while not repeating any items names
			while (true)
			{
				int rows, cols;
				// Find all items on the screen
				(rectangles, cols, rows) = GetPageOfItems(); 

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
			}

			LastPage:
			// scroll down as much as possible
			for (int i = 0; i < 20; i++)
			{
				Navigation.sim.Mouse.VerticalScroll(-1);
				Navigation.SystemRandomWait(Navigation.Speed.InventoryScroll);
			}

			Navigation.Wait(500);

			(rectangles, _, _) = GetPageOfItems();
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
						materials.Add(material);
						UserInterface.ResetCharacterDisplay();
						UserInterface.SetMaterial(nameplate, quantity, material.name, material.count);
						passby = false; // New material found so break on next old material
					}
				}
				else break;
				Navigation.Wait(150);
			}

			return materials;

			{
				// Scan the last of the material items and stop when repeated again or until max of 28
				//int startPostion = 1;
				//currentColumn = 0;
				//currentRow = 0;
				//previousMaterial.name = null;
				//previousRowMaterial.name = null;
				//for (int i = startPostion; i < 5; i++)
				//{
				//	for (int k = 0; k < maxColumns; k++)
				//	{
				//		// Select Material
				//		Navigation.SetCursorPos(Navigation.GetPosition().Left + Convert.ToInt32(materialLocation_X) + ( xOffset * ( k % maxColumns ) ), Navigation.GetPosition().Top + Convert.ToInt32(materialLocation_Y) + ( yOffset * ( i % ( maxRows + 1 ) ) ));
				//		Navigation.SystemRandomWait(Navigation.Speed.SelectNextInventoryItem);
				//		Navigation.Click();
				//		Navigation.SystemRandomWait(Navigation.Speed.SelectNextInventoryItem);

				//		// Scan Material Name
				//		material.name = ScanMaterialName(section, out Bitmap nameplate);
				//		material.count = 0;

				//		// Check if new material has been found
				//		if (material.name == previousMaterial.name || string.IsNullOrWhiteSpace(material.name))
				//		{
				//			break;
				//		}
				//		else if (Scraper.IsValidMaterial(material.name))
				//		{
				//			// Scan material number
				//			material.count = ScanMaterialCountEnd(k, i - 1, out Bitmap quantity);
				//			if (material.count > 0)
				//			{
				//				materials.Add(material);
				//				previousMaterial.name = material.name;
				//				UserInterface.ResetCharacterDisplay();
				//				UserInterface.SetMaterial(nameplate, quantity, material.name, material.count);
				//			}
				//			else
				//			{
				//				UserInterface.AddError($"Unable to determine quantity for {material.name}");
				//			}
				//		}
				//	}
				//}

				//return materials;
			}
		}

		private static (List<Rectangle> rectangles, int cols, int rows) GetPageOfItems()
		{
			var card = new RECT(
				Left: 0,
				Top: 0,
				Right: (int)(83 / 1280.0 * Navigation.GetWidth()),
				Bottom: (int)(100 / 720.0 * Navigation.GetHeight()));

			// Filter for relative size of items in inventory, give or take a few pixels
			BlobCounter blobCounter = new BlobCounter
			{
				FilterBlobs = true,
				MinHeight = card.Height - (Navigation.GetAspectRatio() == new Size(16, 9) ? 0 : 10),
				MaxHeight = card.Height + 15,
				MinWidth  = card.Width - 15,
				MaxWidth  = card.Width + 15,
			};

			// Screenshot of inventory
			Bitmap screenshot = Navigation.CaptureWindow();

			// Copy used to overlay onto in testing
			Bitmap output = new Bitmap(screenshot);

			// Image pre-processing
			ContrastCorrection contrast = new ContrastCorrection(90);
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
				blobCounter.Dispose();
				throw new Exception("No items detected in inventory");
			}

			// Don't save overlapping blobs
			List<Rectangle> rectangles = new List<Rectangle>();
			List<Rectangle> blobRects = blobCounter.GetObjectsRectangles().ToList();
			blobCounter.Dispose();

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
			
			screenshot.Dispose();
			output.Dispose();

			return (rectangles, colCoords.Count, rowCoords.Count);
		}

		public static string ScanMaterialName(InventorySection section, out Bitmap nameplate)
		{
			// Grab item name on right
			var refWidth = 1280.0;
			var refHeight = Navigation.GetAspectRatio() == new Size(16,9) ? 720.0 : 800.0;

			var width = Navigation.GetWidth();
			var height = Navigation.GetHeight();

			var reference = new Rectangle(862, 80, 327, 37);

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
				Top: (int)(rectangle.Y + (rectangle.Height * 0.8)), // Only get the bottom of inventory item
				Right: rectangle.Right,
				Bottom: rectangle.Bottom + 5);

			using (Bitmap bm = Navigation.CaptureRegion(region))
			{
				quantity = (Bitmap)bm.Clone();

				using (Bitmap rescaled = Scraper.ResizeImage(bm, (int)( bm.Width * 3), (int)( bm.Height * 3 )))
				{

					// Image Processing
					Bitmap n  =  Scraper.ConvertToGrayscale(rescaled);
					Scraper.SetContrast(65, ref n); // Setting a high contrast seems to be better than thresholding
					//Scraper.SetThreshold(165, ref n);

					string old_text = Scraper.AnalyzeText(n, Tesseract.PageSegMode.SingleWord).Trim().ToLower();
					
					// Might be worth it to train some more numbers
					var cleaned = old_text.Replace("mm", "111").Replace("m", "11").Replace("nn", "11").Replace("n", "1"); // Tesseract struggles with 1's so close together because of font
					cleaned = cleaned.Replace("a", "4");
					cleaned = cleaned.Replace("e", "1");
					//old_text = old_text.Replace("b", "8");
					//old_text = old_text.Replace("+", "4");

					string text = Regex.Replace(cleaned, @"[^\d]", string.Empty);

					_ = int.TryParse(text, out int count);

					//Debug.WriteLine($"{old_text} -> {cleaned} -> {count}");

					//if (count > 3000 || count == 0)
					//{
					//	//Navigation.DisplayBitmap(n);
					//}

					n.Dispose();
					return count;

				}
			}
		}
	}
}