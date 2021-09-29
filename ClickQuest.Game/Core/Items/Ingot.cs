using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.Extensions.Interface;

namespace ClickQuest.Game.Core.Items
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

		public override void AddItem(int amount = 1)
		{
			CollectionsController.AddItemToCollection(this, User.Instance.Ingots);

			AddAchievementProgress();
			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}

		public override void RemoveItem(int amount = 1)
		{
			CollectionsController.RemoveItemFromCollection(this, User.Instance.Ingots);
		}
	}
}