using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClickQuest.Adventures;
using ClickQuest.Enemies;
using ClickQuest.Extensions.ValidationManager;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Items;
using ClickQuest.Places;

namespace ClickQuest.Data
{
	public static class DataLoader
	{
		public static void Load()
		{
			GameData.Materials = DeserializeType<Material>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "Materials.json"));
			GameData.Artifacts = DeserializeType<Artifact>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "Artifacts.json"));
			GameData.Recipes = DeserializeType<Recipe>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "Recipes.json"));
			GameData.Ingots = DeserializeType<Ingot>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "Ingots.json"));
			GameData.DungeonKeys = DeserializeType<DungeonKey>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "DungeonKeys.json"));
			GameData.Monsters = DeserializeType<Monster>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "Monsters.json"));
			GameData.Regions = DeserializeType<Region>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "Regions.json"));
			GameData.Blessings = DeserializeType<Blessing>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "Blessings.json"));
			GameData.ShopOffer = DeserializeType<int>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "ShopOffer.json"));
			GameData.PriestOffer = DeserializeType<int>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "PriestOffer.json"));
			GameData.Quests = DeserializeType<Quest>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "Quests.json"));
			GameData.Bosses = DeserializeType<Boss>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "Bosses.json"));
			GameData.DungeonGroups = DeserializeType<DungeonGroup>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "DungeonGroups.json"));
			GameData.Dungeons = DeserializeType<Dungeon>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameAssets\", "Dungeons.json"));

#if DEBUG
			DataValidator.ValidateData();
#endif

			PostLoad();

			// Refresh Pages collection in order to  rearrange page bindings.
			GameData.RefreshPages();
		}

		public static void PostLoad()
		{
			foreach (var recipe in GameData.Recipes)
			{
				// Update descriptions now that artifacts are loaded.
				recipe.UpdateDescription();
				recipe.UpdateRequirementsDescription();
			}

			foreach (var quest in GameData.Quests)
			{
				quest.UpdateAllRewardsDescription();
			}
		}

		public static List<T> DeserializeType<T>(string filePath)
		{
			var result = new List<T>();
			string json = File.ReadAllText(filePath);

			// Enums as strings.
			var options = new JsonSerializerOptions {Converters = {new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)}};

			result = JsonSerializer.Deserialize<List<T>>(json, options);
			return result;
		}
	}
}