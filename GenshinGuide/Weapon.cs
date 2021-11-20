using System;
using Newtonsoft.Json;

namespace GenshinGuide
{
	public class Weapon
	{
		[JsonProperty] public string name { get; private set; }
		[JsonProperty] public int level { get; private set; }
		[JsonProperty] public bool ascension { get; private set; }
		[JsonProperty] public int refinementLevel { get; private set; }
		[JsonProperty] public string equippedCharacter { get; private set; }
		public int id { get; private set; }

		public int rarity { get;  private set; }

		public Weapon()
		{
			name = null;
			level = -1;
			ascension = false;
			refinementLevel = -1;
			equippedCharacter = null;
			id = -1;
			rarity = -1;
		}

		public Weapon(string _name, int _level, bool _ascension, int _refinementLevel, string _equippedCharacter = null, int _id = 0, int _rarity = -1)
		{
			name = _name;
			level = _level;
			ascension = _ascension;
			refinementLevel = _refinementLevel;
			equippedCharacter = _equippedCharacter;
			id = _id;
			rarity = _rarity;
		}

		public string GetEquippedCharacter()
		{
			return equippedCharacter;
		}

		public string GetName()
		{
			return name;
		}

		public bool IsValid()
		{
			return	0 <= level && level <= 90 && 0 <= refinementLevel && refinementLevel <= 5 && Scraper.IsValidWeapon(name);
		}

		public int AscensionCount()
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

		public override bool Equals(object obj)
		{
			return base.Equals(obj as Weapon);
		}

		public bool Equals(Weapon weapon)
		{
			if (weapon is null)
			{
				return false;
			}

			if (Object.ReferenceEquals(this, weapon))
			{
				return true;
			}

			if (GetType() != weapon.GetType())
			{
				return false;
			}

			return name == weapon.name && level == weapon.level && ascension == weapon.ascension && refinementLevel == weapon.refinementLevel;
		}

		public override int GetHashCode() => (name, level, ascension, refinementLevel, equippedCharacter).GetHashCode();

		public static bool operator ==(Weapon lhs, Weapon rhs)
		{
			if (lhs is null)
			{
				if (rhs is null)
				{
					return true;
				}
				return false;
			}

			return lhs.Equals(rhs);
		}

		public static bool operator !=(Weapon lhs, Weapon rhs) => !( lhs == rhs );
	}
}