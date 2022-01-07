using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace InventoryKamera
{
	[Serializable]
	public class Character
	{
		[JsonProperty("key")]
		public string Name { get; private set; }

		[JsonProperty("level")]
		public int Level { get; private set; }

		[JsonProperty("constellation")]
		public int Constellation { get; private set; }

		[JsonProperty("ascension")]
		public int Ascension { get { return AscensionLevel(); } private set { } }

		[JsonProperty("talent")]
		public Dictionary<string, int> Talents { get; private set; }

		[JsonIgnore]
		public string Element { get; private set; }

		[JsonIgnore]
		public bool Ascended { get; private set; }

		[JsonIgnore]
		public int Experience { get; private set; }

		[JsonIgnore]
		public Weapon Weapon { get; private set; }

		[JsonIgnore]
		public Dictionary<string, Artifact> Artifacts { get; private set; }

		[JsonIgnore]
		public WeaponType WeaponType { get; private set; }

		public Character()
		{
			Talents = new Dictionary<string, int>
			{
				["auto"] = 0,
				["skill"] = 0,
				["burst"] = 0
			};
			Artifacts = new Dictionary<string, Artifact>();
		}

		public Character(string _name, string _element, int _level, bool _ascension, int _experience, int _constellation, int[] _talents, WeaponType _weaponType) : this()
		{
			try
			{
				Name = (string)Scraper.Characters[_name.ToLower()]["GOOD"];
			}
			catch (Exception)
			{ }
			Element = _element;
			Level = _level;
			Ascended = _ascension;
			Experience = _experience;
			Constellation = _constellation;
			try
			{
				Talents["auto"]  = _talents[0];
				Talents["skill"] = _talents[1];
				Talents["burst"] = _talents[2];
			}
			catch (Exception)
			{ }
			WeaponType = _weaponType;
		}

		public bool IsValid()
		{
			return HasValidName() && HasValidLevel() && HasValidElement() && HasValidConstellation() && HasValidTalents();
		}

		public bool HasValidName()
		{
			return !string.IsNullOrWhiteSpace(Name) && Scraper.IsValidCharacter(Name);
		}

		public bool HasValidLevel()
		{
			return 1 <= Level && Level <= 90;
		}

		public bool HasValidElement()
		{
			return !string.IsNullOrWhiteSpace(Element) && Scraper.IsValidElement(Element);
		}

		public bool HasValidConstellation()
		{
			return 0 <= Constellation && Constellation <= 6;
		}

		public bool HasValidTalents()
		{
			if (Talents is null || Talents.Keys.Count != 3) return false;

			foreach (var value in Talents.Values) if (value < 1 || value > 15) return false;

			return true;
		}

		public void AssignWeapon(Weapon newWeapon)
		{
			Weapon = newWeapon;
		}

		public void AssignArtifact(Artifact artifact)
		{
			Artifacts[artifact.GearSlot] = artifact;
		}

		public int AscensionLevel()
		{
			if (Level < 20 || ( Level == 20 && !Ascended ))
			{
				return 0;
			}
			else if (Level < 40 || ( Level == 40 && !Ascended ))
			{
				return 1;
			}
			else if (Level < 50 || ( Level == 50 && !Ascended ))
			{
				return 2;
			}
			else if (Level < 60 || ( Level == 60 && !Ascended ))
			{
				return 3;
			}
			else if (Level < 70 || ( Level == 70 && !Ascended ))
			{
				return 4;
			}
			else if (Level < 80 || ( Level == 80 && !Ascended ))
			{
				return 5;
			}
			else if (Level <= 90 || ( Level == 90 && !Ascended ))
			{
				return 6;
			}
			return 0;
		}

		public override string ToString()
		{
			string output = "Character\n";
			output += $"Name: {Name}\n";
			output += $"Element: {Element}\n";
			output += $"Level: {Level}{( Ascended ? "+" : "" )}\n";
			output += $"Ascension Level: {Ascension}\n";
			output += $"Constellation: {Constellation}\n";
			foreach (var item in Talents)
			{
				output += $"{item.Key.ToUpper()} : {item.Value}\n";
			}
			return output;
		}
	}
}