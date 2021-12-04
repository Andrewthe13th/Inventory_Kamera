using System.Collections.Generic;
using Newtonsoft.Json;

namespace InventoryKamera
{
    public class Inventory
    {
        [JsonProperty] private List<Weapon> weapons;
        [JsonProperty] private List<Artifact> artifacts;
        [JsonProperty] private List<Material> materials;
        [JsonProperty] private List<Material> characterDevelopmentItems;
        //private List<Artifact> equippedArtifacts;

		public int size
		{
			get => weapons.Count + artifacts.Count;
			set => size = weapons.Count + artifacts.Count;
		}

        public Inventory()
        {
            weapons = new List<Weapon>();
            artifacts = new List<Artifact>();
            materials = new List<Material>();
            characterDevelopmentItems = new List<Material>();
            //equippedArtifacts = new List<Artifact>();
        }

		public void Add(Weapon w)
		{
			weapons.Add(w);
		}

		public void Add(Artifact a)
		{
			artifacts.Add(a);
		}

		public List<Artifact> GetArtifacts()
		{
			return artifacts;
		}

		public List<Weapon> GetWeapons()
		{
			return weapons;
		}

		public List<Material> GetMaterials()
		{
			return materials;
		}

		public List<Material> GetCharDevItems()
		{
			return characterDevelopmentItems;
		}

		public void SetArtifacts(ref List<Artifact> _artifacts)
		{
			artifacts = _artifacts;
		}

		public void SetWeapons(ref List<Weapon> _weapons)
		{
			weapons = _weapons;
		}

		public void SetMaterials(ref List<Material> _material)
		{
			materials = _material;
		}

		public void SetCharacterDevelopmentItems(ref List<Material> _material)
		{
			characterDevelopmentItems = _material;
		}
	}
}