using ClickQuest.Enemies;
using ClickQuest.Heroes;
using ClickQuest.Items;
using ClickQuest.Pages;
using ClickQuest.Places;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
		public static List<Quest> Quests {get;set;}
		public static Dictionary<string, Page> Pages { get; set; }

		#endregion

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
				newRecipe.UpdateRequirementsDescription();
				Recipes.Add(newRecipe);
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
				var health = int.Parse(parsedObject["Monsters"][i]["Health"].ToString());
				var image = parsedObject["Monsters"][i]["Image"].ToString();

				var typesTemp = new List<MonsterType>();
				var typeArray = (JArray)parsedObject["Monsters"][i]["Types"];

				for (int j = 0; j < typeArray.Count; j++)
				{
					var monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), parsedObject["Monsters"][i]["Types"][j].ToString());
					typesTemp.Add(monsterType);
				}

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

					var frequency = Double.Parse(parsedObject["Monsters"][i]["Loot"][j]["Frequency"].ToString());

					frequencySum += frequency;

					lootTemp.Add((item, ItemType.Material, frequency));
				}

				if (frequencySum != 1)
				{
					errorLog.Add($"Error: {name} - loot frequency sums up to {frequencySum} instead of 1.");
				}

				var newMonster = new Monster(id, name, health, image, typesTemp, lootTemp);
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
				var background = parsedObject["Regions"][i]["Background"].ToString();

				var monstersTemp = new List<(Monster, Double)>();
				var monstersArray = (JArray)parsedObject["Regions"][i]["Monsters"];

				// Error check
				double frequencySum = 0;

				for (int j = 0; j < monstersArray.Count; j++)
				{
					var monsterId = int.Parse(parsedObject["Regions"][i]["Monsters"][j]["Id"].ToString());
					var monster = Monsters.Where(x => x.Id == monsterId).FirstOrDefault();

					var frequency = Double.Parse(parsedObject["Regions"][i]["Monsters"][j]["Frequency"].ToString());

					frequencySum += frequency;

					monstersTemp.Add((monster, frequency));
				}

				if (frequencySum != 1)
				{
					errorLog.Add($"Error: {name} - monster frequency sums up to {frequencySum} instead of 1.");
				}

				var newRegion = new Region(id, name, background, monstersTemp, levelRequirement);
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

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["ShopOffer"][i]["RecipeId"].ToString());
				var recipe = Recipes.Where(x => x.Id == id).FirstOrDefault();

				ShopOffer.Add(recipe);
			}
		}

		public static void LoadPriestOffer()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "PriestOffer.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["PriestOffer"];

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["PriestOffer"][i]["BlessingId"].ToString());
				var blessing = Blessings.Where(x => x.Id == id).FirstOrDefault();

				PriestOffer.Add(blessing);
			}
		}
		public static void LoadQuests()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Quests.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Quests"];

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
				foreach(var BlessingId in jArrayBlessings)
				{
					blessingIds.Add(int.Parse(BlessingId.ToString()));
				}

				var jArrayRecipes = (JArray)parsedObject["Quests"][i]["RewardRecipeIds"];
				var recipeIds = new List<int>();
				foreach(var RecipeId in jArrayRecipes)
				{
					recipeIds.Add(int.Parse(RecipeId.ToString()));
				}

				var jArrayMaterials = (JArray)parsedObject["Quests"][i]["RewardMaterialIds"];
				var materialIds = new List<int>();
				foreach(var MaterialId in jArrayRecipes)
				{
					materialIds.Add(int.Parse(MaterialId.ToString()));
				}
				
				var jArrayIngots = (JArray)parsedObject["Quests"][i]["RewardIngotsIds"];
				var ingotRarities = new List<Rarity>();
				foreach(var IngotRarity in jArrayRecipes)
				{
					var actualIngotRarity = (Rarity)int.Parse(IngotRarity.ToString());
					ingotRarities.Add(actualIngotRarity);
				}

				var newQuest = new Quest(id, rarity,heroClass,name,duration,description)
				{
					RewardRecipeIds=recipeIds,
					RewardBlessingIds=blessingIds,
					RewardMaterialIds=materialIds,
					RewardIngots = ingotRarities
				};

				Quests.Add(newQuest);
			}
		}

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

			// Check if every recipe components' and artifact's IDs exist.
			foreach (var recipe in Recipes)
			{
				var artifact = Artifacts.FirstOrDefault(x => x.Id == recipe.ArtifactId);
				if (artifact is null)
				{
					errorLog.Add($"Error: Recipe Id: {recipe.Id} - There is no artifact with Id: {recipe.ArtifactId}");
				}

				foreach (var pair in recipe.MaterialIds)
				{
					var material = Materials.FirstOrDefault(x => x.Id == pair.Key);
					if (materials is null)
					{
						errorLog.Add($"Error: Recipe Id: {recipe.Id} - There is no material with Id: {pair.Key}");
					}

					if (pair.Value <= 0)
					{
						errorLog.Add($"Error: Recipe Id: {recipe.Id} - Invalid count of material with Id: {pair.Key}");
					}
				}
			}

			if (errorLog.Count > 0)
			{
				Logger.Log(errorLog);
			}
		}

		#endregion
		public static void RefreshPages()
		{
			Pages.Clear();

			// Main Menu
			Pages.Add("MainMenu", new MainMenuPage());
			// Town
			Pages.Add("Town", new TownPage());
			// Shop
			Pages.Add("Shop", new ShopPage());
			// Blacksmith
			Pages.Add("Blacksmith", new BlacksmithPage());
			// Hero Creation Page
			Pages.Add("HeroCreation", new HeroCreationPage());
			// Priest Page
			Pages.Add("Priest", new PriestPage());
			// Quest Menu Page
			Pages.Add("QuestMenu", new QuestMenuPage());

			foreach (var region in Regions)
			{
				Pages.Add(region.Name, new RegionPage(region));
			}
		}
	}
}