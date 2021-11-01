using ClickQuest.ContentManager.GameData.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ClickQuest.ContentManager.GameData.JsonFilePaths;

namespace ClickQuest.ContentManager.GameData
{
	public static class ContentLoader
	{
		public static void LoadAllContent()
		{
			GameContent.Artifacts = LoadContent<Artifact>(ArtifactsFilePath);
			GameContent.Blessings = LoadContent<Blessing>(BlessingsFilePath);
			GameContent.Bosses = LoadContent<Boss>(BossesFilePath);
			GameContent.Dungeons = LoadContent<Dungeon>(DungeonsFilePath);
			GameContent.DungeonGroups = LoadContent<DungeonGroup>(DungeonGroupsFilePath);
			GameContent.DungeonKeys = LoadContent<DungeonKey>(DungeonKeysFilePath);
			GameContent.Ingots = LoadContent<Ingot>(IngotsFilePath);
			GameContent.Materials = LoadContent<Material>(MaterialsFilePath);
			GameContent.Monsters = LoadContent<Monster>(MonstersFilePath);
			GameContent.Quests = LoadContent<Quest>(QuestsFilePath);
			GameContent.Recipes = LoadContent<Recipe>(RecipesFilePath);
			GameContent.Regions = LoadContent<Region>(RegionsFilePath);
			GameContent.PriestOffer = LoadContent<VendorPattern>(PriestOfferFilePath);
			GameContent.ShopOffer = LoadContent<VendorPattern>(ShopOfferFilePath);
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