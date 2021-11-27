using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using static ClickQuest.ContentManager.GameData.JsonFilePaths;

namespace ClickQuest.ContentManager.GameData
{
	public static class ContentSaver
	{
		public static void SaveAllContent()
		{
			SaveContent(GameContent.Artifacts, ArtifactsFilePath);
			SaveContent(GameContent.Blessings, BlessingsFilePath);
			SaveContent(GameContent.Bosses, BossesFilePath);
			SaveContent(GameContent.Dungeons, DungeonsFilePath);
			SaveContent(GameContent.DungeonGroups, DungeonGroupsFilePath);
			SaveContent(GameContent.DungeonKeys, DungeonKeysFilePath);
			SaveContent(GameContent.Ingots, IngotsFilePath);
			SaveContent(GameContent.Materials, MaterialsFilePath);
			SaveContent(GameContent.Monsters, MonstersFilePath);
			SaveContent(GameContent.Quests, QuestsFilePath);
			SaveContent(GameContent.Recipes, RecipesFilePath);
			SaveContent(GameContent.Regions, RegionsFilePath);
			SaveContent(GameContent.PriestOffer, PriestOfferFilePath);
			SaveContent(GameContent.ShopOffer, ShopOfferFilePath);
			SaveContent(GameContent.GameMechanicsTabs, GameMechanicsPath);
		}

		public static void SaveContent<T>(List<T> collection, string jsonFilePath)
		{
			string json = JsonSerializer.Serialize(collection, new JsonSerializerOptions
			{
				WriteIndented = true,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				IgnoreReadOnlyProperties = true
			});

			var encryptedJson = DataEncryptionController.EncryptJsonUsingAes(json);

			File.WriteAllBytes(jsonFilePath, encryptedJson);
		}
	}
}