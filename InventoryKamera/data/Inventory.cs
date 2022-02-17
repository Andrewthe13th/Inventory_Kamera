using System.Collections.Generic;
using Newtonsoft.Json;

namespace InventoryKamera
{
	public class Inventory
	{
		[JsonProperty]
		public List<Weapon> Weapons { get; private set; }

		[JsonProperty]
		public List<Artifact> Artifacts { get; private set; }

		[JsonProperty]
		public HashSet<Material> Materials { get; private set; }

		[JsonProperty]
		public HashSet<Material> DevMaterials { get; private set; }

		[JsonProperty]
		public HashSet<Material> AllMaterials
		{
			get
			{
				var all = new HashSet<Material>();
				all.UnionWith(Materials);
				all.UnionWith(DevMaterials);
				return all;
			}
			private set { }
		}

		public int Size
		{
			get
			{
				return Weapons.Count + Artifacts.Count + Materials.Count + DevMaterials.Count;
			}

			set => Size = Weapons.Count + Artifacts.Count + AllMaterials.Count;
		}

		public Inventory()
		{
			Weapons = new List<Weapon>();
			Artifacts = new List<Artifact>();
			Materials = new HashSet<Material>();
			DevMaterials = new HashSet<Material>();
		}

		public void Add(Weapon w)
		{
			Weapons.Add(w);
		}

		public void Add(Artifact a)
		{
			Artifacts.Add(a);
		}

		public void AddMaterials(ref HashSet<Material> _material)
		{
			Materials.UnionWith(_material);
		}

		public void AddDevItems(ref HashSet<Material> _material)
		{
			DevMaterials.UnionWith(_material);
		}
	}
}