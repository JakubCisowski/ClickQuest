using System.Collections.Generic;
using System.Windows.Controls;
using ClickQuest.Enemies;
using ClickQuest.Items;
using ClickQuest.Pages;
using ClickQuest.Places;
using ClickQuest.Heroes.Buffs;

namespace ClickQuest.Data
{
	public static class GameData 
	{
		#region Collections

        public static List<Material> Materials { get; set; }
        public static List<Recipe> Recipes { get; set; }
        public static List<Artifact> Artifacts { get; set; }
        public static List<Monster> Monsters { get; set; }
        public static List<Region> Regions { get; set; }
        public static List<Blessing> Blessings { get; set; }
        public static List<int> ShopOffer { get; set; }
        public static List<int> PriestOffer { get; set; }
        public static List<Quest> Quests { get; set; }
        public static Dictionary<string, Page> Pages { get; set; }
        public static List<Boss> Bosses { get; set; }
        public static List<Dungeon> Dungeons { get; set; }
        public static List<DungeonGroup> DungeonGroups { get; set; }

        #endregion Collections

		static GameData()
		{
			// Create collections.
            Materials = new List<Material>();
            Recipes = new List<Recipe>();
            Artifacts = new List<Artifact>();
            Monsters = new List<Monster>();
            Regions = new List<Region>();
            Blessings = new List<Blessing>();
            ShopOffer = new List<int>();
            PriestOffer = new List<int>();
            Quests = new List<Quest>();
            Pages = new Dictionary<string, Page>();
            Bosses = new List<Boss>();
            Dungeons = new List<Dungeon>();
            DungeonGroups = new List<DungeonGroup>();
		}

        public static void RefreshPages()
        {
            Pages.Clear();

            // Main Menu
            Pages.Add("MainMenu", new MainMenuPage());
            // Hero Creation Page
            Pages.Add("HeroCreation", new HeroCreationPage());
            // Town
            Pages.Add("Town", new TownPage());
            // Shop
            Pages.Add("Shop", new ShopPage());
            // Blacksmith
            Pages.Add("Blacksmith", new BlacksmithPage());
            // Priest Page
            Pages.Add("Priest", new PriestPage());
            // Quest Menu Page
            Pages.Add("QuestMenu", new QuestMenuPage());
            // Dungeon Select Page
            Pages.Add("DungeonSelect", new DungeonSelectPage());
            // Dungeon Boss Page
            Pages.Add("DungeonBoss", new DungeonBossPage());

            foreach (var region in Regions)
            {
                Pages.Add(region.Name, new RegionPage(region));
            }
        }
	}
}
