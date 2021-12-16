using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace InventoryKamera
{
	[Serializable]
	public class Artifact
	{
		[JsonProperty("setKey")]
		public string SetName { get; private set; }

		[JsonProperty("slotKey")]
		public string GearSlot { get; private set; }

		[JsonProperty("rarity")]
		public int Rarity { get; private set; }

		[JsonProperty("mainStatKey")]
		public string MainStat { get; private set; }

		[JsonProperty("level")]
		public int Level { get; private set; }

		[JsonProperty("substats")] 
		public SubStat[] SubStats { get; private set; }

		[JsonProperty("location")]
		public string EquippedCharacter { get; private set; }

		[JsonProperty("lock")] 
		public bool Lock { get; private set; }

		public int SubStatsCount { get; private set; }

		public int Id { get; private set; }

		public Artifact()
		{
			Rarity = -1;
			GearSlot = null;
			MainStat = null;
			Level = -1;
			SubStats = new SubStat[4];
			SubStatsCount = 4;
			SetName = null;
			EquippedCharacter = null;
			Lock = false;
			Id = 0;
		}

		public Artifact(int _rarity, string _gearSlot, string _mainStat, int _level, SubStat[] _subStats, int _subStatsCount, string _setName, string _equippedCharacter = null, int _id = 0, bool _Lock = false)
		{
			GearSlot = _gearSlot;
			Rarity = _rarity;
			MainStat = _mainStat;
			Level = _level;
			SubStats = _subStats;
			SubStatsCount = _subStatsCount;
			SetName = _setName;
			EquippedCharacter = string.IsNullOrWhiteSpace(_equippedCharacter) ? "" : _equippedCharacter;
			Lock = _Lock;
			Id = _id;
		}

		public bool IsValid()
		{
			// Check subStats
			for (int i = 0; i < SubStatsCount; i++)
			{
				if (!Scraper.IsValidStat(SubStats[i].stat) || SubStats[i].value == (decimal)( -1.0 ))
				{
					return false;
				}
			}

			return 0 <= Level
				&& Level <= 20
				&& 0 < Rarity
				&& Scraper.IsValidSlot(GearSlot)
				&& Scraper.IsValidSetName(SetName)
				&& Scraper.IsValidStat(MainStat)
				&& (string.IsNullOrWhiteSpace(EquippedCharacter) || Scraper.IsValidCharacter(EquippedCharacter));
		}

		[Serializable]
		public struct SubStat
		{
			
			[JsonProperty("key")]
			[DefaultValue("")]
			public string stat;

			[JsonProperty("value")]
			[DefaultValue(0)]
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

			return GearSlot == artifact.GearSlot
				&& Rarity == artifact.Rarity
				&& MainStat == artifact.MainStat
				&& Level == artifact.Level
				&& SubStats == artifact.SubStats
				&& SubStatsCount == artifact.SubStatsCount
				&& SetName == artifact.SetName
				&& EquippedCharacter == artifact.EquippedCharacter
				&& Lock == artifact.Lock;
		}

		public override int GetHashCode() => (GearSlot, Rarity, MainStat, Level, SubStats, SubStatsCount, SetName, EquippedCharacter, Lock).GetHashCode();

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