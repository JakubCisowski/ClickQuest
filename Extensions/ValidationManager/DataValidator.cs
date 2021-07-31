using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClickQuest.Adventures;
using ClickQuest.Data;
using ClickQuest.Enemies;
using ClickQuest.Interfaces;
using ClickQuest.Places;

namespace ClickQuest.Extensions.ValidationManager
{
	public static class DataValidator
	{
		public static void ValidateData()
		{
			CheckIdUniqueness();
			CheckAllFrequenciesCorrectness();
			CheckReferencesCorrectness();
			CheckEnumBounds();
			CheckPositiveValues();
			CheckLevelRequirements();
		}

		private static void CheckIdUniqueness()
		{
			var dataProperties = typeof(GameData).GetProperties();

			foreach (var propertyInfo in dataProperties)
			{
				var propertyValue = propertyInfo.GetValue(null) as IEnumerable;
				var collectionType = propertyInfo.PropertyType.GetGenericArguments().FirstOrDefault();

				bool isCollectionIdentifiable = typeof(IIdentifiable).IsAssignableFrom(collectionType);
				if (isCollectionIdentifiable)
				{
					var convertedCollection = propertyValue.Cast<IIdentifiable>().ToList();
					var methodInfo = typeof(DataValidator).GetMethod("CheckCollectionIdUniqueness");
					methodInfo.Invoke(null, new object[] {convertedCollection, collectionType});
				}
			}
		}

		public static void CheckCollectionIdUniqueness(List<IIdentifiable> collection, Type collectionType)
		{
			var idList = collection.Select(x => x.Id);

			var duplicates = idList.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();

			bool areIdsUnique = duplicates.Count == 0;
			if (!areIdsUnique)
			{
				string message = $"Following Id's of type '{collectionType}' are not unique: ";

				var duplicatedValues = duplicates.Distinct();
				foreach (int value in duplicatedValues)
				{
					message += value + ", ";
				}

				Logger.Log(message);
			}
		}

		private static void CheckAllFrequenciesCorrectness()
		{
			GameData.Monsters.ForEach(x => CheckFrequencySum(x.Name, x.Loot.Select(y => y.Frequency)));
			GameData.Regions.ForEach(x => CheckFrequencySum(x.Name, x.Monsters.Select(y => y.Frequency)));
			GameData.Bosses.ForEach(x => x.BossLoot.ForEach(y => CheckIfFrequenciesAreValid(x.Name, y.Frequencies)));
		}

		private static void CheckFrequencySum(string objectName, IEnumerable<double> frequencies)
		{
			double frequenciesSum = frequencies.Sum();
			if (frequenciesSum != 1)
			{
				string message = $"'{objectName}' frequencies do not add up to 1";
				Logger.Log(message);
			}
		}

		private static void CheckIfFrequenciesAreValid(string objectName, IEnumerable<double> frequencies)
		{
			bool areFrequenciesInvalid = frequencies.Any(x => x < 0 || x > 1);

			if (areFrequenciesInvalid)
			{
				string message = $"'{objectName}' frequency value is greater than 1";
				Logger.Log(message);
			}
		}

		private static void CheckReferencesCorrectness()
		{
			GameData.Recipes.ForEach(x => CheckReferencedIds(x.Name, GameData.Materials.Select(y => y.Id), x.Ingredients.Select(z => z.Id)));
			GameData.Recipes.ForEach(x => CheckReferencedIds(x.Name, GameData.Artifacts.Select(y => y.Id), new[] {x.ArtifactId}));
			GameData.Dungeons.ForEach(x => CheckReferencedIds(x.Name, GameData.Bosses.Select(y => y.Id), x.BossIds));

			GameData.Bosses.ForEach(x => CheckBossReferences(x));
			GameData.Monsters.ForEach(x => CheckMonsterReferences(x));
			GameData.Regions.ForEach(x => CheckRegionReferences(x));
			GameData.Quests.ForEach(x => CheckQuestReferences(x));

			CheckReferencedIds("ShopOffer", GameData.Recipes.Select(x => x.Id), GameData.ShopOffer);
			CheckReferencedIds("PriestOffer", GameData.Blessings.Select(x => x.Id), GameData.PriestOffer);
		}

		private static void CheckQuestReferences(Quest quest)
		{
			CheckReferencedIds($"{quest.Name}.RewardMaterialIds", GameData.Materials.Select(x => x.Id), quest.RewardMaterialIds);
			CheckReferencedIds($"{quest.Name}.RewardRecipeIds", GameData.Recipes.Select(x => x.Id), quest.RewardRecipeIds);
			CheckReferencedIds($"{quest.Name}.RewardBlesingIds", GameData.Blessings.Select(x => x.Id), quest.RewardBlessingIds);
			CheckReferencedIds($"{quest.Name}.RewardIngotIds", GameData.Ingots.Select(x => x.Id), quest.RewardIngotIds);
		}

		private static void CheckReferencedIds(string objectName, IEnumerable<int> availableIds, IEnumerable<int> requiredIds)
		{
			var notAvailable = requiredIds.Except(availableIds);
			if (notAvailable.Count() > 0)
			{
				string message = $"Following referenced Id's in '{objectName}' are not available: ";

				foreach (int value in notAvailable)
				{
					message += value + ", ";
				}

				Logger.Log(message);
			}
		}

		private static void CheckBossReferences(Boss boss)
		{
			foreach (var lootPattern in boss.BossLoot)
			{
				var item = lootPattern.Item;

				if (item is null)
				{
					string message = $"Following referenced item Id's in '{boss.Name}' is not available: " + lootPattern.ItemId;
					Logger.Log(message);
				}
			}
		}

		private static void CheckMonsterReferences(Monster monster)
		{
			foreach (var lootPattern in monster.Loot)
			{
				var item = lootPattern.Item;

				if (item is null)
				{
					string message = $"Following referenced item Id's in '{monster.Name}' is not available: " + lootPattern.ItemId;
					Logger.Log(message);
				}
			}
		}

		private static void CheckRegionReferences(Region region)
		{
			foreach (var spawnPattern in region.Monsters)
			{
				var monster = spawnPattern.Monster;

				if (monster is null)
				{
					string message = $"Following referenced item Id's in '{region.Name}' is not available: " + spawnPattern.MonsterId;
					Logger.Log(message);
				}
			}
		}

		private static void CheckEnumBounds()
		{
			CheckEnumCollectionBounds("Artifacts_Rarity", GameData.Artifacts.Select(x => x.Rarity));
			CheckEnumCollectionBounds("Blessings_Rarity", GameData.Blessings.Select(x => x.Rarity));
			CheckEnumCollectionBounds("DungeonKey_Rarity", GameData.DungeonKeys.Select(x => x.Rarity));
			CheckEnumCollectionBounds("Ingots_Rarity", GameData.Ingots.Select(x => x.Rarity));
			CheckEnumCollectionBounds("Materials_Rarity", GameData.Materials.Select(x => x.Rarity));
			CheckEnumCollectionBounds("Recipes_Rarity", GameData.Recipes.Select(x => x.Rarity));

			CheckEnumCollectionBounds("Quests_HeroClass", GameData.Quests.Select(x => x.HeroClass));

			CheckEnumCollectionBounds("Blessings_BlessingType", GameData.Blessings.Select(x => x.Type));

			GameData.DungeonGroups.ForEach(x => CheckEnumCollectionBounds("DungeonGroup", x.KeyRequirementRarities));
		}

		private static void CheckEnumCollectionBounds<T>(string collectionName, IEnumerable<T> enumCollection) where T : Enum
		{
			var availableEnumValues = Enum.GetValues(typeof(T)).OfType<T>().ToList();
			bool isEveryEnumValueInCollectionValid = enumCollection.All(x => availableEnumValues.Contains(x));

			if (!isEveryEnumValueInCollectionValid)
			{
				string message = $"'{collectionName}' - rarity out of scope";
				Logger.Log(message);
			}
		}

		private static void CheckPositiveValues()
		{
			CheckCollectionPositiveValues("Artifacts_Value", GameData.Artifacts.Select(x => x.Value));
			CheckCollectionPositiveValues("Blessings_Value", GameData.Blessings.Select(x => x.Value));
			CheckCollectionPositiveValues("DungeonKey_Value", GameData.DungeonKeys.Select(x => x.Value));
			CheckCollectionPositiveValues("Ingots_Value", GameData.Ingots.Select(x => x.Value));
			CheckCollectionPositiveValues("Materials_Value", GameData.Materials.Select(x => x.Value));
			CheckCollectionPositiveValues("Recipes_Value", GameData.Recipes.Select(x => x.Value));

			CheckCollectionPositiveValues("Blessings_Duration", GameData.Blessings.Select(x => x.Duration));
			CheckCollectionPositiveValues("Quests_Duration", GameData.Quests.Select(x => x.Duration));

			CheckCollectionPositiveValues("Bosses_Health", GameData.Bosses.Select(x => x.Health));
			CheckCollectionPositiveValues("Monsters_Health", GameData.Monsters.Select(x => x.Health));

			CheckCollectionPositiveValues("Blessings_Buff", GameData.Blessings.Select(x => x.Buff));
		}

		private static void CheckCollectionPositiveValues(string collectionValuesInfo, IEnumerable<int> valuesCollection)
		{
			bool isEveryValueValid = valuesCollection.All(x => x > 0);

			if (!isEveryValueValid)
			{
				string message = $"'{collectionValuesInfo}' is nonpositive";
				Logger.Log(message);
			}
		}

		private static void CheckLevelRequirements()
		{
			var levelRequirements = GameData.Regions.Select(x => x.LevelRequirement);

			bool isEveryLevelRequirementValid = levelRequirements.All(x => x >= 0 && x <= 100);

			if (!isEveryLevelRequirementValid)
			{
				string message = "Level requirement of region is invalid";
				Logger.Log(message);
			}
		}
	}
}