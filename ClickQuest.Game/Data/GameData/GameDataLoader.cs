using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClickQuest.Game.Adventures;
using ClickQuest.Game.Enemies;
using ClickQuest.Game.Extensions.ValidationManager;
using ClickQuest.Game.Heroes.Buffs;
using ClickQuest.Game.Items;
using ClickQuest.Game.Places;

namespace ClickQuest.Game.Data.GameData
{
	public static class GameDataLoader
	{
		public static void Load()
		{
			GameData.Materials = DeserializeType<Material>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "Materials.json"));
			GameData.Artifacts = DeserializeType<Artifact>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "Artifacts.json"));
			GameData.Recipes = DeserializeType<Recipe>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "Recipes.json"));
			GameData.Ingots = DeserializeType<Ingot>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "Ingots.json"));
			GameData.DungeonKeys = DeserializeType<DungeonKey>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "DungeonKeys.json"));
			GameData.Monsters = DeserializeType<Monster>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "Monsters.json"));
			GameData.Regions = DeserializeType<Region>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "Regions.json"));
			GameData.Blessings = DeserializeType<Blessing>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "Blessings.json"));
			GameData.ShopOffer = DeserializeType<int>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "ShopOffer.json"));
			GameData.PriestOffer = DeserializeType<int>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "PriestOffer.json"));
			GameData.Quests = DeserializeType<Quest>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "Quests.json"));
			GameData.Bosses = DeserializeType<Boss>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "Bosses.json"));
			GameData.DungeonGroups = DeserializeType<DungeonGroup>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "DungeonGroups.json"));
			GameData.Dungeons = DeserializeType<Dungeon>(Path.Combine(Environment.CurrentDirectory, @"Data\", @"GameData\", @"GameAssets\", "Dungeons.json"));

			PostLoad();

#if DEBUG
			DataValidator.ValidateData();
#endif

			// Refresh Pages collection in order to  rearrange page bindings.
			GameData.RefreshPages();

			GameData.CurrentPage = GameData.Pages["MainMenu"];
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

			LoadArtifactFunctionalities();

			// [PRERELEASE] In the future this will be done in content loader.
			FillMonsterLootWithEmptyLoot();
		}

		public static List<T> DeserializeType<T>(string filePath)
		{
			var result = new List<T>();
			string json = File.ReadAllText(filePath);

			// Enums as strings.
			var options = new JsonSerializerOptions
			{
				Converters =
				{
					new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
				}
			};

			result = JsonSerializer.Deserialize<List<T>>(json, options);
			return result;
		}

		public static void LoadArtifactFunctionalities()
		{
			var artifactFunctionalities = new List<ArtifactFunctionality>();

			var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => string.Equals(t.Namespace, "ClickQuest.Artifacts", StringComparison.Ordinal));

			foreach (var type in types)
			{
				if (Activator.CreateInstance(type) is ArtifactFunctionality artifactFunctionality)
				{
					artifactFunctionalities.Add(artifactFunctionality);
				}
			}

			foreach (var artifact in GameData.Artifacts)
			{
				artifact.ArtifactFunctionality = artifactFunctionalities.FirstOrDefault(x => x.Name == artifact.Name);
			}
		}
		
		public static void FillMonsterLootWithEmptyLoot()
		{
			foreach (var monster in GameData.Monsters)
			{
				var sumOfFrequencies = monster.Loot.Sum(x => x.Frequency);

				if (sumOfFrequencies < 1.0)
				{
					monster.Loot.Add(new MonsterLootPattern() 
					{ 
						ItemId = 0, 
						ItemType = ItemType.Material,
						Frequency = 1.0 - sumOfFrequencies
					});
				}
			}
		}
	}
}