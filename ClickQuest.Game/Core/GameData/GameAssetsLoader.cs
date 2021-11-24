using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.Adventures;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Info;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Items.Patterns;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Places;

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
			GameAssets.ShopOffer = DeserializeType<VendorPattern>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "ShopOffer.json"));
			GameAssets.PriestOffer = DeserializeType<VendorPattern>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "PriestOffer.json"));
			GameAssets.Quests = DeserializeType<Quest>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Quests.json"));
			GameAssets.Bosses = DeserializeType<Boss>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Bosses.json"));
			GameAssets.DungeonGroups = DeserializeType<DungeonGroup>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "DungeonGroups.json"));
			GameAssets.Dungeons = DeserializeType<Dungeon>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Dungeons.json"));
			GameAssets.GameMechanicsTabs = DeserializeType<GameMechanicsTab>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "GameMechanics.json"));

			PostLoad();

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
			LoadArtifactTypeFunctionalities();

			LoadBossAffixFunctionalities();
		}

		public static List<T> DeserializeType<T>(string filePath)
		{
			var result = new List<T>();
			string json = File.ReadAllText(filePath);

			result = JsonSerializer.Deserialize<List<T>>(json);
			return result;
		}

		public static void SaveEnemies()
		{
			// Save Monsters and Bosses to keep track of the BestiaryDiscovered properties.
			SerializeType<Monster>(GameAssets.Monsters, Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Monsters.json"));
			SerializeType<Boss>(GameAssets.Bosses, Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Bosses.json"));
		}

		public static void SerializeType<T>(List<T> objects, string filePath)
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				IgnoreReadOnlyProperties = true
			};

			string json = JsonSerializer.Serialize<List<T>>(objects, options);

			File.WriteAllText(filePath, json);
		}

		public static void LoadArtifactFunctionalities()
		{
			var artifactFunctionalities = new List<ArtifactFunctionality>();

			var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => string.Equals(t.Namespace, "ClickQuest.Game.Core.Items.Artifacts", StringComparison.Ordinal));

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

		public static void LoadArtifactTypeFunctionalities()
		{
			var artifactTypeFunctionalities = new List<ArtifactTypeFunctionality>();

			var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => string.Equals(t.Namespace, "ClickQuest.Game.Core.Items.ArtifactTypes", StringComparison.Ordinal));

			foreach (var type in types)
			{
				if (Activator.CreateInstance(type) is ArtifactTypeFunctionality artifactTypeFunctionality)
				{
					artifactTypeFunctionalities.Add(artifactTypeFunctionality);
				}
			}

			foreach (var artifact in GameAssets.Artifacts)
			{
				artifact.ArtifactFunctionality.ArtifactTypeFunctionality = artifactTypeFunctionalities.FirstOrDefault(x => x.ArtifactType == artifact.ArtifactType);
			}
		}

		public static void LoadBossAffixFunctionalities()
		{
			var affixFunctionalities = new List<AffixFunctionality>();

			var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => string.Equals(t.Namespace, "ClickQuest.Game.Core.Enemies.Affixes", StringComparison.Ordinal));

			foreach (var type in types)
			{
				if (Activator.CreateInstance(type) is AffixFunctionality affixFunctionality)
				{
					affixFunctionalities.Add(affixFunctionality);
				}
			}

			foreach (var boss in GameAssets.Bosses)
			{
				boss.AffixFunctionalities = new List<AffixFunctionality>();

				foreach (var affix in boss.Affixes)
				{
					boss.AffixFunctionalities.Add(affixFunctionalities.FirstOrDefault(x => x.Affix == affix));
				}
			}
		}
	}
}