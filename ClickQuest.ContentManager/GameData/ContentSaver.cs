using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
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
		}

		public static void SaveContent<T>(List<T> collection, string jsonFilePath)
		{
			string json = JsonSerializer.Serialize(collection, new JsonSerializerOptions
			{
				WriteIndented = true,
				Converters =
				{
					new JsonStringEnumConverter(null)
				}
			});

			File.WriteAllText(jsonFilePath, json);
		}
	}
}