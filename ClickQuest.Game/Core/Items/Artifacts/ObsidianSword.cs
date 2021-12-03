using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Items.Artifacts
{
	// Increases your Click Damage by 20, and your Critical Click Damage by 20%.
	public class ObsidianSword : ArtifactFunctionality
	{
		private const int ClickDamageIncrease = 20;
		private const double CritDamageIncrease = 0.20;

		public override void OnEquip()
		{
			User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;
			User.Instance.CurrentHero.CritDamage += CritDamageIncrease;
		}

		public override void OnUnequip()
		{
			User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
			User.Instance.CurrentHero.CritDamage -= CritDamageIncrease;
		}

		public ObsidianSword()
		{
			Name = "Obsidian Sword";
		}
	}
}