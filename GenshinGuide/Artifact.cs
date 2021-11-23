using System;
using Newtonsoft.Json;

namespace GenshinGuide
{
	public class Artifact
	{
		[JsonProperty] public string gearSlot { get; private set; }
		[JsonProperty] public int rarity { get; private set; }
		[JsonProperty] public string mainStat { get; private set; }
		[JsonProperty] public int level { get; private set; }
		[JsonProperty] public SubStat[] subStats { get; private set; }
		[JsonProperty] public int subStatsCount { get; private set; }
		[JsonProperty] public string setName { get; private set; }
		[JsonProperty] public string equippedCharacter { get; private set; }
		[JsonProperty] public bool @lock { get; private set; }
		[JsonProperty] public int id { get; private set; }

		public Artifact()
		{
			rarity = -1;
			gearSlot = null;
			mainStat = null;
			level = -1;
			subStats = new SubStat[4];
			subStatsCount = 4;
			setName = null;
			equippedCharacter = null;
			@lock = false;
			id = 0;
		}

		public Artifact(int _rarity, string _gearSlot, string _mainStat, int _level, SubStat[] _subStats, int _subStatsCount, string _setName, string _equippedCharacter = null, int _id = 0, bool _Lock = false)
		{
			gearSlot = _gearSlot;
			rarity = _rarity;
			mainStat = _mainStat;
			level = _level;
			subStats = _subStats;
			subStatsCount = _subStatsCount;
			setName = _setName;
			equippedCharacter = _equippedCharacter;
			@lock = _Lock;
			id = _id;
		}

		public string GetGearSlot()
		{
			return gearSlot;
		}

		public bool IsValid()
		{
			// Check subStats
			for (int i = 0; i < subStatsCount; i++)
			{
				if (subStats[i].stat == null)
				{
					continue;
				}
				if (!Scraper.IsValidStat(subStats[i].stat) || subStats[i].value == (decimal)( -1.0 ))
				{
					return false;
				}
			}

			return 0 <= level && level <= 20 && rarity != 0 && Scraper.IsValidSlot(gearSlot) && Scraper.IsValidSetName(setName) && Scraper.IsValidStat(mainStat);
		}

		public string GetEquippedCharacter()
		{
			return equippedCharacter;
		}

		public struct SubStat
		{
			public string stat;
			public decimal value;

			public override string ToString()
			{
				return stat.Contains("%") || stat.Contains("crit") || stat.Contains("bonus") ? $"{stat.Replace("%", "")} + {value}%" : $"{stat} + {value}";
			}
		}

		public override bool Equals(object obj) => this.Equals(obj as Artifact);

		public bool Equals(Artifact artifact)
		{
			if (artifact is null)
			{
				return false;
			}

			if (Object.ReferenceEquals(this, artifact))
			{
				return true;
			}

			if (GetType() != artifact.GetType())
			{
				return false;
			}

			return gearSlot == artifact.gearSlot
				&& rarity == artifact.rarity
				&& mainStat == artifact.mainStat
				&& level == artifact.level
				&& subStats == artifact.subStats
				&& subStatsCount == artifact.subStatsCount
				&& setName == artifact.setName
				&& equippedCharacter == artifact.equippedCharacter
				&& @lock == artifact.@lock;
		}

		public override int GetHashCode() => (gearSlot, rarity, mainStat, level, subStats, subStatsCount, setName, equippedCharacter, @lock).GetHashCode();

		public static bool operator ==(Artifact lhs, Artifact rhs)
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

		public static bool operator !=(Artifact lhs, Artifact rhs) => !( lhs == rhs );
	}
}