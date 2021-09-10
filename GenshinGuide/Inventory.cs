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

        public void AssignArtifacts(ref List<Artifact> equippedArtifacts)
        {
            artifacts = ArtifactScraper.ScanArtifacts(ref equippedArtifacts);
            //ArtifactScraper.ScanArtifacts();
        }

        public void AssignWeapons(ref List<Weapon> equippedWeapon)
        {
            weapons = WeaponScraper.ScanWeapons(ref equippedWeapon);
        }
    }
}
