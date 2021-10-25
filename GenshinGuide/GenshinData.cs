using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;

namespace GenshinGuide
{
	public class GenshinData
	{
		[JsonProperty]
		private static List<Character> characters = new List<Character>();
		[JsonProperty]
		private static Inventory inventory = new Inventory();

		private static List<Artifact> equippedArtifacts = new List<Artifact>();
		private static List<Weapon> equippedWeapons = new List<Weapon>();
		public static Queue<OCRImage> workerQueue = new Queue<OCRImage>();
		private static volatile bool b_ScanWeapons = false;
		private static Thread ImageProcessor = new Thread(() => { ImageProcessorWorker(); });
		private static volatile bool b_threadCancel = false;
		// TODO add language option

		public GenshinData()
		{
			characters = new List<Character>();
			inventory = new Inventory();
			equippedArtifacts = new List<Artifact>();
			equippedWeapons = new List<Weapon>();
		}

		public void StopImageProcessorWorker()
		{
			if (ImageProcessor.IsAlive)
			{
				b_threadCancel = true;
				workerQueue = new Queue<OCRImage>();
			}
		}

		public List<Character> GetCharacters()
		{
			return characters;
		}

		public Inventory GetInventory()
		{
			return inventory;
		}

		public void GatherData(bool[] checkbox)
		{
			// Initize Image Processor Queue
			ImageProcessor = new Thread(() => { ImageProcessorWorker(); }) { IsBackground = true };
			ImageProcessor.Start();

			// Scan Main character Name
			string mainCharacterName = CharacterScraper.ScanMainCharacterName();
			if (Scraper.b_AssignedTravelerName == false && Scraper.GetCharacterCode(mainCharacterName) == 1)
			{
				Scraper.AssignTravelerName(mainCharacterName);
				Scraper.b_AssignedTravelerName = true;
			}

			if (checkbox[0])
			{
				// Get Weapons
				Navigation.InventoryScreen();
				Navigation.SelectWeaponInventory();
				WeaponScraper.ScanWeapons();
				//inventory.AssignWeapons(ref equippedWeapons);
				Navigation.MainMenuScreen();
			}

			if (checkbox[1])
			{
				// Get Artifacts
				Navigation.InventoryScreen();
				Navigation.SelectArtifactInventory();
				ArtifactScraper.ScanArtifacts();
				//inventory.AssignArtifacts(ref equippedArtifacts);
				Navigation.MainMenuScreen();
			}

			workerQueue.Enqueue(new OCRImage(null, "END", 0));

			if (checkbox[2])
			{
				// Get characters
				Navigation.CharacterScreen();
				characters = new List<Character>();
				characters = CharacterScraper.ScanCharacters();
				Navigation.MainMenuScreen();
			}

			// Wait for Image Processor to finish
			ImageProcessor.Join();

			if (checkbox[2])
			{
				// Assign Artifacts to Characters
				if (checkbox[1])
					AssignArtifacts();
				if (checkbox[0])
					AssignWeapons();
			}
		}

		public static void ImageProcessorWorker()
		{
			Weapon w; Artifact a;
			int weaponCount = 0;
			int artifactCount = 0;
			bool b_End = false;
			while (!b_End)
			{
				if (b_threadCancel)
				{
					workerQueue.Clear();
					b_End = true;
				}

				if (workerQueue.Count > 0)
				{
					OCRImage card = workerQueue.Dequeue();
					if (card.type != "END" && card.bm != null)
					{
						if (card.type == "weapon")
						{
							if (!WeaponScraper.IsEnhancementOre(card.bm[0]))
							{
								weaponCount++;
								// Scan as weapon
								UserInterface.ResetGearDisplay();
								w = WeaponScraper.CatalogueFromBitmaps(card.bm, card.id);

								if (w.IsValid())
								{
									Debug.WriteLine("Valid weapon scanned");
									UserInterface.IncrementWeaponCount();
									inventory.Add(w);
									if (w.equippedCharacter != 0)
										equippedWeapons.Add(w);
								}
								else
								{
									UserInterface.AddError($"Unable to validate information for weapon #{card.id}");
									// Maybe save bitmaps in some directory to see what an issue might be
								}
							}
						}
						else if (card.type == "artifact")
						{
							// Notify weapon has finished
							if (!b_ScanWeapons)
								b_ScanWeapons = true;

							artifactCount++;
							// Scan as weapon
							UserInterface.ResetGearDisplay();
							a = ArtifactScraper.ScanArtifact(card.bm, card.id);

							if (a.IsValid())
							{
								UserInterface.IncrementArtifactCount();
								inventory.Add(a);

								if (a.equippedCharacter >= 1)
								{
									equippedArtifacts.Add(a);
								}
							}
						}
						else // not supposed to happen
						{
							Form1.UnexpectedError("Unknown Image type for Image Processor");
						}
					}
					else
					{
						workerQueue.Clear();
						b_End = true;
					}
					card.bm = null; card.type = "";
				}
				else
				{ // Wait for more images to process
					System.Threading.Thread.Sleep(500);
				}
			}
			b_threadCancel = false;
			// Assign weapons and artifacts to inventory
			//inventory.AssignArtifacts(ref artifacts);
			//inventory.AssignWeapons(ref weapons);
		}

		public void AssignArtifacts()
		{
			foreach (Artifact a in equippedArtifacts)
			{
				foreach (Character c in characters)
				{
					if (a.GetEquippedCharacter() == c.GetName())
					{
						c.AssignArtifact(a);
					}
				}
			}
		}

		public void AssignWeapons()
		{
			foreach (Weapon w in equippedWeapons)
			{
				foreach (Character c in characters)
				{
					if (w.GetEquippedCharacter() == c.GetName())
					{
						c.AssignWeapon(w);
					}
				}
			}
		}



	}
}
