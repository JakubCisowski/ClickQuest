using ClickQuest.Items;

namespace ClickQuest.Artifacts
{
	// Your poison damage can now critically strike, dealing 200% of its normal damage. The probability is calculated using your Critical Click Chance.
	public class WillOfTheAncients : ArtifactFunctionality
	{
		private const double CritDamageModifier = 2.0;

		public override void OnDealingPoisonDamage(int poisonDamage)
		{
			base.OnDealingPoisonDamage(poisonDamage);
		}

		public WillOfTheAncients()
		{
			Name = "Will of the Ancients";
		}
	}
}