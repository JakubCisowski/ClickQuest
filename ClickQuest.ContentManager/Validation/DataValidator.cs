using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.GameData.Models;

namespace ClickQuest.ContentManager.Validation;

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
		CheckRewardBlessingsQuantity();
		CheckRecipeArtifactLinks();
		CheckMonsterRewardTypes();
		CheckEmptyCollections();
	}

	private static void CheckIdUniqueness()
	{
		var dataProperties = typeof(GameContent).GetProperties();

		foreach (var propertyInfo in dataProperties)
		{
			var propertyValue = propertyInfo.GetValue(null) as IEnumerable;
			var collectionType = propertyInfo.PropertyType.GetGenericArguments().FirstOrDefault();

			var isCollectionIdentifiable = typeof(IIdentifiable).IsAssignableFrom(collectionType);
			if (isCollectionIdentifiable)
			{
				var convertedCollection = propertyValue.Cast<IIdentifiable>().ToList();
				var methodInfo = typeof(DataValidator).GetMethod("CheckCollectionIdUniqueness");
				methodInfo.Invoke(null, new object[] { convertedCollection, collectionType });
			}
		}
	}

	public static void CheckCollectionIdUniqueness(List<IIdentifiable> collection, Type collectionType)
	{
		var idList = collection.Select(x => x.Id);

		var duplicates = idList.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();

		var areIdsUnique = duplicates.Count == 0;
		if (!areIdsUnique)
		{
			var message = $"> Following Id's of type '{collectionType}' are not unique: ";

			var duplicatedValues = duplicates.Distinct();
			foreach (var value in duplicatedValues)
			{
				message += value + " ";
			}

			Logger.Log(message);
		}
	}

	private static void CheckAllFrequenciesCorrectness()
	{
		GameContent.Monsters.ForEach(x => CheckFrequencySumLesserThanOne(x.Name, x.MonsterLootPatterns.Select(y => y.Frequency)));
		GameContent.Regions.ForEach(x => CheckFrequencySumEqualToOne(x.Name, x.MonsterSpawnPatterns.Select(y => y.Frequency)));
		GameContent.Bosses.ForEach(x => x.BossLootPatterns.ForEach(y => CheckIfFrequenciesAreValid(x.Name, y.Frequencies)));
	}

	private static void CheckFrequencySumEqualToOne(string objectName, IEnumerable<double> frequencies)
	{
		var frequenciesSum = frequencies.Sum();
		if (frequenciesSum <= 0.9999 || frequenciesSum >= 1.0001) // <== cringe	
		{
			var message = $"> {objectName} loot/spawn frequencies do not add up to 1";
			Logger.Log(message);
		}
	}

	private static void CheckFrequencySumLesserThanOne(string objectName, IEnumerable<double> frequencies)
	{
		var frequenciesSum = frequencies.Sum();

		if (frequenciesSum >= 1.0001)
		{
			var message = $"> {objectName} loot/spawn frequencies is greater than 1";
			Logger.Log(message);
		}
	}

	private static void CheckIfFrequenciesAreValid(string objectName, IEnumerable<double> frequencies)
	{
		var areFrequenciesInvalid = frequencies.Any(x => x < 0);

		if (areFrequenciesInvalid)
		{
			var message = $"> {objectName} loot/spawn frequency value is less than 0";
			Logger.Log(message);
		}
	}

	private static void CheckReferencesCorrectness()
	{
		GameContent.Recipes.ForEach(x => CheckReferencedIds(x.Name, GameContent.Materials.Select(y => y.Id), x.IngredientPatterns.Select(z => z.MaterialId)));
		GameContent.Recipes.ForEach(x => CheckReferencedIds(x.Name, GameContent.Artifacts.Select(y => y.Id), new[] { x.ArtifactId }));
		GameContent.Dungeons.ForEach(x => CheckReferencedIds(x.Name, GameContent.Bosses.Select(y => y.Id), x.BossIds));

		GameContent.Bosses.ForEach(x => CheckBossReferences(x));
		GameContent.Monsters.ForEach(x => CheckMonsterReferences(x));
		GameContent.Regions.ForEach(x => CheckRegionReferences(x));
		GameContent.Quests.ForEach(x => CheckQuestReferences(x));

		CheckReferencedIds("ShopOffer", GameContent.Recipes.Select(x => x.Id), GameContent.ShopOffer.Select(x => x.VendorItemId));
		CheckReferencedIds("PriestOffer", GameContent.Blessings.Select(x => x.Id), GameContent.PriestOffer.Select(x => x.VendorItemId));
	}

	private static void CheckQuestReferences(Quest quest)
	{
		CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Material)", GameContent.Materials.Select(x => x.Id), quest.QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Material).Select(y => y.QuestRewardId));
		CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Artifact)", GameContent.Artifacts.Select(x => x.Id), quest.QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Artifact).Select(y => y.QuestRewardId));
		CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Recipe)", GameContent.Recipes.Select(x => x.Id), quest.QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Recipe).Select(y => y.QuestRewardId));
		CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Blessing)", GameContent.Blessings.Select(x => x.Id), quest.QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Blessing).Select(y => y.QuestRewardId));
		CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Ingot)", GameContent.Ingots.Select(x => x.Id), quest.QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Ingot).Select(y => y.QuestRewardId));
	}

	private static void CheckReferencedIds(string objectName, IEnumerable<int> availableIds, IEnumerable<int> requiredIds)
	{
		var notAvailable = requiredIds.Except(availableIds);
		if (notAvailable.Count() > 0)
		{
			var message = $"> Following referenced Id's in '{objectName}' do not exist: ";

			foreach (var value in notAvailable)
			{
				message += value + " ";
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
				var blessing = GameContent.Blessings.FirstOrDefault(x => x.Id == lootPattern.BossLootId);

				if (blessing is null)
				{
					var message = $"> Following referenced blessing Id's in '{boss.Name}' do not exist: " + lootPattern.BossLootId;
					Logger.Log(message);
				}
			}
			else
			{
				var item = lootPattern.Item;

				if (item is null)
				{
					var message = $"> Following referenced item Id's in '{boss.Name}' do not exist: " + lootPattern.BossLootId;
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
				var message = $"> Following referenced item Id's in '{monster.Name}' do not exist: " + lootPattern.MonsterLootId;
				Logger.Log(message);
			}
		}
	}

	private static void CheckRegionReferences(Region region)
	{
		foreach (var spawnPattern in region.MonsterSpawnPatterns)
		{
			var monster = spawnPattern.Monster;

			if (monster is null)
			{
				var message = $"> Following referenced item Id's in '{region.Name}' do not exist: " + spawnPattern.MonsterId;
				Logger.Log(message);
			}
		}
	}

	private static void CheckEnumBounds()
	{
		CheckEnumCollectionBounds("Artifacts_Rarity", GameContent.Artifacts.Select(x => x.Rarity));
		CheckEnumCollectionBounds("Blessings_Rarity", GameContent.Blessings.Select(x => x.Rarity));
		CheckEnumCollectionBounds("DungeonKey_Rarity", GameContent.DungeonKeys.Select(x => x.Rarity));
		CheckEnumCollectionBounds("Ingots_Rarity", GameContent.Ingots.Select(x => x.Rarity));
		CheckEnumCollectionBounds("Materials_Rarity", GameContent.Materials.Select(x => x.Rarity));
		CheckEnumCollectionBounds("Recipes_Rarity", GameContent.Recipes.Select(x => x.Rarity));

		CheckEnumCollectionBounds("Quests_HeroClass", GameContent.Quests.Select(x => x.HeroClass));

		CheckEnumCollectionBounds("Blessings_BlessingType", GameContent.Blessings.Select(x => x.Type));

		GameContent.DungeonGroups.ForEach(x => CheckEnumCollectionBounds("DungeonGroup", x.KeyRequirementRarities));
	}

	private static void CheckEnumCollectionBounds<T>(string collectionName, IEnumerable<T> enumCollection) where T : Enum
	{
		var availableEnumValues = Enum.GetValues(typeof(T)).OfType<T>().ToList();
		var isEveryEnumValueInCollectionValid = enumCollection.All(x => availableEnumValues.Contains(x));

		if (!isEveryEnumValueInCollectionValid)
		{
			var message = $"> '{collectionName}' - rarity out of scope";
			Logger.Log(message);
		}
	}

	private static void CheckPositiveValues()
	{
		CheckCollectionPositiveValues("Blessings_Value", GameContent.Blessings.Select(x => x.Value));
		CheckCollectionPositiveValues("DungeonKey_Value", GameContent.DungeonKeys.Select(x => x.Value));
		CheckCollectionPositiveValues("Ingots_Value", GameContent.Ingots.Select(x => x.Value));
		CheckCollectionPositiveValues("Materials_Value", GameContent.Materials.Select(x => x.Value));
		CheckCollectionPositiveValues("Recipes_Value", GameContent.Recipes.Select(x => x.Value));

		CheckCollectionPositiveValues("Blessings_Duration", GameContent.Blessings.Select(x => x.Duration));
		CheckCollectionPositiveValues("Quests_Duration", GameContent.Quests.Select(x => x.Duration));

		CheckCollectionPositiveValues("Bosses_Health", GameContent.Bosses.Select(x => x.Health));
		CheckCollectionPositiveValues("Monsters_Health", GameContent.Monsters.Select(x => x.Health));

		CheckCollectionPositiveValues("Blessings_Buff", GameContent.Blessings.Select(x => x.Buff));

		CheckCollectionPositiveValues("QuestRewardPatterns_Quantity", GameContent.Quests.SelectMany(x => x.QuestRewardPatterns).Select(y => y.Quantity));
		CheckCollectionPositiveValues("IngredientPatterns_Quantity", GameContent.Recipes.SelectMany(x => x.IngredientPatterns).Select(y => y.Quantity));
	}

	private static void CheckEmptyCollections()
	{
		for (var i = 0; i < GameContent.Recipes.Count; i++)
		{
			if (GameContent.Recipes[i].IngredientPatterns.Count == 0)
			{
				var message = $"> Recipe of id {GameContent.Recipes[i].Id} has no ingredients";
				Logger.Log(message);
			}
		}
	}

	private static void CheckCollectionPositiveValues(string collectionValuesInfo, IEnumerable<int> valuesCollection)
	{
		var isEveryValueValid = valuesCollection.All(x => x > 0);

		if (!isEveryValueValid)
		{
			var message = $"> At least one {collectionValuesInfo} is nonpositive";
			Logger.Log(message);
		}
	}

	private static void CheckLevelRequirements()
	{
		var levelRequirements = GameContent.Regions.Select(x => x.LevelRequirement);

		var isEveryLevelRequirementValid = levelRequirements.All(x => x >= 0 && x <= 100);

		if (!isEveryLevelRequirementValid)
		{
			var message = "> Level requirement of region is invalid";
			Logger.Log(message);
		}
	}

	private static void CheckRewardBlessingsQuantity()
	{
		foreach (var boss in GameContent.Bosses)
		{
			var blessingPatterns = boss.BossLootPatterns.Where(pattern => pattern.BossLootType == RewardType.Blessing);

			if (blessingPatterns.Any(pattern => pattern.Frequencies.Any(frequency => frequency > 1)))
			{
				var message = $"> More than one of the same Blessing can be dropped from Boss of Id {boss.Id}";
				Logger.Log(message);
			}

			if (blessingPatterns.Count() > 1)
			{
				var message = $"> More than one Blessing can be dropped from Boss of Id {boss.Id}";
				Logger.Log(message);
			}
		}

		foreach (var quest in GameContent.Quests)
		{
			var blessingPatterns = quest.QuestRewardPatterns.Where(pattern => pattern.QuestRewardType == RewardType.Blessing);

			if (blessingPatterns.Any(pattern => pattern.Quantity > 1))
			{
				var message = $"> More than one of the same Blessing is awarded from Quest of Id {quest.Id}";
				Logger.Log(message);
			}

			if (blessingPatterns.Count() > 1)
			{
				var message = $"> More than one Blessing is awarded from Quest of Id {quest.Id}";
				Logger.Log(message);
			}
		}
	}

	private static void CheckRecipeArtifactLinks()
	{
		foreach (var recipe in GameContent.Recipes)
		{
			var artifact = GameContent.Artifacts.FirstOrDefault(x => x.Id == recipe.ArtifactId);

			if (!recipe.Name.Contains(artifact.Name))
			{
				var message = $"> {recipe.FullName} of Id: {recipe.Id} is not linked to the correct artifact ({artifact.Name} instead)";
				Logger.Log(message);
			}
		}
	}

	private static void CheckMonsterRewardTypes()
	{
		foreach (var monster in GameContent.Monsters)
		{
			var monsterBlessingPatterns = monster.MonsterLootPatterns.Where(pattern => pattern.MonsterLootType == RewardType.Blessing);

			if (monsterBlessingPatterns.Count() != 0)
			{
				var message = $"> Blessing can be dropped from Monster of Id {monster.Id} (blessing drops from monsters are not handled)";
				Logger.Log(message);
			}
		}
	}
}