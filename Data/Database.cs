using ClickQuest.Enemies;
using ClickQuest.Items;
using ClickQuest.Places;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClickQuest.Data
{
	public static partial class Database
	{
		public static List<Material> Materials { get; set; }
		public static List<Monster> Monsters { get; set; }
		public static List<Region> Regions { get; set; }

		public static void Load()
		{
			Materials = new List<Material>();
			Monsters = new List<Monster>();
			Regions = new List<Region>();

			LoadMaterials();
			LoadMonsters();
			LoadRegions();
		}

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
		public static void LoadMonsters()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Monsters.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Monsters"];

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

				var lootTemp = new List<(Material, double)>();
				var lootArray = (JArray)parsedObject["Monsters"][i]["Loot"];

				for (int j = 0; j < lootArray.Count; j++)
				{
					var materialId = int.Parse(parsedObject["Monsters"][i]["Loot"][j]["Id"].ToString());
					var material = Materials.Where(x => x.Id == materialId).FirstOrDefault();
					var frequency = Double.Parse(parsedObject["Monsters"][i]["Loot"][j]["Frequency"].ToString());

					lootTemp.Add((material, frequency));
				}

				var newMonster = new Monster(id, name, health, image, typesTemp, lootTemp);
				Monsters.Add(newMonster);
			}
		}
		public static void LoadRegions()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Regions.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Regions"];

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["Regions"][i]["Id"].ToString());
				var name = parsedObject["Regions"][i]["Name"].ToString();
				var background = parsedObject["Regions"][i]["Background"].ToString();

				var monstersTemp = new List<(Monster, Double)>();
				var monstersArray = (JArray)parsedObject["Regions"][i]["Monsters"];

				for (int j = 0; j < monstersArray.Count; j++)
				{
					var monsterId = int.Parse(parsedObject["Regions"][i]["Monsters"][j]["Id"].ToString());
					var monster = Monsters.Where(x => x.Id == monsterId).FirstOrDefault();

					var frequency = Double.Parse(parsedObject["Regions"][i]["Monsters"][j]["Frequency"].ToString());

					monstersTemp.Add((monster, frequency));
				}

				var newRegion = new Region(id, name, background, monstersTemp);
				Regions.Add(newRegion);
			}
		}
	}
}