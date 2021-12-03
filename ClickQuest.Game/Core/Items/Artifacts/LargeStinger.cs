using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Items.Artifacts
{
	// Increases your Poison Damage by 5.
	public class LargeStinger : ArtifactFunctionality
	{
		private const int PoisonDamageIncrease = 5;

		public override void OnEquip()
		{
			User.Instance.CurrentHero.PoisonDamage += PoisonDamageIncrease;
		}

		public override void OnUnequip()
		{
			User.Instance.CurrentHero.PoisonDamage -= PoisonDamageIncrease;
		}

		public LargeStinger()
		{
			Name = "Large Stinger";
		}
	}
}