using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Tesseract;

namespace InventoryKamera
{
    public static class Scraper
	{
		private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		private const int numEngines = 8;

		private static readonly string tesseractDatapath = $"{Directory.GetCurrentDirectory()}\\tessdata";
		private static readonly string tesseractLanguage = "genshin_fast_09_04_21";

		public static Dictionary<string, string> Stats = new Dictionary<string, string>
		{
			["hp"] = "hp",
			["hp%"] = "hp_",
			["atk"] = "atk",
			["atk%"] = "atk_",
			["def"] = "def",
			["def%"] = "def_",
			["energyrecharge"] = "enerRech_",
			["elementalmastery"] = "eleMas",
			["healingbonus"] = "heal_",
			["critrate"] = "critRate_",
			["critdmg"] = "critDMG_",
			["physicaldmgbonus"] = "physical_dmg_",
		};

		public static readonly List<string> gearSlots = new List<string>
		{
			"flower",
			"plume",
			"sands",
			"goblet",
			"circlet",
		};

		private static readonly List<string> elements = new List<string>
		{
			"pyro",
			"hydro",
			"dendro",
			"electro",
			"anemo",
			"cryo",
			"geo",
		};

		public static readonly HashSet<string> enhancementMaterials = new HashSet<string>
		{
			"enhancementore",
			"fineenhancementore",
			"mysticenhancementore",
			"sanctifyingunction",
			"sanctifyingessence",
		};

		public static readonly List<string> customNames = new List<string>
		{
			"Traveler",
			"Wanderer"
		};

		public static ConcurrentBag<TesseractEngine> engines;

		public static Dictionary<string, string> Weapons, DevItems, Materials, Elements;

		public static Dictionary<string, JObject> Characters, Artifacts;

		static Scraper()
        {
            InitEngines();

			ReloadData();

			Elements = new Dictionary<string, string>();
			foreach (var element in elements)
			{
				Stats.Add($"{element.ToLower()}dmgbonus", $"{element.ToLower()}_dmg_");  // ["anemodmgbonus"] = "anemo_dmg_"
				Elements.Add(element, char.ToUpper(element[0]) + element.Substring(1));
			}

			Logger.Info("Scraper initialized");
        }

		internal static void ReloadData()
        {
			var listManager = new DatabaseManager();

			Characters = listManager.LoadCharacters();
			Artifacts = listManager.LoadArtifacts();
			Weapons = listManager.LoadWeapons();
			DevItems = listManager.LoadDevItems();
			Materials = listManager.LoadMaterials();

		}

		internal static void UpdateCharacterKey(string target, string name)
        {
			target = ConvertToGOOD(target).ToLower();
			name = ConvertToGOOD(name).ToLower();

			if (target == name) return;

			if (Characters.TryGetValue(target, out JObject value))
			{
				Characters.Add(name, value);
				Characters.Remove(target);
				Logger.Info("Internally set {0} custom name to {1}", target, name);
			}
			else if (Characters.ContainsKey(name))
			{
				Logger.Info("{0} already exists internally");
			}
			else throw new KeyNotFoundException($"Could not find '{target}' entry in characters.json");
		}

		public static void AssignTravelerName(string name)
		{
			name = string.IsNullOrWhiteSpace(name) ? CharacterScraper.ScanMainCharacterName() : name.ToLower();
			if (!string.IsNullOrWhiteSpace(name))
			{
				UpdateCharacterKey("traveler", name);
				UserInterface.SetMainCharacterName(name);
			}
			else
			{
				UserInterface.AddError("Could not parse Traveler's username");
			}
		}

		#region OCR

		private static void InitEngines()
		{
			engines = new ConcurrentBag<TesseractEngine>();
			try
			{
				for (int i = 0; i < numEngines; i++)
				{
					engines.Add(new TesseractEngine(tesseractDatapath, tesseractLanguage, EngineMode.LstmOnly));
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "Failed to initialize Tesseract engines.");
				throw;
			}
		}

		public static void RestartEngines()
		{
			
			if (engines is null) engines = new ConcurrentBag<TesseractEngine>();
			lock (engines)
			{
				while (!engines.IsEmpty)
				{
					if (engines.TryTake(out TesseractEngine e))
						e.Dispose();
				}

				for (int i = 0; i < numEngines; i++)
				{
					engines.Add(new TesseractEngine(tesseractDatapath, tesseractLanguage, EngineMode.LstmOnly));
				}
			}
			Logger.Debug("{numEngines} Engines restarted", numEngines);
		}

		/// <summary> Use Tesseract OCR to find words on picture to string </summary>
		public static string AnalyzeText(Bitmap bitmap, PageSegMode pageMode = PageSegMode.SingleLine, bool numbersOnly = false)
		{
			string text = "";
			TesseractEngine e;
			while (!engines.TryTake(out e)) { Thread.Sleep(10); }

			if (numbersOnly) e.SetVariable("tessedit_char_whitelist", "0123456789");
			using (var page = e.Process(bitmap, pageMode))
			{
				using (var iter = page.GetIterator())
				{
					iter.Begin();
					do
					{
						text += iter.GetText(PageIteratorLevel.TextLine);
					}
					while (iter.Next(PageIteratorLevel.TextLine));
				}
			}
			engines.Add(e);

			return text;
		}

		#endregion OCR

		#region Check valid parameters

		public static bool IsValidSetName(string setName)
		{
			if (Artifacts.TryGetValue(setName, out var _) || Artifacts.TryGetValue(setName.ToLower(), out var _)) return true;
			foreach (var artifactSet in Artifacts.Values)
				foreach (var field in artifactSet)
					if (field.ToString() == setName) return true;
                
			return false;
		}

		public static bool IsValidArtifactName(string artifactName)
        {
			return false;
        }

		internal static bool IsValidMaterial(string name)
		{
			return Materials.ContainsValue(name) || Materials.ContainsKey(name.ToLower());
		}

		public static bool IsValidStat(string stat)
		{
			return string.IsNullOrWhiteSpace(stat) || Stats.ContainsValue(stat);
		}

		public static bool IsValidSlot(string gearSlot)
		{
			return gearSlots.Contains(gearSlot);
		}

		public static bool IsValidCharacter(string character)
		{
			return character.Contains("Traveler") || character == "Wanderer" || Characters.ContainsKey(character.ToLower());
		}

		public static bool IsValidElement(string element)
		{
			return Elements.ContainsValue(element) || Elements.ContainsKey(element.ToLower());
		}

		public static bool IsEnhancementMaterial(string material)
		{
			return enhancementMaterials.Contains(material.ToLower()) || Materials.ContainsValue(material) || Materials.ContainsKey(material.ToLower());
		}

		public static bool IsValidWeapon(string weapon)
		{
			return Weapons.ContainsValue(weapon) || Weapons.ContainsKey(weapon.ToLower());
		}

		#endregion Check valid parameters

		#region Element Searching

		public static string FindClosestGearSlot(string input)
		{
			foreach (var slot in gearSlots)
			{
				if (input.Contains(slot))
				{
					return slot;
				}
			}
			return input;
		}

		public static string FindClosestStat(string stat, int maxEdits = 5)
		{
			return FindClosestInDict(source: stat, targets: Stats, maxEdits: maxEdits);
		}

		public static string FindElementByName(string name, int maxEdits = 5)
		{
			return FindClosestInDict(source: name, targets: Elements, maxEdits: maxEdits);
		}

		public static string FindClosestWeapon(string name, int maxEdits = 5)
		{
			return FindClosestInDict(source: name, targets: Weapons, maxEdits: maxEdits);
		}

		public static string FindClosestSetName(string name, int maxEdits = 5)
		{
			return FindClosestInDict(source: name, targets: Artifacts, maxEdits: maxEdits);
		}
		
		public static string FindClosestArtifactSetFromArtifactName(string name, int maxEdits = 5)
		{
			if (string.IsNullOrWhiteSpace(name)) return "";
			string closestMatch = null;
			int edits = int.MaxValue;


            foreach (var artifactSet in Artifacts)
            {
                foreach (var slot in artifactSet.Value["artifacts"].Values())
                {
                    string normalizedName = slot["normalizedName"].ToString();
                    if (name == normalizedName)
						return artifactSet.Value["GOOD"].ToString();

					int dist = CalcDistance(name, slot["normalizedName"].ToString(), maxEdits);
					if (dist < maxEdits && dist < edits)
					{
						edits = dist;
						closestMatch = artifactSet.Value["GOOD"].ToString();
					}
				}
			}
			return closestMatch;
		}

		public static string FindClosestCharacterName(string name, int maxEdits = 10)
		{
			return FindClosestInDict(source: name, targets: Characters, maxEdits: maxEdits);
		}

		public static string FindClosestDevelopmentName(string name, int maxEdits = 15)
		{
			string value = FindClosestInDict(source: name, targets: DevItems, maxEdits: maxEdits);
			return !string.IsNullOrWhiteSpace(value) ? value : FindClosestInDict(source: name, targets: Materials, maxEdits: maxEdits);
		}

		public static string FindClosestMaterialName(string name, int maxEdits = 15)
		{
			string value = FindClosestInDict(source: name, targets: Materials, maxEdits: maxEdits);
			return !string.IsNullOrWhiteSpace(value) ? value : FindClosestInDict(source: name, targets: Materials, maxEdits: maxEdits);
		}

		private static string FindClosestInDict(string source, Dictionary<string, string> targets, int maxEdits = 5)
		{
			if (string.IsNullOrWhiteSpace(source)) return "";
			if (targets.TryGetValue(source, out string value)) return value;

			HashSet<string> keys = new HashSet<string>(targets.Keys);

			if (keys.Where(key => key.Contains(source)).Count() == 1) return targets[keys.First(key => key.Contains(source))];

			source = FindClosestInList(source, keys, maxEdits);

			return targets.TryGetValue(source, out value) ? value : source;
		}

		private static string FindClosestInDict(string source, Dictionary<string, JObject> targets, int maxEdits = 5)
		{
			if (string.IsNullOrWhiteSpace(source)) return "";
			if (targets.TryGetValue(source, out JObject value)) return (string)value["GOOD"];

			HashSet<string> keys = new HashSet<string>(targets.Keys);

			if (keys.Where(key => key.Contains(source)).Count() == 1) return (string)targets[keys.First(key => key.Contains(source))]["GOOD"];

			source = FindClosestInList(source, keys, maxEdits);

			return targets.TryGetValue(source, out value) ? (string)value["GOOD"] : source;
		}

		private static string FindClosestInList(string source, HashSet<string> targets, int maxEdits)
		{
			if (targets.Contains(source)) return source;
			if (string.IsNullOrWhiteSpace(source)) return null;

			string value = "";

			foreach (var target in targets)
			{
				int edits = CalcDistance(source, target, maxEdits);

				if (edits < maxEdits)
				{
					value = target;
					maxEdits = edits;
				}
			}
			return value;
		}

		// Adapted from https://stackoverflow.com/a/9454016/13205651
		private static int CalcDistance(string text, string setName, int maxEdits)
		{
			int length1 = text.Length;
			int length2 = setName.Length;

			// Return trivial case - difference in string lengths exceeds threshhold
			if (Math.Abs(length1 - length2) > maxEdits) { return int.MaxValue; }

			// Ensure arrays [i] / length1 use shorter length
			if (length1 > length2)
			{
				Swap(ref setName, ref text);
				Swap(ref length1, ref length2);
			}

			int maxi = length1;
			int maxj = length2;

			int[] dCurrent = new int[maxi + 1];
			int[] dMinus1 = new int[maxi + 1];
			int[] dMinus2 = new int[maxi + 1];
			int[] dSwap;

			for (int i = 0; i <= maxi; i++) { dCurrent[i] = i; }

			int jm1 = 0, im1 = 0, im2 = -1;

			for (int j = 1; j <= maxj; j++)
			{
				// Rotate
				dSwap = dMinus2;
				dMinus2 = dMinus1;
				dMinus1 = dCurrent;
				dCurrent = dSwap;

				// Initialize
				int minDistance = int.MaxValue;
				dCurrent[0] = j;
				im1 = 0;
				im2 = -1;

				for (int i = 1; i <= maxi; i++)
				{
					int cost = text[im1] == setName[jm1] ? 0 : 1;

					int del = dCurrent[im1] + 1;
					int ins = dMinus1[i] + 1;
					int sub = dMinus1[im1] + cost;

					//Fastest execution for min value of 3 integers
					int min = (del > ins) ? (ins > sub ? sub : ins) : (del > sub ? sub : del);

					if (i > 1 && j > 1 && text[im2] == setName[jm1] && text[im1] == setName[j - 2])
						min = Math.Min(min, dMinus2[im2] + cost);

					dCurrent[i] = min;
					if (min < minDistance) { minDistance = min; }
					im1++;
					im2++;
				}
				jm1++;
				if (minDistance > maxEdits) { return int.MaxValue; }
			}

			int result = dCurrent[maxi];
			return ( result > maxEdits ) ? int.MaxValue : result;

			void Swap<T>(ref T arg1, ref T arg2)
			{
				T temp = arg1;
				arg1 = arg2;
				arg2 = temp;
			}
		}

		#endregion Element Searching


		public static void FindDelay(List<Rectangle> rectangles)
		{
			Navigation.SetDelay(180);
			int delayOffset = 20;
			bool bStoppedOnce = false; bool bStop = false;
			Bitmap card1; Bitmap card2; Bitmap card3;
			Rectangle item1 = rectangles[0];

			RECT reference; int width = Navigation.GetWidth(); int height = Navigation.GetHeight();

			#region Get first card

			if (Navigation.GetAspectRatio() == new Size(16, 9))
			{
				reference = new RECT(new Rectangle(862, 80, 327, 560));

				int left = (int)Math.Round(reference.Left / 1280.0 * width, MidpointRounding.AwayFromZero);
				int top = (int)Math.Round(reference.Top / 720.0 * height, MidpointRounding.AwayFromZero);
				int right = (int)Math.Round(reference.Right / 1280.0 * width, MidpointRounding.AwayFromZero);
				int bottom = (int)Math.Round(reference.Bottom / 720.0 * height, MidpointRounding.AwayFromZero);

				card1 = Navigation.CaptureRegion(new RECT(left, top, right, bottom));
			}
			else // if (Navigation.GetAspectRatio() == new Size(8, 5))
			{
				reference = new RECT(new Rectangle(862, 80, 327, 640));

				int left = (int)Math.Round(reference.Left / 1280.0 * width, MidpointRounding.AwayFromZero);
				int top = (int)Math.Round(reference.Top / 800.0 * height, MidpointRounding.AwayFromZero);
				int right = (int)Math.Round(reference.Right / 1280.0 * width, MidpointRounding.AwayFromZero);
				int bottom = (int)Math.Round(reference.Bottom / 800.0 * height, MidpointRounding.AwayFromZero);

				card1 = Navigation.CaptureRegion(new RECT(left, top, right, bottom));
			}

			#endregion Get first card

			do
			{
				if (bStoppedOnce)
					delayOffset = 10;

				// Do mouse movement to first and second UI element in Inventory
				Navigation.SetCursor(item1.Center().X, item1.Center().Y);
				Navigation.Click();
				Navigation.Wait(((int)Navigation.GetDelay()) - delayOffset);

				Rectangle item2 = rectangles[1];
				Navigation.SetCursor(item2.Center().X, item2.Center().Y);
				Navigation.Click();
				Navigation.Wait(((int)Navigation.GetDelay()) - delayOffset);

				// Take image after second click
				if (Navigation.GetAspectRatio() == new Size(16, 9))
				{
					int left = (int)Math.Round(reference.Left / 1280.0 * width, MidpointRounding.AwayFromZero);
					int top = (int)Math.Round(reference.Top / 720.0 * height, MidpointRounding.AwayFromZero);
					int right = (int)Math.Round(reference.Right / 1280.0 * width, MidpointRounding.AwayFromZero);
					int bottom = (int)Math.Round(reference.Bottom / 720.0 * height, MidpointRounding.AwayFromZero);

					card2 = Navigation.CaptureRegion(new RECT(left, top, right, bottom));
				}
				else
				{
					int left = (int)Math.Round(reference.Left / 1280.0 * width, MidpointRounding.AwayFromZero);
					int top = (int)Math.Round(reference.Top / 800.0 * height, MidpointRounding.AwayFromZero);
					int right = (int)Math.Round(reference.Right / 1280.0 * width, MidpointRounding.AwayFromZero);
					int bottom = (int)Math.Round(reference.Bottom / 800.0 * height, MidpointRounding.AwayFromZero);

					card2 = Navigation.CaptureRegion(new RECT(left, top, right, bottom));
				}

				Rectangle item3 = rectangles[2];
				Navigation.SetCursor(item3.Center().X, item3.Center().Y);
				Navigation.Click();
				Navigation.Wait(((int)Navigation.GetDelay()) - delayOffset);

				// Take image after third click
				if (Navigation.GetAspectRatio() == new Size(16, 9))
				{
					int left = (int)Math.Round(reference.Left / 1280.0 * width, MidpointRounding.AwayFromZero);
					int top = (int)Math.Round(reference.Top / 720.0 * height, MidpointRounding.AwayFromZero);
					int right = (int)Math.Round(reference.Right / 1280.0 * width, MidpointRounding.AwayFromZero);
					int bottom = (int)Math.Round(reference.Bottom / 720.0 * height, MidpointRounding.AwayFromZero);

					card3 = Navigation.CaptureRegion(new RECT(left, top, right, bottom));
				}
				else
				{
					int left = (int)Math.Round(reference.Left / 1280.0 * width, MidpointRounding.AwayFromZero);
					int top = (int)Math.Round(reference.Top / 800.0 * height, MidpointRounding.AwayFromZero);
					int right = (int)Math.Round(reference.Right / 1280.0 * width, MidpointRounding.AwayFromZero);
					int bottom = (int)Math.Round(reference.Bottom / 800.0 * height, MidpointRounding.AwayFromZero);

					card3 = Navigation.CaptureRegion(new RECT(left, top, right, bottom));
				}

				// logic for continuing to run
				if (!CompareBitmapsFast(card1, card2) && !CompareBitmapsFast(card2, card3))
				{
					Navigation.SetDelay(Navigation.GetDelay() - delayOffset);
					//Navigation.SystemRandomWait();

					Navigation.SetCursor(item1.Center().X, item1.Center().Y);
					Navigation.Click();
					//Navigation.SystemRandomWait();
				}
				else
				{
					if (bStoppedOnce)
					{
						bStop = true;
					}
					bStoppedOnce = true;
				}
			} while (!bStoppedOnce && ( Navigation.GetDelay() - delayOffset > 0 ));

			// delay of compare function
			Navigation.SetDelay(Navigation.GetDelay() + 7);
			Debug.WriteLine($"Delay found:  {Navigation.GetDelay()}");
			card1.Dispose(); card2.Dispose();

			// set back to first element
			Navigation.SystemWait(Navigation.Speed.Slowest);
			Navigation.SetCursor(item1.Center().X, item1.Center().Y);
			Navigation.Click();
			Navigation.SystemWait(Navigation.Speed.Slower);
		}

        #region Image Operations

        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}

		public static Bitmap ScaleImage(System.Drawing.Image image, double factor)
		{
			return ResizeImage(image, (int)( image.Width * factor ), (int)( image.Height * factor ));
		}

		public static bool CompareColors(Color a, Color b)
		{
			int[] diff = new int[3];
			diff[0] = Math.Abs(a.R - b.R);
			diff[1] = Math.Abs(a.G - b.G);
			diff[2] = Math.Abs(a.B - b.B);

			return diff[0] < 10 && diff[1] < 10 && diff[2] < 10;
		}

		internal static Color ClosestColor(List<Color> colors, ImageStatistics color)
		{
			var c2 = Color.FromArgb((int)color.Red.Mean, (int)color.Green.Mean, (int)color.Blue.Mean);
			var diff = colors.Select(x => new { Value = x, Diff = GetColorDifference(x, c2) }).ToList();

			foreach (var c in colors)
            {
                if (CompareColors(c, c2)) return c;
            }

            return diff.Find(x=> x.Diff == diff.Min(y=>y.Diff)).Value;
		}

        private static int GetColorDifference(Color c, Color c2)
        {
			int r = c.R - c2.R, g = c.G - c2.G, b = c.B - c2.B;
			return r*r + g*g + b*b;
		}

        public static Bitmap ConvertToGrayscale(Bitmap bitmap)
		{
			return new Grayscale(0.2125, 0.7154, 0.0721).Apply(bitmap);
		}

		public static void SetContrast(double contrast, ref Bitmap bitmap)
		{
			new ContrastCorrection((int)contrast).ApplyInPlace(bitmap);
		}

		public static void SetGamma(double red, double green, double blue, ref Bitmap bitmap)
		{
			Bitmap temp = bitmap;
			Bitmap bmap = (Bitmap)temp.Clone();
			Color c;
			byte[] redGamma = CreateGammaArray(red);
			byte[] greenGamma = CreateGammaArray(green);
			byte[] blueGamma = CreateGammaArray(blue);
			for (int i = 0; i < bmap.Width; i++)
			{
				for (int j = 0; j < bmap.Height; j++)
				{
					c = bmap.GetPixel(i, j);
					bmap.SetPixel(i, j, Color.FromArgb(redGamma[c.R],
					   greenGamma[c.G], blueGamma[c.B]));
				}
			}
			bitmap = (Bitmap)bmap.Clone();
		}

		private static byte[] CreateGammaArray(double color)
		{
			byte[] gammaArray = new byte[256];
			for (int i = 0; i < 256; ++i)
			{
				gammaArray[i] = (byte)Math.Min(255,
		(int)( ( 255.0 * Math.Pow(i / 255.0, 1.0 / color) ) + 0.5 ));
			}
			return gammaArray;
		}

		public static void SetInvert(ref Bitmap bitmap)
		{
			new Invert().ApplyInPlace(bitmap);
		}

		public static void SetColor(string colorFilterType, ref Bitmap bitmap)
		{
			Bitmap temp = bitmap;
			Bitmap bmap = (Bitmap)temp.Clone();
			Color c;
			for (int i = 0; i < bmap.Width; i++)
			{
				for (int j = 0; j < bmap.Height; j++)
				{
					c = bmap.GetPixel(i, j);
					int nPixelR = 0;
					int nPixelG = 0;
					int nPixelB = 0;
					if (colorFilterType == "red")
					{
						nPixelR = c.R;
						nPixelG = c.G - 255;
						nPixelB = c.B - 255;
					}
					else if (colorFilterType == "green")
					{
						nPixelR = c.R - 255;
						nPixelG = c.G;
						nPixelB = c.B - 255;
					}
					else if (colorFilterType == "blue")
					{
						nPixelR = c.R - 255;
						nPixelG = c.G - 255;
						nPixelB = c.B;
					}
					nPixelR = Math.Max(nPixelR, 0);
					nPixelR = Math.Min(255, nPixelR);

					nPixelG = Math.Max(nPixelG, 0);
					nPixelG = Math.Min(255, nPixelG);

					nPixelB = Math.Max(nPixelB, 0);
					nPixelB = Math.Min(255, nPixelB);

					bmap.SetPixel(i, j, Color.FromArgb((byte)nPixelR,
					  (byte)nPixelG, (byte)nPixelB));
				}
			}
			bitmap = (Bitmap)bmap.Clone();
		}

		public static void SetBrightness(int brightness, ref Bitmap bitmap)
		{
			if (brightness < -255) brightness = -255;
			if (brightness > 255) brightness = 255;

			Bitmap temp = bitmap;
			Bitmap bmap = (Bitmap)temp.Clone();
			Color c;
			for (int i = 0; i < bmap.Width; i++)
			{
				for (int j = 0; j < bmap.Height; j++)
				{
					c = bmap.GetPixel(i, j);
					int cR = c.R + brightness;
					int cG = c.G + brightness;
					int cB = c.B + brightness;

					if (cR < 0) cR = 1;
					if (cR > 255) cR = 255;

					if (cG < 0) cG = 1;
					if (cG > 255) cG = 255;

					if (cB < 0) cB = 1;
					if (cB > 255) cB = 255;

					bmap.SetPixel(i, j, Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
				}
			}
			bitmap = (Bitmap)bmap.Clone();
		}

		public static void SetThreshold(int threshold, ref Bitmap bitmap)
		{
			new Threshold(threshold).ApplyInPlace(bitmap);
		}

		public static void FilterColors(ref Bitmap bm, IntRange red, IntRange green, IntRange blue)
		{
			ColorFiltering colorFilter = new ColorFiltering
			{
				Red = red,
				Green = green,
				Blue = blue,
				FillColor = new RGB(255,255,255)
			};
			colorFilter.ApplyInPlace(bm);
		}

		public static bool CompareBitmapsFast(Bitmap bmp1, Bitmap bmp2)
		{
			if (bmp1 == null || bmp2 == null)
				return false;
			if (object.Equals(bmp1, bmp2))
				return true;
			if (!bmp1.Size.Equals(bmp2.Size) || !bmp1.PixelFormat.Equals(bmp2.PixelFormat))
				return false;

			int bytes = bmp1.Width * bmp1.Height * (System.Drawing.Image.GetPixelFormatSize(bmp1.PixelFormat) / 8);

			bool result = true;
			byte[] b1bytes = new byte[bytes];
			byte[] b2bytes = new byte[bytes];

			BitmapData bitmapData1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, bmp1.PixelFormat);
			BitmapData bitmapData2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, bmp2.PixelFormat);

			Marshal.Copy(bitmapData1.Scan0, b1bytes, 0, bytes);
			Marshal.Copy(bitmapData2.Scan0, b2bytes, 0, bytes);

			for (int n = 0; n <= bytes - 1; n++)
			{
				if (b1bytes[n] != b2bytes[n])
				{
					result = false;
					break;
				}
			}

			bmp1.UnlockBits(bitmapData1);
			bmp2.UnlockBits(bitmapData2);

			return result;
		}

		#endregion Image Operations

		private static string ConvertToGOOD(string text)
		{
			text = text.ToLower();
			var pascal = CultureInfo.GetCultureInfo("en-US").TextInfo.ToTitleCase(text);
			return Regex.Replace(pascal, @"[\W]", string.Empty);
		}
	}
}