// Comment or uncomment this line: based on whether we want to write JSON contents encrypted or not.
#define WRITE_ENCRYPTED
#define READ_ENCRYPTED

using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using ClickQuest.ContentManager.Logic.DataTypes.Structs;
using ClickQuest.ContentManager.Logic.Models;

using static ClickQuest.ContentManager.Logic.Helpers.FilePathHelper;

namespace ClickQuest.ContentManager.Logic.Helpers;

public static class GameAssetsHelper
{
	public static void LoadAllContent()
	{
		GameAssets.Artifacts = LoadContent<Artifact>(ArtifactsFilePath);
		GameAssets.Blessings = LoadContent<Blessing>(BlessingsFilePath);
		GameAssets.Bosses = LoadContent<Boss>(BossesFilePath);
		GameAssets.Dungeons = LoadContent<Dungeon>(DungeonsFilePath);
		GameAssets.DungeonGroups = LoadContent<DungeonGroup>(DungeonGroupsFilePath);
		GameAssets.DungeonKeys = LoadContent<DungeonKey>(DungeonKeysFilePath);
		GameAssets.Ingots = LoadContent<Ingot>(IngotsFilePath);
		GameAssets.Materials = LoadContent<Material>(MaterialsFilePath);
		GameAssets.Monsters = LoadContent<Monster>(MonstersFilePath);
		GameAssets.Quests = LoadContent<Quest>(QuestsFilePath);
		GameAssets.Recipes = LoadContent<Recipe>(RecipesFilePath);
		GameAssets.Regions = LoadContent<Region>(RegionsFilePath);
		GameAssets.PriestOffer = LoadContent<VendorPattern>(PriestOfferFilePath);
		GameAssets.ShopOffer = LoadContent<VendorPattern>(ShopOfferFilePath);
		GameAssets.GameMechanicsTabs = LoadContent<GameMechanicsTab>(GameMechanicsPath);
	}

	public static List<T> LoadContent<T>(string jsonFilePath)
	{
#if READ_ENCRYPTED
		var encryptedJson = File.ReadAllBytes(jsonFilePath);
		var json = EncryptionHelper.DecryptJsonUsingAes(encryptedJson);
#else
		var json = File.ReadAllText(jsonFilePath);
#endif

		var objects = JsonSerializer.Deserialize<List<T>>(json);

		return objects;
	}

	// Use when clearing a JSON file's contents to generate padding and the default values needed.
	public static void SeedContent<T>(string jsonFilePath)
	{
		var emptyList = new List<T>();

		var json = JsonSerializer.Serialize(emptyList);

		var encryptedJson = EncryptionHelper.EncryptJsonUsingAes(json);

		File.WriteAllBytes(jsonFilePath, encryptedJson);
	}
	
	public static void SaveAllContent()
	{
		SaveContent(GameAssets.Artifacts, ArtifactsFilePath);
		SaveContent(GameAssets.Blessings, BlessingsFilePath);
		SaveContent(GameAssets.Bosses, BossesFilePath);
		SaveContent(GameAssets.Dungeons, DungeonsFilePath);
		SaveContent(GameAssets.DungeonGroups, DungeonGroupsFilePath);
		SaveContent(GameAssets.DungeonKeys, DungeonKeysFilePath);
		SaveContent(GameAssets.Ingots, IngotsFilePath);
		SaveContent(GameAssets.Materials, MaterialsFilePath);
		SaveContent(GameAssets.Monsters, MonstersFilePath);
		SaveContent(GameAssets.Quests, QuestsFilePath);
		SaveContent(GameAssets.Recipes, RecipesFilePath);
		SaveContent(GameAssets.Regions, RegionsFilePath);
		SaveContent(GameAssets.PriestOffer, PriestOfferFilePath);
		SaveContent(GameAssets.ShopOffer, ShopOfferFilePath);
		SaveContent(GameAssets.GameMechanicsTabs, GameMechanicsPath);
	}

	public static void SaveContent<T>(List<T> collection, string jsonFilePath)
	{
		var json = JsonSerializer.Serialize(collection, new JsonSerializerOptions
		{
			WriteIndented = true,
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			IgnoreReadOnlyProperties = true
		});
		
#if WRITE_ENCRYPTED
		var encryptedJson = EncryptionHelper.EncryptJsonUsingAes(json);
		File.WriteAllBytes(jsonFilePath, encryptedJson);
#else
		File.WriteAllText(jsonFilePath, json);
#endif
	}
}