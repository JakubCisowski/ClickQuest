using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.Core.Items;

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

	public override void AddItem(int amount = 1)
	{
		CollectionsController.AddItemToCollection(this, User.Instance.CurrentHero.Materials, amount);

		AddAchievementProgress();
		InterfaceController.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(Material));
	}

	public override void RemoveItem(int amount = 1)
	{
		CollectionsController.RemoveItemFromCollection(this, User.Instance.CurrentHero.Materials, amount);

		InterfaceController.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(Material));
	}
}