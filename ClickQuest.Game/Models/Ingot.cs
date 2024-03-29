using System.Windows;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Windows;

namespace ClickQuest.Game.Models;

public class Ingot : Item
{
	public override Ingot CopyItem(int quantity)
	{
		var copy = new Ingot
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

	public override void AddAchievementProgress(int amount = 1)
	{
		NumericAchievementType achievementType = 0;

		// Increase achievement amount.
		switch (Rarity)
		{
			case Rarity.General:
				achievementType = NumericAchievementType.GeneralIngotsEarned;
				break;
			case Rarity.Fine:
				achievementType = NumericAchievementType.FineIngotsEarned;
				break;
			case Rarity.Superior:
				achievementType = NumericAchievementType.SuperiorIngotsEarned;
				break;
			case Rarity.Exceptional:
				achievementType = NumericAchievementType.ExceptionalIngotsEarned;
				break;
			case Rarity.Mythic:
				achievementType = NumericAchievementType.MythicIngotsEarned;
				break;
			case Rarity.Masterwork:
				achievementType = NumericAchievementType.MasterworkIngotsEarned;
				break;
		}

		User.Instance.Achievements.IncreaseAchievementValue(achievementType, amount);
	}

	public override void AddItem(int amount = 1, bool displayFloatingText = true)
	{
		CollectionsHelper.AddItemToCollection(this, User.Instance.Ingots, amount);

		if (amount != 0)
		{
			(Application.Current.MainWindow as GameWindow)?.CreateFloatingTextUtility($"+{amount}", ColorsHelper.GetRarityColor(Rarity), FloatingTextHelper.GetIngotRarityPosition(Rarity));
		}

		AddAchievementProgress();
		InterfaceHelper.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(Ingot));
	}

	public override void RemoveItem(int amount = 1)
	{
		CollectionsHelper.RemoveItemFromCollection(this, User.Instance.Ingots, amount);

		if (amount != 0)
		{
			(Application.Current.MainWindow as GameWindow).CreateFloatingTextUtility($"-{amount}", ColorsHelper.GetRarityColor(Rarity), FloatingTextHelper.GetIngotRarityPosition(Rarity));
		}

		InterfaceHelper.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(Ingot));
	}
}