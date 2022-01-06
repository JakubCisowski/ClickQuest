using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ClickQuest.ContentManager.Logic.DataTypes.Enums;
using ClickQuest.ContentManager.Logic.Models;
using ClickQuest.ContentManager.Logic.Models.Interfaces;

namespace ClickQuest.ContentManager.Logic.Helpers;

public static class ValidationHelper
{
	public static DateTime SessionStartDate = DateTime.Now;

	public static void Log(string log)
	{
		var folderPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.ToString(), "Logs");

		if (!Directory.Exists(folderPath))
		{
			Directory.CreateDirectory(folderPath);
		}

		var fullPath = Path.Combine(folderPath, "Logs " + SessionStartDate.ToString("dd-MM-yyyy-HH-mm-ss") + ".txt");
		var isFirstLogInThisFile = !File.Exists(fullPath);

		// Log bugs in specified format.
		using var writer = new StreamWriter(fullPath, true);

		if (isFirstLogInThisFile)
		{
			writer.WriteLine($"ASSETS LOGS - {SessionStartDate.ToString("dd.MM.yyyy - HH:mm:ss")}\n");
		}

		writer.WriteLine(log);
	}
	
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
		var dataProperties = typeof(GameAssets).GetProperties();

		foreach (var propertyInfo in dataProperties)
		{
			var propertyValue = propertyInfo.GetValue(null) as IEnumerable;
			var collectionType = propertyInfo.PropertyType.GetGenericArguments().FirstOrDefault();

			var isCollectionIdentifiable = typeof(IIdentifiable).IsAssignableFrom(collectionType);
			if (isCollectionIdentifiable)
			{
				var convertedCollection = propertyValue.Cast<IIdentifiable>().ToList();
				var methodInfo = typeof(ValidationHelper).GetMethod("CheckCollectionIdUniqueness");
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

			Log(message);
		}
	}

	private static void CheckAllFrequenciesCorrectness()
	{
		GameAssets.Monsters.ForEach(x => CheckFrequencySumLesserThanOne(x.Name, x.MonsterLootPatterns.Select(y => y.Frequency)));
		GameAssets.Regions.ForEach(x => CheckFrequencySumEqualToOne(x.Name, x.MonsterSpawnPatterns.Select(y => y.Frequency)));
		GameAssets.Bosses.ForEach(x => x.BossLootPatterns.ForEach(y => CheckIfFrequenciesAreValid(x.Name, y.Frequencies)));
	}

	private static void CheckFrequencySumEqualToOne(string objectName, IEnumerable<double> frequencies)
	{
		var frequenciesSum = frequencies.Sum();
		if (frequenciesSum <= 0.9999 || frequenciesSum >= 1.0001) // <== cringe	
		{
			var message = $"> {objectName} loot/spawn frequencies do not add up to 1";
			Log(message);
		}
	}

	private static void CheckFrequencySumLesserThanOne(string objectName, IEnumerable<double> frequencies)
	{
		var frequenciesSum = frequencies.Sum();

		if (frequenciesSum >= 1.0001)
		{
			var message = $"> {objectName} loot/spawn frequencies is greater than 1";
			Log(message);
		}
	}

	private static void CheckIfFrequenciesAreValid(string objectName, IEnumerable<double> frequencies)
	{
		var areFrequenciesInvalid = frequencies.Any(x => x < 0);

		if (areFrequenciesInvalid)
		{
			var message = $"> {objectName} loot/spawn frequency value is less than 0";
			Log(message);
		}
	}

	private static void CheckReferencesCorrectness()
	{
		GameAssets.Recipes.ForEach(x => CheckReferencedIds(x.Name, GameAssets.Materials.Select(y => y.Id), x.IngredientPatterns.Select(z => z.MaterialId)));
		GameAssets.Recipes.ForEach(x => CheckReferencedIds(x.Name, GameAssets.Artifacts.Select(y => y.Id), new[] { x.ArtifactId }));
		GameAssets.Dungeons.ForEach(x => CheckReferencedIds(x.Name, GameAssets.Bosses.Select(y => y.Id), x.BossIds));

		GameAssets.Bosses.ForEach(x => CheckBossReferences(x));
		GameAssets.Monsters.ForEach(x => CheckMonsterReferences(x));
		GameAssets.Regions.ForEach(x => CheckRegionReferences(x));
		GameAssets.Quests.ForEach(x => CheckQuestReferences(x));

		CheckReferencedIds("ShopOffer", GameAssets.Recipes.Select(x => x.Id), GameAssets.ShopOffer.Select(x => x.VendorItemId));
		CheckReferencedIds("PriestOffer", GameAssets.Blessings.Select(x => x.Id), GameAssets.PriestOffer.Select(x => x.VendorItemId));
	}

	private static void CheckQuestReferences(Quest quest)
	{
		CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Material)", GameAssets.Materials.Select(x => x.Id), quest.QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Material).Select(y => y.QuestRewardId));
		CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Artifact)", GameAssets.Artifacts.Select(x => x.Id), quest.QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Artifact).Select(y => y.QuestRewardId));
		CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Recipe)", GameAssets.Recipes.Select(x => x.Id), quest.QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Recipe).Select(y => y.QuestRewardId));
		CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Blessing)", GameAssets.Blessings.Select(x => x.Id), quest.QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Blessing).Select(y => y.QuestRewardId));
		CheckReferencedIds($"{quest.Name}.QuestRewardPatterns(Ingot)", GameAssets.Ingots.Select(x => x.Id), quest.QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Ingot).Select(y => y.QuestRewardId));
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

			Log(message);
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
					var message = $"> Following referenced blessing Id's in '{boss.Name}' do not exist: " + lootPattern.BossLootId;
					Log(message);
				}
			}
			else
			{
				var item = lootPattern.Item;

				if (item is null)
				{
					var message = $"> Following referenced item Id's in '{boss.Name}' do not exist: " + lootPattern.BossLootId;
					Log(message);
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
				Log(message);
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
				Log(message);
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
		var isEveryEnumValueInCollectionValid = enumCollection.All(x => availableEnumValues.Contains(x));

		if (!isEveryEnumValueInCollectionValid)
		{
			var message = $"> '{collectionName}' - rarity out of scope";
			Log(message);
		}
	}

	private static void CheckPositiveValues()
	{
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

		CheckCollectionPositiveValues("QuestRewardPatterns_Quantity", GameAssets.Quests.SelectMany(x => x.QuestRewardPatterns).Select(y => y.Quantity));
		CheckCollectionPositiveValues("IngredientPatterns_Quantity", GameAssets.Recipes.SelectMany(x => x.IngredientPatterns).Select(y => y.Quantity));
	}

	private static void CheckEmptyCollections()
	{
		for (var i = 0; i < GameAssets.Recipes.Count; i++)
		{
			if (GameAssets.Recipes[i].IngredientPatterns.Count == 0)
			{
				var message = $"> Recipe of id {GameAssets.Recipes[i].Id} has no ingredients";
				Log(message);
			}
		}
	}

	private static void CheckCollectionPositiveValues(string collectionValuesInfo, IEnumerable<int> valuesCollection)
	{
		var isEveryValueValid = valuesCollection.All(x => x > 0);

		if (!isEveryValueValid)
		{
			var message = $"> At least one {collectionValuesInfo} is nonpositive";
			Log(message);
		}
	}

	private static void CheckLevelRequirements()
	{
		var levelRequirements = GameAssets.Regions.Select(x => x.LevelRequirement);

		var isEveryLevelRequirementValid = levelRequirements.All(x => x >= 0 && x <= 100);

		if (!isEveryLevelRequirementValid)
		{
			var message = "> Level requirement of region is invalid";
			Log(message);
		}
	}

	private static void CheckRewardBlessingsQuantity()
	{
		foreach (var boss in GameAssets.Bosses)
		{
			var blessingPatterns = boss.BossLootPatterns.Where(pattern => pattern.BossLootType == RewardType.Blessing);

			if (blessingPatterns.Any(pattern => pattern.Frequencies.Any(frequency => frequency > 1)))
			{
				var message = $"> More than one of the same Blessing can be dropped from Boss of Id {boss.Id}";
				Log(message);
			}

			if (blessingPatterns.Count() > 1)
			{
				var message = $"> More than one Blessing can be dropped from Boss of Id {boss.Id}";
				Log(message);
			}
		}

		foreach (var quest in GameAssets.Quests)
		{
			var blessingPatterns = quest.QuestRewardPatterns.Where(pattern => pattern.QuestRewardType == RewardType.Blessing);

			if (blessingPatterns.Any(pattern => pattern.Quantity > 1))
			{
				var message = $"> More than one of the same Blessing is awarded from Quest of Id {quest.Id}";
				Log(message);
			}

			if (blessingPatterns.Count() > 1)
			{
				var message = $"> More than one Blessing is awarded from Quest of Id {quest.Id}";
				Log(message);
			}
		}
	}

	private static void CheckRecipeArtifactLinks()
	{
		foreach (var recipe in GameAssets.Recipes)
		{
			var artifact = GameAssets.Artifacts.FirstOrDefault(x => x.Id == recipe.ArtifactId);

			if (!recipe.Name.Contains(artifact.Name))
			{
				var message = $"> {recipe.FullName} of Id: {recipe.Id} is not linked to the correct artifact ({artifact.Name} instead)";
				Log(message);
			}
		}
	}

	private static void CheckMonsterRewardTypes()
	{
		foreach (var monster in GameAssets.Monsters)
		{
			var monsterBlessingPatterns = monster.MonsterLootPatterns.Where(pattern => pattern.MonsterLootType == RewardType.Blessing);

			if (monsterBlessingPatterns.Count() != 0)
			{
				var message = $"> Blessing can be dropped from Monster of Id {monster.Id} (blessing drops from monsters are not handled)";
				Log(message);
			}
		}
	}
}