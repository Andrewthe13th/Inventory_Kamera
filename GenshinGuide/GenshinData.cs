using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace GenshinGuide
{
    public class GenshinData
    {
        [JsonProperty]
        private static List<Character> characters = new List<Character>();
        [JsonProperty]
        private static Inventory inventory = new Inventory();

        private static List<Artifact> equippedArtifacts = new List<Artifact>();
        private static List<Weapon> equippedWeapons = new List<Weapon>();
        public static Queue<OCRImage> workerQueue = new Queue<OCRImage>();
        private static volatile bool b_ScanWeapons = false;
        private static Thread ImageProcessor = new Thread(() => { ImageProcessorWorker(); });
        // TODO add language option

        public GenshinData()
        {
            characters = new List<Character>();
            inventory = new Inventory();
            equippedArtifacts = new List<Artifact>();
            equippedWeapons = new List<Weapon>();
        }

        public void StopImageProcessorWorker()
        {
            if (ImageProcessor.IsAlive)
            {
                ImageProcessor.Abort();
            }
        }

        public List<Character> GetCharacters()
        {
            return characters;
        }

        public Inventory GetInventory()
        {
            return inventory;
        }

        public void GatherData()
        {
            // Initize Image Processor Queue
            ImageProcessor.IsBackground = true;
            ImageProcessor.Start();

            // Get Weapons
            Navigation.InventoryScreen();
            Navigation.SelectWeaponInventory();
            WeaponScraper.ScanWeapons();
            //inventory.AssignWeapons(ref equippedWeapons);
            Navigation.MainMenuScreen();


            // Get Artifacts
            Navigation.InventoryScreen();
            Navigation.SelectArtifactInventory();
            ArtifactScraper.ScanArtifacts();
            //inventory.AssignArtifacts(ref equippedArtifacts);
            Navigation.MainMenuScreen();
            workerQueue.Enqueue(new OCRImage(null, "END", 0));

            // Wait till weapons have been scanned
            // Used to Get name of traveler
            while (b_ScanWeapons == false)
            {
                System.Threading.Thread.Sleep(1000);
            }

            // Get characters
            Navigation.CharacterScreen();
            characters = new List<Character>();
            characters = CharacterScraper.ScanCharacters();
            Navigation.MainMenuScreen();

            // Wait for Image Processor to finish
            ImageProcessor.Join();

            // Assign Artifacts to Characters
            AssignArtifacts();
            AssignWeapons();

            //Console.ReadKey();
        }

        public static void ImageProcessorWorker()
        {
            Weapon w; Artifact a;
            int weaponCount = 0;
            int artifactCount = 0;
            //List<Weapon> weapons = new List<Weapon>();
            //List<Artifact> artifacts = new List<Artifact>();
            bool b_End = false;
            while (!b_End)
            {
                if (workerQueue.Count > 0)
                {
                    OCRImage img = workerQueue.Dequeue();
                    if (img.type != "END" && img.bm != null)
                    {
                        if (img.type == "weapon")
                        {
                            weaponCount++;
                            // Scan as weapon
                            UserInterface.Reset_Artifact();
                            w = WeaponScraper.ScanWeapon(img.bm, img.id);
                            UserInterface.IncrementWeaponCount();
                            inventory.AssignWeapon(w);

                            if (w.equippedCharacter != 0)
                            {
                                equippedWeapons.Add(w);
                            }
                        }
                        else if (img.type == "artifact")
                        {
                            // Notify weapon has finished
                            if (!b_ScanWeapons)
                                b_ScanWeapons = true;

                            artifactCount++;
                            // Scan as weapon
                            UserInterface.Reset_Artifact();
                            a = ArtifactScraper.ScanArtifact(img.bm, img.id);
                            UserInterface.IncrementArtifactCount();
                            inventory.AssignArtifact(a);

                            if (a.equippedCharacter != 0)
                            {
                                equippedArtifacts.Add(a);
                            }

                        }
                        else // not supposed to happen
                        {
                            Form1.UnexpectedError("Unknown Image type for Image Processor");
                        }
                    }
                    else
                    {
                        workerQueue.Clear();
                        b_End = true;
                    }
                    img.bm = null; img.type = "";
                }
                else
                { // Wait for more images to process
                    System.Threading.Thread.Sleep(1000);
                }
            }

            // Assign weapons and artifacts to inventory
            //inventory.AssignArtifacts(ref artifacts);
            //inventory.AssignWeapons(ref weapons);
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

        

    }
}
