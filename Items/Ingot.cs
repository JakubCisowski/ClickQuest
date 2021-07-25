using ClickQuest.Extensions.CollectionsManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Player;

namespace ClickQuest.Items
{
	public class Ingot : Item
	{
		public override Ingot CopyItem(int quantity)
		{
			var copy = new Ingot();

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

		public override void AddItem()
		{
			CollectionsController.AddItemToCollection(this, User.Instance.Ingots);

			AddAchievementProgress(1);
			InterfaceController.RefreshEquipmentPanels();
		}

		public override void RemoveItem()
		{
			CollectionsController.RemoveItemFromCollection(this, User.Instance.Ingots);
		}
	}
}