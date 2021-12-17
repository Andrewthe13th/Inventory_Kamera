using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;

namespace InventoryKamera
{
	public class InventoryKamera
	{
		[JsonProperty]
		public List<Character> Characters { get; private set; }

		[JsonProperty]
		public Inventory Inventory { get; private set; }

		private static List<Artifact> equippedArtifacts;
		private static List<Weapon> equippedWeapons;
		private static HashSet<Material> Materials;
		public static Queue<OCRImage> workerQueue;
		private static List<Thread> ImageProcessors;
		private static volatile bool b_threadCancel = false;
		private static int maxProcessors = 2; // TODO: Add support for more processors
											  // TODO add language option

		public InventoryKamera()
		{
			Characters = new List<Character>();
			Inventory = new Inventory();
			equippedArtifacts = new List<Artifact>();
			equippedWeapons = new List<Weapon>();
			Materials = new HashSet<Material>();
			ImageProcessors = new List<Thread>();
			workerQueue = new Queue<OCRImage>();
		}

		public void StopImageProcessorWorkers()
		{
			b_threadCancel = true;
			AwaitProcessors();
			workerQueue = new Queue<OCRImage>();
		}

		public List<Character> GetCharacters()
		{
			return Characters;
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
					Characters = new List<Character>();
					Characters = CharacterScraper.ScanCharacters();
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
					HashSet<Material> devItems = MaterialScraper.Scan_Materials(InventorySection.CharacterDevelopmentItems);
					Inventory.AddDevItems(ref devItems);
					Navigation.MainMenuScreen();
				}

				// Scan Materials
				if (checkbox[4])
				{
					// Get Materials
					Navigation.InventoryScreen();
					Navigation.SelectMaterialInventory();
					HashSet<Material> materials = MaterialScraper.Scan_Materials(InventorySection.Materials);
					Inventory.AddMaterials(ref materials);
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
								Weapon w = WeaponScraper.CatalogueFromBitmapsAsync(image.bm, image.id).Result;
								if (w.Rarity >= 3) // TODO: Add options for choosing rarities
								{
									if (w.IsValid())
									{
										UserInterface.IncrementWeaponCount();
										Inventory.Add(w);
										UserInterface.SetGear(image.bm[4], w);
										if (!string.IsNullOrWhiteSpace(w.EquippedCharacter))
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

							if (artifact.Rarity >= 4) // TODO: Add options for choosing rarities
							{
								if (artifact.IsValid())
								{
									UserInterface.IncrementArtifactCount();

									Inventory.Add(artifact);

									UserInterface.SetGear(image.bm[7], artifact);
									if (!string.IsNullOrWhiteSpace(artifact.EquippedCharacter))
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
				foreach (Character character in Characters)
				{
					if (artifact.EquippedCharacter == character.Name)
					{
						Debug.WriteLine($"Assigned {artifact.GearSlot} to {character.Name}");
						character.AssignArtifact(artifact); // Do we even need to do this?
						break;
					}
				}
			}
		}

		public void AssignWeapons()
		{
			foreach (Character character in Characters)
			{
				foreach (Weapon weapon in equippedWeapons)
				{
					if (weapon.EquippedCharacter == character.Name)
					{
						Debug.WriteLine($"Assigned {weapon.Name} to {character.Name}");
						character.AssignWeapon(weapon);
						break;
					}
				}
				if (character.Weapon is null)
				{
					Inventory.Add(new Weapon(character.WeaponType, character.Name));
				}
			}
		}
	}
}