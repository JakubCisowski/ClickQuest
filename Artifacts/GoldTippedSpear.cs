using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// Convert your Poison Damage into Click Damage. For each 1 Poison Damage converted, gain 1 Click Damage.
	// Additionally, for each 10 Poison Damage converted, gain 1% Critical Click Damage.
	public class GoldTippedSpear : ArtifactFunctionality
	{
		private const int ClickDamagePerPoison = 1;
		private const int PoisonThresholdForCritDamageIncrease = 10;
		private const double CritDamagePerTenPoison = 0.01;

		private int _poisonDamageConverted;

		public override void OnEquip()
		{
			_poisonDamageConverted = User.Instance.CurrentHero.PoisonDamage;
			User.Instance.CurrentHero.PoisonDamage = 0;
			
			User.Instance.CurrentHero.ClickDamage += _poisonDamageConverted * ClickDamagePerPoison;
			User.Instance.CurrentHero.CritDamage += (int)(_poisonDamageConverted/PoisonThresholdForCritDamageIncrease) * CritDamagePerTenPoison;
		}

		public override void OnUnequip()
		{
			User.Instance.CurrentHero.ClickDamage -= _poisonDamageConverted * ClickDamagePerPoison;
			User.Instance.CurrentHero.CritDamage -= ((int)(_poisonDamageConverted/PoisonThresholdForCritDamageIncrease)) * CritDamagePerTenPoison;
			
			User.Instance.CurrentHero.PoisonDamage += _poisonDamageConverted;
			_poisonDamageConverted = 0;
		}

		public GoldTippedSpear()
		{
			Name = "Gold-Tipped Spear";
		}
	}
}