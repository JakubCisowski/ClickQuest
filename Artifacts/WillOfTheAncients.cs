using ClickQuest.Extensions.CombatManager;
using ClickQuest.Items;
using ClickQuest.Player;
using static ClickQuest.Extensions.RandomnessManager.RandomnessController;

namespace ClickQuest.Artifacts
{
	// Your poison damage can now critically strike, dealing 200% of its normal damage. The probability is calculated using your Critical Click Chance.
	public class WillOfTheAncients : ArtifactFunctionality
	{
		private const double CritDamageModifier = 2.0;

		public override void OnDealingPoisonDamage(int poisonDamage)
		{
			double critChance = User.Instance.CurrentHero.CritChance;
			double randomizedValue = RNG.Next(1, 101) / 100d;

			if (randomizedValue <= critChance)
			{
				int bonusPoisonDamage = (int)(poisonDamage * CritDamageModifier - poisonDamage);
				
				CombatController.DealDamageToEnemy(bonusPoisonDamage, DamageType.Artifact);
			}
		}

		public WillOfTheAncients()
		{
			Name = "Will of the Ancients";
		}
	}
}