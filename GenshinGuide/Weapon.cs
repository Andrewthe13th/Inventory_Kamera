using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GenshinGuide
{
    public class Weapon
    {
        [JsonProperty] private int name;
        [JsonProperty] private int level;
        [JsonProperty] private bool ascension;
        [JsonProperty] private int refinementLevel;
        [JsonProperty] private int equippedCharacter;
        [JsonProperty] private int id;

        public Weapon(int _name, int _level,bool _ascension, int _refinementLevel, int _equippedCharacter = 0, int _id = 0)
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


    }
}
