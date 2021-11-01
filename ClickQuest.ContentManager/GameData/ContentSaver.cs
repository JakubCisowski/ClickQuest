using ClickQuest.ContentManager.GameData.Models;
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
			SaveContent<Artifact>(GameContent.Artifacts, ArtifactsFilePath);
			SaveContent<Blessing>(GameContent.Blessings, BlessingsFilePath);
			SaveContent<Boss>(GameContent.Bosses, BossesFilePath);
			SaveContent<Dungeon>(GameContent.Dungeons, DungeonsFilePath);
			SaveContent<DungeonGroup>(GameContent.DungeonGroups, DungeonGroupsFilePath);
			SaveContent<DungeonKey>(GameContent.DungeonKeys, DungeonKeysFilePath);
			SaveContent<Ingot>(GameContent.Ingots, IngotsFilePath);
			SaveContent<Material>(GameContent.Materials, MaterialsFilePath);
			SaveContent<Monster>(GameContent.Monsters, MonstersFilePath);
			SaveContent<Quest>(GameContent.Quests, QuestsFilePath);
			SaveContent<Recipe>(GameContent.Recipes, RecipesFilePath);
			SaveContent<Region>(GameContent.Regions, RegionsFilePath);
			SaveContent<VendorPattern>(GameContent.PriestOffer, PriestOfferFilePath);
			SaveContent<VendorPattern>(GameContent.ShopOffer, ShopOfferFilePath);
		}

		public static void SaveContent<T>(List<T> collection, string jsonFilePath)
		{
			var json = JsonSerializer.Serialize(collection, new JsonSerializerOptions()
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