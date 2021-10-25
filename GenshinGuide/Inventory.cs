using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace GenshinGuide
{
	public class Inventory
	{
		[JsonProperty] private List<Weapon> weapons;
		[JsonProperty] private List<Artifact> artifacts;

		public Inventory()
		{
			weapons = new List<Weapon>();
			artifacts = new List<Artifact>();
		}

		public void Add(Weapon w)
		{
			Debug.WriteLine("Weapon scanned to inventory");
			weapons.Add(w);
		}

		public void Add(Artifact a)
		{
			Debug.WriteLine("Artifact scanned to inventory");
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
