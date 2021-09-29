using ClickQuest.Items;

namespace ClickQuest.Artifacts
{
	// Your poison damage is increased by 20%.
	public class ToxicAmulet : ArtifactFunctionality
	{
		private const double PoisonDamageModifier = 1.2;

		public override void OnDealingPoisonDamage(ref int poisonDamage)
		{
			int bonusPoisonDamage = (int) (poisonDamage * PoisonDamageModifier);

			poisonDamage += bonusPoisonDamage;
		}

		public ToxicAmulet()
		{
			Name = "Toxic Amulet";
		}
	}
}