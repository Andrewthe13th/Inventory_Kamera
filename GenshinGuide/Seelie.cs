using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GenshinGuide
{
    class Seelie
    {
        [JsonProperty] private List<SeelieItem> inventory; // char dev item list
        private string[] charDevItems = {
            // character exp materials
            "xp|xp|0", 
            "xp|xp_sub_1|0", 
            "xp|xp_sub_0|0", 

            // character level-up materials
                // enemies
            "common|slime|2",
            "common|slime|1",
            "common|slime|0",
            "common|mask|2",
            "common|mask|1",
            "common|mask|0",
            "common|scroll|2",
            "common|scroll|1",
            "common|scroll|0",
            "common|arrowhead|2",
            "common|arrowhead|1",
            "common|arrowhead|0",
            "common_rare|horn|2",
            "common_rare|horn|1",
            "common_rare|horn|0",
            "common_rare|ley_line|2",
            "common_rare|ley_line|1",
            "common_rare|ley_line|0",
            "common_rare|chaos|2",
            "common_rare|chaos|1",
            "common_rare|chaos|0",
            "common_rare|mist|2",
            "common_rare|mist|1",
            "common_rare|mist|0",
            "common_rare|sacrificial_knife|2",
            "common_rare|sacrificial_knife|1",
            "common_rare|sacrificial_knife|0",
            "common|f_insignia|2",
            "common|f_insignia|1",
            "common|f_insignia|0",
            "common|th_insignia|2",
            "common|th_insignia|1",
            "common|th_insignia|0",
            "common|nectar|2",
            "common|nectar|1",
            "common|nectar|0",
            "common_rare|bone_shard|2",
            "common_rare|bone_shard|1",
            "common_rare|bone_shard|0",
            "common|handguard|2",
            "common|handguard|1",
            "common|handguard|0",
            "common_rare|chaos_g|2",
            "common_rare|chaos_g|1",
            "common_rare|chaos_g|0",
            "common_rare|prism|2",
            "common_rare|prism|1",
            "common_rare|prism|0",
            "common|spectral_husk|2",
            "common|spectral_husk|1",
            "common|spectral_husk|0",
            "common_rare|claw|2",
            "common_rare|claw|1",
            "common_rare|claw|0",

            // Weekly Boss Items
            "boss|dvalins_plume|0",
            "boss|dvalins_claw|0",
            "boss|dvalins_sigh|0",
            "boss|tail_of_boreas|0",
            "boss|ring_of_boreas|0",
            "boss|spirit_locket_of_boreas|0",
            "boss|tusk_of_monoceros_caeli|0",
            "boss|shard_of_a_foul_legacy|0",
            "boss|shadow_of_the_warrior|0",
            "boss|dragon_lords_crown|0",
            "boss|bloodjade_branch|0",
            "boss|gilded_scale|0",
            "boss|signora_flower|0",
            "boss|signora_wings|0",
            "boss|signora_heart|0", 

                // World Boss
            "element_2|everflame_seed|0", 
            "element_2|cleansing_heart|0",
            "element_2|lightning_prism|0",
            "element_2|hoarfrost_core|0",
            "element_2|hurricane_seed|0",
            "element_2|basalt_pillar|0",
            "element_2|juvenile_jade|0",
            "element_2|crystalline_bloom|0",
            "",
            "element_2|maguu_kishin|0",
            "element_2|perpetual_heart|0",
            "element_2|smoldering_pearl|0",
            "element_2|dew_of_repudiation|0",
            "element_2|storm_beads|0",
            "element_2|riftborn_regalia|0", 


            // gemstones
            "element_1|agnidus_agate|3", 
            "element_1|agnidus_agate|2", 
            "element_1|agnidus_agate|1", 
            "element_1|agnidus_agate|0", 
             
            "element_1|varunada_lazurite|3", 
            "element_1|varunada_lazurite|2", 
            "element_1|varunada_lazurite|1", 
            "element_1|varunada_lazurite|0", 
             
            "element_1|vajrada_amethyst|3", 
            "element_1|vajrada_amethyst|2", 
            "element_1|vajrada_amethyst|1", 
            "element_1|vajrada_amethyst|0", 
             
            "element_1|vayuda_turqoise|3", 
            "element_1|vayuda_turqoise|2", 
            "element_1|vayuda_turqoise|1", 
            "element_1|vayuda_turqoise|0", 
             
            "element_1|shivada_jade|3", 
            "element_1|shivada_jade|2", 
            "element_1|shivada_jade|1", 
            "element_1|shivada_jade|0", 
             
            "element_1|prithiva_topaz|3", 
            "element_1|prithiva_topaz|2", 
            "element_1|prithiva_topaz|1",
            "element_1|prithiva_topaz|0", 

            // talent level-up materials
            "talent|freedom|2",
            "talent|freedom|1",
            "talent|freedom|0",
            "talent|resistance|2",
            "talent|resistance|1",
            "talent|resistance|0",
            "talent|ballad|2",
            "talent|ballad|1",
            "talent|ballad|0",
            "talent|prosperity|2",
            "talent|prosperity|1",
            "talent|prosperity|0",
            "talent|diligence|2",
            "talent|diligence|1",
            "talent|diligence|0",
            "talent|gold|2",
            "talent|gold|1",
            "talent|gold|0",
            "talent|transience|2",
            "talent|transience|1",
            "talent|transience|0",
            "talent|elegance|2",
            "talent|elegance|1",
            "talent|elegance|0",
            "talent|light|2",
            "talent|light|1",
            "talent|light|0",

            "special|crown|0",

            // weapon ascension materials
            "wam|decarabian|3",
            "wam|decarabian|2",
            "wam|decarabian|1",
            "wam|decarabian|0",

            "wam|boreal_wolf|3",
            "wam|boreal_wolf|2",
            "wam|boreal_wolf|1",
            "wam|boreal_wolf|0",

            "wam|dandelion_gladiator|3",
            "wam|dandelion_gladiator|2",
            "wam|dandelion_gladiator|1",
            "wam|dandelion_gladiator|0",

            "wam|guyun|3",
            "wam|guyun|2",
            "wam|guyun|1",
            "wam|guyun|0",

            "wam|mist_veiled_elixer|3",
            "wam|mist_veiled_elixer|2",
            "wam|mist_veiled_elixer|1",
            "wam|mist_veiled_elixer|0",

            "wam|aerosiderite|3",
            "wam|aerosiderite|2",
            "wam|aerosiderite|1",
            "wam|aerosiderite|0",

            "wam|sea_branch|3",
            "wam|sea_branch|2",
            "wam|sea_branch|1",
            "wam|sea_branch|0",

            "wam|narukami|3",
            "wam|narukami|2",
            "wam|narukami|1",
            "wam|narukami|0",

            "wam|mask_w|3",
            "wam|mask_w|2",
            "wam|mask_w|1",
            "wam|mask_w|0",
        };
        private string[] materialItems =
        {
            // material
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",

            // furnishings
            "",
            "",
            "",
            "",

            // wood
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",

            // forging ore
            "",
            "",
            "",
            "",
            "",
            "",

            // billets
            "",
            "",
            "",
            "",
            "",

            // fishbait
            "",
            "",
            "",
            "",

            // fish
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",

            // cooking ingredient
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",

            // local specialty
            "local|calla_lily|0",
            "local|wolfhook|0",
            "local|valberry|0",
            "local|cecilia|0",
            "local|windwheel_aster|0",
            "local|philanemo_mushroom|0",
            "local|jueyun_chili|0",
            "local|noctilucous_jade|0",
            "local|silk_flower|0",
            "local|glaze_lilly|0",
            "local|qingxin|0",
            "local|starconch|0",
            "local|violetgrass|0",
            "local|small_lamp_grass|0",
            "local|dandelion_seed|0",
            "local|cor_lapis|0",
            "local|onikabuto|0",
            "local|sakura_bloom|0",
            "local|crystal_marrow|0",
            "local|dendrobium|0",
            "local|naku_weed|0",
            "local|sea_ganoderma|0",
            "local|sango_pearl|0",
            "local|tenkumo_fruit|0",
            "local|fluorescent_fungus|0",
        };

        public Seelie()
        {
            inventory = new List<SeelieItem>();
        }

        public Seelie(GenshinData genshinData)
        {
            inventory = new List<SeelieItem>();

            // Assign CharDevItems
            List<Material> _charDevItems = genshinData.GetInventory().GetCharDevItems();
            foreach (Material x in _charDevItems)
            {
                SeelieItem temp = new SeelieItem();
                // Look up name in Dictionary
                string itemString = charDevItems[x.name];
                if(itemString != "")
                {
                    string[] itemInfo = itemString.Split('|');

                    // check if it splits into 3 sections
                    if(itemInfo.Length != 3)
                    {
                        break;
                    }

                    temp.type = itemInfo[0];
                    temp.item = itemInfo[1];
                    temp.tier = Int16.Parse(itemInfo[2]);
                    temp.value = x.count;
                    inventory.Add(temp);
                }
                
            }

            // Assign Materials
            List<Material> _materials = genshinData.GetInventory().GetMaterialList();
            foreach (Material x in _materials)
            {
                SeelieItem temp = new SeelieItem();
                // Look up name in Dictionary
                string itemString = materialItems[x.name];
                if (itemString != "")
                {
                    string[] itemInfo = itemString.Split('|');

                    // check if it splits into 3 sections
                    if (itemInfo.Length != 3)
                    {
                        break;
                    }

                    temp.type = itemInfo[0];
                    temp.item = itemInfo[1];
                    temp.tier = Int16.Parse(itemInfo[2]);
                    temp.value = x.count;
                    inventory.Add(temp);
                }

            }

        }

        private struct SeelieItem // char dev Item
        {
            [JsonProperty] public string type;
            [JsonProperty] public string item;
            [JsonProperty] public int tier;
            [JsonProperty] public int value;
        }
    }
}
