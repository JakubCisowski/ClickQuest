﻿using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ClickQuest.ContentManager.Models;
using static ClickQuest.ContentManager.JsonFilePaths;

namespace ClickQuest.ContentManager
{
	public static class ContentSaver
	{
		public static void SaveAllContent()
		{
			SaveContent<Artifact>(GameContent.Artifacts, ArtifactsFilePath);
			SaveContent<Blessing>(GameContent.Blessings, BlessingsFilePath);
			SaveContent<Boss>(GameContent.Bosses, BossesFilePath);
			SaveContent<Dungeon>(GameContent.Dungeons, DungeonsFilePath);
			SaveContent<Material>(GameContent.Materials, MaterialsFilePath);
			SaveContent<Monster>(GameContent.Monsters, MonstersFilePath);
			SaveContent<Quest>(GameContent.Quests, QuestsFilePath);
			SaveContent<Recipe>(GameContent.Recipes, RecipesFilePath);
			SaveContent<Region>(GameContent.Regions, RegionsFilePath);
			SaveContent<int>(GameContent.PriestOffer, PriestOfferFilePath);
			SaveContent<int>(GameContent.ShopOffer, ShopOfferFilePath);
		}

		public static void SaveContent<T>(List<T> collection, string jsonFilePath)
		{
			var json = JsonSerializer.Serialize(collection, new JsonSerializerOptions() {WriteIndented = true});
			
			File.WriteAllText(jsonFilePath, json);
		}
	}
}