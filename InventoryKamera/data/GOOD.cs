using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace InventoryKamera
{
    public class GOOD
	{
		[JsonProperty("format")]
		public string Format { get; private set; }

		[JsonProperty("version")]
		public int Version { get; private set; }

		[JsonProperty("kamera_version")]
		public string AppVersion { get; private set; }

		[JsonProperty("source")]
		public string Source { get; private set; }

		[JsonProperty("weapons", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public List<Weapon> Weapons { get; private set; }

		[JsonProperty("artifacts", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public List<Artifact> Artifacts { get; private set; }

		[JsonProperty("characters", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public List<Character> Characters { get; private set; }

		[JsonProperty("materials", DefaultValueHandling = DefaultValueHandling.Ignore)]
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
			AppVersion = Regex.Replace(Assembly.GetExecutingAssembly().GetName().Version.ToString(), @"[.0]*$", string.Empty);
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
			
			// Write file
			WriteToJson(outputFile);
			

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