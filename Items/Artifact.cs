using ClickQuest.Extensions.CollectionsManager;
using ClickQuest.Player;
using ClickQuest.Interfaces;

namespace ClickQuest.Items
{
	public partial class Artifact : Item, IMeltable
	{
		public int BaseIngotBonus => 100;

		public Artifact() : base()
		{
			
		}

		public override Artifact CopyItem(int quantity)
		{
			Artifact copy = new Artifact();

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
			switch(this.Rarity)
			{
				case Rarity.General:
					achievementType=NumericAchievementType.GeneralArtifactsGained;
					break;
				case Rarity.Fine:
					achievementType=NumericAchievementType.FineArtifactsGained;
					break;
				case Rarity.Superior:
					achievementType=NumericAchievementType.SuperiorArtifactsGained;
					break;
				case Rarity.Exceptional:
					achievementType=NumericAchievementType.ExceptionalArtifactsGained;
					break;
				case Rarity.Mythic:
					achievementType=NumericAchievementType.MythicArtifactsGained;
					break;
				case Rarity.Masterwork:
					achievementType = NumericAchievementType.MasterworkArtifactsGained;
					break;
			}
			User.Instance.Achievements.IncreaseAchievementValue(achievementType, amount);
		}

		public override void AddItem()
		{
			CollectionsController.AddItemToCollection<Artifact>(this, User.Instance.CurrentHero.Artifacts);

			this.AddAchievementProgress(1);
			Extensions.InterfaceManager.InterfaceController.RefreshEquipmentPanels();
		}
		
		public override void RemoveItem()
		{
			CollectionsController.RemoveItemFromCollection<Material>(this, User.Instance.CurrentHero.Materials);
		}
	}
}