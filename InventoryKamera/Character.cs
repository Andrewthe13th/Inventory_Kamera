using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InventoryKamera
{
    public class Character
    {
        [JsonProperty] public int name { get; private set; }
        [JsonProperty] public int element { get; private set; }
        [JsonProperty] public int level { get; private set; }
        [JsonProperty] public bool ascension { get; private set; }
        [JsonProperty] public int experience { get; private set; }
        [JsonProperty] public Weapon weapon { get; private set; }
        [JsonProperty] private Artifact[] artifacts = new Artifact[5];
        [JsonProperty] public int constellation { get; private set; }
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

        public bool IsValid()
        {
            for (int i = 0; i < 3; i++)
            {
                if (talents[i] == -1 )
                {
                    return false;
                }
            }

            if (name == -1 || level == -1 || element == -1 || constellation == -1)
            {
                return false;
            }
            return true;
        }

        public Artifact[] GetArtifacts()
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

        public int GetName()
        {
            return name;
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
