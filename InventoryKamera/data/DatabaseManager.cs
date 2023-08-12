using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;


namespace InventoryKamera
{
    public class DatabaseManager
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _listdir = @".\inventorylists\";

        public string ListsDir
        {
            get
            {
                Directory.CreateDirectory(_listdir);
                return _listdir;
            }
            set { _listdir = value; }
        }

        private const string WeaponsJson = "weapons.json";
        private const string ArtifactsJson = "artifacts.json";
        private const string CharactersJson = "characters.json";
        private const string MaterialsJson = "materials.json";

        private readonly string NewVersion = "version.txt";

        private const string commitsAPIURL = "https://gitlab.com/api/v4/projects/41287973/repository/commits";
        private const string repoBaseURL = "https://gitlab.com/Dimbreath/AnimeGameData/-/raw/master/";
        private const string TextMapEnURL = repoBaseURL + "TextMap/TextMapEN.json";
        private const string CharactersURL = repoBaseURL + "ExcelBinOutput/AvatarExcelConfigData.json";
        private const string ConstellationsURL = repoBaseURL + "ExcelBinOutput/FetterInfoExcelConfigData.json";
        private const string TalentsURL = repoBaseURL + "ExcelBinOutput/AvatarTalentExcelConfigData.json";
        private const string SkillsURL = repoBaseURL + "ExcelBinOutput/AvatarSkillExcelConfigData.json";

        private const string ArtifactsDisplayItemURL = repoBaseURL + "ExcelBinOutput/DisplayItemExcelConfigData.json";
        private const string ArtifactsCodex = repoBaseURL + "ExcelBinOutput/ReliquaryCodexExcelConfigData.json";
        private const string ArtifactSetConfig = repoBaseURL + "ExcelBinOutput/ReliquaryExcelConfigData.json";

        private const string WeaponsURL = repoBaseURL + "ExcelBinOutput/WeaponExcelConfigData.json";
        private const string MaterialsURL = repoBaseURL + "ExcelBinOutput/MaterialExcelConfigData.json";

        private ConcurrentDictionary<string, string> Mappings = new ConcurrentDictionary<string, string>();

        internal Version LocalVersion = new Version();
        internal Version RemoteVersion = new Version();

        private static readonly List<string> elements = new List<string>
        {
            "Pyro",
            "Hydro",
            "Dendro",
            "Electro",
            "Anemo",
            "Cryo",
            "Geo",
        };

        private static readonly List<string> slots = new List<string>
        {
            "flower of life",
            "plume of death",
            "sands of eon",
            "goblet of eonothem",
            "circlet of logos",
        };

        public DatabaseManager()
        {
            Directory.CreateDirectory(ListsDir);


            if (!File.Exists(ListsDir + NewVersion)) File.WriteAllText(ListsDir + NewVersion, new Version().ToString());


            LocalVersion = new Version(File.ReadAllText(ListsDir + NewVersion));
        }

        public bool UpdateAvailable()
        {
            Logger.Info("Checking for newer game data...");
            RemoteVersion = CheckRemoteVersion();
            return RemoteVersion == null
                ? throw new NullReferenceException("Could not get current Genshin version.")
                : RemoteVersion.CompareTo(LocalVersion) > 0;
        }

        private Version CheckRemoteVersion()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(commitsAPIURL)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add( new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json") );


            using (var response = client.GetAsync(commitsAPIURL))
            {
                string pattern = @"(\d+\.\d+\.\d+)";

                string body = response.Result.Content.ReadAsStringAsync().Result;
                var commits = JsonConvert.DeserializeObject<List<JObject>>(body);
                foreach (var commit in commits)
                {
                    var c = commit.ToObject<Dictionary<string, object>>();
                    var match = Regex.Match(c["title"].ToString(), pattern);
                    if (match.Success)
                    {
                        var v = match.Groups[1].Value;
                        Debug.WriteLine("Latest version found {0}", v);
                        return new Version(v);
                    }
                }
            }
            return null;
        }

        public Dictionary<string, JObject> LoadCharacters()
        {
            return GetList(ListType.Characters).ToObject<Dictionary<string, JObject>>();
        }

        public Dictionary<string, string> LoadWeapons()
        {
            return GetList(ListType.Weapons).ToObject<Dictionary<string, string>>();
        }

        public Dictionary<string, JObject> LoadArtifacts()
        {
            return GetList(ListType.Artifacts).ToObject<Dictionary<string, JObject>>();
        }

        public Dictionary<string, string> LoadMaterials()
        {
            return GetList(ListType.Materials).ToObject<Dictionary<string, string>>();
        }

        public Dictionary<string, string> LoadDevItems()
        {
            return GetList(ListType.CharacterDevelopmentItems).ToObject<Dictionary<string, string>>();
        }

        private JToken GetList(ListType list)
        {
            string file = "";
            switch (list)
            {
                case ListType.Weapons:
                    file = WeaponsJson;
                    break;

                case ListType.Artifacts:
                    file = ArtifactsJson;
                    break;

                case ListType.Characters:
                    file = CharactersJson;
                    break;

                case ListType.CharacterDevelopmentItems:
                case ListType.Materials:
                    file = MaterialsJson;
                    break;

                default:
                    break;
            }

            if (!File.Exists(ListsDir + file)) throw new FileNotFoundException($"Data file does not exist for {list}.");
            string json = LoadJsonFromFile(file);
            return json == "{}"
                ? throw new FormatException($"Data file for {list} is invalid. Please try running the auto updater and try again.")
                : JToken.Parse(json);
        }

        public UpdateStatus UpdateGameData(bool force = false)
        {
            try
            {
                UpdateStatus overallStatus = UpdateStatus.Success;
                var statusLock = new object();
                LoadMappings();

                if (force)
                {
                    Logger.Info("Forcing update for game data");
                    Properties.Settings.Default.LastUpdateCheck = DateTime.MinValue;
                    RemoteVersion = CheckRemoteVersion();
                }

                var lists = Enum.GetValues(typeof(ListType)).Cast<ListType>().ToList();

                lists.RemoveAll(e => e == ListType.CharacterDevelopmentItems);

                lists.AsParallel().ForAll(e =>
                {
                    var status = UpdateList(e, force);
                    if (status == UpdateStatus.Fail)
                    {
                        lock (statusLock) overallStatus = UpdateStatus.Fail;
                        Logger.Error("Failed to update {0} data", e);
                    }
                    else if (overallStatus != UpdateStatus.Fail)
                    {
                        lock (statusLock) overallStatus = status;
                    }
                });

                if (overallStatus != UpdateStatus.Fail)
                {

                    if (RemoteVersion != new Version())
                    {
                        LocalVersion = RemoteVersion;
                        File.WriteAllText(ListsDir + NewVersion, LocalVersion.ToString());
                    }
                }
                else
                    Logger.Error($"Could not update all information for version {RemoteVersion}");

                ReleaseMappings();
                return overallStatus;
            }
            catch (Exception e)
            {
                ReleaseMappings();
                Logger.Error(e);
                return UpdateStatus.Fail;
            }
            
        }

        private void LoadMappings()
        {
            lock (Mappings)
            {
                if (!Mappings.Any())
                {
                    Mappings = new ConcurrentDictionary<string, string>(JObject.Parse(LoadJsonFromURLAsync(TextMapEnURL))
                        .ToObject<Dictionary<string, string>>()
                        .Where(e => !string.IsNullOrWhiteSpace(e.Value)) // Remove any mapping with empty
                        .ToDictionary(i => i.Key, i => i.Value));
                }
            }
        }

        private void ReleaseMappings()
        {
            Mappings.Clear();
        }

        private string LoadJsonFromURLAsync(string url)
        {
            string json = "";
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                json = client.DownloadString(url);
            }
            return json;
        }

        private UpdateStatus UpdateList(ListType list, bool force = false)
        {
            UpdateStatus status = UpdateStatus.Success;

            Logger.Info("Updating {0}", list);

            try
            {
                switch (list)
                {
                    case ListType.Weapons:
                        status = UpdateWeapons(force);
                        break;

                    case ListType.Artifacts:
                        status = UpdateArtifacts(force);
                        break;

                    case ListType.Characters:
                        status = UpdateCharacters(force);
                        break;

                    case ListType.Materials:
                        status = UpdateMaterials(force);
                        break;

                    default:
                        break;
                }
                Logger.Info("Finished updating {0} ({1})", list, status);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to update {0} data", list);
                Logger.Error(e);
                return UpdateStatus.Fail;
            }

            return status;
        }

        private UpdateStatus UpdateCharacters(bool force)
        {
            if (force) File.Delete(ListsDir + CharactersJson);

            var status = UpdateStatus.Skipped;

            var data = JToken.Parse(LoadJsonFromFile(CharactersJson)).ToObject<ConcurrentDictionary<string, JObject>>();

            try
            {
                var characters = JArray.Parse(LoadJsonFromURLAsync(CharactersURL)).ToObject<List<JObject>>();
                var constellations = JArray.Parse(LoadJsonFromURLAsync(ConstellationsURL)).ToObject<List<JObject>>();
                var talents = JArray.Parse(LoadJsonFromURLAsync(TalentsURL)).ToObject<List<JObject>>();
                var skills = JArray.Parse(LoadJsonFromURLAsync(SkillsURL)).ToObject<List<JObject>>();

                characters.RemoveAll(c => !c.ContainsKey("useType") || c["useType"].ToString() != "AVATAR_FORMAL");

                Logger.Debug("Found {0} playable characters available", characters.Count);
                characters.AsParallel().ForAll(character =>
                {
                    string name = Mappings[character["nameTextMapHash"].ToString()].ToString();

                    try
                    {

                        if (name.ToLower() == "PlayerGirl".ToLower()) return; // Both travelers are listed but are essentially identical

                        string PascalCase = CultureInfo.GetCultureInfo("en").TextInfo.ToTitleCase(name);
                        string nameGOOD = Regex.Replace(PascalCase, @"[\W]", string.Empty);
                        string nameKey = nameGOOD.ToLower();
                        int characterID = (int)character["id"];

                        string const3Description = "";
                        string skill = "";

                        var value = new JObject();

                        if (!data.ContainsKey(nameKey))
                        {
                            value.Add("GOOD", nameGOOD);

                            // Some characters have different internal names.
                            // Ex: Jean -> Qin, Yanfei -> Feiyan, etc.
                            name = character["iconName"].ToString().Split('_').Last(); // UI_AvatarIcon_[Qin] -> Qin

                            if (name.ToLower() == "PlayerBoy".ToLower()) // Handle traveler elements separately
                            {
                                var constellationNames = new JArray { "Viator", "Viatrix" };
                                var constellationOrder = new JObject();

                                var playerElements = new Dictionary<string, string>
                                {
                                    { "Electric", "electro" },
                                    { "Fire", "pyro" },
                                    { "Grass", "dendro" },
                                    { "Rock", "geo"},
                                    { "Water", "hydro"},
                                    { "Wind", "anemo"}
                                };

                                value.Add("Element", new JArray(playerElements.Values));

                                foreach (var element in playerElements)
                                {
                                    var elementSkill = skills.FirstOrDefault(entry => entry["skillIcon"].ToString().Contains($"Player{element.Key}") && !entry.ContainsKey("costElemType"));
                                    

                                    if (elementSkill == null) continue;

                                    skill = Mappings[elementSkill["nameTextMapHash"].ToString()].ToString();

                                    const3Description = talents.Where(entry => entry["openConfig"].ToString().Contains($"Player_{element.Key}")).ElementAt(2)["descTextMapHash"].ToString();
                                    const3Description = Mappings[const3Description].ToString();

                                    if (const3Description.Contains(skill))
                                    {
                                        constellationOrder.Add(element.Value, new JArray { "skill", "burst" });
                                    }
                                    else
                                    {
                                        constellationOrder.Add(element.Value, new JArray { "burst", "skill" });
                                    }
                                }
                                value.Add("ConstellationOrder", constellationOrder);
                            }
                            else // Any other character that isn't traveler
                            {
                                skill = skills.First(entry => entry["skillIcon"].ToString().Contains($"Skill_S_{name}"))["nameTextMapHash"].ToString();
                                skill = Mappings[skill].ToString();


                                value.Add("ConstellationName", new JArray
                                {
                                    GetConstellationNameFromId(characterID)
                                });

                                var constellationOrder = new JArray();

                                // The skill/burst name is always mentioned in the constellation's description so we'll check for it
                                const3Description = talents.Where(entry => entry["icon"].ToString().Contains(name)).ElementAt(2)["descTextMapHash"].ToString();
                                const3Description = Mappings[const3Description].ToString();

                                if (const3Description.Contains(skill))
                                {
                                    constellationOrder.Add("skill");
                                    constellationOrder.Add("burst");
                                }
                                else
                                {
                                    constellationOrder.Add("burst");
                                    constellationOrder.Add("skill");
                                }

                                value.Add("ConstellationOrder", constellationOrder);

                                value.Add("Element", new JArray(GetElementFromID(characterID)));
                            }

                            var weaponArchetype = character["weaponType"].ToString();
                            WeaponType weaponType;
                            if (weaponArchetype.Contains("SWORD_ONE_HAND")) weaponType = WeaponType.Sword;
                            else if (weaponArchetype.Contains("CLAYMORE")) weaponType = WeaponType.Claymore;
                            else if (weaponArchetype.Contains("POLE")) weaponType = WeaponType.Polearm;
                            else if (weaponArchetype.Contains("BOW")) weaponType = WeaponType.Bow;
                            else if (weaponArchetype.Contains("CATALYST")) weaponType = WeaponType.Catalyst;
                            else throw new IndexOutOfRangeException($"{name} uses unknown weapon type {weaponArchetype}");

                            value.Add("WeaponType", (int)weaponType);

                            if (data.TryAdd(nameKey, value)) status = UpdateStatus.Success;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn("Problem when generating character information for {0}", name);
                        Logger.Warn(ex);
                    }
                });

                string GetConstellationNameFromId(int targetID)
                {
                    foreach (var constellation in from constellation in constellations
                                                  where (((int)constellation["avatarId"]) == targetID)
                                                  select constellation)
                    {
                        return Mappings[constellation["avatarConstellationBeforTextMapHash"].ToString()].ToString();
                    }

                    return "";
                }

                string GetElementFromID(int targetID)
                {
                    foreach (var element in from constellation in constellations
                                            where (((int)constellation["avatarId"]) == targetID)
                                            select constellation)
                    {
                        return Mappings[element["avatarVisionBeforTextMapHash"].ToString()].ToString().ToLower();
                    }
                    return "";
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
                status = UpdateStatus.Fail;
            }            

            if (status == UpdateStatus.Success)
                SaveJsonToFile(JsonConvert.SerializeObject(new SortedDictionary<string, JObject>(data)), CharactersJson);

            return status;
        }

        private UpdateStatus UpdateArtifacts(bool force)
        {
            if (force) File.Delete(ListsDir + ArtifactsJson);

            var status = UpdateStatus.Skipped;

            var data = JToken.Parse(LoadJsonFromFile(ArtifactsJson)).ToObject<ConcurrentDictionary<string, JObject>>();

            var artifactDisplays = JArray.Parse(LoadJsonFromURLAsync(ArtifactsDisplayItemURL)).ToObject<List<JObject>>();
            artifactDisplays.RemoveAll(a => a.TryGetValue("icon", out var icon) && !icon.ToString().Contains("RelicIcon"));

            var codex = JArray.Parse(LoadJsonFromURLAsync(ArtifactsCodex)).ToObject<List<JObject>>();
            var sets = JArray.Parse(LoadJsonFromURLAsync(ArtifactSetConfig)).ToObject<List<JObject>>();

            artifactDisplays.AsParallel().ForAll(artifactDisplay =>
            {
                if (Mappings.ContainsKey(artifactDisplay["nameTextMapHash"].ToString()))
                {
                    string setName = Mappings[artifactDisplay["nameTextMapHash"].ToString()];                  // Archaic Petra
                    try
                    {
                        var setNamePascalCase = CultureInfo.GetCultureInfo("en").TextInfo.ToTitleCase(setName); // Archaic Petra
                        var setNameGOOD = Regex.Replace(setNamePascalCase, @"[\W]", string.Empty);              // ArchaicPetra
                        var setNameKey = setNameGOOD.ToLower();                                                 // archaicpetra
                        var setNameNormalized = setName;
                        var setID = (int)artifactDisplay["param"];

                        if (!data.ContainsKey(setNameKey))
                        {
                            foreach (var set in codex)
                            {
                                var artifacts = new JObject();

                                if ((int)set["suitId"] == setID)
                                {
                                    var artifactIDs = new Dictionary<string, JToken>()
                                    {
                                        { "flower", set["flowerId"]},
                                        { "plume", set["leatherId"] },
                                        { "sands" , set["sandId"] },
                                        { "goblet", set["cupId"] },
                                        { "circlet", set["capId"] },
                                    };
                                    
                                    foreach (var artifact in sets.Where(s => artifactIDs.Values.Contains((int)s["id"])))
                                    {
                                        var slot = artifactIDs.First(x => x.Value != null && (int)x.Value == (int)artifact["id"]).Key;
                                        var artifactName = Mappings[artifact["nameTextMapHash"].ToString()];                                  // Goblet of the Sojourner
                                        string artifactNamePascalCase = CultureInfo.GetCultureInfo("en").TextInfo.ToTitleCase(artifactName);  // Goblet Of The Sojourner
                                        string artifactNameGOOD = Regex.Replace(artifactNamePascalCase, @"[\W]", string.Empty);               // GobletOfTheSojourner
                                        string artifactNormalized = artifactNameGOOD.ToLower();                                               // gobletofthesojourner

                                        artifacts.Add(slot, new JObject
                                        {
                                            { "artifactName", artifactName },
                                            { "GOOD", artifactNameGOOD },
                                            { "normalizedName", artifactNormalized },
                                        });
                                    }
                                }

                                if (artifacts.Count < 1) continue;
                                var value = new JObject
                                {
                                    { "setName", setName },
                                    { "GOOD", setNameGOOD },
                                    { "normalizedName", setNameGOOD.ToLower() },
                                    { "artifacts", artifacts }
                                };
                                if (data.TryAdd(setNameKey, value) && status != UpdateStatus.Fail) status = UpdateStatus.Success;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("An error was encountered when gathering information for the artifact set {0}", setName);
                        Logger.Error(ex);
                        status = UpdateStatus.Fail;
                    }
                }
            });

            if (status == UpdateStatus.Success)
                SaveJsonToFile(JsonConvert.SerializeObject(new SortedDictionary<string, JObject>(data)), ArtifactsJson);

            return status;
        }

        private UpdateStatus UpdateWeapons(bool force)
        {
            if (force) File.Delete(ListsDir + WeaponsJson);

            var status = UpdateStatus.Skipped;

            var data = JToken.Parse(LoadJsonFromFile(WeaponsJson)).ToObject<ConcurrentDictionary<string, string>>();

            try
            {
                List<JObject> weapons = JArray.Parse(LoadJsonFromURLAsync(WeaponsURL)).ToObject<List<JObject>>();

                weapons.AsParallel().ForAll(weapon =>
                {
                    try
                    {
                        if (Mappings.ContainsKey(weapon["nameTextMapHash"].ToString()))
                        {
                            var name = Mappings[weapon["nameTextMapHash"].ToString()];
                            string PascalCase = CultureInfo.GetCultureInfo("en").TextInfo.ToTitleCase(name); // Dull Blade
                            string nameGOOD = Regex.Replace(PascalCase, @"[\W]", string.Empty);              // DullBlade
                            string nameKey = nameGOOD.ToLower();                                             // dullblade

                            if (!data.TryAdd(nameKey, nameGOOD)) status = UpdateStatus.Success;
                        }
                        else Logger.Warn("Weapon hash {0} not found in Mappings. It's likely unreleased.", weapon["nameTextMapHash"].ToString());
                    }
                    catch (Exception ex) { Logger.Warn(ex, weapon["nameTextMapHash"].ToString()); }
                });
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
                status = UpdateStatus.Fail;
            }

            if (status == UpdateStatus.Success)
                SaveJsonToFile(JsonConvert.SerializeObject(new SortedDictionary<string, string>(data)), WeaponsJson);

            return status;
        }

        private UpdateStatus UpdateMaterials(bool force)
        {
            if (force) File.Delete(ListsDir + MaterialsJson);

            var status = UpdateStatus.Skipped;

            var materialCategories = new List<string>
            {
                "MATERIAL_EXP_FRUIT",
                "MATERIAL_AVATAR_MATERIAL",
                "MATERIAL_EXCHANGE",
                "MATERIAL_WOOD",
                "MATERIAL_FISH_BAIT",
                "MATERIAL_WEAPON_EXP_STONE",
            };

            var data = JToken.Parse(LoadJsonFromFile(MaterialsJson)).ToObject<ConcurrentDictionary<string, string>>();

            var materials = JArray.Parse(LoadJsonFromURLAsync(MaterialsURL)).ToObject<List<JObject>>();
            materials.RemoveAll(material => !(material.TryGetValue("materialType", out var materialType) && materialCategories.Contains(materialType.ToString())));

            materials.AsParallel().ForAll(material =>
            {
                try
                {
                    JToken jToken = material["nameTextMapHash"];
                    if (Mappings.TryGetValue(jToken.ToString(), out var name))
                    {
                        var PascalCase = CultureInfo.GetCultureInfo("en").TextInfo.ToTitleCase(name);
                        var nameGood = Regex.Replace(PascalCase, @"[\W]", string.Empty);
                        var nameKey = nameGood.ToLower();

                        if (data.TryAdd(nameKey, nameGood) && status != UpdateStatus.Fail) status = UpdateStatus.Success;
                    }
                    else
                    {
                        Logger.Warn("Material hash {0} not found in Mappings.", material["nameTextMapHash"].ToString());
                    }
                }
                catch (Exception ex) { Logger.Warn(ex); }
            });

            if (status == UpdateStatus.Success)
                SaveJsonToFile(JsonConvert.SerializeObject(new SortedDictionary<string, string>(data)), MaterialsJson);

            return status;
        }

        private string FetchHTML(string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    return client.DownloadString(url);
                }
            }
            catch { throw; }

        }

        private string LoadJsonFromFile(string fileName)
        {
            lock (this)
            {
                try
                {
                    using (StreamReader file = File.OpenText(ListsDir + fileName))
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        return JToken.ReadFrom(reader).ToString();
                    }
                }
                catch (Exception)
                {
                    File.Create(ListsDir + fileName).Close();
                    return "{}";
                }
            }
        }

        private bool SaveJsonToFile(string json, string fileName)
        {
            lock (this)
            {
                try
                {
                    using (StreamWriter file = new StreamWriter(ListsDir + fileName))
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        writer.Formatting = Formatting.Indented;
                        JToken.Parse(json).WriteTo(writer);
                    }
                    return true;
                }
                catch (Exception ex) { Logger.Warn(ex); return false; }
            }
        }

        private string ConvertToGOOD(string text)
        {
            foreach (Match match in Regex.Matches(text, @"&#\d*;"))
            {
                if (int.TryParse(Regex.Replace(match.Value, @"\D", string.Empty), out int dec))
                    text = text.Replace(match.Value, Char.ToString(Convert.ToChar(dec)));

                else
                    Logger.Warn("Failed to parse decimal value {0} from string {1}", match.Value, text);
            }
            var pascal = CultureInfo.GetCultureInfo("en-US").TextInfo.ToTitleCase(text);
            return Regex.Replace(pascal, @"[\W]", string.Empty);
        }
    }

    public enum ListType
    {
        Characters,
        Weapons,
        Artifacts,
        CharacterDevelopmentItems,
        Materials,
    }

    public enum UpdateStatus
    {
        Fail,
        Skipped,
        Success,
    }
}
