using System.Collections.Generic;
using Newtonsoft.Json;

namespace GenshinGuide
{
	public class Inventory
	{
		[JsonProperty] private List<Weapon> weapons;
		[JsonProperty] private List<Artifact> artifacts;

		public int size
		{
			get => weapons.Count + artifacts.Count;
			set => size = weapons.Count + artifacts.Count;
		}

		public Inventory()
		{
			weapons = new List<Weapon>();
			artifacts = new List<Artifact>();
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

		public void SetArtifacts(ref List<Artifact> _artifacts)
		{
			artifacts = _artifacts;
		}

		public void SetWeapons(ref List<Weapon> _weapons)
		{
			weapons = _weapons;
		}
	}
}