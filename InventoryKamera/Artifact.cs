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
			GearSlot = string.IsNullOrWhiteSpace(_gearSlot) ? "" : _gearSlot ;
			Rarity = _rarity;
			MainStat = string.IsNullOrWhiteSpace(_mainStat) ? "" : _mainStat;
			Level = _level;
			SubStats = _subStats;
			SubStatsCount = _subStatsCount;
			SetName = string.IsNullOrWhiteSpace(_setName) ? "" : _setName;
			EquippedCharacter = string.IsNullOrWhiteSpace(_equippedCharacter) ? "" : _equippedCharacter;
			Lock = _Lock;
			Id = _id;
		}

		public bool IsValid()
		{
			return HasValidLevel() && HasValidRarity() && HasValidSlot() && HasValidSetName() && HasValidMainStat() && HasValidSubStats() && HasValidEquippedCharacter();
		}

		public bool HasValidLevel()
		{
			return 0 <= Level && Level <= 20;
		}

		public bool HasValidRarity()
		{
			return 1 <= Rarity && Rarity <= 5;
		}

		public bool HasValidSlot()
		{
			return Scraper.IsValidSlot(GearSlot);
		}

		public bool HasValidSetName()
		{
			return Scraper.IsValidSetName(SetName);
		}

		public bool HasValidMainStat()
		{
			return Scraper.IsValidStat(MainStat);
		}

		public bool HasValidSubStats()
		{
			bool valid = true;
			for (int i = 0; i < SubStatsCount; i++)
			{
				if (!Scraper.IsValidStat(SubStats[i].stat) || SubStats[i].value == (decimal)( -1.0 ))
				{
					valid = false;
				}
			}
			return valid;
		}

		public bool	HasValidEquippedCharacter()
		{
			return  string.IsNullOrWhiteSpace(EquippedCharacter) || Scraper.IsValidCharacter(EquippedCharacter) ;
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
				return stat is null
					? "NULL"
					: stat.Contains("%") || stat.Contains("crit") || stat.Contains("bonus") ? $"{stat.Replace("%", "")} + {value}%" : $"{stat} + {value}";
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

		public override string ToString()
{
			string output = $"Artifact ID: {Id}\n"
				+ $"Set: {SetName}\n"
				+ $"Rarity: {Rarity}\n"
				+ $"Level: {Level}\n"
				+ $"Slot: {GearSlot}\n"
				+ $"Main Stat: {MainStat}\n";

			for (int i = 0; i < SubStatsCount; i++)
			{
				if (!string.IsNullOrWhiteSpace(SubStats[i].stat)) output += $"Substat {i + 1}: {SubStats[i]}\n";
			}

			output += $"Locked: {Lock}\n";

			if (!string.IsNullOrWhiteSpace(EquippedCharacter)) output += $"Equipped character: {EquippedCharacter}\n";
			return output;
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