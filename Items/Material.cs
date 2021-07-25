using ClickQuest.Extensions.CollectionsManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Interfaces;
using ClickQuest.Player;

namespace ClickQuest.Items
{
	public class Material : Item, IMeltable
	{
		public int BaseIngotBonus
		{
			get
			{
				return 1;
			}
		}

		public override Material CopyItem(int quantity)
		{
			var copy = new Material();

			copy.Id = Id;
			copy.Name = Name;
			copy.Rarity = Rarity;
			copy.Value = Value;
			copy.Description = Description;
			copy.Quantity = quantity;

			return copy;
		}

		public override void AddAchievementProgress(int amount)
		{
			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.MaterialsGained, amount);
		}

		public override void AddItem()
		{
			CollectionsController.AddItemToCollection(this, User.Instance.CurrentHero.Materials);

			AddAchievementProgress(1);
			InterfaceController.RefreshEquipmentPanels();
		}

		public override void RemoveItem()
		{
			CollectionsController.RemoveItemFromCollection(this, User.Instance.CurrentHero.Materials);
		}
	}
}