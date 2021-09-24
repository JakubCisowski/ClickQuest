using ClickQuest.Extensions.CombatManager;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// Your poison damage is increased by 20%.
	public class ToxicAmulet : ArtifactFunctionality
	{
		private const double PoisonDamageModifier = 1.2;
		
		public override void OnDealingPoisonDamage(ref int poisonDamage)
		{
			int bonusPoisonDamage = (int) (poisonDamage * PoisonDamageModifier);
			CombatController.DealDamageToEnemy(bonusPoisonDamage, DamageType.Artifact);
		}

		public ToxicAmulet()
		{
			Name = "Toxic Amulet";
		}
	}
}