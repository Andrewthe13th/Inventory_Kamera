using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GenshinGuide
{
    public class Character
    {
        [JsonProperty] private int name;
        [JsonProperty] private int element;
        [JsonProperty] private int level;
        [JsonProperty] private bool ascension;
        [JsonProperty] private int experience;
        [JsonProperty] private Weapon weapon;
        [JsonProperty] private Artifact[] artifacts = new Artifact[5];
        [JsonProperty] private int constellation;
        [JsonProperty] private int[] talents = new int[3];

        public Character(int _name, int _element, int _level, bool _ascension, int _experience, int _constellation, int[] _talents)
        {
            name = _name;
            element = _element;
            level = _level;
            ascension = _ascension;
            experience = _experience;
            constellation = _constellation;
            talents = _talents;
        }

        public void AssignWeapon(Weapon newWeapon)
        {
            weapon = newWeapon;
        }

        public void AssignArtifact(Artifact artifact)
        {
            artifacts[artifact.GetGearSlot()] = artifact;
        }

        public int GetName()
        {
            return name;
        }

    }
}
