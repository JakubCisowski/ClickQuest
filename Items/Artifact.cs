using ClickQuest.Extensions.CollectionsManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Interfaces;
using ClickQuest.Player;

namespace ClickQuest.Items
{
	public class Artifact : Item, IMeltable
	{
		public int BaseIngotBonus
		{
			get { return 100; }
		}

		public override Artifact CopyItem(int quantity)
		{
			var copy = new Artifact();

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
					achievementType = NumericAchievementType.GeneralArtifactsGained;
					break;
				case Rarity.Fine:
					achievementType = NumericAchievementType.FineArtifactsGained;
					break;
				case Rarity.Superior:
					achievementType = NumericAchievementType.SuperiorArtifactsGained;
					break;
				case Rarity.Exceptional:
					achievementType = NumericAchievementType.ExceptionalArtifactsGained;
					break;
				case Rarity.Mythic:
					achievementType = NumericAchievementType.MythicArtifactsGained;
					break;
				case Rarity.Masterwork:
					achievementType = NumericAchievementType.MasterworkArtifactsGained;
					break;
			}

			User.Instance.Achievements.IncreaseAchievementValue(achievementType, amount);
		}

		public override void AddItem()
		{
			CollectionsController.AddItemToCollection(this, User.Instance.CurrentHero.Artifacts);

			AddAchievementProgress(1);
			InterfaceController.RefreshEquipmentPanels();
		}

		public override void RemoveItem()
		{
			CollectionsController.RemoveItemFromCollection(this, User.Instance.CurrentHero.Materials);
		}
	}
}