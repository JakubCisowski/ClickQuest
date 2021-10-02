using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClickQuest.ContentManager.Models;

namespace ClickQuest.ContentManager
{
	public static class ContentLoader
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
		
		public static void LoadAllContent()
		{
			GameContent.Artifacts = LoadContent<Artifact>(ArtifactsFilePath);
			GameContent.Blessings = LoadContent<Blessing>(BlessingsFilePath);
			GameContent.Bosses = LoadContent<Boss>(BossesFilePath);
			GameContent.Dungeons = LoadContent<Dungeon>(DungeonsFilePath);
			GameContent.Materials = LoadContent<Material>(MaterialsFilePath);
			GameContent.Monsters = LoadContent<Monster>(MonstersFilePath);
			GameContent.Quests = LoadContent<Quest>(QuestsFilePath);
			GameContent.Recipes = LoadContent<Recipe>(RecipesFilePath);
			GameContent.Regions = LoadContent<Region>(RegionsFilePath);
			GameContent.PriestOffer = LoadContent<int>(PriestOfferFilePath);
			GameContent.ShopOffer = LoadContent<int>(ShopOfferFilePath);
		}

		public static List<T> LoadContent<T>(string jsonFilePath)
		{
			var json = File.ReadAllText(jsonFilePath);
			var options = new JsonSerializerOptions
			{
				Converters =
				{
					new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
				}
			};
			
			var objects = JsonSerializer.Deserialize<List<T>>(json, options);

			return objects;
		}
	}
}