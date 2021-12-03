using ClickQuest.ContentManager.GameData.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
            GameContent.GameMechanicsTabs = LoadContent<GameMechanicsTab>(GameMechanicsPath);
        }

        public static List<T> LoadContent<T>(string jsonFilePath)
        {
            byte[] encryptedJson = File.ReadAllBytes(jsonFilePath);

            string json = DataEncryptionController.DecryptJsonUsingAes(encryptedJson);

            var objects = JsonSerializer.Deserialize<List<T>>(json);

            return objects;
        }

        // Use when clearing a JSON file's contents to generate padding and the default values needed.
        public static void SeedContent<T>(string jsonFilePath)
        {
            var emptyList = new List<T>();

            string json = JsonSerializer.Serialize(emptyList);

            byte[] encryptedJson = DataEncryptionController.EncryptJsonUsingAes(json);

            File.WriteAllBytes(jsonFilePath, encryptedJson);
        }
    }
}