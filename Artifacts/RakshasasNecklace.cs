using ClickQuest.Items;

namespace ClickQuest.Artifacts
{
	// Increases passive Aura Attack Speed by 50%.
	public class RakshasasNecklace : ArtifactFunctionality
	{
		private const double AuraAttackSpeedModifier = 1.5;

		public override void OnEquip()
		{
		}

		public override void OnUnequip()
		{
			base.OnUnequip();
		}

		public RakshasasNecklace()
		{
			Name = "Rakshasa's Necklace";
		}
	}
}