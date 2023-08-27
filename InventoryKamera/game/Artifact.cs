using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
		public List<SubStat> SubStats { get; private set; }

		[JsonProperty("location")]
		public string EquippedCharacter { get; internal set; }	

		[JsonProperty("lock")]
		public bool Lock { get; private set; }

        [JsonProperty("id")]
		public int Id { get; private set; }
		
		public Artifact()
		{
			Rarity = -1;
			GearSlot = null;
			MainStat = null;
			Level = -1;
			SubStats = new List<SubStat>(4);
			SetName = null;
			EquippedCharacter = null;
			Lock = false;
			Id = 0;
		}

		public Artifact(string _setName, int _rarity, int _level, string _gearSlot, string _mainStat, List<SubStat> _subStats, string _equippedCharacter = null, int _id = 0, bool _Lock = false)
		{
			GearSlot = string.IsNullOrWhiteSpace(_gearSlot) ? "" : _gearSlot;
			Rarity = _rarity;
			MainStat = string.IsNullOrWhiteSpace(_mainStat) ? "" : _mainStat;
			Level = _level;
			SubStats = _subStats.ToList().Where(e => e.value > 0).ToList();
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
			return GenshinProcesor.IsValidSlot(GearSlot);
		}

		public bool HasValidSetName()
		{
			return GenshinProcesor.IsValidSetName(SetName);
		}

		public bool HasValidMainStat()
		{
			return GenshinProcesor.IsValidStat(MainStat);
		}

		public bool HasValidSubStats()
		{
			bool valid = true;

			SubStats.ForEach(s =>
			{
                if (!string.IsNullOrWhiteSpace(s.stat) &&
                    (!GenshinProcesor.IsValidStat(s.stat) || s.value == (decimal)(-1.0)))
                {
                    valid = false;
                }
            });

			return valid;
		}

		public bool HasValidEquippedCharacter()
		{
			return string.IsNullOrWhiteSpace(EquippedCharacter) || GenshinProcesor.IsValidCharacter(EquippedCharacter);
		}

		[Serializable]
		public struct SubStat
		{
			[JsonProperty("key")]
			[DefaultValue("")]
			public string stat;

			[JsonProperty("value")]
			[DefaultValue(-1)]
			public decimal value;

			public override string ToString()
			{
				return stat is null
					? "NULL"
					: stat.Contains("_") ? $"{stat} + {value}%" : $"{stat} + {value}";
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
				&& SetName == artifact.SetName
				&& EquippedCharacter == artifact.EquippedCharacter
				&& Lock == artifact.Lock;
		}

		public override string ToString()
		{
			string output = $"Artifact ID: {Id}\n"
				+ $"Slot: {GearSlot}\n"
				+ $"Set: {SetName}\n"
				+ $"Rarity: {Rarity}\n"
				+ $"Level: {Level}\n"
				+ $"Main Stat: {MainStat}\n";

			SubStats.ForEach(s => output += $"Substat {SubStats.IndexOf(s)+1}: {s}\n");

			output += $"Locked: {Lock}\n";

			if (!string.IsNullOrWhiteSpace(EquippedCharacter)) output += $"Equipped character: {EquippedCharacter}\n";
			return output;
		}

		public override int GetHashCode() => (GearSlot, Rarity, MainStat, Level, SubStats, SetName, EquippedCharacter, Lock).GetHashCode();

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