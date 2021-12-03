using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Items.Artifacts
{
	// Increases your Poison Damage by 10, and your Critical Click Damage by 20%.
	public class EnchantedKoboldPike : ArtifactFunctionality
	{
		private const int PoisonDamageIncrease = 10;
		private const double CritDamageIncrease = 0.20;

		public override void OnEquip()
		{
			User.Instance.CurrentHero.PoisonDamage += PoisonDamageIncrease;
			User.Instance.CurrentHero.CritDamage += CritDamageIncrease;
		}

		public override void OnUnequip()
		{
			User.Instance.CurrentHero.PoisonDamage -= PoisonDamageIncrease;
			User.Instance.CurrentHero.CritDamage -= CritDamageIncrease;
		}

		public EnchantedKoboldPike()
		{
			Name = "Enchanted Kobold Pike";
		}
	}
}