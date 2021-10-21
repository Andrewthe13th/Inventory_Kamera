using Newtonsoft.Json;

namespace GenshinGuide
{
	public class Weapon
	{
		[JsonProperty] public int name { get; private set; }
		[JsonProperty] public int level { get; private set; }
		[JsonProperty] public bool ascension { get; private set; }
		[JsonProperty] public int refinementLevel { get; private set; }
		[JsonProperty] public int equippedCharacter { get; private set; }
		[JsonProperty] public int id { get; private set; }

		public Weapon(int _name, int _level, bool _ascension, int _refinementLevel, int _equippedCharacter = 0, int _id = 0)
		{
			name = _name;
			level = _level;
			ascension = _ascension;
			refinementLevel = _refinementLevel;
			equippedCharacter = _equippedCharacter;
			id = _id;
		}

		public int GetEquippedCharacter()
		{
			return equippedCharacter;
		}

		public int GetName()
		{
			return name;
		}

		public bool IsValid()
		{
			if (name == -1 || level == -1 || refinementLevel == -1 || equippedCharacter == -1)
			{
				return false;
			}
			return true;
		}

		public int AscensionCount()
		{
			if (level < 20 || (level == 20 && ascension == false))
			{
				return 0;
			}
			else if (level < 40 || (level == 40 && ascension == false))
			{
				return 1;
			}
			else if (level < 50 || (level == 50 && ascension == false))
			{
				return 2;
			}
			else if (level < 60 || (level == 60 && ascension == false))
			{
				return 3;
			}
			else if (level < 70 || (level == 70 && ascension == false))
			{
				return 4;
			}
			else if (level < 80 || (level == 80 && ascension == false))
			{
				return 5;
			}
			else if (level <= 90 || (level == 90 && ascension == false))
			{
				return 6;
			}
			return 0;
		}

	}
}
