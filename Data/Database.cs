using ClickQuest.Enemies;
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
		public static List<Recipe> ShopOffer { get; set; }
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
			ShopOffer = new List<Recipe>();
			Pages = new Dictionary<string, Page>();

			// Fill collections with JSON data.
			LoadMaterials();
			LoadRecipes();
			LoadArtifacts();
			LoadMonsters();
			LoadRegions();
			LoadShopOffer();

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
				var value = int.Parse(parsedObject["Materials"][i]["Value"].ToString());

				var newMaterial = new Material(id, name, rarity, value);
				Materials.Add(newMaterial);
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
				var name = parsedObject["Recipes"][i]["Name"].ToString();
				var rarity = (Rarity)int.Parse(parsedObject["Recipes"][i]["Rarity"].ToString());
				var value = int.Parse(parsedObject["Recipes"][i]["Value"].ToString());

				var newRecipe = new Recipe(id, name, rarity, value);
				Recipes.Add(newRecipe);
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
				var value = int.Parse(parsedObject["Artifacts"][i]["Value"].ToString());

				var newArtifact = new Artifact(id, name, rarity, value);
				Artifacts.Add(newArtifact);
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

				var newRegion = new Region(id, name, background, monstersTemp);
				Regions.Add(newRegion);
			}

			if (errorLog.Count > 0)
			{
				Logger.Log(errorLog);
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

		#endregion
		public static void RefreshPages()
		{
			Pages.Clear();

			// Town
			Pages.Add("Town", new TownPage());
			// Shop
			Pages.Add("Shop", new ShopPage());
			// Regions
			foreach (var region in Regions)
			{
				Pages.Add(region.Name, new RegionPage(region));
			}
		}
	}
}