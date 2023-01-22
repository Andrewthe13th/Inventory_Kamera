using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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

        private const string Wiki = "https://genshin-impact.fandom.com";
        private const string CharacterWiki = Wiki + "/wiki/Character/List";
        private const string ArtifactsWiki = Wiki + "/wiki/Artifact/Sets";
        private const string WeaponsWiki = Wiki + "/wiki/Weapon/List";
        private const string DevelopmentItemWiki = Wiki + "/wiki/Character_Development_Item";
        private const string MaterialsWiki = Wiki + "/wiki/Material";
        private const string VersionURL = "https://genshin-impact.fandom.com/wiki/Version";

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
                ? throw new NullReferenceException("Could not get current Genshin version from Wiki.")
                : RemoteVersion.CompareTo(LocalVersion) > 0;
        }

        private Version CheckRemoteVersion()
        {
            var html = FetchHTML(VersionURL);
            var doc = new HtmlDocument();

            doc.LoadHtml(html);

            List<List<string>> table = doc.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable')]")
                                              .Descendants("tr")
                                              .Where(tr => tr.Elements("td").Count() > 1)
                                              .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                                              .ToList();

            Logger.Debug("Parsing wiki version history.");
            foreach (var entry in table)
            {
                if (Version.TryParse(entry[0], out Version version))
                {
                    if (DateTime.TryParse(entry[2], out DateTime date))
                    {
                        if (version == LocalVersion)
                            return version;

                        Logger.Debug("Parsed release date {0} for version {1}", date, version);
                        if (DateTime.Now > date)
                        {
                            Logger.Debug("Version {0} is released!", version);
                            return version;
                        }
                        else
                        {
                            Logger.Debug("Version {0} is announced but not released!", version);
                        }
                    }
                    else Logger.Debug("Could not parse date from {0} for version {1}", entry[2], entry[0]);
                }
                else Logger.Debug("Could not parse version from {0}", entry[0]);
            }
            Logger.Warn("Could not determine latest game version from {0}", VersionURL);
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
                var statusLock = new Object();

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

                if (overallStatus == UpdateStatus.Success)
                {
                    LocalVersion = RemoteVersion;
                    File.WriteAllText(ListsDir + NewVersion, LocalVersion.ToString());
                }
                else if (overallStatus == UpdateStatus.Fail)
                    Logger.Error($"Could not update all information for version {RemoteVersion}");
                else
                    Logger.Info("No update neccessary");

                return overallStatus;
            }
            catch (Exception e)
            {

                Logger.Error(e);
                return UpdateStatus.Fail;
            }
            
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
            var statusLock = new Object();

            var data = JToken.Parse(LoadJsonFromFile(CharactersJson)).ToObject<ConcurrentDictionary<string, JObject>>();

            var html = FetchHTML(CharacterWiki);
            var doc = new HtmlDocument();

            doc.LoadHtml(html);

            var table = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'article-table')]")
                               .Descendants("tr")
                               .Where(tr => tr.Elements("td").Count() > 1)
                               .Select(tr => tr.Elements("td").Where(t => t.InnerText.Trim().Length > 0).ToList());
            
                table.AsParallel().ForAll(entry =>
                {
                    var nameCell = entry[0].FirstChild;

                    var characterName = nameCell.InnerText.Trim();
                    var nameGOOD = ConvertToGOOD(characterName);
                    var nameKey = nameGOOD.ToLower();
                    try
                    {
                        if (data.ContainsKey(nameKey)) return;

                        var characterLink = Wiki + nameCell.Attributes["href"].Value;

                        var constellationOrder = new JArray();
                        var constellationName = new JArray();
                        WeaponType weaponType;
                        var value = new JObject();

                        if (nameKey != "traveler")
                        {
                            var characterHTML = FetchHTML(characterLink);
                            var characterDoc = new HtmlDocument();

                            characterDoc.LoadHtml(characterHTML);

                            var talents = characterDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'talent-table')]")
                                                       .Descendants("tr")
                                                       .Where(tr => tr.Elements("td").Count() == 3)
                                                       .Take(3)
                                                       .ToList();
                            var skill = talents[1].ChildNodes[1].InnerText;
                            var burst = talents[2].ChildNodes[1].InnerText;

                            constellationName = new JArray(characterDoc.DocumentNode.SelectSingleNode("//div[contains(@data-source, 'constellation')]")
                                                                 .Descendants("div")
                                                                 .First().FirstChild.InnerText);


                            var constellationTable = characterDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'talent-table-container')]")
                                                                  .Descendants("tr")
                                                                  .ToList();
                            var constellationDescriptions = constellationTable.Where(tr => tr.Elements("td").Count() == 1)
                                                                  .ToList();

                            if (constellationDescriptions[2].InnerText.Contains(skill))
                            {
                                Logger.Debug($"{characterName} skill is leveled at constellation 3");
                                constellationOrder.Add("skill");
                                constellationOrder.Add("burst");
                            }
                            else if (constellationDescriptions[2].InnerText.Contains(burst))
                            {
                                Logger.Debug($"{characterName} burst is leveled at constellation 3");
                                constellationOrder.Add("burst");
                                constellationOrder.Add("skill");
                            }
                            else
                            {
                                Logger.Debug($"{characterName} doesn't have skill or burst leveled at constellation 3??");
                                constellationOrder.Add("skill");
                                constellationOrder.Add("burst");
                            }

                            var weapon = characterDoc.DocumentNode.SelectSingleNode("//td[contains(@data-source, 'weapon')]").Descendants("a").First().Attributes["title"].Value.ToLower();
                            switch (weapon)
                            {
                                case "sword":
                                    weaponType = WeaponType.Sword;
                                    break;
                                case "claymore":
                                    weaponType = WeaponType.Claymore;
                                    break;
                                case "polearm":
                                    weaponType = WeaponType.Polearm;
                                    break;
                                case "catalyst":
                                    weaponType = WeaponType.Catalyst;
                                    break;
                                case "bow":
                                    weaponType = WeaponType.Bow;
                                    break;
                                default:
                                    throw new IndexOutOfRangeException($"{characterName} uses unknown weapon type {weapon}");
                            }
                            value = new JObject
                        {
                            { "GOOD", nameGOOD },
                            { "ConstellationName", constellationName },
                            { "ConstellationOrder", constellationOrder },
                            { "WeaponType", (int)weaponType }
                        };
                        }
                        else
                        {
                            constellationName = JArray.FromObject(new string[] { "Viator", "Viatrix" });
                            weaponType = WeaponType.Sword;

                            var constellationOrders = new JObject();

                            foreach (var element in elements)
                            {
                                string travelerHTML;
                                var travelerLink = characterLink + $"_({element})";

                                try
                                {
                                    travelerHTML = FetchHTML(travelerLink);
                                }
                                catch { Logger.Debug($"Error trying to fetch {element} page for Traveler. Probably is not in the game yet."); continue; }
                                var travelerDoc = new HtmlDocument();
                                travelerDoc.LoadHtml(travelerHTML);

                                var talents = travelerDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'talent-table')]")
                                                    .Descendants("tr")
                                                    .Where(tr => tr.InnerText.Contains("Normal Attack") || tr.InnerText.Contains("Elemental Skill") || tr.InnerText.Contains("Elemental Burst"))
                                                    .Where(tr => tr.Elements("td").Count() == 3)
                                                    .Take(3)
                                                    .ToList();
                                var skill = talents[1].ChildNodes[1].InnerText;
                                var burst = talents[2].ChildNodes[1].InnerText;


                                var constellationTable = travelerDoc.DocumentNode.SelectSingleNode("//table[contains(@class, 'wikitable talent_table')]")
                                                                        .Descendants("tr")
                                                                        .ToList();
                                var constellationDescriptions = constellationTable.Where(tr => tr.Elements("td").Count() == 1)
                                                                        .ToList();

                                constellationOrder = new JArray();
                                if (constellationDescriptions[2].InnerText.Contains(skill))
                                {
                                    Logger.Debug("{0} {1} skill is leveled at constellation 3", element, characterName);
                                    constellationOrder.Add("skill");
                                    constellationOrder.Add("burst");
                                }
                                else if (constellationDescriptions[2].InnerText.Contains(burst))
                                {
                                    Logger.Debug("{0} {1} burst is leveled at constellation 3", element, characterName);
                                    constellationOrder.Add("burst");
                                    constellationOrder.Add("skill");
                                }

                                constellationOrders.Add(element.ToLower(), constellationOrder);
                            }
                            value = new JObject
                            {
                                { "GOOD", nameGOOD },
                                { "ConstellationName", constellationName },
                                { "ConstellationOrder", constellationOrders },
                                { "WeaponType", (int)weaponType }
                            };
                        }
                        lock (statusLock)
                            if (data.TryAdd(nameKey, value) && status != UpdateStatus.Fail)
                                status = UpdateStatus.Success;
                    }
                    catch (Exception e)
                    {
                        Logger.Error("An error was encountered when gathering information for the character {0}", nameCell); 
                        Logger.Error(e);
                        lock (statusLock) status = UpdateStatus.Fail;
                    }
                });
            

            if (status == UpdateStatus.Success)
                SaveJsonToFile(JsonConvert.SerializeObject(data), CharactersJson);

            return status;
        }

        private UpdateStatus UpdateArtifacts(bool force)
        {
            if (force) File.Delete(ListsDir + ArtifactsJson);
            var status = UpdateStatus.Skipped;
            var statusLock = new Object();


            var data = JToken.Parse(LoadJsonFromFile(ArtifactsJson)).ToObject<ConcurrentDictionary<string, JObject>>();

            var html = FetchHTML(ArtifactsWiki);
            var doc = new HtmlDocument();

            doc.LoadHtml(html);

            var table = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'wikitable')]")
                               .Descendants("tr")
                               .Where(tr => tr.Elements("td").Count() == 4)
                               .Select(tr => tr.Elements("td").ToList()).ToList();

            table.AsParallel().ForAll(entry =>
            {
                var setName = entry[0].InnerText.Trim();
                var setGOOD = ConvertToGOOD(setName);
                var setKey = setGOOD.ToLower();
                try
                {
                    var setLink = Wiki + entry[0].FirstChild.Attributes["href"].Value;
                    var setHTML = FetchHTML(setLink);
                    var setDoc = new HtmlDocument();

                    setDoc.LoadHtml(setHTML);

                    var aside = setDoc.DocumentNode.SelectSingleNode("//aside[contains(@class, 'portable-infobox')]")
                                       .Descendants("div")
                                       .Where(div =>
                                       {
                                           var text = div.InnerText.ToLower().Trim();
                                           return
                                           text.Contains("flower of life") ||
                                           text.Contains("plume of death") ||
                                           text.Contains("sands of eon") ||
                                           text.Contains("goblet of eonothem") ||
                                           text.Contains("circlet of logos");
                                       })
                                       .Where(div =>
                                       {
                                           return div.Attributes["class"].Value.Contains("value");
                                       }
                                       ).ToList();
                    var artifacts = new JObject();
                    foreach (var slotNode in aside)
                    {
                        var slot = slotNode.FirstChild.InnerText.Trim();

                        slot = slots.Where(x => slot.ToLower().Contains(x)).First().Split(' ').First();

                        var name = slotNode.LastChild.InnerText.Trim();
                        var nameGOOD = ConvertToGOOD(name);
                        var nameNormalized = nameGOOD.ToLower();

                        artifacts.Add(slot, new JObject
                    {
                        { "artifactName", name },
                        { "GOOD", nameGOOD },
                        { "normalizedName", nameNormalized }
                    });
                    }

                    var value = new JObject
                {
                    { "setName", setName },
                    { "GOOD", setGOOD },
                    { "normalizedName", setGOOD.ToLower() },
                    { "artifacts", artifacts }
                };

                    if (data.TryAdd(setKey, value))
                        lock (statusLock) status = UpdateStatus.Success;
                }
                catch (Exception e)
                {
                    Logger.Error("An error was encountered when gathering information for the artifact set {0}", setName);
                    Logger.Error(e);
                    lock (statusLock) status = UpdateStatus.Fail;
                }
            });

            if (status == UpdateStatus.Success)
                SaveJsonToFile(JsonConvert.SerializeObject(data), ArtifactsJson);

            return status;
        }

        private UpdateStatus UpdateWeapons(bool force)
        {
            if (force) File.Delete(ListsDir + WeaponsJson);
            var status = UpdateStatus.Skipped;
            var statusLock = new Object();

            var data = JToken.Parse(LoadJsonFromFile(WeaponsJson)).ToObject<ConcurrentDictionary<string, string>>();

            var html = FetchHTML(WeaponsWiki);
            var doc = new HtmlDocument();

            doc.LoadHtml(html);

            var table = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'article-table')]")
                               .Descendants("tr")
                               .Where(tr => tr.Elements("td").Count() > 1)
                               .Select(tr => tr.Elements("td").Where(t => t.InnerText.Trim().Length > 0).ToList()).ToList();

            table.AsParallel().ForAll(entry =>
            {
                var weaponName = entry[0].InnerText.Trim();
                var weaponGOOD = ConvertToGOOD(weaponName);
                var weaponKey = weaponGOOD.ToLower();

                if (data.TryAdd(weaponKey, weaponGOOD))
                    lock (statusLock) status = UpdateStatus.Success;
            });

            if (status == UpdateStatus.Success)
                SaveJsonToFile(JsonConvert.SerializeObject(data), WeaponsJson);

            return status;
        }

        private UpdateStatus UpdateMaterials(bool force)
        {
            if (force) File.Delete(ListsDir + MaterialsJson);
            var status = UpdateStatus.Skipped;

            var data = JToken.Parse(LoadJsonFromFile(MaterialsJson)).ToObject<Dictionary<string, string>>();

            var xpBooks = new List<string> { "Hero's Wit", "Adventurer's Experience", "Wanderer's Advice" };
            foreach (var xpBook in xpBooks)
            {
                var itemGOOD = ConvertToGOOD(xpBook);
                var itemKey = itemGOOD.ToLower();

                if (!data.ContainsKey(itemKey))
                {
                    data.Add(itemKey, itemGOOD);
                    status = UpdateStatus.Success;
                }
            }

            var wikis = new List<string> { DevelopmentItemWiki, MaterialsWiki };


            foreach (var wiki in wikis)
            {
                var html = FetchHTML(wiki);
                var doc = new HtmlDocument();

                doc.LoadHtml(html);

                var items = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'hlist')]")
                                .SelectNodes("//span[contains(@class, 'card-image')]")
                                .Descendants("a")
                                .Select(i => i.Attributes["title"].Value.Trim()).ToList();

                foreach (var item in items)
                {
                    var itemGOOD = ConvertToGOOD(item);
                    var itemKey = itemGOOD.ToLower();
                    if (!data.ContainsKey(itemKey))
                    {
                        data.Add(itemKey, itemGOOD);
                        status = UpdateStatus.Success;
                    }
                }
            }

            if (status == UpdateStatus.Success)
                SaveJsonToFile(JsonConvert.SerializeObject(data), MaterialsJson);

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
