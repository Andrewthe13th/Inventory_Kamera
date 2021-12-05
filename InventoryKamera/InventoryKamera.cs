using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;

namespace InventoryKamera
{
	public class InventoryKamera
	{
		[JsonProperty]
		private static List<Character> characters = new List<Character>();

		[JsonProperty]
		private static Inventory inventory = new Inventory();

		private static List<Artifact> equippedArtifacts = new List<Artifact>();
		private static List<Weapon> equippedWeapons = new List<Weapon>();
		public static Queue<OCRImage> workerQueue = new Queue<OCRImage>();
		private static List<Thread> ImageProcessors = new List<Thread>();
		private static volatile bool b_threadCancel = false;
		private static int maxProcessors = 2; // TODO: Add support for more processors
											  // TODO add language option

		public InventoryKamera()
		{
			characters = new List<Character>();
			inventory = new Inventory();
			equippedArtifacts = new List<Artifact>();
			equippedWeapons = new List<Weapon>();
			ImageProcessors = new List<Thread>();
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

		public void GatherData(bool[] formats, bool[] checkbox)
		{
			// Initize Image Processors
			for (int i = 0; i < maxProcessors; i++)
			{
				Thread processor = new Thread(ImageProcessorWorker){ IsBackground = true };
				processor.Start();
				ImageProcessors.Add(processor);
			}
			Debug.WriteLine($"Added {ImageProcessors.Count} workers");

			Scraper.RestartEngines();

			// Scan Main character Name
			string mainCharacterName = CharacterScraper.ScanMainCharacterName();
			Scraper.AssignTravelerName(mainCharacterName);
			Scraper.b_AssignedTravelerName = true;

			if (formats[0])
			{
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

			if (formats[1])
			{
				// Scan Character Development Items
				if (checkbox[3])
				{
					// Get Materials
					Navigation.InventoryScreen();
					Navigation.SelectCharacterDevelopmentInventory();
					List<Material> materials = MaterialScraper.Scan_Materials(InventorySection.CharacterDevelopmentItems);
					inventory.SetCharacterDevelopmentItems(ref materials);
					Navigation.MainMenuScreen();
				}

				// Scan Materials
				if (checkbox[4])
				{
					// Get Materials
					Navigation.InventoryScreen();
					Navigation.SelectMaterialInventory();
					List<Material> materials = MaterialScraper.Scan_Materials(InventorySection.Materials);
					inventory.SetMaterials(ref materials);
					Navigation.MainMenuScreen();
				}
			}
		}

		private void AwaitProcessors()
		{
			while (ImageProcessors.Count > 0)
			{
				ImageProcessors.RemoveAll(process => !process.IsAlive);
			}
			b_threadCancel = false;
		}

		public void ImageProcessorWorker()
		{
			Debug.WriteLine($"Thread #{Thread.CurrentThread.ManagedThreadId} priority: {Thread.CurrentThread.Priority}");
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
								Weapon w = WeaponScraper.CatalogueFromBitmaps(image.bm, image.id);
								if (w.rarity >= 3) // TODO: Add options for choosing rarities
								{
									if (w.IsValid())
									{
										UserInterface.IncrementWeaponCount();
										inventory.Add(w);
										UserInterface.SetGear(image.bm[4], w);
										if (!string.IsNullOrEmpty(w.equippedCharacter))
											equippedWeapons.Add(w);
									}
									else
									{
										UserInterface.AddError($"Unable to validate information for weapon #{image.id}");
										// Maybe save bitmaps in some directory to see what an issue might be
									}
								}
							}
						}
						else if (image.type == "artifact")
						{
							// Scan as artifact
							Artifact artifact = ArtifactScraper.CatalogueFromBitmapsAsync(image.bm, image.id).Result;

							if (artifact.rarity >= 4) // TODO: Add options for choosing rarities
							{
								if (artifact.IsValid())
								{
									UserInterface.IncrementArtifactCount();

									inventory.Add(artifact);

									UserInterface.SetGear(image.bm[7], artifact);
									if (!string.IsNullOrEmpty(artifact.equippedCharacter))
										equippedArtifacts.Add(artifact);
								}
								else
								{
									UserInterface.AddError($"Unable to validate information for artifact #{image.id}");
									// Maybe save bitmaps in some directory to see what an issue might be
								}
							}
						}
						else // not supposed to happen
						{
							Form1.UnexpectedError("Unknown Image type for Image Processor");
						}

						// Dispose of everything
						image.bm.ForEach(b => b.Dispose());
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
			foreach (Artifact artifact in equippedArtifacts)
			{
				foreach (Character character in characters)
				{
					if (artifact.GetEquippedCharacter() == character.GetName())
					{
						character.AssignArtifact(artifact);
						break;
					}
				}
			}
		}

		public void AssignWeapons()
		{
			foreach (Weapon weapon in equippedWeapons)
			{
				foreach (Character character in characters)
				{
					if (weapon.equippedCharacter == character.name)
					{
						Debug.WriteLine($"Assigned {weapon.name} to {character.name}");
						character.AssignWeapon(weapon);
						break;
					}
				}
			}
		}
	}
}