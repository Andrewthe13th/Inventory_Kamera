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

		private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		[JsonProperty]
		public List<Character> Characters { get; private set; }

		[JsonProperty]
		public Inventory Inventory = new Inventory();

		private static List<Artifact> equippedArtifacts;
		private static List<Weapon> equippedWeapons;
		public static Queue<OCRImageCollection> workerQueue;
		private static List<Thread> ImageProcessors;
		private static volatile bool b_threadCancel = false;
		private static int maxProcessors = 2; // TODO: Add support for more processors

		public InventoryKamera()
		{
			Characters = new List<Character>();
			Inventory = new Inventory();
			equippedArtifacts = new List<Artifact>();
			equippedWeapons = new List<Weapon>();
			ImageProcessors = new List<Thread>();
			workerQueue = new Queue<OCRImageCollection>();

			ResetLogging();
		}

		public void ResetLogging()
		{
			try
			{
				Directory.Delete("./logging/weapons", true);
				Directory.Delete("./logging/artifacts", true);
				Directory.Delete("./logging/characters", true);
				Directory.Delete("./logging/materials", true);
			}
			catch { }

			Directory.CreateDirectory("./logging/weapons");
			Directory.CreateDirectory("./logging/artifacts");
			Directory.CreateDirectory("./logging/characters");
			Directory.CreateDirectory("./logging/materials");

			Logger.Info("Logging directory created");
		}

		public void StopImageProcessorWorkers()
		{
			b_threadCancel = true;
			AwaitProcessors();
			workerQueue = new Queue<OCRImageCollection>();
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
			Logger.Debug("Added {ImageProcessors.Count} workers", ImageProcessors.Count);

			Scraper.RestartEngines();

			// Scan Main character Name
			string mainCharacterName = CharacterScraper.ScanMainCharacterName();
			Scraper.AssignTravelerName(mainCharacterName);

			if (Properties.Settings.Default.ScanWeapons)
			{
				Logger.Info("Scanning weapons...");
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
				}
				Navigation.MainMenuScreen();
				Logger.Info("Done scanning weapons");
			}

			if (Properties.Settings.Default.ScanArtifacts)
			{
				Logger.Info("Scanning artifacts...");

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
				Logger.Info("Done scanning artifacts");
			}

			workerQueue.Enqueue(new OCRImageCollection(null, "END", 0));

			if (Properties.Settings.Default.ScanCharacters)
			{
				Logger.Info("Scanning characters...");
				// Get characters
				Navigation.CharacterScreen();
				try
				{
					Characters = CharacterScraper.ScanCharacters();
				}
				catch (ThreadAbortException) { }
				catch (Exception ex)
				{
					UserInterface.AddError(ex.Message + "\n" + ex.StackTrace);
				}
				Navigation.MainMenuScreen();
				Logger.Info("Done scanning characters");
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
				Logger.Info("Scanning character development materials...");
				// Get Materials
				Navigation.InventoryScreen();
				Navigation.SelectCharacterDevelopmentInventory();
				HashSet<Material> devItems = new HashSet<Material>();
				try
				{
					MaterialScraper.Scan_Materials(InventorySection.CharacterDevelopmentItems, ref Inventory);
				}
				catch (FormatException ex) { UserInterface.AddError(ex.Message); }
				catch (ThreadAbortException) { }
				catch (Exception ex)
				{
					UserInterface.AddError(ex.Message + "\n" + ex.StackTrace);
				}
				Navigation.MainMenuScreen();
				Logger.Info("Done scanning character development materials");
			}

			// Scan Materials
			if (Properties.Settings.Default.ScanMaterials)
			{
				Logger.Info("Scanning materials...");
				// Get Materials
				Navigation.InventoryScreen();
				Navigation.SelectMaterialInventory();
				HashSet<Material> materials = new HashSet<Material>();
				try
				{
					MaterialScraper.Scan_Materials(InventorySection.Materials, ref Inventory);
				}
				catch (FormatException ex) { UserInterface.AddError(ex.Message); }
				catch (ThreadAbortException) { }
				catch (Exception ex)
				{
					UserInterface.AddError(ex.Message + "\n" + ex.StackTrace);
				}
				Navigation.MainMenuScreen();
				Logger.Info("Done scanning materials");
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
			Logger.Debug("Thread #{0} priority: {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Priority);
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
								Logger.Debug("Enhancement Material found for weapon #{weaponID}", imageCollection.Id);
								WeaponScraper.StopScanning = true;
								break;
							}

							UserInterface.SetGearPictureBox(imageCollection.Bitmaps.Last());

							// Scan as weapon
							Weapon weapon = WeaponScraper.CatalogueFromBitmapsAsync(imageCollection.Bitmaps, imageCollection.Id).Result;
							UserInterface.SetGear(imageCollection.Bitmaps.Last(), weapon);

							string weaponPath = $"./logging/weapons/weapon{weapon.Id}/";

							if (Properties.Settings.Default.LogScreenshots) Directory.CreateDirectory(weaponPath);

							if (weapon.IsValid())
							{
								if (weapon.Rarity <= (int)Properties.Settings.Default.MinimumWeaponRarity &&
									weapon.Level < (int)Properties.Settings.Default.MinimumWeaponLevel)
                                {
									WeaponScraper.StopScanning = true;
									continue;
                                }
								if (weapon.Level < (int)Properties.Settings.Default.MinimumWeaponLevel) continue;

								UserInterface.IncrementWeaponCount();
								Inventory.Add(weapon);
								if (!string.IsNullOrWhiteSpace(weapon.EquippedCharacter))
									equippedWeapons.Add(weapon);
							}
							else
							{
								UserInterface.AddError($"Unable to validate information for weapon ID#{weapon.Id}");
								string error = "";
								if (!weapon.HasValidWeaponName()) error += "Invalid weapon name\n"; 
								if (!weapon.HasValidRarity()) error += "Invalid weapon rarity\n";
								if (!weapon.HasValidLevel()) error += "Invalid weapon level\n";
								if (!weapon.HasValidRefinementLevel()) error += "Invalid refinement level\n";
								if (!weapon.HasValidEquippedCharacter()) error += "Inavlid equipped character\n";
								UserInterface.AddError(error + weapon.ToString());
								Directory.CreateDirectory(weaponPath);
								using (var writer = File.CreateText(weaponPath + "log.txt"))
								{
									writer.WriteLine($"Version: {Regex.Replace(Assembly.GetExecutingAssembly().GetName().Version.ToString(), @"[.0]*$", string.Empty)}");
									writer.WriteLine($"Resolution: {Navigation.GetWidth()}x{Navigation.GetHeight()}");
									writer.WriteLine("Settings:");
									writer.WriteLine($"\tDelay: {Properties.Settings.Default.ScannerDelay}");
									writer.WriteLine($"\tMinimum Rarity: {Properties.Settings.Default.MinimumWeaponRarity}");
									writer.WriteLine($"\tMinimum Level: {Properties.Settings.Default.MinimumWeaponLevel}");
									writer.WriteLine($"\tEquip: {Properties.Settings.Default.EquipWeapons}");
									writer.WriteLine($"Error log:\n\t{error.Replace("\n", "\n\t")}");
								}
							}

                            if (!weapon.IsValid() || Properties.Settings.Default.LogScreenshots)
                            {
								Directory.CreateDirectory(weaponPath + "name");
								imageCollection.Bitmaps[0].Save(weaponPath + "name/name.png");
								Directory.CreateDirectory(weaponPath + "rarity");
								imageCollection.Bitmaps[0].Save(weaponPath + "rarity/rarity.png");
								Directory.CreateDirectory(weaponPath + "level");
								imageCollection.Bitmaps[1].Save(weaponPath + "level/level.png");
								Directory.CreateDirectory(weaponPath + "refinement");
								imageCollection.Bitmaps[2].Save(weaponPath + "refinement/refinement.png");
								Directory.CreateDirectory(weaponPath + "equipped");
								imageCollection.Bitmaps[4].Save(weaponPath + "equipped/equipped.png");

								imageCollection.Bitmaps.Last().Save(weaponPath + "card.png");
							}

							// Dispose of everything
							imageCollection.Bitmaps.ForEach(b => b.Dispose());
							break;

						case "artifact":
							if (ArtifactScraper.IsEnhancementMaterial(imageCollection.Bitmaps.Last()))
							{
								Logger.Debug("Enhancement Material found for artifact #{artifactID}", imageCollection.Id);
								ArtifactScraper.StopScanning = true;
								break;
							}

							UserInterface.SetGearPictureBox(imageCollection.Bitmaps.Last());
							// Scan as artifact
							Artifact artifact = ArtifactScraper.CatalogueFromBitmapsAsync(imageCollection.Bitmaps, imageCollection.Id).Result;
							UserInterface.SetGear(imageCollection.Bitmaps.Last(), artifact);

							string artifactPath = $"./logging/artifacts/artifact{artifact.Id}/";

                            if (Properties.Settings.Default.LogScreenshots) Directory.CreateDirectory(artifactPath);

							if (artifact.IsValid())
							{
								if (artifact.Rarity <= (int)Properties.Settings.Default.MinimumArtifactRarity &&
									artifact.Level < (int)Properties.Settings.Default.MinimumArtifactLevel)
								{
									ArtifactScraper.StopScanning = true;
									continue;
								}
								if (artifact.Level < (int)Properties.Settings.Default.MinimumArtifactLevel) continue;

								UserInterface.IncrementArtifactCount();
								Inventory.Add(artifact);
								if (!string.IsNullOrWhiteSpace(artifact.EquippedCharacter))
									equippedArtifacts.Add(artifact);
							}
							else
							{
								UserInterface.AddError($"Unable to validate information for artifact ID#{artifact.Id}");
								string error = "";
								if (!artifact.HasValidSlot()) error += "Invalid artifact gear slot\n";
								if (!artifact.HasValidSetName()) error += "Invalid artifact set name\n";
								if (!artifact.HasValidRarity()) error += "Invalid artifact rarity\n";
								if (!artifact.HasValidLevel()) error += "Invalid artifact level\n";
								if (!artifact.HasValidMainStat()) error += "Invalid artifact main stat\n";
								if (!artifact.HasValidSubStats()) error += "Invalid artifact sub stats\n";
								if (!artifact.HasValidEquippedCharacter()) error += "Invalid equipped character\n";
								UserInterface.AddError(error + artifact.ToString());
								Directory.CreateDirectory(artifactPath);
								using (var writer = File.CreateText(artifactPath + "log.txt"))
								{
									writer.WriteLine($"Version: {Regex.Replace(Assembly.GetExecutingAssembly().GetName().Version.ToString(), @"[.0]*$", string.Empty)}");
									writer.WriteLine($"Resolution: {Navigation.GetWidth()}x{Navigation.GetHeight()}");
									writer.WriteLine("Settings:");
									writer.WriteLine($"\tDelay: {Properties.Settings.Default.ScannerDelay}");
									writer.WriteLine($"\tMinimum Rarity: {Properties.Settings.Default.MinimumArtifactRarity}");
									writer.WriteLine($"\tMinimum Level: {Properties.Settings.Default.MinimumArtifactLevel}");
									writer.WriteLine($"\tEquip: {Properties.Settings.Default.EquipArtifacts}");
									writer.WriteLine($"Error Log:\n\t{error.Replace("\n", "\n\t")}");
								}
							}

                            if (!artifact.IsValid() || Properties.Settings.Default.LogScreenshots)
                            {
								Directory.CreateDirectory(artifactPath + "slot");
								imageCollection.Bitmaps[1].Save(artifactPath + "slot/slot.png");
								Directory.CreateDirectory(artifactPath + "set");
								imageCollection.Bitmaps[4].Save(artifactPath + "set/set.png");
								Directory.CreateDirectory(artifactPath + "rarity");
								imageCollection.Bitmaps[0].Save(artifactPath + "rarity/rarity.png");
								Directory.CreateDirectory(artifactPath + "level");
								imageCollection.Bitmaps[3].Save(artifactPath + "level/level.png");
								Directory.CreateDirectory(artifactPath + "mainstat");
								imageCollection.Bitmaps[2].Save(artifactPath + "mainstat/mainstat.png");
								Directory.CreateDirectory(artifactPath + "substats");
								imageCollection.Bitmaps[4].Save(artifactPath + "substats/substats.png");
								Directory.CreateDirectory(artifactPath + "equipped");
								imageCollection.Bitmaps[5].Save(artifactPath + "equipped/equipped.png");

								imageCollection.Bitmaps.Last().Save(artifactPath + "card.png");
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
			Logger.Debug("Thread {threadId} exit", Thread.CurrentThread.ManagedThreadId);
		}

		public void AssignArtifacts()
		{
			foreach (Artifact artifact in equippedArtifacts)
			{
				foreach (Character character in Characters)
				{
					if (artifact.EquippedCharacter == character.Name)
					{
						character.AssignArtifact(artifact); // Do we even need to do this?
						Logger.Debug("Assigned {fearSlot} to {character}", artifact.GearSlot, character.Name);
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
						character.AssignWeapon(weapon);
						Logger.Debug("Assigned {weapon} to {character}", weapon.Name, character.Name);
						break;
					}
				}
				if (character.Weapon is null)
				{
					Inventory.Add(new Weapon(character.WeaponType, character.Name));
					Logger.Info("Default weapon assigned to {character}", character.Name);
				}
			}
		}
	}
}