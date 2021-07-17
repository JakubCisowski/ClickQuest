using ClickQuest.Extensions.CollectionsManager;
using ClickQuest.Player;

namespace ClickQuest.Items
{
	public partial class Material : Item
	{
		public Material() : base()
		{

		}

		public override Material CopyItem(int quantity)
		{
			Material copy = new Material();

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
			CollectionsController.AddItemToCollection<Material>(this, User.Instance.CurrentHero.Materials);

			this.AddAchievementProgress(1);
			Extensions.InterfaceManager.InterfaceController.RefreshEquipmentPanels();
		}
		
		public override void RemoveItem()
		{
			CollectionsController.RemoveItemFromCollection<Material>(this, User.Instance.CurrentHero.Materials);
		}
	}
}