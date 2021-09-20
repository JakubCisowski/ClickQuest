using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	public class MeatCleaver : ArtifactFunctionality
	{
		private const double CritDamageIncrease = 0.15;
		
		// In case this artifact hits the Crit Damage cap of 250% and only increases it by eg. 5%.
		private double _amountIncreased;
		
		public override void OnEquip()
		{
			if (User.Instance.CurrentHero.CritDamage + CritDamageIncrease > 2.5)
			{
				_amountIncreased = 2.5 - User.Instance.CurrentHero.CritDamage;
				User.Instance.CurrentHero.CritDamage += _amountIncreased;
			}
			else
			{
				User.Instance.CurrentHero.CritDamage += CritDamageIncrease;
				_amountIncreased = CritDamageIncrease;
			}
		}

		public override void OnUnequip()
		{
			User.Instance.CurrentHero.CritDamage -= _amountIncreased;
			_amountIncreased = 0;
		}

		public MeatCleaver()
		{
			Name = "Meat Cleaver";
		}
	}
}