using Accord.Imaging;
using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

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

	internal class MaterialScraper : InventoryScraper
	{
		private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


		public MaterialScraper()
		{
			inventoryPage = InventoryPage.CharacterDevelopmentItems;
		}

		public MaterialScraper(InventoryPage section) : base()
		{
			inventoryPage = section;
		}

		internal void SetInventoryPage(InventoryPage page)
		{
			if (materialPages.Contains(page)) inventoryPage = page;
		}

		public void Scan_Materials(ref Inventory inventory)
		{
			if (!inventory.Materials.Contains(new Material("Mora", 0)))
			{
				inventory.Materials.Add(new Material("Mora", ScanMora()));
			}

			int scrollCount = 0;

			Material material = new Material(null, 0);
			Material previousMaterial = new Material(null, -1);

			List<Rectangle> rectangles;
			int page = 1;

			// Keep scanning while not repeating any items names
			while (true)
			{
				int rows, cols;
				// Find all items on the screen
				(rectangles, cols, rows) = GetPageOfItems(page, acceptLess: true);

				// Remove last row. Sometimes the bottom of a page of items is caught which results
				// in a faded quantity that can't be parsed. Removing slightly increases the number of pages that
				// need to be scrolled but it's fine.
				var r = rectangles.Take(rectangles.Count() - cols).ToList();

				Logger.Debug("Scanning material page {0}", page);
				Logger.Debug("Located {0} possible item locations on page.", rectangles.Count);

				foreach (var rectangle in r)
				{
					// Select Material
					Navigation.SetCursor(rectangle.Center().X, rectangle.Center().Y);
					Navigation.Click();
					Navigation.SystemWait(Navigation.Speed.SelectNextInventoryItem);

					material.name = ScanMaterialName(out Bitmap nameplate);
					material.count = 0;

					// Check if new material has been found
					if (inventory.Materials.Contains(material))
					{
						Logger.Debug("Repeat material found. Scrolling until end");
						goto LastPage;
					}
					else
					{
						if (!string.IsNullOrEmpty(material.name))
						{
							// Scan Material Quantity
							material.count = ScanMaterialCount(rectangle, out Bitmap quantity);
							if (material.count == 0)
							{
								UserInterface.AddError($"Failed to parse quantity for {material.name}");
								SaveInventoryBitmap(quantity, $"{material.name}_Quantity.png");
							}
							if (Properties.Settings.Default.LogScreenshots || material.count == 0)
                            {
								SaveInventoryBitmap(quantity, $"{material.name}_Quantity.png");
                            }
							inventory.Materials.Add(material);
							UserInterface.ResetCharacterDisplay();
							UserInterface.SetMaterial(nameplate, quantity, material.name, material.count);

							previousMaterial.name = material.name;
						}
					}
					nameplate.Dispose();
					Navigation.Wait(150);
				}

				Navigation.SetCursor(r.Last().Center().X, r.Last().Center().Y);
				Navigation.Click();
				Navigation.Wait(150);

				Logger.Debug("Finished page of materials. Scrolling...");

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
						Navigation.SystemWait(Navigation.Speed.InventoryScroll);
					}
				}
				Navigation.SystemWait(Navigation.Speed.Normal);
				++page;
			}

		LastPage:
			// scroll down as much as possible
			for (int i = 0; i < 20; i++)
			{
				Navigation.sim.Mouse.VerticalScroll(-1);
				Navigation.SystemWait(Navigation.Speed.InventoryScroll);
			}

			Navigation.Wait(500);

			(rectangles, _, _) = GetPageOfItems(page, acceptLess: true);
			bool passby = true;
			for (int i = rectangles.Count - 1; i >= 0; i--) // Click through but backwards to short-circuit after new materials
			{
				// Select Material
				Rectangle rectangle = rectangles[i];
				Navigation.SetCursor(rectangle.Center().X, rectangle.Center().Y);
				Navigation.Click();
				Navigation.SystemWait(Navigation.Speed.SelectNextInventoryItem);

				material.name = ScanMaterialName(out Bitmap nameplate);
				material.count = 0;

				if (inventory.Materials.Contains(material) && passby) continue;

				if (!inventory.Materials.Contains(material))
				{
					if (!string.IsNullOrEmpty(material.name))
					{
						// Scan Material Number
						material.count = ScanMaterialCount(rectangle, out Bitmap quantity);
						if (material.count == 0)
						{
							UserInterface.AddError($"Failed to parse quantity for {material.name}");
							SaveInventoryBitmap(quantity, $"{material.name}_Quantity.png");
						}
						else if (Properties.Settings.Default.LogScreenshots)
						{
							SaveInventoryBitmap(quantity, $"{material.name}_Quantity.png");
						}
						inventory.Materials.Add(material);
						UserInterface.ResetCharacterDisplay();
						UserInterface.SetMaterial(nameplate, quantity, material.name, material.count);
						passby = false; // New material found so break on next old material
						quantity.Dispose();
					}
				}
				else
				{
					Logger.Debug("Last material scanned, {0}.", inventory.Materials.Last().name);
					nameplate.Dispose();
					break;
				}
				Navigation.Wait(150);
			}
		}

		private int ScanMora()
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
                    if (Properties.Settings.Default.LogScreenshots) 
						SaveInventoryBitmap(screenshot, "mora.png");
				}
				else
				{
					UserInterface.SetNavigation_Image(screenshot);
					UserInterface.AddError("Unable to parse mora count");
					SaveInventoryBitmap(screenshot, "mora.png");
				}
				return count;
			}
		}

		public static string ParseMoraFromScreenshot(Bitmap screenshot)
		{
			using (var gray = GenshinProcesor.ConvertToGrayscale(screenshot))
			{
				var invert = (Bitmap)gray.Clone();
				GenshinProcesor.SetInvert(ref invert);
				var input = GenshinProcesor.AnalyzeText(invert).Split(' ').ToList();
				Logger.Debug("Scanned mora input: {0}", input.ToString());
				input.RemoveAll(e => Regex.IsMatch(e.Trim(), @"[^0-9]") || string.IsNullOrWhiteSpace(e.Trim()));
				var mora = input.LastOrDefault();
				Logger.Debug("Parsed mora input: {0}", mora);
				return mora;
			}
		}

		public string ScanMaterialName(out Bitmap nameplate)
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
			GenshinProcesor.SetGamma(0.2, 0.2, 0.2, ref bm);
			Bitmap n = GenshinProcesor.ConvertToGrayscale(bm);
			GenshinProcesor.SetInvert(ref n);

			string text = GenshinProcesor.AnalyzeText(n,Tesseract.PageSegMode.Auto);
			text = Regex.Replace(text, @"[\W\s]", string.Empty).ToLower();

			//UI
			n.Dispose();
			bm.Dispose();

			if (inventoryPage == InventoryPage.CharacterDevelopmentItems)
				return GenshinProcesor.FindClosestDevelopmentName(text);

			if (inventoryPage == InventoryPage.Materials)
				return GenshinProcesor.FindClosestMaterialName(text);

			return null;
		}

		public static int ScanMaterialCount(Rectangle rectangle, out Bitmap quantity)
		{
			Dictionary<int, int> counts = new Dictionary<int, int>();
			var region = new RECT(
				Left: rectangle.X,
				Top: (int)(rectangle.Y + (0.8 * rectangle.Height)), // Only get the bottom of inventory item
				Right: rectangle.Right,
				Bottom: rectangle.Bottom + 10);

            var rRange = new Accord.IntRange(0, 150);
            var bRange = new Accord.IntRange(0, 150);
            var gRange = new Accord.IntRange(0, 150);

            using (Bitmap bm = Navigation.CaptureRegion(region))
			{
				quantity = (Bitmap)bm.Clone();

				for (var scale = 1.0; scale <= 3; scale += 0.5)
                {
					using (Bitmap rescaled = GenshinProcesor.ResizeImage(bm, (int)(bm.Width * scale), (int)(bm.Height * scale)))
                    {
                        Bitmap copy = (Bitmap)rescaled.Clone();
                        GenshinProcesor.FilterColors(ref copy, rRange, bRange, gRange);

                        for (int i = 0; i < copy.Width; i++)
                            for (int j = 0; j < copy.Height * 0.25; j++)
                                copy.SetPixel(i, j, Color.White);

                        Bitmap n = GenshinProcesor.ConvertToGrayscale(copy);
                        
						GenshinProcesor.SetInvert(ref n);
                        n = new Threshold(50).Apply(n);

                        string original = GenshinProcesor.AnalyzeText(n).Trim();

						n.Dispose();
						copy.Dispose();

                        if (int.TryParse(original, out int val)) return val;

                        // Might be worth it to train some more numbers
                        var cleaned = original;

                        cleaned = cleaned.Replace("M", "111");

                        cleaned = Regex.Replace(cleaned, @"[^0-9]", string.Empty);

                        int.TryParse(cleaned, out val);

                        Logger.Debug($"Scanned: {original} -> Regex: {cleaned} -> Parsed: {val}");

                        if (counts.TryGetValue(val, out var counter))
                        {
                            if (counter >= 3 && val != 0) return val;
                            counts[val]++;
                        }
                        else
                        {
                            counts.Add(val, 1);
                        }
                    }
                }
			}
			
			var nullableMode = SafeExtractMaxCounter(counts);
			if (nullableMode == null)
				return 0;

			var mode = nullableMode.Value;
			if (mode.Key == 0 && counts.Count >= 5) 
				return 0;
			
			counts.Remove(mode.Key);
			return SafeExtractMaxCounter(counts)?.Value ?? 0;
		}

		private static KeyValuePair<int, int>? SafeExtractMaxCounter(Dictionary<int, int> counts)
		{
            return counts.Count == 0 ? null : (KeyValuePair<int, int>?)counts.Aggregate((l, r) => l.Value > r.Value ? l : r);
        }
    }
}