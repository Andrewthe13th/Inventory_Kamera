using System.Collections.Generic;
using Newtonsoft.Json;

namespace GenshinGuide
{
	public class Character
	{
		[JsonProperty] public string name { get; private set; }
		[JsonProperty] public string element { get; private set; }
		[JsonProperty] public int level { get; private set; }
		[JsonProperty] public bool ascension { get; private set; }
		[JsonProperty] public int experience { get; private set; }
		[JsonProperty] public Weapon weapon { get; private set; }
		[JsonProperty] private readonly Dictionary<string, Artifact> artifacts = new Dictionary<string, Artifact>();
		[JsonProperty] public int constellation { get; private set; }
		[JsonProperty] private readonly int[] talents = new int[3];

		public Character(string _name, string _element, int _level, bool _ascension, int _experience, int _constellation, int[] _talents)
		{
			name = _name;
			element = _element;
			level = _level;
			ascension = _ascension;
			experience = _experience;
			constellation = _constellation;
			talents = _talents;
		}

		public bool IsValid()
		{
			for (int i = 0; i < 3; i++)
			{
				if (talents[i] == -1)
				{
					return false;
				}
			}

			return Scraper.IsValidCharacter(name) && level != -1 && Scraper.IsValidElement(element) && constellation != -1;
		}

		public Dictionary<string, Artifact> GetArtifacts()
		{
			return artifacts;
		}

		public int[] GetTalents()
		{
			return talents;
		}

		public void AssignWeapon(Weapon newWeapon)
		{
			weapon = newWeapon;
		}

		public void AssignArtifact(Artifact artifact)
		{
			artifacts[artifact.GetGearSlot()] = artifact;
		}

		public string GetName()
		{
			return name;
		}

		public int AscensionLevel()
		{
			if (level < 20 || ( level == 20 && ascension == false ))
			{
				return 0;
			}
			else if (level < 40 || ( level == 40 && ascension == false ))
			{
				return 1;
			}
			else if (level < 50 || ( level == 50 && ascension == false ))
			{
				return 2;
			}
			else if (level < 60 || ( level == 60 && ascension == false ))
			{
				return 3;
			}
			else if (level < 70 || ( level == 70 && ascension == false ))
			{
				return 4;
			}
			else if (level < 80 || ( level == 80 && ascension == false ))
			{
				return 5;
			}
			else if (level <= 90 || ( level == 90 && ascension == false ))
			{
				return 6;
			}
			return 0;
		}
	}
}