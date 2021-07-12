using ClickQuest.Player;

namespace ClickQuest.Items
{
	public partial class Material : Item
	{
		public Material() : base()
		{

		}

		public override Material CopyItem()
		{
			Material copy = new Material();

			copy.Id = Id;
			copy.Name = Name;
			copy.Rarity = Rarity;
			copy.Value = Value;
			copy.Description = Description;
			copy.Quantity = 1;

			return copy;
		}

		public override void AddAcheievementProgress()
		{
			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.MaterialsGained, 1);
		}
	}
}