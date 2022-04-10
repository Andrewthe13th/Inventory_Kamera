using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
		public static Queue<OCRImageCollection> workerQueue;
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
			workerQueue = new Queue<OCRImageCollection>();

			ResetLogging();
		}

		private void ResetLogging()
		{
			try
			{
				Directory.Delete("./logging", true);
			}
			catch { }

			Directory.CreateDirectory("./logging/weapons");
			Directory.CreateDirectory("./logging/artifacts");
			Directory.CreateDirectory("./logging/characters");
			Directory.CreateDirectory("./logging/materials");
		}

		public void StopImageProcessorWorkers()
		{
			b_threadCancel = true;
			AwaitProcessors();
			workerQueue = new Queue<OCRImageCollection>();
		}

		public List<Character> GetCharacters()
		{
			return Characters;
		}

		public void GatherData()
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

			if (Properties.Settings.Default.ScanWeapons)
			{
				// Get Weapons
				Navigation.InventoryScreen();
				Navigation.SelectWeaponInventory();
				try
				{
					WeaponScraper.ScanWeapons();
				}
				catch (FormatException ex) { UserInterface.AddError(ex.Message); }
				catch (ThreadAbortException) { }
				catch (Exception ex)
				{
					UserInterface.AddError(ex.Message + "\n" + ex.StackTrace);
					Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}");
				}
				Navigation.MainMenuScreen();
			}

			if (Properties.Settings.Default.ScanArtifacts)
			{
				// Get Artifacts
				Navigation.InventoryScreen();
				Navigation.SelectArtifactInventory();
				try
				{
					ArtifactScraper.ScanArtifacts();
				}
				catch (FormatException ex) { UserInterface.AddError(ex.Message); }
				catch (ThreadAbortException) { }
				catch (Exception ex)
				{
					UserInterface.AddError(ex.Message + "\n" + ex.StackTrace);
				}
				Navigation.MainMenuScreen();
			}

			workerQueue.Enqueue(new OCRImageCollection(null, "END", 0));

			if (Properties.Settings.Default.ScanCharacters)
			{
				// Get characters
				Navigation.CharacterScreen();
				var c = new List<Character>();
				try
				{
					CharacterScraper.ScanCharacters(ref c);
				}
				catch (ThreadAbortException) { }
				catch (Exception ex)
				{
					UserInterface.AddError(ex.Message + "\n" + ex.StackTrace);
				}
				Characters = c;
				Navigation.MainMenuScreen();
			}

			// Wait for Image Processors to finish
			AwaitProcessors();

			if (Properties.Settings.Default.ScanCharacters)
			{
				// Assign Artifacts to Characters
				if (Properties.Settings.Default.ScanArtifacts)
					AssignArtifacts();
				if (Properties.Settings.Default.ScanWeapons)
					AssignWeapons();
			}

			// Scan Character Development Items
			if (Properties.Settings.Default.ScanCharDevItems)
			{
				// Get Materials
				Navigation.InventoryScreen();
				Navigation.SelectCharacterDevelopmentInventory();
				HashSet<Material> devItems = new HashSet<Material>();
				try
				{
					MaterialScraper.Scan_Materials(InventorySection.CharacterDevelopmentItems, ref Materials);
				}
				catch (FormatException ex) { UserInterface.AddError(ex.Message); }
				catch (ThreadAbortException) { }
				catch (Exception ex)
				{
					UserInterface.AddError(ex.Message + "\n" + ex.StackTrace);
				}
				Navigation.MainMenuScreen();
			}

			// Scan Materials
			if (Properties.Settings.Default.ScanMaterials)
			{
				// Get Materials
				Navigation.InventoryScreen();
				Navigation.SelectMaterialInventory();
				HashSet<Material> materials = new HashSet<Material>();
				try
				{
					MaterialScraper.Scan_Materials(InventorySection.Materials, ref Materials);
				}
				catch (FormatException ex) { UserInterface.AddError(ex.Message); }
				catch (ThreadAbortException) { }
				catch (Exception ex)
				{
					UserInterface.AddError(ex.Message + "\n" + ex.StackTrace);
				}
				Navigation.MainMenuScreen();
			}

			Inventory.AddMaterials(ref Materials);
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

				if (workerQueue.TryDequeue(out OCRImageCollection imageCollection))
				{
					switch (imageCollection.Type)
					{
						case "weapon":
							if (WeaponScraper.IsEnhancementMaterial(imageCollection.Bitmaps.First()))
							{
								Debug.WriteLine($"Enhancement Material found for weapon #{imageCollection.Id}");
								WeaponScraper.StopScanning = true;
								break;
							}

							UserInterface.SetGearPictureBox(imageCollection.Bitmaps.Last());

							// Scan as weapon
							Weapon weapon = WeaponScraper.CatalogueFromBitmapsAsync(imageCollection.Bitmaps, imageCollection.Id).Result;
							UserInterface.SetGear(imageCollection.Bitmaps.Last(), weapon);

							
							if (weapon.IsValid())
							{
								if (weapon.Rarity >= (int)Properties.Settings.Default.MinimumWeaponRarity)
								{
									UserInterface.IncrementWeaponCount();
									Inventory.Add(weapon);
									if (!string.IsNullOrWhiteSpace(weapon.EquippedCharacter))
										equippedWeapons.Add(weapon);
								}
							}
							else
							{
								string weaponPath = $"./logging/weapons/weapon{weapon.Id}/";
								Directory.CreateDirectory(weaponPath);
								UserInterface.AddError($"Unable to validate information for weapon ID#{weapon.Id}");
								string error = "";
								if (!weapon.HasValidWeaponName())
								{
									try
									{
										error += "Invalid weapon name\n";
										Directory.CreateDirectory(weaponPath + "name");
										imageCollection.Bitmaps[0].Save(weaponPath + "name/name.png");
									}
									catch { }
								}
								if (!weapon.HasValidRarity())
								{
									try
									{
										error += "Invalid weapon rarity\n";
										Directory.CreateDirectory(weaponPath + "rarity");
										imageCollection.Bitmaps[0].Save(weaponPath + "rarity/rarity.png");
									}
									catch { }
								}
								if (!weapon.HasValidLevel())
								{
									try
									{
										error += "Invalid weapon level\n";
										Directory.CreateDirectory(weaponPath + "level");
										imageCollection.Bitmaps[1].Save(weaponPath + "level/level.png");
									} catch{ }
								}
								if (!weapon.HasValidRefinementLevel())
								{
									try
									{
										error += "Invalid refinement level\n";
										Directory.CreateDirectory(weaponPath + "refinement");
										imageCollection.Bitmaps[2].Save(weaponPath + "refinement/refinement.png");
									} catch { }
								}
								if (!weapon.HasValidEquippedCharacter())
								{
									try
									{
										error += "Inavlid equipped character\n";
										Directory.CreateDirectory(weaponPath + "equipped");
										imageCollection.Bitmaps[4].Save(weaponPath + "equipped/equipped.png");
									}
									catch { }
								}
								UserInterface.AddError(error + weapon.ToString());
								imageCollection.Bitmaps.Last().Save(weaponPath + "card.png");
								using (var writer = File.CreateText(weaponPath + "log.txt"))
								{
									writer.WriteLine($"Version: {Regex.Replace(Assembly.GetExecutingAssembly().GetName().Version.ToString(), @"[.0]*$", string.Empty)}");
									writer.WriteLine($"Resolution: {Navigation.GetWidth()}x{Navigation.GetHeight()}");
									writer.WriteLine("Settings:");
									writer.WriteLine($"\tDelay: {Properties.Settings.Default.ScannerDelay}");
									writer.WriteLine($"\tMinimum Rarity: {Properties.Settings.Default.MinimumArtifactRarity}");
									writer.WriteLine($"Error log:\n\t{error.Replace("\n", "\n\t")}");
								}
							}
							
							// Dispose of everything
							imageCollection.Bitmaps.ForEach(b => b.Dispose());
							break;

						case "artifact":
							if (ArtifactScraper.IsEnhancementMaterial(imageCollection.Bitmaps.Last()))
							{
								Debug.WriteLine($"Enhancement Material found for artifact #{imageCollection.Id}");
								ArtifactScraper.StopScanning = true;
								break;
							}

								UserInterface.SetGearPictureBox(imageCollection.Bitmaps.Last());
							// Scan as artifact
							Artifact artifact = ArtifactScraper.CatalogueFromBitmapsAsync(imageCollection.Bitmaps, imageCollection.Id).Result;
							UserInterface.SetGear(imageCollection.Bitmaps.Last(), artifact);
							if (artifact.IsValid())
							{
								if (artifact.Rarity >= (int)Properties.Settings.Default.MinimumArtifactRarity)
								{
									UserInterface.IncrementArtifactCount();
									Inventory.Add(artifact);
									if (!string.IsNullOrWhiteSpace(artifact.EquippedCharacter))
										equippedArtifacts.Add(artifact);
								}
							}
							else
							{
								string artifactPath = $"./logging/artifacts/artifact{artifact.Id}/";

								Directory.CreateDirectory(artifactPath);

								UserInterface.AddError($"Unable to validate information for artifact ID#{artifact.Id}");
								string error = "";
								if (!artifact.HasValidSlot())
								{
									try
									{
										error += "Invalid artifact gear slot\n";
										Directory.CreateDirectory(artifactPath + "slot");
										imageCollection.Bitmaps[1].Save(artifactPath + "slot/slot.png");
									}
									catch { }
								}
								if (!artifact.HasValidSetName())
								{
									try
									{
										error += "Invalid artifact set name\n";
										Directory.CreateDirectory(artifactPath + "set");
										imageCollection.Bitmaps[4].Save(artifactPath + "set/set.png");
									}
									catch { }
								}
								if (!artifact.HasValidRarity())
								{
									try
									{
										error += "Invalid artifact rarity\n";
										Directory.CreateDirectory(artifactPath + "rarity");
										imageCollection.Bitmaps[0].Save(artifactPath + "rarity/rarity.png");
									}
									catch { }
								}
								if (!artifact.HasValidLevel())
								{
									try
									{
										error += "Invalid artifact level\n";
										Directory.CreateDirectory(artifactPath + "level");
										imageCollection.Bitmaps[3].Save(artifactPath + "level/level.png");
									}
									catch { }
								}
								if (!artifact.HasValidMainStat())
								{
									try
									{
										error += "Invalid artifact main stat\n";
										Directory.CreateDirectory(artifactPath + "mainstat");
										imageCollection.Bitmaps[2].Save(artifactPath + "mainstat/mainstat.png");
									}
									catch { }
								}
								if (!artifact.HasValidSubStats())
								{
									try
									{
										error += "Invalid artifact sub stats\n";
										Directory.CreateDirectory(artifactPath + "substats");
										imageCollection.Bitmaps[4].Save(artifactPath + "substats/substats.png");
									}
									catch { }
								}
								if (!artifact.HasValidEquippedCharacter())
								{
									try
									{
										error += "Invalid equipped character\n";
										Directory.CreateDirectory(artifactPath + "equipped");
										imageCollection.Bitmaps[5].Save(artifactPath + "equipped/equipped.png");
									}
									catch { }
								}
								UserInterface.AddError(error + artifact.ToString());

								imageCollection.Bitmaps.Last().Save($"./logging/artifacts/artifact{artifact.Id}/card.png");
								using (var writer = File.CreateText(artifactPath + "log.txt"))
								{
									writer.WriteLine($"Version: {Regex.Replace(Assembly.GetExecutingAssembly().GetName().Version.ToString(), @"[.0]*$", string.Empty)}");
									writer.WriteLine($"Resolution: {Navigation.GetWidth()}x{Navigation.GetHeight()}");
									writer.WriteLine("Settings:");
									writer.WriteLine($"\tDelay: {Properties.Settings.Default.ScannerDelay}");
									writer.WriteLine($"\tMinimum Rarity: {Properties.Settings.Default.MinimumArtifactRarity}");
									writer.WriteLine($"Error Log:\n\t{error.Replace("\n", "\n\t")}");
								}
							}
							
							// Dispose of everything
							imageCollection.Bitmaps.ForEach(b => b.Dispose());
							break;

						case "END":
							b_threadCancel = true;
							break;

						default:
							MainForm.UnexpectedError("Unknown Image type for Image Processor");
							break;
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