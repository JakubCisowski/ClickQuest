using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Items.Artifacts
{
	// Increases your Poison Damage by 1.
	public class TinyVenomSac : ArtifactFunctionality
	{
		private const int PoisonDamageIncrease = 1;

		public override void OnEquip()
		{
			User.Instance.CurrentHero.PoisonDamage += PoisonDamageIncrease;
		}

		public override void OnUnequip()
		{
			User.Instance.CurrentHero.PoisonDamage -= PoisonDamageIncrease;
		}

		public TinyVenomSac()
		{
			Name = "Tiny Venom Sac";
		}
	}
}