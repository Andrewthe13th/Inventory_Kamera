using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InventoryKamera
{
	public class GOOD
	{
		[JsonProperty("format")]
		public string Format { get; private set; }

		[JsonProperty("version")]
		public int Version { get; private set; }

		[JsonProperty("source")]
		public string Source { get; private set; }

		[JsonProperty("weapons", NullValueHandling = NullValueHandling.Ignore)]
		public List<Weapon> Weapons { get; private set; }

		[JsonProperty("artifacts", NullValueHandling = NullValueHandling.Ignore)]
		public List<Artifact> Artifacts { get; private set; }

		[JsonProperty("characters", NullValueHandling = NullValueHandling.Ignore)]
		public List<Character> Characters { get; private set; }

		[JsonProperty("materials", NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<string, int> Materials { get; private set; }

		public GOOD()
		{
			Format = "EMPTY";
			Version = 0;
			Source = "NOT FILLED";
		}

		public GOOD(InventoryKamera genshinData) : this()
		{
			// Get rid of VS warning since we are converting this class to JSON
			Format = "GOOD";
			Version = 1;
			Source = "Inventory_Kamera";

			// Assign Characters
			if (genshinData.Characters.Count > 0) Characters = new List<Character>(genshinData.Characters);

			// Assign Weapons
			if (genshinData.Inventory.Weapons.Count > 0) Weapons = new List<Weapon>(genshinData.Inventory.Weapons);

			// Assign Artifacts
			if (genshinData.Inventory.Artifacts.Count > 0) Artifacts = new List<Artifact>(genshinData.Inventory.Artifacts);

			// Assign materials
			if (genshinData.Inventory.AllMaterials.Count > 0) Materials = new Dictionary<string, int>();
			genshinData.Inventory.AllMaterials.ToList().ForEach(material => Materials.Add(material.name, material.count));
		}

		internal void WriteToJSON(string outputDirectory, string oldDataFilePath = "")
		{
			// Creates directory if doesn't exist
			Directory.CreateDirectory(outputDirectory);

			// Create file with timestamp in name
			string fileName = "\\genshinData_GOOD_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + ".json";
			fileName = fileName.Replace('/', '_');
			string outputFile = outputDirectory + fileName;
			if (File.Exists(oldDataFilePath) && false) // && false hack. TODO: add support for data merging
			{
				try
				{
					// Try to load external GOOD data to update.
					// For preserving information at when uploading data to
					// https://frzyc.github.io/genshin-optimizer
					// This may not even be necessary
					JObject database;
					// Load source data
					using (StreamReader file = File.OpenText(oldDataFilePath))
					using (JsonTextReader reader = new JsonTextReader(file))
					{
						database = (JObject)JToken.ReadFrom(reader);
					}

					// Characters
					//foreach (ICharacter character in characters)
					//{
					//	foreach (JObject dbCharacter in database["characters"])
					//	{
					//		{
					//			if ((string)dbCharacter["key"] == character.key)
					//			{
					//				dbCharacter["level"] = character.level;
					//				dbCharacter["constellation"] = character.constellation;
					//				dbCharacter["ascension"] = character.ascension;
					//				dbCharacter["talent"] = JObject.FromObject(character.talent);
					//				break;
					//			}
					//		}
					//	}
					//}
					// Weapons
					//foreach (IWeapon weapon in weapons)
					//{
					//	foreach (JToken dbWeapon in database["weapons"])
					//	{
					//		if ((string)dbWeapon["key"] == weapon.key)
					//		{
					//			break;
					//		}
					//	}
					//}
					// Artifacts

					using (StreamWriter file = File.CreateText(outputFile))
					using (JsonWriter writer = new JsonTextWriter(file))
					{
						database.WriteTo(writer);
					}
					Debug.WriteLine("Successfully merged databases");
				}
				catch (Exception)
				{
					UserInterface.AddError("Unable to create merged database.");
					WriteToJson(outputFile);
				}
			}
			else
			{
				// Write file
				WriteToJson(outputFile);
			}

			if (!File.Exists(outputFile)) // did not make file
			{
				UserInterface.AddError($"Failed to output at : {outputDirectory}");
			}
		}

		private void WriteToJson(string outputFile)
		{
			using (var streamWriter = new StreamWriter(outputFile))
			{
				streamWriter.WriteLine(JsonConvert.SerializeObject(this).ToString());
			}
		}
	}
}