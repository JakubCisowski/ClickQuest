using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClickQuest.Game.Core.Adventures;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Interfaces;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Places;

namespace ClickQuest.Game.Extensions.Validation
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
			var dataProperties = typeof(GameAssets).GetProperties();

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
			GameAssets.Monsters.ForEach(x => CheckFrequencySum(x.Name, x.MonsterLootPatterns.Select(y => y.Frequency)));
			GameAssets.Regions.ForEach(x => CheckFrequencySum(x.Name, x.Monsters.Select(y => y.Frequency)));
			GameAssets.Bosses.ForEach(x => x.BossLootPatterns.ForEach(y => CheckIfFrequenciesAreValid(x.Name, y.Frequencies)));
		}

		private static void CheckFrequencySum(string objectName, IEnumerable<double> frequencies)
		{
			double frequenciesSum = frequencies.Sum();
			if (frequenciesSum <= 0.9999 || frequenciesSum >= 1.0001) // <== cringe	
			{
				string message = $"'{objectName}' frequencies do not add up to 1";
				Logger.Log(message);
			}
		}

		private static void CheckIfFrequenciesAreValid(string objectName, IEnumerable<double> frequencies)
		{
			bool areFrequenciesInvalid = frequencies.Any(x => x < 0);

			if (areFrequenciesInvalid)
			{
				string message = $"'{objectName}' frequency value is less than 0";
				Logger.Log(message);
			}
		}

		private static void CheckReferencesCorrectness()
		{
			GameAssets.Recipes.ForEach(x => CheckReferencedIds(x.Name, GameAssets.Materials.Select(y => y.Id), x.Ingredients.Select(z => z.MaterialId)));
			GameAssets.Recipes.ForEach(x => CheckReferencedIds(x.Name, GameAssets.Artifacts.Select(y => y.Id), new[] {x.ArtifactId}));
			GameAssets.Dungeons.ForEach(x => CheckReferencedIds(x.Name, GameAssets.Bosses.Select(y => y.Id), x.BossIds));

			GameAssets.Bosses.ForEach(x => CheckBossReferences(x));
			GameAssets.Monsters.ForEach(x => CheckMonsterReferences(x));
			GameAssets.Regions.ForEach(x => CheckRegionReferences(x));
			GameAssets.Quests.ForEach(x => CheckQuestReferences(x));

			CheckReferencedIds("ShopOffer", GameAssets.Recipes.Select(x => x.Id), GameAssets.ShopOffer);
			CheckReferencedIds("PriestOffer", GameAssets.Blessings.Select(x => x.Id), GameAssets.PriestOffer);
		}

		private static void CheckQuestReferences(Quest quest)
		{
			CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Material)", GameAssets.Materials.Select(x => x.Id), quest.QuestRewardPatterns.Where(x=>x.QuestRewardType == RewardType.Material).Select(y=>y.QuestRewardId));
			CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Artifact)", GameAssets.Artifacts.Select(x => x.Id), quest.QuestRewardPatterns.Where(x=>x.QuestRewardType == RewardType.Artifact).Select(y=>y.QuestRewardId));
			CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Recipe)", GameAssets.Recipes.Select(x => x.Id), quest.QuestRewardPatterns.Where(x=>x.QuestRewardType == RewardType.Recipe).Select(y=>y.QuestRewardId));
			CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Blessing)", GameAssets.Blessings.Select(x => x.Id), quest.QuestRewardPatterns.Where(x=>x.QuestRewardType == RewardType.Blessing).Select(y=>y.QuestRewardId));
			CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Ingot)", GameAssets.Ingots.Select(x => x.Id), quest.QuestRewardPatterns.Where(x=>x.QuestRewardType == RewardType.Ingot).Select(y=>y.QuestRewardId));
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
			foreach (var lootPattern in boss.BossLootPatterns)
			{
				if (lootPattern.BossLootType == RewardType.Blessing)
				{
					var blessing = GameAssets.Blessings.FirstOrDefault(x => x.Id == lootPattern.BossLootId);

					if (blessing is null)
					{
						string message = $"Following referenced blessing Id's in '{boss.Name}' is not available: " + lootPattern.BossLootId;
						Logger.Log(message);
					}
				}
				else
				{
					var item = lootPattern.Item;

					if (item is null)
					{
						string message = $"Following referenced item Id's in '{boss.Name}' is not available: " + lootPattern.BossLootId;
						Logger.Log(message);
					}
				}
			}
		}

		private static void CheckMonsterReferences(Monster monster)
		{
			foreach (var lootPattern in monster.MonsterLootPatterns)
			{
				var item = lootPattern.Item;

				if (item is null)
				{
					string message = $"Following referenced item Id's in '{monster.Name}' is not available: " + lootPattern.MonsterLootId;
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
			CheckEnumCollectionBounds("Artifacts_Rarity", GameAssets.Artifacts.Select(x => x.Rarity));
			CheckEnumCollectionBounds("Blessings_Rarity", GameAssets.Blessings.Select(x => x.Rarity));
			CheckEnumCollectionBounds("DungeonKey_Rarity", GameAssets.DungeonKeys.Select(x => x.Rarity));
			CheckEnumCollectionBounds("Ingots_Rarity", GameAssets.Ingots.Select(x => x.Rarity));
			CheckEnumCollectionBounds("Materials_Rarity", GameAssets.Materials.Select(x => x.Rarity));
			CheckEnumCollectionBounds("Recipes_Rarity", GameAssets.Recipes.Select(x => x.Rarity));

			CheckEnumCollectionBounds("Quests_HeroClass", GameAssets.Quests.Select(x => x.HeroClass));

			CheckEnumCollectionBounds("Blessings_BlessingType", GameAssets.Blessings.Select(x => x.Type));

			GameAssets.DungeonGroups.ForEach(x => CheckEnumCollectionBounds("DungeonGroup", x.KeyRequirementRarities));
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
			CheckCollectionPositiveValues("Artifacts_Value", GameAssets.Artifacts.Select(x => x.Value));
			CheckCollectionPositiveValues("Blessings_Value", GameAssets.Blessings.Select(x => x.Value));
			CheckCollectionPositiveValues("DungeonKey_Value", GameAssets.DungeonKeys.Select(x => x.Value));
			CheckCollectionPositiveValues("Ingots_Value", GameAssets.Ingots.Select(x => x.Value));
			CheckCollectionPositiveValues("Materials_Value", GameAssets.Materials.Select(x => x.Value));
			CheckCollectionPositiveValues("Recipes_Value", GameAssets.Recipes.Select(x => x.Value));

			CheckCollectionPositiveValues("Blessings_Duration", GameAssets.Blessings.Select(x => x.Duration));
			CheckCollectionPositiveValues("Quests_Duration", GameAssets.Quests.Select(x => x.Duration));

			CheckCollectionPositiveValues("Bosses_Health", GameAssets.Bosses.Select(x => x.Health));
			CheckCollectionPositiveValues("Monsters_Health", GameAssets.Monsters.Select(x => x.Health));

			CheckCollectionPositiveValues("Blessings_Buff", GameAssets.Blessings.Select(x => x.Buff));
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
			var levelRequirements = GameAssets.Regions.Select(x => x.LevelRequirement);

			bool isEveryLevelRequirementValid = levelRequirements.All(x => x >= 0 && x <= 100);

			if (!isEveryLevelRequirementValid)
			{
				string message = "Level requirement of region is invalid";
				Logger.Log(message);
			}
		}
	}
}