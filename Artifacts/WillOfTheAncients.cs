using ClickQuest.Extensions.CombatManager;
using ClickQuest.Items;
using ClickQuest.Player;
using static ClickQuest.Extensions.RandomnessManager.RandomnessController;

namespace ClickQuest.Artifacts
{
	// Your poison damage can now critically strike. The probability is calculated using your Critical Click Chance, and the damage using your Critical Click Damage.
	public class WillOfTheAncients : ArtifactFunctionality
	{
		public override void OnDealingPoisonDamage(ref int poisonDamage)
		{
			double critChance = User.Instance.CurrentHero.CritChance;
			double critDamage = User.Instance.CurrentHero.CritDamage;
			double randomizedValue = RNG.Next(1, 101) / 100d;

			if (randomizedValue <= critChance)
			{
				poisonDamage = (int) (poisonDamage * critDamage);
			}
		}

		public WillOfTheAncients()
		{
			Name = "Will of the Ancients";
		}
	}
}