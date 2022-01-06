using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using ClickQuest.Game.DataTypes.Structs;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models;
using ClickQuest.Game.Models.Functionalities;

namespace ClickQuest.Game.Core.GameData;

public static class GameAssetsLoader
{
	public static void Load()
	{
		GameAssets.Materials = DeserializeType<Material>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Materials.aes"));
		GameAssets.Artifacts = DeserializeType<Artifact>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Artifacts.aes"));
		GameAssets.Recipes = DeserializeType<Recipe>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Recipes.aes"));
		GameAssets.Ingots = DeserializeType<Ingot>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Ingots.aes"));
		GameAssets.DungeonKeys = DeserializeType<DungeonKey>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "DungeonKeys.aes"));
		GameAssets.Monsters = DeserializeType<Monster>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Monsters.aes"));
		GameAssets.Regions = DeserializeType<Region>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Regions.aes"));
		GameAssets.Blessings = DeserializeType<Blessing>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Blessings.aes"));
		GameAssets.ShopOffer = DeserializeType<VendorPattern>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "ShopOffer.aes"));
		GameAssets.PriestOffer = DeserializeType<VendorPattern>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "PriestOffer.aes"));
		GameAssets.Quests = DeserializeType<Quest>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Quests.aes"));
		GameAssets.Bosses = DeserializeType<Boss>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Bosses.aes"));
		GameAssets.DungeonGroups = DeserializeType<DungeonGroup>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "DungeonGroups.aes"));
		GameAssets.Dungeons = DeserializeType<Dungeon>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "Dungeons.aes"));
		GameAssets.GameMechanicsTabs = DeserializeType<GameMechanicsTab>(Path.Combine(Environment.CurrentDirectory, @"Core\", @"GameData\", @"GameAssets\", "GameMechanics.aes"));

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

		var encryptedJson = File.ReadAllBytes(filePath);

		var json = EncryptionHelper.DecryptJsonUsingAes(encryptedJson);

		result = JsonSerializer.Deserialize<List<T>>(json);
		return result;
	}

	public static void LoadArtifactFunctionalities()
	{
		var artifactFunctionalities = new List<ArtifactFunctionality>();

		var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => string.Equals(t.Namespace, "ClickQuest.Game.Models.Functionalities.Artifacts", StringComparison.Ordinal));

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

		var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => string.Equals(t.Namespace, "ClickQuest.Game.Models.Functionalities.ArtifactTypes", StringComparison.Ordinal));

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

		var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => string.Equals(t.Namespace, "ClickQuest.Game.Models.Functionalities.Affixes", StringComparison.Ordinal));

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