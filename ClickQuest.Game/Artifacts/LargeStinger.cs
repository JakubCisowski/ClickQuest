using ClickQuest.Game.Items;
using ClickQuest.Game.Player;

namespace ClickQuest.Game.Artifacts
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