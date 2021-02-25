using ClickQuest.Enemies;
using ClickQuest.Heroes;
using ClickQuest.Items;
using ClickQuest.Pages;
using ClickQuest.Places;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace ClickQuest.Data
{
	public static partial class Database
	{
		#region Collections

		public static List<Material> Materials { get; set; }
		public static List<Recipe> Recipes { get; set; }
		public static List<Artifact> Artifacts { get; set; }
		public static List<Monster> Monsters { get; set; }
		public static List<Region> Regions { get; set; }
		public static List<Blessing> Blessings { get; set; }
		public static List<Recipe> ShopOffer { get; set; }
		public static List<Blessing> PriestOffer { get; set; }
		public static List<Quest> Quests { get; set; }
		public static Dictionary<string, Page> Pages { get; set; }
		public static List<Monster> Bosses { get; set; }
		public static List<Dungeon> Dungeons { get; set; }
		public static List<DungeonGroup> DungeonGroups { get; set; }

		#endregion Collections

		public static void Load()
		{
			// Create collections.
			Materials = new List<Material>();
			Recipes = new List<Recipe>();
			Artifacts = new List<Artifact>();
			Monsters = new List<Monster>();
			Regions = new List<Region>();
			Blessings = new List<Blessing>();
			ShopOffer = new List<Recipe>();
			PriestOffer = new List<Blessing>();
			Quests = new List<Quest>();
			Pages = new Dictionary<string, Page>();
			Bosses = new List<Monster>();
			Dungeons = new List<Dungeon>();
			DungeonGroups = new List<DungeonGroup>();

			// Fill collections with JSON data.
			LoadMaterials();
			LoadArtifacts();
			LoadRecipes();
			LoadMonsters();
			LoadRegions();
			LoadBlessings();
			LoadShopOffer();
			LoadPriestOffer();
			LoadQuests();
			LoadBosses();
			LoadDungeonGroups();
			LoadDungeons();

			// Check if Ids exist and are unique.
			ValidateData();

			// Refresh Pages collection in order to rearrange page bindings.
			RefreshPages();
		}

		#region JSON Load

		public static void LoadMaterials()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Materials.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Materials"];

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["Materials"][i]["Id"].ToString());
				var name = parsedObject["Materials"][i]["Name"].ToString();
				var rarity = (Rarity)int.Parse(parsedObject["Materials"][i]["Rarity"].ToString());
				var description = parsedObject["Materials"][i]["Description"].ToString();
				var value = int.Parse(parsedObject["Materials"][i]["Value"].ToString());

				var newMaterial = new Material(id, name, rarity, description, value);
				Materials.Add(newMaterial);
			}
		}

		public static void LoadArtifacts()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Artifacts.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Artifacts"];

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["Artifacts"][i]["Id"].ToString());
				var name = parsedObject["Artifacts"][i]["Name"].ToString();
				var rarity = (Rarity)int.Parse(parsedObject["Artifacts"][i]["Rarity"].ToString());
				var description = parsedObject["Artifacts"][i]["Description"].ToString();
				var value = int.Parse(parsedObject["Artifacts"][i]["Value"].ToString());

				var newArtifact = new Artifact(id, name, rarity, description, value);
				Artifacts.Add(newArtifact);
			}
		}

		public static void LoadRecipes()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Recipes.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Recipes"];

			// Error check
			var errorLog = new List<string>();

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["Recipes"][i]["Id"].ToString());
				var artifactId = int.Parse(parsedObject["Recipes"][i]["ArtifactId"].ToString());

				var materialIds = new Dictionary<int, int>();
				var materialIdsArray = (JArray)parsedObject["Recipes"][i]["MaterialIds"];

				for (int j = 0; j < materialIdsArray.Count; j++)
				{
					var materialId = int.Parse(parsedObject["Recipes"][i]["MaterialIds"][j]["MaterialId"].ToString());
					var materialQuantity = int.Parse(parsedObject["Recipes"][i]["MaterialIds"][j]["Quantity"].ToString());

					materialIds.Add(materialId, materialQuantity);
				}

				var name = parsedObject["Recipes"][i]["Name"].ToString();
				var rarity = (Rarity)int.Parse(parsedObject["Recipes"][i]["Rarity"].ToString());
				var artifactDescription = Artifacts.FirstOrDefault(x => x.Id == artifactId).Description;
				var value = int.Parse(parsedObject["Recipes"][i]["Value"].ToString());

				var newRecipe = new Recipe(id, name, rarity, artifactDescription, value, artifactId);
				newRecipe.MaterialIds = materialIds;
				Recipes.Add(newRecipe);
				newRecipe.UpdateRequirementsDescription();
			}

			// Check if every recipe components' and artifact's IDs exist.
			foreach (var recipe in Recipes)
			{
				var artifact = Artifacts.FirstOrDefault(x => x.Id == recipe.ArtifactId);
				if (artifact is null)
				{
					errorLog.Add($"Error in LoadRecipes: Recipe Id: {recipe.Id} - There is no artifact with Id: {recipe.ArtifactId}");
				}

				foreach (var pair in recipe.MaterialIds)
				{
					var material = Materials.FirstOrDefault(x => x.Id == pair.Key);
					if (material is null)
					{
						errorLog.Add($"Error in LoadRecipes: Recipe Id: {recipe.Id} - There is no material with Id: {pair.Key}");
					}

					if (pair.Value <= 0)
					{
						errorLog.Add($"Error in LoadRecipes: Recipe Id: {recipe.Id} - Invalid count of material with Id: {pair.Key}");
					}
				}
			}

			if (errorLog.Count > 0)
			{
				Logger.Log(errorLog);
			}
		}

		public static void LoadMonsters()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Monsters.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Monsters"];

			// Error check
			var errorLog = new List<string>();

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["Monsters"][i]["Id"].ToString());
				var name = parsedObject["Monsters"][i]["Name"].ToString();
				var description = parsedObject["Monsters"][i]["Description"].ToString();
				var health = int.Parse(parsedObject["Monsters"][i]["Health"].ToString());
				var image = parsedObject["Monsters"][i]["Image"].ToString();

				var lootTemp = new List<(Item, ItemType, double)>();
				var lootArray = (JArray)parsedObject["Monsters"][i]["Loot"];

				// Error check
				double frequencySum = 0;

				for (int j = 0; j < lootArray.Count; j++)
				{
					var itemType = (ItemType)Enum.Parse(typeof(ItemType), parsedObject["Monsters"][i]["Loot"][j]["Type"].ToString());
					var itemId = int.Parse(parsedObject["Monsters"][i]["Loot"][j]["Id"].ToString());

					Item item = null;

					switch (itemType)
					{
						case ItemType.Material:
							item = Materials.Where(x => x.Id == itemId).FirstOrDefault();
							break;

						case ItemType.Recipe:
							item = Recipes.Where(x => x.Id == itemId).FirstOrDefault();
							break;

						case ItemType.Artifact:
							item = Artifacts.Where(x => x.Id == itemId).FirstOrDefault();
							break;
					}

					if (item is null)
					{
						// Item of that Id does not exist.
						errorLog.Add($"Error in LoadMonsters: {itemType} of id {itemId} does not exist.");
					}

					var frequency = Double.Parse(parsedObject["Monsters"][i]["Loot"][j]["Frequency"].ToString());

					frequencySum += frequency;

					lootTemp.Add((item, itemType, frequency));
				}

				if (frequencySum != 1)
				{
					errorLog.Add($"Error in LoadMonsters: {name} - loot frequency sums up to {frequencySum} instead of 1.");
				}

				var newMonster = new Monster(id, name, health, image, lootTemp, description);
				Monsters.Add(newMonster);
			}

			if (errorLog.Count > 0)
			{
				Logger.Log(errorLog);
			}
		}

		public static void LoadRegions()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Regions.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Regions"];

			// Error check
			var errorLog = new List<string>();

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["Regions"][i]["Id"].ToString());
				var levelRequirement = int.Parse(parsedObject["Regions"][i]["LevelRequirement"].ToString());
				var name = parsedObject["Regions"][i]["Name"].ToString();
				var description = parsedObject["Regions"][i]["Description"].ToString();
				var background = parsedObject["Regions"][i]["Background"].ToString();

				var monstersTemp = new List<(Monster, Double)>();
				var monstersArray = (JArray)parsedObject["Regions"][i]["Monsters"];

				// Error check
				double frequencySum = 0;

				for (int j = 0; j < monstersArray.Count; j++)
				{
					var monsterId = int.Parse(parsedObject["Regions"][i]["Monsters"][j]["Id"].ToString());
					var monster = Monsters.Where(x => x.Id == monsterId).FirstOrDefault();

					if (monster is null)
					{
						// Monster of monsterId does not exist.
						errorLog.Add($"Error: Monster of id {monsterId} does not exist.");
					}

					var frequency = Double.Parse(parsedObject["Regions"][i]["Monsters"][j]["Frequency"].ToString());

					frequencySum += frequency;

					monstersTemp.Add((monster, frequency));
				}

				if (frequencySum != 1)
				{
					errorLog.Add($"Error in LoadRegions: {name} - monster frequency sums up to {frequencySum} instead of 1.");
				}

				var newRegion = new Region(id, name,description, background, monstersTemp, levelRequirement);
				Regions.Add(newRegion);
			}

			if (errorLog.Count > 0)
			{
				Logger.Log(errorLog);
			}
		}

		public static void LoadBlessings()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Blessings.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Blessings"];

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["Blessings"][i]["Id"].ToString());
				var name = parsedObject["Blessings"][i]["Name"].ToString();
				var type = (BlessingType)Enum.Parse(typeof(BlessingType), parsedObject["Blessings"][i]["Type"].ToString());
				var rarity = (Rarity)int.Parse(parsedObject["Blessings"][i]["Rarity"].ToString());
				var duration = int.Parse(parsedObject["Blessings"][i]["Duration"].ToString());
				var description = parsedObject["Blessings"][i]["Description"].ToString();
				var buff = int.Parse(parsedObject["Blessings"][i]["Buff"].ToString());
				var value = int.Parse(parsedObject["Blessings"][i]["Value"].ToString());

				var newBlessing = new Blessing(id, name, type, rarity, duration, description, buff, value);
				Blessings.Add(newBlessing);
			}
		}

		public static void LoadShopOffer()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "ShopOffer.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["ShopOffer"];

			var errorLog = new List<string>();

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["ShopOffer"][i]["RecipeId"].ToString());
				var recipe = Recipes.Where(x => x.Id == id).FirstOrDefault();

				if (recipe is null)
				{
					// Recipe of id does not exist.
					errorLog.Add($"Error in LoadShopOffer: Recipe of id {id} does not exist.");
				}

				ShopOffer.Add(recipe);
			}

			if (errorLog.Count > 0)
			{
				Logger.Log(errorLog);
			}
		}

		public static void LoadPriestOffer()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "PriestOffer.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["PriestOffer"];

			var errorLog = new List<string>();

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["PriestOffer"][i]["BlessingId"].ToString());
				var blessing = Blessings.Where(x => x.Id == id).FirstOrDefault();

				if (blessing is null)
				{
					// Blessing of id does not exist.
					errorLog.Add($"Error in LoadPriestOffer: Blessing of id {id} does not exist.");
				}

				PriestOffer.Add(blessing);
			}

			if (errorLog.Count > 0)
			{
				Logger.Log(errorLog);
			}
		}

		public static void LoadQuests()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Quests.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Quests"];

			var errorLog = new List<string>();

			for (var i = 0; i < jArray.Count; i++)
			{
				// Load: id(int), rare(string to bool), hero class(string to hero class), name(string), duration(int), description(string)
				// + interpret rewards, add them to rewards id lists

				var id = int.Parse(parsedObject["Quests"][i]["Id"].ToString());
				var name = parsedObject["Quests"][i]["Name"].ToString();
				var duration = int.Parse(parsedObject["Quests"][i]["Duration"].ToString());
				var description = parsedObject["Quests"][i]["Description"].ToString();
				var heroClass = (HeroClass)Enum.Parse(typeof(HeroClass), parsedObject["Quests"][i]["HeroClass"].ToString());
				var rarity = bool.Parse(parsedObject["Quests"][i]["Rare"].ToString());

				var jArrayBlessings = (JArray)parsedObject["Quests"][i]["RewardBlessingIds"];
				var blessingIds = new List<int>();
				foreach (var BlessingId in jArrayBlessings)
				{
					var idAsInt = int.Parse(BlessingId.ToString());
					if (Blessings.FirstOrDefault(x => x.Id == idAsInt) is null)
					{
						// Blessing of id Blessing id does not exist.
						errorLog.Add($"Error in LoadQuests: Blessing of id {idAsInt} does not exist.");
					}
					blessingIds.Add(idAsInt);
				}

				var jArrayRecipes = (JArray)parsedObject["Quests"][i]["RewardRecipeIds"];
				var recipeIds = new List<int>();
				foreach (var RecipeId in jArrayRecipes)
				{
					var idAsInt = int.Parse(RecipeId.ToString());
					if (Recipes.FirstOrDefault(x => x.Id == idAsInt) is null)
					{
						// Blessing of id Blessing id does not exist.
						errorLog.Add($"Error in LoadQuests: Recipe of id {idAsInt} does not exist.");
					}
					recipeIds.Add(idAsInt);
				}

				var jArrayMaterials = (JArray)parsedObject["Quests"][i]["RewardMaterialsIds"];
				var materialIds = new List<int>();
				foreach (var MaterialId in jArrayMaterials)
				{
					var idAsInt = int.Parse(MaterialId.ToString());
					if (Materials.FirstOrDefault(x => x.Id == idAsInt) is null)
					{
						// Blessing of id Blessing id does not exist.
						errorLog.Add($"Error in LoadQuests: Material of id {idAsInt} does not exist.");
					}
					materialIds.Add(idAsInt);
				}

				var jArrayIngots = (JArray)parsedObject["Quests"][i]["RewardIngots"];
				var ingotRarities = new List<Rarity>();
				foreach (var IngotRarity in jArrayIngots)
				{
					var ingotRarityAsInt = int.Parse(IngotRarity.ToString());
					if (ingotRarityAsInt < 0 || ingotRarityAsInt > 5)
					{
						// Ingot of this rarity does not exist.
						errorLog.Add($"Error in LoadQuests: Ingot of rarity {ingotRarityAsInt} does not exist.");
					}
					var actualIngotRarity = (Rarity)ingotRarityAsInt;
					ingotRarities.Add(actualIngotRarity);
				}

				var newQuest = new Quest(id, rarity, heroClass, name, duration, description)
				{
					RewardRecipeIds = recipeIds,
					RewardBlessingIds = blessingIds,
					RewardMaterialIds = materialIds,
					RewardIngots = ingotRarities
				};

				Quests.Add(newQuest);
			}

			if (errorLog.Count > 0)
			{
				Logger.Log(errorLog);
			}
		}

		public static void LoadBosses()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Bosses.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Bosses"];

			var errorLog = new List<string>();

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["Bosses"][i]["Id"].ToString());
				var name = parsedObject["Bosses"][i]["Name"].ToString();
				var health = int.Parse(parsedObject["Bosses"][i]["Health"].ToString());
				var image = parsedObject["Bosses"][i]["Image"].ToString();
				var description = parsedObject["Bosses"][i]["Description"].ToString();

				var lootTemp = new List<(Item, ItemType, List<double>)>();
				var lootArray = (JArray)parsedObject["Bosses"][i]["Loot"];

				for (int j = 0; j < lootArray.Count; j++)
				{
					var itemType = (ItemType)Enum.Parse(typeof(ItemType), parsedObject["Bosses"][i]["Loot"][j]["Type"].ToString());
					var itemId = int.Parse(parsedObject["Bosses"][i]["Loot"][j]["Id"].ToString());

					Item item = null;

					switch (itemType)
					{
						case ItemType.Material:
							item = Materials.Where(x => x.Id == itemId).FirstOrDefault();
							break;

						case ItemType.Recipe:
							item = Recipes.Where(x => x.Id == itemId).FirstOrDefault();
							break;

						case ItemType.Artifact:
							item = Artifacts.Where(x => x.Id == itemId).FirstOrDefault();
							break;
					}

					if (item is null)
					{
						// Item of that Id does not exist.
						errorLog.Add($"Error in LoadBosses: {itemType} of id {itemId} does not exist.");
					}

					var frequencies = (JArray)parsedObject["Bosses"][i]["Loot"][j]["Frequency"];
					var frequencyList = new List<double>();

					for (int k = 0; k < frequencies.Count; k++)
					{
						frequencyList.Add(double.Parse(frequencies[k].ToString()));
					}

					lootTemp.Add((item, itemType, frequencyList));
				}

				var newBoss = new Monster(id, name, health, image, lootTemp, description);
				Bosses.Add(newBoss);
			}

			if (errorLog.Count > 0)
			{
				Logger.Log(errorLog);
			}
		}

		public static void LoadDungeonGroups()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "DungeonGroups.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["DungeonGroups"];

			var errorLog = new List<string>();

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["DungeonGroups"][i]["Id"].ToString());
				var name = parsedObject["DungeonGroups"][i]["Name"].ToString();
				var colour = parsedObject["DungeonGroups"][i]["Colour"].ToString();
				var description = parsedObject["DungeonGroups"][i]["Description"].ToString();

				var keysTemp = new List<int>();
				var keysArray = (JArray)parsedObject["DungeonGroups"][i]["DungeonKeyRequirementRarities"];

				for (int j = 0; j < keysArray.Count; j++)
				{
					var keyRarity = int.Parse(parsedObject["DungeonGroups"][i]["DungeonKeyRequirementRarities"][j].ToString());

					if (keyRarity < 0 || keyRarity > 5)
					{
						// Dungeon key of this rarity does not exist.
						errorLog.Add($"Error in LoadDungeonGroups: Dungeon key of rarity {keyRarity} does not exist.");
					}

					keysTemp.Add(keyRarity);
				}

				var newDungeonGroup = new DungeonGroup(id, name, description, keysTemp, Color.FromName(colour));
				DungeonGroups.Add(newDungeonGroup);
			}

			if (errorLog.Count > 0)
			{
				Logger.Log(errorLog);
			}
		}

		public static void LoadDungeons()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Dungeons.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Dungeons"];

			var errorLog = new List<string>();

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["Dungeons"][i]["Id"].ToString());
				var dungeonGroupId = int.Parse(parsedObject["Dungeons"][i]["DungeonGroupId"].ToString());

				if (DungeonGroups.FirstOrDefault(x => x.Id == dungeonGroupId) is null)
				{
					// Dungeon group of this id does not exist.
					errorLog.Add($"Error in LoadDungeons: Dungeon group of id {dungeonGroupId} does not exist.");
				}

				var name = parsedObject["Dungeons"][i]["Name"].ToString();
				var background = parsedObject["Dungeons"][i]["Background"].ToString();
				var description = parsedObject["Dungeons"][i]["Description"].ToString();

				var bossesTemp = new List<Monster>();
				var bossesArray = (JArray)parsedObject["Dungeons"][i]["BossIds"];

				for (int j = 0; j < bossesArray.Count; j++)
				{
					var bossId = int.Parse(parsedObject["Dungeons"][i]["BossIds"][j].ToString());
					var boss = Bosses.Where(x => x.Id == bossId).FirstOrDefault();

					if (boss is null)
					{
						// Boss of that Id does not exist.
						errorLog.Add($"Error in LoadDungeons: Boss of id {bossId} does not exist.");
					}

					bossesTemp.Add(boss);
				}

				var newDungeon = new Dungeon(id, DungeonGroups.FirstOrDefault(x => x.Id == dungeonGroupId), name, background, description, bossesTemp);
				Dungeons.Add(newDungeon);
			}

			if (errorLog.Count > 0)
			{
				Logger.Log(errorLog);
			}
		}

		#endregion JSON Load

		private static void ValidateData()
		{
			var errorLog = new List<string>();

			// Check if all Ids are unique.
			var materials = Materials.Select(x => x.Id).Distinct();
			if (materials.Count() != Materials.Count())
			{
				errorLog.Add($"Error: Id duplicates detected in Materials.JSON");
			}

			var artifacts = Artifacts.Select(x => x.Id).Distinct();
			if (artifacts.Count() != Artifacts.Count())
			{
				errorLog.Add($"Error: Id duplicates detected in Artifacts.JSON");
			}

			var recipes = Recipes.Select(x => x.Id).Distinct();
			if (recipes.Count() != Recipes.Count())
			{
				errorLog.Add($"Error: Id duplicates detected in Recipes.JSON");
			}

			var regions = Regions.Select(x => x.Id).Distinct();
			if (regions.Count() != Regions.Count())
			{
				errorLog.Add($"Error: Id duplicates detected in Regions.JSON");
			}

			var monsters = Monsters.Select(x => x.Id).Distinct();
			if (monsters.Count() != Monsters.Count())
			{
				errorLog.Add($"Error: Id duplicates detected in Monsters.JSON");
			}

			var blessings = Blessings.Select(x => x.Id).Distinct();
			if (blessings.Count() != Blessings.Count())
			{
				errorLog.Add($"Error: Id duplicates detected in Blessings.JSON");
			}

			var quests = Quests.Select(x => x.Id).Distinct();
			if (quests.Count() != Quests.Count())
			{
				errorLog.Add($"Error: Id duplicates detected in Quests.JSON");
			}

			var dungeons = Dungeons.Select(x => x.Id).Distinct();
			if (dungeons.Count() != Dungeons.Count())
			{
				errorLog.Add($"Error: Id duplicates detected in Dungeons.JSON");
			}

			var bosses = Bosses.Select(x => x.Id).Distinct();
			if (bosses.Count() != Bosses.Count())
			{
				errorLog.Add($"Error: Id duplicates detected in Bosses.JSON");
			}

			var dungeonGroups = DungeonGroups.Select(x => x.Id).Distinct();
			if (dungeonGroups.Count() != DungeonGroups.Count())
			{
				errorLog.Add($"Error: Id duplicates detected in DungeonGroups.JSON");
			}

			if (errorLog.Count > 0)
			{
				Logger.Log(errorLog);
			}
		}

		public static void RefreshPages()
		{
			Pages.Clear();

			// Main Menu
			Pages.Add("MainMenu", new MainMenuPage());
			// Hero Creation Page
			Pages.Add("HeroCreation", new HeroCreationPage());
			// Town
			Pages.Add("Town", new TownPage());
			// Shop
			Pages.Add("Shop", new ShopPage());
			// Blacksmith
			Pages.Add("Blacksmith", new BlacksmithPage());
			// Priest Page
			Pages.Add("Priest", new PriestPage());
			// Quest Menu Page
			Pages.Add("QuestMenu", new QuestMenuPage());
			// Dungeon Select Page
			Pages.Add("DungeonSelect", new DungeonSelectPage());
			// Dungeon Boss Page
			Pages.Add("DungeonBoss", new DungeonBossPage());

			foreach (var region in Regions)
			{
				Pages.Add(region.Name, new RegionPage(region));
			}
		}
	}
}