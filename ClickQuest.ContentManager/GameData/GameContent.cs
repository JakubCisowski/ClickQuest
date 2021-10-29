using System.Collections.Generic;
using ClickQuest.ContentManager.Models;

namespace ClickQuest.ContentManager.GameData
{
	public static class GameContent
	{
		public static List<Artifact> Artifacts { get; set; }
		public static List<Blessing> Blessings { get; set; }
		public static List<Boss> Bosses { get; set; }
		public static List<DungeonGroup> DungeonGroups { get; set; }
		public static List<Dungeon> Dungeons { get; set; }
		public static List<Material> Materials { get; set; }
		public static List<Monster> Monsters { get; set; }
		public static List<Quest> Quests { get; set; }
		public static List<Recipe> Recipes { get; set; }
		public static List<Region> Regions { get; set; }
		public static List<int> PriestOffer { get; set; }
		public static List<int> ShopOffer { get; set; }
	}
}