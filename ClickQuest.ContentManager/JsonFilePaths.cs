using System;
using System.IO;

namespace ClickQuest.ContentManager
{
	public static class JsonFilePaths
	{
		public static string ArtifactsFilePath { get; set; }
		public static string BlessingsFilePath { get; set; }
		public static string BossesFilePath { get; set; }
		public static string DungeonsFilePath { get; set; }
		public static string MaterialsFilePath { get; set; }
		public static string MonstersFilePath { get; set; }
		public static string QuestsFilePath { get; set; }
		public static string RecipesFilePath { get; set; }
		public static string RegionsFilePath { get; set; }
		public static string PriestOfferFilePath { get; set; }
		public static string ShopOfferFilePath { get; set; }
		
		public static void CalculateGameAssetsFilePaths()
		{
			ArtifactsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Content\", "Artifacts.json");
			BlessingsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Content\", "Blessings.json");
			BossesFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Content\", "Bosses.json");
			DungeonsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Content\", "Dungeons.json");
			MaterialsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Content\", "Materials.json");
			MonstersFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Content\", "Monsters.json");
			QuestsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Content\", "Quests.json");
			RecipesFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Content\", "Recipes.json");
			RegionsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Content\", "Regions.json");
			PriestOfferFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Content\", "PriestOffer.json");
			ShopOfferFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Content\", "ShopOffer.json");
		}
	}
}