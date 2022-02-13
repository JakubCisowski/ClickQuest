using ClickQuest.Game.Helpers;
using ClickQuest.Game.UserInterface.Helpers;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.Game.Models;

public class Material : Item
{
	public const int BaseMeltingIngotBonus = 10;

	public override Material CopyItem(int quantity)
	{
		var copy = new Material
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
		User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.MaterialsGained, amount);
	}

	public override void AddItem(int amount = 1, bool displayFloatingText = false)
	{
		CollectionsHelper.AddItemToCollection(this, User.Instance.CurrentHero.Materials, amount);

		if (displayFloatingText)
		{
			LootQueueHelper.AddToQueue(Name, Rarity, PackIconKind.Cog);
		}

		AddAchievementProgress();
		InterfaceHelper.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(Material));
	}

	public override void RemoveItem(int amount = 1)
	{
		CollectionsHelper.RemoveItemFromCollection(this, User.Instance.CurrentHero.Materials, amount);

		InterfaceHelper.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(Material));
	}
}