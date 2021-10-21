using Newtonsoft.Json;
using System.Collections.Generic;

namespace GenshinGuide
{
	public class Inventory
	{
		[JsonProperty] private List<Weapon> weapons;
		[JsonProperty] private List<Artifact> artifacts;
		//private List<Artifact> equippedArtifacts;

		public Inventory()
		{
			weapons = new List<Weapon>();
			artifacts = new List<Artifact>();
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

		public void AssignArtifact(Artifact _artifact)
		{
			artifacts.Add(_artifact);
			//artifacts = ArtifactScraper.ScanArtifacts(ref equippedArtifacts);
			//ArtifactScraper.ScanArtifacts();
		}

		public void AssignWeapon(Weapon _weapon)
		{
			weapons.Add(_weapon);
			//weapons = WeaponScraper.ScanWeapons(ref equippedWeapon);
		}

		public void AssignArtifacts(ref List<Artifact> _artifacts)
		{
			artifacts = _artifacts;
			//artifacts = ArtifactScraper.ScanArtifacts(ref equippedArtifacts);
			//ArtifactScraper.ScanArtifacts();
		}

		public void AssignWeapons(ref List<Weapon> _weapons)
		{
			weapons = _weapons;
			//weapons = WeaponScraper.ScanWeapons(ref equippedWeapon);
		}
	}
}
