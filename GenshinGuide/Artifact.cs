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
        [JsonProperty] private int gearSlot;
        [JsonProperty] private int rarity;
        [JsonProperty] private int mainStat;
        [JsonProperty] private decimal mainStatValue;
        [JsonProperty] private int level;
        [JsonProperty] private SubStats[] subStats;
        [JsonProperty] private int subStatsCount;
        [JsonProperty] private int setName;
        [JsonProperty] private int equippedCharacter;
        [JsonProperty] private int id;

        public Artifact(int _rarity, int _gearSlot, int _mainStat, decimal _mainStatValue, int _level, SubStats[] _subStats, int _subStatsCount, int _setName, int _equippedCharacter = 0, int _id = 0)
        {
            gearSlot = _gearSlot;
            rarity = _rarity;
            mainStat = _mainStat;
            mainStatValue = _mainStatValue;
            level = _level;
            subStats = _subStats;
            subStatsCount = _subStatsCount;
            setName = _setName;
            equippedCharacter = _equippedCharacter;
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

        //public void DebugPrintArtifact()
        //{
        //    string artifactText = "GearSlot: " + _gearSlot.ToString() + "\n";
        //    artifactText += "MainStat: " + _mainStat.ToString() + "\n";
        //    artifactText += "MainStatValue: " + _mainStatValue.ToString() + "\n";
        //    artifactText += "Rarity: " + _rarity.ToString() + "\n";
        //    artifactText += "Level: " + _level.ToString() + "\n";
        //    artifactText += "substat Count: " + _subStatsCount + "\n";
        //    artifactText += "Substat[0]: " + _subStats[0]._stat.ToString() + " " + _subStats[0]._value.ToString() + "\n";
        //    artifactText += "Substat[1]: " + _subStats[1]._stat.ToString() + " " + _subStats[1]._value.ToString() + "\n";
        //    artifactText += "Substat[2]: " + _subStats[2]._stat.ToString() + " " + _subStats[2]._value.ToString() + "\n";
        //    artifactText += "Substat[3]: " + _subStats[3]._stat.ToString() + " " + _subStats[3]._value.ToString() + "\n";
        //    artifactText += "SetName: " + _setName.ToString() + "\n";
        //    artifactText += "Equipped Character: " + _equippedCharacter.ToString() + "\n";
        //    artifactText += "\n\n\n\n";

        //    Debug.Print(artifactText);
        //}

        //public void TextBoxPrintArtifact(TextBox textBox)
        //{
        //    textBox.Clear();

        //    textBox.AppendText("GearSlot: " + _gearSlot.ToString());
        //    textBox.AppendText(Environment.NewLine);
        //    textBox.AppendText("MainStat: " + _mainStat.ToString());
        //    textBox.AppendText(Environment.NewLine);
        //    textBox.AppendText("MainStatValue: " + _mainStatValue.ToString());
        //    textBox.AppendText(Environment.NewLine);
        //    textBox.AppendText("Rarity: " + _rarity.ToString());
        //    textBox.AppendText(Environment.NewLine);
        //    textBox.AppendText("Level: " + _level.ToString());
        //    textBox.AppendText(Environment.NewLine);
        //    textBox.AppendText("Substat Count: " + _subStatsCount);
        //    textBox.AppendText(Environment.NewLine);
        //    textBox.AppendText("Substat[0]: " + _subStats[0]._stat.ToString() + " " + _subStats[0]._value.ToString());
        //    textBox.AppendText(Environment.NewLine);
        //    textBox.AppendText("Substat[1]: " + _subStats[1]._stat.ToString() + " " + _subStats[1]._value.ToString());
        //    textBox.AppendText(Environment.NewLine);
        //    textBox.AppendText("Substat[2]: " + _subStats[2]._stat.ToString() + " " + _subStats[2]._value.ToString());
        //    textBox.AppendText(Environment.NewLine);
        //    textBox.AppendText("Substat[3]: " + _subStats[3]._stat.ToString() + " " + _subStats[3]._value.ToString());
        //    textBox.AppendText(Environment.NewLine);
        //    textBox.AppendText("SetName: " + _setName.ToString());
        //    textBox.AppendText(Environment.NewLine);
        //    textBox.AppendText("Equipped Character: " + _equippedCharacter.ToString());

        //    textBox.Refresh();
        //}

        public struct SubStats
        {
            public int stat;
            public decimal value;
        }
        
    }

    
}
