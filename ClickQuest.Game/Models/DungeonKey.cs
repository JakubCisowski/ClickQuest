using System.Collections.Generic;
using System.Windows;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Windows;

namespace ClickQuest.Game.Models;

public class DungeonKey : Item
{
	public override void AddAchievementProgress(int amount = 1)
	{
		NumericAchievementType achievementType = 0;
		// Increase achievement amount.
		switch (Rarity)
		{
			case Rarity.General:
				achievementType = NumericAchievementType.GeneralDungeonKeysEarned;
				break;
			case Rarity.Fine:
				achievementType = NumericAchievementType.FineDungeonKeysEarned;
				break;
			case Rarity.Superior:
				achievementType = NumericAchievementType.SuperiorDungeonKeysEarned;
				break;
			case Rarity.Exceptional:
				achievementType = NumericAchievementType.ExceptionalDungeonKeysEarned;
				break;
			case Rarity.Mythic:
				achievementType = NumericAchievementType.MythicDungeonKeysEarned;
				break;
			case Rarity.Masterwork:
				achievementType = NumericAchievementType.MasterworkDungeonKeysEarned;
				break;
		}

		User.Instance.Achievements.IncreaseAchievementValue(achievementType, amount);
	}

	public override DungeonKey CopyItem(int quantity)
	{
		var copy = new DungeonKey
		{
			Id = Id,
			Name = Name,
			Rarity = Rarity,
			Value = Value,
			Description = Description,
			Quantity = quantity
		};

		return copy;
	}

	public override void AddItem(int amount = 1, bool displayFloatingText = true)
	{
		CollectionsHelper.AddItemToCollection(this, User.Instance.DungeonKeys, amount);

		if (amount != 0)
		{
			(Application.Current.MainWindow as GameWindow).CreateFloatingTextUtility($"+{amount}", ColorsHelper.GetRarityColor(Rarity), FloatingTextHelper.GetDungeonKeyRarityPosition(Rarity));
		}

		AddAchievementProgress();
		InterfaceHelper.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(DungeonKey));
	}

	public override void RemoveItem(int amount = 1)
	{
		CollectionsHelper.RemoveItemFromCollection(this, User.Instance.DungeonKeys, amount);

		if (amount != 0)
		{
			(Application.Current.MainWindow as GameWindow).CreateFloatingTextUtility($"-{amount}", ColorsHelper.GetRarityColor(Rarity), FloatingTextHelper.GetDungeonKeyRarityPosition(Rarity));
		}

		InterfaceHelper.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(DungeonKey));
	}

	public static List<double> CreateRarityChancesList(int monsterHealth)
	{
		var dungeonKeyRarityChances = new List<double>();
		const double dungeonKeyRarity0Chance = 0.0048;
		const double dungeonKeyRarity1Chance = 0.0039;
		const double dungeonKeyRarity2Chance = 0.0024;
		const double dungeonKeyRarity3Chance = 0.0010;
		const double dungeonKeyRarity4Chance = 0.0003;
		const double dungeonKeyRarity5Chance = 0.0001;
		const double emptyLootChance = 1 - dungeonKeyRarity0Chance - dungeonKeyRarity1Chance - dungeonKeyRarity2Chance - dungeonKeyRarity3Chance - dungeonKeyRarity4Chance - dungeonKeyRarity5Chance;

		// Set drop rates in list for randomizing algorithm.
		// Note that index 0 is for empty loot, rarities start with index 1 up to 6.
		dungeonKeyRarityChances.Add(emptyLootChance);
		dungeonKeyRarityChances.Add(dungeonKeyRarity0Chance);
		dungeonKeyRarityChances.Add(dungeonKeyRarity1Chance);
		dungeonKeyRarityChances.Add(dungeonKeyRarity2Chance);
		dungeonKeyRarityChances.Add(dungeonKeyRarity3Chance);
		dungeonKeyRarityChances.Add(dungeonKeyRarity4Chance);
		dungeonKeyRarityChances.Add(dungeonKeyRarity5Chance);

		return dungeonKeyRarityChances;
	}
}