using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GenshinGuide
{
    public class Artifact
    {
        [JsonProperty] public int gearSlot { get; private set; }
        [JsonProperty] public int rarity { get; private set; }
        [JsonProperty] public int mainStat { get; private set; }
        [JsonProperty] public int level { get; private set; }
        [JsonProperty] public SubStats[] subStats { get; private set; }
        [JsonProperty] public int subStatsCount { get; private set; }
        [JsonProperty] public int setName { get; private set; }
        [JsonProperty] public int equippedCharacter { get; private set; }

        [JsonProperty] public bool _lock { get; private set; }
        [JsonProperty] public int id { get; private set; }

        public Artifact(int _rarity, int _gearSlot, int _mainStat, int _level, SubStats[] _subStats, int _subStatsCount, int _setName, int _equippedCharacter = 0, int _id = 0, bool _Lock = false)
        {
            gearSlot = _gearSlot;
            rarity = _rarity;
            mainStat = _mainStat;
            level = _level;
            subStats = _subStats;
            subStatsCount = _subStatsCount;
            setName = _setName;
            equippedCharacter = _equippedCharacter;
            _lock = _Lock;
            id = _id;
        }

        public int GetGearSlot()
        {
            return gearSlot;
        }

        public int GetEquippedCharacter()
        {
            return equippedCharacter;
        }

        public struct SubStats
        {
            public int stat;
            public decimal value;
        }
        
    }

    
}
