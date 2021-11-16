using System;
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

		private static HashSet<Artifact> equippedArtifacts = new HashSet<Artifact>();
		private static HashSet<Weapon> equippedWeapons = new HashSet<Weapon>();
		public static Queue<OCRImage> workerQueue = new Queue<OCRImage>();
		private static List<Thread> ImageProcessors = new List<Thread>();
		private static volatile bool b_threadCancel = false;
		private static int maxProcessors = 2; // TODO: Add support for more processors
		// TODO add language option

		public GenshinData()
		{
			characters = new List<Character>();
			inventory = new Inventory();
			equippedArtifacts = new HashSet<Artifact>();
			equippedWeapons = new HashSet<Weapon>();
		}

		public void StopImageProcessorWorkers()
		{
			b_threadCancel = true;
			AwaitProcessors();
			workerQueue = new Queue<OCRImage>();
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
			// Initize Image Processors
			for (int i = 0; i < maxProcessors; i++)
			{
				Thread processor = new Thread(ImageProcessorWorker){ IsBackground = true };
				processor.Start();
				ImageProcessors.Add(processor);
			}

			// Scan Main character Name
			string mainCharacterName = CharacterScraper.ScanMainCharacterName();
			Scraper.AssignTravelerName(mainCharacterName);
			Scraper.b_AssignedTravelerName = true;

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

			// Wait for Image Processors to finish
			AwaitProcessors();

			if (checkbox[2])
			{
				// Assign Artifacts to Characters
				if (checkbox[1])
					AssignArtifacts();
				if (checkbox[0])
					AssignWeapons();
			}
		}

		private void AwaitProcessors()
		{
			while (ImageProcessors.Count > 0)
			{
				ImageProcessors.RemoveAll(process => !process.IsAlive);
			}
		}

		public void ImageProcessorWorker()
		{
			Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
			var threadPriority = Thread.CurrentThread.Priority;
			Debug.WriteLine($"Thread #{Thread.CurrentThread.ManagedThreadId} priority: {threadPriority}");
			while (true)
			{
				if (b_threadCancel)
				{
					workerQueue.Clear();
					break;
				}

				if (workerQueue.TryDequeue(out OCRImage image))
				{
					if (image.type != "END" && image.bm != null)
					{
						if (image.type == "weapon")
						{
							if (!WeaponScraper.IsEnhancementOre(image.bm[0]))
							{
								// Scan as weapon
								UserInterface.ResetGearDisplay();
								Weapon w = WeaponScraper.CatalogueFromBitmaps(image.bm, image.id);
								if (w.IsValid())
								{
									UserInterface.IncrementWeaponCount();
									inventory.Add(w);
									if (!string.IsNullOrEmpty(w.equippedCharacter))
										equippedWeapons.Add(w);
								}
								else //if (!string.IsNullOrEmpty(w.GetName()))
								{
									UserInterface.AddError($"Unable to validate information for weapon #{image.id}");
									// Maybe save bitmaps in some directory to see what an issue might be
								}
							}
						}
						else if (image.type == "artifact")
						{
							// Scan as artifact
							UserInterface.ResetGearDisplay();

							Stopwatch stopwatch = new Stopwatch();
							stopwatch.Start();

							Artifact a= ArtifactScraper.CatalogueFromBitmapsAsync(image.bm, image.id).Result;

							stopwatch.Stop();
							TimeSpan ts = stopwatch.Elapsed;
							Debug.WriteLine($"Time to process artifact #{image.id}: {ts.Seconds}.{ts.Milliseconds / 10}");

							if (a.rarity >= 4)
							{
								if (a.IsValid())
								{
									UserInterface.IncrementArtifactCount();

									inventory.Add(a);

									if (!string.IsNullOrEmpty(a.equippedCharacter))
										equippedArtifacts.Add(a);
									Debug.WriteLine($"Inventory size: {inventory.size}");
								}
								else //if (!string.IsNullOrEmpty(a.setName))
								{
									UserInterface.AddError($"Unable to validate information for artifact #{image.id}");
									// Maybe save bitmaps in some directory to see what an issue might be

									continue;
								}
							}

							// Dispose of everything
							image.bm.ForEach(b => b.Dispose());
						}
						else // not supposed to happen
						{
							Form1.UnexpectedError("Unknown Image type for Image Processor");
						}
					}
					else
					{
						b_threadCancel = true;
					}
				}
				else
				{
					// Wait for more images to process
					Thread.Sleep(250);
				}
			}
			Debug.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} exit");
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