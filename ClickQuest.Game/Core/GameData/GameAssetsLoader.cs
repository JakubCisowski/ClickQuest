using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.Adventures;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Places;
using ClickQuest.Game.Extensions.Validation;

namespace ClickQuest.Game.Core.GameData
{
	public static class GameAssetsLoader
	{
		public static void Load()
		{
			GameAssets.Materials = DeserializeType<Material>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Materials.json"));
			GameAssets.Artifacts = DeserializeType<Artifact>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Artifacts.json"));
			GameAssets.Recipes = DeserializeType<Recipe>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Recipes.json"));
			GameAssets.Ingots = DeserializeType<Ingot>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Ingots.json"));
			GameAssets.DungeonKeys = DeserializeType<DungeonKey>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "DungeonKeys.json"));
			GameAssets.Monsters = DeserializeType<Monster>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Monsters.json"));
			GameAssets.Regions = DeserializeType<Region>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Regions.json"));
			GameAssets.Blessings = DeserializeType<Blessing>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Blessings.json"));
			GameAssets.ShopOffer = DeserializeType<int>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "ShopOffer.json"));
			GameAssets.PriestOffer = DeserializeType<int>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "PriestOffer.json"));
			GameAssets.Quests = DeserializeType<Quest>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Quests.json"));
			GameAssets.Bosses = DeserializeType<Boss>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Bosses.json"));
			GameAssets.DungeonGroups = DeserializeType<DungeonGroup>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "DungeonGroups.json"));
			GameAssets.Dungeons = DeserializeType<Dungeon>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Dungeons.json"));

			PostLoad();

#if DEBUG
			DataValidator.ValidateData();
#endif

			// Refresh Pages collection in order to  rearrange page bindings.
			GameAssets.RefreshPages();

			GameAssets.CurrentPage = GameAssets.Pages["MainMenu"];
		}

		public static void PostLoad()
		{
			foreach (var recipe in GameAssets.Recipes)
			{
				// Update descriptions now that artifacts are loaded.
				recipe.UpdateDescription();
				recipe.UpdateRequirementsDescription();
			}

			foreach (var quest in GameAssets.Quests)
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

			var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => string.Equals(t.Namespace, "ClickQuest.Game.Core.Artifacts", StringComparison.Ordinal));

			foreach (var type in types)
			{
				if (Activator.CreateInstance(type) is ArtifactFunctionality artifactFunctionality)
				{
					artifactFunctionalities.Add(artifactFunctionality);
				}
			}

			foreach (var artifact in GameAssets.Artifacts)
			{
				artifact.ArtifactFunctionality = artifactFunctionalities.FirstOrDefault(x => x.Name == artifact.Name);
			}
		}

		public static void FillMonsterLootWithEmptyLoot()
		{
			foreach (var monster in GameAssets.Monsters)
			{
				double sumOfFrequencies = monster.Loot.Sum(x => x.Frequency);

				if (sumOfFrequencies < 1.0)
				{
					monster.Loot.Add(new MonsterLootPattern
					{
						LootId = 0,
						LootType = LootType.Material,
						Frequency = 1.0 - sumOfFrequencies
					});
				}
			}
		}
	}
}