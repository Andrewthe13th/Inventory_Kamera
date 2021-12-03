using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GenshinGuide
{
    public class Inventory
    {
        [JsonProperty] private List<Weapon> weapons;
        [JsonProperty] private List<Artifact> artifacts;
        [JsonProperty] private List<Material> materials;
        [JsonProperty] private List<Material> characterDevelopmentItems;
        //private List<Artifact> equippedArtifacts;

        public Inventory()
        {
            weapons = new List<Weapon>();
            artifacts = new List<Artifact>();
            materials = new List<Material>();
            characterDevelopmentItems = new List<Material>();
            //equippedArtifacts = new List<Artifact>();
        }

        public List<Artifact> GetArtifactList()
        {
            return artifacts;
        }

        public List<Weapon> GetWeaponList()
        {
            return weapons;
        }

        public List<Material> GetMaterialList()
        {
            return materials;
        }

        public List<Material> GetCharDevItems()
        {
            return characterDevelopmentItems;
        }

        public void AssignArtifact(Artifact _artifact)
        {
            artifacts.Add(_artifact);
        }

        public void AssignWeapon(Weapon _weapon)
        {
            weapons.Add(_weapon);
        }

        public void AssignMaterials(ref List<Material> _material)
        {
            materials = _material;
        }

        public void AssignCharacterDevelopmentItems(ref List<Material> _material)
        {
            characterDevelopmentItems = _material;
        }

        public void AssignArtifacts(ref List<Artifact> _artifacts)
        {
            artifacts = _artifacts;
        }

        public void AssignWeapons(ref List<Weapon> _weapons)
        {
            weapons = _weapons;
        }
    }
}
