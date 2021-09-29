using System.Text.Json.Serialization;
using ClickQuest.Game.Core.Interfaces;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.Extensions.Interface;

namespace ClickQuest.Game.Core.Items
{
	public class Material : Item, IMeltable
	{
		[JsonIgnore]
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

		public override void AddAchievementProgress(int amount = 1)
		{
			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.MaterialsGained, amount);
		}

		public override void AddItem(int amount = 1)
		{
			CollectionsController.AddItemToCollection(this, User.Instance.CurrentHero.Materials);

			AddAchievementProgress();
			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}

		public override void RemoveItem(int amount = 1)
		{
			CollectionsController.RemoveItemFromCollection(this, User.Instance.CurrentHero.Materials);
		}
	}
}