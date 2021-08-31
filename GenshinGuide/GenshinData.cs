using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GenshinGuide
{
    public class GenshinData
    {
        [JsonProperty]
        private List<Character> characters = new List<Character>();
        [JsonProperty]
        private Inventory inventory = new Inventory();

        private List<Artifact> equippedArtifacts = new List<Artifact>();
        private List<Weapon> equippedWeapons = new List<Weapon>();
        // TODO add language option

        public GenshinData()
        {
            characters = new List<Character>();
            inventory = new Inventory();
            equippedArtifacts = new List<Artifact>();
        }

        public void GatherData()
        {
            // Get Traveler Name
            AssignTravelerName();

            // Get characters
            //Navigation.CharacterScreen();
            //characters = CharacterScraper.ScanCharacters();
            //Navigation.MainMenuScreen();

            // Get Weapons
            Navigation.InventoryScreen();
            Navigation.SelectWeaponInventory();
            inventory.GetWeapons(ref equippedWeapons);
            Navigation.MainMenuScreen();

            // Get Artifacts
            //Navigation.InventoryScreen();
            //Navigation.SelectArtifactInventory();
            //inventory.GetArtifacts(ref equippedArtifacts);
            //Navigation.MainMenuScreen();

            // Assign Artifacts to Characters
            //AssignArtifacts();
            //AssignWeapons();
        }

        public void AssignArtifacts()
        {
            foreach(Artifact a in equippedArtifacts)
            {
                foreach(Character c in characters)
                {
                    if(a.GetEquippedCharacter() == c.GetName())
                    {
                        c.AssignArtifact(a);
                    }
                }
            }
        }

        public void AssignWeapons()
        {
            foreach (Weapon w in equippedWeapons)
            {
                foreach (Character c in characters)
                {
                    if (w.GetEquippedCharacter() == c.GetName())
                    {
                        c.AssignWeapon(w);
                    }
                }
            }
        }

        public void AssignTravelerName()
        {
            string traveler = CharacterScraper.ScanMainCharacterName();
            if(traveler != "")
            {
                Scraper.AddTravelerToCharacterList(traveler);
            }
            else
            {
                System.Environment.Exit(1);
            }
        }

    }
}
